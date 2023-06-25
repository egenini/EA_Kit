using EAUtils;
using Newtonsoft.Json.Linq;
using Persistencia.frw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UIResources;

namespace Persistencia.custom_table
{
    public class SincronizaColumnas
    {
        //const string HEADER_CELLS_BGCOLOR = "15453831";
        //const string DATA_SPECIFICATION = "<?xml version=\"1.0\" ?><customtable><table></table></customtable>";
        //const string META = "<?xml version=\"1.0\"?><dataformat><style><grid rows=\"2\" columns=\"8\"/><cells></cells><rows/></style><layout autosizecolumns=\"1\" version=\"1\" xmlns:dt=\"urn:schemas-microsoft-com:datatypes\" dt:dt=\"bin.base64\"> CAABAAAAAQAAAAEAAAABAAAAAQAAAAEAAAABAAAAAQAAAAgAKAAAACgAAAAoAAAAKAAAACgA AAAoAAAAKAAAACgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA =</layout></dataformat>";

        EAUtils.EAUtils eaUtils;
        EA.Element table;
        EA.Element customTable;
        EA.ElementGrid grid;

        public XmlDocument xmlDOMMeta = new XmlDocument();

        public SincronizaColumnas( EAUtils.EAUtils eaUtils, EA.Element table, EA.Element customTable )
        {
            this.table = table;
            this.eaUtils = eaUtils;
            this.customTable = customTable;
            this.grid = customTable.GetElementGrid();
        }

        public void sincornizar()
        {
            sincronizarColumnas();

            if( customTable.Name.ToLower().Contains("custom"))
            {
                customTable.Name = table.Name + "_test_data";
                customTable.Update();
            }

            Alert.Success( "Se han actualizado los datos" );
        }

        private void sincronizarColumnas()
        {

            string infoStr = this.eaUtils.taggedValuesUtils.get(table, Framework.FRAMEWORK_NAME + "::json", "").asString();

            if( infoStr != "")
            {
                dynamic data = JObject.Parse(infoStr);

                List<string> requeridos = new List<string>();

                if (grid.GetRowCount() == 0)
                {
                    grid.SetGridSize(1, data.attributes.Count + 2);

                    grid.SetCell(0, 0, "Modo");
                    grid.SetCell(0, 1, "Resultado esperado");

                    int column = 2;
                    
                    for( int i = 0; i < data.attributes.Count; i++ )
                    {
                        dynamic attr = data.attributes[i];

                        grid.SetCell(0, column++, StringUtils.toCamel(attr.name));

                        if( attr.required)
                        {
                            requeridos.Add(attr.name);
                        }
                    }

                    // que haga lo que tiene que hacer.
                    // esto implica que si los requeridos tienen un valor correcto entonces no va a fallar.
                    if( requeridos.Count != 0)
                    {

                    }
                    else
                    {
                        // sino no hay requeridos, podría ser 1 sólo dato el que satisfaga.

                    }
                    grid.Update();
                }
                
            }
        }
    }
}
