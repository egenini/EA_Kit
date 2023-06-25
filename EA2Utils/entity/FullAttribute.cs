using EAUtils.framework2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAUtils.entity
{
    /// <summary>
    /// Dejar de usar esta clase y usar ColumnFullAttribute
    /// </summary>
    public class FullAttribute : AttributeBase, Jsonable
    {


        /// <summary>
        /// Es el namespace que le corresponde al tipo del atributo
        /// Es necesario cuando el tipo debe ser incluido en un "import"
        /// </summary>
        public string package;
        public string dataTypeAsObject;
        public string dbDataType;
        public string columnName;

        public FullAttribute(EA.Attribute eaAttribute, EAUtils eautils) : base( eaAttribute, eautils)
        {

            if(eaAttribute.Alias == "")
            {
                eaAttribute.Alias = StringUtils.toLabel(eaAttribute.Name);
                // si hago el update parece meterse en un loop, quizás sea por el foreach
                //eaAttribute.Update();
            }

            if( eaAttribute.Stereotype == "column")
            {
                this.columnName = this.name;
            }
            else
            {
                this.columnName = StringUtils.toSnake(this.name);
            }

        }

        public FullAttribute() : base()
        {
        }


        public new FullAttribute parse(dynamic jsonPart, EAUtils eautils)
        {
            this.parseCommon(jsonPart, eautils);

            this.dataTypeAsObject = jsonPart.dataTypeAsObject;
            this.package = jsonPart.package;

            this.dbDataType = jsonPart.dbDataType;
            this.columnName = jsonPart.columnName;

            return this;
        }

        /// <summary>
        /// Este método queda por compatibilidad, use stringfity
        /// </summary>
        /// <param name="writer"></param>
        public void strinfity(JsonWriter writer)
        {
            writer.WriteStartObject();

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

            writer.WritePropertyName("dataTypeAsObject");
            writer.WriteValue(this.dataTypeAsObject);

            writer.WritePropertyName("package");
            writer.WriteValue(this.package);

            writer.WritePropertyName("defaultValue");
            writer.WriteValue(this.defaultValue);

            writer.WritePropertyName("search");
            writer.WriteValue(this.search);

            writer.WritePropertyName("realPk");
            writer.WriteValue(this.realPk);

            writer.WritePropertyName("required");
            writer.WriteValue(this.required);

            writer.WritePropertyName("precision");
            writer.WriteValue(this.precision);

            writer.WritePropertyName("scale");
            writer.WriteValue(this.scale);

            writer.WritePropertyName("domainValues");
            writer.WriteValue(this.domainValues);

            writer.WritePropertyName("dbDataType");
            writer.WriteValue(this.dbDataType);

            writer.WritePropertyName("columnName");
            writer.WriteValue(this.columnName);

            writer.WriteEnd();
        }

        public new string stringfity()
        {
            this.stringfityStart();

            this.stringfityProperties();

            writer.WritePropertyName("dataTypeAsObject");
            writer.WriteValue(this.dataTypeAsObject);

            writer.WritePropertyName("package");
            writer.WriteValue(this.package);

            writer.WritePropertyName("dbDataType");
            writer.WriteValue(this.dbDataType);

            writer.WritePropertyName("columnName");
            writer.WriteValue(this.columnName);

            return this.strinfityEnd();
        }
    }
}
