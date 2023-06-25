using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDGBuilder.mdg
{
    public class ModelUtil
    {
        EAUtils.EAUtils eaUtils;
        public StereotypeInfo info = new StereotypeInfo();

        public ModelUtil( EAUtils.EAUtils eaUtils)
        {
            this.eaUtils = eaUtils;
        }

        public void reset()
        {
            info = new StereotypeInfo();
        }

        public Element getMetaClass(Element metaclassElement, string mdgName)
        {
            EA.Element element;
            element = null;
            Element metaclassFound;

            if (metaclassElement.ClassifierID != 0)
            {
                info.stereotypeInstance = metaclassElement;

                metaclassElement = this.eaUtils.repository.GetElementByID(metaclassElement.ClassifierID);

                info.setStereotype(metaclassElement);
            }

            if (metaclassElement.FQStereotype.StartsWith(mdgName) || metaclassElement.Stereotype == "stereotype" || metaclassElement.Stereotype == "metaclass")
            {
                foreach (EA.Connector connector in metaclassElement.Connectors)
                {
                    if (connector.ClientID != metaclassElement.ElementID)
                    {
                        continue;
                    }

                    if (connector.Type == "Extension")
                    {
                        element = this.eaUtils.repository.GetElementByID(connector.SupplierID);

                        info.metaclass = element;

                        break;
                    }
                    else if (connector.Type == "Generalization")
                    {
                        info.metaStereotype.Add(metaclassElement);

                        metaclassFound = getMetaClass(this.eaUtils.repository.GetElementByID(connector.SupplierID), mdgName);

                        if (metaclassFound != null && metaclassFound.Stereotype == "metaclass")
                        {
                            element = metaclassFound;
                            break;
                        }
                    }
                }
            }
            else
            {
                info.metaclass = metaclassElement;
                info.setStereotype(metaclassElement);
                element = metaclassElement;
            }
            return element;
        }
    }

    public class StereotypeInfo
    {
        private Element stereotype;

        /// <summary>
        /// El primer nivel en la jerarquía. 
        /// </summary>
        public Element metaclass = null;
        
        /// <summary>
        /// Un estereotipo puede heredar de otros estereotipos, esta lista representa eso.
        /// </summary>
        public List<Element> metaStereotype = new List<Element>();

        /// <summary>
        /// Por alguna razón dentro del modelado se usan instancias de los esterotipos que se están definiendo, por ej al definir el Quicklinker.
        /// </summary>
        public Element stereotypeInstance = null;

        public Element deepTypeAsElement()
        {
            return metaclass != null ? metaclass : metaStereotype.Count != 0 ? metaStereotype[metaStereotype.Count - 1] : null;
        }

        public string deepType()
        {
            string type = "";

            if( metaclass != null)
            {
                if( metaclass.Stereotype == "metaclass")
                {
                    type = metaclass.Name;
                }
                else
                {
                    type = metaclass.Type;
                }
            }
            else if( metaStereotype.Count != 0)
            {
                type = metaStereotype[metaStereotype.Count - 1].Name;
            }
            return type;
        }

        public void setStereotype(Element element)
        {
            this.stereotype = element;
        }

        public string getStereotype()
        {
            string stereotype = "";

            if( this.stereotype != null)
            {

                if( this.stereotype.Stereotype == "stereotype" || this.stereotype.Stereotype == "metaclass")
                {
                    stereotype = this.stereotype.Name;
                }
                else
                {
                    stereotype = this.stereotype.Stereotype;
                }
            }
            return stereotype;
        }
    }
}
