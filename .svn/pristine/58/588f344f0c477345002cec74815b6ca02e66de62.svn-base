using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using EAUtils;
using System.Collections;
using System.Windows.Forms;
using DMN.dominio;
using DMN.framework;

namespace DMN
{
    public class ModelManager
    {
        private EAUtils.EAUtils eaUtils;

        public ModelManager(EAUtils.EAUtils eaUtils)
        {
            this.eaUtils = eaUtils;
        }
        
        public void createEnumeration(Element enumerationElement, Framework framework)
        {
            // hay que buscar esto en la tabla de decision para obtener los valores utilizados en las reglas sea como condición o como conclusión.
            framework.build();

            // buscamos a su parent
            Element tableElement = eaUtils.repository.GetElementByID(enumerationElement.ParentID);

            Decision decision = new Decision(tableElement);

            DecisionBuilder decisionBuilder = new DecisionBuilder(decision, eaUtils, framework);
            if (decisionBuilder.build(false))
            {
                EnumExporter enumExporter = new EnumExporter(eaUtils);

                enumExporter.export(enumerationElement, decision);
            }
        }
        
        internal void exportCode(Element tableElement, Framework framework)
        {
            Decision decision = new Decision(tableElement);
            DecisionJsonBuilder jsonBuilder = new DecisionJsonBuilder(this.eaUtils, decision.element, framework);
            //DecisionBuilder decisionBuilder = new DecisionBuilder(decision, eaUtils,framework);

            string decsionJson = jsonBuilder.getJson();

            //if (decisionBuilder.build(false))
            //{
                CodeExporter codeExporter = new CodeExporter(decsionJson, framework, eaUtils);
                codeExporter.export();
            //}
        }
    }
}
