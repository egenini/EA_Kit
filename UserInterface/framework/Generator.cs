using EA;
using EAUtils;
using EAUtils.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UIResources;

namespace UserInterface.framework
{
    class Generator
    {
        Element element;
        Framework frameworkInstance;
        EAUtils.EAUtils eaUtils;
        Dictionary<int, string> idColumnaSourceGuid = new Dictionary<int, string>();
        string language;
        
        public void build(EA.Attribute attribute, Framework frameworkInstance, EAUtils.EAUtils eaUtils)
        {
            this.eaUtils = eaUtils;

            this.frameworkInstance = frameworkInstance;
            
            this.element = this.eaUtils.repository.GetElementByID(attribute.ParentID);

            // por convención tomamos como lenguaje lo que está delante del 1er "-"
            // si no se pudiera se tendría que buscar la lista de los lenguajes y hacer que se elija 1.

            if(this.frameworkInstance.languageForCodeGeneration.name.Contains("-"))
            {
                this.language = this.frameworkInstance.languageForCodeGeneration.name.Substring(0, this.frameworkInstance.languageForCodeGeneration.name.IndexOf("-"));
            }
            else
            {
                this.language = this.frameworkInstance.languageForCodeGeneration.name;
            }

            Entity entity = configure();

            entity.entity = StringUtils.toPascal(this.element.Name);

            entity.attributes.Add(buildFrom(attribute));

            if (this.frameworkInstance.generate(entity.stringfity()))
            {
                Alert.Success("Copiado al portapapeles");
            }
        }
        public void build(Element element, Framework frameworkInstance, EAUtils.EAUtils eaUtils)
        {
            this.element = element;
            this.frameworkInstance = frameworkInstance;
            this.eaUtils = eaUtils;

            this.language = this.frameworkInstance.languageForCodeGeneration.name.Substring(0, this.frameworkInstance.languageForCodeGeneration.name.IndexOf("-"));

            Entity entity = configure();
            //PojoInfo pojoInfo = configure();

            //onAceptConfigure(pojoInfo);

            if (this.frameworkInstance.generate(entity.stringfity()))
            {
                Alert.Success("Copiado al portapapeles");
            }
        }

        private Entity configure()
        {
            Entity pojoInfo = new Entity();

            pojoInfo.entity = StringUtils.toPascal(this.element.Name);

            // obtiene las columnas "nuevas" y las agrega al final.
            buildFroms(pojoInfo);

            return pojoInfo;
        }

        private string getSourceGuid(EA.Attribute column)
        {
            string guid = null;

            if (idColumnaSourceGuid.ContainsKey(column.AttributeID))
            {
                guid = idColumnaSourceGuid[column.AttributeID];
            }
            else
            {
                guid = this.eaUtils.taggedValuesUtils.get(column, "source.guid", null).asString();
                idColumnaSourceGuid[column.AttributeID] = guid;
            }

            return guid;
        }
        
        private void buildFroms(Entity pojoInfo)
        {
            
            foreach( EA.Attribute attr in this.element.Attributes)
            {
                pojoInfo.attributes.Add(buildFrom(attr));
            }
            
        }

        private FullAttribute buildFrom(EA.Attribute attr)
        {
            FullAttribute toReturn = null;
            EA.Attribute source = null;
            Element parentSource = null;

            string guid = this.getSourceGuid(attr);

            if (guid != null)
            {
                source = this.eaUtils.repository.GetAttributeByGuid(guid);

                // este control es porque se pudo haber eliminado el origen de la columna
                if (source != null && source.ParentID != 0)
                {
                    // buscamos para saber si el origen de la definción de la columna es una tabla o clase.
                    parentSource = this.eaUtils.repository.GetElementByID(source.ParentID);
                }
                else
                {
                    // la columna perdió su relación con el origen.
                    this.eaUtils.taggedValuesUtils.delete(attr, "source.guid");
                }
            }

            // determinamos si vamos a usar la columna o el "origen" (atributo de una clase u otra columna).
            if (source != null && parentSource != null && parentSource.Stereotype != "table")
            {
                toReturn = new FullAttribute(source, this.eaUtils);
            }
            else
            {
                toReturn = new FullAttribute(attr, this.eaUtils);

                // Buscamos el tipo de dato que le correponde a un atributo de clase.
                DataTypeInfo dataTypeInfo = eaUtils.dataTypeUtils.getFrom(this.element.Gentype, attr.Type, language);

                toReturn.dataType = dataTypeInfo.name;

                toReturn.name = StringUtils.toCamel(toReturn.name);
            }

            toReturn.search = this.eaUtils.taggedValuesUtils.getSearch(attr, "false").asBoolean();
            toReturn.realPk = this.eaUtils.taggedValuesUtils.getRealPrimaryKey(attr, "false").asBoolean();

            toReturn.dbDataType       = attr.Type;
            toReturn.dataTypeAsObject = toReturn.dataType;


            return toReturn;
        }
    }
}
