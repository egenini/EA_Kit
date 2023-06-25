using EA;
using EAUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Model2Table
{
    public class FrameworkManager
    {
        const string DIAGRAM_MAPPING_QUERY = @"select tdo.rectleft, tdo.recttop, o.pdata1 diagram_id from t_diagramobjects tdo join t_object o on o.object_id = tdo.object_id where tdo.diagram_id = {{diagram_id}} order by tdo.recttop desc, tdo.rectleft asc";
        const string PACKAGE_GUID = "{06343F42-A8A9-4674-8813-9749B55C2CB8}";

        Dictionary<int, Diagram> diagramasConectados = new Dictionary<int, Diagram>();
        public Dictionary<int, Framework> diagramaFramework = new Dictionary<int, Framework>();
        
        Framework defaultFramework = null;
        EAUtils.EAUtils eaUtils;
        public FrameworkManager(EAUtils.EAUtils eaUtils) 
        { 
            this.eaUtils = eaUtils;

            EA.Package package = this.eaUtils.repository.GetPackageByGuid(PACKAGE_GUID);
            EA.Package mappingPackage = null;

            foreach (Package p in package.Packages)
            {
                if (p.Element.Tag == "mapping")
                {
                    mappingPackage = p;
                }
                else if (p.Element.Tag == "options")
                {
                    defaultFramework = new Framework(eaUtils, p );
                    defaultFramework.diagramasConectados = this.diagramasConectados;
                }
            }

            this.loadDiagramasConectados(mappingPackage);
        }

        internal void loadDiagramasConectados(Package p)
        {
            this.diagramasConectados.Clear();
            this.diagramaFramework.Clear();

            Dictionary<int, Framework> paqueteFramework = new Dictionary<int, Framework>();

            int diagramaModeloId;
            int diagramaTablaId;
            int diagramaPaqueteOptions;

            foreach (EA.Diagram d in p.Diagrams)
            {
                ResultAsList r = (ResultAsList)new SQLUtils(eaUtils).excecute( DIAGRAM_MAPPING_QUERY.Replace( "{{diagram_id}}", d.DiagramID.ToString() ));

                if (r.data.Count > 0)
                {
                    int col   = 0;
                    int index = 0;

                    while( index < r.data.Count )
                    {
                        try
                        { 
                            diagramaModeloId = int.Parse((string)r.data[ index     ][ 2 ]);
                            diagramaTablaId  = int.Parse((string)r.data[ index + 1 ][ 2 ]);

                            diagramasConectados.Add( diagramaModeloId, this.eaUtils.repository.GetDiagramByID( diagramaTablaId ) );

                            // tomamos el left de la row del diagrama de la tabla y vemos si la siguiente row tiene un valor meyor, entonces...
                            // significa que hay otro diagrama que es el de options, sino pasamos a la próxima row.

                            col = int.Parse((string)r.data[ index + 1 ][ 0 ]);
                                
                            if ( ( index + 2 ) < r.data.Count)
                            {
                                if( col < int.Parse((string)r.data[ index + 2 ][ 0 ] ) )
                                {
                                    diagramaPaqueteOptions = int.Parse((string)r.data[ index + 2 ][ 2 ]);
                                    
                                    diagramaFramework.Add( diagramaModeloId, new Framework( this.eaUtils,
                                        eaUtils.repository.GetPackageByID( eaUtils.repository.GetDiagramByID( diagramaPaqueteOptions ).PackageID ) ) );

                                    index = index + 3;
                                }
                                else
                                {
                                    diagramaFramework.Add( diagramaModeloId, this.defaultFramework );
                                    index = index + 2;
                                }
                            }
                            else
                            {
                                diagramaFramework.Add(diagramaModeloId, this.defaultFramework);
                                index = r.data.Count;
                            }                            
                        }
                        catch (Exception)
                        {
                            this.eaUtils.printOut("El elemento " + (index) + " de la columna A no se puede repetir,");
                            this.eaUtils.printOut(" remuevalo o cambielo por otro");
                        }
                    }
                }
            }
        }
        public bool isSourceDiagram(EA.Diagram diagram)
        {
            return diagram != null && diagramasConectados.ContainsKey(diagram.DiagramID);
        }
        public Diagram targetDiagram(Diagram diagram)
        {
            Diagram d = null;

            if (diagramasConectados.ContainsKey(diagram.DiagramID))
            {
                d = diagramasConectados[diagram.DiagramID];
            }

            return d;
        }

    }
}
