﻿using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestFul
{
    /// <summary>
    /// Establece la elegibilidad de los elementos sea 
    /// para habilitar opciones de menú o para ejecutar alguna acción sobre ellos
    /// </summary>
    public class MainUtils
    {
        public static string MENU_LOCATION__TREEVIEW = "TreeView";
        public static string MENU_LOCATION__MAIN_MENU = "MainMenu";
        public static string MENU_LOCATION__DIAGRAM = "Diagram";

        public Element getSwaggerClassSelected(Repository repository, string location)
        {
            Element selected = null;
            Element current = null;

            if (location == MENU_LOCATION__TREEVIEW)
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

            if (current != null && current.Name == "Swagger")
            {
                selected = current;
            }
            return selected;
        }


        public Element resourceSelected(Repository repository, string location)
        {
            Element selected = null;
            Element current = null;

            if (location == MENU_LOCATION__TREEVIEW)
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

            if (current != null && current.Stereotype == "Resource")
            {
                selected = current;
            }
            return selected;
        }
    }
}
