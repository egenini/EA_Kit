using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Productividad
{
    /// <summary>
    /// Responsable por el comportamiento básico de todo un addin.
    /// </summary>
    public class MainCommons : MainUtils
    {
        const string addinName = "Productividad";

        const string menuHeader = "-&" + addinName;

        protected const string ITEM_MENU__VER_MENSAJE_AS_JSON = "Ver como JSON";
        protected const string ITEM_MENU__VER_MENSAJE_AS_XML = "Ver como XML";
        protected const string ITEM_MENU__GENERAR_CODIGO = "Generar código enum en el porta papeles";
        protected const string ITEM_MENU__GENERAR_CODIGO_CLASS = "Generar código class en el porta papeles";
        //protected const string ITEM_MENU__ESTABLECER_TIPO_ENUM = "Establecer el mismo tipo para todos (toma el primero como referencia)";
        protected const string ITEM_MENU__INSTANCIAR_DESDE_CONECTOR = "Instanciar desde conector";
        //protected const string ITEM_MENU__GENERAR_DAO = "Generar DAO";
        //protected const string ITEM_MENU__GENERAR_POJO = "Generar Pojo";
        //protected const string ITEM_MENU__GENERAR_POJO_METADATA = "Generar Pojo metadata";
        protected const string ITEM_MENU__ETIQUETAS = "Generar etiquetas";
        //protected const string ITEM_MENU__TEST_JIRA = "Test jira login";
        protected const string ITEM_MENU__JSON_2_CLASS = "JSON to Class";
        protected const string ITEM_MENU__MOVE_ELEMENT_TO_ACTIVE_PACKAGE = "Mover el elelento al paquete del diagrama";

        protected EAUtils.JsonUtils jutils;
        protected EAUtils.EAUtils eaUtils;

        public String EA_Connect(EA.Repository Repository)
        {
            // No special processing req'd

            return "Productividad";
        }
        bool IsProjectOpen(EA.Repository Repository)
        {
            try
            {
                EA.Collection c = Repository.Models;
                eaUtils = new EAUtils.EAUtils();
                eaUtils.setRepositorio(Repository);
                eaUtils.createOutput();
                jutils = new EAUtils.JsonUtils(eaUtils);

                return true;
            }
            catch
            {
                return false;
            }
        }
        public void EA_ShowHelp(EA.Repository Repository, string Location, string MenuName, string ItemName)
        {
            MessageBox.Show("Help for: " + MenuName + "/" + ItemName);
        }

        public object EA_GetMenuItems(EA.Repository Repository, string Location, string MenuName)
        {
            /* nb example of out parameter:
			object item;
			EA.ObjectType t = Repository.GetTreeSelectedItem(out item);
			EA.Package r = (EA.Package) item;
			*/

            switch (MenuName)
            {
                case "":
                    return menuHeader;
                case menuHeader:
                    string[] ar = { ITEM_MENU__MOVE_ELEMENT_TO_ACTIVE_PACKAGE, ITEM_MENU__JSON_2_CLASS, ITEM_MENU__GENERAR_CODIGO_CLASS, ITEM_MENU__VER_MENSAJE_AS_JSON, ITEM_MENU__VER_MENSAJE_AS_XML, ITEM_MENU__GENERAR_CODIGO, ITEM_MENU__INSTANCIAR_DESDE_CONECTOR, /*ITEM_MENU__GENERAR_DAO, ITEM_MENU__GENERAR_POJO, ITEM_MENU__GENERAR_POJO_METADATA,*/ ITEM_MENU__ETIQUETAS };
                    return ar;
            }
            return "";
        }

        public void EA_GetMenuState(EA.Repository repository, string location, string MenuName, string ItemName, ref bool IsEnabled, ref bool IsChecked)
        {
            if (IsProjectOpen(repository))
            {
                bool tableCondition = this.getTableSelected(repository) != null;
                bool classCondition = this.getClassSelected(repository) != null;

                if (ItemName == ITEM_MENU__VER_MENSAJE_AS_JSON)
                    IsEnabled = true;
                else if (ItemName == ITEM_MENU__VER_MENSAJE_AS_XML)
                    IsEnabled = true;
                else if (ItemName == ITEM_MENU__GENERAR_CODIGO)
                {
                    IsEnabled = this.getEnumSelected(repository, location) != null;
                }
                else if (ItemName == ITEM_MENU__INSTANCIAR_DESDE_CONECTOR)
                {
                    IsEnabled = this.getConnectorInstantiationSelected(repository) != null;
                }
                /*
                else if (ItemName == ITEM_MENU__GENERAR_DAO)
                {
                    IsEnabled = tableCondition;
                }
                else if (ItemName == ITEM_MENU__GENERAR_POJO)
                {
                    IsEnabled = tableCondition;
                }
                else if (ItemName == ITEM_MENU__GENERAR_POJO_METADATA)
                {
                    IsEnabled = tableCondition;
                }
                */
                else if (ItemName == ITEM_MENU__ETIQUETAS)
                {
                    IsEnabled = classCondition;
                }
                /*
                else if (ItemName == ITEM_MENU__TEST_JIRA)
                {
                    IsEnabled = true;
                }
                */
                else if (ItemName == ITEM_MENU__JSON_2_CLASS)
                {
                    IsEnabled = classCondition;
                }
                else if (ItemName == ITEM_MENU__GENERAR_CODIGO_CLASS)
                {
                    IsEnabled = classCondition;
                }
                else if (ItemName == ITEM_MENU__MOVE_ELEMENT_TO_ACTIVE_PACKAGE)
                {
                    IsEnabled = this.getElementSelected(repository) != null;
                }
            }
            else
                // If no open project, disable all menu options
                IsEnabled = false;
        }

        /*
        * http://sparxsystems.com/enterprise_architect_user_guide/13.0/automation/ea_getribboncategory.html
        */
        /*
        public string EA_GetRibbonCategory(Repository repository)
        {
            return "DISEÑAR";
        }
        */

        // http://sparxsystems.com/enterprise_architect_user_guide/13.0/automation/ea_onretrievemodeltemplate.html

        public string EA_OnRetrieveModelTemplate(Repository repository, string location)
        {
            string template = "";

            MessageBox.Show("Plantilla solicitada " + location);

            return template;
        }

        public void EA_Disconnect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
