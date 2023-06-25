using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DMN
{
    /// <summary>
    /// Opciones de menú del combo de tipo de dato.
    /// </summary>
    public class Option
    {
        public int id = 0;

        public const int emptyOptionId = 0;
        private string emptyOption = "";

        public const int chooseOptionId = -1;
        private string chooseOption = "<"+Properties.Resources.seleccionar+">";

        public Element languageDataType = null;
        public Element language = null;

        public Element enumeration = null;
        public string enumerationDataType;

        public EA.Attribute attribute = null;
        public Element atributeClassOwner = null;

        public EA.Method method = null;
        public Element methodClassOwner = null;

        public Element ditAttribute = null;
        public Element ditAttributeOwner = null;
        public string ditAttributeDataType = null;

        public string getSimpleName()
        {
            string simpleName = "";

            if( languageDataType != null)
            {
                simpleName = languageDataType.Name;
            }
            else
            if (enumeration != null)
            {
                simpleName = enumeration.Name;
            }
            else
            if (attribute != null)
            {
                simpleName = attribute.Name;
            }
            else
            if (method != null)
            {
                simpleName = method.Name;
            }
            else
            if (ditAttribute != null)
            {
                simpleName = ditAttribute.Name;
            }
            return simpleName;
        }

        public Option( int id)
        {
            this.id = id;
        }

        public Option()
        {

        }

        public static Option EmptyOption( )
        {
            return new Option(emptyOptionId);
        }
        public static Option ChooseOption()
        {
            return new Option(chooseOptionId);
        }

        public static Option LanguageOption( Element languageDataType, Element language)
        {
            Option option = new Option();

            option.id = languageDataType.ElementID;
            option.language = language;
            option.languageDataType = languageDataType;

            return option;
        }

        public static Option EnumerationOption( Element enumeration, string enumerationDataType)
        {
            Option option = new Option();

            option.id = enumeration.ElementID;
            option.enumeration = enumeration;
            option.enumerationDataType = enumerationDataType;

            return option;
        }

        public static Option AttributeOption(EA.Attribute attribute, Element atributeClassOwner)
        {
            Option option = new Option();

            option.id = attribute.AttributeID;
            option.attribute = attribute;
            option.atributeClassOwner = atributeClassOwner;

            return option;
        }

        public static Option MethodOption(EA.Method method, Element methodClassOwner)
        {
            Option option = new Option();

            option.id = method.MethodID;
            option.method = method;
            option.methodClassOwner = methodClassOwner;

            return option;
        }

        public static Option DitAttributeOption(Element ditAttribute, string ditAttributeDataType, Element ditAttributeOwner)
        {
            Option option = new Option();

            option.id = ditAttribute.ElementID;
            option.ditAttribute = ditAttribute;
            option.ditAttributeDataType = ditAttributeDataType;
            option.ditAttributeOwner = ditAttributeOwner;

            return option;
        }

        public override string ToString()
        {
            string toStr = "";

            if (languageDataType != null)
            {
                toStr = language.Name +"::"+ languageDataType.Name;
            }
            else if (enumeration != null)
            {
                toStr = enumeration.Name +"::"+ enumerationDataType;
            }
            else if (attribute != null)
            {
                toStr = atributeClassOwner.Name +"::"+ attribute.Name +"::"+ attribute.Type;
            }
            else if (method != null)
            {
                toStr = methodClassOwner.Name +"::"+ method.Name +"::"+ method.ReturnType;
            }
            else if (ditAttribute != null)
            {
                toStr = ditAttributeOwner.Name +"::"+ ditAttribute.Name +"::"+ ditAttributeDataType;
            }
            else if (id == 0)
            {
                toStr = emptyOption;
            }
            else if (id == -1)
            {
                toStr = chooseOption;
            }

            return toStr;
        }
    }
}
