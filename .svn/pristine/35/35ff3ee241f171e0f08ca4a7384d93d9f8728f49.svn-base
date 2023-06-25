using DMN.framework.domain;
using EA;
using Jint;
using Jint.Native.Json;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UIResources;

namespace DMN.framework
{
    public class Framework
    {
        public const string FRAMEWORK_GUID = "{A415BE84-BB42-4333-B7C9-42804C79E44F}";

        public const string LANGUAGES_GUID = "{E81A28F6-F3B7-4752-AE08-17D562F61298}";

        public Package frameworkPackage = null;

        public List<Language> languages = new List<Language>();

        public Language feelLanguage = null;

        /// <summary>
        /// Usado para guardar el último lenguaje usado para generar código.
        /// </summary>
        public Language languageForCodeGeneration = null;

        private EAUtils.EAUtils eaUtils = null;

        /// <summary>
        /// Usado para determinar el lenguaje para el combo de las variables.
        /// </summary>
        public Language currentLanguage = null;

        public string handlebarsJs = null;
        

        public Framework( EAUtils.EAUtils eaUtils )
        {
            try
            {
                this.handlebarsJs = System.IO.File.ReadAllText(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\localResources\handlebars.min-7b74175.js");
            }
            catch ( Exception e )
            {
                Clipboard.SetText(e.ToString());
            }
            this.eaUtils = eaUtils;
        }

        public bool isEnableToGenerateCode()
        {
            bool isEnable = false;
            foreach (Language language in languages)
            {
                if (language.name != feelLanguage.name)
                {
                    if( language.mainTemplate != null)
                    {
                        isEnable = true;
                    }
                }
            }
            return isEnable;
        }

        public void build()
        {
            frameworkPackage = this.eaUtils.repository.GetPackageByGuid(FRAMEWORK_GUID);

            Package languagesPackages = this.eaUtils.repository.GetPackageByGuid(LANGUAGES_GUID);
            Language currentLanguage;
            Element dataTypeElement;
            Element languageElement;
            Element dataTypeMapped;
            Element languageMapped;
            DataType dataType;

            if( languages.Count == 0 && languagesPackages != null )
            {
                foreach (Package languagePackage in languagesPackages.Packages)
                {
                    List<Object> languageElements = eaUtils.packageUtils.getElementFromFilter(languagePackage, "Class", "Language", null, true);

                    if (languageElements.Count != 0)
                    {
                        languageElement = (Element)languageElements[0];

                        List<ArrayList> elementsAndConnectors = eaUtils.connectorUtils.getFromConnectorFilter(false, languageElement, "Dependency", null, null, "DataType");

                        currentLanguage = new Language(languageElement);

                        if( currentLanguage.name == "FEEL")
                        {
                            this.feelLanguage = currentLanguage;
                        }

                        // para cada lenguaje cargamos los helpers
                        currentLanguage.handlebarsHelpers.Clear();

                        foreach( Method method in languageElement.Methods)
                        {
                            if ( ! currentLanguage.handlebarsHelpers.ContainsKey(method.Name) )
                            {
                                currentLanguage.handlebarsHelpers.Add(method.Name, method.Code);
                            }
                            else
                            {
                                this.eaUtils.printOut("Para el lenguaje " + currentLanguage.name +" el helper de handlebars "+ method.Name +" está repetido, elimínelo o cambie el nombre");
                            }
                        }

                        foreach (ArrayList row in elementsAndConnectors)
                        {
                            dataTypeElement = (Element)row[0];

                            dataType = new DataType(dataTypeElement);

                            currentLanguage.dataTypes.Add( dataType );

                            // agregamos los mapeos a otros lenguajes
                            dataTypeMapped = null;
                            List <ArrayList> elementsAndConnectorsForMap = eaUtils.connectorUtils.getFromConnectorFilter(dataTypeElement, "Association", null, null, "DataType");
                            foreach (ArrayList rowForMap in elementsAndConnectorsForMap)
                            {
                                dataTypeMapped = (Element)rowForMap[0];
                                languageMapped = null;
                                List <ArrayList> elementsAndConnectorsLanguageMapped = eaUtils.connectorUtils.getFromConnectorFilter(true, dataTypeMapped, "Dependency", null, null, "Language");
                                try
                                {
                                    languageMapped = (Element)elementsAndConnectorsLanguageMapped[0][0];

                                    dataType.mappings.Add(languageMapped.Name+"."+dataTypeMapped.Name, dataType.name);
                                }
                                catch (Exception) { }

                            }
                        }

                        this.languages.Add(currentLanguage);
                    }
                }
            }
        }

        public void loadTemplates(Package languagePackage)
        {
            this.build();

            languageForCodeGeneration = this.getByName(languagePackage.Name);

            string mainTemplate = null;

            languageForCodeGeneration.handlebarsTemplates.Clear();

            foreach (Package templatePackage in languagePackage.Packages)
            {
                if (templatePackage.Name.ToUpper() == "TEMPLATES")
                {
                    foreach (Element element in templatePackage.Elements)
                    {
                        if( element.Type != "Class")
                        {
                            continue;
                        }

                        if (element.Name.ToUpper() == "MAIN")
                        {
                            mainTemplate = this.eaUtils.repository.GetFormatFromField("TXT", element.Notes);
                        }
                        else
                        {
                            try
                            {
                                languageForCodeGeneration.handlebarsTemplates.Add(element.Name, this.eaUtils.repository.GetFormatFromField("TXT", element.Notes));
                            }
                            catch (Exception e)
                            {
                                this.eaUtils.printOut(e.ToString());
                                Alert.Error(e.ToString());
                            }
                        }
                    }
                    break;
                }
            }

            if (mainTemplate != null)
            {
                languageForCodeGeneration.mainTemplate = mainTemplate;
            }
        }

        internal void setCurrentLanguage(Package package)
        {
            if( package.Diagrams.Count == 1)
            {
                Diagram diagram = package.Diagrams.GetAt(0);
                if( diagram.MetaType == "DMN::DMN")
                {
                    this.setCurrentLanguage(diagram);
                }
            }
        }
        internal void setCurrentLanguage(Diagram diagram)
        {
            this.build();

            Package package = this.eaUtils.repository.GetPackageByID(diagram.PackageID);

            this.currentLanguage = null;

            foreach (Element currentElement in package.Elements)
            {
                if( currentElement.ClassifierID != 0)
                {
                    foreach(Language language in languages)
                    {
                        if( currentElement.ClassifierID == language.elementId)
                        {
                            this.currentLanguage = language;
                            break;
                        }
                    }
                }
            }

            /*
            if( this.currentLanguage == null)
            {
                Alert.Error("Este elemento no posee definido el lenguaje");
            }
            */
        }

        public Language getByName(string name)
        {
            Language language = null;

            foreach(Language currentLanguage in languages)
            {
                if( currentLanguage.name == name)
                {
                    language = currentLanguage;
                    break;
                }
            }
            return language;
        }

        internal string getDataType(string language, string dataType)
        {
            string dataTypeMapped = dataType;

            if(this.languageForCodeGeneration != null && this.languageForCodeGeneration.name != language)
            {
                dataTypeMapped = languageForCodeGeneration.getDataType(language, dataType);
            }

            return dataTypeMapped;
        }

        public Element createVariable(Element businessKnowledgeElement)
        {
            Element variable = businessKnowledgeElement.EmbeddedElements.AddNew("Anonymous", "ActivityParameter");
            // agregar tipo de dato default?

            variable.Update();

            TaggedValue tv = variable.TaggedValues.AddNew("Format", "");
            tv.Update();
            tv = variable.TaggedValues.AddNew("Default", "");
            tv.Update();
            variable.TaggedValues.Refresh();
            businessKnowledgeElement.EmbeddedElements.Refresh();

            return variable;
        }
    }
}
