using EA;
using EAUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Productividad.classtable
{
    public class Attribute2Column
    {
        EAUtils.EAUtils eaUtils;
        Element table;
        public Element classOwner = null;
        public Framework framework = null;
        //public bool isPrimaryKey = false;

        public Attribute2Column(Repository repository, Element table )
        {
            this.table = table;

            this.eaUtils = new EAUtils.EAUtils();
            this.eaUtils.setRepositorio(repository);
            framework = new Framework(this.eaUtils);
        }

        public void toColumn( EA.Attribute attribute )
        {
            if( framework == null)
            {
                framework = new Framework(this.eaUtils);
            }

            string columnName;
            string precision;
            string scale;
            string type;

            if ( attribute.Stereotype == "column")
            {
                columnName = attribute.Name;
                precision  = attribute.Type.Contains("char") ? attribute.Length : attribute.Precision;
                scale      = attribute.Scale;
                type       = attribute.Type;
            }
            else
            {
                this.setClassOwner(attribute);

                DataTypeInfo dataTypeInfo = eaUtils.dataTypeUtils.getFrom(this.classOwner.Gentype, attribute.Type, "SQL");

                columnName         = buildColumnName(attribute.Name);
                Element classOwner = eaUtils.repository.GetElementByID(attribute.ParentID);
                precision          = eaUtils.taggedValuesUtils.getPrecision(attribute, "").asString();
                scale              = eaUtils.taggedValuesUtils.getScale(attribute, "").asString();
                type               = getColumnType(attribute, dataTypeInfo);

                if ((precision == "" || precision == "0") && dataTypeInfo != null)
                {
                    precision = dataTypeInfo.ddl.defaultLength;
                }
                if ((scale == "" || scale == "0") && dataTypeInfo != null)
                {
                    scale = dataTypeInfo.ddl.defaultScale;
                }
            }

            EA.Attribute column = getColumn(columnName);

            if ( column == null )
            {
                addColumn( columnName, type, precision , scale, attribute );
            }
            else
            {
                updateColumn(column, type, precision, scale, attribute);
            }
        }

        internal void toColumn(AttributeInfo attrinfo)
        {
            // si un atributo es por relación es una fk para esta tabla.
            // todo: ver el tema de fk.
           if( ! attrinfo.isByRelation )
            {
                toColumn(attrinfo.attribute);
            }
        }

        public EA.Attribute addColumn( string name, string type, string precision, string scale, EA.Attribute attribute)
        {
            EA.Attribute column;

            column            = table.Attributes.AddNew(name, "");
            column.Stereotype = "column";

            // en las enumeraciones no hay un attribute
            if ( attribute != null)
            {
                column.Notes = attribute.Notes;
                column.Alias = attribute.Alias;
            }

            updateColumn( column, type, precision, scale, attribute );

            return column;
        }

        public void updateColumn(EA.Attribute column, string type, string precision, string scale, EA.Attribute attribute)
        {
            column.Type = type;

            if (precision != null && precision != "" && precision != "0")
            {
                column.Length = precision;
            }

            if (scale != null && scale != "" && scale != "0")
            {
                column.Scale = scale;
            }

            if (attribute != null)
            {
                setDefaulValue(column, attribute);

                column.LowerBound = attribute.LowerBound;
                column.UpperBound = attribute.UpperBound;

                if (attribute.Stereotype == "column")
                {
                    // es nuleable?
                    column.AllowDuplicates = attribute.AllowDuplicates;
                    // es clave primaria?
                    column.IsOrdered = attribute.IsOrdered;
                }
                else
                {
                    column.AllowDuplicates = this.isNullable(attribute);
                }
            }

            column.Update();

            // esto tiene que ir despues del update porque cuando es nuevo aún no se creó en la base.
            if (attribute != null)
            {
                eaUtils.taggedValuesUtils.set(column, "source.guid", attribute.AttributeGUID);

                if(column.IsOrdered)
                {
                    buildPrimaryKey(column);
                }
            }

            // clonamos los tagged values que correspondan.
            this.eaUtils.taggedValuesUtils.setExtends(attribute, column);
        }

        private void setDefaulValue(EA.Attribute column, EA.Attribute attribute)
        {
            if(column.Type.ToLower().Contains("char"))
            {
                if(attribute.Default != "")
                {
                    column.Default = (attribute.Default.StartsWith("'") ? "" : "'") + attribute.Default + (attribute.Default.EndsWith("'") ? "" : "'");
                }
                else
                {
                    column.Default = "";
                }
            }
            else
            {
                column.Default = attribute.Default;
            }
        }

        public void buildPrimaryKey(EA.Attribute column)
        {
            EA.Method method = null;

            foreach( EA.Method currentMethod in table.Methods)
            {
                if( currentMethod.Stereotype == "PK")
                {
                    method = currentMethod;
                    break;
                }
            }

            if( method == null )
            {
                method = table.Methods.AddNew("PK_" + table.Name, "");
                method.Stereotype = "PK";
                method.Update();
            }

            Parameter parameter = null;

            if( method.Parameters.Count != 0)
            {
                parameter = method.Parameters.GetAt(0);
                parameter.Type = column.Type;
            }

            if (parameter == null)
            {
                parameter = method.Parameters.AddNew(column.Name, column.Type);
            }

            parameter.Update();
        }

        private String getColumnType( EA.Attribute attribute, DataTypeInfo dataTypeInfo)
        {
            string newType = null;

            if( dataTypeInfo != null)
            {
                newType = dataTypeInfo.name;
            }

            if ( newType == null )
            {
                newType = "varchar";
            }
            return newType;
        }

        public EA.Attribute getColumn( string columnName)
        {
            EA.Attribute column = null;

            foreach ( EA.Attribute current in table.Attributes )
            {
                if( current.Name == columnName )
                {
                    column = current;
                }
            }
            return column;
        }

        private void setClassOwner( EA.Attribute attribute )
        {
            if( this.classOwner == null || this.classOwner.ElementID != attribute.ParentID)
            {
                classOwner = eaUtils.repository.GetElementByID( attribute.ParentID );
            }
        }

        public string buildColumnName(string attributeName)
        {
            return framework.toCase(attributeName);
        }

        private bool isNullable( EA.Attribute attribute)
        {
            bool isNullable = false;

            TaggedValueWrapper tv = this.eaUtils.taggedValuesUtils.getRequired(attribute, null);

            if( tv.value != null)
            {
                isNullable = tv.asBoolean().Value;
            }

            return isNullable;
        }
    }
}
