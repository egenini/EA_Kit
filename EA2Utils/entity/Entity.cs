using EA;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAUtils.entity
{
    public class Entity
    {
        public string package;
        public string entity;

        public List<FullAttribute> attributes = new List<FullAttribute>();

        public void setElement(Element element)
        {
            this.entity = StringUtils.toPascal(element.Name);
        }

        public void add(EA.Attribute attribute, EAUtils eaUtils)
        {
            attributes.Add(new FullAttribute(attribute, eaUtils));
        }

        public string stringfity()
        {
            List<string> uniqueImports = new List<string>();

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            JsonWriter writer = new JsonTextWriter(sw);

            writer.Formatting = Newtonsoft.Json.Formatting.Indented;
            writer.WriteStartObject();

            writer.WritePropertyName("entity");
            writer.WriteValue(this.entity);

            writer.WritePropertyName("package");
            writer.WriteValue(this.package);

            // init lista atributos
            writer.WritePropertyName("attributes");
            writer.WriteStartArray();

            writeList(writer, this.attributes);

            writer.WriteEndArray();
            // end lista atributos

            // init lista atributos pk's
            writer.WritePropertyName("realPrimaryKeys");
            writer.WriteStartArray();

            writeRealPkList(writer, this.attributes);

            writer.WriteEndArray();
            // end lista atributos pk's

            // init lista imports
            writer.WritePropertyName("imports");
            writer.WriteStartArray();

            writeImports(writer, this.attributes, uniqueImports);

            writer.WriteEndArray();
            // end lista imports

            writer.WriteEnd();

            return sw.ToString();
        }

        private void writeList(JsonWriter writer, List<FullAttribute> list)
        {
            foreach (FullAttribute ca in list)
            {
                ca.strinfity(writer);
            }
        }

        private void writeRealPkList(JsonWriter writer, List<FullAttribute> list)
        {
            foreach (FullAttribute ca in list)
            {
                if (ca.realPk != null && ca.realPk == true)
                {
                    ca.strinfity(writer);
                }
            }
        }
        private void writeImports(JsonWriter writer, List<FullAttribute> list, List<string> uniqueImports)
        {
            foreach (FullAttribute ca in list)
            {
                if (ca.package != null && !uniqueImports.Contains(ca.package))
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("import");
                    writer.WriteValue(ca.package);
                    writer.WriteEnd();

                    uniqueImports.Add(ca.package);
                }
            }
        }
        internal void update(FullAttribute fullAttribute)
        {
            FullAttribute current;
            for (short i = 0; i < this.attributes.Count; i++)
            {
                current = this.attributes[i];
                if (current.eaAttribute.AttributeGUID == fullAttribute.eaAttribute.AttributeGUID)
                {
                    this.attributes[i] = fullAttribute;
                    break;
                }
            }
        }
    }
}
