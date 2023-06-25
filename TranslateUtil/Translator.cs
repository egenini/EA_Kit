using EA;
using Google.Cloud.Translation.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TranslateUtil
{
    public class Translator
    {
        public const string OPTIONS_STEREOTYPE = "TranslateOptions";
        private TranslationClient client;
        private string toLanguage;
        private string fromLanguage = "en";
        private EAUtils.EAUtils eaUtils;
        private List<LanguageOption> languages = new List<LanguageOption>();
        private bool transaleName = true;
        private bool transaleNotes = true;
        private bool transaleAlias = true;
        public bool enable = false;
        public bool ready = false;

        private Element currentDeepElement = null;

        /// <summary>
        /// Esto implica que la traducción reemplaza al lenguaje original, de otro modo esta traducción va a valor etiquetado.
        /// </summary>
        private LanguageOption languageMain = null;
        private LanguageOption languageSwitch = null;

        public Translator(EAUtils.EAUtils eaUtils)
        {
            this.eaUtils = eaUtils;

            try
            {
                this.client = TranslationClient.Create();
            }
            catch (Exception e)
            {
                Clipboard.SetText(e.ToString());
            }
        }

        public Translator( string toLanguage )
        {
            try
            {
                this.client = TranslationClient.Create();
            }
            catch (Exception e )
            {
                Clipboard.SetText(e.ToString());
            }
            this.toLanguage = toLanguage;
        }

        public bool switchReady()
        {
            return this.languageSwitch != null;
        }

        public void buildOptions( Package package)
        {
            foreach( Element element in package.Elements)
            {
                if( element.Stereotype == OPTIONS_STEREOTYPE)
                {
                    buildOptions(element);
                    break;
                }
            }
        }

        public void buildOptions(Element optionsElement)
        {
            EA.Attribute attrName = EAUtils.AttributeUtils.get(optionsElement, "Name");

            EA.Attribute attrNotes = EAUtils.AttributeUtils.get(optionsElement, "Notes");

            if ( attrName.Default.ToLower() == "true")
            {
                this.transaleName = true;
            }
            if (attrNotes.Default.ToLower() == "true")
            {
                this.transaleNotes = true;
            }

            try
            {
                EA.Attribute attrTranslateNow = EAUtils.AttributeUtils.get(optionsElement, "TranslateNow");
                if (attrTranslateNow.Default.ToLower() == "true")
                {
                    this.enable = true;
                }
            }
            catch (Exception) { }

            try
            {
                EA.Attribute attrEnable = EAUtils.AttributeUtils.get(optionsElement, "Enabled");
                if (attrEnable.Default.ToLower() == "true")
                {
                    this.enable = true;
                }
            }
            catch (Exception) { }

            LanguageOption languajeOpcion;

            foreach( Connector connector in optionsElement.Connectors)
            {
                if(connector.ClientID == optionsElement.ElementID && connector.SupplierID == optionsElement.ElementID)
                {
                    languajeOpcion = new LanguageOption(connector);

                    if (languajeOpcion.isMain)
                    {
                        this.languageMain = languajeOpcion;

                        if( languajeOpcion.isSwitch)
                        {
                            this.languageSwitch = languajeOpcion;
                        }
                    }
                    else
                    {
                        if (languajeOpcion.isSwitch)
                        {
                            languageSwitch = languajeOpcion;
                        }

                        this.languages.Add(languajeOpcion);
                    }
                }
            }

            if (this.languages.Count == 0)
            {
                this.languageSwitch = this.languageMain;
            }
            else if ( this.languages.Count == 1 )
            {
                this.languageMain   = languages[0];
                this.languageSwitch = this.languageMain;

                languages.Clear();
            }

            ready = true;
        }

        public string translateText(string text)
        {
            if ( enable && languageMain != null)
            {
                text = this.translate(text, languageMain.toLanguage, languageMain.fromLanguage);
            }
            return text;
        }

        public void translate( Package package )
        {
            string text;
            string origText;

            // si viene el nombre del paquete traducido entonces sólo vemos si es necesario traducir las notas.
            if( enable && languageMain != null )
            {
                if( this.transaleName )
                {
                    origText = package.Name;

                    text = this.translate(origText, languageMain.toLanguage, languageMain.fromLanguage);

                    if (text != "")
                    {
                        package.Name = text;
                        package.Update();

                        // si pudo traducir algo entonces guardamos el texto original en su lenguaje.
                        this.eaUtils.taggedValuesUtils.set(package.Element, "translation_Name_" + languageMain.fromLanguage, origText, true);

                        if( package.Diagrams.Count == 1)
                        {
                            Diagram diagram = package.Diagrams.GetAt(0);

                            if( diagram.Name == origText )
                            {
                                diagram.Name = text;
                                diagram.Update();
                            }
                        }
                    }
                }

                if ( this.transaleNotes )
                {
                    origText = package.Notes;

                    text = this.translate(origText, languageMain.toLanguage, languageMain.fromLanguage);

                    if (text != "")
                    {
                        package.Notes = text;
                        package.Update();

                        // si pudo traducir algo entonces guardamos el texto original en su lenguaje.
                        this.eaUtils.taggedValuesUtils.set(package.Element, "translation_Notes_" + languageMain.fromLanguage, origText, true);
                    }
                }
            }
            if( enable)
            {
                foreach (LanguageOption languageOption in languages)
                {
                    if (this.transaleName)
                    {
                        text = this.translate(package.Name, languageOption.toLanguage, languageOption.fromLanguage);

                        if (text != "")
                        {
                            this.eaUtils.taggedValuesUtils.set(package.Element, "translation_Name_" + languageOption.toLanguage, text, true);
                        }
                    }
                    if (this.transaleNotes)
                    {
                        text = this.translate(package.Notes, languageOption.toLanguage, languageOption.fromLanguage);

                        if (text != "")
                        {
                            this.eaUtils.taggedValuesUtils.set(package.Element, "translation_Notes_" + languageOption.toLanguage, text, true);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// traduce si no está traducido. Si se quiere vovler a tradudir entonces hay que eliminar el valor etiquetado.
        /// </summary>
        /// <param name="element"></param>
        public void translate( Element element )
        {
            string text;
            string origText;
            Diagram diagram;

            if( enable && element.Stereotype != OPTIONS_STEREOTYPE)
            {
                if (this.transaleName)
                {
                    if (languageMain != null)
                    {
                        origText = element.Name;

                        text = this.eaUtils.taggedValuesUtils.get(element, "translation_Name_" + languageMain.fromLanguage, "").asString();

                        if (text == "")
                        {
                            text = this.translate(element.Name, languageMain.toLanguage, languageMain.fromLanguage);

                            if (text != "")
                            {
                                element.Name = text;
                                element.Update();

                                // si pudo traducir algo entonces guardamos el texto original en su lenguaje.
                                this.eaUtils.taggedValuesUtils.set(element, "translation_Name_" + languageMain.fromLanguage, origText, true);

                                if( element.Diagrams.Count == 1)
                                {
                                    diagram = element.Diagrams.GetAt(0);
                                    if(diagram.Name == origText)
                                    {
                                        diagram.Name = text;
                                        diagram.Update();
                                    }
                                }
                            }
                        }
                    }
                }

                if (this.transaleNotes)
                {
                    if (languageMain != null)
                    {
                        origText = element.Notes;

                        text = this.eaUtils.taggedValuesUtils.get(element, "translation_Notes_" + languageMain.fromLanguage, "").asString();

                        if (text == "")
                        {
                            text = this.translate(element.Notes, languageMain.toLanguage, languageMain.fromLanguage);

                            if (text != "")
                            {
                                element.Notes = text;
                                element.Update();

                                // si pudo traducir algo entonces guardamos el texto original en su lenguaje.
                                this.eaUtils.taggedValuesUtils.set(element, "translation_Notes_" + languageMain.fromLanguage, origText, true);
                            }
                        }
                    }
                }
                if (this.transaleAlias)
                {
                    if (languageMain != null)
                    {
                        origText = element.Notes;

                        text = this.eaUtils.taggedValuesUtils.get(element, "translation_Alias_" + languageMain.fromLanguage, "").asString();

                        if (text == "")
                        {
                            text = this.translate(element.Notes, languageMain.toLanguage, languageMain.fromLanguage);

                            if (text != "")
                            {
                                element.Alias = text;
                                element.Update();

                                // si pudo traducir algo entonces guardamos el texto original en su lenguaje.
                                this.eaUtils.taggedValuesUtils.set(element, "translation_Alias_" + languageMain.fromLanguage, origText, true);
                            }
                        }
                    }
                }
                foreach (LanguageOption languageOption in languages)
                {
                    if (this.transaleName)
                    {
                        text = this.eaUtils.taggedValuesUtils.get(element, "translation_Name_" + languageOption.toLanguage, "").asString();

                        if (text == "")
                        {
                            text = this.translate(element.Name, languageOption.toLanguage, languageOption.fromLanguage);

                            if (text != "")
                            {
                                this.eaUtils.taggedValuesUtils.set(element, "translation_Name_" + languageOption.toLanguage, text, true);
                            }
                        }
                    }
                    if (this.transaleNotes)
                    {
                        text = this.eaUtils.taggedValuesUtils.get(element, "translation_Notes_" + languageOption.toLanguage, "").asString();

                        if (text == "")
                        {
                            text = this.translate(element.Notes, languageOption.toLanguage, languageOption.fromLanguage);

                            if (text != "")
                            {
                                this.eaUtils.taggedValuesUtils.set(element, "translation_Notes_" + languageOption.toLanguage, text, true);
                            }
                        }
                    }
                    if (this.transaleAlias)
                    {
                        text = this.eaUtils.taggedValuesUtils.get(element, "translation_Alias_" + languageOption.toLanguage, "").asString();

                        if (text == "")
                        {
                            text = this.translate(element.Alias, languageOption.toLanguage, languageOption.fromLanguage);

                            if (text != "")
                            {
                                this.eaUtils.taggedValuesUtils.set(element, "translation_Alias_" + languageOption.toLanguage, text, true);
                            }
                        }
                    }
                }
            }
        }

        public string translate( string text, string toLanguage, string fromLanguage)
        {
            if (! enable || text == null || text == "")
            {
                return "";
            }
            return this.client.TranslateText(text, toLanguage, fromLanguage).TranslatedText;
        }

        public string translate( string text )
        {
            if (!enable || text == null || text == "")
            {
                return "";
            }
            return this.client.TranslateText(text, this.toLanguage, this.fromLanguage).TranslatedText;
        }

        public void translateDeep(Package package)
        {
            if (this.ready && this.enable)
            { 
                this.translate(package);

                foreach (Package child in package.Packages)
                {
                    this.translateDeep(child);
                }

                foreach (Element element in package.Elements)
                {
                    this.translateDeep(element);
                }
            }
        }

        public void translateDeep( Element element )
        {
            if( this.ready && this.enable)
            {
                if (element.Stereotype == "Pool")
                {
                    if (this.currentDeepElement != null)
                    {
                        element.Name = this.currentDeepElement.Name;
                        element.Update();
                    }
                    else
                    {
                        this.translate(element);
                    }
                    // ¿vamos a buscar los elementos que están en el pool para buscar los conectores y traducir la guarda?
                }
                else
                {
                    this.translate(element);
                }

                this.currentDeepElement = element;

                foreach (Element child in element.Elements)
                {
                    this.translateDeep(child);
                }
            }
        }
        public void translateSwitchDeep(Package package)
        {
            string text;

            if (this.transaleName)
            {
                // si el idioma a cambiar es el principal entonces este puede no estar en ve si no que está en el nombre.
                // una vez que se haga el primer cambio este pasa a ve para que cuando se quiera volver a hacer el cambio no se tenga que 
                // volver a traducir.
                
                text = this.eaUtils.taggedValuesUtils.get(package.Element, "translation_Name_" + languageSwitch.switchFromLanguage, "").asString();

                if (text != "")
                {
                    this.eaUtils.taggedValuesUtils.set(package.Element, "translation_Name_" + languageSwitch.switchToLanguage, package.Name, true);

                    if (package.Diagrams.Count == 1)
                    {
                        Diagram diagram = package.Diagrams.GetAt(0);
                        if (diagram.Name == package.Name)
                        {
                            diagram.Name = text;

                            diagram.Update();
                        }
                    }

                    package.Name = text;
                }
            }
            if (this.transaleNotes)
            {
                text = this.eaUtils.taggedValuesUtils.get(package.Element, "translation_Notes_" + languageSwitch.toLanguage, "").asString();

                if (text != "")
                {
                    this.eaUtils.taggedValuesUtils.set(package.Element, "translation_Notes_" + languageSwitch.fromLanguage, package.Notes, true);

                    package.Notes = text;
                }
            }
            if (this.transaleAlias)
            {
                text = this.eaUtils.taggedValuesUtils.get(package.Element, "translation_Alias_" + languageSwitch.toLanguage, "").asString();

                if (text != "")
                {
                    this.eaUtils.taggedValuesUtils.set(package.Element, "translation_Alias_" + languageSwitch.fromLanguage, package.Alias, true);

                    package.Alias = text;
                }
            }

            package.Update();

            foreach (Element child in package.Elements)
            {
                this.translateSwitchDeep(child);
            }

            foreach (Package child in package.Packages)
            {
                this.translateSwitchDeep(child);
            }
        }

        public void translateSwitchDeep(Element element)
        {
            string text;

            if( this.transaleName)
            {
                text = this.eaUtils.taggedValuesUtils.get(element, "translation_Name_" + languageSwitch.toLanguage, "").asString();

                if( text != "")
                {
                    this.eaUtils.taggedValuesUtils.set(element, "translation_Name_" + languageSwitch.fromLanguage, element.Name, true);

                    if( element.Diagrams.Count == 1)
                    {
                        Diagram diagram = element.Diagrams.GetAt(0);
                        if( diagram.Name == element.Name)
                        {
                            diagram.Name = text;

                            diagram.Update();
                        }
                    }

                    element.Name = text;
                }
            }
            if (this.transaleNotes)
            {
                text = this.eaUtils.taggedValuesUtils.get(element, "translation_Notes_" + languageSwitch.toLanguage, "").asString();

                if (text != "")
                {
                    this.eaUtils.taggedValuesUtils.set(element, "translation_Notes_" + languageSwitch.fromLanguage, element.Notes, true);

                    element.Notes = text;
                }
            }
            if (this.transaleAlias)
            {
                text = this.eaUtils.taggedValuesUtils.get(element, "translation_Alias_" + languageSwitch.toLanguage, "").asString();

                if (text != "")
                {
                    this.eaUtils.taggedValuesUtils.set(element, "translation_Alias_" + languageSwitch.fromLanguage, element.Alias, true);

                    element.Alias = text;
                }
            }

            element.Update();

            foreach ( Element child in element.Elements)
            {
                this.translateSwitchDeep(child);
            }
        }
    }
    
    class SwitchRunInfo
    {

    }

    public class LanguageOption
    {
        public bool   isMain   = false;
        public bool   isSwitch = false;
        public string fromLanguage;
        public string toLanguage;
        public string switchFromLanguage;
        public string switchToLanguage;

        public LanguageOption( Connector connector)
        {
            isMain             = connector.Name.ToLower() == "main";
            isSwitch           = connector.Name.ToLower() == "switch";
            toLanguage         = connector.SupplierEnd.Role;
            fromLanguage       = connector.ClientEnd.Role;
            switchFromLanguage = toLanguage;
            switchToLanguage   = fromLanguage;
        }
    }
}
