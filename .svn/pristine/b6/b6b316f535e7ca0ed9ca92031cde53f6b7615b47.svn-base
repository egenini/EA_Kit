using EA;
using EAUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceCommons
{
    class AttributeMessage
    {
        private Diagram diagrama;
        private EAUtils.EAUtils eaUtils;
        private Element elemento;

        public AttributeMessage(EAUtils.EAUtils eaUtils, Element elemento, Diagram diagrama)
        {
            this.eaUtils = eaUtils;
            this.elemento = elemento;
            this.diagrama = diagrama;
        }

        public Boolean go()
        {
            Boolean changed = true;
            Boolean instanciarAttribute = false;
            EA.Element parentElement = null;

            if (this.diagrama.ParentID != 0)
            {

                EA.DiagramObject diagramObject = null;

                parentElement = eaUtils.repository.GetElementByID(this.diagrama.ParentID);

                // verificar que no exista en el diagrama.
                if (parentElement.Stereotype.Contains("Message") && elemento.ClassifierID == 0
                    && (elemento.Stereotype == "Attribute" || elemento.Stereotype == "Data" || elemento.Stereotype == "DataContainer")
                     )
                {
                    //diagrama.Update();
                    diagramObject = this.eaUtils.diagramUtils.findInDiagramObjects(elemento, diagrama);
                    instanciarAttribute = diagramObject == null ? false : true;
                }
            }

            if (instanciarAttribute)
            {
                // agregar un elemento e instanciarlo

                // Si el elemento a agregar tiene en el explorador un parent y ese parent tiene una instancia
                // en el mismo diagrama donde se tiene que instanciar este elemento, entonces el parent de la 
                // instancia de este nuevo elemento es el parent instanciado en el diagrama.

                if( elemento.ParentID != 0 )
                {
                    DiagramObject brotherDiagramObject;
                    Element brotherInstanceElement;
                    List<ArrayList> elementsAndConnectors;
                    Element parent =  eaUtils.repository.GetElementByID(elemento.ParentID );
                    DiagramObject parentDiagramObject = eaUtils.diagramUtils.findInDiagramObjects(parent, diagrama, true);
                    if(parentDiagramObject != null)
                    {
                        parentElement = eaUtils.repository.GetElementByID(parentDiagramObject.ElementID);
                    }
                    else
                    {
                        // si una instancia de un atributo (un hermano de este) del mismo parent está relacionado con un elemento entonces 
                        // la nueva instancia se relacionará con el mismo elemento que su atributo hermano
                        foreach ( Element brother in parent.Elements )
                        {
                            if( brother.ElementID != elemento.ElementID)
                            {
                                brotherDiagramObject = eaUtils.diagramUtils.findInDiagramObjects(brother, diagrama, true);

                                if (brotherDiagramObject != null)
                                {
                                    brotherInstanceElement = eaUtils.repository.GetElementByID(brotherDiagramObject.ElementID);

                                    elementsAndConnectors = eaUtils.connectorUtils.getFromConnectorFilter(false, brotherInstanceElement, EAUtils.ConnectorUtils.CONNECTOR__NESTING, null, null, "Attribute");

                                    if( elementsAndConnectors.Count != 0 )
                                    {
                                        parentElement = (Element) elementsAndConnectors[0][0];
                                    }
                                }
                            }
                        }
                    }
                }

                EA.Element newElement = eaUtils.instanceUtils.createInstance(elemento, diagrama, parentElement, "Attribute");

                if (newElement == null)
                {
                    eaUtils.instanceUtils.fillNameAliasAndStereotype2InstanceElement(elemento, diagrama);
                }

                //diagrama.Update();
                //this.eaUtils.repository.ReloadDiagram(diagrama.DiagramID);
            }
            return changed;
        }
    }
}
