using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAUtils
{
    class JsonBuilder
    {
        StringBuilder sb;
        StringWriter sw;
        JsonWriter writer;
        bool printNotes = false;
        short toCase = StringUtils.NONE;

        public string build(RealClass realClass, bool printNotes)
        {
            return this.build(realClass, printNotes, StringUtils.NONE);
        }

        public string build( RealClass realClass, bool printNotes, short toCase )
        {
            this.printNotes = printNotes;
            this.toCase = toCase;

            sb     = new StringBuilder();
            sw     = new StringWriter(sb);
            writer = new JsonTextWriter(sw);

            writer.Formatting = Newtonsoft.Json.Formatting.Indented;

            //writer.WriteStartObject();

            build(realClass);

            //writer.WriteEnd();

            return sw.ToString();
        }

        private void build( RealClass realClass )
        {
            writer.WriteStartObject();

            build(realClass.getAttributesInfo());

            foreach (RealClass inner in realClass.innerClasses)
            {
                build(inner);
            }

            writer.WriteEnd();
        }
        private void build(List<AttributeInfo> attributesInfo)
        {
            if (attributesInfo.Count != 0)
            {
                foreach (AttributeInfo attributeInfo in attributesInfo)
                {
                    writer.WritePropertyName( StringUtils.to( this.toCase, attributeInfo.name));

                    if (attributeInfo.cardinality != null && attributeInfo.cardinality.isCollection())
                    {
                        writer.WriteStartArray();
                    }

                    if (attributeInfo.relationClass != null )
                    {

                        build(attributeInfo.relationClass);

                    }
                    else
                    {
                        if (attributeInfo.cardinality == null || ! attributeInfo.cardinality.isCollection())
                        {
                            writer.WriteValue(attributeInfo.value);
                        }

                        if (this.printNotes && attributeInfo.attribute != null && attributeInfo.attribute.Notes.Length != 0)
                        {
                            writer.WriteComment(attributeInfo.attribute.Notes);
                        }
                    }

                    if (attributeInfo.cardinality != null && attributeInfo.cardinality.isCollection())
                    {
                        writer.WriteEndArray();
                    }

                }
            }
        }
    }
}
