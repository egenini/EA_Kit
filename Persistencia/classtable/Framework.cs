using EA;
using EAUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistencia.classtable
{
    public class Framework
    {
        public const string SQL_FRAMEWORK_GUID = "{0C048BE3-E064-4563-85AC-E09BC76744EF}";
        private const string TV_CASE_NAME = "SQL_CASE";
        private const string TV_SNAKE_UL_NAME = "SQL_SNAKE_U_L";

        private bool isLower = true;
        private bool isPascal = false;
        private bool isCamel = false;
        private bool isSnake = true;

        public EnumerationInfo enumerationInfo = new EnumerationInfo();

        EAUtils.EAUtils eaUtils;

        public Framework(EAUtils.EAUtils eaUtils)
        {
            this.eaUtils = eaUtils;

            // buscamos si estan los valores etiquetados en el paquete donde está el elemento

            try
            {
                Package sql = this.eaUtils.repository.GetPackageByGuid(SQL_FRAMEWORK_GUID);

                if (sql != null)
                {
                    string caseName = eaUtils.taggedValuesUtils.get(sql.Element, TV_CASE_NAME, "").asString().ToLower();

                    if ( caseName == "snake")
                    {
                        isSnake = true;
                    }
                    else if( caseName == "camel")
                    {
                        isCamel = true;
                    }
                    else if (caseName == "pascal")
                    {
                        isPascal = true;
                    }

                    string upperLower = eaUtils.taggedValuesUtils.get(sql.Element, TV_SNAKE_UL_NAME, "").asString().ToLower();

                    isLower = upperLower == "lower";

                    buildEnumerationInfo(sql);
                }
            }
            catch (Exception) { }
        }

        public string toCase( string name)
        {
            string nameCase = name;
            if( isPascal )
            {
                nameCase = StringUtils.toPascal(name);
            }
            else if( isCamel)
            {
                nameCase = StringUtils.toCamel(name);
            }
            else if (isSnake)
            {
                nameCase = StringUtils.toSnake(name);
                nameCase = isLower? nameCase.ToLower() : nameCase.ToUpper();
            }

            return nameCase;
        }

        private void buildEnumerationInfo(Package sql)
        {
            string attributeName;

            foreach( Element element in sql.Elements)
            {
                if(element.Type == "Enumeration")
                {
                    foreach( EA.Attribute attribute in element.Attributes)
                    {
                        attributeName = attribute.Name.ToLower();

                        if (attributeName.Contains("key") || attributeName.Contains("clave"))
                        {
                            enumerationInfo.keyColumnName = attribute.Default;
                        }
                        else
                        if (attributeName.Contains("value") || attributeName.Contains("valor"))
                        {
                            enumerationInfo.valueColumnName = attribute.Default;
                        }
                        else
                        if (attributeName.Contains("label") || attributeName.Contains("etiqueta"))
                        {
                            enumerationInfo.labelColumnName = attribute.Default;
                        }
                    }
                    break;
                }
            }
        }

        public class EnumerationInfo
        {
            public string keyColumnName = "key";
            public string valueColumnName = "value";
            public string labelColumnName = "label";
        }
    }
}
