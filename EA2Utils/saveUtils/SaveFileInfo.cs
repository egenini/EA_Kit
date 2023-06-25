using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAUtils.saveUtils
{
    public class SaveFileInfo
    {
        private string initialDir = @"C:\";
        private string fName;
        private string defaultExt = "Java"; // "java"
        private string pth;
        private string fltr = ""; // "Fuente Java (*.java)|*.java"
        internal string artifactName;

        /// <summary>
        /// Si es false el artefacto no se genera.
        /// </summary>
        public bool write = true;

        /// <summary>
        /// Pega el contenido del resultado de plantilla en el portapapeles.
        /// </summary>
        public bool pasteClipboard = true;

        /// <summary>
        /// Cuando este atributo tiene valor true se abre la ventana de diálogo de guardar archivo, 
        /// caso contrario guarda directamente el archivo.
        /// Esto si saveAlways es true.
        /// </summary>
        public bool showFileDialogAlways = true;
        
        /// <summary>
        /// Guardar siempre y no preguntar (diálogo que pregunta si quiere guardar al disco o no ).
        /// </summary>
        public bool saveAlways = true;
        
        /// <summary>
        /// Este atributo cuando tiene valor false no graba a disco el archivo, es útil para guardar sólo por primera vez,
        /// por ejemplo cuando queremos crear una clase que hereda de otra y en este poder agregar manualmente lo que se necesite.
        /// Si tiene un valor false verifica si existe el archivo y lo guarda si no existe, si existe no hace nada.
        /// </summary>
        public bool oneTime = false;

        /// <summary>
        /// El contenido que se genera desde la plantilla.
        /// </summary>
        public string fileContent;

        public SaveFileInfo initialDirectory(string initialDirectory)
        {
            this.initialDir = initialDirectory;

            return this;
        }
        public string initialDirectory()
        {
            return this.initialDir;
        }

        public SaveFileInfo fileName(string fileName)
        {
            this.fName = fileName;
            return this;
        }
        public string fileName()
        {
            return this.fName;
        }

        /// <summary>
        /// por ej: Java
        /// </summary>
        /// <param name="defaultExtension"></param>
        /// <returns></returns>
        public SaveFileInfo defaultExtension(string defaultExtension)
        {
            this.defaultExt = defaultExtension;
            return this;
        }
        public string defaultExtension()
        {
            return this.defaultExt;
        }
        public SaveFileInfo path(string path)
        {
            this.pth = path;
            return this;
        }
        public string path()
        {
            return this.pth;
        }

        /// <summary>
        /// Por ej: "Fuente Java (*.java)|*.java"
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public SaveFileInfo filter(string filter)
        {
            this.fltr = filter;
            return this;
        }
        public string filter()
        {
            return this.fltr;
        }
        public void stringfity(JsonWriter writer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("path");
            writer.WriteValue(pth);

            writer.WriteEnd();
        }
    }
}
