using EA;
using EAUtils;
using Productividad.classtable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Productividad.table2class
{
    public class ColumnOrAttribute2AttributeOrColumn
    {
        const string SOURCE_GUID_KEY = "source.guid";

        EAUtils.EAUtils eaUtils;
        Element targetElement;
        private Element sourceElement = null;
        public Framework framework = null;
        bool isTargetTable = false;
        bool isSourceElementTable = false;
        string namePrefix = "";
        bool forceAddNamePrefix = false;

        Dictionary<string, EA.Attribute> columsByName = new Dictionary<string, EA.Attribute>();
        Dictionary<string, List<EA.Attribute>> columsByGuid = new Dictionary<string, List<EA.Attribute>>();

        public ColumnOrAttribute2AttributeOrColumn(Repository repository, Element element)
        {
            this.targetElement = element;
            this.isTargetTable = element.Stereotype == "table";

            this.eaUtils = new EAUtils.EAUtils();
            this.eaUtils.setRepositorio(repository);

        }

        public void setSourceElement( Element sourceElement, bool forceAddNamePrefix)
        {
            this.sourceElement = sourceElement;
            this.forceAddNamePrefix = forceAddNamePrefix;

            namePrefix = sourceElement.Name;

            string guid;
            foreach (EA.Attribute current in this.targetElement.Attributes)
            {
                if(columsByName.ContainsKey(current.Name ))
                {
                    columsByName[current.Name] = current;
                }
                else
                {
                    columsByName.Add(current.Name, current);
                }

                guid = this.eaUtils.taggedValuesUtils.get(current, SOURCE_GUID_KEY, "").asString();

                if(guid != "")
                {
                    if ( ! columsByGuid.ContainsKey(guid))
                    {
                        columsByGuid.Add(guid, new List<EA.Attribute>());
                    }
                    columsByGuid[guid].Add(current);
                }
            }
        }

        public void toColumnOrAttribute(EA.Attribute columnOrAttribueSource)
        {
            ColumnInfo columnInfo;

            if (framework == null)
            {
                framework = new Framework(this.eaUtils);
            }
            string name;
            string precision;
            string scale;
            string type;
            bool isNullAble = true;
            bool? isRealPrimaryKey = false;
            bool? isSearch = true;
            string alias = columnOrAttribueSource.Alias;
            string defaultValue;
            string notes;
            string valueExample;

            this.lookingForClassOrTableSource(columnOrAttribueSource);

            // para determinar si es necesario realizar alguna conversión es necesario saber si es:
            // columna a tabla, columna a clase, atributo a tabla, atributo a clase.

            name             = namePrefix + columnOrAttribueSource.Name;
            alias            = columnOrAttribueSource.Alias;
            isRealPrimaryKey = eaUtils.taggedValuesUtils.getRealPrimaryKey(columnOrAttribueSource, "false").asBoolean();
            isSearch         = eaUtils.taggedValuesUtils.getSearch(columnOrAttribueSource, "false").asBoolean();
            isNullAble       = isNullable(columnOrAttribueSource);
            defaultValue     = columnOrAttribueSource.Default;
            type             = columnOrAttribueSource.Type;
            notes            = columnOrAttribueSource.Notes;
            valueExample     = eaUtils.taggedValuesUtils.getExample(columnOrAttribueSource, "").asString();

            // primero obtener los datos según su origen.
            if ( isSourceElementTable )
            {
                precision = columnOrAttribueSource.Type.Contains("char") ? columnOrAttribueSource.Length : columnOrAttribueSource.Precision;
                scale     = columnOrAttribueSource.Scale;
            }
            else
            {
                precision = eaUtils.taggedValuesUtils.getPrecision(columnOrAttribueSource, "").asString();
                scale     = eaUtils.taggedValuesUtils.getScale(columnOrAttribueSource, "").asString();
            }

            if( isTargetTable && ! isSourceElementTable )
            {
                DataTypeInfo dataTypeInfo = eaUtils.dataTypeUtils.getFrom(this.sourceElement.Gentype, columnOrAttribueSource.Type, "SQL");

                type = getColumnType(columnOrAttribueSource, dataTypeInfo);

                if ((precision == "" || precision == "0") && dataTypeInfo != null)
                {
                    precision = dataTypeInfo.ddl.defaultLength;
                }
                if ((scale == "" || scale == "0") && dataTypeInfo != null)
                {
                    scale = dataTypeInfo.ddl.defaultScale;
                }
            }
            else if( ! isTargetTable && isSourceElementTable)
            {
                DataTypeInfo dataTypeInfo = eaUtils.dataTypeUtils.getFrom(this.sourceElement.Gentype, columnOrAttribueSource.Type, this.targetElement.Gentype);

                if( dataTypeInfo != null )
                {
                    type = dataTypeInfo.name;
                }
            }


            columnInfo = new ColumnInfo(columnOrAttribueSource);

            //name = buildColumnOrAttributeName(columnOrAttribueSource.Name);
            EA.Attribute columnOrAttribute = getColumnOrAttribute(columnInfo);

            name = columnInfo.newName;

            //if( columnInfo.accionNew)
            if (columnOrAttribute == null)
            {
                if( isTargetTable)
                {
                    addColumn(name, alias, notes, type, precision, scale, defaultValue, isNullAble, isRealPrimaryKey, isSearch, valueExample,columnOrAttribueSource);
                }
                else
                {
                    addAttribute(name, alias, notes, type, precision, scale, defaultValue, isNullAble, isRealPrimaryKey, isSearch, valueExample, columnOrAttribueSource);
                }
            }
            else
            {
                if (isTargetTable)
                {
                    updateColumn(columnOrAttribute, alias, notes, type, precision, scale, defaultValue, isNullAble, isRealPrimaryKey, isSearch, valueExample, columnOrAttribueSource);
                }
                else
                {
                    updateAttribute(columnOrAttribute, alias, notes, type, precision, scale, defaultValue, isNullAble, isRealPrimaryKey, isSearch, valueExample, columnOrAttribueSource);
                }
            }
        }

        internal void toColumnOrAttribute(AttributeInfo attrinfo)
        {
            // si un atributo es por relación es una fk para esta tabla.
            // todo: ver el tema de fk.
            if (!attrinfo.isByRelation)
            {
                toColumnOrAttribute(attrinfo.attribute);
            }
        }

        public EA.Attribute addAttribute(string name, string alias, string notes, string type, string precision, string scale, string defaultValue, bool? isNullAble, bool? isRealPrimaryKey, bool? isSearch, string valueExample, EA.Attribute attribute)
        {
            EA.Attribute column;

            column = this.targetElement.Attributes.AddNew(name, "");

            if (this.targetElement.Stereotype == "DomainClass" || this.targetElement.Stereotype == "DAL::DomainClass")
            {
                column.Stereotype = "DAL::ExtendedAttribute";
            }

            updateAttribute(column, alias, notes, type, precision, scale, defaultValue, isNullAble, isRealPrimaryKey, isSearch, valueExample, attribute);

            return column;
        }

        public void updateAttribute(EA.Attribute toUpdateAttribute, string alias, string notes, string type, string precision, string scale, string defaultValue, bool? isNullAble, bool? isRealPrimaryKey, bool? isSearch, string valueExample, EA.Attribute attribute)
        {
            toUpdateAttribute.Type = type;

            if (notes != "")
            {
                toUpdateAttribute.Notes = notes;
            }

            if ( alias != "")
            {
                toUpdateAttribute.Alias = alias;
            }

            setDefaulValue(toUpdateAttribute, defaultValue);

            toUpdateAttribute.LowerBound = attribute.LowerBound;
            toUpdateAttribute.UpperBound = attribute.UpperBound;

            // es nuleable?
            toUpdateAttribute.AllowDuplicates = isNullAble == null ? false : isNullAble == true;
            // es clave primaria?
            toUpdateAttribute.IsOrdered = attribute.IsOrdered;

            toUpdateAttribute.Update();

            eaUtils.taggedValuesUtils.set(toUpdateAttribute, SOURCE_GUID_KEY, attribute.AttributeGUID);

            if (precision != null && precision != "" && precision != "0")
            {
                eaUtils.taggedValuesUtils.setPrecision(toUpdateAttribute, precision);
            }

            if (scale != null && scale != "" && scale != "0")
            {
                eaUtils.taggedValuesUtils.setScale(toUpdateAttribute, scale);
            }

            eaUtils.taggedValuesUtils.setRealPrimaryKey(toUpdateAttribute, isRealPrimaryKey == true ? "true" : "false");
            eaUtils.taggedValuesUtils.setSearch(toUpdateAttribute, isSearch == true ? "true" : "false");
            eaUtils.taggedValuesUtils.setRequired(toUpdateAttribute, isNullAble == true ? "true" : "false");
            eaUtils.taggedValuesUtils.setExample(toUpdateAttribute, valueExample);
        }

        public EA.Attribute addColumn(string name, string alias, string notes, string type, string precision, string scale, string defaultValue, bool? isNullAble, bool? isRealPrimaryKey, bool? isSearch, string valueExample, EA.Attribute attribute)
        {
            EA.Attribute column;

            column = this.targetElement.Attributes.AddNew(name, "");

            column.Stereotype = "column";

            updateColumn(column, alias, notes, type, precision, scale, defaultValue, isNullAble, isRealPrimaryKey, isSearch, valueExample,attribute);

            return column;
        }

        public void updateColumn(EA.Attribute column, string alias, string notes, string type, string precision, string scale, string defaultValue, bool? isNullAble, bool? isRealPrimaryKey, bool? isSearch, string valueExample, EA.Attribute attribute)
        {
            column.Type = type;

            if( alias != "")
            {
                column.Alias = alias;
            }

            if( notes != "")
            {
                column.Notes = notes;
            }

            if (precision != null && precision != "" && precision != "0")
            {
                column.Length = precision;
            }

            if (scale != null && scale != "" && scale != "0")
            {
                column.Scale = scale;
            }

            setDefaulValue(column, defaultValue);

            column.LowerBound = attribute.LowerBound;
            column.UpperBound = attribute.UpperBound;

            // es nuleable?
            column.AllowDuplicates = isNullAble == null ? false: isNullAble == true;
            // es clave primaria?
            column.IsOrdered = attribute.IsOrdered;

            column.Update();

            // esto tiene que ir despues del update porque cuando es nuevo aún no se creó en la base.
            if (attribute != null)
            {
                eaUtils.taggedValuesUtils.set(column, SOURCE_GUID_KEY, attribute.AttributeGUID);
                eaUtils.taggedValuesUtils.set(column, TaggedValuesUtils.EN__REAL_PK, isRealPrimaryKey == true ? "true": "false");
                eaUtils.taggedValuesUtils.set(column, TaggedValuesUtils.EN__BUSCAR, isSearch == true ? "true" : "false");
                eaUtils.taggedValuesUtils.setExample(column, valueExample); 

                if (column.IsOrdered)
                {
                    buildPrimaryKey(column);
                }
            }
        }

        private void setDefaulValue(EA.Attribute column, string defaultValue )
        {

            if (column.Type.ToLower().Contains("char"))
            {
                if (defaultValue != "")
                {
                    column.Default = (defaultValue.StartsWith("'") ? "" : "'") + defaultValue + (defaultValue.EndsWith("'") ? "" : "'");
                }
                else
                {
                    column.Default = "";
                }
            }
            else
            {
                column.Default = defaultValue;
            }
        }

        public void buildPrimaryKey(EA.Attribute column)
        {
            EA.Method method = null;

            foreach (EA.Method currentMethod in this.targetElement.Methods)
            {
                if (currentMethod.Stereotype == "PK")
                {
                    method = currentMethod;
                    break;
                }
            }

            if (method == null)
            {
                method = this.targetElement.Methods.AddNew("PK_" + this.targetElement.Name, "");
                method.Stereotype = "PK";
                method.Update();
            }

            Parameter parameter = null;

            if (method.Parameters.Count != 0)
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

        private String getColumnType(EA.Attribute attribute, DataTypeInfo dataTypeInfo)
        {
            string newType = null;

            if (dataTypeInfo != null)
            {
                newType = dataTypeInfo.name;
            }

            if (newType == null)
            {
                newType = "varchar";
            }
            return newType;
        }

        public EA.Attribute getColumnOrAttribute(ColumnInfo sourceInfo)
        {
            // busco por guid, si no por nombre.
            EA.Attribute columnOrAttribute = null;
            string name = this.buildColumnOrAttributeName(sourceInfo.source.Name);
            string nameAndPrefix = this.buildColumnOrAttributeName(this.namePrefix + StringUtils.toPascal(sourceInfo.source.Name));

            columnOrAttribute = lookingForColumnOrAttributeByGuid( name,  nameAndPrefix,  sourceInfo);

            if( ! sourceInfo.accionNew  && ! sourceInfo.accionUpdate )
            {
                // no se pudo resolver nada a nivel de origen.
                // intentamos con el nombre.
                columnOrAttribute = lookingForColumnOrAttributeByName(name, nameAndPrefix, sourceInfo);
            }
            return columnOrAttribute;
        }

        private EA.Attribute lookingForColumnOrAttributeByName(string name, string nameAndPrefix, ColumnInfo sourceInfo)
        {
            EA.Attribute columnOrAttribute = null;

            if ( this.columsByName.ContainsKey(nameAndPrefix))
            {
                // vemos si es el mismo orgigen.
                columnOrAttribute = this.columsByName[nameAndPrefix];
            }

            if (columnOrAttribute == null && this.columsByName.ContainsKey( name ) )
            {
                columnOrAttribute = this.columsByName[name];
            }

            if ( columnOrAttribute != null)
            {
                this.eaUtils.printOut("Se encuentra por nombre");

                // vemos si es el mismo orgigen, puede ser de otra clase o tabla que tenga el mismo nombre.
                string origenGuid = this.eaUtils.taggedValuesUtils.get(columnOrAttribute, SOURCE_GUID_KEY, "").asString();

                if( origenGuid == "" )
                {
                    this.eaUtils.printOut("No tiene origen");

                    // le asignamos de prepo el origen
                    this.eaUtils.taggedValuesUtils.set(columnOrAttribute, SOURCE_GUID_KEY, sourceInfo.source.AttributeGUID);

                    sourceInfo.accionUpdate = true;

                    if( this.forceAddNamePrefix && ! columnOrAttribute.Name.Equals(nameAndPrefix) )
                    {
                        // pregunto si quiere cambiar el nombre.
                        DialogResult dialogResult = MessageBox.Show(
                            "Acepta cambiar el nombre de " + columnOrAttribute.Name + " a " + nameAndPrefix
                            ,"Cambio de nombre"
                            , MessageBoxButtons.YesNo);

                        if (dialogResult != DialogResult.Yes)
                        {
                            sourceInfo.newName = nameAndPrefix;
                        }
                        else
                        {
                            sourceInfo.newName = columnOrAttribute.Name;
                        }
                    }
                }
                else if(origenGuid == sourceInfo.source.AttributeGUID)
                {
                    this.eaUtils.printOut("El origen es el mismo");

                    sourceInfo.accionUpdate = true;

                    if (this.forceAddNamePrefix && ! columnOrAttribute.Name.Equals(nameAndPrefix))
                    {
                        // pregunto si quiere cambiar el nombre.
                        DialogResult dialogResult = MessageBox.Show(
                            "Acepta cambiar el nombre de " + columnOrAttribute.Name + " a " + nameAndPrefix
                            ,"Cambio de nombre"
                            , MessageBoxButtons.YesNo);

                        if (dialogResult != DialogResult.Yes)
                        {
                            sourceInfo.newName = nameAndPrefix;
                        }
                        else
                        {
                            sourceInfo.newName = columnOrAttribute.Name;
                        }
                    }
                }
                else
                {
                    this.eaUtils.printOut("No coincide el origen, se creará uno nuevo");

                    columnOrAttribute = null;
                    sourceInfo.accionNew = true;
                    sourceInfo.newName = this.forceAddNamePrefix ? nameAndPrefix : name;
                }
            }
            else
            {
                this.eaUtils.printOut("No se encuentra por nombre, se creará uno nuevo");

                columnOrAttribute = null;
                sourceInfo.accionNew = true;
                sourceInfo.newName = this.forceAddNamePrefix ? nameAndPrefix : name;

            }
            return columnOrAttribute;
        }

        private EA.Attribute lookingForColumnOrAttributeWithoutGuid(string name, string nameAndPrefix, ColumnInfo sourceInfo)
        {
            EA.Attribute columnOrAttribute = null;

            if (this.columsByGuid.ContainsKey(""))
            {
                foreach (EA.Attribute attrOrColumn in this.columsByGuid[""])
                {
                    if (attrOrColumn.Name == name || attrOrColumn.Name == nameAndPrefix)
                    {
                        columnOrAttribute = attrOrColumn;
                        break;
                    }
                }
            }
            return columnOrAttribute;
        }

        private EA.Attribute lookingForColumnOrAttributeByGuid( string name, string nameAndPrefix, ColumnInfo sourceInfo)
        {
            EA.Attribute columnOrAttribute = null;

            if (this.columsByGuid.ContainsKey(sourceInfo.source.AttributeGUID))
            {
                this.eaUtils.printOut("Se encontró por origen");

                foreach (EA.Attribute attrOrColumn in this.columsByGuid[sourceInfo.source.AttributeGUID])
                {
                    if (attrOrColumn.Name.Equals( name ) || attrOrColumn.Name.Equals( nameAndPrefix) )
                    {
                        // mismo nombre - mismo guid
                        columnOrAttribute = attrOrColumn;
                        sourceInfo.accionUpdate = true;
                        sourceInfo.newName = this.forceAddNamePrefix ? nameAndPrefix : name;

                        this.eaUtils.printOut("Se encuentra por origen y por nombre " + attrOrColumn.Name);

                        if ( ! attrOrColumn.Name.Equals( sourceInfo.newName))
                        {
                            DialogResult dialogResult = MessageBox.Show(
                                "Acepta cambiar el nombre de " + attrOrColumn.Name +" a "+ sourceInfo.newName
                                ,"Cambio de nombre"
                                , MessageBoxButtons.YesNo);

                            if(dialogResult != DialogResult.Yes)
                            {
                                sourceInfo.newName = attrOrColumn.Name;
                            }
                        }
                        break;
                    }
                }
                // si se encunetra que alguno tiene origen pero no coincide el nombre
                // vemos si hay sólo 1
                if(columnOrAttribute == null && this.columsByGuid[sourceInfo.source.AttributeGUID].Count == 1)
                {
                    this.eaUtils.printOut("El orgien extiste pero no coincide el nombre y hay 1 que puede ser candidato");

                    DialogResult dialogResult = MessageBox.Show(
                        "Existe otro con el nombre de " + this.columsByGuid[sourceInfo.source.AttributeGUID][0].Name +" ¿Creamos uno nuevo?"
                        ,"Difiere el nombre"
                        , MessageBoxButtons.YesNo);

                    if (dialogResult == DialogResult.Yes)
                    {
                        sourceInfo.accionNew = true;
                        sourceInfo.newName = this.forceAddNamePrefix ? nameAndPrefix : name;

                        this.eaUtils.printOut("Existe uno con el mismo origen pero se eligió crear otro." + sourceInfo.newName);
                    }
                    else
                    {
                        columnOrAttribute       = this.columsByGuid[sourceInfo.source.AttributeGUID][0];
                        sourceInfo.accionUpdate = true;

                        this.eaUtils.printOut("Mismo origen y se eligió no crear uno nuevo, ¿se cambia el nombre?");

                        dialogResult = MessageBox.Show(
                            "Acepta cambiar el nombre de " + this.columsByGuid[sourceInfo.source.AttributeGUID][0].Name + " a " + name
                            ,"Cambio de nombre"
                            , MessageBoxButtons.YesNo);

                        if (dialogResult == DialogResult.Yes)
                        {
                            sourceInfo.newName = this.forceAddNamePrefix ? nameAndPrefix : name;
                        }
                        else
                        {
                            sourceInfo.newName = columnOrAttribute.Name;
                        }

                        if (columnOrAttribute != null)
                        {
                            columnOrAttribute.Name = sourceInfo.newName;
                            columnOrAttribute.Update();
                        }
                    }
                }
                else if(columnOrAttribute == null)
                {
                    // hay más de 1, de prepo agregamos otro.
                    sourceInfo.accionNew = true;
                    sourceInfo.newName = this.forceAddNamePrefix ? nameAndPrefix : name;

                    this.eaUtils.printOut("Hay más de 1 con el mismo origen, se crea uno de prepo");
                }
            }
            return columnOrAttribute;
        }

        private void lookingForClassOrTableSource(EA.Attribute attribute)
        {
            if (this.sourceElement == null || this.sourceElement.ElementID != attribute.ParentID)
            {
                sourceElement = eaUtils.repository.GetElementByID(attribute.ParentID);
            }
            isSourceElementTable = sourceElement.Stereotype == "table";
        }

        public string buildColumnOrAttributeName(string attributeName)
        {
            string name = attributeName;

            if ( isTargetTable)
            {
                name = framework.toCase(attributeName);
            }
            else
            {
                name = StringUtils.toCamel(attributeName);
            }
            return name;
        }

        private bool isNullable(EA.Attribute attribute)
        {
            bool isNullable = false;

            if( attribute.Stereotype == "column")
            {
                isNullable = attribute.AllowDuplicates;
            }
            else
            {
                TaggedValueWrapper tv = this.eaUtils.taggedValuesUtils.getRequired(attribute, null);

                if (tv.value != null)
                {
                    isNullable = tv.asBoolean().Value;
                }
            }

            return isNullable;
        }
    }

    public class ColumnInfo
    {
        /// <summary>
        /// El nombre que va a tener la columna o el atributo.
        /// </summary>
        public string newName;
        public bool accionNew = false;
        public bool accionUpdate = false;
        public EA.Attribute source;

        public ColumnInfo( EA.Attribute source)
        {
            this.source = source;
        }
    }
}
