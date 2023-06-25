using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIResources.saveFile;

namespace EAUtils.framework2
{
    public class JsonableCommon
    {
        protected JsonWriter writer = null;
        private StringBuilder sb = new StringBuilder();
        private StringWriter sw;

        public void start()
        {
            sw = new StringWriter(sb);
            writer = new JsonTextWriter(sw);

            writer.Formatting = Newtonsoft.Json.Formatting.Indented;

            writer.WriteStartObject();
        }

        public string end()
        {
            writer.WriteEnd();

            return sw.ToString();
        }
    }
}
