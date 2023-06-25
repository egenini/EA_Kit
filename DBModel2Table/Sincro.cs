using EA;
using EAUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model2Table
{
    public class Sincro
    {
        private const string GET_COLUMNA_FROM_ATTR_GUID = "select elementid from t_attributetag where value = '{{attr_guid}}' and property = 'm2t.source'";
        
        EAUtils.EAUtils eaUtils;
        public Framework framework = null;
        public Sincro(EAUtils.EAUtils eaUtils) 
        {
            this.eaUtils = eaUtils;
        }
        public void enModelModificado(Element model, string lastName)
        {
            /*
             * Se podría modificar:
             * El nombre
             * Las notas
             * El resto no importa.
             */

            string guid = this.eaUtils.taggedValuesUtils.get(model, "m2t.target", "").asString();

            if( guid != "")
            {
                Element tabla = this.eaUtils.repository.GetElementByGuid(guid);

                if( model.Name != lastName)
                {
                    tabla.Name = framework.tableName(model.Name);

                    foreach (EA.Method m in tabla.Methods)
                    {
                        if (m.Stereotype == "PK")
                        {
                            m.Name = framework.pkTable(tabla);
                            m.Update();
                        }
                    }
                }

                tabla.Notes = model.Notes;

                tabla.Update();
            }
            else
            {   
                if( model.Name != lastName)
                {
                    crearTabla(model);

                    if (framework.addIdAttribute())
                    {
                        if (model.Attributes.Count == 0)
                        {
                            EA.Attribute a = model.Attributes.AddNew(framework.idAttribute(model), "");

                            a.Update();
                            model.Attributes.Refresh();
                        }
                    }
                }
            }
        }
        public void enModelBorrado(Element model)
        {
            this.eliminarTabla(model);
        }
        public void enNuevoAtributo( EA.Attribute a)
        {
            EA.Element model = this.eaUtils.repository.GetElementByID(a.ParentID);

            TaggedValueWrapper tgv = eaUtils.taggedValuesUtils.get(model, "m2t.target", "");

            if (tgv.value != "")
            {
                EA.Element tabla = this.eaUtils.repository.GetElementByGuid(tgv.asString());

                this.crearColumna(model, tabla, a);
            }
        }
        public void enAtributoModificado( EA.Attribute a)
        {
            this.modificarColumna(a);
        }
        public void enBorrarAtributo( EA.Attribute a)
        {
            this.eliminarColumna(a);
        }
        public void crearTabla(Element model)
        {
            Diagram d = this.eaUtils.repository.GetCurrentDiagram();
            Diagram targetDiagram = framework.targetDiagram(d);
            Package targetPackage = this.eaUtils.repository.GetPackageByID(targetDiagram.PackageID);

            EA.Element tabla = targetPackage.Elements.AddNew(framework.tableName(model.Name), "table");
            
            tabla.Update();

            this.eaUtils.taggedValuesUtils.set(model, "m2t.target", tabla.ElementGUID);

            Package database = this.eaUtils.repository.GetPackageByID(this.eaUtils.repository.GetPackageByID(tabla.PackageID).ParentID);

            if( database.StereotypeEx == "Database")
            {
                tabla.Gentype = this.eaUtils.taggedValuesUtils.get(database.Element, "DBMS", "").asString();

                tabla.Update();
            }

            this.eaUtils.taggedValuesUtils.set(tabla, "m2t.source", model.ElementGUID);

            EA.Attribute pk = tabla.Attributes.AddNew(framework.idColumnName(tabla), framework.idColumnType(model, tabla, "ID"));
            pk.Stereotype = "column";
            pk.IsOrdered = true;
            pk.AllowDuplicates = true;
            pk.Update();

            EA.Method m = tabla.Methods.AddNew(framework.pkTable(tabla), "");
            m.Stereotype = "PK";
            m.Update();

            EA.Parameter p = m.Parameters.AddNew(framework.idColumnName(tabla), pk.Type);
            p.Update();

            this.eaUtils.connectorUtils.addRealization(tabla, model);

            //this.eaUtils.diagramUtils.addDiagramElement(targetDiagram, tabla, );
            //targetDiagram.DiagramObjects.AddNew()
        }

        private void eliminarTabla(Element model)
        {
            string guid = this.eaUtils.taggedValuesUtils.get(model, "m2t.target", "").asString();

            if(guid != "")
            {
                EA.Element tabla = this.eaUtils.repository.GetElementByGuid(guid);

                Package package = this.eaUtils.repository.GetPackageByID(tabla.PackageID);

                for( short i = 0; i < package.Elements.Count; i++)
                {
                    if( package.Elements.GetAt(i).ElementGUID == guid)
                    {
                        package.Elements.Delete(i);
                        break;
                    }
                }
                package.Elements.Refresh();
            }
        }
        public void crearColumnas(Element model)
        {
            TaggedValueWrapper tgv = eaUtils.taggedValuesUtils.get(model, "m2t.target", "");

            if (tgv.value != "")
            {
                EA.Element tabla = this.eaUtils.repository.GetElementByGuid(tgv.asString());

                foreach(EA.Attribute a in model.Attributes)
                {
                    tabla.Attributes.AddNew(a.Name, this.framework.dataType(model, tabla, a.Type));
                }
            }
        }
        public void crearColumna(EA.Element model, EA.Element tabla, EA.Attribute a)
        {
            ResultAsList r = (ResultAsList)new SQLUtils(eaUtils).excecute(GET_COLUMNA_FROM_ATTR_GUID.Replace("{{attr_guid}}", a.AttributeGUID));

            if (r.data.Count == 0)
            {
                EA.Attribute c = tabla.Attributes.AddNew(this.framework.columnName(a.Name), this.framework.dataType(model, tabla, a.Type));

                c.Stereotype = "column";
                c.Notes = a.Notes;

                c.Update();

                this.eaUtils.taggedValuesUtils.set(c, "m2t.source", a.AttributeGUID);
            }
            else
            {
                EA.Attribute c = this.eaUtils.repository.GetAttributeByID(int.Parse((string)r.data[0][0]));

                c.Notes = a.Notes;
                c.Update();
            }
        }
        private void eliminarColumna(EA.Attribute a)
        {
            ResultAsList r = (ResultAsList)new SQLUtils(eaUtils).excecute(GET_COLUMNA_FROM_ATTR_GUID.Replace("{{attr_guid}}", a.AttributeGUID));

            if (r.data.Count != 0)
            {
                EA.Attribute c = this.eaUtils.repository.GetAttributeByID(int.Parse((string)r.data[0][0]));

                EA.Element tabla = this.eaUtils.repository.GetElementByID(c.ParentID);

                for( short i = 0; i < tabla.Attributes.Count; i++)
                {
                    if(tabla.Attributes.GetAt(i).AttributeID == c.AttributeID )
                    {
                        tabla.Attributes.Delete(i);
                        break;
                    }
                }
                    
                // refrescar diagrama
                this.eaUtils.repository.ReloadDiagram(framework.targetDiagram(this.eaUtils.repository.GetCurrentDiagram()).DiagramID);
                tabla.Attributes.Refresh();
            }
        }

        public void modificarColumna(EA.Attribute a)
        {
            ResultAsList r = (ResultAsList)new SQLUtils(eaUtils).excecute(GET_COLUMNA_FROM_ATTR_GUID.Replace("{{attr_guid}}", a.AttributeGUID));

            if (r.data.Count != 0)
            {
                EA.Attribute c = this.eaUtils.repository.GetAttributeByID(int.Parse((string)r.data[0][0]));

                c.Name = this.framework.columnName(a.Name);
                c.Notes = a.Notes;
                c.Update();
                // refrescar diagrama
                this.eaUtils.repository.ReloadDiagram(framework.targetDiagram(this.eaUtils.repository.GetCurrentDiagram()).DiagramID);
            }
        }
    }
}
