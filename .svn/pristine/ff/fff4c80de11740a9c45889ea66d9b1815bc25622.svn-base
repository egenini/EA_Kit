using EA;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UIResources;

namespace RestFul
{
    class GenerarDesdeResumen
    {
        EAUtils.EAUtils eautils;
        Package definicionPackage = null;
        Element uri = null;
        Element servicio;

        public GenerarDesdeResumen( EA.Repository repository )
        {
            eautils = new EAUtils.EAUtils(repository);
        }

        public void generar( Package packageResumen )
        {
            // buscamos paquete Definicion.
            Package parent = eautils.repository.GetPackageByID(packageResumen.ParentID);
            foreach( Package currentPackage in parent.Packages)
            {
                if( currentPackage.Name == "Definicion" || currentPackage.Name == "Definición" || currentPackage.Name == "Definition")
                {
                    definicionPackage = currentPackage;
                    break;
                }
            }
            if ( definicionPackage == null )
            {
                MessageBox.Show("No se encuentra el paquete donde generar el servicio");
            }
            else
            {
                foreach (Element current in packageResumen.Elements )
                {
                    if( current.Stereotype == "URI" )
                    {
                        uri = current;
                        break;
                    }
                }

                if( uri == null)
                {
                    Alert.Error(Properties.Resources.error_uri_no_encontrado);
                    //MessageBox.Show("No se ha encontrado ningún elemento con estereotipo URI");
                }
                else
                {
                    // movemos el elemento al paquete de definicion.
                    //uri.PackageID = definicionPackage.PackageID;
                    //uri.Update();

                    generar();
                }
            }
        }
        private void generar()
        {
            // crear un servicio
            crearServicio();
            analizarArbol(uri);

            Alert.Success(Properties.Resources.servicio_generado);
        }

        private void analizarArbol(Element branch)
        {
            List<ArrayList> elementsAndConnectors = eautils.connectorUtils.getFromConnectorFilter(false, branch, EAUtils.ConnectorUtils.CONNECTOR__ASSOCIATION, null, null, "Recurso");

            Element currentElement;
            foreach (var elemenentAndConnector in elementsAndConnectors)
            {
                currentElement = (Element)elemenentAndConnector[0];

                if( currentElement.Stereotype == "Recurso" )
                {
                    // crear interfaz y metodos
                    crearInterfaz(currentElement);

                    analizarArbol(currentElement);
                }
            }
        }

        private void crearInterfaz(Element recurso)
        {
            string parametro = null;
            if(recurso.Name.IndexOf("{") != -1)
            {
                try
                {
                    parametro = EAUtils.StringUtils.wordBetween(recurso.Name, '{', '}');
                }
                catch(Exception e)
                {
                    eautils.printOut(e.ToString());
                }
            }

            List<string> nombres = new List<string>();
            nombreCompleto(recurso, nombres);
            nombres.Reverse();
            string alias = String.Join("", nombres.ToArray());

            string name = EAUtils.StringUtils.toPascal(alias.Replace('/', '_').Replace('{','_').Replace('}','_'));

            Element interfaz = this.definicionPackage.Elements.AddNew( name, "Interface");
            interfaz.Alias = alias ;

            interfaz.Update();
            this.definicionPackage.Elements.Refresh();

            // agregar metodos
            agregarMetodos(recurso, interfaz, parametro);
        }

        private void agregarMetodos(Element recurso, Element interfaz, string parametro)
        {
            this.agregarMetodo(recurso, interfaz, "get", parametro);
            this.agregarMetodo(recurso, interfaz, "delete", parametro);
            this.agregarMetodo(recurso, interfaz, "post", parametro);
            this.agregarMetodo(recurso, interfaz, "put", parametro);
            this.agregarMetodo(recurso, interfaz, "options", parametro);
            this.agregarMetodo(recurso, interfaz, "patch", parametro);
        }

        private void agregarMetodo(Element recurso, Element interfaz, string nombreMetodo, string parametro)
        {
            List<ArrayList> elementsAndConnectors;
            EA.Method metodo;

            elementsAndConnectors = eautils.connectorUtils.getFromConnectorFilter(false, recurso, EAUtils.ConnectorUtils.CONNECTOR__ASSOCIATION, null, null, nombreMetodo.ToUpper());

            if (elementsAndConnectors.Count() != 0)
            {
                metodo       = interfaz.Methods.AddNew(nombreMetodo, "");
                metodo.Alias = nombreMetodo + interfaz.Name;
                metodo.Update();

                if(parametro != null && parametro.Length != 0)
                {
                    EA.Parameter param = metodo.Parameters.AddNew(parametro, "string");

                    param.Update();

                    if ( nombreMetodo == "get")
                    {
                        eautils.taggedValuesUtils.set(param, "in", "query");
                        eautils.taggedValuesUtils.set(param, "required", "true");
                    }
                }
            }

        }
        private void nombreCompleto(Element recurso, List<string> nombreCompleto)
        {
            nombreCompleto.Add(recurso.Name);

            List<ArrayList> elementsAndConnectors = eautils.connectorUtils.getFromConnectorFilter(true, recurso, EAUtils.ConnectorUtils.CONNECTOR__ASSOCIATION, null, null, "Recurso");

            Element currentElement;
            foreach (var elemenentAndConnector in elementsAndConnectors)
            {
                currentElement = (Element)elemenentAndConnector[0];

                if (currentElement.Stereotype == "Recurso")
                {
                    this.nombreCompleto(currentElement, nombreCompleto);
                }
            }

        }
        private void crearServicio()
        {
            string[] uriSplitted = uri.Name.Split('/');
            string uriPart;
            int numb;
            string nombreServicio = "anon";

            for( int i = uriSplitted.Count()-1; i >= 0; i--)
            {
                uriPart = uriSplitted[i];

                if(uriPart.Substring(0,1).ToLower() == "v" && int.TryParse(uriPart.Substring(1), out numb ) )
                {
                    nombreServicio = EAUtils.StringUtils.toPascal(uriSplitted[i - 1]);
                    break;
                }
            }

            this.servicio = this.definicionPackage.Elements.AddNew(nombreServicio, "Component");
            this.servicio.Stereotype = "Servicio";

            this.servicio.Update();
            this.definicionPackage.Elements.Refresh();
        }
    }
}
