using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EA;
using System.Collections;

namespace EAUtils
{
    public class InstanceUtil
    {
        private Repository repository;
        private EAUtils utils;

        public InstanceUtil(EAUtils utils)
        {
            this.utils = utils;
            this.repository = utils.repository;
        }

        public void makeInstance(EA.Attribute attribute, Element instanceOf)
        {
            Element attributeOwner = this.repository.GetElementByID(attribute.ParentID);

            instanceOf.ClassifierID = attributeOwner.ElementID;
            instanceOf.ClassifierName = attributeOwner.Name;
            try
            {
                instanceOf.ClassfierID = attributeOwner.ElementID;
            }
            catch (Exception) { }

            instanceOf.Name = attributeOwner.Name + "." + attribute.Name;
            instanceOf.Alias = attribute.Alias;

            instanceOf.Update();

            this.utils.taggedValuesUtils.set(instanceOf, "instanceOf", attribute.AttributeGUID);
        }

        public void makeInstance(EA.Method method, Element instanceOf)
        {
            Element methodOwner = this.repository.GetElementByID(method.ParentID);

            instanceOf.ClassifierID = methodOwner.ElementID;
            instanceOf.ClassifierName = methodOwner.Name;
            try
            {
                instanceOf.ClassfierID = methodOwner.ElementID;
            }
            catch (Exception) { }

            instanceOf.Name = methodOwner.Name + "." + method.Name;
            instanceOf.Alias = method.Alias;

            instanceOf.Update();

            this.utils.taggedValuesUtils.set(instanceOf, "instanceOf", method.MethodGUID);
        }
        /**
         * Crea una instancia si no la tiene.
         **/
        public Element createInstance(EA.Element elementoClasificador, EA.Diagram currentDiagram, EA.Element newDataParent, string toStereoType)
        {
            string[] sourceTaggedValues = new string[] { "Precisión", "Escala", "Obligatorio", "Formato", "Dominio de valores", "Tipo de dato" };

            EA.Element       newElement                 = null;
            EA.DiagramObject parentDiagramObjectElement;
            EA.Element       parentElement              = null;
            EA.Collection    parentElements             = null;
            Boolean          makeNestConnector          = false;

            //repository.SaveDiagram(currentDiagram.DiagramID);

            if (elementoClasificador.ClassifierID == 0)
            {
                /*Busca si el elemento está graficiamente (en el diagrama) dentro de otro  */
                parentDiagramObjectElement = this.utils.diagramUtils.lookingParentBySize(this.utils.diagramUtils.findInDiagramObjects(elementoClasificador, currentDiagram), currentDiagram);

                if (parentDiagramObjectElement != null)
                {
                    parentElement = repository.GetElementByID(parentDiagramObjectElement.ElementID);

                    parentElements = parentElement.Elements;
                }
                else
                {
                    /* Elemento que se le asigna como parent (viene por parámetro) */
                    if (newDataParent != null)
                    {
                        parentElements = newDataParent.Elements;
                        makeNestConnector = true;
                    }

                    /* Si no está incluido gráficamente en otro y no se le asigna un parent y si el diagrama es de una clase */
                    if (parentElements == null && currentDiagram.ParentID != 0)
                    {
                        parentElement = repository.GetElementByID(currentDiagram.ParentID);

                        if (parentElement.Type == "Class")
                        {
                            parentElements = parentElement.Elements;
                            makeNestConnector = true;
                        }
                    }
                    /* finalmente si nada de lo anterior se da, entonces el nuevo elemento se agrega al paquete donde está el diagrama*/
                    else if (parentElements == null)
                    {
                        parentElements = repository.GetPackageByID(currentDiagram.PackageID).Elements;
                    }
                }

                var newElementStereotype = elementoClasificador.Stereotype;
                if (toStereoType != null)
                {
                    newElementStereotype = toStereoType;
                }

                var newElementType = elementoClasificador.Type;
                if (newElementStereotype == "UI_TextField" && currentDiagram.MetaType != "AdvancedUserInterface::aui")
                {
                    newElementType = "GUIElement";
                    newElementStereotype = "text";
                }

                newElement = parentElements.AddNew(elementoClasificador.Name, newElementType);
                newElement.Stereotype = newElementStereotype;
                newElement.Alias = elementoClasificador.Alias;
                newElement.ClassifierID = elementoClasificador.ElementID;
                newElement.ClassifierName = elementoClasificador.Name;

                newElement.Update();
                parentElements.Refresh();

                for (var tvi = 0; tvi < sourceTaggedValues.Length; tvi++)
                {
                    try
                    {
                        this.utils.setValueToTaggedValue(newElement, sourceTaggedValues[tvi], elementoClasificador.TaggedValues.GetByName(sourceTaggedValues[tvi]).Value);
                    }
                    catch (Exception) { }
                }

                utils.connectorUtils.settingConnectorsForInstanceElement(newElement, elementoClasificador, currentDiagram);

                if (makeNestConnector && newElementStereotype != "DBColumn")
                {
                    try
                    {
                        parentElement = repository.GetElementByID(newElement.ParentID);
                        utils.connectorUtils.addConnector(newElement, parentElement, "Nesting", null, null);
                    }
                    catch ( Exception ) { }
                }

                DiagramObject newDiagramObject = utils.diagramUtils.replaceDiagramElement(currentDiagram, newElement, elementoClasificador);

                if (elementoClasificador.Stereotype == "DataContainer")
                {
                    Dictionary<string, int> dataChildPositionInfo = new Dictionary<string, int>();

                    var dataChildPositionOffset = 50;
                    
                    dataChildPositionInfo.Add("bottom", newDiagramObject.bottom );
                    dataChildPositionInfo.Add("left", newDiagramObject.left );
                    dataChildPositionInfo.Add("right", newDiagramObject.right );
                    dataChildPositionInfo.Add("top", newDiagramObject.top );

                    foreach (EA.Element dataChild in elementoClasificador.Elements)
                    {
                        dataChildPositionOffset *= 2;

                        dataChildPositionInfo["bottom"] += dataChildPositionOffset;
                        dataChildPositionInfo["left"] += dataChildPositionOffset;
                        dataChildPositionInfo["right"] += dataChildPositionOffset;
                        dataChildPositionInfo["top"] += dataChildPositionOffset;

                        this.utils.diagramUtils.addDiagramElement(currentDiagram, dataChild, dataChildPositionInfo, null);

                        createInstance(dataChild /* EA.Element */, currentDiagram /* EA.Diagram */ , newElement, newElementStereotype);
                    }
                }   
            }
           
            return newElement;
        }

        /**
         * Actualiza elemento instancia.
         **/
        public void fillNameAliasAndStereotype2InstanceElement(EA.Element instanceElement, EA.Diagram diagram)
        {
            EA.Element classifier;

            if (instanceElement.ClassifierID != 0)
            {
                classifier = repository.GetElementByID(instanceElement.ClassifierID);

                this.utils.connectorUtils.settingConnectorsForInstanceElement(instanceElement, classifier, diagram);

                if (instanceElement.Name == "")
                {
                    instanceElement.Name = classifier.Name;
                }
                if (instanceElement.Alias == "")
                {
                    instanceElement.Alias = classifier.Alias;
                }
                if (instanceElement.Stereotype == "")
                {
                    instanceElement.Stereotype = classifier.Stereotype;
                }

                instanceElement.Update();
            }
        }
    }
}
