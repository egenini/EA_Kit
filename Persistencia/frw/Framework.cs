using EA;
using EAUtils.framework2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistencia.frw
{
    class Framework : FrameworkCommons2
    {
        const string NAMESPACE_ATTR_LIST_NAME = "_namespaceDefaultAttributes";
        public const string FRAMEWORK_NAME = "Persistence";

        public Framework(EAUtils.EAUtils eaUtils) : base(eaUtils, "{8EB49DB9-CF86-4d6b-B8A2-4196AB3F2160}")
        {
            frameworkName = FRAMEWORK_NAME;
        }

        /*
        public void addNamespaceAttributes(Element artifactElement, Element namespaceElement)
        {
            EA.Attribute added;
            bool namespaceAttrExists = false;

            foreach (EA.Attribute attr in artifactElement.Attributes)
            {
                if (attr.Name == NAMESPACE_ATTR_LIST_NAME)
                {
                    namespaceAttrExists = true;
                    break;
                }
            }

            if (!namespaceAttrExists)
            {
                added = artifactElement.Attributes.AddNew(NAMESPACE_ATTR_LIST_NAME, "");
                added.Default = "namespace,plus";

                added.Update();
            }

            foreach ( EA.Attribute attr in artifactElement.Attributes)
            {
                if( attr.Name == NAMESPACE_ATTR_LIST_NAME)
                {
                    if (attr.Default != "")
                    {
                        // puede o no venir con un valor por ej: plus, apiVersion = v1, ...
                        string[] attrs = attr.Default.Split(',');
                        string[] attrNameValue;

                        foreach( string newAttr in attrs )
                        {
                            if( newAttr.Contains("="))
                            {
                                attrNameValue = newAttr.Split('=');

                                if (this.canAdd(attrNameValue[0].Trim(), namespaceElement))
                                {
                                    added = namespaceElement.Attributes.AddNew(attrNameValue[0].Trim(), "String");
                                    added.Default = attrNameValue[1].Trim();

                                    added.Update();
                                }
                            }
                            else
                            {
                                if( this.canAdd( newAttr.Trim(), namespaceElement))
                                {
                                    added = namespaceElement.Attributes.AddNew(newAttr.Trim(), "String" );
                                    added.Update();
                                }
                            }
                        }
                    }
                    break;
                }
            }
        }

        private bool canAdd( string attrName, Element namespaceElement)
        {
            bool canAdd = true;

            foreach( EA.Attribute attr in namespaceElement.Attributes)
            {
                if( attr.Name == attrName)
                {
                    canAdd = false;
                    break;
                }
            }

            return canAdd;
        }
        */
    }
}
