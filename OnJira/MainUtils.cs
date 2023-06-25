using EA;
using System;

namespace OnJira
{
    public class MainUtils
    {
        public static string MENU_LOCATION__TREEVIEW = "TreeView";
        public static string MENU_LOCATION__MAIN_MENU = "MainMenu";
        public static string MENU_LOCATION__DIAGRAM = "Diagram";

        public Element getEnumSelected(Repository repository, string location)
        {
            return this.getSelected(repository, location, "Enumeration", null);
        }

        /// <summary>
        /// Método genérico que retorna el elemento seleccionado sea del explorador o de un diagrama.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="location"></param>
        /// <param name="type"></param>
        /// <param name="stereotype"></param>
        /// <returns></returns>
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

            if (current != null && (type == null || (type != null && type == current.Type)) && (stereotype == null || (stereotype != null && current.Stereotype == stereotype)))
            {
                selected = current;
            }
            return selected;
        }

        public Element getTableSelected( Repository repository)
        {
            ObjectType ot = repository.GetContextItemType();
            Element element = null;

            switch (ot)
            {
                case EA.ObjectType.otElement:

                    element = repository.GetContextObject();
                    if( element.Stereotype != "table")
                    {
                        element = null;
                    }
                    break;
            }
            return element;
        }

        public Element getClassSelected(Repository repository)
        {
            ObjectType ot = repository.GetContextItemType();
            Element element = null;

            switch (ot)
            {
                case EA.ObjectType.otElement:

                    element = repository.GetContextObject();
                    if (element.Type != "Class")
                    {
                        element = null;
                    }
                    break;
            }
            return element;
        }

        public Connector getConnectorInstantiationSelected(Repository repository)
        {
            Connector connector = null;
            try
            {
                connector = repository.GetCurrentDiagram().SelectedConnector;

                if( connector.Type != EAUtils.ConnectorUtils.CONNECTOR__INSTANTIATION)
                {
                    connector = null;
                }
            }
            catch (Exception )
            {
            }
            return connector;
        }
    }
}