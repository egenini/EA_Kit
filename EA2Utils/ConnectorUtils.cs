using EA;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EAUtils
{
    public class ConnectorUtils
    {
        public const string DIRECTION__UNSPECIFIED = "Unspecified";
        public const string DIRECTION__BI_DIRECTIONAL = "Bi-Directional";
        public const string DIRECTION__SOURCE_DESTINATION = "Source -> Destination";
        public const string DIRECTION__DESTINATION_SOURCE = "Destination -> Source";

        public static string[] DIRECTION_FILTER__NOT_DESTINATION_SOURCE = new string[] { "-" + ConnectorUtils.DIRECTION__DESTINATION_SOURCE };

        EA.Repository repository;

        /* http://sparxsystems.com/enterprise_architect_user_guide/13.0/automation/connector2_2.html
         * Aggregation
Assembly
Association
Collaboration
CommunicationPath
Connector
ControlFlow
Delegate
Dependency
Deployment
ERLink
Generalization
InformationFlow
Instantiation
InterruptFlow
Manifest
Nesting
NoteLink
ObjectFlow
Package
Realization
Sequence
StateFlow
TemplateBinding
UseCase
         */
        public static string CONNECTOR__AGGREGATION = "Aggregation";
        public static string CONNECTOR__ASSEMBLY = "Assembly";
        public static string CONNECTOR__ASSOCIATION = "Association";
        public static string CONNECTOR__DEPENDENCY = "Dependency";
        public static string CONNECTOR__GENERALIZATION = "Generalization";
        public static string CONNECTOR__NESTING = "Nesting";
        public static string CONNECTOR__INSTANTIATION = "Instantiation";
        public static string CONNECTOR__REALIZATION = "Realisation";
        public static string CONNECTOR__REALISATION = "Realisation";


        public ConnectorUtils(EA.Repository repository)
        {
            this.repository = repository;
        }

        public Connector hasConnector( EA.Element source, EA.Element target, string connectorType, string connectorStereotype)
        {
            Connector connector = null;

            foreach ( Connector current in source.Connectors )
            {
                if( current.SupplierID == target.ElementID && current.Type == connectorType 
                    && ( connectorStereotype != null && connectorStereotype == current.Stereotype) )
                {
                    connector = current;
                    break;
                }
            }
            return connector;
        }
        public EA.Connector addConnectorIfNotHas(EA.Element source, EA.Element target, string type, string name, string stereotype)
        {
            return this.addConnector(source, target, type, name, stereotype, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">Origen de la relación</param>
        /// <param name="target">Destino de la relación</param>
        /// <param name="type">Tipo de relación</param>
        /// <param name="name"> nombre para la relación</param>
        /// <param name="stereotype">Estereotipo de la relación</param>
        /// <param name="onlyOne">el valor en true impide que la relación se duplique</param>
        /// <returns></returns>
        public EA.Connector addConnector(EA.Element source, EA.Element target, string type, string name, string stereotype, Boolean onlyOne)
        {
            Connector connector = null;

            if( onlyOne)
            {
                connector = this.hasConnector(source, target, type, stereotype);

                if( connector == null)
                {
                    connector = this.addConnector(source, target, type, name, stereotype);
                }
            }
            else
            {
                connector = this.addConnector(source, target, type, name, stereotype);
            }
            return connector;
        }

        public EA.Connector addRealization(EA.Element source, EA.Element target)
        {
            return this.addConnectorRealization(source, target, null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"> El origen de la relación </param>
        /// <param name="target">El destino de la relación</param>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="stereotype"></param>
        /// <returns></returns>
        public EA.Connector addConnector(EA.Element source, EA.Element target, string type, string name, string stereotype)
        {
            EA.Connector connector = null;
            if ( type == CONNECTOR__REALISATION || type == CONNECTOR__GENERALIZATION || type == CONNECTOR__INSTANTIATION)
            {
                // conectores que sólo pueden estar 1 vez
                List < ElementConnectorInfo > elementsConnectorsInfo = this.get(source, type, null, target.Type, stereotype, false, null);

                foreach( ElementConnectorInfo elementConnectorInfo in elementsConnectorsInfo)
                {
                    if( elementConnectorInfo.element.ElementID == target.ElementID)
                    {
                        connector = elementConnectorInfo.connector;
                        break;
                    }
                }
            }

            if( connector == null)
            {
                connector = (EA.Connector)source.Connectors.AddNew(name == null ? "" : name, type);

                connector.SupplierID = target.ElementID;

                if (stereotype != null && stereotype.Length != 0)
                {
                    connector.Stereotype = stereotype;
                }

                connector.Update();
                source.Connectors.Refresh();
            }

            return connector;
        }


        public EA.Connector addConnector(EA.Element source, EA.Element target, string type, string name, string stereotype, string direction)
        {
            EA.Connector connector = (EA.Connector)source.Connectors.AddNew(name == null ? "" : name, type);

            connector.SupplierID = target.ElementID;

            if (stereotype != null && stereotype.Length != 0)
            {
                connector.Stereotype = stereotype;
            }
            if(direction != null)
            {
                connector.Direction = direction;
            }
            connector.Update();
            source.Connectors.Refresh();

            return connector;
        }
        public void settingConnectorsForInstanceElement(EA.Element elemento, EA.Element classifier, EA.Diagram currentDiagram)
        {
            EA.Connector connector;
            EA.Collection instanceConnectors;

            Boolean tieneConnectorInstanciacion = false;

            instanceConnectors = elemento.Connectors;

            foreach (EA.Connector localConnector in instanceConnectors)
            {
                if (localConnector.Type == "Instantiation" || localConnector.Type == "Instanciación")
                {
                    tieneConnectorInstanciacion = true;
                    connector = localConnector;
                    break;
                }
            }

            if (!tieneConnectorInstanciacion)
            {
                connector = instanceConnectors.AddNew("", "Instantiation");
                connector.ClientID = elemento.ElementID;
                connector.SupplierID = classifier.ElementID;

                connector.Update();
                instanceConnectors.Refresh();

                if (currentDiagram != null)
                {
                    DiagramLink link = currentDiagram.DiagramLinks.AddNew("Instantiation", "Instantiation");
                    link.LineStyle = EA.LinkLineStyle.LineStyleDirect;
                    link.ConnectorID = connector.ConnectorID;
                    link.Update();
                    currentDiagram.DiagramLinks.Refresh();
                }
            }

            /*
            if(currentDiagram != null)
            {
                EA.Collection diagramLinks;
                diagramLinks = currentDiagram.DiagramLinks;

                
                foreach (EA.DiagramLink diagramLink in diagramLinks)
                {
                    connector = this.repository.GetConnectorByID(diagramLink.ConnectorID);

                    if (connector.Type != "Instantiation")
                    {
                        if (connector.ClientID == classifier.ElementID)
                        {
                            connector.ClientID = elemento.ElementID;

                            connector.Update();
                            elemento.Connectors.Refresh();
                        }
                        else if (connector.SupplierID == classifier.ElementID)
                        {
                            connector.SupplierID = elemento.ElementID;

                            connector.Update();
                            elemento.Connectors.Refresh();
                        }
                    }
                }
            }
            */
        }

        public List<ArrayList> getFromConnectorFilter( Element sourceOfConnectors, string connectorType, string connectorStereotype, string otherElementType, string otherElementStereotype)
        {
            List<ArrayList> elementsAndConnectors = new List<ArrayList>();
            EA.Connector connector;
            EA.Element element;
            EA.Element elementSource;
            EA.Element elementTarget;
            EA.Collection connectors;

            connectors    = sourceOfConnectors.Connectors;
            elementSource = null;
            elementTarget = null;

            for (short i = 0; i < connectors.Count; i++)
            {
                connector = connectors.GetAt(i);
                element = null;

                if ((connectorType != null && connector.Type != connectorType) || (connectorStereotype != null && connector.Stereotype != connectorStereotype))
                {
                    continue;
                }

                elementSource = this.repository.GetElementByID(connector.ClientID);
                elementTarget = this.repository.GetElementByID(connector.SupplierID);

                if (elementSource.ElementID == sourceOfConnectors.ElementID)
                {
                    element = elementTarget;
                }
                else if( elementTarget.ElementID == sourceOfConnectors.ElementID)
                {
                    element = elementSource;
                }

                if ((otherElementType != null && element.Type != otherElementType) || (otherElementStereotype != null && element.Stereotype != otherElementStereotype))
                {
                    element = null;
                    continue;
                }

                if (element != null && element != null)
                {
                    ArrayList elementAndConnector = new ArrayList();
                    elementAndConnector.Add(element);
                    elementAndConnector.Add(connector);
                    elementsAndConnectors.Add(elementAndConnector);
                }
            }
            return elementsAndConnectors;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="isLookingForSource">¿El elemento buscado se trata de un origen de la relación?</param>
        /// <param name="sourceOfConnectors">Elemento del cual se obtienen los conectores</param>
        /// <param name="connectorType">tipo de conector que se busca</param>
        /// <param name="connectorStereotype">estereotipo del conector que se busca</param>
        /// <param name="otherElementType">elemento en el otro extremo de la relacion que se busca</param>
        /// <param name="otherElementStereotype">estereotipo del elemento del otro extremo de la relacion que se busca</param>
        /// <returns>Una lista (linea) con una lista de 2 posiciones, donde el primero es un elemento y el segundo es un conector.</returns>
        /// 
        public List<ArrayList> getFromConnectorFilter(Boolean isLookingForSource, Element sourceOfConnectors, string connectorType
            , string connectorStereotype, string otherElementType, string otherElementStereotype )
        {
            List<ArrayList> elementsAndConnectors = new List<ArrayList>();
            EA.Connector connector;
            EA.Element element;
            EA.Collection connectors;

            connectors = sourceOfConnectors.Connectors;

            for (short i = 0; i < connectors.Count; i++)
            {
                connector = connectors.GetAt(i);
                element = null;

                if ((connectorType != null && connector.Type != connectorType) || (connectorStereotype != null && connector.Stereotype != connectorStereotype))
                {
                    continue;
                }

                if (isLookingForSource)
                {
                    element = this.repository.GetElementByID(connector.ClientID);
                }
                else
                {
                    element = this.repository.GetElementByID(connector.SupplierID);
                }

                // control de abrazo mortal
                if (element.ElementID == sourceOfConnectors.ElementID)
                {
                    continue;
                }
                if ((otherElementType != null && element.Type != otherElementType) || (otherElementStereotype != null && element.Stereotype != otherElementStereotype))
                {
                    element = null;
                    continue;
                }

                if (element != null && element != null)
                {
                    ArrayList elementAndConnector = new ArrayList();
                    elementAndConnector.Add(element);
                    elementAndConnector.Add(connector);
                    elementsAndConnectors.Add(elementAndConnector);
                }
            }
            return elementsAndConnectors;
        }

        /// <summary>
        /// Busca los elementos y su conector según el filtro.
        /// Hay 3 criterios estratégicos:
        /// 1: buscar por origen - destino del conector.
        /// 2: buscar por dirección del conector.
        /// 3: no importa ni origen ni destino ni la dirección.
        /// </summary>
        /// <param name="sourceOfConnectors">Elemento desde el cual se toman los conectores</param>
        /// <param name="connectorType">Tipo de conector buscado</param>
        /// <param name="connectorStereotype">Estereotipo del conector buscado</param>
        /// <param name="otherElementType">El tipo del elemento buscado</param>
        /// <param name="otherElementStereotype"></param>
        /// <param name="searchSource">¿El elemento buscado es origen del conector?</param>
        /// <param name="direction">Si no se busca el source o target se puede buscar por la dirección de la relación</param>
        /// <returns></returns>
        public List<ElementConnectorInfo> get(Element sourceOfConnectors, string connectorType
    , string connectorStereotype, string otherElementType, string otherElementStereotype, bool? searchSource, string[] directions)
        {
            List<ElementConnectorInfo> elementsAndConnectors = new List<ElementConnectorInfo>();
            EA.Connector connector;
            EA.Collection connectors;

            connectors = sourceOfConnectors.Connectors;

            for (short i = 0; i < connectors.Count; i++)
            {
                connector = connectors.GetAt(i);

                ElementConnectorInfo elementConnectorInfo  = this.get(sourceOfConnectors, connector, connectorType, connectorStereotype, otherElementType, otherElementStereotype, searchSource, directions);

                if(elementConnectorInfo != null)
                {
                    elementsAndConnectors.Add(elementConnectorInfo);
                }
            }
            return elementsAndConnectors;
        }

        public List<ElementConnectorInfo> get(Element sourceOfConnectors, List<ElementConnectorInfo> elementsConnectorsInfo
    ,string connectorType, string connectorStereotype, string otherElementType, string otherElementStereotype, bool? searchSource, string[] directions)
        {
            List<ElementConnectorInfo> elementsAndConnectors = new List<ElementConnectorInfo>();

            foreach(ElementConnectorInfo elementConnectorInfo in elementsConnectorsInfo)
            {
                ElementConnectorInfo info = this.get(sourceOfConnectors, elementConnectorInfo.connector, connectorType, connectorStereotype, otherElementType, otherElementStereotype, searchSource, directions);

                if (elementConnectorInfo != null)
                {
                    elementsAndConnectors.Add(elementConnectorInfo);
                }
            }

            return elementsAndConnectors;
        }

        public ElementConnectorInfo get(Element sourceOfConnectors, Connector connector, string connectorType
, string connectorStereotype, string otherElementType, string otherElementStereotype, bool? searchSource, string[] directions)
        {
            EA.Element element = null;
            ElementConnectorInfo elementConnectorInfo = null;

            if (searchSource != null)
            {
                if (searchSource == true)
                {
                    element = this.repository.GetElementByID(connector.ClientID);
                }
                else
                {
                    element = this.repository.GetElementByID(connector.SupplierID);
                }
            }
            else
            {
                element = applyDirections(connector, directions, sourceOfConnectors);
            }

            if( element != null && element.ElementID != sourceOfConnectors.ElementID)
            {
                if ((otherElementType != null && element.Type != otherElementType) || (otherElementStereotype != null && element.Stereotype != otherElementStereotype))
                {
                    element = null;
                }
            }
            else
            {
                element = null;
            }

            if ((connectorType != null && connector.Type != connectorType) || (connectorStereotype != null && connector.Stereotype != connectorStereotype))
            {
                element = null;
            }

            if (element != null )
            {
                elementConnectorInfo = new ElementConnectorInfo();
                elementConnectorInfo.element = element;
                elementConnectorInfo.connector = connector;
            }
            return elementConnectorInfo;
        }

        public Element applyDirections( Connector connector, string[] directions, Element sourceOfConnectors)
        {
            directions = makeDirections(directions);

            Element element = null;

            foreach( string direction in directions)
            {
                if (direction != null && connector.Direction == direction)
                {
                    if (direction == DIRECTION__BI_DIRECTIONAL || direction == DIRECTION__UNSPECIFIED)
                    {
                        element = this.repository.GetElementByID(connector.ClientID);
                        
                        // buscamos el "otro"
                        if (element.ElementID == sourceOfConnectors.ElementID)
                        {
                            element = this.repository.GetElementByID(connector.SupplierID);
                        }
                        break;
                    }
                    else if (direction == DIRECTION__SOURCE_DESTINATION)
                    {
                        element = this.repository.GetElementByID(connector.SupplierID);
                        break;
                    }
                    else
                    {
                        element = this.repository.GetElementByID(connector.ClientID);
                        break;
                    }
                }
            }

            return element;
        }

        public string[] makeDirections(string[] directions)
        {
            List<string> all = new List<string>();
            List<string> newDirections = new List<string>();

            all.Add(DIRECTION__BI_DIRECTIONAL);
            all.Add(DIRECTION__DESTINATION_SOURCE);
            all.Add(DIRECTION__SOURCE_DESTINATION);
            all.Add(DIRECTION__UNSPECIFIED);

            if( directions != null)
            {
                foreach (string direction in directions)
                {
                    // se espera que se defina por la positivia o por la negativa.
                    if (direction.StartsWith("-"))
                    {
                        all.Remove(direction.Substring(1));
                    }
                    else
                    {
                        newDirections.Add(direction);
                    }
                }
                if (newDirections.Count == 0)
                {
                    newDirections.AddRange(all);
                }
            }

            return directions == null ? new string[] { } : newDirections.ToArray<string>();
        }

        public class ElementConnectorInfo
        {
            public Element element;
            public Connector connector;
        }

        public Cardinality getCardinality(ConnectorEnd connectorEnd)
        {
            Cardinality cardinality = new Cardinality(connectorEnd.Cardinality);

            return cardinality;
        }

        public class Cardinality : ICardinality
        {
            public string lower = "1";
            public string upper = "1";
            public string cardinality = "1";

            public Cardinality(string cardinality)
            {
                this.cardinality = cardinality;
                String[] splitted = cardinality.Split('.');

                // ["1"] || ["1", ".", "*"]
                if (splitted.Length == 1)
                {
                    lower = splitted[0];
                }
                else if (cardinality.Length >= 3)
                {
                    lower = splitted[0];
                    upper = splitted[2];
                }
            }
            public Boolean isCollection()
            {
                int l;
                return (this.lower.Contains("*") || this.upper.Contains("*") || (int.TryParse(lower, out l) ? l > 1 : false) || (int.TryParse(upper, out l) ? l > 1 : false));
            }
        }

        public Connector addConnectorInstantiation(Element source, Element target, string connectorStereotype)
        {
            return addConnector(source, target, "Instantiation", null, connectorStereotype);
        }
        public Connector addConnectorDependency(Element source, Element target, string name,string connectorStereotype)
        {
            return addConnector(source, target, "Dependency", name, connectorStereotype);
        }
        public Connector addConnectorRealization(Element source, Element target, string name, string connectorStereotype)
        {
            return addConnector(source, target, "Realisation", name, connectorStereotype);
        }
        public Connector addConnectorNesting(Element source, Element target, string name, string connectorStereotype)
        {
            return addConnector(source, target, "Nesting", name, connectorStereotype);
        }
        public Connector addConnectorAssociation(Element source, Element target, string name, string connectorStereotype)
        {
            return addConnector(source, target, CONNECTOR__ASSOCIATION, name, connectorStereotype);
        }
        public Connector addConnectorAssociation(Element source, Element target, string name, string connectorStereotype, string direction)
        {
            return addConnector(source, target, CONNECTOR__ASSOCIATION, name, connectorStereotype, direction);
        }

        public Connector addOnlyOneAssociation(Element source, Element target, string name, string connectorStereotype, string direction)
        {
            Connector connector = hasConnector(source, target, CONNECTOR__ASSOCIATION, connectorStereotype);

            if (connector == null)
            {
                connector = addConnector(source, target, CONNECTOR__ASSOCIATION, name, connectorStereotype, direction);
            }
            return connector;
        }
    }
}
