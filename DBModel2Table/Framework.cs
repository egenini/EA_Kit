using EA;
using EAUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Model2Table
{
    public class Framework
    {
        const string TABLE_NAME_CASE_SNAKE_LOWER = "SNAKE_LOWER";
        const string TABLE_NAME_CASE_SNAKE_UPPER = "SNAKE_UPPER";
        const string TABLE_NAME_CASE_PASCAL = "PASCAL";
        const string TABLE_NAME_CASE_CAMEL = "CAMEL";

        const string FK_ATTRIBUTE_POSITION_IN_CLASS = "in_class";
        const string FK_ATTRIBUTE_POSITION_IN_RELATION = "in_relation";

        const string FK_ATTRIBUTE_POSITION_PREFIX = "prefix";
        const string FK_ATTRIBUTE_POSITION_POSTFIX = "postfix";

        public Dictionary<int, Diagram> diagramasConectados = null;

        EAUtils.EAUtils eaUtils;
        Repository repository;
        Package package;
        TableOptions tableOptions = null;
        ClassOptions classOptions = null;

        Dictionary<string, Dictionary<string, string>> tiposDeDatos = new Dictionary<string, Dictionary<string, string>>();
        public Framework(EAUtils.EAUtils eaUtils, Package optionPackage) 
        { 
            this.eaUtils = eaUtils;
            
            foreach( Element element in optionPackage.Elements )
            {
                if( element.Name == "TableOptions")
                {
                    this.tableOptions = new TableOptions(element);
                }
                else if( element.Name == "ClassOptions")
                {
                    this.classOptions = new ClassOptions(element);
                }

                else if (element.Name == "TypePreferences")
                {
                    leerTiposDeDatos(element);
                }
                
                if ( this.tableOptions != null && this.classOptions != null)
                {
                    break;
                }
            }
        }
        public bool isSourceDiagram(EA.Diagram diagram)
        {
            return diagram != null && diagramasConectados.ContainsKey(diagram.DiagramID);
        }
        public Diagram targetDiagram(Diagram diagram)
        {
            Diagram d = null;

            if (diagramasConectados.ContainsKey(diagram.DiagramID))
            {
                d = diagramasConectados[diagram.DiagramID];
            }

            return d;
        }
        private void leerTiposDeDatos(Element element)
        {
            List<int> herederos = this.eaUtils.elementUtils.heredanDeMi(element.ElementID);

            EA.Element heredero;
            string key;

            foreach (int id in herederos)
            {
                heredero = this.eaUtils.repository.GetElementByID(id);

                key = heredero.Alias + "-" + heredero.Name;

                this.tiposDeDatos.Add( key, new Dictionary<string, string>(heredero.Attributes.Count));

                foreach( EA.Attribute a in heredero.Attributes)
                {
                    tiposDeDatos[key][a.Name] = a.Default;
                }
            }
        }
        public string tableName(string name)
        {
            return this.tableOptions.name(name);
        }
        public string columnName(string name)
        {
            return this.tableOptions.column(name);
        }

        public string idColumnName( Element table)
        {
            return tableOptions.idColumnName(table);
        }
        public string idColumnType(Element model, Element tabla, string v)
        {
            string dataType = "VARCHAR";

            string k = model.Gentype +"-"+ tabla.Gentype;

            if(this.tiposDeDatos.ContainsKey(k))
            {
                dataType = this.tiposDeDatos[k].ContainsKey(v) ? this.tiposDeDatos[k][v] : dataType;
            }

            return dataType;
        }
        public string pkTable(Element table)
        {
            return tableOptions.pkTable(table);
        }
        public string fkTable(Element source, Element target)
        {
            return tableOptions.fkTable(source, target);
        }
        public string className(string name)
        {
            return this.classOptions.name(name);
        }
        public string attributeName(string name)
        {
            return this.classOptions.attribute(name);
        }
        public string idAttribute(Element c)
        {
            return this.classOptions.idAttribute(c);
        }
        public bool addIdAttribute()
        {
            return classOptions.addId;
        }
        public string dataType( EA.Element modelo, EA.Element tabla, string sourceType  ) 
        {
            string k = modelo.Gentype + "-" + tabla.Gentype;
            string datatype = "VARCHAR";

            if( this.tiposDeDatos.ContainsKey(k) )
            {
                datatype = this.tiposDeDatos[k].ContainsKey(sourceType) ? this.tiposDeDatos[k][sourceType] : datatype;
            }

            return datatype;
        }
    }
    internal class TableOptions
    {
        public short tableNameCase;
        public bool tableNameSnakeUpper = true;
        public short columnNameCase;
        public bool columnNameSnakeUpper = false;
        public string idColumnTemplate;
        public string idColumnType = "integer";
        public string fkIdColumnTemplate;
        public string pkTemplate;
        public string fkTemplate;
        public TableOptions(Element element)
        {
            foreach (EA.Attribute attribute in element.Attributes)
            {
                switch(attribute.Name)
                {
                    case "table_name_case":
                        if( attribute.Default == "snake_lower" )
                        {
                            tableNameCase = StringUtils.SNAKE;
                            tableNameSnakeUpper = false;
                        }
                        else if (attribute.Default == "snake_upper")
                        {
                            tableNameCase = StringUtils.SNAKE;
                            
                        }
                        else if( attribute.Default == "Pascal")
                        {
                            tableNameCase = StringUtils.PASCAL;
                        }
                        else if( attribute.Default == "camel")
                        {
                            tableNameCase = StringUtils.CAMEL;
                        }
                        break;
                    case "column_name_case":
                        if( attribute.Default == "snake_lower")
                        {
                            columnNameCase = StringUtils.SNAKE;
                        }
                        else if( attribute.Default == "snake_upper")
                        {
                            columnNameCase = StringUtils.SNAKE;
                            columnNameSnakeUpper= true;
                        }
                        else if( attribute.Default == "Pascal")
                        {
                            columnNameCase = StringUtils.PASCAL;
                        }
                        else if( attribute.Default == "camel")
                        {
                            columnNameCase = StringUtils.CAMEL;
                        }
                        
                        break;
                    case "id_column_template":
                        idColumnTemplate = attribute.Default;
                        break;
                    case "id_column_type":
                        idColumnType = attribute.Default;
                        break;
                    case "fk_id_column_template":
                        fkIdColumnTemplate = attribute.Default;
                        break;
                    case "pk_template":
                        pkTemplate = attribute.Default;
                        break;
                    case "fk_template":
                        fkTemplate = attribute.Default;
                        break;
                    default:
                        break;
                }
            }
        }
        public string name( string tableName )
        {
            string newName = StringUtils.to(tableNameCase, tableName);
            return tableNameCase == StringUtils.SNAKE ? ( tableNameSnakeUpper ? newName.ToUpper() : newName.ToLower() ) : newName; 
        }
        public string column(string name)
        {
            string newName = StringUtils.to(columnNameCase, name);
            return columnNameCase == StringUtils.SNAKE ? (columnNameSnakeUpper ? newName.ToUpper() : newName.ToLower() ) : newName;
        }

        internal string idColumnName(Element table)
        {
            return this.idColumnTemplate.Replace("{{table.name}}", this.column(table.Name));
        }
        internal string pkTable(Element table)
        {
            return this.pkTemplate.Replace("{{table.name}}", this.name(table.Name)).Replace("{{id_column}}", idColumnName(table) );
        }
        internal string fkTable(Element source, Element target)
        {
            return this.fkTemplate.Replace("{{source.name}}", this.name(target.Name)).Replace("{{target.name}}", this.name(target.Name));
        }
    }
    internal class ClassOptions
    {
        public short classNameCase = StringUtils.PASCAL;
        public bool classNameCaseUpper = false;
        public short attributeNameCase = StringUtils.CAMEL;
        public bool attributeNameCaseUpper = false;
        public string idAttributeTemplate;
        public bool fkIdAttributLocationClass;
        public bool addId = false;
        public string idType;

        public ClassOptions(Element element)
        {
            foreach( EA.Attribute attribute in element.Attributes)
            {
                switch(attribute.Name)
                {
                    case "class_name_case":
                        if( attribute.Default == "snake_lower" )
                        {
                            classNameCase = StringUtils.SNAKE;
                            classNameCaseUpper = false;
                        }
                        else if (attribute.Default == "snake_upper")
                        {
                            classNameCase = StringUtils.SNAKE;
                        }
                        else if (attribute.Default == "Pascal")
                        {
                            classNameCase = StringUtils.PASCAL;
                        }
                        else if (attribute.Default == "camel")
                        {
                            classNameCase = StringUtils.CAMEL;
                        }

                        break;
                    case "attribute_name_case":
                        if (attribute.Default == "snake_lower")
                        {
                            attributeNameCase = StringUtils.SNAKE;
                            attributeNameCaseUpper = false;
                        }
                        else if (attribute.Default == "snake_upper")
                        {
                            attributeNameCase = StringUtils.SNAKE;
                        }
                        else if (attribute.Default == "Pascal")
                        {
                            attributeNameCase = StringUtils.PASCAL;
                        }
                        else if (attribute.Default == "camel")
                        {
                            attributeNameCase = StringUtils.CAMEL;
                        }
                        break;
                    case "id_attribute_template":
                        idAttributeTemplate = attribute.Default;
                        break;
                    case "add_id_attribute":
                        addId = attribute.Default == "true";
                        break;
                    case "id_attribute_type":
                        idType = attribute.Default;
                        break;
                    case "fk_id_attribute_location":
                        fkIdAttributLocationClass = attribute.Default == "in_class";
                        break;
                    default:
                        break;
                }
            }
        }
        public string name(string className)
        {
            string newName = StringUtils.to(classNameCase, className);
            return classNameCase == StringUtils.SNAKE ? (this.classNameCaseUpper ? newName.ToUpper() : newName.ToLower()) : newName;
        }
        public string attribute(string name)
        {
            string newName = StringUtils.to(attributeNameCase, name);

            return this.attributeNameCase == StringUtils.SNAKE ? (this.attributeNameCaseUpper ? newName.ToUpper() : newName.ToLower()) : newName;
        }

        internal string idAttribute(Element c)
        {
            return this.idAttributeTemplate.Replace("{{class.name}}", this.name(c.Name));
        }
    }
}
