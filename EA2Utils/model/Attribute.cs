using EA;
using EAUtils.entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// Este es el nuevo modelo para serializar a json.
/// 
/// </summary>
namespace EAUtils.model
{
    public class Attribute
    {
        [JsonIgnore]
        public EA.Attribute eaAttribute;

        public string Guid { set; get; }
        public string Name { set; get; }
        public string Alias { set; get; }
        public string Notes { set; get; }
        public string DataType { set; get; }
        public string DefaultValue { set; get; }

        public string DataTypeClass { set; get; }
        public string DataTypeNamespace { set; get; }

        // extendidos por valores etiquetados
        public bool? RealPk { set; get; }
        public bool? Search { set; get; } = false;
        public bool? Required { set; get; } = false;
        public bool? Encrypted { set; get; } = false;
        public int? Precision { set; get; }
        public int? Scale { set; get; }
        public string DomainValues { set; get; }
        public string Example { set; get; }

        public Multiplicity multiplicity;

        public Column Column { set; get; }

        public Attribute( EA.Attribute eaAttribute, EAUtils eaUtils)
        {
            if( eaAttribute.Stereotype == "column")
            {
                Column = new Column(eaAttribute, eaUtils);

                string guid = eaUtils.taggedValuesUtils.get(eaAttribute, "source.guid", "").asString();

                if( guid != "")
                {
                    eaAttribute = eaUtils.repository.GetAttributeByGuid(guid);
                }
            }

            this.eaAttribute = eaAttribute;

            this.Alias = eaAttribute.Alias;
            this.Name = eaAttribute.Name;
            this.Notes = eaUtils.notes2Txt(eaAttribute.Notes);
            this.DataType = eaAttribute.Type;
            this.DefaultValue = eaAttribute.Default;

            this.multiplicity = new Multiplicity(eaAttribute, eaUtils);

            // desde valores etiquetados
            this.RealPk = eaUtils.taggedValuesUtils.getRealPrimaryKey(eaAttribute, "false").asBoolean();
            this.Search = eaUtils.taggedValuesUtils.getSearch(eaAttribute, "false").asBoolean();
            this.Required = eaUtils.taggedValuesUtils.getRequired(eaAttribute, "false").asBoolean();
            this.Encrypted = eaUtils.taggedValuesUtils.getEncrypted(eaAttribute, "false").asBoolean();
            this.Precision = eaUtils.taggedValuesUtils.getPrecision(eaAttribute, "0").asInt();
            this.Scale = eaUtils.taggedValuesUtils.getScale(eaAttribute, "0").asInt();
            this.DomainValues = eaUtils.notes2Txt(eaUtils.taggedValuesUtils.getDomainValues(eaAttribute, "").asString());
            this.Example = eaUtils.taggedValuesUtils.getExample(eaAttribute, "").asString();

        }
    }
}
