using DMN.framework;
using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DMN
{
    /// <summary>
    /// Establece la elegibilidad de los elementos sea 
    /// para habilitar opciones de menú o para ejecutar alguna acción sobre ellos
    /// </summary>
    public class MainUtils
    {
        public static string MENU_LOCATION__TREEVIEW  = "TreeView";
        public static string MENU_LOCATION__MAIN_MENU = "MainMenu";
        public static string MENU_LOCATION__DIAGRAM   = "Diagram";

        public Framework framework = null;

        public Element getVariableSelected(Repository repository, string location)
        {
            Element selected = null;
            Element current = null;

            if (location == MENU_LOCATION__TREEVIEW)
            {
                ObjectType ot = repository.GetTreeSelectedItemType();
                if (ot.Equals(ObjectType.otElement))
                {
                    current = (Element)repository.GetTreeSelectedObject();

                    if( current.Type == "ActivityParameter")
                    {
                        if( repository.GetElementByID(current.ParentID).GetDecisionTable().Length != 0)
                        {
                            selected = current;
                        }
                    }
                }
            }
            return selected;

        }
        public Element getBusinessKnowledgeSelected(Repository repository, string location)
        {
            Element selected = null;
            Element current = null;

            if (location == MENU_LOCATION__TREEVIEW )
            {
                ObjectType ot = repository.GetTreeSelectedItemType();
                if (ot.Equals(ObjectType.otElement))
                {
                    current = (Element)repository.GetTreeSelectedObject();
                }
            }
            else
            {
                ObjectType ot = repository.GetContextItemType();
                if (ot.Equals(ObjectType.otElement))
                {
                    current = (Element)repository.GetContextObject();
                }
            }

            if (current != null && current.Stereotype == "BusinessKnowledge")
            {
                selected = current;
            }
            return selected;
        }

        public Element getDecisionSelected(Repository repository, string location)
        {
            Element selected = null;
            Element current = null;

            if (location ==  MENU_LOCATION__TREEVIEW)
            {
                ObjectType ot = repository.GetTreeSelectedItemType();
                if (ot.Equals(ObjectType.otElement))
                {
                    current = (Element)repository.GetTreeSelectedObject();
                }
            }
            else
            {
                ObjectType ot = repository.GetContextItemType();
                if (ot.Equals(ObjectType.otElement))
                {
                    current = (Element)repository.GetContextObject();
                }
            }

            if (current != null && current.Stereotype == "Decision")
            {
                selected = current;
            }
            return selected;
        }

        public Element getSelected(Repository repository, string location, string type, string stereotype)
        {
            Element selected = null;
            Element current = null;

            if (location == "TreeView")
            {
                ObjectType ot = repository.GetTreeSelectedItemType();
                if (ot.Equals(ObjectType.otElement))
                {
                    current = (Element)repository.GetTreeSelectedObject();
                }
            }
            else
            {
                ObjectType ot = repository.GetContextItemType();
                if (ot.Equals(ObjectType.otElement))
                {
                    current = (Element)repository.GetContextObject();
                }
            }

            if (current != null && (type == null || ( type != null && type == current.Type )) && (stereotype == null || (stereotype != null && current.Stereotype == stereotype)) )
            {
                selected = current;
            }
            return selected;
        }
    }
}
