using EAUtils.framework2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAUtils.entity
{
    public class AttributeBase : JsonableCommon
    {
        public EA.Attribute eaAttribute;
        // atributos estándar
        public string alias;
        public string name;
        public string notes;
        public string dataType;
        public string defaultValue;
        public string example;

        public Multiplicity multiplicity;

        // Si el tipo es una clase
        public string dataTypeClass;
        public string dataTypeNamespace;

        // extendidos por valores etiquetados
        public bool? realPk;
        public bool? search = false;
        public bool? required;
        public bool? encrypted = false;
        public int? precision;
        public int? scale;
        public string domainValues;

        public AttributeBase(EA.Attribute eaAttribute, EAUtils eautils)
        {
            this.eaAttribute = eaAttribute;

            this.alias = eaAttribute.Alias;
            this.name = eaAttribute.Name;
            this.notes = eautils.notes2Txt( eaAttribute.Notes );
            this.dataType = eaAttribute.Type;
            this.defaultValue = eaAttribute.Default;

            this.multiplicity = new Multiplicity(eaAttribute, eautils);

            // desde valores etiquetados
            this.realPk = eautils.taggedValuesUtils.getRealPrimaryKey(eaAttribute, "false").asBoolean();
            this.search = eautils.taggedValuesUtils.getSearch(eaAttribute, "false").asBoolean();
            this.required = eautils.taggedValuesUtils.getRequired(eaAttribute, "false").asBoolean();
            this.encrypted = eautils.taggedValuesUtils.getEncrypted(eaAttribute, "false").asBoolean();
            this.precision = eautils.taggedValuesUtils.getPrecision(eaAttribute, "0").asInt();
            this.scale = eautils.taggedValuesUtils.getScale(eaAttribute, "0").asInt();
            this.domainValues = eautils.notes2Txt( eautils.taggedValuesUtils.getDomainValues(eaAttribute, "").asString());
            this.example = eautils.taggedValuesUtils.getExample(eaAttribute, "").asString();
        }

        public AttributeBase()
        {
        }

        public AttributeBase parse(dynamic jsonPart, EAUtils eautils)
        {
            this.parseCommon(jsonPart, eautils);

            return this;

        }

        public void parseCommon(dynamic jsonPart, EAUtils eautils)
        {
            this.alias = jsonPart.alias;
            this.dataType = jsonPart.dataType;
            this.domainValues = jsonPart.domainValues;
            this.eaAttribute = eautils.repository.GetAttributeByGuid(jsonPart.guid);
            this.name = jsonPart.name;
            this.notes = jsonPart.notes;

            this.dataTypeClass = jsonPart.dataTypeClass;
            this.dataTypeNamespace = jsonPart.dataTypeNamespace;

            this.precision = jsonPart.precision;
            this.realPk = jsonPart.realPk;
            this.search = jsonPart.search;
            this.required = jsonPart.required;
            this.scale = jsonPart.scale;
            this.search = jsonPart.search;
            this.encrypted = jsonPart.encrypted;
        }

        public void stringfityProperties()
        {

            writer.WritePropertyName("guid");
            writer.WriteValue(this.eaAttribute.AttributeGUID);

            writer.WritePropertyName("alias");
            writer.WriteValue(this.alias);

            writer.WritePropertyName("name");
            writer.WriteValue(this.name);

            writer.WritePropertyName("notes");
            writer.WriteValue(this.notes);

            writer.WritePropertyName("dataType");
            writer.WriteValue(this.dataType);

            writer.WritePropertyName("defaultValue");
            writer.WriteValue(this.defaultValue);

            writer.WritePropertyName("example");
            writer.WriteValue(this.example);

            writer.WritePropertyName("dataTypeClass");
            writer.WriteValue(this.dataTypeClass);

            writer.WritePropertyName("dataTypeNamespace");
            writer.WriteValue(this.dataTypeNamespace);


            writer.WritePropertyName("search");
            writer.WriteValue(this.search);

            writer.WritePropertyName("realPk");
            writer.WriteValue(this.realPk);

            writer.WritePropertyName("required");
            writer.WriteValue(this.required);

            writer.WritePropertyName("encrypted");
            writer.WriteValue(this.encrypted);

            writer.WritePropertyName("precision");
            writer.WriteValue(this.precision);

            writer.WritePropertyName("scale");
            writer.WriteValue(this.scale);

            writer.WritePropertyName("domainValues");
            writer.WriteValue(this.domainValues);

            this.multiplicity.stringfity(this.writer);
        }

        public void stringfity(JsonWriter writer)
        {
            this.writer = writer;
            this.stringfityProperties();

        }

        public string stringfity()
        {
            this.stringfityStart();

            this.stringfityProperties();

            return this.strinfityEnd();
        }

        public void stringfityStart()
        {
            this.start();
        }

        public string strinfityEnd()
        {
            return this.end();
        }
    }

    public class Multiplicity
    {
        public int lowerBound;
        public int upperBound;
        public bool isLowerBoundMany = false;
        public bool isUpperBoundMany = false;

        public Multiplicity(EA.Attribute attribute, EAUtils eautils)
        {
            if(attribute.LowerBound.Contains("*"))
            {
                isLowerBoundMany = true;
            }
            else
            {
                int.TryParse(attribute.LowerBound, out lowerBound);
            }

            if (attribute.UpperBound.Contains("*"))
            {
                isUpperBoundMany = true;
            }
            else
            {
                int.TryParse(attribute.UpperBound, out upperBound);
            }
        }
        public void stringfity(JsonWriter writer)
        {
            writer.WritePropertyName("multiplicity");
            writer.WriteStartObject();

            writer.WritePropertyName("lowerBound");
            writer.WriteValue(this.lowerBound);
            writer.WritePropertyName("isLowerBoundMany");
            writer.WriteValue(this.isLowerBoundMany);

            writer.WritePropertyName("upperBound");
            writer.WriteValue(this.upperBound);
            writer.WritePropertyName("isUpperBoundMany");
            writer.WriteValue(this.isUpperBoundMany);

            writer.WriteEnd();
        }
    }
}
