using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Productividad.modelo
{
    public class EntityDefinition : Definition
    {
        public EA.Element element;
        public Dictionary<string, AttribueReference> attributesByReference = new Dictionary<string, AttribueReference>();

        public EntityDefinition( EA.Element element )
        {
            this.element = element;
        }

        public void addAtributeByReference( string name, EA.Element element, Boolean? isRequired, Boolean isArray )
        {
            if( ! attributesByReference.ContainsKey(name) )
            {
                attributesByReference.Add(name, new AttribueReference(name, element, isRequired, isArray));
            }
        }

        public class AttribueReference
        {
            public string name;
            public Boolean? isRequired = false;
            public Boolean isArray = false;
            public EA.Element element;

            public AttribueReference(string name, EA.Element element, Boolean? isRequired, Boolean isArray)
            {
                this.name = name;
                this.element = element;
                this.isRequired = isRequired;
                this.isArray = isArray;
            }
        }
    }
}
