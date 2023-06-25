using EA;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAUtils.framework2.json
{
    public class TemplateInfo : JsonableCommon
    {
        public List<TemplateReplace> templates = new List<TemplateReplace>();
        public PackageReplace package = new PackageReplace();

        public void add(Element template)
        {
            package.name = "";
            package.guid = "";

            templates.Add(new TemplateReplace(template.FQName, (template.Alias != "" ? template.Alias : template.Name), template.ElementGUID));
        }

        public void add( Package packageTemplate)
        {
            templates.Clear();

            package.name = packageTemplate.Name;
            package.guid = packageTemplate.PackageGUID;
        }

        public void parse( string jsonString )
        {
            try
            {
                dynamic replaceInfoJson = JObject.Parse(jsonString);

                try
                {
                    foreach (var templateJson in replaceInfoJson.templates)
                    {
                        templates.Add(new TemplateReplace((string)templateJson.name, (string)templateJson.replace, (string)templateJson.guid));
                    }
                }
                catch (Exception e) { e.ToString(); }

                try
                {
                    package.name = (string)replaceInfoJson.package.name;
                    package.guid = (string)replaceInfoJson.package.guid;
                }
                catch (Exception e) { e.ToString(); }
            }
            catch (Exception e) { e.ToString(); }
        }

        public string stringfity()
        {
            this.start();

            if( package.isEmpty())
            {
                this.writer.WritePropertyName("templates");

                this.writer.WriteStartArray();

                foreach (TemplateReplace template in templates)
                {
                    this.writer.WriteStartObject();

                    this.writer.WritePropertyName("name");
                    this.writer.WriteValue(template.name);

                    this.writer.WritePropertyName("replace");
                    this.writer.WriteValue(template.replace);

                    this.writer.WritePropertyName("guid");
                    this.writer.WriteValue(template.guid);

                    this.writer.WriteEnd();
                }

                this.writer.WriteEndArray();
            }
            else
            {
                this.writer.WritePropertyName("package");
                this.writer.WriteStartObject();

                this.writer.WritePropertyName("name");
                this.writer.WriteValue(this.package.name);

                this.writer.WritePropertyName("guid");
                this.writer.WriteValue(this.package.guid);

                this.writer.WriteEnd();
            }

            return this.end();
        }
    }

    public class PackageReplace : ReplaceCommon
    {
        public bool isEmpty()
        {
            return this.guid == null || this.guid == "";
        }
    }

    public class TemplateReplace : ReplaceCommon
    {
        public string replace = "";

        public TemplateReplace(string name, string replace, string guid)
        {
            this.name = name;
            this.replace = replace;
            this.guid = guid;
        }
    }

    public abstract class ReplaceCommon
    {
        public string name = "";
        public string guid = "";
    }
}
