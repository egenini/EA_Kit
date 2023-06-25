using EAUtils.framework2.domain;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAUtils.framework2.json
{
    public class FilePreference : JsonableCommon

    {
        public const string tvValue = @"{"
                        + "\r\n"
                        + "    \"file_name\" : \"\",\r\n"
                        + "    \"write\" : true ,\r\n"
                        + "    \"__write\" :\"permite elegir no generar este archivo si se pone en false\",\r\n"
                        + "    \"paste_clipboard\" : false ,\r\n"
                        + "    \"__paste_clipboard\" :\"Pega en el portapapeles el resultado de la plantilla.\",\r\n"
                        + "    \"__paste_clipboard__write\" :\"Si ambos tienen valor false se ignora este activo\",\r\n"
                        + "    \"always_show_file_dialog\" : false ,\r\n"
                        + "    \"__always_show_file_dialog\" :\"si es true abre siempre el cuadro de diálogo\",\r\n"
                        + "    \"one_time\" : false ,\r\n"
                        + "    \"__one_time\" :\"Sólo guarda el archivo si no existe y luego no lo guarda nunca más\",\r\n"
                        + "    \"always\" : true ,\r\n"
                        + "    \"__always\" :\"Guarda sin preguntar si quiere guardar o no\"\r\n"
                        + "}\r\n";

        private JObject dialectJson = null;
        public DialectInfo dialectInfo;
        public bool needUpdateTaggedValue = false;

        public FilePreference(DialectInfo dialectInfo)
        {
            this.dialectInfo = dialectInfo;
        }

        public bool updateFileName(Artifact artifact)
        {
            bool updated = false;
            string currentValue;
            string newValue;
            if (this.dialectJson != null)
            {
                JToken token = this.dialectJson.SelectToken(artifact.name);
                if (token != null)
                {
                    currentValue = (string)this.dialectJson[artifact.name]["file_name"];
                    newValue     = artifact.saveFileInfo.fileName();

                    if (currentValue != null && currentValue == "" && newValue != null && newValue != "" && currentValue != newValue)
                    {
                        this.dialectJson[artifact.name]["file_name"] = newValue;
                        updated                                      = true;
                        this.needUpdateTaggedValue                   = true;
                    }
                }
            }
            return updated;
        }

        public bool fill(Artifact artifact)
        {
            bool canFill = false;

            if (this.dialectJson != null)
            {
                JToken token = this.dialectJson.SelectToken(artifact.name);
                if( token != null)
                {
                    try
                    {
                        artifact.saveFileInfo.fileName( (string)this.dialectJson[artifact.name]["file_name"]);
                    }
                    catch (Exception) { }
                    try
                    {
                        artifact.saveFileInfo.write = (bool)this.dialectJson[artifact.name]["write"];
                    }
                    catch (Exception) { }
                    try
                    {
                        artifact.saveFileInfo.showFileDialogAlways = (bool)this.dialectJson[artifact.name]["always_show_file_dialog"];
                    }
                    catch (Exception) { }
                    try
                    {
                        artifact.saveFileInfo.saveAlways = (bool)this.dialectJson[artifact.name]["always"];
                    }
                    catch (Exception) { }
                    try
                    {
                        artifact.saveFileInfo.oneTime = (bool)this.dialectJson[artifact.name]["one_time"];
                    }
                    catch (Exception) { }
                    try
                    {
                        artifact.saveFileInfo.pasteClipboard = (bool)this.dialectJson[artifact.name]["paste_clipboard"];
                    }
                    catch (Exception) { }

                    canFill = true;
                }
            }
            return canFill;
        }
        public void parse(string jsonString)
        {
            dialectJson = JObject.Parse(jsonString);
            dynamic token;
            // sólo interesa si está 
            foreach (ArtifactInfo info in dialectInfo.artifacts)
            {
                token = dialectJson.SelectToken(info.name);
                if (token == null)
                {
                    dialectJson[info.name] = JObject.Parse(tvValue);
                }
            }
        }

        public string stringfity()
        {
            string json = "{}";

            if (this.dialectJson != null)
            {
                json = this.dialectJson.ToString();
            }
            else
            {
                this.start();

                foreach (ArtifactInfo info in dialectInfo.artifacts)
                {
                    this.writer.WritePropertyName(info.name);
                    this.writer.WriteRawValue(tvValue);
                }

                json = this.end();
            }

            return json;
        }
    }


}
