using EA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDGBuilder.mdg
{
    public class MDGRenamerV2
    {
        EAUtils.EAUtils eaUtils = null;
        Package mainPackage= null;
        string oldName;
        string newName;
        string oldAlias;
        string newAlias;
        public MDGRenamerV2(EAUtils.EAUtils eaUtils, Package mainPackage)
        {
            this.eaUtils = eaUtils;
            this.mainPackage = mainPackage;
        }
        public void MDGRenamer(string newName)
        {
            this.newName = newName;
            this.newAlias = newName.Replace("_", " ");
            this.oldName = this.mainPackage.Name;
            this.oldAlias= this.mainPackage.Alias;

            // name de clase en el mainPackage
            // el genfile de mainPackage
            //
            // renombrar el mts y eliminar el mdg.xml
            // 
            // parte del nombre en subpaquetes de frameworks y etiquetas genfile, antes eliminar el file
            // parte del nombre en subpaquetes de patterns y etiquetas genfile, antes eliminar el file
            // parte del nombre en subpaquetes de templates  y etiquetas genfile, antes eliminar el file
            //
            // nombre del diagrama en el paquete <<profile>> y genfile
            // 
            // nombre del diagrama en el paquete <<diagram profile>> y genfile, antes eliminar el file
            // dentro del paquete <<diagram profile>>:
            //  atributos: toolbox parte del default y alias.
            //  elementos <stereotype> parte del nombre.
            //  
            // nombre del diagrama en el paquete <<toolbox profile>> y genfile, antes eliminar el file
            // dentro del paquete <<toolbox profile>>:
            //  por cada elemento sterotype:
            //      reemplazar parte del nombre.
            //      con persistence == persistente o persistence, parte del nombre de sus atributos

            this.renameMain();
            this.renameNonProfiles(this.eaUtils.packageUtils.getChildPackageByName(this.mainPackage, "Frameworks"));
            this.renameNonProfiles(this.eaUtils.packageUtils.getChildPackageByName(this.mainPackage, "Patterns"));
            this.renameNonProfiles(this.eaUtils.packageUtils.getChildPackageByName(this.mainPackage, "Templates"));
            this.renameProfile();
            this.renameDiagramProfile();
            this.renameToolboxProfile();
        }

        private void renameMain()
        {
            this.mainPackage.Name = this.newName;
            this.mainPackage.Alias = this.newAlias;

            string oldGenfile = mainPackage.Element.Genfile;

            if (Path.GetFileName(oldGenfile) == "MDG Builder_TEMPLATE_MDG_X.xml")
            {
                mainPackage.Element.Genfile = this.newName + ".mts";
            }
            else
            {
                mainPackage.Element.Genfile = Path.GetFileName(oldGenfile).Replace(this.oldName, this.newName);
            }

            mainPackage.Update();

            if (System.IO.File.Exists(oldGenfile))
            {
                System.IO.File.Move(oldGenfile, mainPackage.Element.Genfile);
            }

            // el xml del mdg.
            if (System.IO.File.Exists(oldGenfile.Replace(".mts", "xml")))
            {
                System.IO.File.Delete(oldGenfile.Replace(".mts", "xml"));
            }

            foreach (Element element in mainPackage.Elements)
            {
                element.Name = element.Name.Replace(this.oldName, this.newName);
                element.Alias = element.Name.Replace(this.oldAlias, this.newAlias);
            }

            renameDiagrams(mainPackage);
        }
        internal void renameNonProfiles(Package package)
        {
            // etquetas genfile
            Dictionary<string, string> genFiles = this.eaUtils.taggedValuesUtils.getByPrefix(package.Element, "GenFile-", null);

            foreach(KeyValuePair<string, string> kv in genFiles)
            {
                if (System.IO.File.Exists(kv.Value))
                {
                    System.IO.File.Delete(kv.Value);
                }
                this.eaUtils.taggedValuesUtils.delete(package.Element, kv.Key);

                this.eaUtils.taggedValuesUtils.set(package.Element, kv.Key.Replace(oldName, this.newName), kv.Value.Replace(this.oldName, this.newName));
            }

            renameDiagrams(package);

        }
        internal void renameProfile()
        {
            Package package = this.eaUtils.packageUtils.getChildPackageByStereotype(mainPackage, "profile");

            renameDiagrams(package);

            if (System.IO.File.Exists(package.Element.Genfile))
            {
                System.IO.File.Delete(package.Element.Genfile);
            }

            package.Element.Genfile = package.Element.Genfile.Replace(this.oldName, this.newName);

            package.Update();
        }
        internal void renameDiagramProfile()
        {
            Package package = this.eaUtils.packageUtils.getChildPackageByStereotype(mainPackage, "diagram profile");

            renameDiagrams(package);

            if (System.IO.File.Exists(package.Element.Genfile))
            {
                System.IO.File.Delete(package.Element.Genfile);
            }

            package.Element.Genfile = package.Element.Genfile.Replace(this.oldName, this.newName);

            package.Update();

            foreach(Element element in package.Elements)
            {
                if( element.Stereotype == "stereotype")
                {
                    element.Name = element.Name.Replace(this.oldName, this.newName);
                    element.Update();
                }
                
                foreach (EA.Attribute attr in element.Attributes)
                {
                    attr.Name = attr.Name.Replace(this.oldName, this.newName).Replace(this.oldAlias, newAlias);
                    attr.Default = attr.Default.Replace(this.newName, this.oldName).Replace(this.oldAlias, newAlias);
                    attr.Update();
                }
            }
        }
        internal void renameToolboxProfile()
        {
            Package package = this.eaUtils.packageUtils.getChildPackageByStereotype(mainPackage, "toolbox profile");

            renameDiagrams(package);

            if (System.IO.File.Exists(package.Element.Genfile))
            {
                System.IO.File.Delete(package.Element.Genfile);
            }

            package.Element.Genfile = package.Element.Genfile.Replace(this.oldName, this.newName);

            package.Update();

            foreach (Element element in package.Elements)
            {
                if (element.Persistence == "Persistent" || element.Persistence == "Persistente")
                {
                    foreach(EA.Attribute attr in element.Attributes)
                    {
                        attr.Name = attr.Name.Replace(this.oldName, this.newName).Replace(this.oldAlias, newAlias);
                        attr.Default = attr.Default.Replace(this.newName, this.oldName).Replace(this.oldAlias, newAlias);
                        attr.Update();
                    }
                }
            }
        }
        internal void renameDiagrams(Package package)
        {
            foreach (Diagram diagram in package.Diagrams)
            {
                diagram.Name = diagram.Name.Replace(this.oldName, this.newName);
                diagram.Update();
            }
        }
    }
}
