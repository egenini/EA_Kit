using DMN.dominio;
using DMN.framework;
using EA;
using EAUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using UIResources;

namespace DMN
{
    internal class DecisionBuilder
    {
        private Decision decision;
        private XmlDocument eaDecisionTableXml;
        private EAUtils.EAUtils eaUtils;
        private Framework framework;

        public DecisionBuilder(Decision decision, EAUtils.EAUtils eaUtils, Framework framework)
        {
            this.decision = decision;
            this.eaUtils = eaUtils;
            this.framework = framework;

        }
        public DecisionBuilder(Decision decision)
        {
            this.decision = decision;
        }

        public bool build(bool silenceMode)
        {
            bool canBuild = this.buildXmlEaTable(silenceMode);

            Clipboard.SetText(this.eaDecisionTableXml.OuterXml); 

            if ( canBuild )
            {
                XmlNode rootNode           = this.eaDecisionTableXml.SelectSingleNode("/decision_table");

                this.decision.hitPolicy    = rootNode.Attributes["hit_policy"].Value;
                this.decision.completeness = rootNode.Attributes["completeness"].Value;
                this.decision.aggregation   = rootNode.Attributes["aggregation"].Value;

                XmlNodeList conditionsNode = eaDecisionTableXml.SelectNodes("/decision_table/conditions/condition_item");
                XmlNodeList actionsNode    = eaDecisionTableXml.SelectNodes("/decision_table/actions/action_item");

                this.generarCondiciones(conditionsNode);
                this.generarConclusiones(actionsNode);

                this.generarReglas(conditionsNode, actionsNode);

                canBuild = this.decision.isCompleted(silenceMode);
            }

            return canBuild;
        }

        private void generarReglas(XmlNodeList conditionsNode, XmlNodeList actionsNode)
        {
            Rule regla;

            XmlNode conditionItemNode;
            XmlNode actionItemNode;

            XmlNodeList valuesNode;
            XmlNode valueNode;

            XmlNodeList resultsNode;
            XmlNode resultNode;

            Condition condition;
            Conclusion conclusion;
            string value;

            // cada regla es una "fila" por lo que voy a contar las "filas", da igual sobre cual de los nodos cuento.
            int rulesQuantity = conditionsNode.Item(0).SelectNodes("values/value").Count;

            // buscar en cada condicion y en cada acción con el mismo indice de regla.
            for (int rulesIndex = 0; rulesIndex < rulesQuantity; rulesIndex++)
            {
                regla = new Rule();
                this.decision.rules.Add(regla);

                for (short i = 0; i < conditionsNode.Count; i++)
                {
                    conditionItemNode = conditionsNode.Item(i);
                    valuesNode = conditionItemNode.SelectNodes("values/value");
                    valueNode = valuesNode.Item(rulesIndex);
                    value = valueNode.SelectSingleNode("selected").InnerText;

                    // ver si el valueNode es un alias o name de un allowedValue
                    condition = this.decision.conditions[i];

                    if( ! condition.isDescrition)
                    {
                        foreach (Variable.AllowedValue allowed in condition.allowedValues)
                        {
                            if (value == allowed.businessName)
                            {
                                value = allowed.value;
                            }
                            else if (value == allowed.attributeName)
                            {
                                value = allowed.value;
                            }
                        }

                        if( value != "-" && value.Replace(" ", "").Length != 0)
                        {
                            regla.conditionValueAdd(new CondicionValue(value, this.decision.conditions[i]));
                        }
                    }
                }

                for (short i = 0; i < actionsNode.Count; i++)
                {
                    actionItemNode = actionsNode.Item(i);
                    resultsNode = actionItemNode.SelectNodes("results/result");
                    resultNode = resultsNode.Item(rulesIndex);

                    value = resultNode.SelectSingleNode("selected").InnerText;

                    // ver si el value es un alias o name de un allowedValue
                    conclusion = this.decision.conclusions[i];

                    if( ! conclusion.isDescrition )
                    {
                        foreach (Variable.AllowedValue allowed in conclusion.allowedValues)
                        {
                            if (value == allowed.businessName)
                            {
                                value = allowed.value;
                            }
                            else if (value == allowed.attributeName)
                            {
                                value = allowed.value;
                            }
                        }
                        regla.conclusionValueAdd(new ConclusionValue(value, this.decision.conclusions[i]));
                    }
                }
            }
        }

        private void generarCondiciones(XmlNodeList node)
        {
            this.generarVariables( node, true );
        }
        private void generarConclusiones(XmlNodeList node)
        {
            this.generarVariables(node, false);
        }
        private void generarVariables(XmlNodeList variableNode, bool isCondicion)
        {
            XmlNode itemNode;
            Element variableAsociada = null;
            VariableManager variableManager = null; 

            for (short i = 0; i < variableNode.Count; i++)
            {
                itemNode         = variableNode.Item(i);
                variableAsociada = null;
                variableManager  = new VariableManager(this.eaUtils);

                if (variableAsociada == null)
                {
                    try
                    {
                        variableAsociada = eaUtils.repository.GetElementByGuid(itemNode.Attributes["guid"].Value);
                    }
                    catch (Exception) { }
                }
                // cada condicion es un input
                if( variableAsociada != null)
                {
                    string dataType = variableManager.getDataType(variableAsociada, framework);
                    
                    if ( isCondicion)
                    {
                        if( variableAsociada.Stereotype != "Condition")
                        {
                            variableAsociada.Stereotype = "Condition";
                            variableAsociada.Update();
                        }

                        Condition condicion = new Condition(variableAsociada);

                        condicion.businessName = variableAsociada.Name;
                        condicion.attributeName = variableAsociada.Alias;
                        if( variableAsociada.Name.Contains("*"))
                        {
                            condicion.isDescrition = true;
                        }
                        condicion.isMethod = variableManager.isMethod;
                        condicion.language = variableManager.language;

                        condicion.dataType.Add(dataType, true);

                        this.decision.conditionAdd(condicion);

                        this.getAllowedValues(condicion, variableManager, itemNode);
                    }
                    else
                    {
                        if (variableAsociada.Stereotype != "Conclusion")
                        {
                            variableAsociada.Stereotype = "Conclusion";
                            variableAsociada.Update();
                        }

                        Conclusion conclusion = new Conclusion(variableAsociada);

                        conclusion.businessName = variableAsociada.Name;
                        conclusion.attributeName   = variableAsociada.Alias;

                        if (variableAsociada.Name.Contains("*"))
                        {
                            conclusion.isDescrition = true;
                        }
                        conclusion.dataType.Add( dataType, true );

                        this.decision.conclusionAdd(conclusion);

                        this.getAllowedValues(conclusion, variableManager, itemNode);
                    }
                }
            }
        }

        private void getAllowedValues(Variable variable, VariableManager variableManager, XmlNode itemNode)
        {
            Variable.AllowedValue allowedValue;

            if ( variableManager.enumerationElement != null)
            {
                string defaultValue = this.eaUtils.taggedValuesUtils.get(variable.element, "Default", "").asString();

                foreach ( EA.Attribute attr in variableManager.enumerationElement.Attributes)
                {
                    allowedValue = new Variable.AllowedValue();
                    variable.allowedValues.Add(allowedValue);

                    if ( attr.Default != null && attr.Default.Length != 0)
                    {
                        allowedValue.value = attr.Default;
                        allowedValue.businessName = attr.Alias;
                        allowedValue.attributeName = attr.Name;

                        if( defaultValue != "" && defaultValue != allowedValue.value && ( defaultValue == allowedValue.businessName ||  defaultValue == allowedValue.attributeName ) )
                        {
                            variable.defaultValue = allowedValue.value;
                        }
                    }
                    else if ( attr.Alias.Length != 0)
                    {
                        allowedValue.value = attr.Alias;
                        allowedValue.businessName = attr.Alias;
                        allowedValue.attributeName = attr.Name;

                        if (defaultValue != "" && defaultValue != allowedValue.businessName && (defaultValue == allowedValue.value || defaultValue == allowedValue.attributeName))
                        {
                            variable.defaultValue = allowedValue.businessName;
                        }
                    }
                    else
                    {
                        allowedValue.value = attr.Name;
                        allowedValue.businessName = attr.Name;
                        allowedValue.attributeName = attr.Name;

                        // en este caso el default no se puede traducir a nada porque no hay con que comparar.
                        variable.defaultValue = defaultValue;
                    }
                }

                // una vez que tomamos los valores de la enumeración agregamos los valores permitidos que estén en la tabla.
                XmlNodeList allowedValues = itemNode.SelectNodes("allowable_value");
                bool addToEnum = true;
                foreach (XmlNode xmlNode in allowedValues)
                {
                    addToEnum = true;
                    foreach (Variable.AllowedValue currentAllowedValue in variable.allowedValues)
                    {
                        if(currentAllowedValue.value == xmlNode.InnerText)
                        {
                            addToEnum = false;
                            break;
                        }
                    }

                    if(addToEnum)
                    {
                        allowedValue = new Variable.AllowedValue();
                        allowedValue.value = xmlNode.InnerText;
                        variable.allowedValues.Add(allowedValue);
                    }
                }
            }
            else
            {
                XmlNodeList allowedValues = itemNode.SelectNodes("allowable_value");
                foreach ( XmlNode xmlNode in allowedValues)
                {
                    allowedValue = new Variable.AllowedValue();
                    allowedValue.value = xmlNode.InnerText;
                    variable.allowedValues.Add(allowedValue);
                }
            }
        }

        public bool buildXmlEaTable( bool silenceMode )
        {
            bool isValid = false;

            string decisionTable = this.decision.element.GetDecisionTable();

            if (decisionTable.Length != 0)
            {
                this.eaDecisionTableXml = new XmlDocument();
                try
                {
                    eaDecisionTableXml.LoadXml(decisionTable);

                    isValid = eaDecisionTableXml.SelectNodes("/decision_table/conditions/condition_item/values/value").Count != 0;
                }
                catch (Exception e)
                {
                    if (!silenceMode)
                    {
                        this.eaUtils.printOut(e.ToString());

                        Alert.Error( Properties.Resources.error_tablaDecision_obtener + " " + this.decision.element.Name);
                    }
                }
            }
            else
            {
                if (!silenceMode)
                {
                    Alert.Error( Properties.Resources.error_tablaDecision_requerida + " " + this.decision.element.Name);
                }
            }
            return isValid;
        }
    }
}