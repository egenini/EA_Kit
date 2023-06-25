using System;
using EA;
using EAUtils;
using UserInterface.frw;
using System.Collections.Generic;
using static EAUtils.ConnectorUtils;

namespace UserInterface.html
{
    internal class Select : FormField
    {
        public List<Option>      Options       = new List<Option>();
        public List<OptionGroup> OptionsGroups = new List<OptionGroup>();

        public Select(Element element, Framework frameworkInstance, EAUtils.EAUtils eaUtils) : base(element, frameworkInstance, eaUtils)
        {
            Metatype = "Select";

            string optionsFromTV = this.eaUtils.taggedValuesUtils.get(element, "options", "").asString();

            if(optionsFromTV != "")
            {
                string[] splitted = optionsFromTV.Split(';');
                string[] labelValue;
                foreach( string split in splitted)
                {
                    if(split.Contains(":"))
                    {
                        labelValue = split.Split(':');

                        Options.Add( new Option( labelValue[0], labelValue[1] ) );
                    }
                    else
                    {
                        Options.Add( new Option(split) );
                    }
                }
            }
            else
            {
                List < ElementConnectorInfo > elementsConnectorInfo = this.eaUtils.connectorUtils.get(element, ConnectorUtils.CONNECTOR__DEPENDENCY, null, "Enumeration", null, false, null);

                if(elementsConnectorInfo.Count == 1)
                {
                    foreach (ElementConnectorInfo elementConnectorInfo in elementsConnectorInfo)
                    {
                        foreach(EA.Attribute attr in elementConnectorInfo.element.Attributes)
                        {
                            Options.Add(new Option(attr.Default, attr.Name));
                        }
                    }
                }
                else if (elementsConnectorInfo.Count > 1)
                {
                    foreach (EAUtils.ConnectorUtils.ElementConnectorInfo elementConnectorInfo in elementsConnectorInfo)
                    {
                        OptionGroup group = new OptionGroup(elementConnectorInfo.element.Alias != "" ? elementConnectorInfo.element.Alias : elementConnectorInfo.element.Name);

                        OptionsGroups.Add(group);

                        foreach (EA.Attribute attr in elementConnectorInfo.element.Attributes)
                        {
                            group.Options.Add(new Option(attr.Default, attr.Name));
                        }
                    }
                }
            }
        }
    }
}