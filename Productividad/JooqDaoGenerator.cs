using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EA;
using Productividad.framework;
using Jint;
using System.Windows.Forms;
using UIResources;
using Jint.Native.Json;
using EAUtils;
using System.IO;
using Newtonsoft.Json;
using Productividad.framework.ui;
using System.Text.RegularExpressions;

namespace Productividad
{
    public class JooqDaoGenerator
    {
        Element element;
        Framework frameworkInstance;
        EAUtils.EAUtils eaUtils;
        DaoConfiguration configuration;
        string path;
        string fileName;
        List<String> foreingKeys = new List<string>();

        public void doIt(Element element, Framework frameworkInstance, EAUtils.EAUtils eaUtils)
        {
            this.element = element;
            this.frameworkInstance = frameworkInstance;
            this.eaUtils = eaUtils;

            List<ConnectorUtils.ElementConnectorInfo>  ecList = eaUtils.connectorUtils.get(element, EAUtils.ConnectorUtils.CONNECTOR__ASSOCIATION, "FK", "Class", "table", false, null);

            foreach(ConnectorUtils.ElementConnectorInfo ecInfo in ecList)
            {
                if( ecInfo.connector.Name.Contains(" = ")){

                    // el nombre de la relación aparece como por ej: (telefono_id = id), donde telefono_id es la fk.
                    foreingKeys.Add(ecInfo.connector.Name.Substring(1, (ecInfo.connector.Name.IndexOf("=") - 2)));
                }
            }

            configure();
        }

        private void configure()
        {
            string tgvName = frameworkInstance.languageForCodeGeneration.name;
            string infoString = eaUtils.taggedValuesUtils.get(element, tgvName, "").asString();
            string extraInfoString = eaUtils.taggedValuesUtils.get(element, tgvName + "_extension", "{}").asString();

            this.configuration = (DaoConfiguration)eaUtils.repository.AddTab("Configure " + element.Name, "Productividad.framework.ui.DaoConfiguration");
            this.configuration.setCaller(this);

            // Obtiene la última configuración actualizada según las columnas actuales de la tabla.
            // Agrega a la pantalla los datos que estaban previamente configurados.
            JooqDaoInfo jooqInfo = fromLastConfiguration(infoString);
            // obtiene las columnas "nuevas" y las agrega al final.
            List<string> columnNames = getFilteredActualColummns(jooqInfo);

            // sino hay configuración previa.
            if (jooqInfo == null)
            {
                configuration.setFromTableName(element);

            }

            configuration.Show(true);
            eaUtils.activateTab("Configure " + element.Name);

        }

        private JooqDaoInfo fromLastConfiguration(string infoString)
        {
            JooqDaoInfo jooqInfo = null;

            if (infoString != "")
            {
                jooqInfo = new JooqDaoInfo();
                jooqInfo.parse(infoString);

                this.path = jooqInfo.path;
                this.fileName = jooqInfo.fileName;

                configuration.set(jooqInfo.entity, jooqInfo.package, jooqInfo.package_entity, jooqInfo.package_tables, jooqInfo.table_class, jooqInfo.table_constant);

                JooqDaoColumnAttribute colAttr;
                bool isValid = true;
                for (int i = jooqInfo.column_info.Count - 1; i >= 0; i--)
                {
                    isValid = false;
                    colAttr = jooqInfo.column_info[i];

                    foreach (EA.Attribute attr in element.Attributes)
                    {
                        if (colAttr.column_name == attr.Name.ToUpper())
                        {
                            isValid = true;
                            break;
                        }
                    }
                    if (!isValid)
                    {
                        jooqInfo.column_info.RemoveAt(i);
                    }
                    else
                    {
                        configuration.add(colAttr.column_name, colAttr.attribute_name, colAttr.pk, colAttr.search);
                    }
                }
            }
            return jooqInfo;
        }

        private List<string> getFilteredActualColummns(JooqDaoInfo jooqInfo)
        {
            List<string> columnNames = new List<string>();
            string[] filter = null;
            string notes = frameworkInstance.languageForCodeGeneration.element.Notes;

            int filterIndex;
            if (notes != "" && (filterIndex = notes.IndexOf("filter=")) != -1)
            {
                notes = notes.Substring(filterIndex + 7);
                notes = notes.Substring(0, notes.IndexOf(";"));
                if (notes != "")
                {
                    filter = notes.Split(',');
                }
            }

            foreach (EA.Attribute attr in element.Attributes)
            {
                bool add = true;
                if (filter != null)
                {
                    foreach (string filt in filter)
                    {
                        if (Regex.IsMatch(attr.Name, filt))
                        {
                            add = false;
                            break;
                        }
                    }
                }
                if (add)
                {
                    if (jooqInfo != null)
                    {
                        if (!previousConfiguration(attr.Name, jooqInfo))
                        {
                            configuration.add(attr.Name.ToUpper(), EAUtils.StringUtils.toPascal(attr.Name.ToLower()));
                        }
                    }
                    else
                    {
                        configuration.add(attr.Name.ToUpper(), EAUtils.StringUtils.toPascal(attr.Name.ToLower()));
                    }
                }
            }
            return columnNames;
        }

        private bool previousConfiguration(String name, JooqDaoInfo jooqInfo)
        {
            bool has = false;

            foreach (JooqDaoColumnAttribute colAttr in jooqInfo.column_info)
            {
                if (colAttr.column_name == name.ToUpper())
                {
                    has = true;
                    break;
                }
            }
            return has;
        }

        public void onAceptConfigure(JooqDaoInfo info)
        {
            // tomo los datos y los guardo en el valor etiquetado.
            bool saveFile = false;
            // pregunto si se quiere al porta papeles o al disco
            DialogResult dialogResult = MessageBox.Show("Guardar en el disco", "Opciones de generación", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                saveFileDialog1.InitialDirectory = path != null ? path : @"C:\";

                saveFileDialog1.FileName = fileName == null ? info.table_class + "DaoBase" : fileName;

                saveFileDialog1.Title = "Guardar";

                saveFileDialog1.CheckFileExists = false;

                saveFileDialog1.CheckPathExists = true;

                saveFileDialog1.DefaultExt = "java";

                saveFileDialog1.Filter = "Fuente Java (*.java)|*.java";

                saveFileDialog1.FilterIndex = 1;

                saveFileDialog1.RestoreDirectory = false;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)

                {
                    info.fileName = saveFileDialog1.FileName;
                    saveFile = true;
                }
            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }

            // agregamos/actualizamos cada columna columna con los valores 
            // de realPk y search. En este lugar agregamos el tipo de dato de la columna!!!.
            foreach (JooqDaoColumnAttribute colAttr in info.column_info)
            {
                foreach (EA.Attribute attr in this.element.Attributes)
                {
                    if (attr.Name == colAttr.column_name.ToLower())
                    {
                        // todo agregamos acá la info que indica si es o no una foreing key.
                        if( this.foreingKeys.Contains(attr.Name))
                        {
                            colAttr.fk = true;
                        }

                        colAttr.column_type = attr.Type;
                        this.eaUtils.taggedValuesUtils.set(attr, EAUtils.TaggedValuesUtils.EN__REAL_PK, colAttr.pk ? "true" : "false");
                        this.eaUtils.taggedValuesUtils.set(attr, EAUtils.TaggedValuesUtils.EN__BUSCAR, colAttr.search ? "true" : "false");
                        break;
                    }
                }
            }
            this.element.Attributes.Refresh();

            eaUtils.taggedValuesUtils.set(element, frameworkInstance.languageForCodeGeneration.name, info.stringfity());

            if (generate())
            {
                if (!saveFile)
                {
                    Alert.Success(Properties.Resources.copiado_portapapeles);
                }
                else
                {
                    try
                    {
                        System.IO.File.WriteAllText(info.path + info.fileName, Clipboard.GetText());

                        Alert.Success(Properties.Resources.guardado_ok);

                    }
                    catch (Exception e)
                    {
                        Clipboard.SetText(e.ToString());
                    }
                }
            }
        }

        // Se necesita:
        // 1. El elemento table. (se hace con botón derecho sobre el elemento)
        // 2. El lenguaje a exportar. (se elige desde una ventana especial)
        // 3. datos variables:
        // 3.1 paquete para la clase. Se solicita desde una ventana.
        // 3.2 El directorio donde generar la clase o bien al clippboard. será un botón en la ventana de configuración.
        // 3.3 Las columnas que son parte de la pk real. ventana para elegir podría ser una ventana genérica
        // 3.4 Las columnas que se pueden usar para buscar la entidad se preseleccionan las de la pk real (podrían ser las mismas a usar en el insert y update).
        internal bool generate()
        {
            // esto no tiene que estar
            bool generated = false;

            Engine jsEngine = new Engine();

            jsEngine.Execute(frameworkInstance.handlebarsJs);

            string infoString = eaUtils.taggedValuesUtils.get(element, frameworkInstance.languageForCodeGeneration.name, "").asString();
            string extraInfoString = eaUtils.taggedValuesUtils.get(element, frameworkInstance.languageForCodeGeneration.name + "_extension", "{}").asString();
            string pojoInfoString = eaUtils.taggedValuesUtils.get(element, "Java-Pojo", "{}").asString();

            if (infoString == "")
            {

            }
            else
            {
                // armamos un 1 sólo objeto con todo
                infoString = infoString.Substring(0, infoString.Length - 1)
                    + ",\"entityInfo\":" + pojoInfoString
                    + ",\"extraInfo\":" + extraInfoString
                    + "}";

                var data = new JsonParser(jsEngine).Parse(infoString );

                //var dataExtra = new JsonParser(jsEngine).Parse(extraInfoString);
                //var entityInfo = new JsonParser(jsEngine).Parse("{\"entityInfo\":"+ pojoInfoString +"}");

                jsEngine.SetValue("data", data);
                //jsEngine.SetValue("dataExtra", dataExtra);
                //jsEngine.SetValue("entityInfo", entityInfo);

                try
                {
                    jsEngine.SetValue(frameworkInstance.languageForCodeGeneration.name, data);
                    //jsEngine.SetValue(frameworkInstance.languageForCodeGeneration.name + "_extension", dataExtra);

                    jsEngine.SetValue("mainTemplate", frameworkInstance.languageForCodeGeneration.mainTemplate);


                    foreach (EA.Method method in frameworkInstance.languageForCodeGeneration.element.Methods)
                    {
                        if (method.Alias != "Handlebars.registerHelper")
                        {
                            jsEngine.Execute(method.Code);
                        }
                    }

                    foreach (EA.Method method in frameworkInstance.languageForCodeGeneration.element.Methods)
                    {
                        if (method.Alias == "Handlebars.registerHelper")
                        {
                            jsEngine.Execute("Handlebars.registerHelper('" + method.Name + "', " + method.Code + " );");
                        }
                    }

                    foreach (KeyValuePair<string, string> htemplatesKV in frameworkInstance.languageForCodeGeneration.handlebarsTemplates)
                    {
                        jsEngine.SetValue(htemplatesKV.Key + "_template", htemplatesKV.Value);

                    }

                    foreach (KeyValuePair<string, string> htemplatesKV in frameworkInstance.languageForCodeGeneration.handlebarsTemplates)
                    {
                        jsEngine.Execute("Handlebars.registerPartial('" + htemplatesKV.Key + "', " + htemplatesKV.Key + "_template" + " );");
                    }

                    jsEngine.Execute("var template = Handlebars.compile(mainTemplate);");
                    var result = jsEngine.Execute("template(data)").GetCompletionValue();

                    //var result = Handlebars.Run("main", data);
                    if (result == null || result == "")
                    {
                        result = "no se pudo generar el código";
                        Alert.Info(result.ToString());
                    }
                    else
                    {
                        generated = true;
                        Clipboard.SetText(result.ToString());

                    }
                }
                catch (Exception e)
                {
                    Clipboard.SetText(e.ToString());
                }
            }
            return generated;
        }
    }

    public class JooqDaoInfo
    {
        public string fileName = null;
        public string path = null;
        public string package;
        public string package_tables;
        public string package_entity;
        public string entity;
        public string table_class;
        public string table_constant;
        public List<JooqDaoColumnAttribute> column_info = new List<JooqDaoColumnAttribute>();

        public void setTableName( string tableName )
        {
            this.table_constant = tableName.ToUpper();
            this.table_class = EAUtils.StringUtils.toPascal(tableName);
            this.entity = table_class;
        }

        public void add(string columnName, string attributeName)
        {
            column_info.Add(new JooqDaoColumnAttribute(columnName, attributeName));
        }

        public void add(string columnName, string attributeName, bool pk, bool search)
        {
            column_info.Add(new JooqDaoColumnAttribute(columnName, attributeName, pk, search));
        }

        public void parse( String infoString)
        {
            dynamic json = Newtonsoft.Json.Linq.JObject.Parse(infoString);

            try
            {
                this.entity = json.entity;
            }
            catch (Exception) { }
            try
            {
                this.fileName = json.file_name;
            }
            catch (Exception) { }
            try
            {
                this.package = json.package;
            }
            catch (Exception) { }
            try
            {
                this.package_entity = json.package_entity;
            }
            catch (Exception) { }
            try
            {
                this.package_tables = json.package_tables;
            }
            catch (Exception) { }
            try
            {
                this.path = json.path;
            }
            catch (Exception) { }
            try
            {
                this.table_class = json.table_class;
            }
            catch (Exception) { }
            try
            {
                this.table_constant = json.table_constant;
            }
            catch (Exception) { }

            try
            {
                foreach (dynamic ci in json.column_info)
                {
                    this.add((string)ci.column_name, (string)ci.attribute_name, (bool)ci.pk, (bool)ci.search);
                }
            }
            catch (Exception e) { Clipboard.SetText(e.ToString()); }

        }

        public string stringfity()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            JsonWriter writer = new JsonTextWriter(sw);

            writer.Formatting = Newtonsoft.Json.Formatting.Indented;
            writer.WriteStartObject();

            writer.WritePropertyName("path");
            writer.WriteValue(this.path);

            writer.WritePropertyName("file_name");
            writer.WriteValue(this.fileName);

            writer.WritePropertyName("entity");
            writer.WriteValue(this.entity);

            writer.WritePropertyName("package");
            writer.WriteValue(this.package);

            writer.WritePropertyName("package_entity");
            writer.WriteValue(this.package_entity);

            writer.WritePropertyName("package_tables");
            writer.WriteValue(this.package_tables);

            writer.WritePropertyName("table_class");
            writer.WriteValue(this.table_class);

            writer.WritePropertyName("table_constant");
            writer.WriteValue(this.table_constant);

            writer.WritePropertyName("column_info");
            writer.WriteStartArray();

            writeList(writer, this.column_info);

            writer.WriteEndArray();

            writer.WriteEnd();

            return sw.ToString();
        }

        private void writeList(JsonWriter writer, List<JooqDaoColumnAttribute> list)
        {
            foreach( JooqDaoColumnAttribute ca in list)
            {
                writer.WriteStartObject();

                writer.WritePropertyName("column_name");
                writer.WriteValue(ca.column_name);

                writer.WritePropertyName("attribute_name");
                writer.WriteValue(ca.attribute_name);

                writer.WritePropertyName("column_type");
                writer.WriteValue(ca.column_type);

                writer.WritePropertyName("pk");
                writer.WriteValue(ca.pk);

                writer.WritePropertyName("search");
                writer.WriteValue(ca.search);

                writer.WritePropertyName("fk");
                writer.WriteValue(ca.fk);

                writer.WriteEnd();
            }
        }
    }
    public class JooqDaoColumnAttribute
    {
        public string column_name;
        public string attribute_name;
        public bool pk;
        public bool search;
        public string column_type;
        public bool fk = false;

        public JooqDaoColumnAttribute(string columnName, string attributeName)
        {
            this.column_name = columnName;
            this.attribute_name = attributeName;
            this.pk = false;
            this.search = false;
        }
        public JooqDaoColumnAttribute(string columnName, string attributeName, bool pk, bool search)
        {
            this.column_name = columnName;
            this.attribute_name = attributeName;
            this.pk = pk;
            this.search = search;
        }
    }
}
