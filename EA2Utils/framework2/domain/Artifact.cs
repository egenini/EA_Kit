using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EA;
using EAUtils.saveUtils;
using EAUtils.framework2.json;

namespace EAUtils.framework2.domain
{
    public class Artifact : JsLoader
    {
        public const string JSON_TEMPLATE_AATRIBUTE_NAME = "_taggedValueDefaultContent";

        public Dictionary<string, string> handlebarsTemplates = new Dictionary<string, string>();

        private Package package;
        public string mainTemplate;
        public SaveFileInfo saveFileInfo;
        public string fileNamePrefix = "";
        public string fileNamePostfix = "";
        public bool defaultFileNameFromAlias = false;

        public Artifact(EAUtils eaUtils, Package package)
        {
            this.eaUtils = eaUtils;
            this.package = package;
            this.name = this.package.Name;

            // al crear el artefacto es para usarlo, asi que leemos los templates.
            handlebarsTemplates.Clear();

            foreach ( Element element in this.package.Elements)
            {
                if( element.Name.ToLower() == "artifact")
                {
                    loadJs(element, true);
                    loadFileInfo(element);
                }
                break;
            }

            foreach( Package child in this.package.Packages)
            {
                if( child.Name.Trim().ToLower() == "templates")
                {
                    loadTemplates(child);
                    break;
                }
            }
        }

        private void loadFileInfo( Element element)
        {
            this.saveFileInfo = new SaveFileInfo();
            this.saveFileInfo.artifactName = this.name;

            foreach( EA.Attribute attr in element.Attributes)
            {
                if( attr.Name == "extension")
                {
                    this.saveFileInfo.defaultExtension( attr.Default );
                }
                else if (attr.Name == "filter")
                {
                    this.saveFileInfo.filter(attr.Default);
                }
                else if (attr.Name == "initialDirectory")
                {
                    this.saveFileInfo.initialDirectory(attr.Default);
                }
                else if (attr.Name == "fileNamePrefix")
                {
                    this.fileNamePrefix = attr.Default;
                }
                else if (attr.Name == "fileNamePostfix")
                {
                    this.fileNamePostfix = attr.Default;
                }
                else if (attr.Name == "defaultFileNameFromAlias")
                {
                    string value = attr.Default.ToLower();

                    this.defaultFileNameFromAlias = value == "true" || value == "1" || value == "si" || value == "yes" ? true : false;
                }
            }
        }

        private void loadTemplates(Package templatePackage)
        {
            this.loadTemplates(templatePackage, false);
        }

        private void loadTemplates( Package templatePackage, bool templateNameOrAlias)
        {
            string templateName;

            foreach ( Element element in templatePackage.Elements)
            {
                if(element.Stereotype == "Template")
                {
                    templateName = templateNameOrAlias && element.Alias != "" ? element.Alias : element.Name;

                    this.addTemplate(templateName, element.Notes);
                }
            }
        }

        public void loadTemplates( TemplateInfo templateInfo)
        {
            if( templateInfo.package.isEmpty() )
            {
                foreach( TemplateReplace info in templateInfo.templates)
                {
                    overwriteTemplates(info.guid);
                }
            }
            else
            {
                this.loadTemplates( this.eaUtils.repository.GetPackageByGuid( templateInfo.package.guid ), true);
            }
        }

        /// <summary>
        /// Reemplaza un template con otro usando el Alias (si tiene) o el Name del elemento.
        /// </summary>
        /// <param name="guid"></param>
        public void overwriteTemplates(string guid)
        {
            Element template;
            string templateName;
            template = this.eaUtils.repository.GetElementByGuid(guid);

            if( template != null)
            {
                templateName = template.Alias != "" ? template.Alias : template.Name;

                this.addTemplate(templateName, template.Notes);
            }
        }

        public void addTemplate( String templateName, string templateContent)
        {

            if (templateName.ToLower() == "main")
            {
                mainTemplate = this.eaUtils.repository.GetFormatFromField("TXT", templateContent);
            }
            else
            {
                if (handlebarsTemplates.ContainsKey(templateName))
                {
                    handlebarsTemplates.Remove(templateName);
                }

                handlebarsTemplates.Add(templateName, this.eaUtils.repository.GetFormatFromField("TXT", templateContent));
            }
        }

        public bool hasTemplates()
        {
            return handlebarsTemplates.Count != 0;

        }
    }
}
