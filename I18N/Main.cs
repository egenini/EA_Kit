using EA;
using EAUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UIResources;

namespace I18N
{
    public class Main : MainUtils
    {
        Translator translator = null;

        void buildTranslator()
        {
            if (this.translator == null)
            {
                translator = new Translator(this.eaUtils);
            }
        }

        public void EA_MenuClick(Repository repository, string location, string menuName, string itemName)
        {
            if(this.licenceManager.manageMenuClick(menuName, itemName))
            {
                // nada acá si corresponde lo hace adentro.
            }
            else if (itemName == ITEM_MENU__ACERCA_DE)
            {
                AboutBox ab = new AboutBox( addinInfo.name, "", addinInfo.version, addinInfo.licence.timeRemaining());
                ab.Show();
            }
            else if (itemName == ITEM_MENU__TRADUCIR)
            {
                try
                {
                    buildTranslator();
                    
                    Element translateOptionsElement = null;

                    int elementid = repository.InvokeConstructPicker("IncludedTypes=Artifact;StereoType=I18N");
                    if( elementid != 0)
                    {
                        try
                        {
                            translateOptionsElement = repository.GetElementByID(elementid);
                            this.translator.buildOptions(translateOptionsElement);
                        }
                        catch (Exception)
                        {
                            this.translator.enable = false;
                        }
                    }
                    else
                    {
                        this.translator.enable = false;
                    }

                    if ( this.translator.enable)
                    {
                        try
                        {
                            Element element = getElementSelected(repository);
                            
                            if (element != null)
                            {
                                Elaptime elaptime = new Elaptime();

                                this.translator.translate(element);

                                this.eaUtils.printOut(elaptime.stop());
                            }

                            Package package = getPackageSelected(repository);

                            if (package != null)
                            {
                                Elaptime elaptime = new Elaptime();

                                this.translator.translate(package);

                                this.eaUtils.printOut(elaptime.stop());
                            }

                            Alert.Success(Properties.Resources.FIN);
                        }
                        catch (TranslatorSameLanguageException e)
                        {
                            this.eaUtils.printOut(e.Message);
                            Alert.Error(Properties.Resources.ERROR_EN_VENTANA_SALIDA);
                        }
                    }
                }
                catch (Exception e)
                {
                    Clipboard.SetText(e.ToString());
                    Alert.Error(Properties.Resources.MSG__ERROR_PORTAPAPELES);
                }
            }

            
        }

        void translate(Element element, Repository repository)
        {
            this.translator.translate(element);
        }

        void import(Package package, Repository repository)
        {
            this.buildEAUtils(repository);

            try
            {

            }
            catch (Exception e)
            {
                Clipboard.SetText(e.ToString());
            }
        }

    }
}
