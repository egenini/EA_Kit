using EA;
using EAUtils;
using EAUtils.entity;
using Persistencia.entity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UIResources;
using static EAUtils.ConnectorUtils;

namespace Persistencia.frw
{
    class Generator
    {
        Element element;
        Framework frameworkInstance;
        EAUtils.EAUtils eaUtils;
        Dictionary<int, string> idColumnaSourceGuid = new Dictionary<int, string>();
        Dictionary<string, FkInfo> fkTableFromColumnName = new Dictionary<string, FkInfo>();
        Dictionary<int, Dictionary<string, string>> namespaces = new Dictionary<int, Dictionary<string, string>>();
        List<ConnectorUtils.ElementConnectorInfo> realizacionesInfo = null;
        RelationshipAnalizer relationshipAnalizer = new RelationshipAnalizer();

        public Generator(EA.Element element, Framework frameworkInstance, EAUtils.EAUtils eaUtils)
        {
            this.eaUtils = eaUtils;
            this.frameworkInstance = frameworkInstance;
            this.element = element;

            relationshipAnalizer.generator = this;
            relationshipAnalizer.eaUtils = this.eaUtils;
            relationshipAnalizer.tablaEnAnalisis = this.element;

            this.realizacionesInfo = this.classRealization(this.element);
        }

        public bool generarArtefacto()
        {
            bool canDo = false;

            string infoString = this.frameworkInstance.getFrwJsonFromTaggedValue(this.element);

            if (infoString == "{}" || infoString == "")
            {
                generarJSon();
            }

            if (infoString != "{}" && infoString != "")
            {
                canDo = this.frameworkInstance.generate(infoString, this.element);
            }
            else
            {
                Alert.Error("No se encuentra el JSON");
            }

            return canDo;
        }

        public void generarJSon()
        {
            Info pojoInfo = new Info();

            pojoInfo.entity = StringUtils.toPascal(this.element.Name);
            pojoInfo.tableName = this.element.Name;
            pojoInfo.notes = this.eaUtils.notes2Txt(this.element);
            pojoInfo.tableDbVersion = this.eaUtils.taggedValuesUtils.get(this.element, "DBVersion", "").asString();
            pojoInfo.tablespace = this.eaUtils.taggedValuesUtils.get(this.element, "Tablespace", "").asString();
            pojoInfo.tableOwner = this.eaUtils.taggedValuesUtils.get(this.element, "Owner", "").asString();
            pojoInfo.alias = this.element.Alias;
            pojoInfo.pluralAlias = this.eaUtils.taggedValuesUtils.getPluralAlias(this.element, "").asString();
            pojoInfo.pluralName = this.eaUtils.taggedValuesUtils.getPluralName(this.element, "").asString();

            if(pojoInfo.alias == "")
            {
                this.element.Alias = StringUtils.toLabel(this.element.Name);
                this.element.Update();
                pojoInfo.alias = this.element.Alias;
            }
            // para determinar si la tabla es producto de una relación de muchos a muchos contamos las fks que sean many2One,
            // si hay más de 1 entonces asumimos que se trata de una many2many.
            int fksMany2One = 0;
            FkInfo fkinfo;

            foreach (Method method in element.Methods)
            {
                if (method.Stereotype == "FK")
                {
                    try
                    {
                        fkinfo = fk(method.Name);
                        fkTableFromColumnName.Add(method.Parameters.GetAt(0).Name, fkinfo);

                        fksMany2One += (fkinfo.many2one() ? 1 : 0);
                    }
                    catch (Exception) { }
                }
            }

            // esto no se determina más de esta forma.
            //pojoInfo.many2many = fksMany2One > 1;

            lookingArtifactsNamespace(pojoInfo, this.element);

            this.relationshipAnalizer.analizar(pojoInfo);

            // obtiene las columnas "nuevas" y las agrega al final.
            buildFromColummns(pojoInfo);

            this.frameworkInstance.setFrwJson(this.element, pojoInfo.stringfity());

            this.eaUtils.printOut("JSON finalizado");
        }
        
        public void lookingArtifactsNamespace(Info pojoInfo, Element element)
        {
            List<ConnectorUtils.ElementConnectorInfo> ecList = eaUtils.connectorUtils.get(element, 
                EAUtils.ConnectorUtils.CONNECTOR__DEPENDENCY, null, null, "Namespace", false, null);

            this.eaUtils.printOut("Analizando namespace para "+ element.Name);

            Namespace @namespace;
            foreach (ConnectorUtils.ElementConnectorInfo ecInfo in ecList)
            {
                // restringir los namespace a los artefactos del lenguaje a generar.

                this.eaUtils.printOut("Analizando namespace desde " + ecInfo.element.Name);

                if(ecInfo.element.Name.ToUpper() != "ROOT" 
                    && ! this.frameworkInstance.choosed.choosed.artifactsByName.ContainsKey(ecInfo.element.Alias))
                {
                    this.eaUtils.printOut("___Descartando namespace");
                    continue;
                }

                @namespace = new Namespace(ecInfo.element.Alias);

                this.eaUtils.printOut("___Agregando namespace "+ ecInfo.element.Alias);

                foreach ( EA.Attribute attr in ecInfo.element.Attributes)
                {
                    this.eaUtils.printOut( "___ ___ analizando atributo " + attr.Name );

                    if (attr.Default != "" && ! @namespace.attributes.ContainsKey(attr.Name))
                    {
                        @namespace.attributes.Add(attr.Name, attr.Default);

                        this.eaUtils.printOut("___ ___ ___ agregando el namespace  " + attr.Default);

                        if (attr.Alias.ToLower() == "main")
                        {
                            @namespace.isMain = true;

                            this.eaUtils.printOut("___ ___ ___ el namespace está marcado como main");
                        }
                    }
                }

                pojoInfo.add(@namespace, null);

                addFromNamespaceParent(pojoInfo, ecInfo.element);
            }
        }
        void addFromNamespaceParent(Info pojoInfo, Element element)
        {
            List<ConnectorUtils.ElementConnectorInfo> ecList = eaUtils.connectorUtils.get(element, EAUtils.ConnectorUtils.CONNECTOR__GENERALIZATION, null, null, "Namespace", false, null);

            this.eaUtils.printOut("Buscando root jerarquía desde " + element.Name);

            Namespace @namespace;
            if (ecList.Count == 0)
            {
                this.eaUtils.printOut("--- -No hay más jerarquía se analiza si " + element.Name +" es root");

                this.getRootNamespace(element, pojoInfo);
            }
            string namespaceArtifactName;
            foreach (ConnectorUtils.ElementConnectorInfo ecInfo in ecList)
            {

                this.eaUtils.printOut("--- -hay más en la jerarquía se analiza si " + ecInfo.element.Name + " es root");

                namespaceArtifactName = this.getArtifactNameFromElement(ecInfo.element, pojoInfo);

                this.eaUtils.printOut("--- se crea el namespace "+ namespaceArtifactName );

                @namespace = new Namespace(namespaceArtifactName);

                foreach (EA.Attribute attr in ecInfo.element.Attributes)
                {
                    if (attr.Default != "" && ! @namespace.attributes.ContainsKey(attr.Name))
                    {
                        this.eaUtils.printOut("--- --- se agrega el atributo "+ attr.Name +" con valor "+ attr.Default);
                        @namespace.attributes.Add(attr.Name, attr.Default);

                        if( attr.Alias.ToLower() == "main")
                        {
                            this.eaUtils.printOut("--- --- --- el atributo está marcado como main");
                            @namespace.isMain = true;
                        }
                    }
                }

                if( @namespace.attributes.Count != 0)
                {
                    pojoInfo.add(@namespace, element == null ? null : this.getArtifactNameFromElement(element, pojoInfo));
                }

                // volvemos a entrar acá mismo hasta llegar al root del árbol de jerarquía.
                addFromNamespaceParent(pojoInfo, ecInfo.element);
            }
        }
        string getArtifactNameFromElement(Element element, Info pojoInfo)
        {
            string namespaceArtifactName = "";

            namespaceArtifactName = element.Alias;
            if (namespaceArtifactName == "")
            {
                getRootNamespace(element, pojoInfo);

                namespaceArtifactName = element.Name;
            }
            return namespaceArtifactName;
        }

        void getRootNamespace(Element element, Info pojoInfo)
        {
            if(element.Stereotype == "Namespace" && element.Name.ToLower() == "root")
            {
                foreach( EA.Attribute attr in element.Attributes)
                {
                    if( attr.Name == "namespace" && attr.Default != "")
                    {
                        this.eaUtils.printOut("--- ---Se obtiene root namespace desde " + element.Name );
                        pojoInfo.rootNamespace = attr.Default;
                    }
                }
            }
        }
        /// <summary>
        ///
        /// Una mierda pero este código no se puede usar, EA recorta el nombre del rol por lo tanto el nombre del método no 
        /// coincide con el del rol, este era la manera de encontrar la tabla relacionada.
        /// Se cambia por otro modo que consiste en recorrer los métodos pero usando el parámetro y la coincidencia de este con la primer
        /// parte del nombre del conector que sigue la forma: nombreColumna = columnaDeLaOtraTabla por ej: pais_id = id
        /// </summary>
        /// <param name="fkName"></param>
        /// <returns></returns>
        private FkInfo fk( string fkName )
        {
            FkInfo fkInfo = null;
            Element fkElement;
            string fkInformationSource;
            string @namespace = "";
            FkEntity fkEntity = null;
            Info npInfo = new Info();

            List <ConnectorUtils.ElementConnectorInfo> ecList = eaUtils.connectorUtils.get(element, EAUtils.ConnectorUtils.CONNECTOR__ASSOCIATION, "FK", "Class", "table", false, null);

            foreach (ConnectorUtils.ElementConnectorInfo ecInfo in ecList)
            {
                fkInformationSource = ecInfo.connector.ForeignKeyInformation.Substring(4, ecInfo.connector.ForeignKeyInformation.IndexOf(":")-4);

                if (fkInformationSource == fkName)
                {
                    fkInfo = new FkInfo();
                    fkElement = this.eaUtils.repository.GetElementByID(ecInfo.connector.SupplierID);
                    fkInfo.tableName = fkElement.Name;
                    fkInfo.tableAlias = fkElement.Alias;
                    fkInfo.pluralName = this.eaUtils.taggedValuesUtils.get(fkElement, TaggedValuesUtils.EN__NAME_PLURAL, "").asString();
                    fkInfo.pluralAlias = this.eaUtils.taggedValuesUtils.get(fkElement, TaggedValuesUtils.EN__ALIAS_PLURAL, "").asString();
                    fkInfo.tableGUID = fkElement.ElementGUID;
                    fkInfo.cardinalitySource = this.eaUtils.connectorUtils.getCardinality(ecInfo.connector.ClientEnd);
                    fkInfo.cardinalityTarget = this.eaUtils.connectorUtils.getCardinality(ecInfo.connector.SupplierEnd);

                    // este elemento es la tabla relacionada vamos a buscar la clase para agregar
                    lookingArtifactsNamespace(npInfo, ecInfo.element);
                    //@namespace = this.loadNamespace(ecInfo.element);

                    @namespace = npInfo.getFullNamespace();

                    if (@namespace != null && @namespace != "")
                    {
                        fkEntity                      = new FkEntity();
                        fkEntity.entity               = ecInfo.element;
                        fkEntity.@namespace           = @namespace;

                        fkEntity.namespacesByArtifact = npInfo.getNamespacesByArtifact();

                        List<ConnectorUtils.ElementConnectorInfo> realizationList = this.classRealization(ecInfo.element);

                        if (realizationList.Count != 0)
                        {
                            fkEntity.entity      = realizationList[0].element;
                            fkEntity.pluralAlias = this.eaUtils.taggedValuesUtils.getPluralAlias(fkEntity.entity, "").asString();
                            fkEntity.pluralName  = this.eaUtils.taggedValuesUtils.getPluralName(fkEntity.entity, "").asString();

                            if (fkEntity.relationName == "")
                            {
                                realizationList[0].connector.SupplierEnd.Role = StringUtils.toCamel(fkEntity.entity.Name);
                                fkEntity.entity.Connectors.Refresh();
                            }

                            fkEntity.relationName = realizationList[0].connector.SupplierEnd.Role;
                        }

                        fkInfo.entity = fkEntity;
                    }

                    break;
                }
            }
            return fkInfo;
        }

        public List<ConnectorUtils.ElementConnectorInfo> classRealization( Element table )
        {
            // buscamos las relaciones de realización que esta tabla es origen.
            return eaUtils.connectorUtils.get(table, EAUtils.ConnectorUtils.CONNECTOR__REALISATION, null, "Class", null, false, null);
        }

        /// <summary>
        /// Se analizan las relaciones (buscando su origen desde el destino) de las clases del modelo de dominio para determinar si corresponde establecer una referencia
        /// de la clase relacionada que no es origen de la relación.
        ///
        /// </summary>
        /// <param name="pojoInfo"></param>
        private void builCollectionsInverse(Info pojoInfo)
        {
            this.eaUtils.printOut("Buscando asociaciones destino");
            this.builCollections(pojoInfo, false);
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
        private void builCollections(Info pojoInfo)
        {
            this.eaUtils.printOut("Buscando asociaciones origen");
            this.builCollections(pojoInfo, true);
        }
            
        private void builCollections(Info pojoInfo, bool searchSource)
        {
            List<ConnectorUtils.ElementConnectorInfo> tablasAsociadasInfo = null;

            // no debería ser más de 1, peeeeero, podemos tirarle una clase y después agregar columnas desde una tabla patrón. 
            if (realizacionesInfo.Count != 0)
            {
                // buscamos las tablas con relación de asociación que esta tabla tiene como destino / origen
                tablasAsociadasInfo = eaUtils.connectorUtils.get( this.element, EAUtils.ConnectorUtils.CONNECTOR__ASSOCIATION, 
                    null, null, "table", ! searchSource, null );

                this.eaUtils.printOut("Se encontraron "+ tablasAsociadasInfo.Count + " tablas asociadas como "
                    + ( ( ! searchSource ) ? "origen" : "destino"));

                if(tablasAsociadasInfo.Count != 0)
                {
                    foreach (ConnectorUtils.ElementConnectorInfo realizacionInfo in realizacionesInfo)
                    {
                        // esta es la clase que es realizada por la tabla
                        // buscamos las asociaciones que se originan en esta clase.
                        // también se deben buscar en el árbol de herencia 

                        analizarAsociaciones(pojoInfo, realizacionInfo, tablasAsociadasInfo, searchSource);

                        //analizarArbolHerencia(pojoInfo, realizacionInfo, tablasAsociadasInfo, searchSource);
                    }
                }
            }
        }
        private void analizarArbolHerencia(Info pojoInfo, ConnectorUtils.ElementConnectorInfo info, List<ConnectorUtils.ElementConnectorInfo> tablasAsociadasInfo, bool searchSource)
        {
            List<ConnectorUtils.ElementConnectorInfo> parentsInfo = eaUtils.connectorUtils.get(info.element, EAUtils.ConnectorUtils.CONNECTOR__GENERALIZATION, null, "Class", null, false, null);

            if (parentsInfo.Count != 0)
            {
                foreach (ConnectorUtils.ElementConnectorInfo asociacionInfo in parentsInfo)
                {
                    this.analizarAsociaciones(pojoInfo, asociacionInfo, tablasAsociadasInfo, searchSource);
                }
            }
        }

        private void analizarAsociaciones(Info pojoInfo, ConnectorUtils.ElementConnectorInfo info, List<ConnectorUtils.ElementConnectorInfo> tablasAsociadasInfo, bool searchSource)
        {
            // info.element es la clase que realiza a la tabla que estamos analizando.
            List<ConnectorUtils.ElementConnectorInfo> asociacionesInfo = eaUtils.connectorUtils.get(info.element, EAUtils.ConnectorUtils.CONNECTOR__ASSOCIATION, null, "Class", null, ! searchSource, null);

            if (asociacionesInfo.Count != 0)
            {
                foreach (ConnectorUtils.ElementConnectorInfo asociacionInfo in asociacionesInfo)
                {
                    this.analizarRelacion(pojoInfo, asociacionInfo, tablasAsociadasInfo, searchSource);
                }
            }
        }

        private void analizarRelacion(Info pojoInfo, ConnectorUtils.ElementConnectorInfo relacionInfo, List<ConnectorUtils.ElementConnectorInfo> tablasAsociadasInfo, bool searchSource)
        {
            ConnectorUtils.Cardinality cardinalitySupplier;
            ConnectorUtils.Cardinality cardinalityClient;
            List<ConnectorUtils.ElementConnectorInfo> realizadasPorLasAsociacionesInfo = null;
            List<ConnectorUtils.ElementConnectorInfo> candidatasManyToManyInfo         = null;
            List<ConnectorUtils.ElementConnectorInfo> confirmaManyToManyInfo           = null;

            cardinalitySupplier = eaUtils.connectorUtils.getCardinality(relacionInfo.connector.SupplierEnd);
            cardinalityClient   = eaUtils.connectorUtils.getCardinality(relacionInfo.connector.ClientEnd);

            // si esta clase (la asociada), es realizada por una tabla que tiene una relación de asociación como destino
            // de la tabla que estamos analizando.
            Info npInfo;

            this.eaUtils.printOut("Buscando tabla que realiza a "+ relacionInfo.element.Name);

            realizadasPorLasAsociacionesInfo = eaUtils.connectorUtils.get(relacionInfo.element, EAUtils.ConnectorUtils.CONNECTOR__REALISATION, null, "Class", "table", true, null);

            if (realizadasPorLasAsociacionesInfo.Count != 0)
            {
                foreach (ConnectorUtils.ElementConnectorInfo realizadaPorLasAsociacionInfo in realizadasPorLasAsociacionesInfo)
                {
                    // vamos a comparar este con las tablas asociadas.
                    if (tablasAsociadasInfo.Count != 0)
                    {
                        foreach (ConnectorUtils.ElementConnectorInfo tablaAsociadaInfo in tablasAsociadasInfo)
                        {
                            // caso típico de una foreing key
                            if ( 
                                this.element.ElementID != tablaAsociadaInfo.element.ElementID 
                                && 
                                tablaAsociadaInfo.element.ElementID == realizadaPorLasAsociacionInfo.element.ElementID
                                )
                            {
                                FkEntity fkEntity = new FkEntity();

                                npInfo = new Info();

                                lookingArtifactsNamespace(npInfo, tablaAsociadaInfo.element);

                                fkEntity.@namespace           = npInfo.getFullNamespace();
                                fkEntity.namespacesByArtifact = npInfo.getNamespacesByArtifact();

                                List<ConnectorUtils.ElementConnectorInfo> realizationList = this.classRealization(tablaAsociadaInfo.element);

                                if (realizationList.Count != 0)
                                {
                                    fkEntity.entity      = realizationList[0].element;
                                    fkEntity.pluralAlias = this.eaUtils.taggedValuesUtils.getPluralAlias(realizationList[0].element, "").asString();
                                    fkEntity.pluralName  = this.eaUtils.taggedValuesUtils.getPluralName(realizationList[0].element, "").asString();
                                }

                                if ( searchSource )
                                {
                                    if(relacionInfo.connector.SupplierEnd.Role == "")
                                    {
                                        if (cardinalitySupplier.isCollection()) {

                                            relacionInfo.connector.SupplierEnd.Role = StringUtils.toCamel(fkEntity.pluralName);
                                        }
                                        else
                                        {
                                            relacionInfo.connector.SupplierEnd.Role = StringUtils.toCamel(fkEntity.entity.Name);

                                        }
                                        relacionInfo.connector.SupplierEnd.Update();
                                        relacionInfo.element.Connectors.Refresh();
                                    }

                                    fkEntity.relationName = relacionInfo.connector.SupplierEnd.Role;

                                    break;
                                }
                                else
                                {
                                    // inverse
                                    // analizamos la dirección, si esta no apunta a esta clase entonces la ignoramos.
                                    
                                    if(relacionInfo.connector.Direction == EAUtils.ConnectorUtils.DIRECTION__BI_DIRECTIONAL 
                                        || relacionInfo.connector.Direction == EAUtils.ConnectorUtils.DIRECTION__UNSPECIFIED)
                                    {
                                        if(relacionInfo.connector.ClientEnd.Role == "")
                                        {
                                            if (cardinalityClient.isCollection())
                                            {
                                                relacionInfo.connector.ClientEnd.Role = StringUtils.toCamel(fkEntity.pluralName);
                                            }
                                            else
                                            {
                                                relacionInfo.connector.ClientEnd.Role = StringUtils.toCamel(fkEntity.entity.Name);
                                            }

                                            relacionInfo.connector.ClientEnd.Update();
                                            relacionInfo.element.Connectors.Refresh();
                                        }

                                        fkEntity.relationName = relacionInfo.connector.ClientEnd.Role;

                                        if ( ! cardinalityClient.isCollection())
                                        {
                                            pojoInfo.addEntity(fkEntity);
                                        }
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                // entonces es posible que exista una relación de muchos a muchos
                                // por ej 
                                // clases: User 0..*         -->             1..*  Rol
                                // tablas: user 1 <-- 0..* user_rol 0..* --> 1     rol
                                // si la principal es "User" podríamos estar analizando Rol, 
                                // tablaAsociadaInfo.element esta es la tabla rol y vamos a buscar las asociaciones hasta llegar
                                // a la tabla que coincida con la clase principal (User - user)
                                // si la tabla intermedia agrega datos se genera una clase desde la tabla del mismo que el resto, en
                                // caso que no agrega datos entonces la referencia se hace desde las clases relacionadas.

                                candidatasManyToManyInfo = eaUtils.connectorUtils.get( tablaAsociadaInfo.element, EAUtils.ConnectorUtils.CONNECTOR__ASSOCIATION, null, "Class", "table", true, null );
                                
                                foreach(ConnectorUtils.ElementConnectorInfo puedeSerManyToMany in candidatasManyToManyInfo)
                                {
                                    // vamos a buscar entre las tablas asociadas si alguna es la principal.
                                    confirmaManyToManyInfo = eaUtils.connectorUtils.get( puedeSerManyToMany.element, EAUtils.ConnectorUtils.CONNECTOR__ASSOCIATION, null, "Class", "table", false, null);

                                    foreach (ConnectorUtils.ElementConnectorInfo confirmaInfo in confirmaManyToManyInfo)
                                    {
                                        if( confirmaInfo.element.ElementID == this.element.ElementID )
                                        {
                                            this.eaUtils.printOut("puta many 2 many");

                                        }
                                    }
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
        private ColumnFullAttribute buildFromColumn(EA.Attribute column)
        {
            ColumnFullAttribute toReturn = null;
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
            if (source != null && parentSource != null && parentSource.Stereotype != "table")
            {
                // datos desde la clase
                toReturn = new ColumnFullAttribute(source, column, this.eaUtils);

            }
            else
            {
                toReturn = new ColumnFullAttribute(column, column,this.eaUtils);

                // Buscamos el tipo de dato que le correponde a un atributo de clase.
                DataTypeInfo dataTypeInfo = eaUtils.dataTypeUtils.getFrom(this.element.Gentype, column.Type, this.frameworkInstance.choosed.name);

                if(dataTypeInfo == null)
                {
                    Alert.Error("No se encuentra el tipo " + column.Type + " para la columna " + column.Name);
                    eaUtils.printOut("No se encuentra el tipo " + column.Type + " para la columna " + column.Name +"el lenguaje es "+ this.element.Gentype +" el elemento que lo provee es "+ this.element.Name );
                }

                toReturn.dataType = dataTypeInfo.name;

                if( fkTableFromColumnName.ContainsKey(toReturn.name))
                {
                    toReturn.fkInfo = fkTableFromColumnName[toReturn.name];
                }
            }

            toReturn.search = this.eaUtils.taggedValuesUtils.getSearch(         column, "false" ).asBoolean();
            toReturn.realPk = this.eaUtils.taggedValuesUtils.getRealPrimaryKey( column, "false" ).asBoolean();

            toReturn.columnType    = column.Type;
            toReturn.dataTypeClass = toReturn.dataType;

            try
            {
                if (frameworkInstance.choosed.primitive2Object.Count != 0)
                {
                    string newValue = frameworkInstance.choosed.primitive2Object[toReturn.dataType];

                    if (newValue != null)
                    {
                        toReturn.dataTypeClass = newValue;
                    }
                }
            }
            catch (Exception)
            {
            }
            return toReturn;
        }

        private List<string> buildFromColummns(Info pojoInfo)
        {
            List<string> names = new List<string>();
            string[] filter = null;
            string notes = frameworkInstance.choosed.element.Notes;

            int filterIndex;
            if (notes != "" && (filterIndex = notes.IndexOf("filter=")) != -1)
            {
                notes = notes.Substring(filterIndex + 7);
                notes = notes.Substring(0, notes.IndexOf(";"));

                if (notes != "")
                {
                    this.eaUtils.printOut("Se aplicarán los siguientes filtros " + notes);

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
                        if (Regex.IsMatch(attr.Name, filt, RegexOptions.ECMAScript))
                        {
                            this.eaUtils.printOut("se descarta "+ attr.Name);

                            add = false;
                            break;
                        }
                    }
                }
                if (add)
                {
                    pojoInfo.attributes.Add(buildFromColumn(attr));
                }
            }
            return names;
        }

    }

    public class FkEntity
    {
        public Dictionary<string, Namespace> namespacesByArtifact = new Dictionary<string, Namespace>();
        public Element entity = null;
        public string @namespace = "";
        public string plusNamespace = "";
        public string relationName = "";
        public string pluralName = "";
        public string pluralAlias = "";
        public bool isRelationSource = false;
        public Element table = null;
    }

    public class FkInfo
    {
        internal string pluralName;
        internal string pluralAlias;

        public string tableName { get; internal set; }
        public string tableAlias { get; internal set; }
        public string tableGUID { get; internal set; }
        public Cardinality cardinalitySource { get; internal set; }
        public Cardinality cardinalityTarget { get; internal set; }
        public FkEntity entity = null;

        public bool one2one()
        {
            return !cardinalitySource.isCollection() && !cardinalityTarget.isCollection();
        }
        public bool one2many()
        {
            return !cardinalitySource.isCollection() && cardinalityTarget.isCollection();
        }
        public bool many2one()
            {
            return cardinalitySource.isCollection() && ! cardinalityTarget.isCollection();
        }
        public bool many2many()
        {
            return cardinalitySource.isCollection() && cardinalityTarget.isCollection();
        }
    }
}
