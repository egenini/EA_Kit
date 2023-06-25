using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using OpenAPI.parser;
using OpenAPI.generar;
using EA;

namespace OpenAPI
{
    public class Main : MainCommons
    {
        public void EA_MenuClick(EA.Repository repository, string location, string menuName, string itemName)
        {
            if (itemName == MENU_ITEM_CARGAR_ESPECIFICACION)
            {
                new Sincronizar(this.eaUtils).sincronizar();
            }
            else if (itemName == MENU_ITEM_GENERAR_OPENAPI)
            {
                new Generar(this.eaUtils).generar();
            }
        }
    }
}