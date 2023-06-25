using DMN.dominio;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EAUtils;
using EA;
using DMN.framework;

namespace DMN
{
    public class DecisionJsonBuilder : JsonProvider
    {
        private Decision decision = null;
        private EAUtils.EAUtils eaUtils;
        private Element businessKnowledge;
        private Framework framework;

        public DecisionJsonBuilder(EAUtils.EAUtils eaUtils, Element businessKnowledge, Framework framework)
        {
            this.eaUtils = eaUtils;
            this.businessKnowledge = businessKnowledge;
            this.framework = framework;
        }

        public string getJson()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            JsonWriter writer = new JsonTextWriter(sw);

            writer.Formatting = Newtonsoft.Json.Formatting.Indented;
            writer.WriteStartObject();


            //writer.WritePropertyName("functions");

            //this.writeFunctions( writer );


            writer.WritePropertyName("decision");

            writer.WriteStartObject();

            this.decision = new Decision(this.businessKnowledge);

            DecisionBuilder decisionBuilder = new DecisionBuilder(decision, eaUtils, framework);

            if (decisionBuilder.build(false))
            {
                objectToString(writer);
            }
            writer.WriteEnd();

            writer.WriteEnd();

            return sw.ToString();
        }

        private void objectToString(JsonWriter writer)
        {
            writer.WritePropertyName("businessName");
            writer.WriteValue(this.decision.businessName);

            writer.WritePropertyName("name");
            writer.WriteValue(this.decision.name);

            writer.WritePropertyName("id");
            writer.WriteValue(this.decision.id);

            writeHitPolicy(writer);

            writeCompleteness(writer);

            writeAggregation(writer);

            writeConditions(writer);

            writeConclusions(writer);

            writeRules(writer);

        }

        private void writeHitPolicy(JsonWriter writer)
        {
            // todas las opciones
            this.decision.allHitPolicys();

            writer.WritePropertyName("hitPolicy");
            writer.WriteStartObject();

            foreach (KeyValuePair<object, bool> hitPolicyKV in this.decision.allHitPolicy)
            {
                writer.WritePropertyName((string)hitPolicyKV.Key);
                writer.WriteValue(hitPolicyKV.Value);
            }

            writer.WriteEnd();

        }
        private void writeCompleteness(JsonWriter writer)
        {
            // todas las opciones
            this.decision.allCompletes();

            writer.WritePropertyName("completeness");
            writer.WriteStartObject();

            foreach (KeyValuePair<object, bool> completenessKV in this.decision.allCompleteness)
            {
                writer.WritePropertyName((string)completenessKV.Key);
                writer.WriteValue(completenessKV.Value);
            }
            writer.WriteEnd();
        }
        private void writeAggregation(JsonWriter writer)
        {
            // todas las opciones
            this.decision.allAggregations();

            writer.WritePropertyName("aggregation");
            writer.WriteStartObject();

            foreach (KeyValuePair<object, bool> aggregations in this.decision.allAggregation)
            {
                writer.WritePropertyName((string)aggregations.Key);
                writer.WriteValue(aggregations.Value);
            }
            writer.WriteEnd();
        }

        private void writeConditions(JsonWriter writer)
        {
            writer.WritePropertyName("conditions");
            writer.WriteStartArray();

            foreach (Condition condition in this.decision.conditions)
            {
                this.writeAttribute(condition, false, writer);
            }
            writer.WriteEndArray();
        }

        private void writeConclusions(JsonWriter writer)
        {
            writer.WritePropertyName("conclusions");
            writer.WriteStartArray();

            foreach (Conclusion conclusion in this.decision.conclusions)
            {
                this.writeAttribute( conclusion, false, writer);
            }

            writer.WriteEndArray();
        }

        private void writeRules(JsonWriter writer)
        {
            writer.WritePropertyName("rules");
            writer.WriteStartArray();

            foreach (Rule rule in this.decision.rules)
            {
                writer.WriteStartObject();

                writer.WritePropertyName("rule");

                writer.WriteStartObject();

                writeConditionsValue(rule, writer);
                writeConclusionsValue(rule, writer);

                writer.WriteEnd();

                writer.WriteEnd();
            }

            writer.WriteEndArray();
        }

        private void writeConditionsValue(Rule rule, JsonWriter writer)
        {
            writer.WritePropertyName("conditionValues");
            writer.WriteStartArray();

            foreach (CondicionValue conditionValue in rule.conditionValues)
            {
                this.writeAttribute(conditionValue, true, writer);
            }

            writer.WriteEndArray();
        }

        private void writeConclusionsValue(Rule rule, JsonWriter writer)
        {
            writer.WritePropertyName("conclusionValues");
            writer.WriteStartArray();

            foreach (ConclusionValue conclusionValue in rule.conclusionValues)
            {
                this.writeAttribute(conclusionValue, true, writer);
            }
            writer.WriteEndArray();
        }

        private void writeAttribute(Variable variable, bool asRuleMember, JsonWriter writer)
        {
            if( ! variable.isDescrition)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("attributeName");
                writer.WriteValue(variable.attributeName);
                writer.WritePropertyName("businessName");
                writer.WriteValue(variable.businessName);

                writer.WritePropertyName("dataType");

                writer.WriteStartObject();
                foreach (KeyValuePair<object, bool> dataTypeKV in variable.dataType)
                {
                    writer.WritePropertyName((string)dataTypeKV.Key);
                    writer.WriteValue(dataTypeKV.Value);
                }

                if (!asRuleMember)
                {
                    // agregamos format y default.
                    writer.WritePropertyName("format");
                    writer.WriteValue(this.eaUtils.taggedValuesUtils.get(variable.element, "Format", "").asString());

                    writer.WritePropertyName("default");
                    writer.WriteValue(variable.defaultValue);
                }

                writer.WriteEnd();

                if (asRuleMember)
                {
                    writer.WritePropertyName("value");
                    writer.WriteValue(variable.value);
                }
                else
                {
                    writer.WritePropertyName("allowedValues");

                    writer.WriteStartArray();

                    Variable.AllowedValue allowed;
                    foreach (object allowedValue in variable.allowedValues)
                    {
                        allowed = (Variable.AllowedValue)allowedValue;
                        writer.WriteStartObject();

                        writer.WritePropertyName("attributeName");
                        writer.WriteValue(allowed.attributeName);
                        writer.WritePropertyName("businessName");
                        writer.WriteValue(allowed.businessName);
                        writer.WritePropertyName("value");
                        writer.WriteValue(allowed.value);

                        writer.WriteEnd();
                    }
                    writer.WriteEndArray();
                }

                writer.WriteEnd();

            }
        }
    }
}