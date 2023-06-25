using EA;
using EAUtils.entity;
using EAUtils.framework2;
using Newtonsoft.Json;
using Persistencia.frw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Persistencia.entity
{
    class Info : JsonableCommon, Jsonable
    {
        public string entity;
        public string alias;
        public string pluralName;
        public string pluralAlias;
        public string notes;
        public List<FkEntity> entities = new List<FkEntity>();
        public List<FkEntity> collectionsOneToMany = new List<FkEntity>();
        public List<FkEntity> collectionsManyToMany = new List<FkEntity>();
        public List<ColumnFullAttribute> attributes = new List<ColumnFullAttribute>();
        public string tableName;
        public string tableOwner;
        public string tablespace;
        public string tableDbVersion;
        private Dictionary<string, Namespace> namespacesByArtifact = new Dictionary<string, Namespace>();
        public string rootNamespace = null;
        public Namespace mainNamespace = null;
        internal bool many2many;

        public void setElement(Element element)
        {
            this.entity = EAUtils.StringUtils.toPascal(element.Name);
            this.notes =  element.Notes;
        }

        public void add(EA.Attribute attribute, EAUtils.EAUtils eaUtils)
        {
            attributes.Add(new ColumnFullAttribute(attribute, eaUtils));
        }

        /// <summary>
        /// Para el mismo artefacto puede suceder que un atributo sobreescriba el valor (se ignora porque arrancamos de abajo) y también que agregue.
        /// En la jerarquía puede suceder que se sobreescriba el valor.
        /// </summary>
        /// <param name="np"></param>
        /// <param name="inheritorArtifactName">Artifact name que hereda del actual</param>
        public void add(Namespace np, string inheritorArtifactName)
        {
            // el que se está agregando es el parent del mismo artefacto.
            if (namespacesByArtifact.ContainsKey(np.artifactName))
            {
                // evaluar si se sobreescribe algún valor, se ignora porque arrancamos del fondo de la jerarquía.
                // evaluar si se agrega algún valor.
                foreach (KeyValuePair<string, string> newAttr in np.attributes)
                {
                    if (newAttr.Value != "")
                    {
                        if (!namespacesByArtifact[np.artifactName].attributes.ContainsKey(newAttr.Key))
                        {
                            namespacesByArtifact[np.artifactName].attributes.Add(newAttr.Key, newAttr.Value);
                        }
                    }
                }
            }
            else
            {
                namespacesByArtifact.Add(np.artifactName, np);

                if (mainNamespace == null && np.isMain)
                {
                    mainNamespace = np;
                }
            }
        }

        public Dictionary<string, Namespace> getNamespacesByArtifact()
        {
            return this.namespacesByArtifact;
        }

        public string getFullNamespace()
        {
            string fullNamespace = "";

            if (mainNamespace.attributes.ContainsKey("namespace"))
            {
                fullNamespace = mainNamespace.attributes["namespace"] + (mainNamespace.attributes.ContainsKey("plus") ? "."+ mainNamespace.attributes["plus"] : "");
            }
            else
            {
                fullNamespace = rootNamespace + (mainNamespace.attributes.ContainsKey("plus") ? "." + mainNamespace.attributes["plus"] : "");
            }
            return fullNamespace;
        }

        public void parse(String infoString, EAUtils.EAUtils eaUtils)
        {
            dynamic json = Newtonsoft.Json.Linq.JObject.Parse(infoString);

                this.entity = json.entity;

            foreach (dynamic ci in json.attributes)
            {
                attributes.Add(new FullAttribute().parse(ci, eaUtils));
            }
        }

        public string stringfity()
        {
            this.start();

            List<string> uniqueImports = new List<string>();

            writer.WritePropertyName("namespace_root");
            writer.WriteValue(this.rootNamespace);

            writer.WritePropertyName("plural_alias");
            writer.WriteValue(this.pluralAlias);

            writer.WritePropertyName("plural_name");
            writer.WriteValue(this.pluralName);

            writer.WritePropertyName("alias");
            writer.WriteValue(this.alias);

            writer.WritePropertyName("entity");
            writer.WriteValue(this.entity);

            writer.WritePropertyName("notes");
            writer.WriteValue(this.notes);

            writer.WritePropertyName("many2many");
            writer.WriteValue(this.many2many);

            writer.WritePropertyName("tableName");
            writer.WriteValue(this.tableName);

            writer.WritePropertyName("tableExtra");

            writer.WriteStartObject();

            writer.WritePropertyName("owner");
            writer.WriteValue(this.tableOwner);

            writer.WritePropertyName("space");
            writer.WriteValue(this.tablespace);

            writer.WritePropertyName("version");
            writer.WriteValue(this.tableDbVersion);

            writer.WriteEnd();

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

            this.writeNamespaces(writer, this.namespacesByArtifact );

            // entidades asociadas
            writer.WritePropertyName("entities");
            writer.WriteStartArray();

            writeEntities(writer, this.entities);

            writer.WriteEndArray();

            // init lista collections
            writer.WritePropertyName("collections");
            writer.WriteStartArray();

            writeCollections(writer, this.collectionsOneToMany, false);

            writeCollections(writer, this.collectionsManyToMany, true);

            writer.WriteEndArray();
            // end lista collections

            return this.end();
        }

        private void writeNamespaces(JsonWriter writer, Dictionary<string, Namespace> namespacesByArtifact )
        {
            // namespaces relacionados con los artefactos
            bool hasFullnamespace = false;
            string plusNamespace = null;

            foreach (KeyValuePair<string, Namespace> np in namespacesByArtifact)
            {
                writer.WritePropertyName(EAUtils.StringUtils.toSnake(np.Value.artifactName));

                hasFullnamespace = false;
                plusNamespace = null;

                writer.WriteStartObject();

                foreach (KeyValuePair<string, string> attr in np.Value.attributes)
                {
                    writer.WritePropertyName(attr.Key);
                    writer.WriteValue(attr.Value);

                    if (attr.Key == "namespace")
                    {
                        hasFullnamespace = true;

                    }
                    else if (attr.Key == "plus")
                    {
                        plusNamespace = attr.Value;
                    }
                }

                if (!hasFullnamespace && plusNamespace != null && this.rootNamespace != null)
                {
                    writer.WritePropertyName("namespace");
                    writer.WriteValue(this.rootNamespace + "." + plusNamespace);
                }

                writer.WriteEnd();
            }
        }

        private void writeList(JsonWriter writer, List<ColumnFullAttribute> list)
        {
            foreach (ColumnFullAttribute ca in list)
            {
                ca.stringfity(writer);
            }
        }

        private void writeRealPkList(JsonWriter writer, List<ColumnFullAttribute> list)
        {
            foreach (ColumnFullAttribute ca in list)
            {
                if (ca.realPk != null && ca.realPk == true)
                {
                    ca.stringfity(writer);
                }
            }
        }

        private void writeExternalEntity(JsonWriter writer, FkEntity externalEntity, bool? isManyToMany)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("entity");

            writer.WriteStartObject();

            writer.WritePropertyName("name");
            writer.WriteValue(externalEntity.entity.Name);

            writer.WritePropertyName("plural_name");
            writer.WriteValue(externalEntity.pluralName);

            writer.WritePropertyName("alias");
            writer.WriteValue(externalEntity.entity.Alias);

            writer.WritePropertyName("plural_alias");
            writer.WriteValue(externalEntity.pluralAlias);

            writer.WritePropertyName("namespace");
            writer.WriteValue(externalEntity.@namespace);

            writer.WritePropertyName("relation_name");
            writer.WriteValue( externalEntity.relationName);

            writer.WritePropertyName("is_relation_source");
            writer.WriteValue(externalEntity.isRelationSource);

            if( isManyToMany != null)
            {
                writer.WritePropertyName("many2many");
                writer.WriteValue(isManyToMany);
            }

            // table
            writer.WritePropertyName("table");

            writer.WriteStartObject();

            writer.WritePropertyName("alias");
            writer.WriteValue(externalEntity.table.Alias);

            writer.WritePropertyName("name");
            writer.WriteValue(externalEntity.table.Name);

            writer.WriteEnd();
            // end table

            this.writeNamespaces(writer, externalEntity.namespacesByArtifact);

            writer.WriteEnd();

            writer.WriteEnd();
        }

        private void writeCollections(JsonWriter writer, List<FkEntity> list, bool isManyToMany)
        {
            foreach (FkEntity ca in list)
            {
                writeExternalEntity(writer, ca, isManyToMany);
            }
        }
        private void writeEntities(JsonWriter writer, List<FkEntity> list)
        {
            foreach (FkEntity ca in list)
            {
                writeExternalEntity(writer, ca, null);
            }
        }

        internal void update(ColumnFullAttribute fullAttribute)
        {
            ColumnFullAttribute current;
            for (short i = 0; i < this.attributes.Count; i++)
            {
                current = this.attributes[i];
                if (current.eaAttribute.AttributeGUID == fullAttribute.eaAttribute.AttributeGUID)
                {
                    this.attributes[i] = fullAttribute;
                    break;
                }
            }
        }

        internal void addCollection(FkEntity fkEntiy, bool isManyToMany)
        {
            bool found = false;
            if( isManyToMany)
            {
                foreach (FkEntity info in this.collectionsManyToMany)
                {
                    if (info.entity.ElementID == fkEntiy.entity.ElementID)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    this.collectionsManyToMany.Add(fkEntiy);
                }

            }
            else
            {
                foreach (FkEntity info in this.collectionsOneToMany)
                {
                    if (info.entity.ElementID == fkEntiy.entity.ElementID)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    this.collectionsOneToMany.Add(fkEntiy);
                }

            }
        }

        internal void addEntity(FkEntity fkEntiy)
        {
            bool found = false;
            foreach(FkEntity entity in this.entities)
            {
                if(entity.relationName == fkEntiy.relationName && entity.entity.ElementID == fkEntiy.entity.ElementID)
                {
                    found = true;
                    break;
                }
            }
            if( ! found)
            {
                this.entities.Add(fkEntiy);
            }
        }
    }

}
