using EA;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EAUtils
{
    /// <summary>
    /// Completa la clase con todos los atributos, sea que estos esten difinidos por relaciones
    /// tales como asociación, generalizacion y/o realización. No hace update!.
    /// </summary>
    public class ModelClass2RealClass
    {
        EAUtils eaUtils;
        public RealClass realClass = null;
        // TODO : parametrizar este maxLevel. Podría ser por otro parámetro como por ej: "nivel de paquete" sería algo así como que todos los elementos estén bajo este paquete "padre".
        public short maxLevel = 105;
        private short currentLevel = 0;
        private Dictionary<int, RealClass> realClasses = new Dictionary<int, RealClass>();
        private Dictionary<int, ConnectorInfo> connectorsById = new Dictionary<int, ConnectorInfo>();

        public ModelClass2RealClass(EAUtils eaUtils)
        {
            this.eaUtils = eaUtils;
        }

        public void go( Element element )
        {
            try
            {
                realClass = build(element);
            }
            catch (Exception e) { Clipboard.SetText(e.ToString()); }
        }

        private RealClass build( Element element )
        {
            RealClass realClass = null;

            if (realClasses.ContainsKey(element.ElementID))
            {
                realClass = realClasses[element.ElementID];
            }
            else
            {
                realClass = new RealClass();

                realClass.element = element;

                addAttributes(element, realClass);

                addFromInheritanceAndImplements(element, realClass);

                addInnerClass(element, realClass);
            }

            return realClass;
        }

        private void addInnerClass(Element element, RealClass realClass)
        {
            RealClass innerClass;

            // por elementos contenidos
            foreach( Element inner in element.Elements)
            {
                if( inner.Type == "Class")
                {
                    innerClass = new RealClass();
                    innerClass.element = inner;

                    realClass.innerClasses.Add(innerClass);

                    addAttributes(inner, innerClass);
                }
            }

            // por relación de nesting
            List<ArrayList> elementsAndConnectors = eaUtils.connectorUtils.getFromConnectorFilter(element, ConnectorUtils.CONNECTOR__NESTING, null, "Class", null);
            Element relatedElement;
            Connector connector;

            foreach (ArrayList line in elementsAndConnectors)
            {
                relatedElement = (Element)line[EAUtils.ELEMENT_AND_CONNECTOR__ELEMENT_POSITION];
                connector = (Connector)line[EAUtils.ELEMENT_AND_CONNECTOR__CONNECTOR_POSITION];

                innerClass = new RealClass();
                innerClass.element = relatedElement;

                realClass.innerClasses.Add(innerClass);

                addAttributes(relatedElement, innerClass);
            }

        }

        private void addFromInheritanceAndImplements(Element element, RealClass realClass)
        {
            foreach( Element currentElement in element.BaseClasses)
            {
                addAttributes(currentElement, realClass);

                addFromInheritanceAndImplements(currentElement, realClass);
            }

            foreach (Element currentElement in element.Realizes)
            {
                addAttributes(currentElement, realClass);

                addFromInheritanceAndImplements(currentElement, realClass);
            }
        }

        private void addAttributes( Element element, RealClass realClass)
        {
            // atributos del elemento y atributos por asociaciones

            addAttributes(element.Name, element.Attributes, realClass);

            addByAssociation(element, ConnectorUtils.CONNECTOR__ASSOCIATION, realClass);
            addByAssociation(element, ConnectorUtils.CONNECTOR__AGGREGATION, realClass);

        }

        /// <summary>
        /// Crea un atributo con el tipo de la clase y el nombre de la relación o de la clase si no lo tuviera.
        /// </summary>
        /// <param name="element"></param>
        private void addByAssociation(Element element, string connectorType, RealClass realClass)
        {
            List<ConnectorUtils.ElementConnectorInfo> elementsAndConnectors = null;
            // situaciones:
            // si la dirección no está espeficada o es bi entonces tomo el destino porque si la otra clase aparece entonces agrega a esta como atributo.
            // si la dirección es origen destino entonces idem anterior.
            // si la dirección es destino origen entonces no se debe incluir a esta clase como atributo.

            if( connectorType == ConnectorUtils.CONNECTOR__ASSOCIATION)
            {
                elementsAndConnectors = eaUtils.connectorUtils.get(element, connectorType, null, "Class", null, null, ConnectorUtils.DIRECTION_FILTER__NOT_DESTINATION_SOURCE);
            }
            else if (connectorType == ConnectorUtils.CONNECTOR__AGGREGATION)
            {
                elementsAndConnectors = eaUtils.connectorUtils.get(element, connectorType, null, "Class", null, true, null);
            }

            Element relatedElement;
            Connector       connector;

            foreach (ConnectorUtils.ElementConnectorInfo info in elementsAndConnectors)
            {
                relatedElement = info.element;
                connector      = info.connector;

                if( relatedElement == null)
                {
                    continue;
                }

                // controlar que si elemento relacionado es el mismo entonces ambos extremos de la relación deberían ser el mismo elemento.
                if( relatedElement.ElementID == element.ElementID && ( connector.ClientID != element.ElementID || connector.SupplierID != element.ElementID)){
                    continue;
                }

                ConnectorInfo connectorInfo = new ConnectorInfo(connector);

                // controlar que no se produzca un bucle sin fin.
                // obtener el extremo según sea la dirección de la relación que se debe analizar.
                int elementId = 0;
                ConnectorEnd connectorEnd = null;

                if (connector.Direction == ConnectorUtils.DIRECTION__SOURCE_DESTINATION)
                {
                    // en este caso no hay posibilidades de bucle sin fin.
                    elementId    = connector.SupplierID;
                    connectorEnd = connector.SupplierEnd;

                    connectorInfo.supplierId = elementId;
                    connectorInfo.clientId   = connector.ClientID;

                }
                else
                {
                    // dado que se descarta destino origen acá entran las bi (implícitas o explícitas)
                    // hay que buscar el extremo que no se hubiera tratado aún.
                    // si no se trató ningún extremo empezamos por el supplier.

                    if ( ! connectorsById.ContainsKey(connector.ConnectorID))
                    {
                        elementId = connector.SupplierID;
                        connectorEnd = connector.SupplierEnd;

                        connectorInfo.supplierId = elementId;
                    }
                    else
                    {
                        connectorInfo = connectorsById[connector.ConnectorID];
                        elementId = connector.ClientID;
                        connectorEnd = connector.ClientEnd;
                        connectorInfo.clientId = connector.ClientID;
                    }
                }

                if ( connectorsById.ContainsKey(connector.ConnectorID))
                {
                    connectorInfo = connectorsById[connector.ConnectorID];

                    if (connectorInfo.isComplete )
                    {
                        continue;
                    }
                }
                else
                {
                    connectorsById.Add(connector.ConnectorID, connectorInfo);
                }

                connectorInfo.lookForComplete();

                ConnectorUtils.Cardinality cardinality = eaUtils.connectorUtils.getCardinality(connectorEnd);

                if (connectorEnd.Role.Length == 0)
                {
                    realClass.add(new AttributeInfo(relatedElement.Name, relatedElement.Name, this.build(relatedElement), cardinality));
                }
                else
                {
                    realClass.add(new AttributeInfo(connectorEnd.Role, relatedElement.Name, this.build(relatedElement), cardinality));
                }
            }
        }

        private void addAttributes(string elementName, Collection attributes, RealClass realClass)
        {
            Element clasificador;
            AttributeInfo attributeInfo;
            RealClass realClassClassifierd;

            foreach ( EA.Attribute atributo in attributes )
            {
                attributeInfo = new AttributeInfo(atributo, elementName);
               
                if (atributo.ClassifierID != 0)
                {
                    clasificador = this.eaUtils.repository.GetElementByID(atributo.ClassifierID);

                    attributeInfo.classifierd = clasificador;

                    if ( clasificador.Type == "Enumeration")
                    {
                        if( clasificador.Attributes.Count != 0)
                        {
                            EA.Attribute attr = clasificador.Attributes.GetAt(0);
                            attributeInfo.type = attr.Type;
                        }
                    }
                    else
                    if (clasificador.Type == "Class" || clasificador.Type == "Interface")
                    {
                        if (currentLevel <= maxLevel)
                        {
                            currentLevel++;
                            realClassClassifierd = this.build(clasificador);
                            attributeInfo.relationClass = realClassClassifierd;
                        }
                    }
                }

                realClass.add(attributeInfo);
            }
        }
        private class ConnectorInfo
        {
            public int supplierId = 0;
            public int clientId = 0;
            public Connector connector;
            internal bool isComplete;

            public ConnectorInfo( Connector connector)
            {
                this.connector = connector;
            }

            internal void lookForComplete()
            {
                isComplete = supplierId != 0 && clientId != 0;
            }
        }
    }
}
