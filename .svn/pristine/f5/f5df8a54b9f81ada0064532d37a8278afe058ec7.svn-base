using EA;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrazabilidadDetallada
{

    public class Mapper
    {
        const short IN_SWIMLANE_UNKNOWN = 0;
        const short IN_SWIMLANE_SOURCE = 1;
        const short IN_SWIMLANE_TARGET = -1;

        EAUtils.EAUtils eaUtils;
        Diagram diagram;
        int origenSwimlaneFrom;
        int origenSwimlaneWidth;
        int destinoSwimlaneFrom;
        int destinoSwimlaneWidth;

        public List<string> swimlanesAllowedNames = new List<string>();
        public List<string> swimlanesSourcesNames = new List<string>();
        public List<string> swimlanesTargetsNames = new List<string>();

        public Mapper()
        {
            swimlanesAllowedNames.Add(MainConstants.SWIMLANE_ALLOWED_NAME__ORIGEN);
            swimlanesAllowedNames.Add(MainConstants.SWIMLANE_ALLOWED_NAME__FUENTE);
            swimlanesAllowedNames.Add(MainConstants.SWIMLANE_ALLOWED_NAME__DESTINO);
            swimlanesAllowedNames.Add(MainConstants.SWIMLANE_ALLOWED_NAME__SOURCE);
            swimlanesAllowedNames.Add(MainConstants.SWIMLANE_ALLOWED_NAME__TARGET);

            swimlanesSourcesNames.Add(MainConstants.SWIMLANE_ALLOWED_NAME__ORIGEN);
            swimlanesSourcesNames.Add(MainConstants.SWIMLANE_ALLOWED_NAME__FUENTE);

            swimlanesTargetsNames.Add(MainConstants.SWIMLANE_ALLOWED_NAME__DESTINO);
            swimlanesTargetsNames.Add(MainConstants.SWIMLANE_ALLOWED_NAME__SOURCE);
            swimlanesTargetsNames.Add(MainConstants.SWIMLANE_ALLOWED_NAME__TARGET);

        }
        public Mapper( EAUtils.EAUtils eaUtils, Diagram diagram) : this()
        {
            this.diagram = diagram;
            this.eaUtils = eaUtils;
        }

        public bool elegible(Element element)
        {
            return element.Stereotype.Length != 0 && (element.Stereotype == MainConstants.STEREOTYPE__CLASS || element.Stereotype == MainConstants.STEREOTYPE__ATTRIBUTE || element.Stereotype == MainConstants.STEREOTYPE__DBCOLUMN || element.Stereotype.StartsWith(MainConstants.STEREOTYPE__UI_PREFIX));
        }

        public void go( Element element )
        {
            if ( this.elegible( element ) )
            {
                short inSwimlane;
                Element candidato;
                List<int> mappingsByIdCandidato;

                this.lookingForSwimlanes();

                // ver si este elemento implementa por si o por su clasificador.
                List<int> mappingsById = this.resolveMapping(element);

                // buscar en diagrama otro elemento que pueda ser origen o destino.
                foreach ( DiagramObject diagramObject in diagram.DiagramObjects)
                {
                    candidato = eaUtils.repository.GetElementByID(diagramObject.ElementID);

                    if( this.elegible(candidato) )
                    {
                        // si concide con alguna implementación o por el clasificador.
                        mappingsByIdCandidato = this.resolveMapping(element);

                        foreach( int idToCompare in mappingsById )
                        {
                            foreach ( int idFromCandidato in mappingsByIdCandidato )
                            {
                                if( idToCompare == idFromCandidato )
                                {
                                    // resolver el rol de ambos, sólo hay 1 en el diagrama
                                    inSwimlane = resolveSwimlnane(diagramObject);

                                    if( inSwimlane == IN_SWIMLANE_SOURCE )
                                    {
                                        this.eaUtils.connectorUtils.addConnector(candidato, element
                                               , "Dependency", null, null, true);

                                    }
                                    else if (inSwimlane == IN_SWIMLANE_TARGET)
                                    {
                                        this.eaUtils.connectorUtils.addConnector( element, candidato
                                               , "Dependency", null, null, true);

                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void go()
        {
            this.eaUtils.repository.SaveDiagram(this.diagram.DiagramID);

            this.lookingForSwimlanes();

            Element element;
            List<int> mappingsById;
            Dictionary<int, List<Element>> elementosOrigen = new Dictionary<int, List<Element>>();
            Dictionary<int, List<Element>> elementosDestino = new Dictionary<int, List<Element>>();
            short inSwimlane; 

            foreach (DiagramObject diagramObject in diagram.DiagramObjects)
            {
                element = this.eaUtils.repository.GetElementByID(diagramObject.ElementID);

                if ( this.elegible(element) )
                {
                    mappingsById = this.resolveMapping(element);
                    // ubicamos el diagramObject en origen o destino.

                    inSwimlane = resolveSwimlnane(diagramObject);

                    if (inSwimlane == IN_SWIMLANE_SOURCE)
                    {
                        foreach ( int mappedId in mappingsById)
                        {
                            if (! elementosOrigen.ContainsKey( mappedId ) )
                            {
                                elementosOrigen.Add(mappedId, new List<Element>() );
                            }

                            elementosOrigen[mappedId].Add(element);
                        }

                    }
                    else if (inSwimlane == IN_SWIMLANE_TARGET)
                    {
                        foreach (int mappedId in mappingsById)
                        {
                            if (!elementosDestino.ContainsKey(mappedId))
                            {
                                elementosDestino.Add(mappedId, new List<Element>());
                            }

                            elementosDestino[mappedId].Add(element);
                        }
                    }
                }
            }
            // hasta aca tengo los origenes y destinos y su referencia al modelo de dominio.
            // ahora tengo que establecer por cada origen su o sus destinos en base a la referencia al dominio.

	        foreach(int mappingId in elementosOrigen.Keys)
            {
			//Session.Output("e " + elementosOrigenClasificador);
			
			    foreach(EA.Element origenElement in elementosOrigen[mappingId])
                {
                    if (elementosDestino.ContainsKey(mappingId) )
                    {
					    foreach(EA.Element destinoElement in elementosDestino[mappingId])
                        {
                            this.eaUtils.connectorUtils.addConnector(destinoElement, origenElement
                            , "Dependency", null, null, true);
                        }
                    }
                }
            }

            this.eaUtils.repository.ReloadDiagram(this.diagram.DiagramID);
        }

        public short resolveSwimlnane(DiagramObject diagramObject)
        {
            short inSwimlane = IN_SWIMLANE_UNKNOWN;

            if (diagramObject.left > origenSwimlaneFrom && diagramObject.left <= (origenSwimlaneFrom + origenSwimlaneWidth))
            {
                inSwimlane = IN_SWIMLANE_SOURCE;
            }
            else if (diagramObject.left > destinoSwimlaneFrom && diagramObject.left <= (destinoSwimlaneFrom + destinoSwimlaneWidth))
            {
                inSwimlane = IN_SWIMLANE_TARGET;
            }
            return inSwimlane;
        }

        public List<int> resolveMapping( Element element)
        {
            List<Element> implementaciones;
            List<int> mappingsById;

            implementaciones = buscarImplementacion(element.ClassifierID);

            if (implementaciones == null)
            {
                implementaciones = buscarImplementacion(element.ElementID);
            }

            mappingsById = new List<int>();

            if (implementaciones != null)
            {
                foreach (Element implementacion in implementaciones)
                {
                    mappingsById.Add(implementacion.ElementID);
                }
            }
            else
            {
                mappingsById.Add(element.ClassifierID);
            }
            return mappingsById;
        }

        public void lookingForSwimlanes()
        {
            int currentWidth = 0;

            if(this.diagram.SwimlaneDef.Swimlanes.Count == 2)
            {
                EA.Swimlane swimlane = this.diagram.SwimlaneDef.Swimlanes.Items(0);
                this.origenSwimlaneFrom = 0;
                this.origenSwimlaneWidth = swimlane.Width;

                swimlane = this.diagram.SwimlaneDef.Swimlanes.Items(1);
                this.destinoSwimlaneFrom = this.origenSwimlaneFrom + this.origenSwimlaneWidth;
                this.destinoSwimlaneWidth = swimlane.Width;
            }
            else
            {
                foreach (EA.Swimlane swimlane in this.diagram.SwimlaneDef.Swimlanes)
                {
                    if ( swimlanesSourcesNames.Contains(swimlane.Title.ToUpper()) )
                    {
                        this.origenSwimlaneFrom = currentWidth;
                        this.origenSwimlaneWidth = swimlane.Width;
                    }
                    if (swimlanesTargetsNames.Contains(swimlane.Title.ToUpper()))
                    {
                        this.destinoSwimlaneFrom = currentWidth;
                        this.destinoSwimlaneWidth = swimlane.Width;
                    }

                    currentWidth += swimlane.Width;
                }
            }
        }

        public List<Element> buscarImplementacion( int elementId)
        {
            List <Element> elements = null;

            if (elementId != 0)
            {
                Element classifier = this.eaUtils.repository.GetElementByID(elementId);

                List<ArrayList> elementsAndConnectors = this.eaUtils.connectorUtils.getFromConnectorFilter(false, classifier, "Realisation", null, null, null);

                if (elementsAndConnectors.Count != 0)
                {
                    try
                    {
				for(int elementAndConnectorIndex = 0; elementAndConnectorIndex < elementsAndConnectors.Count; elementAndConnectorIndex++)
                        {
                            if (elements == null)
                            {
                                elements = new List<Element>();
                            }
                            //Session.Output("Implementa "+ elementsAndConnectors[elementAndConnectorIndex][0].Name);
                            elements.Add((Element)elementsAndConnectors[elementAndConnectorIndex][0]);
                        }
                    }
                    catch (Exception ) { }
                }
            }
            return elements;
        }
    }
}
