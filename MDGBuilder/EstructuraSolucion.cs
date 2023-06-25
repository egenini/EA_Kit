using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MDGBuilder
{
    public class EstructuraSolucion
    {
        EAUtils.EAUtils eaUtils;
        public EA.Package principal;
        public EA.Package datosReferencia;
        public EA.Package frameworks;
        public EA.Package patterns;
        public EA.Package templates;
        public EA.Package quicklinker;
        public EA.Package profileElements;
        public EA.Package profileDiagrams;
        public EA.Package profileToolboxes;
        public EA.Element mtsElement;
        public EA.Element sincronizarEstereotiposScript;

        public EstructuraSolucion(EAUtils.EAUtils eaUtils)
        {
            this.eaUtils = eaUtils; 
        }

        public EA.Package establecer()
        {
            ObjectType ot = eaUtils.repository.GetContextItemType();
            Package package = null;

            switch (ot)
            {
                case ObjectType.otPackage:

                    Package current = eaUtils.repository.GetContextObject();

                    if (current.Element.Genfile.Contains(".mts") || current.Element.Genfile.Contains("MDG_X.xml"))
                    {
                        package = current;
                    }
                    else if (current.Element.Stereotype == "profile")
                    {
                        package = eaUtils.repository.GetPackageByID(current.ParentID);
                    }
                    else
                    {
                        int parentId = current.ParentID;

                        while (parentId != 0)
                        {
                            current = eaUtils.repository.GetPackageByID(parentId);

                            if (current.Element != null && (current.Element.Genfile.Contains(".mts") || current.Element.Genfile.Contains("MDG_X.xml")))
                            {
                                package = current;
                                break;
                            }

                            parentId = current.ParentID;
                        }
                    }
                    break;
            }

            if (package != null)
            {
                this.principal = package;
                
                this.datosReferencia = this.principal.Packages.GetByName("Datos Referencia");
                this.frameworks = this.principal.Packages.GetByName("Frameworks");
                this.patterns = this.principal.Packages.GetByName("Patterns");
                this.quicklinker = this.principal.Packages.GetByName("Quicklinker");
                this.templates = this.principal.Packages.GetByName("Templates");
                this.profileElements = this.principal.Packages.GetByName("elements");
                this.profileDiagrams = this.principal.Packages.GetByName("diagrams");
                this.profileToolboxes = this.principal.Packages.GetByName("toolbox");

                foreach (Element currentElement in principal.Elements)
                {
                    if (currentElement.Type == "Class")
                    {                        
                        if (currentElement.Attributes.Count > 0)
                        {
                            mtsElement = currentElement;
                        }
                        else if(currentElement.Name == "SincronizarEstereotiposJS")
                        {
                            this.sincronizarEstereotiposScript = currentElement;
                        }
                    }
                }
                if( this.sincronizarEstereotiposScript == null)
                {
                    this.sincronizarEstereotiposScript =  this.principal.Elements.AddNew("SincronizarEstereotiposJS", "Class");

                    this.sincronizarEstereotiposScript.Update();

                    this.principal.Element.Refresh();
                }
            }
            return package;
        }
    }
}
