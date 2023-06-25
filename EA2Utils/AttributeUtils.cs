using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EAUtils
{
    public class AttributeUtils
    {
        Repository repository;

        public AttributeUtils(Repository repository)
        {
            this.repository = repository;
        }

        public static string getValue(Element element, string name, string defaultValue)
        {
            EA.Attribute attribute = get( element, name );

            return attribute == null ? defaultValue : attribute.Default;
        }

        public static EA.Attribute get(Element element, string name)
        {
            EA.Attribute attribute = null;

            foreach (EA.Attribute current in element.Attributes)
            {
                if ( name != null && current.Name == name)
                {
                    attribute = current;
                    break;

                }
            }
            return attribute;
        }

        public static EA.Attribute synchronize(Element  element, string name, string type, string value )
        {
            EA.Attribute attribute = null;
            bool mustSave = false;
            bool mustRefresh = false;

            foreach( EA.Attribute current in element.Attributes)
            {
                if ( current.Name == name )
                {
                    attribute = current;
                    break;
                }
            }

            if( attribute == null )
            {
                attribute = element.Attributes.AddNew(name, type == null ? "": type);
                mustRefresh = true;
                mustSave = true;
            }

            if( type != null && type != attribute.Type)
            {
                attribute.Type = type;
                mustSave = true;
            }

            if( value != attribute.Default)
            {
                attribute.Default = value;
                mustSave = true;
            }

            if( mustSave)
            {
                attribute.Update();

                if( mustRefresh)
                {
                    element.Attributes.Refresh();
                }
            }

            return attribute;
        }

        public class Cardinality : ICardinality
        {
            EA.Attribute attribute;
            public Cardinality(EA.Attribute attribute)
            {
                this.attribute = attribute;
            }
            public bool isCollection()
            {
                int l;
                return (this.attribute.LowerBound.Contains("*") || this.attribute.UpperBound.Contains("*") || (int.TryParse(this.attribute.LowerBound, out l) ? l > 1 : false) || (int.TryParse(this.attribute.UpperBound, out l) ? l > 1 : false));

            }
        }
    }

    
}
