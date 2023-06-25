using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EAUtils.ConnectorUtils;

namespace ProjectManagement
{
    public class TareaPriorizada
    {
        

        public Dictionary<string, List<Nodo>> agrupacionPorEjecutor = new Dictionary<string, List<Nodo>>();
        public Dictionary<int, List<Element>> nodosConVariosPadres = new Dictionary<int, List<Element>>();
        public Dictionary<string, List<Nodo>> arbolPorEjecutor = new Dictionary<string, List<Nodo>>();
        public Dictionary<string, List<Nodo>> noDependeDeNinguno = new Dictionary<string, List<Nodo>>();

        public void add( EjecutorManager ejecutorManager, Element element, List<ElementConnectorInfo> elementsAndConnectors)
        {
            if ( ! agrupacionPorEjecutor.ContainsKey(ejecutorManager.ejecutor.Name)){

                agrupacionPorEjecutor.Add(ejecutorManager.ejecutor.Name, new List<Nodo>());
            }

            Nodo nodo;
            if( elementsAndConnectors.Count == 0)
            {
                nodo = new Nodo(element);

                if( ! noDependeDeNinguno.ContainsKey(ejecutorManager.ejecutor.Name) )
                {
                    noDependeDeNinguno.Add(ejecutorManager.ejecutor.Name, new List<Nodo>());
                }

                /// antes de armar el árbol hay que "ordenar" a estos nodos
                /// ese órden implica ver si tienen prioridad y si no la tienen se los ordena por aparición
                /// esto es porque el mismo ejecutor no púede ejecutar más de 1 tarea a la vez.
                /// Como poder puede pero desde el punto de vista de esta funcionalidad no es factible.
                noDependeDeNinguno[ejecutorManager.ejecutor.Name].Add(nodo);

                agrupacionPorEjecutor[ejecutorManager.ejecutor.Name].Add(nodo);
            }
            else if(elementsAndConnectors.Count == 1)
            {
                nodo = new Nodo(element, elementsAndConnectors[0].element.ElementID);

                agrupacionPorEjecutor[ejecutorManager.ejecutor.Name].Add(nodo);
            }
            else
            {
                // en este caso la tarea depende de más de 1 tarea
                int index = 0;

                foreach(ElementConnectorInfo info in elementsAndConnectors)
                {
                    ejecutorManager.desprogramar();

                    if ( ! nodosConVariosPadres.ContainsKey(element.ElementID))
                    {
                        nodosConVariosPadres.Add(element.ElementID, new List<Element>());
                    }

                    nodosConVariosPadres[element.ElementID].Add(info.element);

                    if( index == 0)
                    {
                        agrupacionPorEjecutor[ejecutorManager.ejecutor.Name].Add(new Nodo(element, info.element.ElementID));
                    }
                    index++;
                }
            }
        }

        public void construirArbol()
        {
            ordenarPrioridadDeLosQueNoDepdendenDeNinguno();

            foreach( KeyValuePair<string, List<Nodo> > kv in agrupacionPorEjecutor)
            {
                arbolPorEjecutor.Add(kv.Key, buildTree(kv.Value));
            }
        }

        private void ordenarPrioridadDeLosQueNoDepdendenDeNinguno()
        {
            List<Nodo> priorizados = null;

            foreach (KeyValuePair<string, List<Nodo>> kv in this.noDependeDeNinguno)
            {
                if(kv.Value.Count == 0 || kv.Value.Count == 1)
                {
                    continue;
                }
                else
                {
                    priorizados = kv.Value;
                    try
                    {
                        priorizados.Sort(
                        delegate (Nodo n1, Nodo n2)
                        {
                            return int.Parse(n1.element.Priority).CompareTo(int.Parse(n2.element.Priority));
                        }
                        );
                    }
                    catch (Exception)
                    {

                    }
                }
            }

            Nodo nodoAnterior = null;

            foreach( Nodo nodo in priorizados)
            {
                if(nodoAnterior != null)
                {
                    nodo.parentId = nodoAnterior.element.ElementID;
                }
            }
        }

        private List<Nodo> buildTree(List<Nodo> nodes)
        {
            var nodeMap = nodes.ToDictionary(node => node.id);
            var rootNodes = new List<Nodo>();
            foreach (var node in nodes)
            {
                Nodo parent;
                if (nodeMap.TryGetValue(node.parentId, out parent))
                    parent.children.Add(node);
                else
                    rootNodes.Add(node);
            }
            return rootNodes;
        }
    }

    public class Nodo
    {
        public Element element;
        public int     id;
        public int     parentId    = -1;
        public bool    esTarea     = false;
        public bool    esRequisito = false;
        public bool    programar   = true;

        public List<Nodo> children = new List<Nodo>();

        public Nodo( Element element, int parentId )
        {
            this.element     = element;
            this.parentId    = parentId;
            this.id          = element.ElementID;
            this.esTarea     = element.Type == "Task";
            this.esRequisito = element.Type == "Requirement";
        }
        public Nodo(Element element)
        {
            this.element = element;
            this.id = element.ElementID;
            this.esTarea = element.Type == "Task";
            this.esRequisito = element.Type == "Requirement";
        }
    }
}
