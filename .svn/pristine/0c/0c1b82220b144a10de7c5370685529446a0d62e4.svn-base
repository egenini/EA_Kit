using DMN.framework;
using EA;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DMN
{
    class VariableManager
    {
       
        private EAUtils.EAUtils eaUtils = null;

        public string language = "";
        public string dataType = "";
        public bool isMethod = false;
        public Element enumerationElement = null;

        public VariableManager(EAUtils.EAUtils eaUtils)
        {
            this.eaUtils = eaUtils;
        }

        public string getDataType( Element element, Framework framework )
        {
            // el tipo de dato depende de donde sale la variable.
            // en cualquier caso que sea una instancia "valida" puede ser:
            // * de un tipo de dato de lenguaje.
            // * de enumeracion - en el caso de ser una conclusión -
            // * de un atributo de una clase (input data)
            // * de un método
            // * de un Data

            if( element.ClassifierID != 0)
            {
                Element classifierElement = this.eaUtils.repository.GetElementByID(element.ClassifierID);

                if( classifierElement.Stereotype == "DataType")
                {
                    dataType = classifierElement.Name;

                    List<ArrayList> elementsAndConnectors = this.eaUtils.connectorUtils.getFromConnectorFilter(true, classifierElement, "Dependency", null, null, "Language");
                    if(elementsAndConnectors.Count != 0)
                    {
                        language = ((Element)elementsAndConnectors[0][EAUtils.EAUtils.ELEMENT_AND_CONNECTOR__ELEMENT_POSITION]).Name;
                    }
                }
                else if(classifierElement.Stereotype == "Data")
                {
                    dataType = this.eaUtils.taggedValuesUtils.get(element, EAUtils.TaggedValuesUtils.ES__TIPO_DATO, "").asString();
                    if( dataType == "")
                    {
                        dataType = this.eaUtils.taggedValuesUtils.get(element, EAUtils.TaggedValuesUtils.EN__TIPO_DATO, "").asString();
                    }
                }
                else if( classifierElement.Type == "Enumeration")
                {
                    if( classifierElement.Attributes.Count != 0)
                    {
                        dataType = classifierElement.Attributes.GetAt(0).Type;
                    }
                    enumerationElement = classifierElement;
                    language           = classifierElement.Gentype;
                }
                else if( classifierElement.Type == "Class")
                {
                    string guid = this.eaUtils.taggedValuesUtils.get(classifierElement, "instanceOf", "").asString();

                    if( guid != "")
                    {
                        try
                        {
                            EA.Attribute instanceOf = this.eaUtils.repository.GetAttributeByGuid(guid);
                            
                            // puede ser instancia de otra clase o de una enumeración.
                            if( instanceOf.ClassifierID != 0)
                            {
                                try
                                {
                                    Element instanceOfClassifier = this.eaUtils.repository.GetElementByID(instanceOf.ClassifierID);
                                    if( instanceOfClassifier.Type == "Enumeration")
                                    {
                                        if (instanceOfClassifier.Attributes.Count != 0)
                                        {
                                            dataType = instanceOfClassifier.Attributes.GetAt(0).Type;
                                            language = instanceOfClassifier.Gentype;
                                        }
                                        enumerationElement = instanceOfClassifier;
                                    }
                                    else
                                    {
                                        dataType = instanceOfClassifier.Name;
                                        language = instanceOfClassifier.Gentype;
                                    }
                                }
                                catch (Exception) { }
                            }
                            else
                            {
                                dataType = instanceOf.Type;
                                language = classifierElement.Gentype;
                            }
                        }
                        catch ( Exception) { }

                        if( dataType == "" )
                        {
                            try
                            {
                                Method instanceOf = this.eaUtils.repository.GetMethodByGuid(guid);

                                isMethod = true;

                                if( instanceOf.ClassifierID != "" && instanceOf.ClassifierID != "0")
                                {
                                    Element instanceOfClassifier = this.eaUtils.repository.GetElementByID(int.Parse(instanceOf.ClassifierID));

                                    if (instanceOfClassifier.Type == "Enumeration")
                                    {
                                        if (instanceOfClassifier.Attributes.Count != 0)
                                        {
                                            dataType = instanceOfClassifier.Attributes.GetAt(0).Type;
                                            language = instanceOfClassifier.Gentype;
                                        }
                                        enumerationElement = instanceOfClassifier;
                                    }
                                    else
                                    {
                                        dataType = instanceOfClassifier.Name;
                                        language = instanceOfClassifier.Gentype;
                                    }
                                }
                            }
                            catch (Exception) { }
                        }
                    }
                }
            }
            return framework != null ? framework.getDataType(language, dataType) : dataType;
        }
    }
}
