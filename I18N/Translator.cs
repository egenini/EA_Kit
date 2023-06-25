using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EA;
using EAUtils;
using System.Windows.Forms;
using UIResources;
using Google.Cloud.Translation.V2;

namespace I18N
{
    public class Translator
    {
        public const string OPTIONS_STEREOTYPE = "I18N";
        private const string TV_PREFiX = OPTIONS_STEREOTYPE + "::";
        private const string ALIAS = "Alias";
        private const string NAME = "Name";
        private const string NOTES = "Notes";

        private TranslationClient client;

        private EAUtils.EAUtils eaUtils;
        private CheckListUtil check = null;
        public bool enable = false;
        public bool transaleName = false;
        public bool transaleAlias = false;
        public bool transaleNotes = false;
        public bool translateRecursive = false;
        public bool renameDiagram = false;
        public bool notesAsHTML = false;

        private List<LanguageOption> languages = new List<LanguageOption>();

        public bool ready = false;

        public Translator(EAUtils.EAUtils eaUtils)
        {
            this.eaUtils = eaUtils;

            try
            {
                this.client = TranslationClient.Create();
            }
            catch (Exception e)
            {
                Alert.Error(Properties.Resources.MSG__ERROR_PORTAPAPELES);
                Clipboard.SetText(e.ToString());
            }
        }

        internal void buildOptions(Element translateOptionsElement)
        {
            this.languages.Clear();

            check = new CheckListUtil(translateOptionsElement, this.eaUtils);

            check.parse();

            this.enable             = check.exist("enabled") ? check.isChecked("enabled") : false;
            this.transaleName       = check.exist("name") ? check.isChecked("name") : false;
            this.transaleAlias      = check.exist("alias") ? check.isChecked("alias") : false;
            this.transaleNotes      = check.exist("notes") ? check.isChecked("notes") : false;
            this.notesAsHTML        = check.exist("notes as html") ? check.isChecked("notes as html") : false;
            this.translateRecursive = check.exist("recursive") ? check.isChecked("recursive") : false;
            this.renameDiagram      = check.exist("diagram name as parent name") ? check.isChecked("diagram name as parent name") : false;

            this.eaUtils.printOut("Enabled                     = "+ this.enable);
            this.eaUtils.printOut("Transale Alias              = "+ this.transaleAlias);
            this.eaUtils.printOut("Transale Name               = "+ this.transaleName);
            this.eaUtils.printOut("Transale Notes              = "+ this.transaleNotes);
            this.eaUtils.printOut("Notes as HTML               = "+ this.notesAsHTML);
            this.eaUtils.printOut("Recursive                   = "+ this.translateRecursive);
            this.eaUtils.printOut("Diagram name as parent name = "+ this.renameDiagram);

            LanguageOption languajeOpcion;

            foreach (Connector connector in translateOptionsElement.Connectors)
            {
                if (connector.ClientID == translateOptionsElement.ElementID && connector.SupplierID == translateOptionsElement.ElementID)
                {
                    languajeOpcion = new LanguageOption(connector, client);

                    if( languajeOpcion.fromLanguage != languajeOpcion.toLanguage)
                    {
                        this.languages.Add(languajeOpcion);
                    }
                }
            }
            ready = true;
        }

        private string getTrasnlated(Element element, String attrName, LanguageOption language)
        {
            string key = TV_PREFiX + attrName + "-" + language.toLanguage;

            string translatedText = "";
            string originalText = "";

            string translated = this.eaUtils.taggedValuesUtils.get(element, key, "").asString();

            if (translated == "")
            {
                if (attrName == ALIAS)
                {
                    originalText = element.Alias;
                    translatedText = language.translate(element.Alias).getTranslatedText();
                }
                else if (attrName == NAME)
                {
                    originalText = element.Name;
                    translatedText = language.translate(element.Name).getTranslatedText();
                }
                else if (attrName == NOTES)
                {
                    if (notesAsHTML)
                    {
                        translatedText = language.translateHtml(this.eaUtils.notes2Html(element)).getTranslatedText();
                    }
                    else
                    {
                        translatedText = language.translate(this.eaUtils.notes2Txt(element)).getTranslatedText();
                    }
                }

                if (translatedText != "")
                {
                    this.eaUtils.taggedValuesUtils.set(element, key, translatedText, true);

                    string keyFrom = TV_PREFiX + attrName + "-" + language.fromLanguage;

                    if (language.fromLanguage != null && this.eaUtils.taggedValuesUtils.get(element, keyFrom, "").asString() == "")
                    {
                        this.eaUtils.taggedValuesUtils.set(element, keyFrom, originalText, true);
                    }
                }
            }
            else
            {
                translatedText = translated;
            }
            return translatedText;
        }

        public void translate(Element element )
        {
            if( enable)
            {
                foreach (LanguageOption language in this.languages)
                {
                    this.eaUtils.printOut(language.toLanguage);

                    translate(element, language);
                }
            }
        }

        public void translate(Element element, LanguageOption language)
        {
            string translated = "";
            bool needUpdate = false;
            Diagram diagram;

            this.eaUtils.printOut("Element "+ element.Name);

            if (this.transaleAlias)
            {
                translated = getTrasnlated(element, ALIAS, language);

                if (language.isSwitch && translated != "")
                {
                    element.Alias = translated;
                    needUpdate = true;
                }
            }
            if (this.transaleName)
            {
                translated = getTrasnlated(element, NAME, language);

                if (language.isSwitch && translated != "")
                {
                    element.Name = translated;
                    needUpdate = true;
                }
            }
            if (this.transaleNotes)
            {
                translated = getTrasnlated(element, NOTES, language);

                if (language.isSwitch && translated != "")
                {
                    element.Notes = translated;
                    needUpdate = true;
                }
            }
            if (needUpdate)
            {
                element.Update();
            }

            if (this.renameDiagram && element.Diagrams.Count != 0)
            {
                diagram = element.Diagrams.GetAt(0);
                diagram.Name = element.Name;
                diagram.Update();
            }

            if (this.translateRecursive)
            {
                foreach (Element child in element.Elements)
                {
                    this.translate(child, language);
                }
            }
        }

        public void translate(Package package)
        {
            if (this.enable)
            {
                foreach (LanguageOption language in this.languages)
                {
                    this.eaUtils.printOut(language.toLanguage);

                    this.translate(package, language);
                }
            }
        }
        public void translate(Package package, LanguageOption language)
        {
            string translated;
            bool needUpdate = false;
            Diagram diagram;

            this.eaUtils.printOut("Package "+ package.Name);

            if ( this.transaleAlias )
            {
                translated = getTrasnlated(package.Element, ALIAS, language);

                if( language.isSwitch && translated != "" )
                {
                    package.Alias = translated;
                    needUpdate = true;
                }
            }
            if (this.transaleName )
            {
                translated = getTrasnlated(package.Element, NAME, language);

                if (language.isSwitch && translated != "" )
                {
                    package.Name = translated;
                    needUpdate = true;
                }
            }
            if (this.transaleNotes)
            {
                translated = getTrasnlated(package.Element, NOTES, language);

                if (language.isSwitch && translated != "")
                {
                    package.Notes = translated;
                    needUpdate = true;
                }
            }
            if( needUpdate)
            {
                package.Update();
            }

            if( this.renameDiagram && package.Diagrams.Count != 0 )
            {
                diagram = package.Diagrams.GetAt(0);
                diagram.Name = package.Name;
                diagram.Update();
            }

            if( this.translateRecursive)
            {
                foreach( Element element in package.Elements )
                {
                    this.translate( element, language );
                }

                foreach( Package child in package.Packages)
                {
                    this.translate(child, language);
                }
            }
        }
    }

    public class LanguageOption
    {
        private TranslationClient client;

        public bool               isSwitch = false;
        public string             fromLanguage;
        public string             toLanguage;
        private TranslationResult result;

        public LanguageOption(Connector connector, TranslationClient client)
        {
            this.client        = client;
            isSwitch           = connector.Name.ToLower() == "switch";
            toLanguage         = connector.SupplierEnd.Role;
            fromLanguage       = connector.ClientEnd.Role.Trim() == "" ? null : connector.ClientEnd.Role.Trim();
        }

        public LanguageOption translate( string text )
        {
            if( text != null && text != "")
            {
                result = client.TranslateText(text, this.toLanguage, this.fromLanguage);

                if( this.fromLanguage == null)
                {
                    if (result.DetectedSourceLanguage == this.toLanguage)
                    {
                        throw new TranslatorSameLanguageException(text);
                    }

                    this.fromLanguage = result.DetectedSourceLanguage;
                }
            }
            else
            {
                result = null;
            }

            return this;
        }
        public string getTranslatedText()
        {
            return result == null ? "" : result.TranslatedText;
        }

        public LanguageOption translateHtml(string text)
        {
            if (text != null && text != "")
            {
                result = client.TranslateHtml(text, this.toLanguage, this.fromLanguage);

                if (this.fromLanguage == null)
                {
                    if (result.DetectedSourceLanguage == this.toLanguage)
                    {
                        throw new TranslatorSameLanguageException(text);
                    }

                    this.fromLanguage = result.DetectedSourceLanguage;
                }
            }
            else
            {
                result = null;
            }

            return this;
        }
    }

    class TranslatorSameLanguageException : Exception
    {
        private string message;
        public override string Message
        {
            get { return this.message; }
        }

        public TranslatorSameLanguageException(string languageTo, string languageFrom)
        {
            this.message = String.Format(Properties.Resources.ERROR_IDIOMA_ORIGEN_DESTINO_IGUALES_CONFIGURACION, languageFrom, languageTo);
        }

        public TranslatorSameLanguageException(string palabra )
        {
            this.message = String.Format(Properties.Resources.ERROR_IDIOMA_ORIGEN_DESTINO_IGUALES_DETECTADO, palabra);
        }
    }
}
