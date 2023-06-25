using EA;
using EAUtils.flow.ui;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UIResources.ui.flow;

namespace EAUtils.flow
{
    public class RecorreFlow
    {
        const string inicioQuery = "select \r\ntdo.object_id, to2.object_type\r\nfrom t_diagram td\r\njoin t_diagramobjects tdo on tdo.diagram_id = td.diagram_id\r\njoin t_object to2 on to2.object_id = tdo.object_id\r\njoin t_connector tc on tc.start_object_id = tdo.object_id\r\nwhere \r\ntd.ea_guid = '__ea_guid__'\r\nand\r\nto2.object_type in ('Event', 'StateNode')\r\nand\r\ntc.connector_type = 'ControlFlow'";
        const string cuantosSalientesTiene = "select end_object_id from t_connector where start_object_id = ";

        SiguienteActividadUI elegirSiguienteUI = null;

        bool esBpmn = false;
        List<object> posiblesSiguiente = new List<object>();
        List<string> posiblesSiguientesGuardas = new List<string>();

        List<EA.Diagram> pilaDiagramas = new List<Diagram>();

        public EAUtils eaUtils;
        public GestorTraza gestorTraza = new GestorTraza();

        public ElegirSiguiente elector;

        public bool modoInvisible = true;

        public EA.Element inicio = null;
        public RecorreFlow(EAUtils eaUtils)
        {
            this.eaUtils = eaUtils;            
        }

        public virtual void enElemento(EA.Element element)
        {
            // este método está para que sea sobreescrito y hacer algo con cada elemento.
        }

        public string infoUsuarioFlujo()
        {
            return infoUsuarioFlujo(null);
        }
        public string infoUsuarioFlujo( string guid )
        {
            string nombreUsuario = Usuario.usuarioNombreOAnonimous(this.eaUtils);
            string info = "";

            if ( guid != null && guid != "" )
            {
                this.eaUtils.taggedValuesUtils.set(this.inicio, nombreUsuario, guid);
            }
            else
            {
                info = this.eaUtils.taggedValuesUtils.get(this.inicio, nombreUsuario, "").asString();

                if (info != null)
                {
                    this.eaUtils.taggedValuesUtils.delete(this.inicio, nombreUsuario);
                }
            }

            return info;

        }
        public void comenzar()
        {
            pilaDiagramas.Clear();

            try
            {
                // buscar donde empezar
                EA.Element inicio = buscarInicio();

                if (inicio != null)
                {
                    this.inicio = inicio;

                    string guid = infoUsuarioFlujo();

                    this.esBpmn = inicio.FQStereotype.Contains("BPMN");

                    if( guid != "")
                    {
                        Element retoma = this.eaUtils.repository.GetElementByGuid(guid);

                        if( retoma != null)
                        {
                            inicio = retoma;
                        }
                    }

                    recorrer(inicio);

                    this.eaUtils.printOut(gestorTraza.asString());
                }

                this.eaUtils.printOut("Fin");
            }
            catch (Exception e)
            {
                this.eaUtils.printOut(e.ToString());
                //Clipboard.SetText(e.ToString());
            }
            finally
            {
                if( this.pilaDiagramas.Count != 0 )
                {
                    pilaDiagramas.Last().FilterElements = "";
                }
            }
        }
        private bool recorrer(EA.Element actual)
        {
            EA.Element siguiente;
            EA.Element gateway;

            if (actual != null)
            {
                // si el destino es un "Decision" hay que mostrar el element -actividad actual-, el "Decision" y por cada guarda mostrar a dónde podría ir
                // sí a dónde podría ir es otro "Decision" saltearlo y mostrar la siguiente actividad

                gestorTraza.agregar(actual);

                enElemento(actual);

                this.pilaDiagramas.Last().FilterElements = "" + actual.ElementID;

                if (actual.CompositeDiagram != null)
                {
                    EA.Element inicio = buscarInicioEnDiagrama(actual.CompositeDiagram);

                    if (inicio != null)
                    {
                        gestorTraza.abrirSubtraza();
                        recorrer(inicio);
                        gestorTraza.cerrarSubtraza();

                        this.eaUtils.repository.CloseDiagram(this.pilaDiagramas.Last().DiagramID);

                        this.pilaDiagramas.Remove(this.pilaDiagramas.Last());

                        this.eaUtils.repository.ActivateDiagram(this.pilaDiagramas.Last().DiagramID);
                    }
                }

                foreach (Connector connector in actual.Connectors)
                {
                    if (connector.Type == "ControlFlow" && connector.SupplierID != actual.ElementID)
                    {
                        siguiente = this.eaUtils.repository.GetElementByID(connector.SupplierID);

                        if (siguiente.Type == "Decision")
                        {
                            // aca hay que armar la pantalla para que decida a dónde va
                            gateway = siguiente;

                            if (esUnGatewayIncusivo(gateway))
                            {
                                foreach( Connector connector1 in gateway.Connectors)
                                {
                                    // solo va a tener 1 sólo conector de salida, el resto no va a cumplir la condición.
                                    if( connector1.SupplierID != gateway.ElementID)
                                    {
                                        if( ! recorrer(this.eaUtils.repository.GetElementByID(connector1.SupplierID)))
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                posiblesSiguiente.Clear();
                                posiblesSiguientesGuardas.Clear();

                                foreach (Connector connector1 in gateway.Connectors)
                                {
                                    if (connector1.SupplierID != gateway.ElementID)
                                    {
                                        siguiente = this.eaUtils.repository.GetElementByID(connector1.SupplierID);

                                        if (siguiente.Type == "Decision")
                                        {
                                            if (esUnGatewayIncusivo(siguiente))
                                            {
                                                agregarGuarda(connector1);

                                                foreach(Connector connector2 in siguiente.Connectors)
                                                {
                                                    if( connector2.SupplierID != siguiente.ElementID)
                                                    {
                                                        posiblesSiguiente.Add(this.eaUtils.repository.GetElementByID(connector2.SupplierID));
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                agregarGuarda(connector1);
                                                posiblesSiguiente.Add(siguiente);
                                            }
                                        }
                                        else
                                        {
                                            agregarGuarda(connector1);
                                            posiblesSiguiente.Add(siguiente);
                                        }
                                    }
                                }

                                recorrer(elegirSiguiente(actual, gateway));
                            }
                        }
                        /*
                        else if (esFinal(siguiente))
                            break;
                        */
                        else
                        {
                            recorrer(siguiente);
                        }
                    }
                }
            }
            return actual != null;
        }

        private EA.Element buscarInicio()
        {
            EA.Diagram diagram = this.eaUtils.repository.GetCurrentDiagram();

            EA.Element inicio = buscarInicioEnDiagrama(diagram);            

            return inicio;
        }

        private EA.Element buscarInicioEnDiagrama(EA.Diagram diagram)
        {
            EA.Element inicio = null;

            DbUtils dbUtils = new DbUtils(this.eaUtils.repository);

            if (dbUtils.execute(inicioQuery.Replace("__ea_guid__", diagram.DiagramGUID)))
            {
                List<List<string>> results = dbUtils.asList();

                // tomo el primero de prepo
                inicio = this.eaUtils.repository.GetElementByID(int.Parse(results[0][0]));

                this.pilaDiagramas.Add(diagram);

                this.eaUtils.repository.OpenDiagram(diagram.DiagramID);
                this.eaUtils.repository.ActivateDiagram(diagram.DiagramID);
            }
            return inicio;
        }

        private EA.Element elegirSiguiente(EA.Element actual, EA.Element gateway)
        {
            ManualResetEvent mre = new ManualResetEvent(false);

            this.elector.elegir(actual, posiblesSiguientesGuardas, posiblesSiguiente, mre);            

            mre.WaitOne();  // This will wait

            this.eaUtils.repository.ActivateDiagram(this.pilaDiagramas.Last().DiagramID);

            int elegido = this.elector.elegido();

            EA.Element siguiente = null;

            if ( elegido > -1)
            {
                siguiente = (EA.Element)this.posiblesSiguiente[elegido];
            }
            else if (elegido == -10)
            {
                // esto es porqque se presionó el botón de suspender.
                infoUsuarioFlujo(actual.ElementGUID);
            }
            return siguiente;
        }

        private bool esUnGatewayIncusivo(EA.Element gateway)
        {
            bool esInclusivo = false;

            DbUtils dbUtils = new DbUtils(this.eaUtils.repository);

            if (dbUtils.execute(cuantosSalientesTiene + gateway.ElementID))
            {
                List<List<string>> results = dbUtils.asList();

                // tomo el primero de prepo
                esInclusivo = results.Count == 1;
            }
            return esInclusivo;
        }

        private void agregarGuarda(EA.Connector connector)
        {
            string guarda = connector.TransitionGuard;

            if ( this.esBpmn)
            {
                guarda = this.eaUtils.taggedValuesUtils.get(connector, "conditionExpression", "Defaul").asString();

                this.posiblesSiguientesGuardas.Add(guarda == "" ? "Default" : guarda);               
            }
            else
            {
                this.posiblesSiguientesGuardas.Add(guarda == "" ? "Default" : guarda);
            }
        }
    }
}
