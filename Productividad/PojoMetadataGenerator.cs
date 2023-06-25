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
using EAUtils.entity;

namespace Productividad
{
    public class PojoMetadataGenerator
    {
        Element element;
        Framework frameworkInstance;
        EAUtils.EAUtils eaUtils;
        string path;
        string fileName;
        Dictionary<int, string> idColumnaSourceGuid = new Dictionary<int, string>();

        public void doIt(Element element, Framework frameworkInstance, EAUtils.EAUtils eaUtils)
        {
            this.element = element;
            this.frameworkInstance = frameworkInstance;
            this.eaUtils = eaUtils;

            PojoInfo pojoInfo = configure();

            onAceptConfigure(pojoInfo);

        }

        private PojoInfo configure()
        {
            string tgvName         = frameworkInstance.languageForCodeGeneration.name;
            string infoString      = eaUtils.taggedValuesUtils.get(element, tgvName, "").asString();

            // Obtiene la última configuración actualizada según las columnas actuales de la tabla.
            // Agrega a la pantalla los datos que estaban previamente configurados.
            PojoInfo  pojoInfo    = fromLastconsolidatedPojoInfo(infoString);

            // sino hay configuración previa.
            if (pojoInfo == null)
            {
                pojoInfo = new PojoInfo();
            }

            // obtiene las columnas "nuevas" y las agrega al final.
            List<string> names = getFilteredActualColummns(pojoInfo);

            return pojoInfo;
        }


        private PojoInfo fromLastconsolidatedPojoInfo( string infoString)
        {
            PojoInfo pojoInfo = null;

            if( infoString != "")
            {
                pojoInfo = new PojoInfo();
                pojoInfo.parse(infoString, this.eaUtils);

                this.path     = pojoInfo.path;
                this.fileName = pojoInfo.fileName;

                FullAttribute fullAttr;
                bool isValid = true;
                string sourceGuid;
                EA.Attribute currentColumn = null;

                for (int i = pojoInfo.attributes.Count - 1; i >= 0; i--)
                {
                    isValid = false;
                    fullAttr = pojoInfo.attributes[i];

                    foreach (EA.Attribute column in element.Attributes)
                    {
                        currentColumn = column;

                        // esto indica que la columna proviene de un atributo de una clase
                        if ( (sourceGuid = this.getSourceGuid(column)) != null)
                        {
                            if (fullAttr.eaAttribute.AttributeGUID == sourceGuid)
                            {
                                isValid = true;
                                break;
                            }
                        }
                        else
                        {
                            // en este caso la columna se definió en el mismo elemento tabla.
                            if ( fullAttr.eaAttribute.AttributeGUID == column.AttributeGUID)
                            {
                                isValid = true;
                                break;
                            }
                        }
                    }
                    if( ! isValid)
                    {
                        pojoInfo.attributes.RemoveAt(i);
                    }
                    else
                    {
                        // actualizar según el valor etiquetado del atributo 
                        pojoInfo.attributes[i] = updateFromAttibuteOfDomainClass(fullAttr, currentColumn);
                    }
                }
            }
            return pojoInfo;
        }

        private string getSourceGuid( EA.Attribute column)
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
        private FullAttribute updateFromAttibuteOfDomainClass(FullAttribute fullAttr, EA.Attribute column)
        {
            FullAttribute toReturn = null;

            string guid = getSourceGuid(column);

            if( guid != null)
            {
                EA.Attribute source = this.eaUtils.repository.GetAttributeByGuid(guid);
                if (source != null)
                {
                    toReturn = new FullAttribute(source, this.eaUtils);
                    toReturn.search = this.eaUtils.taggedValuesUtils.getSearch(column, "false").asBoolean();
                    toReturn.realPk = this.eaUtils.taggedValuesUtils.getRealPrimaryKey(column, "false").asBoolean();
                }
                else
                {
                    // la columna perdió su relación con el origen.
                    this.eaUtils.taggedValuesUtils.delete(column, "source.guid");
                    toReturn = fullAttr;
                    toReturn.search = this.eaUtils.taggedValuesUtils.getSearch(column, "false").asBoolean();
                    toReturn.realPk = this.eaUtils.taggedValuesUtils.getRealPrimaryKey(column, "false").asBoolean();
                }
            }
            else
            {
                if(fullAttr.eaAttribute == null)
                {
                    toReturn = new FullAttribute(column, this.eaUtils);
                    toReturn.name = StringUtils.toCamel(toReturn.name);
                    toReturn.search = this.eaUtils.taggedValuesUtils.getSearch(column, "false").asBoolean();
                    toReturn.realPk = this.eaUtils.taggedValuesUtils.getRealPrimaryKey(column, "false").asBoolean();
                }
                else
                {
                    toReturn = fullAttr;
                    toReturn.search = this.eaUtils.taggedValuesUtils.getSearch(column, "false").asBoolean();
                    toReturn.realPk = this.eaUtils.taggedValuesUtils.getRealPrimaryKey(column, "false").asBoolean();
                }
            }
            return toReturn;
        }

        private List<string> getFilteredActualColummns(PojoInfo pojoInfo)
        {
            List<string> names = new List<string>();
            string[]     filter       = null;
            string       notes        = frameworkInstance.languageForCodeGeneration.element.Notes;

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
                    FullAttribute currentAttibute;
                    if( (currentAttibute = previousconsolidatedPojoInfo(attr, pojoInfo)) != null)
                    {
                        pojoInfo.update(updateFromAttibuteOfDomainClass(currentAttibute, attr));
                    }
                    else
                    {
                        // si no está buscamos el atributo de la clase de dominio
                        pojoInfo.attributes.Add(updateFromAttibuteOfDomainClass(new FullAttribute(), attr));
                    }
                }
            }
            return names;
        }

        private FullAttribute previousconsolidatedPojoInfo( EA.Attribute column, PojoInfo pojoInfo)
        {
            FullAttribute currentAttribute = null;

            string guid = this.getSourceGuid(column);
            if( guid == null)
            {
                guid = column.AttributeGUID;
            }

            foreach (FullAttribute colAttr in pojoInfo.attributes)
            {
                if( colAttr.eaAttribute.AttributeGUID == guid)
                {
                    currentAttribute = colAttr;
                    break;
                }
            }
            return currentAttribute;
        }

        public void onAceptConfigure(PojoInfo info)
        {
            // tomo los datos y los guardo en el valor etiquetado.
            bool saveFile = false;
            // pregunto si se quiere al porta papeles o al disco
            DialogResult dialogResult = MessageBox.Show("Guardar en el disco", "Opciones de generación", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                saveFileDialog1.InitialDirectory = path != null ? path : @"C:\";

                saveFileDialog1.FileName = fileName == null ? info.entity : fileName;

                saveFileDialog1.Title = "Guardar";

                saveFileDialog1.CheckFileExists = false;

                saveFileDialog1.CheckPathExists = true;

                saveFileDialog1.DefaultExt = "props";

                saveFileDialog1.Filter = "Archivo de configuración (*.props)|*.props";

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

            eaUtils.taggedValuesUtils.set(element, frameworkInstance.languageForCodeGeneration.name, info.stringfity());

            if( generate())
            {
                if( ! saveFile)
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
                    catch ( Exception e )
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

            string infoString      = eaUtils.taggedValuesUtils.get(element, frameworkInstance.languageForCodeGeneration.name, "").asString();
            string extraInfoString = eaUtils.taggedValuesUtils.get(element, frameworkInstance.languageForCodeGeneration.name +"_extension", "{}").asString();

            if ( infoString == "")
            {

            }
            else
            {
                var data = new JsonParser(jsEngine).Parse(infoString);

                var dataExtra = new JsonParser(jsEngine).Parse(extraInfoString);

                jsEngine.SetValue("data", data);
                jsEngine.SetValue("dataExtra", dataExtra);

                try
                {
                    jsEngine.SetValue(frameworkInstance.languageForCodeGeneration.name, data);
                    jsEngine.SetValue(frameworkInstance.languageForCodeGeneration.name + "_extension", dataExtra);

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
                catch ( Exception e)
                {
                    Clipboard.SetText(e.ToString());
                }
            }
            return generated;
        }
    }

    public class PojoInfo
    {
        public string fileName = null;
        public string path = null;
        public string package;
        public string entity;
        public List<AssociatedEntity> entities = new List<AssociatedEntity>();
        public List<CollectionInfo> collections = new List<CollectionInfo>();
        public List<FullAttribute> attributes = new List<FullAttribute>();

        public void setElement( Element element)
        {
            this.entity = EAUtils.StringUtils.toPascal(element.Name);
        }

        public void add(EA.Attribute attribute, EAUtils.EAUtils eaUtils)
        {
            attributes.Add(new FullAttribute(attribute, eaUtils));
        }

        public void parse(String infoString, EAUtils.EAUtils eaUtils)
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
                this.path = json.path;
            }
            catch (Exception) { }

            try
            {
                foreach (dynamic ci in json.attributes)
                {
                    attributes.Add(new FullAttribute().parse(ci, eaUtils));
                }
            }
            catch (Exception e) { Clipboard.SetText(e.ToString()); }
        }

        public string stringfity()
        {
            List<string> uniqueImports = new List<string>();

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

            // init lista atributos
            writer.WritePropertyName("attributes");
            writer.WriteStartArray();

            writeList(writer, this.attributes);

            writer.WriteEndArray();
            // end lista atributos

            // init lista atributos pk's
            writer.WritePropertyName("realPrimaryKeys");
            writer.WriteStartArray();

            writeRealPkList(writer, this.attributes);

            writer.WriteEndArray();
            // end lista atributos pk's

            // init lista imports
            writer.WritePropertyName("imports");
            writer.WriteStartArray();

            writeImports(writer, this.attributes, uniqueImports);

            writer.WriteEndArray();
            // end lista imports

            // entidades asociadas
            writer.WritePropertyName("entities");
            writer.WriteStartArray();

            writeEntities(writer, this.entities);

            writer.WriteEndArray();

            // init lista collections
            writer.WritePropertyName("collections");
            writer.WriteStartArray();

            writeCollections(writer, this.collections);

            writer.WriteEndArray();
            // end lista collections

            writer.WriteEnd();

            return sw.ToString();
        }

        private void writeList(JsonWriter writer, List<FullAttribute> list)
        {
            foreach(FullAttribute ca in list)
            {
                ca.strinfity(writer);
            }
        }

        private void writeRealPkList(JsonWriter writer, List<FullAttribute> list)
        {
            foreach (FullAttribute ca in list)
            {
                if(ca.realPk != null && ca.realPk == true)
                {
                    ca.strinfity(writer);
                }
            }
        }
        private void writeImports(JsonWriter writer, List<FullAttribute> list, List<string> uniqueImports)
        {
            foreach (FullAttribute ca in list)
            {
                if (ca.package != null && ! uniqueImports.Contains(ca.package))
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("import");
                    writer.WriteValue(ca.package);
                    writer.WriteEnd();

                    uniqueImports.Add(ca.package);
                }
            }
        }
        private void writeCollections(JsonWriter writer, List<CollectionInfo> list)
        {
            foreach (CollectionInfo ca in list)
            {
                writer.WriteStartObject();

                writer.WritePropertyName("name");
                writer.WriteValue(ca.name);

                writer.WritePropertyName("Name");
                writer.WriteValue( EAUtils.StringUtils.toPascal(ca.name));

                writer.WritePropertyName("type");
                writer.WriteValue(ca.type);

                writer.WritePropertyName("Type");
                writer.WriteValue(EAUtils.StringUtils.toPascal(ca.type));

                writer.WriteEnd();

            }
        }
        private void writeEntities(JsonWriter writer, List<AssociatedEntity> list)
        {
            foreach (AssociatedEntity ca in list)
            {
                writer.WriteStartObject();

                writer.WritePropertyName("name");
                writer.WriteValue(ca.name);

                writer.WritePropertyName("Name");
                writer.WriteValue(EAUtils.StringUtils.toPascal(ca.name));

                writer.WriteEnd();
            }
        }

        internal void update(FullAttribute fullAttribute)
        {
            FullAttribute current;
            for( short i = 0; i < this.attributes.Count; i++)
            {
                current = this.attributes[i];
                if( current.eaAttribute.AttributeGUID == fullAttribute.eaAttribute.AttributeGUID)
                {
                    this.attributes[i] = fullAttribute;
                    break;
                }
            }
        }
    }

    public class AssociatedEntity
    {
        public string name;

        public AssociatedEntity(string name)
        {
            this.name = name;
        }
    }
    public class CollectionInfo
    {
        /// <summary>
        /// El nombre de la colección sale de:
        ///     Buscar la relación de realización que tiene origen en la tabla y destino en la clase del dominio,
        ///     desde esa clase de dominio se buscan las relaciones de asociación (con cardinalidad que implique colección) 
        ///     que existe desde esa clase a las demás 
        ///     por cada una de esas clases se tienen que buscar las tablas que las realizan y de esas tomar sólo la que está
        ///     relacionada con la tabla que estamos analizando.
        /// </summary>
        public string name;

        /// <summary>
        /// El type es el nombre de la clase (en camelCase) que surge del nombre de la tabla, la tabla en cuestión es la que se encuentra según
        /// la regla para encontrar esta tabla se describe en el atributo @see name.
        /// </summary>
        public string type;

        public CollectionInfo(string name, string type)
        {
            this.name = name;
            this.type = type;
        }
    }
    
}
