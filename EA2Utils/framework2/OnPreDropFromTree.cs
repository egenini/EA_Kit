using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EAUtils.framework2
{
    public class OnPreDropFromTree
    {
        EAUtils eaUtils = new EAUtils();
        Element dropped = null;
        Element element = null;
        Package package = null;

        public OnPreDropFromTree(Element dropped, Element element, Package package )
        {
            this.dropped = dropped;
            this.element = element;
            this.package = package;
        }

        public Boolean EA_OnPreDropFromTree(EA.Repository repository, FrameworkCommons2 frameworkInstance, string acceptStereotype )
        {
            Boolean doIt = true;

            if (repository != null && dropped != null)
            {
                try
                {
                    if (dropped != null)
                    {
                        if (element != null && dropped.Stereotype == acceptStereotype )
                        {
                            // agregamos el template como un valor etiquetado
                            this.buildEAUtils(repository);
                            //frw.Framework frameworkInstance = new frw.Framework(this.eaUtils);

                            if (element.Stereotype == "Template")
                            {
                                frameworkInstance.setTemplateTaggedValue(element, dropped);
                                doIt = false;
                            }
                        }

                        if (element != null && element.Stereotype == "Artifact" && dropped.Stereotype == "Namespace")
                        {
                            // agreggamos al namespace 
                            this.buildEAUtils(repository);

                            Package artifactPackage = this.eaUtils.repository.GetPackageByID(element.PackageID);
                            Package dialectPackage = this.eaUtils.repository.GetPackageByID(artifactPackage.ParentID);
                            Package languagePackage = this.eaUtils.repository.GetPackageByID(dialectPackage.ParentID);

                            dropped.Name = StringUtils.toPascal(languagePackage.Name) + StringUtils.toPascal(dialectPackage.Name) + StringUtils.toPascal(artifactPackage.Name);

                            dropped.Alias = artifactPackage.Name;

                            dropped.Tag = element.ElementGUID;

                            this.buildEAUtils(repository);

                            //frw.Framework frameworkInstance = new frw.Framework(this.eaUtils);

                            frameworkInstance.addNamespaceAttributes(element, dropped);

                            dropped.Update();
                            doIt = false;
                        }

                        // agregamos un atributo para que se pueda poner el namespace y asociarlo.
                        if (package != null && package.Element.Stereotype == "Dialect" && dropped.Stereotype == "Namespace")
                        {
                            // agregamos el template como un valor etiquetado
                            this.buildEAUtils(repository);

                            Package languaguePackage = this.eaUtils.repository.GetPackageByID(package.ParentID);

                            string fqname = frameworkInstance.frameworkName +"::" + languaguePackage.Name + "-" + package.Name;

                            bool addattr = true;

                            foreach (EA.Attribute attr in dropped.Attributes)
                            {
                                if (attr.Name == fqname)
                                {
                                    addattr = false;
                                    break;
                                }
                            }

                            if (addattr)
                            {
                                EA.Attribute attr = dropped.Attributes.AddNew(fqname, "");

                                this.eaUtils.taggedValuesUtils.set(attr, frameworkInstance.frameworkName +"::source", package.PackageGUID);

                                attr.Update();
                            }

                            doIt = false;
                        }

                        if (package != null && dropped.Stereotype == acceptStereotype )
                        {
                            // agregamos el template como un valor etiquetado
                            this.buildEAUtils(repository);
                            //frw.Framework frameworkInstance = new frw.Framework(this.eaUtils);

                            if (package.Element.Stereotype == "Dialect")
                            {
                                frameworkInstance.setDialectTaggedValue(package, dropped);

                                doIt = false;
                            }
                            else
                            {
                                // sólo sería un template porque lo estoy filtrando antes.
                                // vamos delegar en el método la verificación de que el paquete contenga templates.
                                frameworkInstance.setTemplateTaggedValue(package, dropped);

                                doIt = false;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Clipboard.SetText(e.ToString());
                }
            }

            return doIt;
        }

        public void buildEAUtils(EA.Repository Repository)
        {
            if (this.eaUtils == null)
            {
                eaUtils = new EAUtils();
            }
            eaUtils.setRepositorio(Repository);
            eaUtils.createOutput();
        }
    }
}
