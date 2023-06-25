using EA;
using EAUtils;
using MDGBuilder;
using MDGBuilder.mdg;
using MDGBuilder.ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UIResources;

namespace MDGBuilder
{
    public class Main : MainCommons
    {
        private QuickLinkerEditor quickLinkerEditor;
        private ui.RenameMDG renameMdgEditor = null;
        private SemanticVersioning semanticVersioning = new SemanticVersioning();

        public void EA_MenuClick(EA.Repository repository, string location, string menuName, string itemName)
        {
            if (itemName == Properties.Resources.ITEM_MENU__GENERAR)
            {
                generar(getMDGPackage(repository), repository);
            }
            else if (itemName == Properties.Resources.ITEM_MENU__CONFIGURAR)
            {
                renombrar(getMDGPackage(repository), repository);
            }
            else if (itemName == Properties.Resources.ITEM_MENU__ADD_METATYPE)
            {
                agregarMetatype(getProfilePackage(repository), repository);
            }
            else if (itemName == Properties.Resources.ITEM_MENU__GENERAR_QUICKLINKER)
            {
                generarQuicklinker(getQuicklinkerPackage(repository), repository);
            }
            else if (itemName == Properties.Resources.ITEM_MENU__EDITOR_QUICKLINKER)
            {
                showQuickLinkerEditor(repository);
            }
            else if (itemName == Properties.Resources.ITEM_MENU__IMPORTAR_MTS)
            {
                importarMts(getMDGPackage(repository), repository);
            }
            else if( itemName == Properties.Resources.ITEM_MENU__GENERAR_ENTREGABLE)
            {
                generarPaqueteEntregable(getMDGPackage(repository), repository);
            }
            else if (itemName == Properties.Resources.ITEM_MENU__EXPORT_REFERENCE_DATA)
            {
                new ExportarDatosReferencia(this.eaUtils).exportar();
            }
            else if(itemName == ITEM_MENU__SINCRONIZAR_ESTEREOTIPOS)
            {
                this.sincronizarEstereotipos();
            }
        }

        private void sincronizarEstereotipos()
        {
            EstructuraSolucion estructuraSolucion = new EstructuraSolucion(eaUtils);
            estructuraSolucion.establecer();

            StringBuilder builder = new StringBuilder();

            builder.AppendLine("!INC Local Scripts.EAConstants-JavaScript");
            builder.AppendLine("");
            builder.AppendLine("function main()");
            builder.AppendLine("{");
            builder.AppendLine("");
            foreach( Element e in estructuraSolucion.profileElements.Elements )
            {
                if( e.Stereotype == "stereotype")
                {
                    if( this.eaUtils.repository.SynchProfile(estructuraSolucion.mtsElement.Name, e.Name))
                    {
                        builder.AppendLine("    Repository.SynchProfile( "+ "\""+ estructuraSolucion.mtsElement.Name +"\", "+ "\""+ e.Name +"\" )" );
                        this.eaUtils.printOut(estructuraSolucion.mtsElement.Name + "::"+ e.Name + " sincronizado");
                    }
                    else
                    {
                        this.eaUtils.printOut(estructuraSolucion.mtsElement.Name + "::" + e.Name + " NO sincronizado");
                    }
                }
            }
            builder.AppendLine("    Session.Output('Finalizó la sincronización');");
            builder.AppendLine("}");
            builder.AppendLine("");
            builder.AppendLine("main();");

            estructuraSolucion.sincronizarEstereotiposScript.Notes = builder.ToString();
            estructuraSolucion.sincronizarEstereotiposScript.Update();
        }
        private void generarPaqueteEntregable(Package package, Repository repository)
        {
            new Packer(eaUtils).pack();
        }

        private void showQuickLinkerEditor(Repository repository)
        {
            if (this.quickLinkerEditor == null)
            {
                this.quickLinkerEditor = (QuickLinkerEditor)eaUtils.repository.AddTab(QuickLinkerEditor.NAME, "MDGBuilder.ui.QuickLinkerEditor");
            }

            this.quickLinkerEditor.eaUtils = this.eaUtils;

            eaUtils.activateTab(QuickLinkerEditor.NAME);
        }

        private void importarMts(Package mdgPackage, Repository repository)
        {

            new ImportFromMts(this.eaUtils, mdgPackage).import();
        }

        private void renombrar(Package mdgPackage, Repository repository)
        {
            if (mdgPackage != null)
            {
                try
                {
                    this.eaUtils = new EAUtils.EAUtils(repository);

                    if (this.renameMdgEditor == null)
                    {
                        this.renameMdgEditor = (RenameMDG)eaUtils.repository.AddWindow(RenameMDG.NAME, "MDGBuilder.ui.RenameMDG");
                        eaUtils.repository.ShowAddinWindow(RenameMDG.NAME);
                    }

                    this.renameMdgEditor.show(mdgPackage, semanticVersioning, this.eaUtils);

                }
                catch (Exception e)
                {
                    Clipboard.SetText(e.ToString());
                    Alert.Error("Error en el portapapeles");
                }
            }
        }
        private void generar(Package mdgPackage, Repository repository)
        {
            MDGRenamer renamer = new MDGRenamer();

            Element classElement = renamer.getMtsElement(mdgPackage);

            EA.Attribute useSemanticVersioningAttribute = EAUtils.AttributeUtils.get(classElement, "useSemanticVersioning");

            semanticVersioning.useSemanticVersioning = useSemanticVersioningAttribute.Default == "true";

            Builder builder = new Builder(mdgPackage, semanticVersioning, new EAUtils.EAUtils(repository));

            try
            {
                builder.build();
            }
            catch (Exception e)
            {
                Clipboard.SetText(e.ToString());
            }
        }

        private void generarQuicklinker(Package package, Repository repository)
        {
            QuickLinkerBuilder builder = new QuickLinkerBuilder(new EAUtils.EAUtils(repository));

            try
            {
                if (builder.build(package))
                {
                    Clipboard.SetText(builder.getLinesAsString());
                    Alert.Success("Quicklinker disponible en el portapapeles");
                }
            }
            catch (Exception e)
            {
                Clipboard.SetText(e.ToString());
            }

        }

        private void agregarMetatype(Package package, Repository repository)
        {
            //this.eaUtils = new EAUtils.EAUtils(repository);

            MDGUtil util = new MDGUtil();

            util.addMetatype(package);
        }
        /*
        public string EA_OnRetrieveModelTemplate( Repository repository, string modelName)
        {
            Clipboard.SetText(modelName);
            return "";
        }
        */
        public void EA_OnContextItemChanged(EA.Repository repository, String GUID, EA.ObjectType ot)
        {
            if (this.isProjectOpen(repository))
            {
                switch (ot)
                {
                    case EA.ObjectType.otConnector:

                        Connector connector = repository.GetContextObject();
                        if( connector.Type == "Dependency")
                        {
                            if( this.quickLinkerEditor != null)
                            {
                                try
                                {
                                    if (this.quickLinkerEditor.isChanged)
                                    {
                                        this.quickLinkerEditor.save();
                                    }
                                    this.quickLinkerEditor.show(connector);
                                }
                                catch(Exception e)
                                {
                                    Clipboard.SetText(e.ToString());
                                    Alert.Error("Se ha producido un error, disponible en el porta papeles");
                                }
                            }
                            
                        }
                       
                        break;
                    case EA.ObjectType.otPackage:

                        Package mainPackage = getMDGPackage(repository);
                        if( mainPackage != null)
                        {
                            this.renombrar(mainPackage, repository);
                        }
                        break;
                }
            }
        }
    }
}
