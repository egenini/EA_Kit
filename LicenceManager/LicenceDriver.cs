using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenceManager
{
    class LicenceDriver
    {
        public string userLicence;
        public string key;
        private LicenceInformation licenceInformation;
        private string type;

        public LicenceDriver(string userLicence, LicenceInformation licenceInformation)
        {
            this.userLicence        = userLicence;
            this.licenceInformation = licenceInformation;
        }
        /*
        public bool checkKey()
        {
            bool ok = false;

            foreach (KeyValuePair<string[], string> kv in this.licenceInformation.licences())
            {
                foreach (string uuidValid in kv.Key)
                {
                    if (uuidValid == this.uuid)
                    {
                        this.type = kv.Value;
                        ok = true;
                        break;
                    }
                }
                if (ok)
                {
                    break;
                }
            }

            return ok;
        }
        */

        public bool checkKey()
        {
            bool ok = false;
            try
            {
                this.key = this.userLicence.Substring(32, 48);

                if( ! string.IsNullOrWhiteSpace(this.key) )
                {
                    this.key = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(this.key));

                    if (!string.IsNullOrWhiteSpace(this.key))
                    {
                        foreach( KeyValuePair<string[], string> kv in this.licenceInformation.licences() )
                        {
                            foreach ( string uuidValid in kv.Key)
                            {
                                if( uuidValid == this.key)
                                {
                                    this.type = kv.Value;
                                    ok = true;
                                    break;
                                }
                            }
                            if( ok )
                            {
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception) {}
            return ok;
        }

        public TimeSpan getTimeRemains(DateTime creation)
        {
            return timeAsDateTime() - creation;
        }

        internal bool checkTime(DateTime creation)
        {
            DateTime date = timeAsDateTime();

            return DateTime.Compare(date, creation) >= 0;
        }

        private long time()
        {
            return this.licenceInformation.typesTimes()[this.type];
        }

        private DateTime timeAsDateTime()
        {
            DateTime start = new DateTime(time(), DateTimeKind.Utc);
            return start.ToLocalTime();
        }
    }
}
