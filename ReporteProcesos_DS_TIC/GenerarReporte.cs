using EA;
using EAUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using static EAUtils.ConnectorUtils;

namespace ReporteProcesos_DS_TIC
{
    public class GenerarReporte
    {
        private const string FILE_DIALOG__TITLE = "Destino del reporte";

        private const string TEMPLATE_ESTILOS = "";
        private const string TEMPLATE_CARATULA = "DS_TIC_PC_Caratula";
        //private const string TEMPLATE_CARATULA = "DS_TIC_PC_CARATULA_2";
        
        private const string INTRODUCCION_TEMPLATE = "DS_TIC_PC_Introduccion";
        private const string REVISION_TEMPLATE = "DS_TIC_PC_Revisiones";
        private const string REVISION_ITEMS_TEMPLATE = "DS_TIC_PC_Revisiones_Items";
        private const string REFERENCIAS_DOCUMENTALES_TEMPLATE = "DS_TIC_PC_REF_DOC";
        private const string FIRMAS_TEMPLATE = "DS_TIC_PC_Firmas";
        private const string MATRIZ_ROLES_RESPONSABILIDADES_TEMPLATE = "DS_TIC_PC_Matriz_RR";
        private const string DIAGRAMA_PROCESO_TEMPLATE = "DS_TIC_PC_Diagrama_Proceso";
        private const string DIAGRAMA_PROCESO_DIAGRAMA_TEMPLATE = "DS_TIC_PC_Diagrama_Proceso_Diagrama";
        private const string MATRIZ_TEMPLATE = "DS_TIC_PC_Matriz";
        private List<Element> insumos = new List<Element>();
        private List<Element> productos = new List<Element>();
        private static string ACTIVIDAD_TEMPLATE = "DS_TIC_PC_Actividad";
        private static string TAREA_TEMPLATE = "DS_TIC_PC_Tarea";
        private static string TAREA_ROLES_TEMPLATE = "DS_TIC_PC_Tarea_Roles";
        private static string PASOS_TEMPLATE = "DS_TIC_PC_Pasos";
        private static string PASO_TEMPLATE = "DS_TIC_PC_Paso";
        private static string PRODUCTOS_TRABAJO_TEMPLATE = "DS_TIC_PC_ProductosTrabajo";
        private static string PRODUCTOS_TRABAJO_ENTRADAS_TEMPLATE = "DS_TIC_PC_ProductosTrabajo_Entradas";
        private static string PRODUCTOS_TRABAJO_SALIDAS_TEMPLATE = "DS_TIC_PC_ProductosTrabajo_Salidas";
        private static string DIAGRAMAS_PROCESOS_TEMPLATE = "DS_TIC_PC_Diagramas_Procesos";

        private static string TIPO_ARCHIVO_REPORTE_KEY = "Tipo archivo reporte";

        EAUtils.EAUtils eaUtils;
        Package package; // package documentación
        Package procesosPackage;
        Diagram procesosDiagram;
        DocumentGenerator documentGenerator = null;
        string fileName = null;
        Element referenciasDocumentalesElement;
        Element firmasElement;
        Element procesoElement;
        string tipoArchivo;
        CheckListUtil opciones;
        List<Element> tareasOrdenadas;
        List<Element> actividadesOrdenadas;
        List<Element> pasosOrdenados;

        public GenerarReporte( Package package, EAUtils.EAUtils eaUtils)
        {
            this.eaUtils = eaUtils;
            this.package = package;

            this.procesosPackage = this.eaUtils.repository.GetPackageByID(package.ParentID);

            this.procesosDiagram = this.eaUtils.repository.GetDiagramByID(this.procesosPackage.Diagrams.GetAt(0).DiagramID);

            tipoArchivo = this.eaUtils.taggedValuesUtils.get(this.package.Element, TIPO_ARCHIVO_REPORTE_KEY, "pdf").asString();

            this.opciones = new CheckListUtil(this.package, this.eaUtils);

            if( this.opciones.findByStereotype())
            {
                this.opciones.parse();
            }

        }

        public void generar()
        {
            // elegir el directorio destino.
            if ( this.chooseDirectory() )
            {
                // instancia el motor de reportes.
                documentGenerator = this.eaUtils.repository.CreateDocumentGenerator();

                documentGenerator.SetProjectConstant("ReporteProcesosVersion", this.package.Version);

                if ( documentGenerator.NewDocument( TEMPLATE_ESTILOS ) )
                {
                    //documentGenerator.SetStyleSheetDocument("normal.rtf");
                    //documentGenerator.InsertTemplate("test_cabecera");

                    if ( TEMPLATE_CARATULA == "" || documentGenerator.InsertCoverPageDocument( TEMPLATE_CARATULA ) )
                    //if (TEMPLATE_CARATULA == "" || documentGenerator.InsertTemplate(TEMPLATE_CARATULA))
                    {

                        documentGenerator.InsertText("Tabla de Contenidos", "0");
                        documentGenerator.InsertTableOfContents();
                        documentGenerator.InsertBreak(DocumentBreak.breakPage);

                        writeRevisiones();

                        documentGenerator.InsertBreak(DocumentBreak.breakPage);

                        writeIntroduccion();

                        documentGenerator.InsertBreak(DocumentBreak.breakPage);

                        documentGenerator.DocumentPackage(this.package.PackageID, 0, REFERENCIAS_DOCUMENTALES_TEMPLATE);

                        documentGenerator.InsertLinkedDocument( this.referenciasDocumentalesElement.ElementGUID );

                        documentGenerator.InsertBreak(DocumentBreak.breakPage);

                        documentGenerator.InsertLinkedDocument(this.procesoElement.ElementGUID);

                        documentGenerator.DocumentPackage(this.package.PackageID, 0, MATRIZ_ROLES_RESPONSABILIDADES_TEMPLATE);

                        documentGenerator.InsertBreak(DocumentBreak.breakPage);

                        XmlUtils xmlUtils = new XmlUtils();

                        XmlNode xmlNode = xmlUtils.createReportFragmentDOM();

                        xmlUtils.createDOMElement("nombre_reporte", this.fileName, xmlNode);

                        documentGenerator.DocumentCustomData(xmlUtils.xml2String(), 0, DIAGRAMA_PROCESO_TEMPLATE);

                        documentGenerator.DocumentDiagram(this.procesosDiagram.DiagramID, 0, DIAGRAMA_PROCESO_DIAGRAMA_TEMPLATE);

                        documentGenerator.InsertBreak(DocumentBreak.breakPage);

                        // Según se puede interpretar al primer nivel lo llaman actividad
                        // al segundo nivel lo llaman tarea
                        // al tercer nivel lo llaman paso
                        // los roles están determinados en el nivel de tarea.
                        // las entradas y salidas están relacionadas con los pasos.

                        recorrerActividades( this.procesosDiagram.DiagramObjects);

                        documentGenerator.InsertBreak(DocumentBreak.breakPage);

                        documentGenerator.DocumentPackage(this.package.PackageID, 0, FIRMAS_TEMPLATE);
                        documentGenerator.InsertLinkedDocument(this.firmasElement.ElementGUID);

                        if ( documentGenerator.SaveDocument( this.fileName, ( this.tipoArchivo == "pdf" ? EA.DocumentType.dtPDF  : EA.DocumentType.dtDOCX ) ) )
                        {
                            this.eaUtils.printOut("Fin documento");
                            this.eaUtils.printOut("Inicia anexo");
                        }
                    }
                }
            }
        }

        private void writeRevisiones()
        {
            documentGenerator.DocumentPackage(package.PackageID, 0, REVISION_TEMPLATE);

            foreach( Element current in this.package.Elements)
            {
                if( current.Type == "Change")
                {
                    XmlUtils xmlUtils = new XmlUtils();

                    XmlNode xmlNode = xmlUtils.createReportFragmentDOM();

                    xmlUtils.createDOMElement("version", current.Version, xmlNode);
                    xmlUtils.createDOMElement("descripcion", current.Name + current.Notes, xmlNode);
                    xmlUtils.createDOMElement("autor", current.Author, xmlNode);
                    xmlUtils.createDOMElement("fecha", this.eaUtils.taggedValuesUtils.get(current, "Fecha de revisión", "").asString(), xmlNode);

                    string xml = xmlUtils.xml2String();

                    this.eaUtils.printOut(xml);

                    bool canWrite = documentGenerator.DocumentCustomData(xml, 0, REVISION_ITEMS_TEMPLATE);

                    this.eaUtils.printOut(current.Name);
                    if (canWrite)
                    {
                        this.eaUtils.printOut("Ok");
                    }else
                    {
                        this.eaUtils.printOut(":o(");
                    }
                }
                else if( current.Name == "Referencias documentales")
                {
                    this.referenciasDocumentalesElement = current;
                }
                else if (current.Name == "Firmas")
                {
                    this.firmasElement = current;
                }
                else if (current.Name == "Proceso")
                {
                    this.procesoElement = current;
                }
            }
        }

        private void writeIntroduccion()
        {

            documentGenerator.DocumentDiagram(this.procesosDiagram.DiagramID, 0, INTRODUCCION_TEMPLATE);
        }

        private void recorrerActividades( Collection diagramObjects )
        {
            actividadesOrdenadas = this.buscarYOrdenar(diagramObjects);

            foreach( Element activity in actividadesOrdenadas)
            {
                if( activity.IsComposite )
                {
                    documentGenerator.DocumentElement(activity.ElementID, 0, ACTIVIDAD_TEMPLATE);

                    if( opciones.isChecked("Intercalar diagramas de proceso"))
                    {
                        documentGenerator.DocumentElement(activity.ElementID, 0, DIAGRAMAS_PROCESOS_TEMPLATE);
                    }

                    // vamos al segundo nivel que es el de la tarea.
                    recorrerTareas(activity.CompositeDiagram.DiagramObjects);
                }
            }
        }

        private void recorrerTareas(Collection diagramObjects)
        {
            tareasOrdenadas = this.buscarYOrdenar(diagramObjects);

            foreach (Element tarea in tareasOrdenadas)
            {
                if (tarea.IsComposite)
                {
                    if (opciones.isChecked("Intercalar diagramas de proceso"))
                    {

                        documentGenerator.DocumentElement(tarea.ElementID, 0, DIAGRAMAS_PROCESOS_TEMPLATE);
                    }

                    documentGenerator.DocumentElement(tarea.ElementID, 0, TAREA_TEMPLATE );

                    // agregamos los roles que son los lanes.
                    roles(diagramObjects);

                    // buscamos los insumos y productos que están en el tercer nivel
                    insumosProductos(tarea.CompositeDiagram.DiagramObjects);

                    // vamos al 3er nivel, los pasos
                    pasos(tarea.CompositeDiagram.DiagramObjects);
                }
            }
        }

        private void roles( Collection diagramObjects)
        {
            Element element;
            List<string> roles = new List<string>();

            foreach( DiagramObject diagramObject in diagramObjects)
            {
                element = this.eaUtils.repository.GetElementByID(diagramObject.ElementID);

                if( element.Stereotype == "Lane")
                {
                    roles.Add(element.Name);
                }
            }

            XmlUtils xmlUtils = new XmlUtils();
            XmlNode xmlNode = xmlUtils.createReportFragmentDOM();

            xmlUtils.createDOMElement("roles", String.Join("\r\n", roles), xmlNode);

            this.documentGenerator.DocumentCustomData(xmlUtils.xml2String(), 0, TAREA_ROLES_TEMPLATE);
        }

        private void insumosProductos(Collection diagramObjects)
        {
            Element element = null;
            List<string> insumos = new List<string>();
            List<string> productos = new List<string>();

            foreach (DiagramObject diagramObject in diagramObjects)
            {
                element = this.eaUtils.repository.GetElementByID(diagramObject.ElementID);

                if (element.Stereotype == "DataObject")
                {
                    if( this.eaUtils.taggedValuesUtils.get(element, "dataInOut", "").asString() == "Input")
                    {
                        insumos.Add(element.Name);
                    }
                    else
                    {
                        productos.Add(element.Name);
                    }
                }
            }

            // no importa el elemento, sólo va para que ponga un título.
            this.documentGenerator.DocumentElement(element.ElementID, 0, PRODUCTOS_TRABAJO_TEMPLATE);

            // insumos - entradas
            XmlUtils xmlUtils = new XmlUtils();
            XmlNode xmlNode = xmlUtils.createReportFragmentDOM();

            xmlUtils.createDOMElement("entradas", String.Join("\r\n", insumos), xmlNode);

            this.documentGenerator.DocumentCustomData(xmlUtils.xml2String(), 0, PRODUCTOS_TRABAJO_ENTRADAS_TEMPLATE);

            // productos - salidas
            xmlUtils = new XmlUtils();
            xmlNode = xmlUtils.createReportFragmentDOM();

            xmlUtils.createDOMElement("salidas", String.Join("\r\n", productos), xmlNode);

            this.documentGenerator.DocumentCustomData(xmlUtils.xml2String(), 0, PRODUCTOS_TRABAJO_SALIDAS_TEMPLATE);
        }

        private void pasos(Collection diagramObjects)
        {
            documentGenerator.DocumentPackage(this.package.PackageID, 0, PASOS_TEMPLATE);

            pasosOrdenados = this.buscarYOrdenar(diagramObjects);

            foreach (Element element in pasosOrdenados)
            {
                documentGenerator.DocumentElement(element.ElementID, 0, PASO_TEMPLATE);
            }
        }

        private List<Element> buscarYOrdenar( Collection diagramObjects)
        {

            SortedDictionary<string, Element> sd = new SortedDictionary<string, Element>();

            Element element = null;
            // buscamos el inicio del proceso y mediante los conectores seguimos con cada proceso del 1er nivel
            foreach (DiagramObject diagramObject in diagramObjects)
            {
                element = this.eaUtils.repository.GetElementByID(diagramObject.ElementID);

                if (element.Stereotype == "Activity")
                {
                    sd.Add(element.Name, element);
                }
            }

            return sd.Values.ToList();
        }

        private bool chooseDirectory()
        {
            bool ok = false;

            string lastVersion = "";
            foreach (Element current in this.package.Elements)
            {
                if (current.Type == "Change")
                {
                    lastVersion = current.Version;
                }
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Title = FILE_DIALOG__TITLE;
            saveFileDialog1.FileName = this.package.Alias + "_v" + lastVersion;
            saveFileDialog1.DefaultExt = this.tipoArchivo;
            saveFileDialog1.CheckFileExists = false;
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if( saveFileDialog1.ShowDialog() == DialogResult.OK )
            {
                ok = true;
                fileName = saveFileDialog1.FileName;
            }
            return ok;
        }

        public void generarDiagramas()
        {
            documentGenerator = this.eaUtils.repository.CreateDocumentGenerator();

            documentGenerator.SetProjectConstant("ReporteProcesosVersion", this.package.Version);

            if (documentGenerator.NewDocument(TEMPLATE_ESTILOS))
            {
                recorrerActividadesDiagramas();

                if (documentGenerator.SaveDocument( (this.tipoArchivo == "pdf" ? this.fileName.Replace(".pdf", "-anexo.pdf") : this.fileName.Replace(".docx", "-anexo.docx")), (this.tipoArchivo == "pdf" ? EA.DocumentType.dtPDF : EA.DocumentType.dtDOCX)))
                {
                    this.eaUtils.printOut("Fin anexo");
                }
            }
        }

        private void recorrerActividadesDiagramas()
        {
            documentGenerator.DocumentDiagram(this.procesosDiagram.DiagramID, 0, DIAGRAMA_PROCESO_DIAGRAMA_TEMPLATE);

            foreach (Element activity in this.actividadesOrdenadas )
            {
                if (activity.IsComposite)
                {
                    documentGenerator.DocumentElement(activity.ElementID, 0, DIAGRAMAS_PROCESOS_TEMPLATE);

                    // vamos al segundo nivel que es el de la tarea.
                    recorrerTareasDiagramas();
                }
            }
        }

        private void recorrerTareasDiagramas()
        {
            foreach (Element tarea in tareasOrdenadas)
            {
                if (tarea.IsComposite)
                {
                    documentGenerator.DocumentElement(tarea.ElementID, 0, DIAGRAMAS_PROCESOS_TEMPLATE);
                }
            }

        }
    }
}
