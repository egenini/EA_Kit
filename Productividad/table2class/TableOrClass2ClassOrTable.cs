using EA;
using EAUtils;
using Productividad.classtable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EAUtils.ConnectorUtils;

namespace Productividad.table2class
{
    public class TableOrClass2ClassOrTable
    {
        protected EAUtils.EAUtils eaUtils;
        protected Element targetElement;
        protected ColumnOrAttribute2AttributeOrColumn attributeOrColumn2ColumnOrAttribute;
        protected Framework framework;
        bool isTargetTable = false;

        public TableOrClass2ClassOrTable(Repository repository, Element targetElement)
        {
            this.targetElement = targetElement;
            this.eaUtils = new EAUtils.EAUtils();

            this.isTargetTable = targetElement.Stereotype == "table";

            this.eaUtils.setRepositorio(repository);

            this.attributeOrColumn2ColumnOrAttribute = new ColumnOrAttribute2AttributeOrColumn(repository, targetElement);
        }

        public void toTableOrClass(Element sourceElement)
        {
            eaUtils.repository.BatchAppend = true;
            bool addColumnPrefix = false;

            this.framework = new Framework(eaUtils);

            if (this.isTargetTable)
            {
                /*
                 * caso 1: la tabla realiza a la clase. No hay aún una clase realizada.
                 * Caso 2: la tabla crea columnas desde una clase que está asociada a la que es realizada y las columnas nuevas se agregan
                 *          con un prefijo que surge del nombre de la clase o del rol de la asociación.
                 */
                this.eaUtils.printOut("Analizando si se pregunta por el agregado de prefijo a la/s columna/s");
                addColumnPrefix = addColumnsWithPrefix(sourceElement);

               if (addColumnPrefix)
                {
                    // preguntamos si la queremos agregar con prefijo.
                }

                this.targetElement.Name  = this.buildTableName(  this.targetElement.Name, sourceElement.Name );
                this.targetElement.Alias = StringUtils.toPascal( this.targetElement.Name );

                eaUtils.connectorUtils.addConnectorRealization(this.targetElement, sourceElement, null, null);

                // todo: buscar como era eso de normalizar las notas de un elemento.
            }
            else
            {
                if(this.targetElement.Name.ToLower().Contains("class"))
                {
                    this.targetElement.Name = StringUtils.toPascal( sourceElement.Name );
                }
            }

            if( sourceElement.Alias != "")
            {
                this.targetElement.Alias = sourceElement.Alias;
            }
            if ( sourceElement.Notes != "")
            {
                this.targetElement.Notes = sourceElement.Notes;
            }

            string pluralAlias = this.eaUtils.taggedValuesUtils.getPluralAlias(this.targetElement, "").asString();

            if ( pluralAlias == "" )
            {
                pluralAlias = this.eaUtils.taggedValuesUtils.getPluralAlias( sourceElement, "" ).asString();

                if (pluralAlias == "")
                {
                    pluralAlias = (sourceElement.Alias != "" ? sourceElement.Alias + "s" : sourceElement.Name + "s");

                    this.eaUtils.taggedValuesUtils.setPluralAlias(sourceElement, pluralAlias );
                    this.eaUtils.taggedValuesUtils.setPluralAlias(this.targetElement, pluralAlias);
                }
                else
                {
                    this.eaUtils.taggedValuesUtils.setPluralAlias(this.targetElement, this.framework.toCase(pluralAlias));
                }
            }

            string pluralName = this.eaUtils.taggedValuesUtils.getPluralName(this.targetElement, "").asString();

            if (pluralName == "")
            {
                pluralName = this.eaUtils.taggedValuesUtils.getPluralName(sourceElement, "").asString();

                if (pluralName == "")
                {
                    pluralName = (sourceElement.Name != "" ? sourceElement.Name + "s" : sourceElement.Name + "s");

                    this.eaUtils.taggedValuesUtils.setPluralName( sourceElement, pluralName );
                    this.eaUtils.taggedValuesUtils.setPluralName(this.targetElement, this.framework.toCase(pluralName));
                }
                else
                {
                    this.eaUtils.taggedValuesUtils.setPluralName(this.targetElement, this.framework.toCase(pluralName));
                }
            }

            this.targetElement.Update();

            this.attributeOrColumn2ColumnOrAttribute.setSourceElement(sourceElement, addColumnPrefix);

            if ( sourceElement.Stereotype == "table")
            {
                foreach( EA.Attribute colummn in sourceElement.Attributes)
                {
                    attributeOrColumn2ColumnOrAttribute.toColumnOrAttribute(colummn);
                }
            }
            else
            {
                ModelClass2RealClass modelToClass = new ModelClass2RealClass(this.eaUtils);

                modelToClass.go(sourceElement);

                foreach (AttributeInfo attrinfo in modelToClass.realClass.getAttributesInfo())
                {
                    attributeOrColumn2ColumnOrAttribute.toColumnOrAttribute(attrinfo);
                }
            }

            this.targetElement.Attributes.Refresh();

            eaUtils.repository.BatchAppend = false;
        }

        public string buildTableName(string name, string mayBeNewName)
        {
            return name.ToLower().Contains("table") ? this.framework.toCase(mayBeNewName) : name;
        }

        /// <summary>
        /// Si una tabla realiza una clase y a esta tabla se le tira una clase que está asociada a la/s que realiza y si la cardinalidad
        /// es de 0..1 ó 1 entonces preguntamos si agregamos estas columnas con el prefijo de la clase ó el rol si lo tiviera.
        /// Hay que buscar en todos los niveles del árbol de herencia para verificar si esta clase está asociada.
        /// </summary>
        /// <returns></returns>
        private bool addColumnsWithPrefix(Element sourceElement)
        {
            bool addWithPrefix = false;

            List<ElementConnectorInfo> elementsAndConnectors = this.eaUtils.connectorUtils.get(this.targetElement, EAUtils.ConnectorUtils.CONNECTOR__REALISATION, null, "Class", null, false, null);

            foreach (ElementConnectorInfo elementConnectorInfo in elementsAndConnectors)
            {
                // si alguna de estas está asociada entonces analizamos la cardinalidad, si no buscamos de quien hereda y hacemos lo mismo.
                addWithPrefix = addColumnsWithPrefixCheckAsociation(elementConnectorInfo.element, sourceElement);

                if( addWithPrefix)
                {
                    break;
                }
            }
            return addWithPrefix;
        }

        private bool addColumnsWithPrefixCheckAsociation( Element current, Element sourceElement)
        {
            bool addWithPrefix = false;
            Connector candidate = null;
            List<ElementConnectorInfo> elementsAndConnectorsAsoc = this.eaUtils.connectorUtils.get(current, EAUtils.ConnectorUtils.CONNECTOR__ASSOCIATION, null, "Class", null, false, null);

            foreach (ElementConnectorInfo elementsAndConnectorAsoc in elementsAndConnectorsAsoc)
            {
                addWithPrefix = elementsAndConnectorAsoc.element.ElementID == sourceElement.ElementID;

                if (addWithPrefix)
                {
                    candidate = elementsAndConnectorAsoc.connector;
                    break;
                }
            }

            if ( addWithPrefix )
            {
                // analizamos cardinalidad.
                Cardinality cardinality = this.eaUtils.connectorUtils.getCardinality(candidate.SupplierEnd);

                addWithPrefix = (cardinality.lower == "0" && cardinality.upper == "1") || (cardinality.lower == "1" && cardinality.upper == "1");
            }
            else
            {
                // subimos por el árbol de herencia hacemos lo mismo.
                List<ElementConnectorInfo> elementsAndConnectorsGeneralization = this.eaUtils.connectorUtils.get(current, EAUtils.ConnectorUtils.CONNECTOR__GENERALIZATION, null, "Class", null, false, null);
                foreach (ElementConnectorInfo elementsAndConnectorGeneralization in elementsAndConnectorsGeneralization)
                {
                    addWithPrefix = addColumnsWithPrefixCheckAsociation(elementsAndConnectorGeneralization.element, sourceElement);

                    if( addWithPrefix)
                    {
                        break;
                    }
                }
                // subimos por el árbol de realizaciones (por si hay interfaces) hacemos lo mismo.
                List<ElementConnectorInfo> elementsAndConnectorsRealization = this.eaUtils.connectorUtils.get(current, EAUtils.ConnectorUtils.CONNECTOR__REALISATION, null, "Class", null, false, null);
                foreach (ElementConnectorInfo elementAndConnectorRealization in elementsAndConnectorsGeneralization)
                {
                    addWithPrefix = addColumnsWithPrefixCheckAsociation(elementAndConnectorRealization.element, sourceElement);

                    if (addWithPrefix)
                    {
                        break;
                    }
                }
            }
            return addWithPrefix;
        }
    }
}
