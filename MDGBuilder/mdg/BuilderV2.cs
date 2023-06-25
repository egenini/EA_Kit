using EA;
using EAUtils;
using EAUtils.saveUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MDGBuilder.mdg
{
    public class BuilderV2
    {
        const string TV__GENFILE_PREFIX = "GenFile-";

        public Package mdgPackage;
        public EAUtils.EAUtils eaUtils;
        public string mdgName;
        public string mdgAlias;

        EA.Project projectInterface;
        SemanticVersioning semanticVersioning;
        XmlUtils xmlMts = new XmlUtils();
        XmlUtils xmlMdg = new XmlUtils();
        MtsUtil mtsUtil;
        ModelUtil modelUtil;
        Element mtsElement = null;
        GenericProfileInfo patternsInfo = null;
        DiagramPatternToolboxHelper diagramPatternToolboxHelper;
        string quicklinker = null;

        public BuilderV2(Package mdgPackage, EAUtils.EAUtils eaUtils, SemanticVersioning semanticVersioning)
        {
            this.mdgPackage = mdgPackage;
            this.eaUtils = eaUtils;
            this.semanticVersioning = semanticVersioning;
            this.projectInterface = this.eaUtils.repository.GetProjectInterface();
            this.mtsUtil = new MtsUtil(xmlMts);

            this.mtsUtil.eaUtils = this.eaUtils;

            this.modelUtil = new ModelUtil(this.eaUtils);

            this.diagramPatternToolboxHelper = new DiagramPatternToolboxHelper(eaUtils);

            List<Object> result = this.eaUtils.packageUtils.getElementFromFilter(this.mdgPackage, "Class", null, null, true);

            if(result.Count > 0)
            {
                mtsElement = (Element)result[0];

                this.mdgName = mtsElement.Name;
                this.mdgAlias = mtsElement.Alias;
            }
        }

        public void build()
        {
            if(mtsElement!= null)
            {
                if (this.resolverArchivoMTS())
                {
                    if( prepararArchivoMTS() )
                    {
                        // primero se resuelve el paquete de patrones porque si estos no están en una caja de herramientas no se ven.
                        // si no existe la caja se creará, corresponde una caja por cada patrón.                        
                        resolverPatrones();

                        // el resto no requiere de un órden específico, pero vamos a seguir el siguiente:
                        // quickliner
                        this.resolverQuickLinker();
                        // profiles
                        this.resolverProfiles();
                        // diagramas
                        // cajas

                        // frameworks
                        // templates

                    }
                }
            }
        }

        internal void resolverQuickLinker()
        {
            Package package = this.eaUtils.packageUtils.getChildPackageByName(this.mdgPackage, "Quicklinker");

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
        internal void resolverProfiles()
        {
            List<object> packages = this.eaUtils.packageUtils.getChildPackagesByStereotype(mdgPackage, "profile");
            Package profile= null;

            foreach( object p in packages)
            {
                profile = (Package) p;


            }
        }

        internal bool prepararArchivoMTS()
        {
            if (! System.IO.File.Exists(mdgPackage.Element.Genfile) )
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
            return xmlMts.xmlDOM != null;
        }
        private bool resolverArchivoMTS()
        {
            if (mdgPackage.Element.Genfile == "")
            {
                // puede ser que exista un mts o bien que no.
                // vamos a buscar, si no hay vamos a tener que crear uno.
                // si lo encontramos vamos a abrirlo para agregar, quitar templates, cajas, diagramas, profiles, modificar versión,etc.
                SaveFileInfo info = new SaveFileInfo();

                info.fileName(this.mdgName);

                if (this.elegirNombreArchivoMTS(info))
                {
                    mdgPackage.Element.Genfile = info.fileName();
                    mdgPackage.Update();
                }
            }
            return mdgPackage.Element.Genfile != "";
        }

        internal bool elegirNombreArchivoMTS(SaveFileInfo info)
        {
            info.defaultExtension("mts");
            info.filter("Metafile (*.MTS)|*.mts");

            ChooseTarget2Save chooser = new ChooseTarget2Save();
            return chooser.choose(info, "Elegir destino y nombre del MTS ");
        }

        internal bool elegirNombreArchivoXML(SaveFileInfo info, string title)
        {
            info.defaultExtension("xml");
            info.filter("Profile uml (*.XML)|*.xml");

            ChooseTarget2Save chooser = new ChooseTarget2Save();
            return chooser.choose(info, title);
        }
        internal void resolverPatrones()
        {
            Package patterns = this.eaUtils.packageUtils.getChildPackageByName(mdgPackage, "Patterns");

            bool darBola = false;

            if (patterns != null)
            {
                foreach (Package pattern in patterns.Packages)
                {
                    // el paquete debe tener 1 diagrama con al menos 1 elemento.
                    if( pattern.Diagrams.Count > 0 )
                    {
                        foreach(Diagram diagram in pattern.Diagrams)
                        {
                            if( diagram.DiagramObjects.Count > 0 )
                            {
                                darBola = true;
                            }
                            break;
                        }
                    }
                    if( darBola )
                    {
                        string genFile = gestionarGenFile(pattern);
                        if (patternsInfo == null)
                        {
                            patternsInfo = new GenericProfileInfo();
                            patternsInfo.directory = Path.GetDirectoryName(genFile);
                        }

                        // este xml exige un poco de laburo, hay que quitar del patrón el paquete y agregar tags.
                        // vamos a asignar esta responsabilidad a una clase
                        DiagramPatternXmlUtil diagramPatternXmlUtil = new DiagramPatternXmlUtil();

                        diagramPatternXmlUtil.build(genFile, pattern);

                        patternsInfo.addFromGenfile(genFile);

                        this.diagramPatternToolboxHelper.verificar(mdgPackage, pattern, genFile);
                    }

                    darBola = false;
                }
            }
        }

        internal string gestionarGenFile(Package package)
        {
            string flieNameTaggedValueName = TV__GENFILE_PREFIX + "_" + package.Name;

            string genFile = this.eaUtils.taggedValuesUtils.get(package.Element, flieNameTaggedValueName, "").asString();

            if (genFile == "")
            {
                SaveFileInfo info = new SaveFileInfo();

                info.filter(package.Name + " .xml (*.xml)|*.xml");

                info.fileName(this.mdgName.Replace(" ", "_") + "_" + (package.Alias != "" ? package.Alias : package.Name));

                if (this.elegirNombreArchivoXML(info, "Elegir destino y nombre del patrón"))
                {
                    genFile = info.fileName();
                    this.eaUtils.taggedValuesUtils.set(package.Element, flieNameTaggedValueName, genFile);

                    package.Update();
                }
            }
            return genFile;
        }
    }
}
