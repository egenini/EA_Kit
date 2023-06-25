using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EA;
using DMN.dominio;
using Productividad.classtable;

namespace DMN.classtable
{
    /// <summary>
    /// Agrega una columna por cada variable de una regla de negocio a un elemento table cuando un elemento
    /// BusinessKnowledge es soltado sobre un elemento con estereotipo table.
    /// TODO: Falta resolver la cuestión de los tipos de datos en DMN y la generación de columnas.
    /// </summary>
    public class BusinessKnwoledge2Table : Class2Table
    {
        public BusinessKnwoledge2Table(Repository repository, Element table) : base(repository, table)
        {
            this.table = table;

            this.eaUtils = new EAUtils.EAUtils();
            this.eaUtils.setRepositorio(repository);

            this.attribute2Column = new Attribute2Column(repository, table);
        }

        public void toTable(Decision decision)
        {
            eaUtils.repository.BatchAppend = true;

            this.framework = new Framework(eaUtils);

            Element toTable = decision.element;

            if (toTable.Gentype == "" )
            {
                // no lo grabamos porque es para que busque el tipo de dato.
                toTable.Gentype = "Java";
            }
             
            table.Name = this.buildTableName(table.Name, toTable.Alias == "" ? toTable.Name : toTable.Alias);

            // todo: buscar como era eso de normalizar las notas de un elemento.
            table.Notes = toTable.Notes;

            table.Update();

            attribute2Column.classOwner = toTable;

            string type      = null;
            string precision = null;
            string scale     = null;
            string columnName;
            EA.Attribute column;

            foreach (Condition condicion in decision.conditions)
            {
                columnName = attribute2Column.buildColumnName(condicion.attributeName == "" ? condicion.businessName : condicion.attributeName);

                column = attribute2Column.getColumn(columnName);

                foreach (var dataType in condicion.dataType)
                {
                    // esto quiere decir que tiene este tipo como variable
                    if (dataType.Value)
                    {
                        type = dataType.Key.ToString();

                        break;
                    }
                }

                // busco en las reglas en valor más largo
                precision = this.getPrecision(decision, condicion);

                if (column == null)
                {
                    attribute2Column.addColumn(columnName, type, precision, scale, null);
                }
                else
                {
                    attribute2Column.updateColumn(column, type, precision, scale, null);
                }

                foreach (Conclusion conclusion in decision.conclusions)
                {
                    columnName = attribute2Column.buildColumnName(conclusion.attributeName == "" ? conclusion.businessName : conclusion.attributeName);

                    column = attribute2Column.getColumn(columnName);

                    foreach (var dataType in conclusion.dataType)
                    {
                        // esto quiere decir que tiene este tipo como variable
                        if (dataType.Value)
                        {
                            type = dataType.Key.ToString();

                            break;
                        }
                    }

                    // busco en las reglas en valor más largo
                    precision = this.getPrecision(decision, conclusion);

                    if (column == null)
                    {
                        attribute2Column.addColumn(columnName, type, precision, scale, null);
                    }
                    else
                    {
                        attribute2Column.updateColumn(column, type, precision, scale, null);
                    }
                }
            }
        }

        private string getPrecision(Decision decision, Condition condicion)
        {
            int len          = 0;
            int ruleValueLen = 0;

            foreach( Rule rule in decision.rules)
            {
                ruleValueLen = rule.conditionValueByAttributeName[condicion.attributeName].value.Length;

                if (ruleValueLen > len)
                {
                    len = ruleValueLen;
                }
            }
            return len.ToString();
        }
        private string getPrecision(Decision decision, Conclusion conclusion)
        {
            int len = 0;
            int ruleValueLen = 0;

            foreach (Rule rule in decision.rules)
            {
                ruleValueLen = rule.conclusionValueByAttributeName[conclusion.attributeName].value.Length;

                if (ruleValueLen > len)
                {
                    len = ruleValueLen;
                }
            }
            return len.ToString();
        }
    }
}