using EA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDGBuilder.mdg
{
    /// <summary>
    /// Renombra los paquetes y diagramas reemplazando el nombre del mdg anterior por el nuevo, ojo que no pisa!, si el nombre contiene reemplaza, si no lo deja tal cual.
    /// También reemplaza en el nombre de los archivos que se generan y en el mts.
    /// </summary>
    public class MDGRenamer
    {
        public string newMdgName = "new name";
        public string newMdgNameU = "new_name";
        public string oldMdgName;
        public string oldmdgNameU;
        public Element mtsElement;
        public Package mdgPackage;

        public Element getMtsElement( Package mdgPackage )
        {
            if (mdgPackage.Elements.Count != 0)
            {
                foreach (Element current in mdgPackage.Elements)
                {
                    if (current.Type == "Class")
                    {
                        newMdgName = current.Name;
                        newMdgNameU = newMdgName.Replace(" ", "_");

                        this.mtsElement = current;
                        break;
                    }
                }
            }
            return mtsElement;
        }
        internal void doIt( Package mdgPackage )
        {
            this.mdgPackage = mdgPackage;

            rename(mdgPackage);
        }

        internal void rename( Package package)
        {
            package.Name = package.Name.Replace(oldMdgName, newMdgName).Replace(oldmdgNameU, newMdgNameU);

            if( package.Element.Genfile != "")
            {
                string mdgOldGenfile = package.Element.Genfile;
                string nameFile = "";

                if (Path.GetFileName(package.Element.Genfile) == "MDG Builder_TEMPLATE_MDG_X.xml")
                {
                    nameFile = newMdgName + ".mts";
                }
                else
                {
                    nameFile = Path.GetFileName(package.Element.Genfile).Replace(oldMdgName, newMdgName).Replace(oldmdgNameU, newMdgNameU);
                }

                string basePath         = Path.GetDirectoryName(mdgOldGenfile);
                package.Element.Genfile = Path.Combine(basePath, nameFile);

                try
                {
                    if( System.IO.File.Exists(mdgOldGenfile))
                    {
                        System.IO.File.Move(mdgOldGenfile, package.Element.Genfile);
                    }
                }
                catch (Exception) { }
            }

            package.Update();

            if (package.Element.Stereotype == "diagram profile")
            {
                // hay que renombrar los datos de los diagramas
                foreach (Element element in package.Elements)
                {
                    if( element.Stereotype == "metaclass")
                    {
                        foreach( EA.Attribute attr in element.Attributes)
                        {
                            if( attr.Name == "alias" || attr.Name == "diagramID" || attr.Name == "toolbox")
                            {
                                attr.Default = attr.Default.Replace(oldMdgName, newMdgName).Replace(oldmdgNameU, newMdgNameU);
                                attr.Update();
                            }
                        }
                    }
                }
            }

            foreach ( Diagram diagram in package.Diagrams)
            {
                diagram.Name = diagram.Name.Replace(oldMdgName, newMdgName).Replace(oldmdgNameU, newMdgNameU);
                diagram.Update();
            }

            foreach (Package child in package.Packages)
            {
                rename(child);
            }
        }
    }
}
