using EA;
using Persistencia.classtable;
using Persistencia.custom_table;
using Persistencia.table2class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UIResources;

namespace Persistencia
{
    public class Main : MainCommons
    {
        public void EA_MenuClick(EA.Repository repository, string location, string menuName, string itemName)
        {
            switch (itemName)
            {
                case ITEM_MENU__GENERAR:

                    generarTodo(this.getTableSelected(repository), repository);

                    break;
                case ITEM_MENU__GENERAR_JSON:

                    generarJSon(this.getTableSelected(repository), repository);

                    break;
                case ITEM_MENU__GENERAR_ARTEFACTO:

                    generarArtefacto(this.getTableSelected(repository), repository);

                    break;
            }
        }

        private void generarTodo(Element element, Repository repository)
        {
            try
            {
                frw.Framework frameworkInstance = new frw.Framework(this.eaUtils);

                if (frameworkInstance.choose( element ))
                {

                    frw.Generator generator = new frw.Generator(element, frameworkInstance, this.eaUtils);

                    generator.generarJSon();

                    generator.generarArtefacto();

                    Alert.Success("Se ha/n generado el/los artefactos");
                }
            }
            catch (Exception e)
            {
                Clipboard.SetText(e.ToString());
                Alert.Error("Error pegado al portapapeles");
            }
        }

        private void generarArtefacto(Element element, Repository repository)
        {
            try
            {
                frw.Framework frameworkInstance = new frw.Framework(this.eaUtils);

                if (frameworkInstance.choose(element))
                {

                    frw.Generator generator = new frw.Generator(element, frameworkInstance, this.eaUtils);

                    generator.generarArtefacto();

                    Alert.Success("Se ha/n generado el/los artefactos");
                }
            }
            catch (Exception e)
            {
                Clipboard.SetText(e.ToString());
                Alert.Error("Error pegado al portapapeles");
            }
        }
        private void generarJSon(Element element, Repository repository)
        {
            try
            {
                frw.Framework frameworkInstance = new frw.Framework(this.eaUtils);

                if (frameworkInstance.choose(element))
                {

                    frw.Generator generator = new frw.Generator(element, frameworkInstance, this.eaUtils);

                    generator.generarJSon();

                    Alert.Success("Se ha generado el JSON");
                }
            }
            catch (Exception e)
            {
                Clipboard.SetText(e.ToString());
                Alert.Error("Error pegado al portapapeles");
            }
        }

        public Boolean EA_OnPostNewConnector(Repository repository, EventProperties info)
        {
            Boolean changed = false;

            if (repository != null && info != null)
            {
                Diagram diagrama = repository.GetCurrentDiagram();

                Connector connector = repository.GetConnectorByID(int.Parse((string)info.Get("ConnectorID").Value));

                Element origen  = repository.GetElementByID( connector.ClientID   );
                Element destino = repository.GetElementByID( connector.SupplierID );

                if (connector.Type == EAUtils.ConnectorUtils.CONNECTOR__ASSOCIATION)
                {

                    if (origen.Stereotype == "table" && destino.Stereotype == "table")
                    {
                        // si el origen no tiene una columna con el nombre de la tabla más el _id se lo agregamos
                        bool addColumn = true;
                        string fkColumnName = destino.Name + "_id";

                        foreach (EA.Attribute columna in origen.Attributes)
                        {
                            if (columna.Name == fkColumnName)
                            {
                                addColumn = false;
                                break;
                            }
                        }

                        if (addColumn)
                        {
                            EA.Attribute fkColumn = origen.Attributes.AddNew(fkColumnName, "integer");
                            fkColumn.Stereotype = "column";
                            fkColumn.Update();

                            this.buildEAUtils(repository);

                            eaUtils.taggedValuesUtils.setRealPrimaryKey(fkColumn, "true");
                            eaUtils.taggedValuesUtils.setSearch(fkColumn, "true" );

                            changed = true;
                        }
                    }
                    //MessageBox.Show("Productividad post connector create - origen " + origen.Name +" destino "+ destino.Name);
                }
                else if (connector.Type == EAUtils.ConnectorUtils.CONNECTOR__DEPENDENCY)
                {
                    if( origen.StereotypeEx == "custom table" && ( destino.Type == "table" || (destino.Type == "Class" && destino.Stereotype == "table"  )) ){

                        this.buildEAUtils(repository);

                        SincronizaColumnas sc = new SincronizaColumnas(eaUtils, destino, origen);

                        sc.sincornizar();
                    }
                }
            }
            return changed;
        }

        public Boolean EA_OnPreDropFromTree(EA.Repository repository, EA.EventProperties info)
        {
            Boolean doIt = true;

            if (repository != null && info != null)
            {
                Element element = null;
                EA.Attribute attribute = null;
                Package package = null;

                try
                {
                    string type = info.Get("Type").Value;

                    if (type == "23")
                    {
                        attribute = repository.GetAttributeByID(info.Get("ID").Value);
                    }

                    else if (type == "4")
                    {
                        element = repository.GetElementByID(info.Get("ID").Value);

                    }
                    else if( type == "5")
                    {
                        package = repository.GetPackageByID(info.Get("ID").Value);
                        if( package.Name.ToLower().StartsWith("template") || package.Element.Stereotype != "Dialect")
                        {
                            package = null;
                        }
                    }

                    Diagram diagram = repository.GetDiagramByID(info.Get("DiagramID").Value);
                    Element dropped = null;

                    try
                    {
                        dropped = repository.GetElementByID(info.Get("DroppedID").Value);
                    }
                    catch (Exception) { }

                    if (dropped != null)
                    {
                        if (dropped.Type == "Class")
                        {
                            if (attribute != null)
                            {
                                ColumnOrAttribute2AttributeOrColumn x2x = new ColumnOrAttribute2AttributeOrColumn(repository, dropped);

                                x2x.toColumnOrAttribute(attribute);

                                doIt = false;
                            }
                            else if (element != null)
                            {
                                if ((element.Type == "Class" && element.Stereotype == "DomainClass") || (element.Type == "Interface" && element.Stereotype == "DomainInterface"))
                                {
                                    TableOrClass2ClassOrTable x2x = new TableOrClass2ClassOrTable(repository, dropped);
                                    x2x.toTableOrClass(element);

                                    /*
                                    Class2Table class2Table = new Class2Table(repository, dropped);
                                    class2Table.toTable(element);
                                    */

                                    doIt = false;
                                }
                                if (element.Type == "Class" && element.Stereotype == "table")
                                {
                                    TableOrClass2ClassOrTable x2x = new TableOrClass2ClassOrTable(repository, dropped);
                                    x2x.toTableOrClass(element);

                                    /*
                                    Class2Table class2Table = new Class2Table(repository, dropped);
                                    class2Table.toTable(element);
                                    */

                                    doIt = false;
                                }
                                else if (element.Type == "Enumeration")
                                {
                                    Enumeration2Table enum2Table = new Enumeration2Table(repository, dropped);
                                    enum2Table.toTable(element);
                                    doIt = false;
                                }
                            }
                            else
                            {
                                // suelto un elemento en un diagrama
                            }
                        }
                        if (element != null)
                        {
                            if (dropped.Stereotype == "table")
                            {
                                if (element.Stereotype == "Template")
                                {
                                    // agregamos el template como un valor etiquetado
                                    this.buildEAUtils(repository);
                                    frw.Framework frameworkInstance = new frw.Framework(this.eaUtils);

                                    frameworkInstance.setTemplateTaggedValue(element, dropped);

                                    doIt = false;
                                }
                            }
                            else if (element.Stereotype == "Artifact" && dropped.Stereotype == "Namespace")
                            {
                                // agreggamos al namespace 
                                this.buildEAUtils(repository);

                                Package artifactPackage = this.eaUtils.repository.GetPackageByID(element.PackageID);
                                Package dialectPackage = this.eaUtils.repository.GetPackageByID(artifactPackage.ParentID);
                                Package languagePackage = this.eaUtils.repository.GetPackageByID(dialectPackage.ParentID);

                                dropped.Name = EAUtils.StringUtils.toPascal(languagePackage.Name) + EAUtils.StringUtils.toPascal(dialectPackage.Name) + EAUtils.StringUtils.toPascal(artifactPackage.Name);

                                dropped.Alias = artifactPackage.Name;

                                dropped.Tag = element.ElementGUID;

                                this.buildEAUtils(repository);
                                frw.Framework frameworkInstance = new frw.Framework(this.eaUtils);

                                frameworkInstance.addNamespaceAttributes(element, dropped);

                                dropped.Update();
                                doIt = false;
                            }
                        }
                        
                        if (package != null)
                        {
                            // agregamos un atributo para que se pueda poner el namespace y asociarlo.
                            if (package.Element.Stereotype == "Dialect" && dropped.Stereotype == "Namespace")
                            {
                                // agregamos el template como un valor etiquetado
                                this.buildEAUtils(repository);

                                Package languaguePackage = this.eaUtils.repository.GetPackageByID(package.ParentID);

                                string fqname = "Persistence::" + languaguePackage.Name + "-" + package.Name;

                                bool addattr = true;

                                foreach (EA.Attribute attr in dropped.Attributes)
                                {
                                    if (attr.Name == fqname)
                                    {
                                        addattr = false;
                                        break;
                                    }
                                }

                                if (addattr)
                                {
                                    EA.Attribute attr = dropped.Attributes.AddNew(fqname, "");

                                    this.eaUtils.taggedValuesUtils.set(attr, "Persistence::source", package.PackageGUID);

                                    attr.Update();
                                }

                                doIt = false;
                            }

                            if (dropped.Stereotype == "table")
                            {
                                // agregamos el template como un valor etiquetado
                                this.buildEAUtils(repository);
                                frw.Framework frameworkInstance = new frw.Framework(this.eaUtils);

                                if (package.Element.Stereotype == "Dialect")
                                {
                                    frameworkInstance.setDialectTaggedValue(package, dropped);

                                    doIt = false;
                                }
                                else
                                {
                                    // sólo sería un template porque lo estoy filtrando antes.
                                    // vamos delegar en el método la verificación de que el paquete contenga templates.
                                    frameworkInstance.setTemplateTaggedValue(package, dropped);

                                    doIt = false;
                                }
                            }
                        }
                    }
                }
                catch ( Exception e)
                {
                    Clipboard.SetText(e.ToString());
                }
            }

            return doIt;
        }
    }
}
