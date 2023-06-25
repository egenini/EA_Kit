using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DMN.dominio
{
    public class Variable
    {
        public EA.Element element;
        public string attributeName = null;
        public string businessName = null;
        public string value = null;
        public bool isMethod = false;
        public string language = "";
        public bool isDescrition = false;
        /// <summary>
        /// Utilizado sólo en el caso de condiciones y conclusiones, no para los conditionValues ni para los conclusionValues.
        /// </summary>
        public List<AllowedValue> allowedValues = new List<AllowedValue>();
        /// <summary>
        /// Utillizado sólo en condiciones y conclusiones.
        /// </summary>
        public string defaultValue = "";

        public Dictionary<object, bool> dataType = new Dictionary<object, bool>();

        public class AllowedValue
        {
            public string value = null;
            public string attributeName = null;
            public string businessName = null;

            public AllowedValue()
            {

            }
            public AllowedValue(string attributeName, string businessName, string value)
            {
                this.attributeName = attributeName;
                this.businessName = businessName;
                this.value = value;
            }
        }
    }
}
