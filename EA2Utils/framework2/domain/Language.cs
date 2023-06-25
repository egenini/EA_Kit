using EA;
using EAUtils.ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIResources;
using static EAUtils.ConnectorUtils;

namespace EAUtils.framework2.domain
{
    public class Language : JsLoader
    {
        public Element element;
        public int elementId;
        public Package package;
        public Dialect choosed = null;
        
        public List<Dialect> dialects = new List<Dialect>();

        // valor etiquetado con mapeo interno del lenguaje, en el caso de Java por ej: int = Integer
        public Dictionary<string, string> primitive2Object = new Dictionary<string, string>();
        public Dictionary<string, string> object2Primitive = new Dictionary<string, string>();
        public Dictionary<string, string> objectNamespace = new Dictionary<string, string>();

        public List<DataType> dataTypes = new List<DataType>();

        public Language( Package package, EAUtils eaUtils)
        {
            this.eaUtils = eaUtils;
            this.package = package;

            foreach(Element element in package.Elements)
            {
                if( element.Stereotype == "Language")
                {
                    this.element = element;
                    break;
                }
            }
            if( this.element != null)
            {
                this.name = this.element.Name;
                this.elementId = this.element.ElementID;
                Element languajeBase = getLanguageBase();

                if( languajeBase != null)
                {
                    loadJs(languajeBase, true);
                }

                loadPrimitiveObjectMapping(element);

                loadJs(element, false);
            }
        }

        private void loadPrimitiveObjectMapping(Element languageElement)
        {
            string saraza = languageElement.FQName + "--" + languageElement.ElementGUID;

            string toObject = eaUtils.taggedValuesUtils.get(languageElement, "ToObject", null).asString();

            primitive2Object.Clear();
            object2Primitive.Clear();

            if (toObject != null)
            {
                string[] keyValues = toObject.Split(',');
                string[] key_value;

                foreach (string keyValue in keyValues)
                {
                    key_value = keyValue.Split('=');

                    primitive2Object.Add(key_value[0], key_value[1]);
                    object2Primitive.Add(key_value[1], key_value[0]);
                }
            }
        }

        internal Element getLanguageBase()
        {
            List<ElementConnectorInfo> elementsAndConnectors = this.eaUtils.connectorUtils.get(this.element, ConnectorUtils.CONNECTOR__GENERALIZATION, null, null, "Language", false, null);
            Element element = null;
            if( elementsAndConnectors.Count != 0)
            {
                element = elementsAndConnectors[0].element;
            }

            return element;
        }

        internal string getDataType(string languageSource, string dataTypeSource)
        {
            string dataTypeMapped = null;

            foreach (DataType dataType in dataTypes)
            {
                dataTypeMapped = dataType.getMapped(languageSource, dataTypeSource);
                if (dataTypeMapped != null)
                {
                    break;
                }
            }

            return dataTypeMapped != null ? dataTypeMapped : dataTypeSource;
        }

        public Dictionary<string, string> getFunctions()
        {
            Dictionary<string, string> functions = new Dictionary<string, string>();

            foreach (Method function in this.element.Methods)
            {
                functions.Add(function.Name, function.Code);
            }

            return functions;
        }

        public void loadDialects()
        {
            foreach (Package package in this.package.Packages)
            {
                dialects.Add(new Dialect(package));
            }
        }

        internal bool choose()
        {
            return choose(null, null, null);
        }

        internal bool choose(Element element, string frameworkName, Language language )
        {
            bool chooseOne = false;

            this.loadDialects();

            if( this.dialects.Count == 1)
            {
                this.choosed = this.dialects[0];
                chooseOne = true;
            }
            else if( this.dialects.Count > 1)
            {
                bool gotoChosse = true;

                if( element != null)
                {
                    int howMany = 0;
                    Dialect lastDialect = null;

                    foreach (Dialect dialect in this.dialects)
                    {
                        Dictionary<string, string> dlc = this.eaUtils.taggedValuesUtils.getByPrefix(element, frameworkName + "::" + language.name +"-"+ dialect.name, null);

                        if (dlc.Count != 0)
                        {
                            lastDialect = dialect;
                            howMany++;
                        }
                    }

                    if( howMany == 1)
                    {
                        this.choosed = lastDialect;
                        this.choosed.loadArtifacts(this.eaUtils);

                        chooseOne  = true;
                        gotoChosse = false;
                    }
                }

                if( gotoChosse )
                {
                    ChooseForm selectForm = new ChooseForm();

                    foreach (Dialect dialect in this.dialects)
                    {
                        selectForm.addOption(dialect.name);
                    }

                    selectForm.ShowDialog();

                    if (selectForm.getSelected() != null)
                    {
                        string dialectSelected = selectForm.getSelected();

                        foreach (Dialect dialect in this.dialects)
                        {
                            if (dialect.name == dialectSelected)
                            {
                                this.choosed = dialect;
                                this.choosed.loadArtifacts(this.eaUtils);
                                chooseOne = true;
                                break;
                            }
                        }
                    }
                }
            }
            return chooseOne;
        }
    }
}
