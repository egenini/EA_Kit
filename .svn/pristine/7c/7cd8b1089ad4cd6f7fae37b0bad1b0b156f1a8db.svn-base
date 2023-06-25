using System;
using EAUtils;
using DMN.dominio;
using EA;
using UIResources;
using System.Collections.Generic;
using static DMN.dominio.Variable;

namespace DMN
{
    internal class EnumExporter
    {
        private EAUtils.EAUtils eaUtils;
        private List<string> attributes;

        public EnumExporter( EAUtils.EAUtils eaUtils)
        {
            this.eaUtils = eaUtils;
            this.attributes = new List<string>();
        }

        internal void export( Element variableElement, Decision decision )
        {
            bool     isInput     = false;
            bool     isOutput    = false;
            Element  enumeration = null;
            Conclusion conclusion = null;
            Condition condition = null;

            // buscar un input o un output que coincida con el alias de la variable.
            try
            {
                condition = decision.conditionByAttributeName[variableElement.Alias];
            }
            catch (Exception) { }

            isInput = condition != null;

            // si no aparece en los inputs buscamos en los outputs, podría no estar y además necesitamos saber en cual columna.
            if( ! isInput)
            {
                try
                {
                    conclusion = decision.conclusionByAttributeName[variableElement.Alias];
                }
                catch (Exception) { }

                isOutput = conclusion != null;
            }

            if (!isInput && !isOutput)
            {
                // la variable no está en uso
                Alert.Warning( Properties.Resources.variable_noUsada);

            }
            else
            {
                // chequamos que la variable no tenga una instancia de una enumeración, si la tiene usamos esa enumeración.
                Element instanceOfVariable = null;
                bool isUpdate = false;
                try
                {
                    if (variableElement.ClassifierID != 0)
                    {
                        instanceOfVariable = eaUtils.repository.GetElementByID(variableElement.ClassifierID);
                    }
                }
                catch (Exception) { }
                if (instanceOfVariable != null && instanceOfVariable.Type == "Enumeration")
                {
                    isUpdate = true;
                    enumeration = instanceOfVariable;
                    for (short i = 0; i < enumeration.Attributes.Count; i++)
                    {
                        enumeration.Attributes.DeleteAt(i, false);
                    }
                    enumeration.Refresh();
                }
                else
                {
                    Package package = this.eaUtils.repository.GetPackageByID(variableElement.PackageID);

                    enumeration = package.Elements.AddNew(EAUtils.StringUtils.toPascal(variableElement.Alias), "Enumeration");

                    enumeration.Update();

                    variableElement.ClassifierID = enumeration.ElementID;
                    variableElement.Update();
                }

                // vemos si hay valores permitidos
                List<AllowedValue> allowedValues = null;
                if (isInput)
                {
                    allowedValues = decision.conditionByAttributeName[variableElement.Alias].allowedValues;
                }
                else
                {
                    allowedValues = decision.conclusionByAttributeName[variableElement.Alias].allowedValues;
                }
                if( allowedValues != null && allowedValues.Count != 0)
                {
                    foreach( AllowedValue allowedValue in allowedValues)
                    {
                        this.add(enumeration, allowedValue.value);
                    }
                }

                string enumValue = "";
                // ya sabemos si es input o output y el indice de la columna, ahora buscamos los valores.
                foreach (Rule rule in decision.rules)
                {
                    if (isInput)
                    {
                        enumValue = rule.conditionValueByAttributeName[variableElement.Alias].value;
                    }
                    else
                    {
                        enumValue = rule.conclusionValueByAttributeName[variableElement.Alias].value;
                    }

                    if (enumValue != "")
                    {
                        this.add(enumeration, enumValue);
                    }
                }

                Alert.Success( String.Format( Properties.Resources.enumeracion_ok, isUpdate ? Properties.Resources.actualizado : Properties.Resources.creado) + enumeration.Name );
            }
        }

        private void add( Element enumeration, string value)
        {
            string attrName = EAUtils.StringUtils.toPascal(value).ToUpper();

            int? asInt = null;

            try
            {
                asInt = int.Parse(value);
                attrName = enumeration.Name + attrName;
            }
            catch (Exception) {}

            if ( ! this.attributes.Contains(attrName) )
            {
                EA.Attribute attr = enumeration.Attributes.AddNew( attrName, "short");

                attr.Alias = value;
                attr.Default = value;
                attr.Stereotype = "enum";

                attr.Update();

                this.attributes.Add(attrName);
            }
        }
    }
}