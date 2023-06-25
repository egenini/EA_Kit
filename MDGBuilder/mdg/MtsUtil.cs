using EA;
using EAUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MDGBuilder.mdg
{
    public class MtsUtil
    {
        public XmlUtils xmlMts;
        public EAUtils.EAUtils eaUtils;

        public MtsUtil(XmlUtils xmlMts)
        {
            this.xmlMts = xmlMts;
        }

        public MtsUtil()
        {
            xmlMts = new XmlUtils();
        }

        public void create(Package mdgPackage, Element mtsElement, string mdgName)
        {
            xmlMts.createDOM();

            XmlElement rootElement = xmlMts.xmlDOM.CreateElement("MDG.Selections");
            xmlMts.xmlDOM.AppendChild(rootElement);

            rootElement.SetAttribute("model", eaUtils.repository.ConnectionString);

            XmlElement technologyElement = xmlMts.xmlDOM.CreateElement("Technology");

            string mdgId = mtsElement.Alias;

            if( mdgId == "")
            {
                mdgId = (mdgName.Length > 12 ? mdgName.Substring(0, 12).Replace(" ", "_") : mdgName.Replace(" ", "_"));

                mtsElement.Alias = mdgId;
                mtsElement.Update();
            }
            else if(mdgId.Length > 12)
            {
                mdgId = mdgId.Substring(0, 12).Replace(" ", "_");

                mtsElement.Alias = mdgId;
                mtsElement.Update();
            }

            mtsElement.Version = "1.0.0";

            mtsElement.Update();

            technologyElement.SetAttribute("id", mdgId);
            technologyElement.SetAttribute("name", mtsElement.Name);
            technologyElement.SetAttribute("version", mtsElement.Version);
            technologyElement.SetAttribute("notes", mtsElement.Version);
            technologyElement.SetAttribute("filename", mdgPackage.Element.Genfile.Replace(".mts", ".xml"));

            foreach (EA.Attribute attr in mtsElement.Attributes)
            {
                if (attr.Name == "infoURI")
                {
                    technologyElement.SetAttribute("infoURI", attr.Default);
                }

                if (attr.Name == "supportURI")
                {
                    technologyElement.SetAttribute("supportURI", attr.Default);
                }
            }

            rootElement.AppendChild(technologyElement);
        }

        public void rename(Package mdgPackage, Element mtsElement, string mdgOldName, string mdgOldNameU, string mdgNewName, string mdgNewNameU)
        {
            XmlNode technologyNode = null;
            bool mtsLoaded = false;

            if (mdgPackage.Element.Genfile != "")
            {
                try
                {
                    if( System.IO.File.Exists(mdgPackage.Element.Genfile))
                    {
                        xmlMts.loadFromFile(mdgPackage.Element.Genfile);

                        mtsLoaded = true;

                        try
                        {
                            technologyNode = xmlMts.xmlDOM.SelectSingleNode("/MDG.Selections/Technology");

                            if (technologyNode != null)
                            {
                                technologyNode.Attributes["name"].Value = technologyNode.Attributes["name"].Value.Replace(mdgOldName, mdgNewName).Replace(mdgOldNameU, mdgNewNameU);
                                technologyNode.Attributes["filename"].Value = mdgPackage.Element.Genfile.Replace(".mts", ".xml");
                            }
                        }
                        catch (Exception e)
                        {
                            this.eaUtils.printOut(e.ToString());
                        }
                    }
                }
                catch (Exception e) {
                    this.eaUtils.printOut(e.ToString());
                }
            }

            string mdgId = mtsElement.Alias;

            if (mdgId == "")
            {
                mdgId = (mdgNewName.Length > 12 ? mdgNewName.Substring(0, 12).Replace(" ", "_") : mdgNewName.Replace(" ", "_"));

                mtsElement.Alias = mdgId;
                mtsElement.Update();
            }
            else if (mdgId.Length > 12)
            {
                mdgId = mdgId.Substring(0, 12).Replace(" ", "_");

                mtsElement.Alias = mdgId;
                mtsElement.Update();
            }

            if (technologyNode != null)
            {
                technologyNode.Attributes["id"].Value = mdgId;
            }

            if (mtsLoaded)
            {
                try
                {
                    XmlNodeList modelsNodeList = xmlMts.xmlDOM.SelectNodes("/MDG.Selections/ModelTemplates/Model");

                    foreach (XmlNode modelNode in modelsNodeList)
                    {
                        modelNode.Attributes["name"].Value = modelNode.Attributes["name"].Value.Replace(mdgOldName, mdgNewName).Replace(mdgOldNameU, mdgNewNameU);
                        modelNode.Attributes["location"].Value = modelNode.Attributes["location"].Value.Replace(mdgOldName, mdgNewName).Replace(mdgOldNameU, mdgNewNameU);
                    }

                    // el resto de los "profiles" no es necesario porque se vuelven a crear cada vez.
                    xmlMts.xmlDOM.Save(mdgPackage.Element.Genfile);
                }
                catch (Exception) { }
            }
        }

        public void importFromMts( string mtsFileName, Package mdgPackage, Element mtsElement )
        {
            XmlNode technologyNode = null;

            mdgPackage.Element.Genfile = mtsFileName;

            xmlMts.loadFromFile(mtsFileName);

            technologyNode = xmlMts.xmlDOM.SelectSingleNode("/MDG.Selections/Technology");

            if (technologyNode != null)
            {
                mdgPackage.Name = technologyNode.Attributes["name"].Value;

                if (mdgPackage.Diagrams.Count != 0)
                {
                    Diagram d = mdgPackage.Diagrams.GetAt(0);
                    d.Name = mdgPackage.Name;
                    d.Update();
                }

                mtsElement.Name = mdgPackage.Name;
                mtsElement.Alias = technologyNode.Attributes["id"].Value;
                mtsElement.Version = technologyNode.Attributes["version"].Value;
                mtsElement.Notes = technologyNode.Attributes["notes"].Value;

                mtsElement.Update();


                this.mtsImportValorAtributo( mtsElement, "infoURI"   , technologyNode );
                this.mtsImportValorAtributo( mtsElement, "supportURI", technologyNode );
                this.mtsImportValorAtributo( mtsElement, "logo"      , technologyNode );
                this.mtsImportValorAtributo( mtsElement, "icon"      , technologyNode );

                mtsElement.Attributes.Refresh();
            }

            mdgPackage.Update();
        }

        public void mtsImportValorAtributo( Element mtsElement, string name, XmlNode technologyNode)
        {
            foreach (EA.Attribute attr in mtsElement.Attributes)
            {
                if( attr.Name == name)
                {
                    attr.Default = technologyNode.Attributes[name] != null ? technologyNode.Attributes[name].Value : "";
                    attr.Update();
                    break;
                }
            }
        }
    }
}
