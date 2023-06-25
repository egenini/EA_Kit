using EA;
using Productividad.framework.domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UIResources;

namespace Productividad.framework
{
    public class Framework
    {
        public const string FRAMEWORK_GUID = "{0C048BE3-E064-4563-85AC-E09BC76744EF}";

        //public const string LANGUAGES_GUID = "{36018CC6-5F11-454e-9A7A-DFE6E2D1E48C}";

        protected string languageRootTaggedValueName    = "language_root";
        protected string languagesReloadTaggedValueName = "language_reload";

        public Package frameworkPackage = null;
        public Package languagesPackages = null;
        public List<Language> languages = new List<Language>();

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
        public string dashbarsJs = null;
        public string momentJs = null;
        public string lodashJs = null;

        public Framework(EAUtils.EAUtils eaUtils)
        {
            try
            {
                this.handlebarsJs = System.IO.File.ReadAllText(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\localResources\handlebars.min-7b74175.js");
                this.momentJs = System.IO.File.ReadAllText(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\localResources\moment-with-locales.js");
                this.dashbarsJs = System.IO.File.ReadAllText(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\localResources\dashbars.js");
                this.lodashJs = System.IO.File.ReadAllText(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\localResources\lodash.js");
                
            }
            catch (Exception e)
            {
                Clipboard.SetText(e.ToString());
            }
            this.eaUtils = eaUtils;

            frameworkPackage = this.eaUtils.repository.GetPackageByGuid(FRAMEWORK_GUID);

        }

        protected bool setLanguageRootPackage()
        {
            string languagePackageGuid = this.eaUtils.taggedValuesUtils.get(frameworkPackage.Element, this.languageRootTaggedValueName, "").asString();

            if(languagePackageGuid == "")
            {
                MessageBox.Show("Debe seleccionar el paquete que contiene los paquetes con los lenguajes");

                int falsoPackageId = this.eaUtils.repository.InvokeConstructPicker("IncludedTypes=Package");

                if(falsoPackageId != 0)
                {
                    Element packageElement = this.eaUtils.repository.GetElementByID(falsoPackageId);

                    this.languagesPackages = this.eaUtils.repository.GetPackageByGuid(packageElement.ElementGUID);
                    this.eaUtils.taggedValuesUtils.set(frameworkPackage.Element, this.languageRootTaggedValueName, this.languagesPackages.PackageGUID );
                }
            }
            else
            {
                this.languagesPackages = this.eaUtils.repository.GetPackageByGuid(languagePackageGuid);
            }

            return this.languagesPackages != null;
        }
        public void build()
        {
            if ( setLanguageRootPackage())
            {
                this.buildLanguages();
            }
        }

        public void buildLanguages()
        {
            Language currentLanguage;
            Element dataTypeElement;
            Element languageElement;
            Element dataTypeMapped;
            Element languageMapped;
            DataType dataType;

            bool? reload = this.eaUtils.taggedValuesUtils.get(this.frameworkPackage.Element, this.languagesReloadTaggedValueName, null).asBoolean();

            if ( reload == null)
            {
                this.eaUtils.taggedValuesUtils.set(this.frameworkPackage.Element, this.languagesReloadTaggedValueName, "false");
                reload = true;
            }

            if (languages.Count == 0 || reload == true)
            {
                foreach (Package languagePackage in languagesPackages.Packages)
                {
                    List<Object> languageElements = eaUtils.packageUtils.getElementFromFilter(languagePackage, "Class", "Language", null, true);

                    if (languageElements.Count != 0)
                    {
                        languageElement = (Element)languageElements[0];

                        List<ArrayList> elementsAndConnectors = eaUtils.connectorUtils.getFromConnectorFilter(false, languageElement, "Dependency", null, null, "DataType");

                        currentLanguage = new Language(languageElement);

                        // el elemento Language puede tener un valor etiquetado ToObject
                        buildPrmitives2Object(languageElement, currentLanguage);

                        loadImports(languageElement, currentLanguage);

                        // para cada lenguaje cargamos los helpers
                        currentLanguage.handlebarsHelpers.Clear();

                        foreach (Method method in languageElement.Methods)
                        {
                            if (!currentLanguage.handlebarsHelpers.ContainsKey(method.Name))
                            {
                                currentLanguage.handlebarsHelpers.Add(method.Name, method.Code);
                            }
                            else
                            {
                                this.eaUtils.printOut("Para el lenguaje " + currentLanguage.name + " el helper de handlebars " + method.Name + " está repetido, elimínelo o cambie el nombre");
                            }
                        }

                        foreach (ArrayList row in elementsAndConnectors)
                        {
                            dataTypeElement = (Element)row[0];

                            dataType = new DataType(dataTypeElement);

                            currentLanguage.dataTypes.Add(dataType);

                            // agregamos los mapeos a otros lenguajes
                            dataTypeMapped = null;
                            List<ArrayList> elementsAndConnectorsForMap = eaUtils.connectorUtils.getFromConnectorFilter(dataTypeElement, "Association", null, null, "DataType");
                            foreach (ArrayList rowForMap in elementsAndConnectorsForMap)
                            {
                                dataTypeMapped = (Element)rowForMap[0];
                                languageMapped = null;
                                List<ArrayList> elementsAndConnectorsLanguageMapped = eaUtils.connectorUtils.getFromConnectorFilter(true, dataTypeMapped, "Dependency", null, null, "Language");
                                try
                                {
                                    languageMapped = (Element)elementsAndConnectorsLanguageMapped[0][0];

                                    dataType.mappings.Add(languageMapped.Name + "." + dataTypeMapped.Name, dataType.name);
                                }
                                catch (Exception) { }

                            }
                        }

                        this.languages.Add(currentLanguage);
                    }
                }
            }
        }

        private void loadImports(Element languageElement, Language currentLanguage)
        {
            string imports = eaUtils.taggedValuesUtils.get(languageElement, "Imports", null).asString();

            currentLanguage.objectNamespace.Clear();

            if (imports != null)
            {
                string[] importLines = imports.Split(';');
                string[] importKeyValue;

                foreach( string line in importLines)
                {
                    if( ! line.Contains("="))
                    {
                        continue;
                    }

                    importKeyValue = line.Split('=');

                    currentLanguage.objectNamespace.Add(importKeyValue[0], importKeyValue[1]);
                }
            }
        }

        private void buildPrmitives2Object(Element languageElement, Language currentLanguage)
        {
            string toObject = eaUtils.taggedValuesUtils.get(languageElement, "ToObject", null).asString();

            currentLanguage.primitive2Object.Clear();

            if ( toObject != null)
            {
                string[] keyValues = toObject.Split(',');
                string[] key_value;

                foreach( string keyValue in keyValues)
                {
                    key_value = keyValue.Split('=');

                    currentLanguage.primitive2Object.Add(key_value[0], key_value[1]);
                    currentLanguage.object2Primitive.Add(key_value[1], key_value[0]);
                }
            }
        }

        public void loadTemplatesByName( string languageName)
        {
            languageForCodeGeneration = this.getByName(languageName);

            Package languagePackage = this.eaUtils.packageUtils.getChildPackageByName(languagesPackages, languageName);

            this.loadTemplates(languagePackage, false);
        }

        public void loadTemplates(Package languagePackage, bool rebuild)
        {
            if( rebuild)
            {
                this.build();
            }

            languageForCodeGeneration = this.getByName(languagePackage.Name);

            string mainTemplate = null;

            languageForCodeGeneration.handlebarsTemplates.Clear();

            foreach (Package templatePackage in languagePackage.Packages)
            {
                if (templatePackage.Name.ToUpper() == "TEMPLATES")
                {
                    foreach (Element element in templatePackage.Elements)
                    {
                        if (element.Type != "Class")
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
            if (package.Diagrams.Count == 1)
            {
                Diagram diagram = package.Diagrams.GetAt(0);
                this.setCurrentLanguage(diagram);
            }
        }
        internal void setCurrentLanguage(Diagram diagram)
        {
            this.build();

            Package package = this.eaUtils.repository.GetPackageByID(diagram.PackageID);

            this.currentLanguage = null;

            foreach (Element currentElement in package.Elements)
            {
                if (currentElement.ClassifierID != 0)
                {
                    foreach (Language language in languages)
                    {
                        if (currentElement.ClassifierID == language.elementId)
                        {
                            this.currentLanguage = language;
                            break;
                        }
                    }
                }
            }
        }

        public Language getByName(string name)
        {
            Language language = null;

            foreach (Language currentLanguage in languages)
            {
                if (currentLanguage.name == name)
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

            if (this.languageForCodeGeneration != null && this.languageForCodeGeneration.name != language)
            {
                dataTypeMapped = languageForCodeGeneration.getDataType(language, dataType);
            }

            return dataTypeMapped;
        }

    }
}
