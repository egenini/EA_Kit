using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EAUtils
{
    public class PackageUtils
    {
        public Repository repository;

        public PackageUtils(Repository repository )
        {
            this.repository = repository;
        } 

        public Package add( string name, Package parent )
        {
            Package newPackage = parent.Packages.AddNew(name, "");
            newPackage.Update();
            return newPackage;
        }

        public Package add(string name, Package parent, string stereotype)
        {
            Package newPackage = parent.Packages.AddNew(name, "");
            newPackage.Update();
            if ( stereotype != null && stereotype != "")
            {
                newPackage.Element.Stereotype = stereotype;
                newPackage.Update();
            }
            
            return newPackage;
        }

        public Package getChildPackageByName( Package parentPackage, string packageName )
        {
            Package childPackage = null;

            foreach( Package package in parentPackage.Packages )
            {
                if( package.Name == packageName )
                {
                    childPackage = package;
                    break;
                }
            }
            if( childPackage == null)
            {
                foreach (Package package in parentPackage.Packages)
                {
                    childPackage = this.getChildPackageByName(package, packageName);

                    if (childPackage != null)
                    {
                        break;
                    }
                }
            }
            return childPackage;
        }

        public Package getChildPackageByStereotype(Package parentPackage, string stereotype)
        {
            Package childPackage = null;

            foreach (Package package in parentPackage.Packages)
            {
                if (package.StereotypeEx == stereotype)
                {
                    childPackage = package;
                    break;
                }
            }
            if (childPackage == null)
            {
                foreach (Package package in parentPackage.Packages)
                {
                    childPackage = this.getChildPackageByStereotype(package, stereotype);

                    if (childPackage != null)
                    {
                        break;
                    }
                }
            }
            return childPackage;
        }

        public List<Object> getChildPackagesByStereotype(Package parentPackage, string steretype)
        {
            List<Object> childPackages = new List<Object>();

            foreach (Package package in parentPackage.Packages)
            {
                if (package.StereotypeEx == steretype)
                {
                    childPackages.Add(package);
                }
            }
            return childPackages;
        }
        public Element getChildElementByStereotype(Package parentPackage, string elementStereotype)
        {
            Element childElement = null;

            foreach ( Element element in parentPackage.Elements )
            {
                if (element.Stereotype == elementStereotype)
                {
                    childElement = element;
                    break;
                }
            }
            return childElement;
        }

        public Element getChildElementByType(Package parentPackage, string type)
        {
            Element childElement = null;

            foreach (Element element in parentPackage.Elements)
            {
                if (element.Type == type)
                {
                    childElement = element;
                    break;
                }
            }
            return childElement;
            public List<Object> getChildrenElementByStereotype(Package parentPackage, string elementStereotype)
        {
            List<Object> childrenElement = new List<Object>();

            foreach (Element element in parentPackage.Elements)
            {
                if (element.Stereotype == elementStereotype)
                {
                    childrenElement.Add(element);
                    break;
                }
            }
            return childrenElement;
        }
        public Element getChildElementByName(Package parentPackage, string name)
        {
            Element childElement = null;

            foreach (Element element in parentPackage.Elements)
            {
                if (element.Name == name)
                {
                    childElement = element;
                    break;
                }
            }
            return childElement;
        }
        public Element getChildElementByStereotype(Package parentPackage, string elementStereotype, bool recursive)
        {
            Element childElement = null;

            while (true)
            {
                childElement = this.getChildElementByStereotype(parentPackage, elementStereotype);

                if(childElement != null)
                {
                    break;
                }
                if(parentPackage.Packages.Count == 0)
                {
                    break;
                }

                foreach( Package child in parentPackage.Packages)
                {
                    childElement = this.getChildElementByStereotype(child, elementStereotype);
                    if( childElement != null)
                    {
                        break;
                    }
                }
                if (childElement != null)
                {
                    break;
                }
            }
            return childElement;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="package"></param>
        /// <param name="type">Tipo de elemento a buscar</param>
        /// <param name="stereotype">Estereotipo del elemento a buscar</param>
        /// <param name="name">Nombre del elemento a buscar</param>
        /// <param name="onlyOne">¿Buscamos sólo 1?</param>
        /// <returns></returns>
        public List<Object> getElementFromFilter( Package package, string type, string stereotype, string name, bool onlyOne )
        {
            List<Object> elements = new List<Object>();

            foreach (Element current in package.Elements)
            {
                if( ( type == null || ( type != null && type == current.Type )) 
                    && ( stereotype == null || ( stereotype != null && current.Stereotype == stereotype ) )
                    && ( name == null || ( name != null && current.Name == name)) )
                {
                    elements.Add(current);
                    if (onlyOne)
                    {
                        break;
                    }
                } 
            }
            return elements;
        }
    }
}
