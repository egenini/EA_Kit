using EA;
using EAUtils.framework2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIResources;
using static EAUtils.ConnectorUtils;

namespace RestFul.tbf
{
    public class Generator
    {
        Element element;
        Framework frameworkInstance;
        EAUtils.EAUtils eaUtils;
        Info info = new Info();
        Element classElement;
        Element tableElement;

        public void build(EA.Element element, Framework frameworkInstance, EAUtils.EAUtils eaUtils)
        {
            this.eaUtils           = eaUtils;
            this.frameworkInstance = frameworkInstance;
            this.element           = element;

            this.doIt();
        }

        private bool doIt()
        {
            bool canDo = true;

            // element es el recurso, puede estar en cualquier parte del árbol.
            // por lo tanto hay que llegar hasta el primer nodo que es la URI.
            // de la uri tomamos el valor etiquetado basePath.
            // a medida que vamos subiendo vamos armando el path.
            // del recurso buscamos la relación de agregación que el recurso tiene como destino.
            // ese agregado es la clase del dominio que tiene una relación de realización con una tabla, donde la tabla realiza la clase.
            // de la clase tomamos el nombre y de la tabla el dao.

            string[] nameSplitter = this.element.Name.Split('/');

            info.ClassName = EAUtils.StringUtils.toPascal(nameSplitter[nameSplitter.Length-1]);

            // recorre el árbol hasta la URI
            info.inversePath.Add(this.element.Name);

            walkToRoot(this.element);

            // buscamos la clase y desde ahí vamos a la tabla.
            findPersistence();

            // si lo genera lo pega al portapapeles.
            if (this.frameworkInstance.generate(info, this.element))
            {
                canDo = true;
            }

            return canDo;
        }

        private void findPersistence()
        {
            List<ElementConnectorInfo> elementsConnectors = this.eaUtils.connectorUtils.get(this.element, EAUtils.ConnectorUtils.CONNECTOR__AGGREGATION, null, "Class", null, true, null);

            if (elementsConnectors.Count != 0)
            {
                this.classElement = elementsConnectors[0].element;

                List<ElementConnectorInfo> tableConnector = this.eaUtils.connectorUtils.get(this.classElement, EAUtils.ConnectorUtils.CONNECTOR__REALISATION, null, "Class", "table", true, null);

                if( tableConnector.Count != 0)
                {
                    this.tableElement = tableConnector[0].element;
                }
            }
        }

        private void walkToRoot(Element element)
        {
            // todos están ligados por una relación de asociacion con estereotipo ApiRestConnector.
            List<ElementConnectorInfo> elementsConnectors = this.eaUtils.connectorUtils.get(element, EAUtils.ConnectorUtils.CONNECTOR__ASSOCIATION, "APIRestConnector", "Class", "Resource", true, null);

            if (elementsConnectors.Count != 0)
            {
                foreach (ElementConnectorInfo elementConnectorInfo in elementsConnectors)
                {
                    if( element.ElementID != elementConnectorInfo.element.ElementID)
                    {
                        info.inversePath.Add(elementConnectorInfo.element.Name);

                        walkToRoot(elementConnectorInfo.element);
                    }
                }
            }
            else
            {
                // si no hay más Resource hacia arriba en el árbol entonces buscamos la URI.
                elementsConnectors = this.eaUtils.connectorUtils.get(element, EAUtils.ConnectorUtils.CONNECTOR__ASSOCIATION, "APIRestConnector", "Class", "URI", true, null);
                if (elementsConnectors.Count != 0)
                {
                    // debería haber 1 sólo así me tomo el primero.
                    // https://localhost:8080/ivr/encuesta/
                    Uri uri = new Uri(elementsConnectors[0].element.Name);

                    string basePath = uri.AbsolutePath.Replace("%7B", "{").Replace("%7D", "}");

                    info.inversePath.Add(basePath);

                    this.eaUtils.taggedValuesUtils.set(elementsConnectors[0].element, "schema", uri.Scheme);
                    this.eaUtils.taggedValuesUtils.set(elementsConnectors[0].element, "host", uri.Host);
                    this.eaUtils.taggedValuesUtils.set(elementsConnectors[0].element, "basePath", basePath);
                }
            }
        }
    }

    class Info : JsonableCommon, Jsonable
    {
        public List<string> inversePath = new List<string>();

        public string ClassName;

        public string stringfity()
        {
            this.start();
            // recorrer hacia atrás la lista para armar el path.
            inversePath.Reverse();
            
            string path = "";
            foreach( string partialPath in inversePath)
            {
                path += partialPath;
            }

            writer.WritePropertyName("path");
            writer.WriteValue(path);

            writer.WritePropertyName("ClassName");
            writer.WriteValue(ClassName);

            return this.end();
        }
    }
}
