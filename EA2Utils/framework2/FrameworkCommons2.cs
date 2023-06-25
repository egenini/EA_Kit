using EA;
using EAUtils.framework2;
using EAUtils.framework2.domain;
using EAUtils.framework2.json;
using EAUtils.saveUtils;
using EAUtils.ui;
using Jint;
using Jint.Runtime.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UIResources;

namespace EAUtils.framework2
{
    public class FrameworkCommons2
    {
        const string NAMESPACE_ATTR_LIST_NAME = "_namespaceDefaultAttributes";

        public List<Language> languages = new List<Language>();
        public string handlebarsJs = null;
        public string dashbarsJs = null;
        public string momentJs = null;
        public string lodashJs = null;
        public EAUtils eaUtils;
        public Package frameworkPackage;
        public Language choosed = null;
        public string frameworkName;

        Engine jsEngine = null;

        public FrameworkCommons2(EAUtils eaUtils, string frameworkGuid )
        {
            this.eaUtils = eaUtils;

            try
            {
                this.frameworkPackage = this.eaUtils.repository.GetPackageByGuid(frameworkGuid);                
            }
            catch (Exception e)
            {
                Clipboard.SetText(e.ToString());
            }

            try
            {
                this.handlebarsJs = System.IO.File.ReadAllText(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\localResources\handlebars.min-v4.7.7.js");
                //this.handlebarsJs = System.IO.File.ReadAllText(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\localResources\handlebars.min-7b74175.js");
                this.momentJs     = System.IO.File.ReadAllText(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\localResources\moment-with-locales.js");
                this.dashbarsJs   = System.IO.File.ReadAllText(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\localResources\dashbars.js");
                this.lodashJs     = System.IO.File.ReadAllText(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\localResources\lodash.js");
            }
            catch (Exception e)
            {
                Clipboard.SetText(e.ToString());
            }

        }

        public bool choose()
        {
            return choose(null);
        }

        public bool choose(Element element)
        {
            bool chooseOne = false;

            this.loadLanguages();

            if(languages.Count == 1)
            {
                this.choosed = this.languages[0];
                chooseOne = true;
            }
            else if(languages.Count > 1)
            {
                bool gotoChosse = true;

                if ( element != null)
                {
                    int howMany = 0;
                    Language lastLanguage = null;

                    foreach (Language language in this.languages)
                    {
                        Dictionary<string, string> lgg = this.eaUtils.taggedValuesUtils.getByPrefix(element, this.frameworkPackage.Name + "::"+ language.name, null);

                        lastLanguage = language;

                        if( lgg.Count != 0)
                        {
                            howMany++;
                        }
                    }

                    if( howMany == 1)
                    {
                        this.choosed = lastLanguage;
                        chooseOne = true;
                        gotoChosse = false;
                    }
                }

                if (gotoChosse)
                {
                    // elige lenguaje y dialecto.
                    ChooseForm selectForm = new ChooseForm();

                    foreach (Language language in this.languages)
                    {
                        selectForm.addOption(language.name);
                    }

                    selectForm.ShowDialog();

                    if (selectForm.getSelected() != null)
                    {
                        string languageSelected = selectForm.getSelected();

                        foreach (Language language in this.languages)
                        {
                            if (language.name == languageSelected)
                            {
                                this.choosed = language;
                                chooseOne = true;
                                break;
                            }
                        }
                    }
                }
            }
            if( chooseOne)
            {
                // le solicita al lengguaje elegido que se seleccione el dialecto.
                chooseOne = this.choosed.choose(element, this.frameworkName, this.choosed);
            }
            return chooseOne;
        }

        public void loadLanguages()
        {
            foreach( Package languagesPackage in this.frameworkPackage.Packages)
            {
                if(languagesPackage.Name.ToLower() == "languages")
                {
                    foreach( Package languagePackage in languagesPackage.Packages)
                    {
                        languages.Add(new Language(languagePackage, eaUtils));
                    }
                }
            }
        }

        public void save()
        {

            SaveFileInfo fileInfo = new SaveFileInfo();
            SaveDialogResolverUtil saveResolver = new SaveDialogResolverUtil();

            // el resolver graba lo que está el portapapeles.
            bool save = saveResolver.resolve(fileInfo);
        }

        private void buildJsEngine()
        {
            jsEngine = new Engine(cfg => cfg.AllowClr(typeof(EAUtils).Assembly));

            jsEngine.Execute(this.lodashJs);
            jsEngine.Execute(this.momentJs);
            jsEngine.Execute(this.handlebarsJs);
            jsEngine.Execute(this.dashbarsJs);
        }

        public bool generate(Jsonable info, Element element)
        {
            String json = info.stringfity();

            bool generated = false;

            if (json != "")
            {
                this.setFrwJson(element, json);

                generated = this.generate(json, element);
            }
            return generated;
        }

        public bool generate(string json, Element element)
        {
            bool generated = false;

            this.buildJsEngine();

            // se sacamos el } del final para agregar más objetos.
            json = json.Trim();
            json = json.Substring(0, json.Length - 1);

            string lng = getLngJsonFromTaggedValue(element);

            if (lng != "{}")
            {
                this.eaUtils.printOut(Properties.Resources.FRK_DATA_LNG_AGREGADO);

                json = json + ",\"language_data\":" + lng;
            }

            string dlc = getDlcJsonFromTaggedValue(element);

            if (dlc != "" && dlc != "{}")
            {
                this.eaUtils.printOut(Properties.Resources.FRK_DATA_DLC_AGREGADO);
                json = json + ",\"dialect_data\":" + dlc;
            }

            json = json + "}";

            //Clipboard.SetText(json);

            // buscar los valores etquiquetados del lenguaje.
            var data = new Jint.Native.Json.JsonParser(jsEngine).Parse(json);

            jsEngine.SetValue("data", data);

            // esta variable es para ir acumulando todo lo generado para luego meterlo en el clippboard.
            string allGeneratedBuffer = "";

            try
            {
                SaveDialogResolverUtil saveResolver = new SaveDialogResolverUtil();

                this.eaUtils.printOut( Properties.Resources.FRK_JS_FUNCTION_AGREGADO                    );
                this.eaUtils.printOut("cargando "+ this.choosed.javaScriptFunctions.Count +" funciones" );

                foreach (string jsFunction in this.choosed.javaScriptFunctions)
                {
                    jsEngine.Execute(jsFunction);
                }

                // el lenguaje puede tener definidos helpers y funciones por lo que las cargamos.
                this.eaUtils.printOut(Properties.Resources.FRK_HANDLEBARS_HELPER_AGREGADO);

                foreach (KeyValuePair<string, string> entry in this.choosed.handlebarsHelpers)
                {
                    this.eaUtils.printOut("Cargando helper " + entry.Key);

                    jsEngine.Execute("Handlebars.registerHelper('" + entry.Key + "', " + entry.Value + " );");
                }

                // a partir de este punto hay que buscar la info para generar los files a partir de cada artefacto.
                this.eaUtils.printOut(Properties.Resources.FRK_ARTIFACT_LOADING);

                this.choosed.choosed.loadArtifacts(this.eaUtils);

                // obtenemos lo definido en el json del dialecto.
                DialectInfo dialectInfo = new DialectInfo(this.choosed.choosed.package);
                dialectInfo.loadArtifacts();

                string dialectFilePreferencesString = getDialectFilePreferencesFromTaggedValue(element);
                if(dialectFilePreferencesString != "" && dialectFilePreferencesString != "{}")
                {
                    dialectInfo.filePreference.parse(dialectFilePreferencesString);
                }

                string jsonStrArtifact;
                string artifactObjectName;

                foreach (Artifact artifact in this.choosed.choosed.artifacts)
                {
                    // por cada artefacto hay que generar un archivo, antes hay que cargar helpers y funciones

                    // tomamos file info para ver si tenemos que generar

                    dialectInfo.filePreference.fill(artifact);

                    this.eaUtils.printOut(Properties.Resources.FRK_ARTIFACT_EVALUATING);

                    this.eaUtils.printOut(artifact.name);

                    if ( ! artifact.saveFileInfo.write && ! artifact.saveFileInfo.pasteClipboard )
                    {
                        this.eaUtils.printOut( String.Format( Properties.Resources.FRK_ARTIFACT_SKIP, artifact.name));
                        continue;
                    }

                    this.eaUtils.printOut(Properties.Resources.FRK_ARTIFACT_LOADING_TEMPLATES);
                    // el main template se puede "sobreescribir si el elemento tiene un valor etiquetado con el nombre del
                    // nuevo main template.
                    TemplateInfo templateInfo = getTemplateFromTaggedValue(element, artifact);

                    this.eaUtils.printOut(Properties.Resources.FRK_ARTIFACT_LOADING_TEMPLATE_FROM_TV);

                    artifact.loadTemplates(templateInfo);

                    jsEngine.SetValue("mainTemplate", artifact.mainTemplate);

                    this.eaUtils.printOut(Properties.Resources.FRK_SETTING_JSE_MAIN_TEMPLATE);

                    foreach (string jsFunction in artifact.javaScriptFunctions)
                    {
                        jsEngine.Execute(jsFunction);
                    }
                    this.eaUtils.printOut(Properties.Resources.FRK_SETTING_JSE_JS_FUNCTIONS);

                    foreach (KeyValuePair<string, string> entry in artifact.handlebarsHelpers)
                    {
                        this.eaUtils.printOut(Properties.Resources.FRK_SETTING_JSE_HB_HELPERS +" "+ entry.Key);

                        jsEngine.Execute("Handlebars.registerHelper('" + entry.Key + "', " + entry.Value + " );");
                    }

                    foreach (KeyValuePair<string, string> htemplatesKV in artifact.handlebarsTemplates)
                    {
                        this.eaUtils.printOut(Properties.Resources.FRK_SETTING_JSE_HB_TEMPLATE + " " + htemplatesKV.Key);

                        jsEngine.SetValue(htemplatesKV.Key + "_template", htemplatesKV.Value);

                        jsEngine.Execute("Handlebars.registerPartial('" + htemplatesKV.Key + "', " + htemplatesKV.Key + "_template" + " );");
                    }

                    jsEngine.Execute("var template = Handlebars.compile(mainTemplate);");

                    // antes de aplicar el template buscamos si hay valor etiquetado con datos para el artefacto.
                    jsonStrArtifact = this.getArtifactFromTaggedValue(element, artifact);

                    if (jsonStrArtifact != "{}")
                    {
                        this.eaUtils.printOut(Properties.Resources.FRK_SETTING_JSE_JSON_ARTIFACT + " " + artifact.name);

                        var jsonArtifact = new Jint.Native.Json.JsonParser(jsEngine).Parse(jsonStrArtifact);

                        artifactObjectName = this.choosed.name +"_"+ this.choosed.choosed.name +"_"+ artifact.name;

                        this.eaUtils.printOut("data." + artifactObjectName);

                        jsEngine.SetValue( artifactObjectName, jsonArtifact );

                        jsEngine.Execute( "data." + artifactObjectName + " = " + artifactObjectName );
                    }

                    var result = jsEngine.Execute("template(data)").GetCompletionValue();

                    //var result = Handlebars.Run("main", data);
                    if (result == null || result == "")
                    {
                        result = "no se pudo generar el código";

                        this.eaUtils.printOut(Properties.Resources.FRK_GENERATION_ERROR +" "+ artifact.name);
                    }
                    else
                    {
                        generated = true;

                        this.eaUtils.printOut(Properties.Resources.FRK_GENERATION_OK + " " + artifact.name);

                        if ( artifact.saveFileInfo.pasteClipboard )
                        {
                            allGeneratedBuffer +=   "**********************************\r\n"
                                                  + "* " + artifact.name + "\r\n"
                                                  + "**********************************\r\n"
                                                  + result.ToString() + "\r\n"; 
                        }

                        artifact.saveFileInfo.fileContent = result.ToString();
                        if (artifact.saveFileInfo.write)
                        {
                            if (artifact.saveFileInfo.fileName() == "")
                            {
                                artifact.saveFileInfo.fileName(artifact.fileNamePrefix + (artifact.defaultFileNameFromAlias ? (element.Alias == "" ? element.Name : element.Alias) : element.Name) + artifact.fileNamePostfix);
                            }

                            saveResolver.resolve(artifact.saveFileInfo);

                            this.eaUtils.printOut(Properties.Resources.FRK_GENERATION_SAVE + " " + artifact.name);

                            dialectInfo.filePreference.updateFileName(artifact);
                        }
                    }
                }

                if(allGeneratedBuffer != "")
                {
                    Clipboard.SetText(allGeneratedBuffer);
                }

                setFrwJsonFull(element, json);

                if( dialectInfo.filePreference.needUpdateTaggedValue)
                {
                    // si alguno de los filename se modificó entonces hay que guardar eso
                    string fileInfoTvName = this.frameworkPackage.Name + "::" + this.choosed.name + "-" + this.choosed.choosed.name + "-file_info";

                    this.eaUtils.taggedValuesUtils.set(element, fileInfoTvName, dialectInfo.filePreference.stringfity(), true, true);
                }

            }
            catch (Exception e)
            {
                Clipboard.SetText(e.ToString());
            }
            return generated;
        }

        public void setFrwJsonFull(Element element, string json)
        {
            this.eaUtils.taggedValuesUtils.set(element, this.frameworkPackage.Name + "::json-full", json, true, true);
        }

        public void setFrwJson(Element element, string json)
        {
            this.eaUtils.taggedValuesUtils.set(element, this.frameworkPackage.Name + "::json", json, true, true);
        }
        /// <summary>
        /// Busca el json guardado en el tagged value en funcion del nombre del framework
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public string getFrwJsonFromTaggedValue(Element element)
        {
            return this.eaUtils.taggedValuesUtils.get(element, this.frameworkPackage.Name +"::json", "{}").asString();
        }

        /// <summary>
        /// Busca el json del "extra" guardado en el tagged value en funcion del nombre del framework
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public string getFrwExtraJsonFromTaggedValue(Element element)
        {
            return this.eaUtils.taggedValuesUtils.get(element, this.frameworkPackage.Name +"::"+ "json-manual", "").asString();
        }

        /// <summary>
        /// Busca el json guardado en el tagged value en funcion del nombre del lenguaje
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public string getLngJsonFromTaggedValue(Element element)
        {
            return this.eaUtils.taggedValuesUtils.get(element, this.frameworkPackage.Name + "::" + this.choosed.name, "{}").asString();
        }

        /// <summary>
        /// Busca el json guardado en el tagged value en funcion del nombre del dialecto
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public string getDlcJsonFromTaggedValue(Element element)
        {
            return this.eaUtils.taggedValuesUtils.get(element, this.frameworkPackage.Name + "::" + this.choosed.name + "-" + this.choosed.choosed.name, "{}").asString();
        }

        public string getArtifactFromTaggedValue(Element element, Artifact artifact)
        {
            return this.eaUtils.taggedValuesUtils.get(element, this.frameworkPackage.Name + "::" + this.choosed.name + "-" + this.choosed.choosed.name + "-" + artifact.name, "{}").asString();
        }

        public string getFileInfoFromTaggedValue(Element element, Artifact artifact)
        {
            return this.eaUtils.taggedValuesUtils.get(element, this.frameworkPackage.Name + "::" + this.choosed.name + "-" + this.choosed.choosed.name + "-" + artifact.name +"-file_info", "{}").asString();
        }

        public TemplateInfo getTemplateFromTaggedValue(Element element, Artifact artifact)
        {
            string tvValue = this.eaUtils.taggedValuesUtils.get(element, this.frameworkPackage.Name + "::" + this.choosed.name + "-" + this.choosed.choosed.name + "-" + artifact.name + "-Template", "").asString();

            TemplateInfo templateInfo = new TemplateInfo();

            if (tvValue != "")
            {
                templateInfo.parse(tvValue);
            }
            return templateInfo;
        }

        public string buildTaggedValueNameFromArtifactPackage(Package artifactPackage)
        {
            Package dialectPackage = this.eaUtils.repository.GetPackageByID(artifactPackage.ParentID);
            Package languagePackage = this.eaUtils.repository.GetPackageByID(dialectPackage.ParentID);

            return frameworkPackage.Name + "::" + languagePackage.Name + "-" + dialectPackage.Name + "-" + artifactPackage.Name;
        }

        public string buildTaggedValueName( Package templatePackage)
        {
            Package artifactPackage  = this.eaUtils.repository.GetPackageByID( templatePackage.ParentID );
            Package dialectPackage   = this.eaUtils.repository.GetPackageByID( artifactPackage.ParentID );
            Package languagePackage  = this.eaUtils.repository.GetPackageByID( dialectPackage.ParentID  );

            return frameworkPackage.Name +"::"+ languagePackage.Name +"-"+ dialectPackage.Name +"-"+ artifactPackage.Name;
        }

        public string getDialectFilePreferencesFromTaggedValue(Element element)
        {
            return this.eaUtils.taggedValuesUtils.get(element, this.frameworkPackage.Name + "::" + this.choosed.name + "-" + this.choosed.choosed.name + "-file_info", "{}").asString();
        }

        /// <summary>
        /// Agrega 2 valores etiquetados. Si existe el valor etiquetado busca en el contenido para agregar un nuevo artefacto, si lo hubiera.
        /// 1. un json, este se compone de los "templates" (_taggedValueDefaultContent) que proveea cada artefacto.
        ///     En el json principal se agrega con el nombre de dialect_data.
        /// 2. un json con parámetros para la generación del archivo.
        /// </summary>
        /// <param name="dialectPackage"></param>
        /// <param name="table"></param>
        public void setDialectTaggedValue(Package dialectPackage, Element table)
        {
            Package languagePackage = this.eaUtils.repository.GetPackageByID(dialectPackage.ParentID);
            Package frameworkPackage = this.eaUtils.repository.GetPackageByID(this.eaUtils.repository.GetPackageByID(languagePackage.ParentID).ParentID);

            string tvName = frameworkPackage.Name + "::" + languagePackage.Name + "-" + dialectPackage.Name;

            DialectInfo dialectInfo = new DialectInfo(dialectPackage);

            dialectInfo.loadArtifacts();

            string dialectDataString = this.eaUtils.taggedValuesUtils.get(table, tvName, "").asString();

            try
            {
                if (dialectDataString != "")
                {
                    dialectInfo.parse(dialectDataString);
                }

                this.eaUtils.taggedValuesUtils.set(table, tvName, dialectInfo.stringfity(), true, true);

                Alert.Success("Los datos han sido actualizados");
            }
            catch (Exception e)
            {
                Alert.Error("Se produjo un error, vea la ventana de salida");
                this.eaUtils.printOut(e.ToString());
            }

            string fileInfoTvName = tvName + "-file_info";

            string dialectFileDataString = this.eaUtils.taggedValuesUtils.get(table, fileInfoTvName, "").asString();

            try
            {
                    if (dialectFileDataString != "")
                {
                    dialectInfo.filePreference.parse(dialectFileDataString);
                }

                this.eaUtils.taggedValuesUtils.set(table, fileInfoTvName, dialectInfo.filePreference.stringfity(), true, true);

                Alert.Success("Los datos de las preferencias de archivo han sido actualizadas");
            }
            catch (Exception e)
            {
                Alert.Error("Se produjo un error, vea la ventana de salida");
                this.eaUtils.printOut(e.ToString());
            }

        }

        /// <summary>
        /// Agrega un tagged value si no existe para poder contener la info de reemplazo de template.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="table"></param>
        public void setTemplateTaggedValue( Element template, Element table )
        {
            // buscar el paquete del artefacto, si no es el mismo paquete seguimos buscando los parents.
            
            Package templatePackage = this.eaUtils.repository.GetPackageByID(template.PackageID);

            templatePackage = this.getPackageOfArtifact(templatePackage);

            if ( templatePackage != null)
            {
                TemplateInfo templateInfo = new TemplateInfo();

                string tvName = this.buildTaggedValueNameFromArtifactPackage(templatePackage) + "-Template";

                string tvValue = this.eaUtils.taggedValuesUtils.get(table, tvName, "").asString();

                if( tvValue != "")
                {
                    templateInfo.parse(tvValue);
                }

                templateInfo.add(template);

                this.eaUtils.taggedValuesUtils.set(table, tvName, templateInfo.stringfity(), true);
            }
        }

        public void setTemplateTaggedValue(Package templatePackage, Element table)
        {
            templatePackage = this.getPackageOfArtifact(templatePackage);
            if (templatePackage != null)
            {
                TemplateInfo templateInfo = new TemplateInfo();

                string tvName = this.buildTaggedValueName(templatePackage) + "-Template";

                string tvValue = this.eaUtils.taggedValuesUtils.get(table, tvName, "").asString();

                if (tvValue != "")
                {
                    templateInfo.parse(tvValue);
                }

                templateInfo.add(templatePackage);

                this.eaUtils.taggedValuesUtils.set(table, tvName, templateInfo.stringfity(), true);
            }
        }

        internal Package getPackageOfArtifact( Package candidate)
        {
            Package ofArtifact = null;

            foreach( Element element in candidate.Elements)
            {
                if( element.Stereotype == "Artifact")
                {
                    ofArtifact = candidate;
                    break;
                }
            }

            if( ofArtifact == null)
            {
                ofArtifact = getPackageOfArtifact(this.eaUtils.repository.GetPackageByID(candidate.ParentID));
            }

            return ofArtifact;
        }

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

            foreach (EA.Attribute attr in artifactElement.Attributes)
            {
                if (attr.Name == NAMESPACE_ATTR_LIST_NAME)
                {
                    if (attr.Default != "")
                    {
                        // puede o no venir con un valor por ej: plus, apiVersion = v1, ...
                        string[] attrs = attr.Default.Split(',');
                        string[] attrNameValue;

                        foreach (string newAttr in attrs)
                        {
                            if (newAttr.Contains("="))
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
                                if (this.canAdd(newAttr.Trim(), namespaceElement))
                                {
                                    added = namespaceElement.Attributes.AddNew(newAttr.Trim(), "String");
                                    added.Update();
                                }
                            }
                        }
                    }
                    break;
                }
            }
        }

        private bool canAdd(string attrName, Element namespaceElement)
        {
            bool canAdd = true;

            foreach (EA.Attribute attr in namespaceElement.Attributes)
            {
                if (attr.Name == attrName)
                {
                    canAdd = false;
                    break;
                }
            }

            return canAdd;
        }
    }
}
