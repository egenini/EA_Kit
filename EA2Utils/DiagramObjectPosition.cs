using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EAUtils
{
    public class DiagramObjectPosition
    {
        public Offset offset = new Offset();
        public int margin = 5;
	    public int height = 0;
	    public int width  = 0;

        public int top = 0;
        public int left = 0;
        public int bottom = 0;
        public int right = 0;

        public DiagramObject diagramObject = null;

        public DiagramObjectPosition()
        {
        }

        public DiagramObjectPosition(DiagramObject diagramObject)
        {
            this.diagramObject = diagramObject;
            setFrom();
        }

        public void setFrom()
        {
            if ( diagramObject != null)
            {
                this.top = diagramObject.top;
                this.left = diagramObject.left;
                this.right = diagramObject.right;
                this.bottom = diagramObject.bottom;

                this.height = this.bottom - this.top; // -50 - - 20 = 30
                this.width = this.right - this.left;
            }
        }
        public void calculate()
        {
            this.calculate(this.diagramObject);
        }

        public void calculate( DiagramObject diagramObject)
        {
            // si es null diagramObject no importa porque todos los valores quedan en 0 (cero)
            DiagramObjectPosition diagramObjectExt = new DiagramObjectPosition(diagramObject);

            this.top = -((-diagramObjectExt.bottom) + this.offset.top + this.margin);
            this.left = (diagramObjectExt.right + this.offset.left + this.margin);

            this.bottom = this.top - this.height;
            this.right = this.left + this.width;
        }

        public void calculateVerticalTree(DiagramObject lastDiagramObject, DiagramObject parentDiagramObject)
        {
            // si es null diagramObject no importa porque todos los valores quedan en 0 (cero)
            DiagramObjectPosition lastDiagramObjectExt = new DiagramObjectPosition(lastDiagramObject);
            DiagramObjectPosition parentDiagramObjectExt = new DiagramObjectPosition(parentDiagramObject);

            this.top = -((-lastDiagramObjectExt.bottom) + this.offset.top + this.margin);
            this.left = parentDiagramObjectExt.right + this.offset.left + this.margin;

            this.bottom = this.top - this.height;
            this.right = this.left + this.width;
        }
    }

    public class Offset
    {
        public int top = 0;
        public int left = 0;

    }
}
