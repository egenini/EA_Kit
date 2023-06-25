using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UIResources;

namespace RestFul
{
    class ResumenValidador
    {
        private Repository repository;

        public ResumenValidador( Repository repository)
        {
            this.repository = repository;
        }

        public Boolean validarConector( int origenId, int destinoId)
        {
            Boolean esValido = true;

            // este conector solo se puede usar desde uri a recursos o desde recursos a recurso o método.
            Element origen;
            Element destino;

            origen = this.repository.GetElementByID(origenId);
            destino = this.repository.GetElementByID(destinoId);

            //MessageBox.Show("conector origen "+ origen.Name +" destino "+ destino.Name);

            if (origen.Stereotype == "URI" && ! ( destino.Stereotype == "Recurso" || destino.Stereotype == "Resource") )
            {
                Alert.Info( Properties.Resources.conector_eliminado );
                esValido = false;
            }
            else if( ( origen.Stereotype == "Recurso" || origen.Stereotype == "Resource") && ! ( 
                ( destino.Stereotype == "Recurso" || destino.Stereotype == "Resource" )
                || destino.Stereotype == "GET"
                || destino.Stereotype == "POST"
                || destino.Stereotype == "PUT"
                || destino.Stereotype == "DELETE"
                || destino.Stereotype == "OPTIONS"
                || destino.Stereotype == "HEAD"
                ) )
            {
                esValido = false;
            }
            else if ( origen.Stereotype == "GET"
                || origen.Stereotype == "POST"
                || origen.Stereotype == "PUT"
                || origen.Stereotype == "DELETE"
                || origen.Stereotype == "OPTIONS"
                || origen.Stereotype == "HEAD"
                )
            {
                esValido = false;
            }
            return esValido;
        }

        public void validarMetodo( Element metodoAgregado )
        {
            Element recursoDelMetodo = null;
            metodoAgregado.Connectors.Refresh();

            //MessageBox.Show("Conectores del elemento: " + metodoAgregado.Connectors.Count);

            foreach (Connector currentConnector in metodoAgregado.Connectors)
            {
                if(currentConnector.Stereotype == "APIRestConnector" && currentConnector.ClientID == metodoAgregado.ElementID )
                {
                    recursoDelMetodo = this.repository.GetElementByID(currentConnector.SupplierID );

                    if ( recursoDelMetodo.Stereotype == "Recurso" || recursoDelMetodo.Stereotype == "Resource")
                    {
                        break;
                    }
                    else
                    {
                        recursoDelMetodo = null;
                    }
                }
            }
            Element currentMethod;
            Element parent;
            Element child;
            Package package;
            if (recursoDelMetodo != null)
            {
                foreach (Connector currentConnector in recursoDelMetodo.Connectors)
                {
                    currentMethod = this.repository.GetElementByID(currentConnector.ClientID );

                    if( currentMethod.Stereotype == metodoAgregado.Stereotype && currentMethod.ElementID != metodoAgregado.ElementID )
                    {
                        if(metodoAgregado.ParentID != 0)
                        {
                            parent = this.repository.GetElementByID(metodoAgregado.ParentID);
                            for(short i = 0; i < parent.Elements.Count; i++ )
                            {
                                child = this.repository.GetElementByID( int.Parse((string)parent.Elements.GetAt(i)));
                                if(child.ElementID == metodoAgregado.ElementID )
                                {
                                    parent.Elements.DeleteAt(i, true);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            package = this.repository.GetPackageByID(metodoAgregado.PackageID);
                            for (short i = 0; i < package.Elements.Count; i++)
                            {
                                child = this.repository.GetElementByID(int.Parse((string)package.Elements.GetAt(i)));
                                if (child.ElementID == metodoAgregado.ElementID)
                                {
                                    package.Elements.DeleteAt(i, true);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
