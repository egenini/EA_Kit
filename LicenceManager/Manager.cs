using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LicenceManager
{
    public class Manager
    {
        private LicenceInformation licenceInformation;
        private FormLicences formLicences;
        private LicenceDriver driver;

        private string path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        private string fileName;

        private string addinName;

        protected static string ITEM_MENU__REGISTRAR = Properties.Resources.ITEM_MENU__REGISTRAR;

        public Manager(string addinName, LicenceInformation licenceInformation)
        {
            this.addinName = addinName;
            this.licenceInformation = licenceInformation;
            this.fileName = Path.Combine(path, addinName + ".info");
        }

        public bool isLicensed()
        {
            bool licenceFound = false;

            try
            {
                string uuid = File.ReadAllText(this.fileName);

                DateTime creation = File.GetCreationTime(this.fileName);

                this.driver = new LicenceDriver(uuid, this.licenceInformation);

                if( driver.checkKey())
                {
                    licenceFound = driver.checkTime(creation);

                    this.licenceInformation.timeRemaining(driver.getTimeRemains(creation));

                }
            }
            catch(Exception){}

            return licenceFound;
        }

        public object menu(string menuName, string menuHeader)
        {
            if( menuName == String.Empty )
            {
                return menuHeader;
            }
            else if(menuName == menuHeader )
            {
                return new string[] { ITEM_MENU__REGISTRAR };
            }
            return "";
        }

        public void toEnter()
        {
            // desde el form se llama a onEnter
            formLicences = new FormLicences(this);

            formLicences.Show();

        }

        public void onEnter(string text)
        {
            driver = new LicenceDriver( text, this.licenceInformation );

            if( driver.checkKey() )
            {
                if( ! this.isOldKey(  ) )
                {
                    if( ! save())
                    {
                        // mensaje error
                    }
                }
            }
            else
            {
                // mensaje error
            }
        }

        private bool isOldKey()
        {
            bool isOld = false;

            if( File.Exists(this.fileName))
            {
                try
                {
                    isOld = (System.IO.File.ReadAllText(this.fileName) == driver.userLicence) ;
                }
                catch (Exception)
                {
                }
            }
            return isOld;
        }

        private bool save()
        {
            bool ok = true;
            try
            {
                System.IO.File.WriteAllText(this.fileName, driver.userLicence);
            }
            catch (Exception)
            {
                ok = false;
            }

            return ok;
        }

        public string[] addLicencedMenu(string[] ar)
        {
            // agregamos las opciones de menú de gestión de la licencia.

            return ar;
        }
        public bool manageMenuClick(string menuName, string itemName)
        {
            bool manage = false;
            if( itemName == ITEM_MENU__REGISTRAR)
            {
                toEnter();
                manage = true;
            }
            return manage;
        }

    }
}
