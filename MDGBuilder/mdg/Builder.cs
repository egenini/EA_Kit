using EA;
using EAUtils;
using EAUtils.saveUtils;
using MDGBuilder.mdg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using UIResources;

namespace MDGBuilder
{
    public class Builder
    {
        public const string TV__GENFILE_PREFIX = "GenFile-";

        public Package mdgPackage;
        public EAUtils.EAUtils eaUtils;
        public string mdgName;
        public string mdgAlias;
        XmlUtils xmlMts = new XmlUtils();
        XmlUtils xmlMdg = new XmlUtils();
        List<TemplateInfo> templatesInfo = new List<TemplateInfo>();
        Package parentPackage = null;
        Element mtsElement = null;
        GenericProfileInfo profilesInfo = new GenericProfileInfo();
        GenericProfileInfo patternsInfo = new GenericProfileInfo();
        GenericProfileInfo diagramsInfo = new GenericProfileInfo();
        GenericProfileInfo toolboxesInfo = new GenericProfileInfo();
        ModelUtil modelUtil;
        SemanticVersioning semanticVersioning;
        MtsUtil mtsUtil;
        DiagramPatternToolboxHelper diagramPatternToolboxHelper;
        public Builder(Package mdgPackage, SemanticVersioning semanticVersioning, EAUtils.EAUtils eaUtils)
        {
            this.mdgPackage = mdgPackage;
            this.eaUtils = eaUtils;

            this.mtsUtil = new MtsUtil(xmlMts);

            this.mtsUtil.eaUtils = this.eaUtils;

            this.modelUtil = new ModelUtil(this.eaUtils);

            this.semanticVersioning = semanticVersioning;

            this.diagramPatternToolboxHelper = new DiagramPatternToolboxHelper(eaUtils);

            try
            {
                foreach (Element currentElement in mdgPackage.Elements)
                {
                    if (currentElement.Type == "Class")
                    {
                        mtsElement = currentElement;
                        break;
                    }
                }

                this.mdgName  = mtsElement.Name;
                this.mdgAlias = mtsElement.Alias;

                // la importación lanza error por eso comento esta línea
                //this.eaUtils.repository.DeleteTechnology(mdgName);

            }
            catch (Exception e)
            {
                eaUtils.printOut(e.ToString());

                Alert.Error("Se produjo un error, vea la ventana de salida");
            }
        }

        public void build()
        {
            string stereotype;
            string quicklinker = null;

            if (mdgPackage.Element.Genfile == "")
            {
                // puede ser que exista un mts o bien que no.
                // vamos a buscar, si no hay vamos a tener que crear uno.
                // si lo encontramos vamos a abrirlo para agregar, quitar templates, cajas, diagramas, profiles, modificar versión,etc.
                SaveFileInfo info = new SaveFileInfo();

                info.fileName(this.mdgName);

                if (this.selectFileNameMTS(info))
                {
                    mdgPackage.Element.Genfile = info.fileName();
                    mdgPackage.Update();
                }
            }

            if (mdgPackage.Element.Genfile != "")
            {
                if (!System.IO.File.Exists(mdgPackage.Element.Genfile) && mtsElement != null)
                {
                    // vemos si podemos crear uno
                    this.mtsUtil.create(this.mdgPackage, mtsElement, mdgName);

                    xmlMts.xmlDOM.Save(mdgPackage.Element.Genfile);
                }

                xmlMts.loadFromFile(mdgPackage.Element.Genfile);

                // eliminamos todos los "profiles" y luego se agregarán.

                if (xmlMts.xmlDOM != null)
                {
                    XmlNode profilesNode = xmlMts.xmlDOM.SelectSingleNode("/MDG.Selections/Profiles");
                    XmlNode diagramNode = xmlMts.xmlDOM.SelectSingleNode("/MDG.Selections/DiagramProfile");
                    XmlNode toolboxNode = xmlMts.xmlDOM.SelectSingleNode("/MDG.Selections/UIToolboxes");
                    XmlNode patternsNode = xmlMts.xmlDOM.SelectSingleNode("/MDG.Selections/Patterns");

                    if (profilesNode != null)
                    {
                        profilesNode.ParentNode.RemoveChild(profilesNode);
                    }
                    if (diagramNode != null)
                    {
                        diagramNode.ParentNode.RemoveChild(diagramNode);
                    }
                    if (toolboxNode != null)
                    {
                        toolboxNode.ParentNode.RemoveChild(toolboxNode);
                    }
                    if (patternsNode != null)
                    {
                        patternsNode.ParentNode.RemoveChild(patternsNode);
                    }
                }
            }

            foreach (Package package in mdgPackage.Packages)
            {
                if ( package.Element.Tag.ToLower().Equals("ignore") || package.Element.Tag.ToLower().Equals("ignorar"))
                {
                    this.eaUtils.printOut("Paquete ignorado " + package.Name);
                    continue;
                }

                stereotype = package.Element.Stereotype;

                if (stereotype.IndexOf("profile") != -1)
                {
                    makeProfile(package);
                }
                else if (package.Name.ToUpper() == "QUICKLINKER")
                {
                    QuickLinkerBuilder builder = new QuickLinkerBuilder(this.eaUtils);

                    try
                    {
                        if (builder.build(package))
                        {
                            quicklinker = builder.getLinesAsString();
                        }
                    }
                    catch (Exception e)
                    {
                        this.eaUtils.printOut("Error en quicklinker");
                        this.eaUtils.printOut(e.ToString());
                    }
                }
                else
                {
                    // si hay otros paquetes pueden ser templates y/o pueden ser patterns.
                    if (package.Packages.Count == 0)
                    {
                        // si no tiene paquetes veo que tenga elementos o un diagrama al memos.
                        if (package.Diagrams.Count != 0 || package.Elements.Count != 0)
                        {
                            // aunque no tiene parent package lo uso igual para facilitar la lógica de lo que sigue.
                            parentPackage = package;
                            savePackage(package);
                        }
                    }
                    else
                    {
                        foreach (Package child in package.Packages)
                        {
                            // veo que tenga elementos o un diagrama al memos.
                            if (child.Diagrams.Count != 0 || child.Elements.Count != 0 || child.Packages.Count != 0)
                            {
                                parentPackage = package;
                                savePackage(child);
                            }
                        }
                    }
                }
            }

            // antes de generar el mdg hay que agregar los templates en el nodo <ModelTemplates>
            // la localización del mdg está en un tag del mts. el tag se llama Technology y el atributo es filename
            //XmlNode fileNameNode = xmlMts.xmlDOM.SelectSingleNode("/MDG.Selections/Technology");
            //xmlMdg.loadFromFile(fileNameNode["filename"].InnerText);
            // ver si está incluido, si no lo está lo agregamos

            XmlNode modelTemplates = xmlMts.xmlDOM.SelectSingleNode("/MDG.Selections/ModelTemplates");

            if (modelTemplates == null)
            {
                XmlElement child = xmlMts.xmlDOM.CreateElement("ModelTemplates");
                modelTemplates = xmlMts.xmlDOM.DocumentElement.AppendChild(child);
            }
            else
            {
                modelTemplates.RemoveAll();
            }

            bool hasFramework = false;
            foreach (TemplateInfo templateInfo in templatesInfo)
            {
                addTemplate(templateInfo, modelTemplates);

                if (!hasFramework && templateInfo.isFramework == "true")
                {
                    hasFramework = true;
                }
            }

            this.addProfile(profilesInfo, "Profiles");

            this.addProfile(diagramsInfo, "DiagramProfile");

            this.addProfile(toolboxesInfo, "UIToolboxes");

            this.addProfile(patternsInfo, "Patterns");

            incrementarVersion();

            XmlNode fileNameNode = xmlMts.xmlDOM.SelectSingleNode("/MDG.Selections/Technology");
            fileNameNode.Attributes["version"].Value = this.mtsElement.Version;

            xmlMts.xmlDOM.Save(mdgPackage.Element.Genfile);

            this.eaUtils.repository.GenerateMDGTechnology(mdgPackage.Element.Genfile);

            if (hasFramework)
            {
                // cargamos el mdg
                string xmlMdgFilename = fileNameNode.Attributes["filename"].InnerText;

                xmlMdg.loadFromFile(xmlMdgFilename);

                // una vez que se genera el xml del mdg lo abrimos para marcar como framework si es que hay alguno.
                // esto es porque a pesar de haberlo marcado así en el mts no le da bola.

                XmlAttribute xmlAttr;
                // sabemos que hay al menos 1 framework, no sabemos cual o cuales.
                foreach (TemplateInfo templateInfo in templatesInfo)
                {
                    if (templateInfo.isFramework == "true")
                    {
                        xmlAttr = xmlMdg.xmlDOM.CreateAttribute("isFramework");
                        xmlAttr.Value = "true";
                        xmlMdg.xmlDOM.SelectSingleNode("/MDG.Technology/ModelTemplates/Model[@location='" + templateInfo.location + "']").Attributes.Append(xmlAttr);
                    }
                }

                if (quicklinker != null && quicklinker != "")
                {
                    XmlNode content = xmlMdg.xmlDOM.SelectSingleNode("/MDG.Technology/UMLProfiles/UMLProfile/Content");

                    XmlElement quickNode = xmlMdg.xmlDOM.CreateElement("QuickLink");
                    XmlAttribute quickAttr = xmlMdg.xmlDOM.CreateAttribute("data");
                    quickAttr.Value = quicklinker;
                    quickNode.Attributes.Append(quickAttr);
                    content.AppendChild(quickNode);
                }

                // guardamos el xml del mdg.
                xmlMdg.xmlDOM.Save(xmlMdgFilename);
            }

            Alert.Success(Properties.Resources.MDG_BUILD_OK);
        }

        internal void makeProfile(Package profilePackage)
        {
            if (profilePackage.Elements.Count != 0)
            {
                if (profilePackage.Element.Stereotype == "profile")
                {
                    if( checkGenFile(profilePackage, "elements_" + (profilesInfo.files.Count + 1)) )
                    {
                        try
                        {
                            this.eaUtils.repository.ActivateDiagram(profilePackage.Diagrams.GetAt(0).DiagramID);
                        }
                        catch (Exception) { }

                        this.eaUtils.repository.SavePackageAsUMLProfile(profilePackage.PackageGUID, profilePackage.Element.Genfile);

                        // el xml que guarda contiene en el tag Documentation nombre y notas que no están bien.
                        XmlUtils xml = new XmlUtils();

                        xml.loadFromFile(profilePackage.Element.Genfile);

                        XmlNode node = xml.xmlDOM.SelectSingleNode("/UMLProfile/Documentation");

                        node.Attributes["name"].Value = mdgName;
                        node.Attributes["notes"].Value = profilePackage.Notes;
                        try
                        {
                            if (node.Attributes.GetNamedItem("alias") != null)
                            {
                                node.Attributes["alias"].Value = this.mdgAlias;
                            }
                        }
                        catch (Exception e)
                        {
                            Clipboard.SetText(e.Message);

                            this.eaUtils.printOut( node.Attributes.ToString() );
                        }

                        agregarAtributosAlEstereotipo(node, profilePackage, xml.xmlDOM);

                        xml.xmlDOM.Save(profilePackage.Element.Genfile);

                        profilesInfo.addFromGenfile(profilePackage.Element.Genfile);
                    }

                }
                else if (profilePackage.Element.Stereotype == "diagram profile")
                {
                    if(profilePackage.Diagrams.Count != 0)
                    {
                        if (checkGenFile(profilePackage, "diagrams_" + (diagramsInfo.files.Count + 1)))
                        {
                            this.eaUtils.repository.SaveDiagramAsUMLProfile(profilePackage.Diagrams.GetAt(0).DiagramGUID, profilePackage.Element.Genfile);

                            XmlUtils xml = new XmlUtils();

                            xml.loadFromFile(profilePackage.Element.Genfile);

                            XmlNode node = xml.xmlDOM.SelectSingleNode("/UMLProfile/Documentation");

                            node.Attributes["name"].Value = mdgName;
                            node.Attributes["notes"].Value = profilePackage.Notes;
                            try
                            {
                                node.Attributes["alias"].Value = this.mdgAlias;

                            }
                            catch (Exception e)
                            {
                                Clipboard.SetText(e.Message);

                                this.eaUtils.printOut(node.Attributes.ToString());
                            }

                            xml.xmlDOM.Save(profilePackage.Element.Genfile);

                            diagramsInfo.addFromGenfile(profilePackage.Element.Genfile);

                        }
                    }
                }
                else if (profilePackage.Element.Stereotype == "toolbox profile")
                {
                    syncToolbox(profilePackage);

                    if (checkGenFile(profilePackage, "toolbox_" + (toolboxesInfo.files.Count + 1)))
                    {
                        this.eaUtils.repository.SavePackageAsUMLProfile(profilePackage.PackageGUID, profilePackage.Element.Genfile);

                        XmlUtils xml = new XmlUtils();

                        xml.loadFromFile(profilePackage.Element.Genfile);

                        XmlNode node = xml.xmlDOM.SelectSingleNode("/UMLProfile/Documentation");

                        node.Attributes["name"].Value = mdgName;
                        node.Attributes["notes"].Value = profilePackage.Notes;
                        try
                        {
                            if( node.Attributes.GetNamedItem("alias") != null )
                            {
                                node.Attributes["alias"].Value = this.mdgAlias;
                            }
                        }
                        catch (Exception e)
                        {
                            Clipboard.SetText(e.Message);

                            this.eaUtils.printOut(node.Attributes.ToString());
                        }

                        xml.xmlDOM.Save(profilePackage.Element.Genfile);

                        toolboxesInfo.addFromGenfile(profilePackage.Element.Genfile);
                    }
                }
            }
        }

        private void agregarAtributosAlEstereotipo(XmlNode node, Package profile, XmlDocument xmlDom)
        {
            XmlNode nodeStereotype;
            XmlAttribute xmlAttr;
            ElementStyle style;

            foreach ( Element element in profile.Elements)
            {
                if( ! element.Name.Contains("::") && element.Stereotype == "stereotype")
                {
                    nodeStereotype = node.SelectSingleNode("/UMLProfile/Content/Stereotypes/Stereotype[@name='" + element.Name + "']");

                    // "/UMLProfile/Stereotypes/Stereotype[@name=\""+ element.Name +"\"]"

                    style = this.eaUtils.getDefaultStyle(element);

                    xmlAttr = xmlDom.CreateAttribute("bgcolor");

                    xmlAttr.Value = style.getBackColor();

                    nodeStereotype.Attributes.Append(xmlAttr);

                    xmlAttr = xmlDom.CreateAttribute("fontcolor");

                    xmlAttr.Value = style.getFontColor();

                    nodeStereotype.Attributes.Append(xmlAttr);

                    xmlAttr = xmlDom.CreateAttribute("bordercolor");

                    xmlAttr.Value = style.getBorderColor();

                    nodeStereotype.Attributes.Append(xmlAttr);

                    xmlAttr = xmlDom.CreateAttribute("borderwidth");

                    xmlAttr.Value = style.getBorderWidth();

                    nodeStereotype.Attributes.Append(xmlAttr);

                    xmlAttr = xmlDom.CreateAttribute("hideicon");

                    xmlAttr.Value = "0";

                    nodeStereotype.Attributes.Append(xmlAttr);

                    if ( element.Attributes.Count != 0)
                    {
                        // buscamos los atributos _sizeX y _sizeY
                        // cx="0" cy="0"
                        foreach (EA.Attribute attr in element.Attributes)
                        {
                            if (attr.Name == "_sizeX")
                            {
                                xmlAttr = xmlDom.CreateAttribute("cx");

                                xmlAttr.Value = attr.Default;

                                nodeStereotype.Attributes.Append(xmlAttr);
                            }
                            else if (attr.Name == "_sizeY")
                            {
                                xmlAttr = xmlDom.CreateAttribute("cy");

                                xmlAttr.Value = attr.Default;

                                nodeStereotype.Attributes.Append(xmlAttr);
                            }
                        }
                    }
                }
            }
        }

        internal bool checkGenFile(Package package, string profileSufijo )
        {
            if (package.Element.Genfile == "")
            {
                SaveFileInfo info = new SaveFileInfo();

                info.fileName(this.mdgName +"_"+ (package.Alias != "" ? package.Alias : profileSufijo));

                if (this.chooseFileNameXML(info, "Elegir directorio y nombre del profile"))
                {
                    package.Element.Genfile = info.fileName();
                    package.Update();
                }
            }
            return package.Element.Genfile != "";
        }

        internal void incrementarVersion()
        {
            if( semanticVersioning.useSemanticVersioning )
            {
                if( ! semanticVersioning.isInitialized )
                {
                    semanticVersioning.parseVersion(this.mtsElement.Version);

                    semanticVersioning.incrementPatch();

                    this.mtsElement.Version = semanticVersioning.buildString();
                }
            }
            else
            {
                string[] versionSplitted = this.mtsElement.Version.Split('.');

                int lastVersion = 0;
                string currentValue;

                for (int i = versionSplitted.Length - 1; i > 0; i--)
                {
                    currentValue = versionSplitted[i];

                    bool res = int.TryParse(currentValue, out lastVersion);
                    if (res)
                    {
                        lastVersion += 1;

                        versionSplitted[i] = lastVersion.ToString();
                        break;
                    }
                }

                this.mtsElement.Version = String.Join(".", versionSplitted);
            }

            this.mtsElement.Update();

            // lanza error
            //this.eaUtils.repository.ImportTechnology(mdgName);
        }

        internal void addProfile(GenericProfileInfo profile, string tagName)
        {
            if ( profile.files.Count != 0)
            {
                XmlElement profileElement = xmlMts.xmlDOM.CreateElement(tagName);

                XmlAttribute directoryAttr = xmlMts.xmlDOM.CreateAttribute("directory");
                directoryAttr.Value = profile.directory;

                XmlAttribute filesAttr = xmlMts.xmlDOM.CreateAttribute("files");
                filesAttr.Value = string.Join(",", profile.files);

                profileElement.Attributes.Append(directoryAttr);
                profileElement.Attributes.Append(filesAttr);

                xmlMts.xmlDOM.SelectSingleNode("/MDG.Selections").AppendChild(profileElement);
            }
        }

        internal void addTemplate(TemplateInfo templateInfo, XmlNode modelTemplates)
        {
            XmlElement child = xmlMts.xmlDOM.CreateElement("Model");

            child.SetAttribute("name", templateInfo.name);
            child.SetAttribute("location", templateInfo.location);
            child.SetAttribute("default", "");
            child.SetAttribute("icon", "34");
            child.SetAttribute("filter", "");
            child.SetAttribute("isFramework", templateInfo.isFramework);

            modelTemplates.AppendChild(child);
        }

        public void savePackage(Package package)
        {
            string name       = package.Name.ToUpper();
            string parentName = parentPackage.Name.ToUpper();

            string quienSoVo = name.IndexOf("PATTERN") != -1 || parentName.IndexOf("PATTERNS") != -1 ? "PATTERN" :
                (name.IndexOf("TEMPLATE") != -1 || parentName.IndexOf("TEMPLATES") != -1 ? "TEMPLATE" :
                (name.IndexOf("FRAMEWORK") != -1 || parentName.IndexOf("FRAMEWORKS") != -1 ? "FRAMEWORK" : ""));
            bool teDoyBola = quienSoVo != "";

            this.eaUtils.printOut("Analizando paquete " + package.Name);

            if (teDoyBola)
            {
                // libreamos el genFile porque en los templates se puede requerrir su uso, lo pasamos a valor etiquetado del package parent.

                Package parentPackage = this.eaUtils.repository.GetPackageByID(package.ParentID);

                string flieNameTaggedValueName = TV__GENFILE_PREFIX + parentName + "_" + package.Name;

                string genFile = this.eaUtils.taggedValuesUtils.get(parentPackage.Element, flieNameTaggedValueName, "").asString();

                if (genFile == "")
                {
                    SaveFileInfo info = new SaveFileInfo();

                    info.filter(quienSoVo + " .xml (*.xml)|*.xml");

                    info.fileName(this.mdgName.Replace(" ", "_") + "_" + quienSoVo + "_" + (package.Alias != "" ? package.Alias : package.Name));

                    if (quienSoVo == "PATTERN" && this.chooseFileNameXML(info))
                    {
                        genFile = info.fileName();
                    }
                    else
                    {
                        // template y frameworks tienen que ir al mismo lugar que el mdg.
                        FileInfo fi = new FileInfo(this.mdgPackage.Element.Genfile);
                        
                        genFile = fi.Directory.FullName +"\\"+ info.fileName();                        
                    }

                    this.eaUtils.taggedValuesUtils.set(parentPackage.Element, flieNameTaggedValueName, genFile);

                    package.Update();
                }

                if (genFile != "")
                {
                    if (quienSoVo == "PATTERN")
                    {
                        EA.Project projectInterface;
                        projectInterface = this.eaUtils.repository.GetProjectInterface();
                        projectInterface.ExportPackageXMIEx(package.PackageGUID, EA.EnumXMIType.xmiEA11, 2, 3, 1, 0, genFile, 1);

                        if (patternsInfo == null)
                        {
                            patternsInfo = new GenericProfileInfo();
                            patternsInfo.directory = Path.GetDirectoryName(genFile);
                        }

                        // este xml exige un poco de laburo, hay que quitar del patrón el paquete y agregar tags.
                        // vamos a asignar esta responsabilidad a una clase
                        DiagramPatternXmlUtil diagramPatternXmlUtil = new DiagramPatternXmlUtil();

                        diagramPatternXmlUtil.build(genFile, package);

                        patternsInfo.addFromGenfile(genFile);

                        // un pattern no se visibiliza sino hay una caja de herramientas, por lo tanto lo generamos sino existe.
                        this.diagramPatternToolboxHelper.verificar(mdgPackage, package, genFile);
                    }
                    else if (quienSoVo == "TEMPLATE")
                    {
                        EA.Project projectInterface;
                        projectInterface = this.eaUtils.repository.GetProjectInterface();
                        
                        projectInterface.ExportPackageXMI(package.PackageGUID, EA.EnumXMIType.xmiEADefault, 2, 3, 0, 0, genFile);

                        TemplateInfo info = new TemplateInfo();
                        info.name = package.Name;
                        info.description = package.Notes != "" ? package.Notes : package.Name;
                        info.location = Path.GetFileName(genFile);
                        // obtener location

                        templatesInfo.Add(info);
                    }

                    else if (quienSoVo == "FRAMEWORK")
                    {
                        EA.Project projectInterface;
                        projectInterface = this.eaUtils.repository.GetProjectInterface();

                        projectInterface.ExportPackageXMI(package.PackageGUID, EA.EnumXMIType.xmiEADefault, 2, 0, 0, 0, genFile);

                        TemplateInfo info = new TemplateInfo();

                        info.name = package.Name;
                        info.description = package.Notes != "" ? package.Notes : package.Name;
                        info.location = Path.GetFileName(genFile);
                        info.isFramework = "true";

                        templatesInfo.Add(info);
                    }
                }
            }
        }

        public bool syncToolbox(Package package)
        {
            // obtener los elementos con estereotipo stereotype
            foreach (Element element in package.Elements)
            {
                if (element.Persistence == "Persistent" || element.Persistence == "Persistente")
                {
                    continue;
                }

                // se trata de la definicion de una caja de herramientas.
                if ( ! element.Name.Contains("::") && element.Stereotype == "stereotype")
                {
                    // eliminamos todos los atributos para evitar que quede alguno que ya no esta mas.
                    for (short i = 0; i < element.Attributes.Count; i++)
                    {
                        element.Attributes.Delete(i);
                    }

                    element.Attributes.Refresh();

                    // dos modos de agregar elementos a las cajas, por instancias dentro del elemento o por relaciones de dependencias.
                    // ambas pueden convivir en el mismo elemento.
                    // la diferencia es la facilidad de ordenar los elementos en la caja, en las relaciones el órden lo da la fecha y hora
                    // de la creación de la relación y si hay que reordenar es complicado, en el caso de los elementos se toma el órden
                    // que tienen en el explorador.

                    syncTooboxAttributes(element);
                    addToolboxAttributes(element);
                }
            }
            return package.Elements.Count != 0;
        }

        public void addToolboxAttributes(Element element)
        {
            EA.Element stereotype;

            foreach (Element stereotypeInstance in element.Elements)
            {
                if (stereotypeInstance.ClassfierID != 0)
                {
                    stereotype = this.eaUtils.repository.GetElementByID(stereotypeInstance.ClassfierID);

                    this.addProfileToToolbox(element, stereotype, stereotypeInstance.TreePos);
                }
            }
            element.Attributes.Refresh();
        }

        public void syncTooboxAttributes(Element element)
        {
            EA.Element profileElement;
            profileElement = null;
            Package patternPackage = null;

            foreach (Connector connector in element.Connectors)
            {
                if (connector.Type != "Dependency")
                {
                    continue;
                }

                try
                {
                    profileElement = this.eaUtils.repository.GetElementByID(connector.SupplierID);
                    patternPackage = this.eaUtils.repository.GetPackageByGuid(profileElement.ElementGUID);

                    //patternPackage = this.eaUtils.repository.GetPackageByID(connector.SupplierID);

                    this.addProfileToToolbox(element, patternPackage);

                } catch (Exception) {

                    profileElement = this.eaUtils.repository.GetElementByID(connector.SupplierID);

                    this.addProfileToToolbox(element, profileElement, -1);
                }
            }

            element.Attributes.Refresh();

            element.Refresh();
        }

        private void addProfileToToolbox(Element toolboxElement, Package pattern)
        {
            EA.Attribute attr;
            string attrName;

            attrName = mdgName + "::" + pattern.Name + "(UMLPatternSilent)";
            attr = toolboxElement.Attributes.AddNew(attrName, "");
            attr.Default = (pattern.Alias != "" ? pattern.Alias : pattern.Name);
            attr.Update();
        }

        private void addProfileToToolbox(Element toolboxElement, Element profileElement, int treePos)
        {
            EA.Element metaclassElement;
            EA.Attribute attr;
            string attrName;
            string metaclassName;

            this.modelUtil.reset();

            metaclassElement = this.modelUtil.getMetaClass(profileElement, mdgName);
            StereotypeInfo sourceInfo = this.modelUtil.info;
            metaclassElement = metaclassElement == null ? sourceInfo.deepTypeAsElement() : metaclassElement;

            if (metaclassElement != null)
            {
                if (metaclassElement.Stereotype.ToLower() == "metaclass")
                {
                    metaclassName = "UML::" + metaclassElement.Name;
                }
                else
                {
                    //if(metaclassElement.Alias == "")
                    //{
                        metaclassName = "UML::" + metaclassElement.Tag;
                    /*
                    }
                    else
                    {
                        metaclassName = "UML::" + metaclassElement.Alias;
                    }
                    */
                }

                attrName = mdgName + "::" + profileElement.Name + "(" + metaclassName + ")";
                attr = toolboxElement.Attributes.AddNew(attrName, "");
                attr.Default = (profileElement.Alias != "" ? profileElement.Alias : profileElement.Name);
                if (treePos != -1)
                {
                    attr.Pos = treePos;
                }
                attr.Update();
            }
        }

        internal bool selectFileNameMTS(SaveFileInfo info)
        {
            info.defaultExtension("mts");
            info.filter("Metafile (*.MTS)|*.mts");

            ChooseTarget2Save chooser = new ChooseTarget2Save();
            return chooser.choose(info, "Elegir destino y nombre del MTS ");
        }

        internal bool chooseFileNameXML(SaveFileInfo info)
        {
            return this.chooseFileNameXML(info, "Elegir destino y nombre del MDG");
        }

        internal bool chooseFileNameXML(SaveFileInfo info, string title)
        {
            info.defaultExtension( "xml" );
            info.filter("Profile uml (*.XML)|*.xml");
            
            ChooseTarget2Save chooser = new ChooseTarget2Save();
            return chooser.choose(info, title);
        }
    }
}
