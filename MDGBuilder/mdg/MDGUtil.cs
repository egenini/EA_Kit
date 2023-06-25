using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MDGBuilder.mdg
{
    public class MDGUtil
    {
        XmlDocument mts;

        public void setMts(XmlDocument mts)
        {
            this.mts = mts;
        }

        public void addMetatype( Package package)
        {
            foreach( Element element in package.Elements)
            {
                addMetatype(element);
            }
        }

        public void addMetatype( Element element)
        {
            bool found = false;

            foreach (EA.Attribute attr in element.Attributes )
            {
                if( attr.Name == "_metatype")
                {
                    found = true;
                    break;
                }
            }

            if( ! found )
            {
                EA.Attribute attr = element.Attributes.AddNew("_metatype", "");
                attr.Default = element.Name;
                attr.Update();
            }
        }

        public void addElements(string directory, string fileName )
        {
            XmlNodeList xnList = this.mts.SelectNodes("/MDG.Selections/Profiles");
            bool found = false;

            foreach (XmlNode xn in xnList)
            {
                if (xn["files"].InnerText == fileName)
                {
                    found = true;
                }
            }

            if (!found)
            {
                XmlElement x = this.mts.CreateElement("Profiles");

                this.fillProfile(x, directory, fileName);

                mts.DocumentElement.AppendChild(x);
            }
        }

        public void addDiagram(string directory, string fileName)
        {
            XmlNodeList xnList = this.mts.SelectNodes("/MDG.Selections/DiagramProfile");
            bool found = false;

            foreach (XmlNode xn in xnList)
            {
                if (xn["files"].InnerText == fileName)
                {
                    found = true;
                }
            }

            if (!found)
            {
                XmlElement x = this.mts.CreateElement("DiagramProfile");

                this.fillProfile(x, directory, fileName);

                mts.DocumentElement.AppendChild(x);
            }
        }

        public void addToolbox(string directory, string fileName)
        {
            XmlNodeList xnList = this.mts.SelectNodes("/MDG.Selections/UIToolboxes");
            bool found = false;

            foreach (XmlNode xn in xnList)
            {
                if( xn["files"].InnerText == fileName )
                {
                    found = true;
                }
            }

            if( ! found )
            {
                XmlElement x = this.mts.CreateElement("UIToolboxes");

                this.fillProfile(x, directory, fileName);

                mts.DocumentElement.AppendChild(x);
            }
        }

        internal void fillProfile(XmlElement x, string directory, string fileName)
        {
            x.SetAttribute( "directory", directory );
            x.SetAttribute( "files"    , fileName  );
        }

    }
}
