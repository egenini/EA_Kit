using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EA;
using System.Windows;
using UIResources;
using System.Runtime.CompilerServices;

namespace EAUtils
{
    public class TaggedValueWrapper
    {
        public string value = null;

        public TaggedValueWrapper( string value )
        {
            this.value = value;
        }

        public bool? asBoolean()
        {
            bool? nullableValue = null;
            if (value != null)
            {
                nullableValue = (value == "" ? false : (value == "0" ? false : (value.ToLower() == "false" ? false : ( value.ToLower() == "no" ? false :  true) ))); ;
            }
            return nullableValue;
        }

        public Nullable<int> asInt(int defaultValue)
        {
            Nullable<int> returnValue = asInt() ;
            return returnValue == null ? defaultValue : returnValue;
        }

        public Nullable<int> asInt()
        {
            Nullable<int> nullableValue = null;
            int intValue;
            if (value != null)
            {
                if (int.TryParse(value, out intValue))
                {
                    nullableValue = intValue;
                }
            }
            return nullableValue;
        }
        public string asString()
        {
            return value;
        }

        public string[] split(char c)
        {
            return value.Length == 0 ? new string[] {} : value.Split(c);
        }

        public string[] split()
        {
            return this.split(',');
        }
    }

    public class TaggedValuesUtils
    {
        public const string ES__TIPO_DATO = "Tipo de dato";
        public const string ES__PRECISION = "Precisión";
        public const string ES__ESCALA = "Escala";
        public const string ES__OBLIGATORIO = "Obligatorio";
        public const string ES__FORMATO = "Formato";
        public const string ES__DOMINIO_VALORES = "Dominio de valores";
        public const string ES__REAL_PK = "Clave primaria real";
        public const string ES__BUSCAR = "Buscar";
        public const string ES__ENCRIPTABLE = "Encriptado";
        // afecta a clases
        public const string ES__ALIAS_PLURAL = "plural_alias";
        public const string ES__NAME_PLURAL = "plural_name";
        public const string ES__EJEMPLO = "Ejemplo";
        

        public const string EN__TIPO_DATO = "Data type";
        public const string EN__PRECISION = "Precision";
        public const string EN__ESCALA = "Escale";
        public const string EN__OBLIGATORIO = "Required";
        public const string EN__FORMATO = "Format";
        public const string EN__ENCRIPTABLE = "Encripted";
        public const string EN__DOMINIO_VALORES = "Domain values";
        public const string EN__REAL_PK = "Real primary key";
        public const string EN__BUSCAR = "Search";
        // afecta a clases
        public const string EN__ALIAS_PLURAL = "plural_alias";
        public const string EN__NAME_PLURAL = "plural_name";

        public const string EN__EJEMPLO = "Example";

        public const string JSON_ALIAS = "JSON_Alias";

        public static readonly String[] ES_EXTEND_ATTRIBUTE_INFO_FOR_COLUMNS = { ES__FORMATO, ES__EJEMPLO, ES__DOMINIO_VALORES, ES__REAL_PK, ES__BUSCAR };
        public static readonly String[] EN_EXTEND_ATTRIBUTE_INFO_FOR_COLUMNS = { EN__FORMATO, EN__EJEMPLO, EN__DOMINIO_VALORES, EN__REAL_PK, EN__BUSCAR };
        public static readonly String[] ES_EXTEND_ATTRIBUTE_INFO_FOR_COLUMNS_UPDATABLE = { ES__FORMATO, ES__DOMINIO_VALORES };
        public static readonly String[] EN_EXTEND_ATTRIBUTE_INFO_FOR_COLUMNS_UPDATABLE = { EN__FORMATO, EN__DOMINIO_VALORES };


        public static readonly string[] ES_EXTEND_ATTRIBUTE_INFO = { ES__PRECISION, ES__ESCALA, ES__OBLIGATORIO, ES__FORMATO, ES__EJEMPLO ,ES__DOMINIO_VALORES, ES__REAL_PK, ES__BUSCAR, ES__ENCRIPTABLE };
        public static readonly string[] EN_EXTEND_ATTRIBUTE_INFO = { EN__PRECISION, EN__ESCALA, EN__OBLIGATORIO, EN__FORMATO, EN__EJEMPLO, EN__DOMINIO_VALORES, EN__REAL_PK, EN__BUSCAR, EN__ENCRIPTABLE };

        private const string MEMO_VALUE = "<memo>";

        public void setJsonAlias(EA.Attribute attribute, string value)
        {
            EA.AttributeTag taggedValue = null;

            if (attribute.TaggedValues.Count != 0)
            {
                taggedValue = attribute.TaggedValues.GetByName(JSON_ALIAS);
                if (taggedValue == null)
                {
                    taggedValue = attribute.TaggedValues.GetByName(JSON_ALIAS);
                }
            }

            if (taggedValue == null)
            {
                this.set(attribute, JSON_ALIAS, value);
            }
            else
            {
                taggedValue.Value = value;
                taggedValue.Update();
            }
        }

        public TaggedValueWrapper getJsonAlias(EA.Attribute attribute, string defaultValue)
        {
            return this.get(attribute, JSON_ALIAS, defaultValue);
        }

        public void setJsonAlias(EA.Element element, string value)
        {
            EA.AttributeTag taggedValue = null;

            if (element.TaggedValues.Count != 0)
            {
                taggedValue = element.TaggedValues.GetByName(JSON_ALIAS);
                if (taggedValue == null)
                {
                    taggedValue = element.TaggedValues.GetByName(JSON_ALIAS);
                }
            }

            if (taggedValue == null)
            {
                this.set(element, JSON_ALIAS, value);
            }
            else
            {
                taggedValue.Value = value;
                taggedValue.Update();
            }
        }

        public TaggedValueWrapper getJsonAlias(EA.Element element, string defaultValue)
        {
            return this.get(element, JSON_ALIAS, defaultValue);
        }

        public void setExtends(EA.Attribute source, EA.Attribute target)
        {
            bool clone = false;
            foreach (TaggedValue sourceTv in source.TaggedValues)
            {
                if (ES_EXTEND_ATTRIBUTE_INFO.Contains(sourceTv.Name))
                {
                    if (target.Stereotype == "column")
                    {
                        if (!ES_EXTEND_ATTRIBUTE_INFO_FOR_COLUMNS.Contains(sourceTv.Name))
                        {
                            continue;
                        }

                        if (ES_EXTEND_ATTRIBUTE_INFO_FOR_COLUMNS_UPDATABLE.Contains(sourceTv.Name)
                            && this.get(source, sourceTv.Name, null).asString() != null)
                        {
                            continue;
                        }
                    }
                    clone = true;
                }
                if (EN_EXTEND_ATTRIBUTE_INFO.Contains(sourceTv.Name))
                {
                    if (target.Stereotype == "column")
                    {
                        if (!EN_EXTEND_ATTRIBUTE_INFO_FOR_COLUMNS.Contains(sourceTv.Name))
                        {
                            continue;
                        }

                        if (EN_EXTEND_ATTRIBUTE_INFO_FOR_COLUMNS_UPDATABLE.Contains(sourceTv.Name)
                            && this.get(source, sourceTv.Name, null).asString() != null)
                        {
                            continue;
                        }
                    }
                    clone = true;
                }

                if (clone)
                {
                    this.set(target, sourceTv.Name, sourceTv.Value);
                }
            }
        }
        public void setExample(EA.Attribute attribute, string value)
        {
            EA.AttributeTag taggedValue = null;

            if (attribute.TaggedValues.Count != 0)
            {
                taggedValue = attribute.TaggedValues.GetByName(EN__EJEMPLO);
                if (taggedValue == null)
                {
                    taggedValue = attribute.TaggedValues.GetByName(ES__EJEMPLO);
                }
            }

            if (taggedValue == null)
            {
                this.set(attribute, EN__EJEMPLO, value);
            }
            else
            {
                taggedValue.Value = toExtendedClassValue(value);

                try
                {
                    taggedValue.Update();
                }
                catch (RuntimeWrappedException e)
                {
                    Clipboard.SetText(e.ToString());
                    Alert.Error( "Se produjo un error que se ha copiado al portapapeles, el programa continúa" );
                }
            }
        }

        public TaggedValueWrapper getExample(EA.Attribute attribute, string defaultValue)
        {
            TaggedValueWrapper wrapper = this.get(attribute, EN__EJEMPLO, null);

            if (wrapper.value == null)
            {
                wrapper = this.get(attribute, ES__EJEMPLO, null);
            }

            if (wrapper.value == null)
            {
                wrapper.value = defaultValue;
            }
            return wrapper;
        }

        public void setFormat(EA.Attribute attribute, string value)
        {
            EA.AttributeTag taggedValue = null;

            if (attribute.TaggedValues.Count != 0)
            {
                taggedValue = attribute.TaggedValues.GetByName(EN__FORMATO);
                if (taggedValue == null)
                {
                    taggedValue = attribute.TaggedValues.GetByName(ES__FORMATO);
                }
            }

            if (taggedValue == null)
            {
                this.set(attribute, EN__FORMATO, value);
            }
            else
            {
                taggedValue.Value = value;
                taggedValue.Update();
            }
        }

        public TaggedValueWrapper getFormat(EA.Attribute attribute, string defaultValue)
        {
            TaggedValueWrapper wrapper = this.get(attribute, EN__FORMATO, null);

            if (wrapper.value == null)
            {
                wrapper = this.get(attribute, ES__FORMATO, null);
            }

            if (wrapper.value == null)
            {
                wrapper.value = defaultValue;
            }
            return wrapper;
        }

        public void setPrecision(EA.Attribute attribute, string value)
        {
            EA.AttributeTag taggedValue = null;

            if (attribute.TaggedValues.Count != 0)
            {
                taggedValue = attribute.TaggedValues.GetByName(EN__PRECISION);
                if (taggedValue == null)
                {
                    taggedValue = attribute.TaggedValues.GetByName(ES__PRECISION);
                }
            }

            if (taggedValue == null)
            {
                this.set(attribute, EN__PRECISION, value);
            }
            else
            {
                taggedValue.Value = value;
                taggedValue.Update();
            }
        }

        public TaggedValueWrapper getPrecision(EA.Attribute attribute, string defaultValue)
        {
            TaggedValueWrapper wrapper = this.get(attribute, ES__PRECISION, null);

            if (wrapper.value == null)
            {
                wrapper = this.get(attribute, EN__PRECISION, null);
            }

            if (wrapper.value == null)
            {
                wrapper.value = defaultValue;
            }
            return wrapper;
        }

        public void setScale(EA.Attribute attribute, string value)
        {
            EA.AttributeTag taggedValue = null;

            if (attribute.TaggedValues.Count != 0)
            {
                taggedValue = attribute.TaggedValues.GetByName(EN__ESCALA);
                if (taggedValue == null)
                {
                    taggedValue = attribute.TaggedValues.GetByName(ES__ESCALA);
                }
            }

            if (taggedValue == null)
            {
                this.set(attribute, EN__ESCALA, value);
            }
            else
            {
                taggedValue.Value = value;
                taggedValue.Update();
            }

        }

        public TaggedValueWrapper getScale(EA.Attribute attribute, string defaultValue)
        {
            TaggedValueWrapper wrapper = this.get(attribute, ES__ESCALA, null);

            if (wrapper.value == null)
            {
                wrapper = this.get(attribute, EN__ESCALA, null);
            }

            if (wrapper.value == null)
            {
                wrapper.value = defaultValue;
            }
            return wrapper;
        }

        public void delete(EA.Attribute eaAttribute, string v)
        {
            for (short i = 0; i < eaAttribute.TaggedValues.Count; i++)
            {
                if (((EA.AttributeTag)eaAttribute.TaggedValues.GetAt(i)).Name == v)
                {
                    eaAttribute.TaggedValues.DeleteAt(i, false);
                    break;
                }
            }
            eaAttribute.TaggedValues.Refresh();
        }

        public void delete(EA.Element element, string v)
        {
            for (short i = 0; i < element.TaggedValues.Count; i++)
            {
                if (((EA.TaggedValue)element.TaggedValues.GetAt(i)).Name == v)
                {
                    element.TaggedValues.DeleteAt(i, false);
                    break;
                }
            }
            element.TaggedValues.Refresh();
        }
        public void setRequired(EA.Attribute attribute, string value)
        {
            EA.AttributeTag taggedValue = null;

            if (attribute.TaggedValues.Count != 0)
            {
                taggedValue = attribute.TaggedValues.GetByName(EN__OBLIGATORIO);
                if (taggedValue == null)
                {
                    taggedValue = attribute.TaggedValues.GetByName(ES__OBLIGATORIO);
                }
            }

            if (taggedValue == null)
            {
                this.set(attribute, EN__OBLIGATORIO, value);
            }
            else
            {
                taggedValue.Value = value;
                taggedValue.Update();
            }

        }

        public TaggedValueWrapper getRequired(EA.Attribute attribute, string defaultValue)
        {
            TaggedValueWrapper wrapper = this.get(attribute, ES__OBLIGATORIO, null);

            if (wrapper.value == null)
            {
                wrapper = this.get(attribute, EN__OBLIGATORIO, null);
            }

            if (wrapper.value == null)
            {
                wrapper.value = defaultValue;
            }
            return wrapper;
        }

        public TaggedValueWrapper getEncrypted(EA.Attribute attribute, string defaultValue)
        {
            TaggedValueWrapper wrapper = this.get(attribute, EN__ENCRIPTABLE, null);

            if (wrapper.value == null)
            {
                wrapper = this.get(attribute, ES__ENCRIPTABLE, null);
            }

            if (wrapper.value == null)
            {
                wrapper.value = defaultValue;
            }
            return wrapper;
        }

        public TaggedValueWrapper getDomainValues(EA.Attribute attribute, string defaultValue)
        {
            TaggedValueWrapper wrapper = this.get(attribute, ES__DOMINIO_VALORES, null);

            if (wrapper.value == null)
            {
                wrapper = this.get(attribute, EN__DOMINIO_VALORES, null);
            }

            if (wrapper.value == null)
            {
                wrapper.value = defaultValue;
            }
            return wrapper;
        }

        public TaggedValueWrapper getDataType(EA.Attribute attribute, string defaultValue)
        {
            TaggedValueWrapper wrapper = this.get(attribute, ES__TIPO_DATO, null);

            if (wrapper.value == null)
            {
                wrapper = this.get(attribute, EN__TIPO_DATO, null);
            }

            if (wrapper.value == null)
            {
                wrapper.value = defaultValue;
            }
            return wrapper;
        }

        public void setRealPrimaryKey(EA.Attribute attribute, string value)
        {
            EA.AttributeTag taggedValue = null;

            if (attribute.TaggedValues.Count != 0)
            {
                taggedValue = attribute.TaggedValues.GetByName(EN__REAL_PK);
                if (taggedValue == null)
                {
                    taggedValue = attribute.TaggedValues.GetByName(ES__REAL_PK);
                }
            }

            if (taggedValue == null)
            {
                this.set(attribute, EN__REAL_PK, value);
            }
            else
            {
                taggedValue.Value = toExtendedClassValue(value);
                taggedValue.Update();
            }

        }

        public TaggedValueWrapper getRealPrimaryKey(EA.Attribute attribute, string defaultValue)
        {
            TaggedValueWrapper wrapper = this.get(attribute, ES__REAL_PK, null);

            if (wrapper.value == null)
            {
                wrapper = this.get(attribute, EN__REAL_PK, null);
            }

            if (wrapper.value == null)
            {
                wrapper.value = defaultValue;
            }
            return wrapper;
        }

        public void setSearch(EA.Attribute attribute, string value)
        {
            EA.AttributeTag taggedValue = null;

            if (attribute.TaggedValues.Count != 0)
            {
                taggedValue = attribute.TaggedValues.GetByName(EN__BUSCAR);
                if (taggedValue == null)
                {
                    taggedValue = attribute.TaggedValues.GetByName(ES__BUSCAR);
                }
            }

            if (taggedValue == null)
            {
                this.set(attribute, EN__BUSCAR, value);
            }
            else
            {
                taggedValue.Value = toExtendedClassValue(value);
                taggedValue.Update();
            }
        }

        public TaggedValueWrapper getSearch(EA.Attribute attribute, string defaultValue)
        {
            TaggedValueWrapper wrapper = this.get(attribute, ES__BUSCAR, null);

            if (wrapper.value == null)
            {
                wrapper = this.get(attribute, EN__BUSCAR, null);
            }

            if (wrapper.value == null)
            {
                wrapper.value = defaultValue;
            }
            return wrapper;
        }

        public void setPluralName(EA.Element element, string value)
        {
            EA.TaggedValue taggedValue = null;

            if (element.TaggedValues.Count != 0)
            {
                taggedValue = element.TaggedValues.GetByName(EN__NAME_PLURAL);
                if (taggedValue == null)
                {
                    taggedValue = element.TaggedValues.GetByName(ES__NAME_PLURAL);
                }
            }

            if (taggedValue == null)
            {
                this.set(element, EN__NAME_PLURAL, value);
            }
            else
            {
                taggedValue.Value = toExtendedClassValue(value);
                taggedValue.Update();
            }
        }

        public TaggedValueWrapper getPluralName(EA.Element element, string defaultValue)
        {
            TaggedValueWrapper wrapper = this.get(element, EN__NAME_PLURAL, null);

            if (wrapper.value == null)
            {
                wrapper = this.get(element, ES__NAME_PLURAL, null);
            }

            if (wrapper.value == null)
            {
                wrapper.value = defaultValue;
            }
            return wrapper;
        }

        public void setPluralAlias(EA.Element element, string value)
        {
            EA.TaggedValue taggedValue = null;

            if (element.TaggedValues.Count != 0)
            {
                taggedValue = element.TaggedValues.GetByName(EN__ALIAS_PLURAL);
                if (taggedValue == null)
                {
                    taggedValue = element.TaggedValues.GetByName(ES__ALIAS_PLURAL);
                }
            }

            if (taggedValue == null)
            {
                this.set(element, EN__ALIAS_PLURAL, value);
            }
            else
            {
                taggedValue.Value = toExtendedClassValue(value);
                taggedValue.Update();
            }
        }

        public TaggedValueWrapper getPluralAlias(EA.Element element, string defaultValue)
        {
            TaggedValueWrapper wrapper = this.get(element, EN__ALIAS_PLURAL, null);

            if (wrapper.value == null)
            {
                wrapper = this.get(element, ES__ALIAS_PLURAL, null);
            }

            if (wrapper.value == null)
            {
                wrapper.value = defaultValue;
            }
            return wrapper;
        }

        public TaggedValueWrapper get(EA.Attribute attribute, string name, string defaultValue)
        {
            EA.AttributeTag taggedValue;
            string value = defaultValue;

            if (attribute.TaggedValues.Count != 0)
            {
                taggedValue = attribute.TaggedValues.GetByName(name);
                if (taggedValue != null)
                {
                    value = taggedValue.Value;

                    if (value == MEMO_VALUE)
                    {
                        value = taggedValue.Notes;
                    }
                }
            }
            return new TaggedValueWrapper(value);
        }

        private string toExtendedClassValue(string value)
        {
            string toReturn = value;

            if (value.ToLower() == "true")
            {
                toReturn = "Yes";
            }
            else if (value.ToLower() == "false")
            {
                toReturn = "No";
            }


            return toReturn;
        }
        public EA.TaggedValue getTaggedValue(Element element, string name)
        {
            EA.TaggedValue taggedValue = null;

            if (element.TaggedValues.Count != 0)
            {
                try
                {
                    taggedValue = element.TaggedValues.GetByName(name);
                    if (taggedValue == null)
                    {
                        taggedValue = element.TaggedValues.AddNew(name, "");
                        taggedValue.Update();
                    }
                }
                catch (Exception)
                {
                    taggedValue = element.TaggedValues.AddNew(name, "");
                    taggedValue.Update();
                    element.TaggedValues.Refresh();
                }
            }
            else
            {
                taggedValue = element.TaggedValues.AddNew(name, "");
                taggedValue.Update();
                element.TaggedValues.Refresh();
            }
            return taggedValue;
        }

        public void set(EA.Attribute attribute, string name, string value)
        {
            EA.AttributeTag taggedValue;

            if (attribute.TaggedValues.Count != 0)
            {
                taggedValue = attribute.TaggedValues.GetByName(name);
                if (taggedValue == null)
                {
                    taggedValue = attribute.TaggedValues.AddNew(name, "");
                    taggedValue.Value = value;
                    taggedValue.Update();
                    attribute.TaggedValues.Refresh();

                }
                else
                {
                    taggedValue.Value = value;
                    taggedValue.Update();
                }
            }
            else
            {
                taggedValue = attribute.TaggedValues.AddNew(name, "");
                taggedValue.Value = value;
                taggedValue.Update();
                attribute.TaggedValues.Refresh();
            }
        }

        public TaggedValueWrapper get(EA.Connector connector, string name, string defaultValue)
        {
            EA.ConnectorTag taggedValue;
            string value = defaultValue;

            if (connector.TaggedValues.Count != 0)
            {
                taggedValue = connector.TaggedValues.GetByName(name);
                if (taggedValue != null)
                {
                    value = taggedValue.Value;

                    if (value == MEMO_VALUE)
                    {
                        value = taggedValue.Notes;
                    }
                }
            }
            return new TaggedValueWrapper(value);
        }

        public void setRequired(EA.Connector connector, string value)
        {
            EA.ConnectorTag taggedValue = null;

            if (connector.TaggedValues.Count != 0)
            {
                taggedValue = connector.TaggedValues.GetByName(EN__OBLIGATORIO);
                if (taggedValue == null)
                {
                    taggedValue = connector.TaggedValues.GetByName(ES__OBLIGATORIO);
                }
            }

            if (taggedValue == null)
            {
                this.set(connector, EN__OBLIGATORIO, value);
            }
            else
            {
                taggedValue.Value = value;
                taggedValue.Update();
            }
        }

        public TaggedValueWrapper getRequired(EA.Connector connector, string defaultValue)
        {
            TaggedValueWrapper wrapper = this.get(connector, ES__OBLIGATORIO, null);

            if (wrapper.value == null)
            {
                wrapper = this.get(connector, EN__OBLIGATORIO, null);
            }

            if (wrapper.value == null)
            {
                wrapper.value = defaultValue;
            }
            return wrapper;
        }

        public void set(EA.Connector connector, string name, string value, bool asMemo)
        {
            EA.ConnectorTag taggedValue;

            if (connector.TaggedValues.Count != 0)
            {
                taggedValue = connector.TaggedValues.GetByName(name);
                if (taggedValue == null)
                {
                    taggedValue = connector.TaggedValues.AddNew(name, "");
                    taggedValue.Value = MEMO_VALUE;
                    taggedValue.Notes = value;
                    taggedValue.Update();
                    connector.TaggedValues.Refresh();

                }
                else
                {
                    taggedValue.Notes = value;
                    taggedValue.Update();
                }
            }
            else
            {
                taggedValue = connector.TaggedValues.AddNew(name, "");
                taggedValue.Value = MEMO_VALUE;
                taggedValue.Notes = value;
                taggedValue.Update();
                connector.TaggedValues.Refresh();
            }

        }
        public void set(EA.Connector connector, string name, string value)
        {
            EA.ConnectorTag taggedValue;

            if (connector.TaggedValues.Count != 0)
            {
                taggedValue = connector.TaggedValues.GetByName(name);
                if (taggedValue == null)
                {
                    taggedValue = connector.TaggedValues.AddNew(name, "");
                    taggedValue.Value = value;
                    taggedValue.Update();
                    connector.TaggedValues.Refresh();

                }
                else
                {
                    taggedValue.Value = value;
                    taggedValue.Update();
                }
            }
            else
            {
                taggedValue = connector.TaggedValues.AddNew(name, "");
                taggedValue.Value = value;
                taggedValue.Update();
                connector.TaggedValues.Refresh();
            }
        }
        public TaggedValueWrapper get(EA.Parameter parameter, string name, string defaultValue)
        {
            EA.ParamTag taggedValue;
            string value = defaultValue;

            if (parameter.TaggedValues.Count != 0)
            {
                taggedValue = parameter.TaggedValues.GetByName(name);
                if (taggedValue != null)
                {
                    value = taggedValue.Value;
                }
            }
            return new TaggedValueWrapper(value);
        }

        public void set(EA.Parameter parameter, string name, string value)
        {
            EA.ParamTag taggedValue;

            if (parameter.TaggedValues.Count != 0)
            {
                taggedValue = parameter.TaggedValues.GetByName(name);
                if (taggedValue == null)
                {
                    taggedValue = parameter.TaggedValues.AddNew(name, "");
                    taggedValue.Value = value;
                    taggedValue.Update();
                    parameter.TaggedValues.Refresh();

                }
                else
                {
                    taggedValue.Value = value;
                    taggedValue.Update();
                }
            }
            else
            {
                taggedValue = parameter.TaggedValues.AddNew(name, "");
                taggedValue.Value = value;
                taggedValue.Update();
                parameter.TaggedValues.Refresh();
            }
        }

        public TaggedValueWrapper get(EA.Method method, string name, string defaultValue)
        {
            EA.MethodTag taggedValue;
            string value = defaultValue;

            if (method.TaggedValues.Count != 0)
            {
                taggedValue = method.TaggedValues.GetByName(name);
                if (taggedValue != null)
                {
                    value = taggedValue.Value;

                    if (value == MEMO_VALUE)
                    {
                        value = taggedValue.Notes;
                    }
                }
            }
            return new TaggedValueWrapper(value);
        }

        public void set(EA.Method method, string name, string value)
        {
            EA.MethodTag taggedValue;

            if (method.TaggedValues.Count != 0)
            {
                taggedValue = method.TaggedValues.GetByName(name);
                if (taggedValue == null)
                {
                    taggedValue = method.TaggedValues.AddNew(name, "");
                    taggedValue.Value = value;
                    taggedValue.Update();
                    method.TaggedValues.Refresh();

                }
                else
                {
                    taggedValue.Value = value;
                    taggedValue.Update();
                }
            }
            else
            {
                taggedValue = method.TaggedValues.AddNew(name, "");
                taggedValue.Value = value;
                taggedValue.Update();
                method.TaggedValues.Refresh();
            }
        }

        public TaggedValueWrapper get(EA.Element element, string name, string defaultValue)
        {
            EA.TaggedValue taggedValue;

            string value = defaultValue;

            if (element.TaggedValues.Count != 0)
            {
                taggedValue = element.TaggedValues.GetByName(name);

                if (taggedValue != null)
                {
                    if (taggedValue.Value == MEMO_VALUE)
                    {
                        value = taggedValue.Notes;
                    }
                    else if(taggedValue.Value != "")
                    {
                        value = taggedValue.Value;
                    }
                }
            }
            return new TaggedValueWrapper(value);
        }

        /// <summary>
        /// Establece el valor en una etiqueta, si no existe la crea
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="asMemo">Fuerza a guardar el valor como memo si es true</param>
        public void set(EA.Element element, string name, string value, bool asMemo, bool replaceValue)
        {
            EA.TaggedValue taggedValue = null;
            bool create = false;

            if (element.TaggedValues.Count != 0)
            {
                taggedValue = element.TaggedValues.GetByName(name);

                create = taggedValue == null;
            }
            else
            {
                create = true;
            }

            if (create)
            {
                taggedValue = element.TaggedValues.AddNew(name, "");
                replaceValue = true;
            }

            if (asMemo)
            {
                if( replaceValue)
                {
                    taggedValue.Value = MEMO_VALUE;
                    taggedValue.Notes = value;
                }
            }
            else
            {
                if (replaceValue)
                {
                    taggedValue.Value = value;
                }
            }

            taggedValue.Update();

            if (create)
            {
                element.TaggedValues.Refresh();
            }
        }

        public void set(EA.Element element, string name, string value, bool asMemo)
        {
            this.set(element, name, value, asMemo, true);
        }
            
        /// <summary>
        /// Establece el valor en una etiqueta, si esta no existe la crea.
        /// Si el valor es muy grande y el update lanza una exception se intenta establecer el valor como memo.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void set(EA.Element element, string name, string value)
        {
            try
            {
                this.set(element, name, value, false);
            }
            catch ( Exception  )
            {
                this.set(element, name, value, true);
            }
        }

        public Dictionary<string, string> getByPrefix(Element element, string prefix, string[] replace)
        {
            Dictionary<string, string> results = new Dictionary<string, string>();

            foreach (TaggedValue tag in element.TaggedValues)
            {
                if (tag.Name.StartsWith(prefix))
                {
                    if( replace != null)
                    {
                        results.Add(tag.Name.Replace( replace[0], replace[1]), tag.Value);
                    }
                    else
                    {
                        results.Add(tag.Name, tag.Value);
                    }
                }
            }
            return results;
        }

        public Dictionary<string, string> getByPrefix(Connector connector, string prefix)
        {
            Dictionary<string, string> results = new Dictionary<string, string>();

            foreach( ConnectorTag tag in connector.TaggedValues)
            {
                if( tag.Name.StartsWith( prefix ) )
                {
                    results.Add(tag.Name, tag.Value);
                }
            }
            return results;
        }
    }
}
