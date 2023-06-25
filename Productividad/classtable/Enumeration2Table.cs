using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EA;
using EAUtils;

namespace Productividad.classtable
{
    class Enumeration2Table : Class2Table
    {
        private Repository repository;

        public Enumeration2Table(Repository repository, Element dropped) : base (repository, dropped)
        {
            this.repository = repository;
            this.table = dropped;
            this.eaUtils = new EAUtils.EAUtils();

            this.eaUtils.setRepositorio(repository);
        }

        internal new void toTable(Element toTable)
        {
            if( framework == null)
            {
                framework = new Framework(this.eaUtils);
            }
            
            table.Name = framework.toCase(toTable.Name);
            table.Update();

            eaUtils.taggedValuesUtils.set(table, "source.guid", toTable.ElementGUID);

            attribute2Column = new Attribute2Column(repository, this.table);

            attribute2Column.framework = this.framework;

            // el tipo de columna lo obtenemos del primer atributo.
            if (toTable.Attributes.Count != 0)
            {
                addColumnKey(toTable);
                addColumnValue(toTable);
                addColumnLabel(toTable);
            }
        }

        private string getPrecision(Element toTable, string forColumn)
        {
            int len = 0;
            foreach (EA.Attribute attribute in toTable.Attributes)
            {
                if(forColumn == "value")
                {
                    if (attribute.Default.Length > len)
                    {
                        len = attribute.Default.Length;
                        // no memos de 1 caracter
                        len = len == 0 ? 1 : len;
                    }
                }
                else if( forColumn == "key")
                {
                    if (attribute.Name.Length > len)
                    {
                        len = attribute.Name.Length;
                        // no memos de 1 caracter
                        len = len == 0 ? 1 : len;
                    }
                }
                else if (forColumn == "label")
                {
                    if (attribute.Alias.Length > len)
                    {
                        len = attribute.Alias.Length;
                    }
                }
            }

            return len.ToString();
        }

        private void addColumnKey( Element toTable )
        {
            EA.Attribute column;
            string columnName;
            string precision = null;
            string scale = null;
            string type = "varchar";

            precision = this.getPrecision(toTable, "key");

            columnName = attribute2Column.buildColumnName(framework.enumerationInfo.keyColumnName);

            column = attribute2Column.getColumn(columnName);

            if (column == null)
            {
                column = attribute2Column.addColumn(columnName, type, precision, scale, null);
                column.IsOrdered = true;
                column.AllowDuplicates = true;
                column.Update();
                attribute2Column.buildPrimaryKey(column);
            }
            else
            {
                attribute2Column.updateColumn(column, type, precision, scale, null);
            }
        }
        private void addColumnValue( Element toTable ) 
        {
            DataTypeInfo dataTypeInfo = null;
            EA.Attribute column;
            string columnName;
            string precision = null;
            string scale = null; // no hay de donde obtenerla por el momento.
            string type = null;
            EA.Attribute current;

            columnName = attribute2Column.buildColumnName(framework.enumerationInfo.valueColumnName);

            current      = toTable.Attributes.GetAt(0);
            dataTypeInfo = eaUtils.dataTypeUtils.getFrom(toTable.Gentype, current.Type, "SQL");
            type         = dataTypeInfo.name;

            // solo para los tipos que llevan precision.
            if (type == "varchar" || type == "char")
            {
                // El largo (presicion) se obtiene de buscar el valor más largo
                precision = this.getPrecision(toTable, "value");
            }

            column = attribute2Column.getColumn(columnName);

            if (column == null)
            {
                column = attribute2Column.addColumn(columnName, type, precision, scale, null);
                column.AllowDuplicates = true;
                column.Update();
            }
            else
            {
                attribute2Column.updateColumn(column, type, precision, scale, null);
            }
        }

        private void addColumnLabel( Element toTable )
        {
            EA.Attribute column;
            string columnName;
            string precision = null;
            string scale = null;
            string type = "varchar";

            precision = this.getPrecision(toTable, "label");

            if( precision != "0" )
            {
                columnName = attribute2Column.buildColumnName(framework.enumerationInfo.labelColumnName);

                column = attribute2Column.getColumn(columnName);

                if (column == null)
                {
                    column = attribute2Column.addColumn(columnName, type, precision, scale, null);
                    column.Update();
                }
                else
                {
                    attribute2Column.updateColumn(column, type, precision, scale, null);
                }
            }
        }
    }
}
