using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EAUtils;
using System.IO;
using System.Windows.Forms;
using DMN.dominio;
using UIResources;
using DMN.framework;
using Jint;
using Jint.Native.Json;

namespace DMN
{
    class CodeExporter
    {
        private EAUtils.EAUtils eaUtils;
        private Decision decision;
        private Framework framework;
        string decisionJson;
        public CodeExporter(Decision decision, Framework framework, EAUtils.EAUtils eaUtils )
        {
            this.decision  = decision;
            this.framework = framework;
            this.eaUtils   = eaUtils;
        }

        public CodeExporter(string decisionJson, Framework framework, EAUtils.EAUtils eaUtils)
        {
            this.decisionJson = decisionJson;
            this.framework = framework;
            this.eaUtils = eaUtils;
        }

        public void export(  )
        {
            Engine jsEngine = new Engine();
            var data = new JsonParser(jsEngine).Parse(this.decisionJson);

            //var data = Newtonsoft.Json.Linq.JObject.Parse(jsonBuilder.getJson());

            /*
            var data = new
            {
                decision = decision.asVar()
            };
            */

            //var gg = data.decision.conditions[0].attributeName;

            try
            {
                /*
                var mustache = File.ReadAllText(@"d:\docs\EA\JS\mustache_original.js");
                jsEngine.Execute(mustache);

                jsEngine.SetValue("data", data);
                jsEngine.SetValue("mainTemplate", framework.languageCache.mainTemplate);
                jsEngine.SetValue("partials", framework.languageCache.partialsTemplates);

                var result = jsEngine.Execute("Mustache.render(mainTemplate, data, partials)").GetCompletionValue();
                */

                jsEngine.Execute(this.framework.handlebarsJs);

                jsEngine.SetValue("OPERATION__LESS", "<");
                jsEngine.SetValue("OPERATION__LESS_OR_EQUALS", "<=");
                jsEngine.SetValue("OPERATION__GREATER", ">");
                jsEngine.SetValue("OPERATION__GREATER_OR_EQUALS", ">=");
                jsEngine.SetValue("OPERATION__EQUALS", "=");

                jsEngine.SetValue( "data" , data);
                jsEngine.SetValue("mainTemplate", framework.languageForCodeGeneration.mainTemplate);

                foreach (EA.Method method in this.framework.feelLanguage.element.Methods)
                {
                    if (method.Alias != "Handlebars.registerHelper")
                    {
                        jsEngine.Execute(method.Code);
                    }
                }

                foreach (EA.Method method in this.framework.languageForCodeGeneration.element.Methods)
                {
                    if (method.Alias != "Handlebars.registerHelper")
                    {
                        jsEngine.Execute(method.Code);
                    }
                }

                foreach (EA.Method method in this.framework.feelLanguage.element.Methods)
                {
                    if (method.Alias == "Handlebars.registerHelper")
                    {
                        jsEngine.Execute("Handlebars.registerHelper('" + method.Name + "', " + method.Code + " );");
                    }
                }

                foreach ( EA.Method method  in this.framework.languageForCodeGeneration.element.Methods)
                {
                    if( method.Alias == "Handlebars.registerHelper")
                    {
                        jsEngine.Execute("Handlebars.registerHelper('" + method.Name + "', "+ method.Code +" );");
                    }
                }

                foreach ( KeyValuePair<string, string> htemplatesKV in this.framework.languageForCodeGeneration.handlebarsTemplates)
                {
                    jsEngine.SetValue(htemplatesKV.Key +"_template", htemplatesKV.Value);

                }

                foreach (KeyValuePair<string, string> htemplatesKV in this.framework.languageForCodeGeneration.handlebarsTemplates)
                {
                    jsEngine.Execute("Handlebars.registerPartial('" + htemplatesKV.Key + "', " + htemplatesKV.Key + "_template" + " );");
                }

                jsEngine.Execute("var template = Handlebars.compile(mainTemplate);");
                var result = jsEngine.Execute("template(data)").GetCompletionValue();

                //var result = Handlebars.Run("main", data);
                if (result == null || result == "")
                {
                    result = "no se pudo generar el código";
                    Alert.Info(result.ToString());
                }
                else
                {
                    Clipboard.SetText(result.ToString());
                    Alert.Success(Properties.Resources.copiado_portapapeles);

                }
            }
            catch ( Exception e)
            {
                Clipboard.SetText(e.ToString());
                eaUtils.printOut(e.ToString());
                Alert.Error(Properties.Resources.error_indeterminado);
            }

            //var result = this.framework.languageCache.template(data);



        }

        /*
        private string getBody()
        {
            string body = @"";

            //if( decision.hitPolicy == "UNIQUE")
            //{
                // if - else if por cada linea
                // cada if va con && con la otra condicion

                for( int r = 0; r < decision.rules.Count; r++)
                {
                    if (r != 0)
                    {
                        body += " else ";
                    }

                    body += "if (";

                    // por cada regla vamos con las condiciones
                    for (int c = 0; c < decision.conditions.Count; c++)
                    {
                        body += this.getCondicion(decision.conditions[c], decision.rules[r].conditionValues[c]);
                        
                    }
                    body += "){\r\n";

                    for (int c = 0; c < decision.conclusions.Count; c++)
                    {
//                     response.put("discountPerc", 5);
                    body += "response.put(\"" + decision.conclusions[c].attributeName +"'\","+ decision.rules[r].conclusionValues[c].value +");\r\n";
                    }
                    body += "}\r\n";
                }
            //}

            return body;
        }
        */
        /*
        public string getCondicion(Condicion condicion, CondicionValue valor)
        {
            string cond = @condicion.attributeName;

            string tipoDato = condicion.type.ToUpper();

            if ( tipoDato == "STRING")
            {
                cond += @".equals(""" + valor.value +'"'+")";
            }
            else if( tipoDato == "INT" || tipoDato == "INTEGER" || tipoDato == "DOUBLE")
            {
                cond += " == " + valor;
            }
            else
            {
                cond += @".equals(""" + valor.value + '"' + ")";
            }
            return cond;
        }
        */
        /*
        public string getImports()
        {
            string imports = "";

            

            return imports;
        }
        */
        /*
        private string getParameters()
        {
            string parameters = "";

            foreach ( Condicion condicion in decision.conditions )
            {
                parameters += condicion.type +" "+ condicion.attributeName + ",";
            }

            return parameters.Substring(0, parameters.Length - 1);

        }
        */
    }
}
