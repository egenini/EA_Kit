using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.Diagnostics;

namespace Productividad
{
    class PojoGenerator
    {
        Element element;
        Framework frameworkInstance;
        EAUtils.EAUtils eaUtils;
        Dictionary<int, string> idColumnaSourceGuid = new Dictionary<int, string>();
        string language;

        public void doIt(Element element, Framework frameworkInstance, EAUtils.EAUtils eaUtils)
        {
            this.element = element;
            this.frameworkInstance = frameworkInstance;
            this.eaUtils = eaUtils;

            try
            {
                this.language = this.frameworkInstance.languageForCodeGeneration.name.Substring(0, this.frameworkInstance.languageForCodeGeneration.name.IndexOf("-"));

                PojoInfo pojoInfo = configure();

                onAceptConfigure(pojoInfo);
            }
            catch (Exception e)
            {
                Clipboard.SetText(e.ToString());
            }
        }

        private PojoInfo configure()
        {
            string tgvName = frameworkInstance.languageForCodeGeneration.name;

            string infoString = eaUtils.taggedValuesUtils.get(element, tgvName, "").asString();

            PojoInfo pojoInfo = new PojoInfo();

            if (infoString != "")
            {
                pojoInfo.parse(infoString, this.eaUtils);

                // descartamos los detalles de los atributos porque se van a volver a buscar para tener
                // todos los datos actualizados.
                pojoInfo.attributes.Clear();
            }

            pojoInfo.entity = StringUtils.toPascal(this.element.Name);

            pojoInfo.fileName = pojoInfo.entity +"Base";

            builCollections(pojoInfo);

            // obtiene las columnas "nuevas" y las agrega al final.
            buildFromColummns(pojoInfo);

            return pojoInfo;
        }

        /// <summary>
        /// Buscamos la relación de realización a la clase del dominio, desde esta clase buscamos las relaciones de asociación
        /// con cardinalidad que implique una colección, en ese caso tomanos el nombre del rol de la relación y para saber
        /// el nombre de la clase se de deben buscar las relaciones de realización que pueda tener y la tabla con la cual nos 
        /// quedamos es aquella que coincida con la relación de asociación que la tabla desde la cual arrancamos realice a la clase
        /// relacionada con la clase que realiza a la tabla analizada.
        /// 
        /// </summary>
        /// <param name="pojoInfo"></param>
        private void builCollections(PojoInfo pojoInfo)
        {
            //List<ConnectorUtils.ElementConnectorInfo> asociacionesInfo = null;
            List<ConnectorUtils.ElementConnectorInfo> tablasAsociadasInfo = null;

            // buscamos las relaciones de realización que esta tabla es destino.
            List<ConnectorUtils.ElementConnectorInfo> realizacionesInfo = eaUtils.connectorUtils.get(this.element, EAUtils.ConnectorUtils.CONNECTOR__REALISATION, null, "Class", null, false, null);

            // no debería ser más de 1, peeeeero.
            if (realizacionesInfo.Count != 0)
            {
                // buscamos las tablas con relación de asociación que esta tabla tiene como destino
                tablasAsociadasInfo = eaUtils.connectorUtils.get(this.element, EAUtils.ConnectorUtils.CONNECTOR__ASSOCIATION, null, "Class", "table", true, null);

                Debug.WriteLine("Cantidad tablas asociadas "+ tablasAsociadasInfo.Count);

                foreach (ConnectorUtils.ElementConnectorInfo realizacionInfo in realizacionesInfo)
                {
                    // esta es la clase que es realizada por la tabla
                    // buscamos las asociaciones que se originan en esta clase.
                    // también se deben buscar en el árbol de herencia 

                    Debug.WriteLine("Analizando realización " + realizacionInfo.element.Name);

                    analizarAsociaciones(pojoInfo, realizacionInfo, tablasAsociadasInfo);

                    analizarArbolHerencia(pojoInfo, realizacionInfo, tablasAsociadasInfo);

                    /*
                    asociacionesInfo = eaUtils.connectorUtils.get(realizacionInfo.element, EAUtils.ConnectorUtils.CONNECTOR__ASSOCIATION, null, "Class", null, false, null);

                    if (asociacionesInfo.Count != 0)
                    {
                        foreach (ConnectorUtils.ElementConnectorInfo asociacionInfo in asociacionesInfo)
                        {
                            this.analizarRelacion(pojoInfo, asociacionInfo, tablasAsociadasInfo);
                        }
                    }
                    */
                }
            }
        }
        private void analizarArbolHerencia(PojoInfo pojoInfo, ConnectorUtils.ElementConnectorInfo info, List<ConnectorUtils.ElementConnectorInfo> tablasAsociadasInfo)
        {
            List<ConnectorUtils.ElementConnectorInfo> parentsInfo = eaUtils.connectorUtils.get(info.element, EAUtils.ConnectorUtils.CONNECTOR__GENERALIZATION, null, "Class", null, false, null);

            if (parentsInfo.Count != 0)
            {
                foreach (ConnectorUtils.ElementConnectorInfo asociacionInfo in parentsInfo)
                {
                    this.analizarAsociaciones(pojoInfo, asociacionInfo, tablasAsociadasInfo);
                }
            }

        }
        private void analizarAsociaciones(PojoInfo pojoInfo, ConnectorUtils.ElementConnectorInfo info, List<ConnectorUtils.ElementConnectorInfo> tablasAsociadasInfo)
        {
            List<ConnectorUtils.ElementConnectorInfo> asociacionesInfo = eaUtils.connectorUtils.get(info.element, EAUtils.ConnectorUtils.CONNECTOR__ASSOCIATION, null, "Class", null, false, null);

            Debug.WriteLine("Asociaciones encontradas "+ asociacionesInfo.Count);

            if (asociacionesInfo.Count != 0)
            {
                foreach (ConnectorUtils.ElementConnectorInfo asociacionInfo in asociacionesInfo)
                {
                    this.analizarRelacion(pojoInfo, asociacionInfo, tablasAsociadasInfo);
                }
            }
        }
        private void analizarRelacion(PojoInfo pojoInfo, ConnectorUtils.ElementConnectorInfo relacionInfo, List<ConnectorUtils.ElementConnectorInfo> tablasAsociadasInfo)
        {
            ConnectorUtils.Cardinality cardinality;
            List<ConnectorUtils.ElementConnectorInfo> realizadasPorLasAsociacionesInfo = null;

            cardinality = eaUtils.connectorUtils.getCardinality(relacionInfo.connector.SupplierEnd);

            // si esta clase (la asociada), es realizada por una tabla que tiene una relación de asociación como destino
            // de la tabla que estamos analizando.

            realizadasPorLasAsociacionesInfo = eaUtils.connectorUtils.get(relacionInfo.element, EAUtils.ConnectorUtils.CONNECTOR__REALISATION, null, "Class", "table", true, null);

            Debug.WriteLine("Realizadas por la asociacion " + relacionInfo.element.Name + " " + realizadasPorLasAsociacionesInfo.Count);

            if (realizadasPorLasAsociacionesInfo.Count != 0)
            {
                foreach (ConnectorUtils.ElementConnectorInfo realizadaPorLasAsociacionInfo in realizadasPorLasAsociacionesInfo)
                {
                    // vamos a comparar este con las tablas asociadas.
                    if (tablasAsociadasInfo.Count != 0)
                    {
                        foreach (ConnectorUtils.ElementConnectorInfo tablaAsociadaInfo in tablasAsociadasInfo)
                        {
                            Console.WriteLine("Analizando "+ tablaAsociadaInfo.element.Name );

                            if (this.element.ElementID != tablaAsociadaInfo.element.ElementID && tablaAsociadaInfo.element.ElementID == realizadaPorLasAsociacionInfo.element.ElementID)
                            {

                                // si la cardinalidad supone una colección.
                                if (cardinality.isCollection())
                                {
                                    Debug.WriteLine("Es una colección");

                                    pojoInfo.collections.Add(new CollectionInfo(relacionInfo.connector.SupplierEnd.Role, EAUtils.StringUtils.toCamel(tablaAsociadaInfo.element.Name)));
                                    break;
                                }
                                else
                                {
                                    Debug.WriteLine("No es una colección");

                                    pojoInfo.entities.Add(new AssociatedEntity(EAUtils.StringUtils.toCamel(tablaAsociadaInfo.element.Name)));
                                    break;
                                }
                            }
                        }
                    }
                }
            }
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
        // 
        // 

        /// <summary>
        /// "Refresca" la definición, ojo que podría diferir con la definición de la tabla!
        /// 
        /// TODO : Ver que hacer en caso de conflicto.
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private FullAttribute buildFromColumn( EA.Attribute column )
        {
            FullAttribute toReturn = null;
            EA.Attribute source = null;
            Element parentSource = null;

            // cuando se crean las columnas estas pueden provenir de otras columnas o de atributos de clases
            // este guid puede referir a cualquiera de ellas
            string guid = this.getSourceGuid(column);

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
                    this.eaUtils.taggedValuesUtils.delete(column, "source.guid");
                }
            }

            // determinamos si vamos a usar la columna o el "origen" (atributo de una clase u otra columna).
            if( source != null && parentSource != null && parentSource.Stereotype != "table" )
            {
                toReturn = new FullAttribute(source, this.eaUtils);

            }
            else
            {
                toReturn = new FullAttribute(column, this.eaUtils);

                // Buscamos el tipo de dato que le correponde a un atributo de clase.
                DataTypeInfo dataTypeInfo = eaUtils.dataTypeUtils.getFrom(this.element.Gentype, column.Type, language);

                toReturn.dataType = dataTypeInfo.name;

                toReturn.name = StringUtils.toCamel(toReturn.name);
            }

            toReturn.search = this.eaUtils.taggedValuesUtils.getSearch(column, "false").asBoolean();
            toReturn.realPk = this.eaUtils.taggedValuesUtils.getRealPrimaryKey(column, "false").asBoolean();

            toReturn.dbDataType       = column.Type;
            toReturn.dataTypeAsObject = toReturn.dataType;

            try
            {
                toReturn.package = frameworkInstance.languageForCodeGeneration.objectNamespace[toReturn.dataTypeAsObject];
            }
            catch (Exception) { }
            try
            {
                if( frameworkInstance.languageForCodeGeneration.primitive2Object.Count != 0)
                {
                    string newValue = frameworkInstance.languageForCodeGeneration.primitive2Object[toReturn.dataType];

                    if (newValue != null)
                    {
                        toReturn.dataTypeAsObject = newValue;
                    }
                }
            }
            catch (Exception) {
            }
            return toReturn;
        }

        private List<string> buildFromColummns(PojoInfo pojoInfo)
        {
            List<string> names = new List<string>();
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
                    pojoInfo.attributes.Add( buildFromColumn( attr ) );
                }
            }
            return names;
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

                saveFileDialog1.InitialDirectory = @"C:\";

                saveFileDialog1.FileName = info.fileName;

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
                        // Encoding.GetEncoding("iso-8859-1")
                        System.IO.File.WriteAllText(info.path + info.fileName, Clipboard.GetText(), Encoding.UTF8);

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

            jsEngine.Execute(frameworkInstance.lodashJs);
            jsEngine.Execute(frameworkInstance.momentJs);
            jsEngine.Execute(frameworkInstance.handlebarsJs);
            jsEngine.Execute(frameworkInstance.dashbarsJs);

            string infoString = eaUtils.taggedValuesUtils.get(element, frameworkInstance.languageForCodeGeneration.name, "").asString();
            string extraInfoString = eaUtils.taggedValuesUtils.get(element, frameworkInstance.languageForCodeGeneration.name + "_extension", "{}").asString();
            string jooqInfoString = eaUtils.taggedValuesUtils.get(element, "Java-JOOQ", "{}").asString();

            if (infoString == "")
            {

            }
            else
            {
                // armamos un 1 sólo objeto con todo
                infoString = infoString.Substring(0, infoString.Length - 1)
                    + ",\"persistenceInfo\":" + jooqInfoString
                    + ",\"extraInfo\":" + extraInfoString
                    + "}";

                var data = new JsonParser(jsEngine).Parse(infoString);

                jsEngine.SetValue("data", data);

                try
                {
                    //jsEngine.SetValue(frameworkInstance.languageForCodeGeneration.name, data);

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
}
