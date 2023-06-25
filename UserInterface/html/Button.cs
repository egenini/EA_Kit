using System;
using EA;
using EAUtils;
using UserInterface.frw;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace UserInterface.html
{
    internal class Button : FormField
    {
        public Button(Element element, Framework frameworkInstance, EAUtils.EAUtils eaUtils) : base(element, frameworkInstance, eaUtils)
        {
            Metatype = "Button";
            this.setProperties();
        }

        public new void setProperties()
        {
            base.setProperties();

            // buscamos si hay links y si este apunta a otra pantalla, en ese caso hay que buscar dónde se ha generado para agregarla al json el evento onclick.
            // onclick="window.location.href='file:///D:/temp/test_ui/UI_Screen3.html'"

            List <ConnectorUtils.ElementConnectorInfo> info = this.eaUtils.connectorUtils.get(element, EAUtils.ConnectorUtils.CONNECTOR__DEPENDENCY, null, "Class", "Screen", false, null);

            if( info.Count != 0)
            {
                string jsonTaggeValueName = frameworkInstance.frameworkName + "::" + frameworkInstance.choosed.name +"-"+ frameworkInstance.choosed.choosed.name +"-file_info";

                string jsonString = this.eaUtils.taggedValuesUtils.get(info[0].element, jsonTaggeValueName, "").asString();

                if( jsonString != "")
                {
                    dynamic infoJson = JObject.Parse(jsonString);

                    string fileName = infoJson.HtmlPage.file_name;

                    if( this.events.ContainsKey("onclick") ){

                        this.events.Remove("onclick");
                    }

                    this.events.Add("onclick", "window.location.href='file:///" + fileName.Replace("\\", "/") +"'");

                }
            }
        }
    }
}