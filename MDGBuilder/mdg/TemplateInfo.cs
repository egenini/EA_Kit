using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDGBuilder.mdg
{
    public class TemplateInfo
    {
        public string name;
        public string description;
        /// <summary>
        /// El lugar donde el va a buscar el xml que contiene al template.
        /// Se debe agregar el nombre del XML o una ruta relativa más el nombre del xml.
        /// </summary>
        public string location = "";
        /// <summary>
        /// Puede tomar 3 valores:
        /// "true": considerado "Framework", implica que sólo se podrá instanciar 1 sóla vez en el repositorio.
        /// "optional": le preguntará al usuario si quiere usarlo como framework o crearlo con GUIDs nuevos.
        /// "false": instancia el template con GUIDs nuevos.
        /// Defalut="false".
        /// </summary>
        public string isFramework = "false";
    }
}
