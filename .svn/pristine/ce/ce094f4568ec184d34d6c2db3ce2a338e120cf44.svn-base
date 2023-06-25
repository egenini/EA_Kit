using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EA;
using UIResources;

namespace DMN.dominio
{
    public class Decision
    {
        // hitPolicy
        public const string HIT_POLICY__NONE         = "NONE";
        public const string HIT_POLICY__UNIQUE       = "UNIQUE";
        public const string HIT_POLICY__ANY          = "ANY";
        public const string HIT_POLICY__PRIORITY     = "PRIORITY";
        public const string HIT_POLICY__FIRST        = "FIRST";
        public const string HIT_POLICY__NO_ORDER     = "NO_ORDER";
        public const string HIT_POLICY__OUTPUT_ORDER = "OUTPUT_ORDER";
        public const string HIT_POLICY__RULE_ORDER   = "RULE_ORDER";

        public Dictionary<string, string> hitPolicyEA2Model = new Dictionary<string, string>();

        // aggregation
        public const string AGGREGATION__NONE       = "NONE";
        public const string AGGREGATION__COLLECTION = "COLLECTION";
        public const string AGGREGATION__SUM        = "SUM";
        public const string AGGREGATION__MIN        = "MIN";
        public const string AGGREGATION__MAX        = "MAX";
        public const string AGGREGATION__COUNT      = "COUNT";
        public const string AGGREGATION__AVERAGE    = "AVERAGE";

        //completeness
        public const string COMPLETENESS__NONE       = "NONE";
        public const string COMPLETENESS__COMPLETE   = "COMPLETE";
        public const string COMPLETENESS__INCOMPLETE = "INCOMPLETE";

        public string id = "";
        public string name = "";
        public string businessName = "";

        public List<Condition> conditions = new List<Condition>();
        public List<Conclusion> conclusions = new List<Conclusion>();
        public Dictionary<string, Condition> conditionByAttributeName = new Dictionary<string, Condition>();
        public Dictionary<string, Conclusion> conclusionByAttributeName = new Dictionary<string, Conclusion>();

        public List<Rule> rules = new List<Rule>();
        public Element element;

        public string hitPolicy = "";
        public string completeness = "";
        public string aggregation = "";

        public Dictionary<object, bool> allHitPolicy = new Dictionary<object, bool>();
        public Dictionary<object, bool> allCompleteness = new Dictionary<object, bool>();
        public Dictionary<object, bool> allAggregation = new Dictionary<object, bool>();

        public Decision(Element tableElement)
        {
            this.element = tableElement;
        }

        public bool isCompleted(bool silenceMode )
        {
            if (this.hitPolicy == null || this.hitPolicy == "")
            {
                this.hitPolicy = HIT_POLICY__UNIQUE;
            }

            if(this.aggregation == null || this.aggregation == "")
            {
                this.aggregation = AGGREGATION__COLLECTION;
            }

            bool conditionsOk   = this.conditions.Count  != 0;
            bool conclusionsOk  = this.conclusions.Count != 0;
            bool rulesOk        = this.rules.Count       != 0;
            bool hitPolicyOk    = this.hitPolicy         != null && this.hitPolicy    != "";
            bool aggregationOk  = this.aggregation       != null && this.aggregation  != "";
            bool completenessOk = this.completeness      != null && this.completeness != "";

            if ( ! silenceMode )
            {
                if( ! conditionsOk )
                {
                    Alert.Error( Properties.Resources.error_condiciones_incompletas );
                }
                if ( !conclusionsOk )
                {
                    Alert.Error( Properties.Resources.error_conclusiones_incompletas);
                }
                if ( ! rulesOk )
                {
                    Alert.Error( Properties.Resources.error_reglas_incompletas);

                }
                if ( ! hitPolicyOk )
                {
                    Alert.Error(Properties.Resources.error_hitPolicy);
                }
                if ( ! aggregationOk )
                {
                    Alert.Error(Properties.Resources.error_aggregation);
                }
                if ( ! completenessOk )
                {
                    Alert.Error(Properties.Resources.error_completeness);
                }
            }
            return conditionsOk && conclusionsOk && rulesOk && hitPolicyOk && completenessOk && aggregationOk;
        }

        public void conditionAdd(Condition condition)
        {
            try
            {
                conditionByAttributeName.Add(condition.attributeName, condition);
                conditions.Add(condition);
            }
            catch (Exception) { }
        }
        public void conclusionAdd(Conclusion conclusion)
        {
            try
            {
                conclusionByAttributeName.Add(conclusion.attributeName, conclusion);
                conclusions.Add(conclusion);
            }
            catch (Exception) { }
        }

        public void allAggregations()
        {
            string normalized = this.aggregation == "" ? "" : this.aggregation.ToUpper();

            this.allAggregation.Add( AGGREGATION__NONE      , normalized == AGGREGATION__NONE       );
            this.allAggregation.Add( AGGREGATION__AVERAGE   , normalized == AGGREGATION__AVERAGE    );
            this.allAggregation.Add( AGGREGATION__COLLECTION, normalized == AGGREGATION__COLLECTION );
            this.allAggregation.Add( AGGREGATION__COUNT     , normalized == AGGREGATION__COUNT      );
            this.allAggregation.Add( AGGREGATION__MAX       , normalized == AGGREGATION__MAX        );
            this.allAggregation.Add( AGGREGATION__MIN       , normalized == AGGREGATION__MIN        );
            this.allAggregation.Add( AGGREGATION__SUM       , normalized == AGGREGATION__SUM        );
        }

        public void allCompletes()
        {
            string completenessNormalized = this.completeness == "" ? "" : this.completeness.ToUpper();

            this.allCompleteness.Add( COMPLETENESS__NONE      , completenessNormalized == COMPLETENESS__NONE       );
            this.allCompleteness.Add( COMPLETENESS__COMPLETE  , completenessNormalized == COMPLETENESS__COMPLETE   );
            this.allCompleteness.Add( COMPLETENESS__INCOMPLETE, completenessNormalized == COMPLETENESS__INCOMPLETE );
        }

        public void allHitPolicys()
        {
            string hitPolicyNormalized = hitPolicy == "" ? "" : hitPolicy.ToUpper().Replace(" ", "_");

            // poner la constante en lugar del upper del dato, así como está debajo.
            this.allHitPolicy.Add( HIT_POLICY__NONE        , hitPolicyNormalized == HIT_POLICY__NONE         );
            this.allHitPolicy.Add( HIT_POLICY__ANY         , hitPolicyNormalized == HIT_POLICY__ANY          );
            this.allHitPolicy.Add( HIT_POLICY__FIRST       , hitPolicyNormalized == HIT_POLICY__FIRST        );
            this.allHitPolicy.Add( HIT_POLICY__NO_ORDER    , hitPolicyNormalized == HIT_POLICY__NO_ORDER     );
            this.allHitPolicy.Add( HIT_POLICY__OUTPUT_ORDER, hitPolicyNormalized == HIT_POLICY__OUTPUT_ORDER );
            this.allHitPolicy.Add( HIT_POLICY__PRIORITY    , hitPolicyNormalized == HIT_POLICY__PRIORITY     );
            this.allHitPolicy.Add( HIT_POLICY__RULE_ORDER  , hitPolicyNormalized == HIT_POLICY__RULE_ORDER   );
            this.allHitPolicy.Add( HIT_POLICY__UNIQUE      , hitPolicyNormalized == HIT_POLICY__UNIQUE       );

        }
    }
}
