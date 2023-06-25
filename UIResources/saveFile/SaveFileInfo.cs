using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIResources.saveFile
{
    public class SaveFileInfo
    {
        private string initialDir = @"C:\";
        private string fName;
        private string defaultExt = "Java"; // "java"
        private string pth;
        private string fltr = ""; // "Fuente Java (*.java)|*.java"

        public SaveFileInfo initialDirectory( string initialDirectory)
        {
            this.initialDir = initialDirectory;

            return this;
        }
        public string initialDirectory()
        {
            return this.initialDir;
        }

        public SaveFileInfo fileName( string fileName)
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
        public SaveFileInfo defaultExtension( string defaultExtension)
        {
            this.defaultExt = defaultExtension;
            return this;
        }
        public string defaultExtension()
        {
            return this.defaultExt;
        }
        public SaveFileInfo path( string path)
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
        public SaveFileInfo filter( string filter)
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
