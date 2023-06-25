using System;
using System.Collections.Generic;

namespace DMN.dominio
{
    public class Rule
    {
        public List<CondicionValue> conditionValues = new List<CondicionValue>();
        public List<ConclusionValue> conclusionValues = new List<ConclusionValue>();
        public Dictionary<string, CondicionValue> conditionValueByAttributeName = new Dictionary<string, CondicionValue>();
        public Dictionary<string, ConclusionValue> conclusionValueByAttributeName = new Dictionary<string, ConclusionValue>();

        public void conditionValueAdd( CondicionValue conditionValue)
        {
            conditionValueByAttributeName.Add( conditionValue.attributeName, conditionValue);
            conditionValues.Add(conditionValue);
        }

        public void conclusionValueAdd(ConclusionValue conclusionValue)
        {
            conclusionValueByAttributeName.Add(conclusionValue.attributeName, conclusionValue);
            conclusionValues.Add(conclusionValue);
        }
    }
}