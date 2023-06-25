using EAUtils.entity;
using EAUtils.framework2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Persistencia.frw;

namespace Persistencia.entity
{
    class ColumnFullAttribute : AttributeBase, Jsonable
    {
        public string columnType = "";
        public string columnName = "";

        public FkInfo fkInfo { get; internal set; }

        public ColumnFullAttribute(EA.Attribute column, EAUtils.EAUtils eautils) : base(column, eautils)
        {
            this.columnType = column.Type;
            this.columnName = column.Name;
        }

        public ColumnFullAttribute(EA.Attribute attribute, EA.Attribute column ,EAUtils.EAUtils eautils) : base(attribute, eautils)
        {
            this.columnType = column.Type;
            this.columnName = column.Name;
        }

        public new void stringfity(JsonWriter writer)
        {
            this.writer = writer;

            writer.WriteStartObject();

            this.stringfityProperties();

            this.writeExternalEntity(writer);

            writer.WriteEnd();
        }

        private void writeExternalEntity(JsonWriter writer )
        {
            FkEntity externalEntity = fkInfo != null ? fkInfo.entity : null;

            writer.WritePropertyName("entity");

            if( externalEntity != null && externalEntity.entity != null)
            {
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
                writer.WriteValue(externalEntity.relationName);

                // table
                writer.WritePropertyName("table");

                writer.WriteStartObject();


                if ( externalEntity.table != null )
                {
                    writer.WritePropertyName("alias");
                    writer.WriteValue(externalEntity.table.Alias);
                    writer.WritePropertyName("name");
                    writer.WriteValue(externalEntity.table.Name);
                }
                else
                {
                    writer.WritePropertyName("alias");
                    writer.WriteNull();
                    writer.WritePropertyName("name");
                    writer.WriteNull();
                }

                writer.WriteEnd();
                // end table

                this.writeNamespaces( writer, fkInfo.entity );

                writer.WriteEnd();
            }
        }

        public new void stringfityProperties()
        {

            base.stringfityProperties();

            writer.WritePropertyName("columnInfo");

            writer.WriteStartObject();

            writer.WritePropertyName("name");
            writer.WriteValue(this.columnName);

            writer.WritePropertyName("type");
            writer.WriteValue(this.columnType);
            
            writer.WritePropertyName("fk");
            writer.WriteValue(fkInfo != null ? true : false);

            writer.WritePropertyName("fkWith");
            writer.WriteValue(fkInfo != null ? fkInfo.tableName : "");

            writer.WritePropertyName("fkWithPluralName");
            writer.WriteValue(fkInfo != null ? fkInfo.pluralName : "");

            writer.WritePropertyName("fkWithGUID");
            writer.WriteValue(fkInfo != null ? fkInfo.tableGUID : "");

            writer.WritePropertyName("one2one");
            writer.WriteValue(fkInfo != null && fkInfo.one2one() ? true : false);

            writer.WritePropertyName("one2many");
            writer.WriteValue(fkInfo != null && fkInfo.one2many() ? true : false);

            writer.WritePropertyName("many2one");
            writer.WriteValue(fkInfo != null && fkInfo.many2one() ? true : false);

            writer.WritePropertyName("many2many");
            writer.WriteValue(fkInfo != null && fkInfo.many2many() ? true : false);

            writer.WriteEnd();
        }

        public new string stringfity()
        {
            this.stringfityStart();

            this.stringfityProperties();


            return this.strinfityEnd();
        }
        private void writeNamespaces(JsonWriter writer, FkEntity entity)
        {
            // namespaces relacionados con los artefactos
            bool hasFullnamespace = false;
            string plusNamespace = null;

            foreach (KeyValuePair<string, Namespace> np in entity.namespacesByArtifact)
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

                if (!hasFullnamespace && plusNamespace != null && entity.@namespace != null)
                {
                    writer.WritePropertyName("namespace");
                    writer.WriteValue(entity.@namespace + "." + plusNamespace);
                }

                writer.WriteEnd();
            }
        }
    }
}
