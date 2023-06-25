using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDGBuilder
{
    public class MainUtils
    {
        public Package getMDGPackage(Repository repository)
        {
            ObjectType ot = repository.GetContextItemType();
            Package package = null;

            switch (ot)
            {
                case ObjectType.otPackage:

                    Package current = repository.GetContextObject();

                    if( current.Element.Genfile.Contains(".mts") || current.Element.Genfile.Contains("MDG_X.xml"))
                    {
                        package = current;
                    }
                    else if ( current.Element.Stereotype == "profile")
                    {
                        package = repository.GetPackageByID(current.ParentID);
                    }
                    else
                    {
                        int parentId = current.ParentID;

                        while( parentId != 0)
                        {
                            current = repository.GetPackageByID(parentId);

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
            return package;
        }

        public Package getProfilePackage(Repository repository)
        {
            ObjectType ot = repository.GetContextItemType();
            Package package = null;

            switch (ot)
            {
                case ObjectType.otPackage:

                    Package current = repository.GetContextObject();
                    if (current.Element.Stereotype == "profile")
                    {
                        foreach( Element lookingForStereotypes in current.Elements)
                        {
                            if( lookingForStereotypes.Stereotype == "Stereotype")
                            {
                                package = current;
                                break;
                            }
                        }
                    }
                    break;
            }
            return package;
        }

        public Element getProfileElement(Repository repository)
        {
            ObjectType ot = repository.GetContextItemType();
            Element element = null;

            switch (ot)
            {
                case ObjectType.otElement:

                    Element current = repository.GetContextObject();
                    if (element.Stereotype == "stereotype")
                    {
                        element = current;
                    }
                    break;
            }
            return element;
        }

        public Package getQuicklinkerPackage(Repository repository)
        {
            ObjectType ot = repository.GetContextItemType();
            Package package = null;

            switch (ot)
            {
                case ObjectType.otPackage:

                    Package current = repository.GetContextObject();
                    if (current.Name.ToUpper() == "QUICKLINKER")
                    {
                        package = current;
                    }
                    break;
            }
            return package;
        }

    }
}
