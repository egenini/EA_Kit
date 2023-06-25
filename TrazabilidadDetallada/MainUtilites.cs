using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrazabilidadDetallada
{
    /// <summary>
    /// Establece la elegibilidad de los elementos sea 
    /// para habilitar opciones de menú o para ejecutar alguna acción sobre ellos
    /// </summary>
    public class MainUtilites
    {
        public Mapper mapper = new Mapper();

        public MainUtilites()
        {

        }

        /// <summary>
        /// Este método es para invocar desde eventos
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        public Diagram getMapeable(Diagram current)
        {
            Diagram diagram = null;

            if (current != null && current.DiagramObjects.Count > 1 )
            {
                if (current.SwimlaneDef.Swimlanes.Count == 2)
                {
                    // asumimos que a la izquerda está el origen y la otra es el destino.
                    diagram = current;
                }
                else if (current.SwimlaneDef.Swimlanes.Count > 2)
                {
                    bool hayOrigen = false;
                    bool hayDestino = false;
                    // hay que encontrar al menos 2 que se llamen origen y destino o source y target.
                    foreach (EA.Swimlane swimlane in current.SwimlaneDef.Swimlanes)
                    {
                        if ( mapper.swimlanesSourcesNames.Contains( swimlane.Title.ToUpper() ) )
                        {
                            hayOrigen = true;
                        }
                        if (mapper.swimlanesTargetsNames.Contains(swimlane.Title.ToUpper()))
                        {
                            hayDestino = true;
                        }
                    }

                    if (hayDestino && hayOrigen)
                    {
                        diagram = current;
                    }
                }
            }
            return diagram;
        }

        public Diagram getMapeable(Repository repository, string location)
        {
            Diagram current = null;

            if (location == MainConstants.MENU_LOCATION__TREEVIEW)
            {
                ObjectType ot = repository.GetTreeSelectedItemType();
                if (ot.Equals(ObjectType.otDiagram))
                {
                    current = (Diagram)repository.GetTreeSelectedObject();
                }
            }
            else
            {
                ObjectType ot = repository.GetContextItemType();
                if (ot.Equals(ObjectType.otDiagram))
                {
                    current = (Diagram)repository.GetContextObject();
                }
            }

            return this.getMapeable(current);
        }
    }
}
