using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RestFul
{
    class DiagramUtils
    {
        private Repository repository;

        public DiagramUtils(Repository repository)
        {
            this.repository = repository;
        }

        public DiagramObject findInDiagramObjects(Element currentElement, Diagram currentDiagram)
        {
            DiagramObject currentDiagramObject = null;

            foreach (EA.DiagramObject currentDO in currentDiagram.DiagramObjects)
            {
                if( currentDO.ElementID == currentElement.ElementID )
                {
                    currentDiagramObject = currentDO;
                    break;
                }
            }

            return currentDiagramObject;
        }
    }
}
