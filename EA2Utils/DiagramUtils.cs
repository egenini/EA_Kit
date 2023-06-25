using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EA;

namespace EAUtils
{
    public class DiagramUtils
    {
        EA.Repository repository;
        public DiagramUtils(EA.Repository repository)
        {
            this.repository = repository;
        }
        public EA.DiagramObject findInDiagramObjects(EA.Element elemento, EA.Diagram diagrama)
        {
            return this.findInDiagramObjects(elemento, diagrama, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elemento"></param>
        /// <param name="diagrama"></param>
        /// <param name="asInstance">true para indicar que se busca una instancia de este elemento</param>
        /// <returns></returns>
        public EA.DiagramObject findInDiagramObjects(EA.Element elemento, EA.Diagram diagrama, bool asInstance)
        {
            EA.DiagramObject currentDiagramObject = null;
            Element currentElement;
            foreach (EA.DiagramObject diagramObject in diagrama.DiagramObjects)
            {
                if( asInstance )
                {
                    currentElement = repository.GetElementByID(diagramObject.ElementID);
                    if (currentElement.ClassifierID == elemento.ElementID)
                    {
                        currentDiagramObject = diagramObject;
                        break;
                    }
                }
                else
                {
                    if (diagramObject.ElementID == elemento.ElementID)
                    {
                        currentDiagramObject = diagramObject;
                        break;
                    }
                }
            }
            return currentDiagramObject;
        }

        public EA.DiagramObject lookingParentBySize(EA.DiagramObject diagramObjectReference, EA.Diagram currentDiagram)
        {
            EA.DiagramObject toBeElement = null;

            foreach (EA.DiagramObject currentElement in currentDiagram.DiagramObjects)
            {
                if (
                    (currentElement.bottom * -1) > (diagramObjectReference.bottom * -1)
                    && currentElement.left < diagramObjectReference.left
                    && (currentElement.top * -1) < (diagramObjectReference.top * -1)
                    && currentElement.right > diagramObjectReference.right
                )
                {
                    // buscamos el elemento mas chico que pueda contenerlo
                    if (toBeElement == null)
                    {
                        toBeElement = currentElement;
                    }
                    else if (
                    (toBeElement.bottom * -1) > (currentElement.bottom * -1)
                    && toBeElement.left < currentElement.left
                    && (toBeElement.top * -1) < (currentElement.top * -1)
                    && toBeElement.right > currentElement.right
                        )
                    {
                        toBeElement = currentElement;
                    }
                }
            }

            return toBeElement;
        }

        public EA.DiagramObject replaceDiagramElement(EA.Diagram diagram, EA.Element elementToAdd, EA.Element elementToReplace)
        {
            EA.DiagramObject diagramObjectToReplace;

            diagramObjectToReplace = findInDiagramObjects(elementToReplace, diagram);
            
            diagramObjectToReplace.ElementID = elementToAdd.ElementID;
            diagramObjectToReplace.Update();

            return diagramObjectToReplace;
        }

        public void deleteToDiagram(EA.Element elementToDelete, EA.Diagram diagram)
        {
            EA.DiagramObject currentDiagramObject;

            currentDiagramObject = null;

            if (elementToDelete != null)
            {
                EA.Collection diagramObjects;

                diagramObjects = diagram.DiagramObjects;
                short toDelete = -1;

                for (short i = 0; i < diagramObjects.Count; i++)
                {
                    currentDiagramObject = diagramObjects.GetAt(i);

                    // Get the element that this Diagram Object represents
                    if (elementToDelete.ElementID == currentDiagramObject.ElementID)
                    {
                        //diagramObjects.Delete(i);
                        toDelete = i;
                        break;
                    }
                }
                if( toDelete != -1)
                {
                    diagramObjects.DeleteAt(toDelete, true);
                }
            }
        }

        public EA.DiagramObject addDiagramElement(EA.Diagram diagram, EA.Element elementToAdd, DiagramObjectExt doe)
        {
            EA.DiagramObject newDiagramObject;

            newDiagramObject = diagram.DiagramObjects.AddNew(elementToAdd.Name, elementToAdd.Type);

            newDiagramObject.ElementID = elementToAdd.ElementID;
            newDiagramObject.Sequence = 1;

            if (doe.style != null && doe.style != "")
            {
                newDiagramObject.Style = doe.style;
            }

            try
            {
                newDiagramObject.bottom = doe.position.bottom;
            }
            catch (Exception) { }
            try
            {
                newDiagramObject.left = doe.position.left;
            }
            catch (Exception) { }
            try
            {
                newDiagramObject.right = doe.position.right;
            }
            catch (Exception) { }
            try
            {
                newDiagramObject.top = doe.position.top;
            }
            catch (Exception) { }

            newDiagramObject.Update();

            diagram.DiagramObjects.Refresh();

            repository.SaveDiagram(diagram.DiagramID);

            if( repository.GetCurrentDiagram() != null && repository.GetCurrentDiagram().DiagramID == diagram.DiagramID)
            {
                repository.ReloadDiagram(diagram.DiagramID);
            }

            doe.diagramObject = newDiagramObject;

            return newDiagramObject;
        }

        public EA.DiagramObject addDiagramElement( EA.Diagram diagram, EA.Element elementToAdd, Dictionary<string, int> info, String style)
        {
            EA.DiagramObject newDiagramObject;

            newDiagramObject = diagram.DiagramObjects.AddNew(elementToAdd.Name, elementToAdd.Type);

            newDiagramObject.ElementID = elementToAdd.ElementID;
            newDiagramObject.Sequence = 1;

            if (style != null)
            {
                newDiagramObject.Style = style;
            }

            try
            {
                newDiagramObject.bottom = info["bottom"];
            }
            catch (Exception) { }
            try
            {
                newDiagramObject.left = info["left"];
            }
            catch (Exception) { }
            try
            {
                newDiagramObject.right = info["right"];
            }
            catch (Exception) { }
            try
            {
                newDiagramObject.top = info["top"];
            }
            catch (Exception) { }

            newDiagramObject.Update();

            diagram.DiagramObjects.Refresh();

            repository.SaveDiagram(diagram.DiagramID);
            repository.ReloadDiagram(diagram.DiagramID);

            return newDiagramObject;
        }

        public List<Object> findInDiagramObjectsByStereotype(string stereotype, Diagram diagrama)
        {
            List<Object>  elements = new List<Object>();
            Element currentElement;

            foreach( DiagramObject diagramObject in diagrama.DiagramObjects)
            {
                currentElement = this.repository.GetElementByID(diagramObject.ElementID);
                if( currentElement.Stereotype == stereotype )
                {
                    elements.Add(currentElement);
                }
            }
            return elements;
        }

        public void showNewProvidedInterface(EA.Element providedInterface, EA.Diagram diagram)
        {
            // la ubicación de un elemento embebido depende la ubicación de su contenedor y del tamaño del elemento a mostrar.
            // Una interfaz embebida tiene un tamaño de 40 de alto x 13 ancho.
            // Vamos a ubicar el elemento contando desde la esquina superior izquierda y considerando que hay un lugar libre, nos 
            // desplazamos a la izquierda buscando un lugar disponible.
            int alto = 40;
            int ancho = 13;

            // buscar el elemento contenedor.
            EA.Element container = this.repository.GetElementByID(providedInterface.ParentID);

            EA.DiagramObject containerObject = this.findInDiagramObjects(container, diagram);

            EA.DiagramObject newDiagramObject = diagram.DiagramObjects.AddNew(providedInterface.Name, providedInterface.Type);

            newDiagramObject.ElementID = providedInterface.ElementID;
            newDiagramObject.Sequence = 1;

            newDiagramObject.bottom = containerObject.bottom + 1;
            newDiagramObject.left = containerObject.left + ancho;
            newDiagramObject.right = newDiagramObject.left + ancho;
            newDiagramObject.top = containerObject.top + alto;

            newDiagramObject.Update();

            diagram.DiagramObjects.Refresh();
        }
    }
}
