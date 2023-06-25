using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EAUtils
{
    /// <summary>
    /// Obtiene los tipos de datos definidos para los lenguajes, sea de base de datos o de programación.
    /// EA provee un sistema de mapeo entre datos de base de datos y además uno a partir de un "común"
    /// que sirve de pivot entre los lenguajes.
    /// </summary>
    public class DataTypeUtils
    {
        public const string DATA_TYPE__TYPE_DDL  = "DDL";
        public const string DATA_TYPE__TYPE_CODE = "Code";

        // orden natural de las columnas en el select *
        private const int COL__TYPE_INDEX          = 0;
        private const int COL__PRODUCT_NAME_INDEX  = 1;
        private const int COL__DATA_TYPE_INDEX     = 2;
        private const int COL__SIZE_INDEX          = 3;
        private const int COL__MAX_LEN_INDEX       = 4;
        private const int COL__MAX_PREC_INDEX      = 5;
        private const int COL__MAX_SCALE_INDEX     = 6;
        private const int COL__DEFAULT_LEN_INDEX   = 7;
        private const int COL__DEFAULT_PREC_INDEX  = 8;
        private const int COL__DEFAULT_SCALE_INDEX = 9;
        private const int COL__USER_INDEX          = 10;
        private const int COL__PDATA1_INDEX        = 11;
        private const int COL__PDATA2_INDEX        = 12;
        private const int COL__PDATA3_INDEX        = 13;
        private const int COL__PDATA4_INDEX        = 14;
        private const int COL__HAS_LENGTH_INDEX    = 15;
        private const int COL__GENERIC_TYPE_INDEX  = 16;
        private const int COL__DATA_TYPE_ID_INDEX  = 17;

        List<string> columnNames = new List<string>();

        Repository repository;

        public DataTypeUtils(Repository repository)
        {
            this.repository = repository;
        }

        public DataTypeInfo getFrom(string languaje, string type, string targetLanguaje)
        {
            string typeFrom = this.genericType(languaje, type);

            DataTypeInfo dataTypeInfo = null;

            if ( typeFrom != null && typeFrom != "")
            {
                dataTypeInfo = this.getFromGenericType(targetLanguaje, typeFrom);

                // por ej: de PostgreSQL a Java
                if(dataTypeInfo == null)
                {
                    // buscamos el generic type en el lenguaje SQL (creado como puente entre ambos)
                   //string genericTypeSQL = this.genericType("SQL", type);
                   string genericTypeSQL = this.genericType("SQL", typeFrom);

                    dataTypeInfo = this.getFromGenericType(targetLanguaje, genericTypeSQL);
                }
            }

            return dataTypeInfo;
        }

        private string genericType(string languaje, string type)
        {
            var sql = "SELECT generictype FROM t_datatypes WHERE productname = '" + languaje + "' and datatype = '" + type + "'";

            string genericType = null;

            string queryResult = repository.SQLQuery(sql);

            if (queryResult.Length > 0)
            {
                XmlDocument xmlDOM = new XmlDocument();
                try
                {
                    xmlDOM.LoadXml(queryResult);

                    XmlNode rootNode = xmlDOM.DocumentElement.SelectSingleNode("/EADATA/Dataset_0/Data");

                    if (rootNode.ChildNodes.Count != 0)
                    {
                        var currentNode = rootNode.ChildNodes.Item(0);

                        genericType = currentNode.ChildNodes[0].InnerText;
                    }
                }
                catch (Exception w) { w.ToString(); }
            }
            return genericType;
        }

        public DataTypeInfo getFromGenericType( string targetLanguaje, string genericType )
        {
            var sql = "SELECT * FROM t_datatypes WHERE productname = '" + targetLanguaje + "' and generictype = '" + genericType + "'";

            DataTypeInfo dataTypeInfo = null;

            string queryResult = repository.SQLQuery(sql);

            if (queryResult.Length > 0)
            {
                XmlDocument xmlDOM = new XmlDocument();
                try
                {
                    xmlDOM.LoadXml(queryResult);

                    XmlNode rootNode = xmlDOM.DocumentElement.SelectSingleNode("/EADATA/Dataset_0/Data");

                    if (rootNode.ChildNodes.Count != 0)
                    {
                        var currentNode = rootNode.ChildNodes.Item(0);

                        dataTypeInfo = new DataTypeInfo();

                        dataTypeInfo.genericType   = currentNode.ChildNodes[COL__GENERIC_TYPE_INDEX].InnerText;
                        dataTypeInfo.isUserDefined = currentNode.ChildNodes[COL__USER_INDEX].InnerText == "1";
                        dataTypeInfo.name          = currentNode.ChildNodes[COL__DATA_TYPE_INDEX].InnerText;
                        dataTypeInfo.product       = currentNode.ChildNodes[COL__PRODUCT_NAME_INDEX].InnerText;
                        dataTypeInfo.size          = currentNode.ChildNodes[COL__SIZE_INDEX].InnerText;
                        dataTypeInfo.type          = currentNode.ChildNodes[COL__TYPE_INDEX].InnerText;
                        
                        // ddl 
                        dataTypeInfo.ddl.defaultLength    = currentNode.ChildNodes[COL__DEFAULT_LEN_INDEX].InnerText;
                        dataTypeInfo.ddl.defaultPrecision = currentNode.ChildNodes[COL__DEFAULT_PREC_INDEX].InnerText;
                        dataTypeInfo.ddl.defaultScale     = currentNode.ChildNodes[COL__DEFAULT_SCALE_INDEX].InnerText;
                        dataTypeInfo.ddl.maxLength        = currentNode.ChildNodes[COL__MAX_LEN_INDEX].InnerText;
                        dataTypeInfo.ddl.maxPresicion     = currentNode.ChildNodes[COL__MAX_PREC_INDEX].InnerText;
                        dataTypeInfo.ddl.maxScale         = currentNode.ChildNodes[COL__MAX_SCALE_INDEX].InnerText;

                    }
                }
                catch (Exception w) { w.ToString(); }
            }
            return dataTypeInfo;
        }
    }

    public class DataTypeInfo
    {
        /// <summary>
        /// The associated generic type for this data type.
        /// </summary>
        public string genericType;
        /// <summary>
        /// The datatype name (such as integer). This appears in the related drop-down datatype lists where appropriate.
        /// </summary>
        public string name;
        /// <summary>
        /// The datatype product, such as Java, C++ or Oracle.
        /// </summary>
        public string product;
        /// <summary>
        /// The type can be DDL for database datatypes or Code for language datatypes.
        /// </summary>
        public string type;
        /// <summary>
        /// The datatype size.
        /// </summary>
        public string size;
        /// <summary>
        /// Indicates if the datatype is a user defined type or system generated.
        /// </summary>
        public bool isUserDefined = false;
        public bool hasLength = false;

        public DDLExtension ddl = new DDLExtension();

    }

    public class DDLExtension
    {
        public string defaultLength;
        public string defaultPrecision;
        public string defaultScale;
        public string maxLength;
        public string maxPresicion;
        public string maxScale;

    }

}
