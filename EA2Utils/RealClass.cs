using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EAUtils.ConnectorUtils;

namespace EAUtils
{
    public class RealClass
    {
        public Element element;
        private List<AttributeInfo> attributesInfo = new List<AttributeInfo>();
        public List<RealClass> innerClasses = new List<RealClass>();
        private List<string> attributesNames = new List<string>();

        public List<AttributeInfo> getAttributesInfo()
        {
            return this.attributesInfo;
        }

        public void add(AttributeInfo info)
        {
            if( ! attributesNames.Contains(info.name) )
            {
                attributesInfo.Add(info);
                attributesNames.Add(info.name);
            }
            else
            {
                // si este es por relación me quedo con este porque aporta más información.
                // TODO : cuando es por relación y la relación no tiene "rol" no se puede reemplazar, en ese caso se duplica.
                if( info.isByRelation)
                {
                    for (int i = attributesInfo.Count - 1; i >= 0; i--)
                    {
                        if (attributesInfo[i].name == info.name)
                        {
                            attributesInfo.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }
    }

    public class AttributeInfo
    {
        public EA.Attribute attribute = null;
        /// <summary>
        /// Nombre de la clase a la cual pertenece el atributo.
        /// </summary>
        public string className = null;
        /// <summary>
        /// Nombre del atributo
        /// </summary>
        public string name = null;
        /// <summary>
        /// El tipo del atributo, si el atributo es una enumeración el tipo es del tipo del primer atributo de la enumeración.
        /// </summary>
        public string type = null;

        public string value = null;

        public bool isByRelation = false;

        public Element classifierd = null;
        public RealClass relationClass = null;
        public ICardinality cardinality = null;

        public AttributeInfo(EA.Attribute attribute, string className)
        {
            this.attribute = attribute;
            this.cardinality = new AttributeUtils.Cardinality(attribute);
            this.name = this.attribute.Name;

            this.type = this.attribute.Type;
            this.className = className;
            this.value = this.attribute.Default;
        }

        /// <summary>
        /// Para usar cuando el atributo se origina en una asociación o en una agregación.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="nombreClase"></param>
        public AttributeInfo(string name, string type, RealClass relationClass, ICardinality cardinality)
        {
            this.isByRelation = true;
            this.name = name;
            this.type = type;
            this.className = relationClass.element.Name;
            this.relationClass = relationClass;
            this.cardinality = cardinality;

        }

        public bool hasTaggedValues()
        {
            return attribute != null && attribute.TaggedValues.Count != 0;
        }
    }
}
