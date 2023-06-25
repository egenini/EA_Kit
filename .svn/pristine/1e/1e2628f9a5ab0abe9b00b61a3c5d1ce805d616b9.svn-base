using EA;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace EAUtils
{

    public class RepositoryConfiguration
    {

        EAUtils eaUtils = null;
        public const string repositoryGUID = "{ED46CF0D-3310-48e5-A036-EF7D6DCA758B}";
        private string language = null;

        public RepositoryConfiguration( EAUtils eaUtils )
        {
            this.eaUtils = eaUtils;
            this.fillLanguage();
        }

        public CultureInfo repositoryCulltureInfo()
        {
            this.fillLanguage();
            return CultureInfo.CreateSpecificCulture(this.language);
        }

        public string getLanguage()
        {
            this.fillLanguage();
            return this.language;
        }

        private void readConfig()
        {
            Package repositoryPackage = eaUtils.repository.GetPackageByGuid(repositoryGUID);
            if (repositoryPackage != null)
            {
                foreach (Element element in repositoryPackage.Elements)
                {
                    if (element.Name == "Config")
                    {
                        foreach (EA.Attribute attr in element.Attributes)
                        {
                            if (attr.Name == "idioma" || attr.Name == "language")
                            {
                                language = attr.Default;
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            else
            {
                language = "es";
            }
        }

        public void fillLanguage()
        {
            if (language == null)
            {
                readConfig();
            }
        }
    }

}
