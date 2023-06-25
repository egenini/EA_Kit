using EA;
using EAUtils;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xml;

namespace APQC
{
    public class Importer
    {
        EAUtils.EAUtils eaUtils;

        ExcelPackage excelPackage = null;

        Dictionary<string, List<string>> process2metrics = new Dictionary<string, List<string>>();
        Dictionary<string, Package> metricsCategories = new Dictionary<string, Package>();
        Dictionary<string, Element> metricsById       = new Dictionary<string, Element>();
        Dictionary<string, string> glossary = new Dictionary<string, string>();

        Dictionary<string, string> metricsCategoriesTranslated = new Dictionary<string, string>();

        Package rootPackage = null;
        Package processPackage = null;
        Package metricsPackage = null;

        CheckListUtil checkListUtil = null;

        private bool importMetricsOption = false;
        private bool importProcessesOption = false;

        FileInfo fileInfo = null;

        public Importer( EAUtils.EAUtils eaUtils)
        {
            this.eaUtils = eaUtils;
        }

        internal void smartImport(Package package)
        {
            this.rootPackage = package;

            Elaptime elaptime = new Elaptime();

            if ( checkPackages() )
            {
                makeOptions(package);

                if ( this.rootPackage.Element.Genfile == "")
                {
                    if ((fileInfo = this.getPCFFile()) != null)
                    {
                        this.rootPackage.Element.Genfile = fileInfo.FullName;
                        this.rootPackage.Update();
                    }
                }
                else
                {
                    fileInfo = new FileInfo(this.rootPackage.Element.Genfile);
                }

                if( fileInfo != null)
                {
                    // cargando planilla
                    this.eaUtils.printOut(Properties.Resources.LOADING_SHEET + " " + fileInfo.FullName);

                    this.excelPackage = new ExcelPackage(fileInfo);

                    // cargando y analizando la opciones de traducción
                    this.eaUtils.printOut(Properties.Resources.LOADING_TRANSLATE_OPTIONS);
                    
                    // cargando las métricas desde el modelo.
                    this.eaUtils.printOut(Properties.Resources.MESSAGE_LOAD_METRICS);

                    loadMetrics();

                    if (this.importMetricsOption)
                    {
                        // importando métricas
                        this.eaUtils.printOut(Properties.Resources.IMPORT_METRICS);

                        importMetrics();
                    }
                    else
                    {
                        // no se importan métricas
                        this.eaUtils.printOut(Properties.Resources.NO_IMPORT_METRICS);
                    }

                    if (this.importProcessesOption)
                    {
                        // obteniendo lista de procesos a importar
                        checkListUtil = new CheckListUtil(this.rootPackage, eaUtils);
                        checkListUtil.findByStereotype();
                        checkListUtil.parse();

                        // importando y/o traduciendo procesos
                        this.eaUtils.printOut(Properties.Resources.IMPORT_PROCESSES);

                        importProcesses();
                    }
                    else
                    {
                        // no se importan procesos.
                        this.eaUtils.printOut(Properties.Resources.NO_IMPORT_PROCESSES);
                    }
                }

                this.eaUtils.printOut(elaptime.stop());
            }
        }

        public FileInfo getPCFFile()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "APQC xlsx|*.xlsx";
            openFileDialog1.Title = "Select APQC process file";

            FileInfo info = null;

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                info = new FileInfo(openFileDialog1.FileName);
            }
            return info;
        }
        /// <summary>
        /// Infiere que es lo que quiere importar según el paquete desde el cual se ejecutó la opción.
        /// </summary>
        void makeOptions(Package pacakageSelected)
        {
            if (pacakageSelected.Element.Stereotype == "metrics_pkg")
            {
                this.importMetricsOption   = true;
                this.importProcessesOption = false;
            }
            else if (pacakageSelected.Element.Stereotype == "process_pkg")
            {
                this.importMetricsOption   = false;
                this.importProcessesOption = true;
            }
            else
            {
                this.importMetricsOption   = true;
                this.importProcessesOption = true;
            }
        }

        internal void importProcesses()
        {
            ProcessImporter processImporter = new ProcessImporter(processPackage, process2metrics, metricsById, this.eaUtils);

            string worksheetName;
            string processId;

            Elaptime elaptime = new Elaptime();

            foreach (ExcelWorksheet worksheet in excelPackage.Workbook.Worksheets)
            {
                int rowNum = 2;
                try
                {
                    worksheetName = worksheet.Name.Trim();

                    if (checkListUtil.exist(worksheetName) && checkListUtil.isChecked(worksheetName))
                    {
                        this.eaUtils.printOut(worksheet.Name +" "+ worksheet.Cells.Count() );


                        while (true)
                        {
                            if( worksheet.Cells.Count() == rowNum)
                            {
                                processImporter.import(worksheet, rowNum);
                                break;
                            }
                            else
                            {
                                processId = worksheet.Cells["A" + rowNum].Value.ToString().Trim();

                                this.eaUtils.printOut("Row " + rowNum + " processId " + processId);

                                if (processId == "")
                                {
                                    break;
                                }
                                else
                                {
                                    processImporter.import(worksheet, rowNum);
                                }
                            }
                            rowNum++;
                        }
                        this.eaUtils.printOut("Fin "+ worksheet.Name);
                    }
                }
                catch( Exception e)
                {
                    processImporter.import(worksheet, rowNum);
                    System.Windows.Clipboard.SetText(e.ToString());
                }
            }
            this.eaUtils.printOut(elaptime.stop());
        }

        void loadMetrics()
        {
            string categoryName;

            Elaptime elaptime = new Elaptime();

            foreach ( Package categoriesPackage in metricsPackage.Packages)
            {
                categoryName = this.eaUtils.taggedValuesUtils.get(categoriesPackage.Element, "translation_Name_en", "").asString();

                metricsCategories.Add(categoryName == "" ? categoriesPackage.Name : categoryName, categoriesPackage);

                foreach( Element metric in categoriesPackage.Elements)
                {
                    metricsById.Add(metric.Alias, metric);

                    if( ! process2metrics.ContainsKey( metric.Tag ) )
                    {
                        process2metrics.Add(metric.Tag, new List<string>() );
                    }

                    process2metrics[metric.Tag].Add( metric.Alias );
                }
            }
            this.eaUtils.printOut(elaptime.stop());
        }

        void importMetrics()
        {
            // primero levantar las métricas
            string processId;
            string metricsCategory;
            Package metricPackage = null;
            Element metricElement = null;
            string metricName;
            string metricId;
            string formula;
            string units;

            Elaptime elaptime = new Elaptime();

            foreach (ExcelWorksheet worksheet in excelPackage.Workbook.Worksheets)
            {
                Diagram diagram;

                if (worksheet.Name == "Metrics")
                {
                    int rowNum = 3;
                    while (true)
                    {
                        try
                        {
                            processId = worksheet.Cells["A" + rowNum].Value.ToString().Trim();

                            this.eaUtils.printOut("Row " + rowNum + " processId " + processId);

                            if (processId == "")
                            {
                                break;
                            }
                            else
                            {
                                metricsCategory = worksheet.Cells["D" + rowNum].Value.ToString();
                                metricId = worksheet.Cells["E" + rowNum].Value.ToString();
                                metricName = worksheet.Cells["F" + rowNum].Value.ToString();
                                formula = worksheet.Cells["G" + rowNum].Value.ToString();
                                units = worksheet.Cells["H" + rowNum].Value.ToString();

                                if (!metricsCategories.ContainsKey(metricsCategory))
                                {
                                    metricPackage = metricsPackage.Packages.AddNew(metricsCategory, "");
                                    metricPackage.Update();

                                    if( metricPackage.Diagrams.Count == 0)
                                    {
                                        diagram = metricPackage.Diagrams.AddNew(metricPackage.Name, "Requirement");
                                        diagram.Update();
                                        diagram.ShowAsElementList(true, true);
                                        diagram.Update();
                                        metricPackage.Diagrams.Refresh();
                                    }

                                    metricsCategories.Add(metricPackage.Name, metricPackage);
                                }
                                else
                                {
                                    if (metricsCategories.ContainsKey(metricsCategory))
                                    {
                                        metricPackage = metricsCategories[metricsCategory];
                                    }
                                }

                                if (!metricsById.ContainsKey(metricId))
                                {
                                    metricElement = metricPackage.Elements.AddNew(metricName, "Requirement");
                                    metricElement.Stereotype = "APQC-EA::PCF_Metric";
                                    metricElement.Notes = "Formula : \r\n" + formula + "\r\n\r\nUnits:\r\n" + units;
                                    metricElement.Tag = processId;

                                    metricElement.Alias = metricId;

                                    metricElement.Update();

                                    diagram = metricPackage.Diagrams.GetAt(0);
                                    DiagramObject diagramObject = diagram.DiagramObjects.AddNew(metricElement.Name, metricElement.Type);
                                    diagramObject.ElementID = metricElement.ElementID;
                                    diagramObject.Sequence = 1;
                                    diagramObject.Update();

                                    metricsById.Add(metricId, metricElement);
                                }
                                else
                                {
                                    metricElement = metricsById[metricId];
                                }

                                if (!process2metrics.ContainsKey(processId))
                                {
                                    process2metrics.Add(processId, new List<string>());
                                }

                                process2metrics[processId].Add(metricId);
                            }
                        }catch(Exception)
                        {
                            break;
                        }
                        rowNum++;
                    }

                    // configuramos los diagramas
                    /*
                    foreach( KeyValuePair<string, Package> kv in metricsCategories)
                    {
                        diagram = kv.Value.Diagrams.GetAt(0);
                        diagram.ShowAsElementList(true, false);
                    }
                    */
                    this.eaUtils.printOut("Fin métricas");
                    break;
                }
            }
            this.eaUtils.printOut(elaptime.stop());
        }

        bool checkPackages()
        {
            if( this.rootPackage.Element.Stereotype == "metrics_pkg" || this.rootPackage.Element.Stereotype == "process_pkg")
            {
                this.rootPackage = this.eaUtils.repository.GetPackageByID(this.rootPackage.ParentID);
            }

            foreach (Package current in this.rootPackage.Packages)
            {
                if (current.Element.Stereotype == "metrics_pkg" )
                {
                    metricsPackage = current;
                }
                if (current.Element.Stereotype == "process_pkg" )
                {
                    processPackage = current;
                }
            }

            return processPackage != null && metricsPackage != null;
        }
    }

    class ElementPosition
    {
        public int left { set; get; }
        public int right { set; get; }
        public int top { set; get; }
        public int bottom { set; get; }

    }
    class ElementSize
    {
        public int width { set; get; }
        public int height { set; get; }
        public int offset { set; get; }
    }

    class ProcessImporter
    {
        Diagram diagram;

        EA.Element currentCategory = null;
        EA.Element currentProcessGroup = null;
        EA.Element currentProcess = null;
        EA.Element currentActivity = null;
        EA.Element currentTask = null;

        EA.Element previousCategory = null;
        EA.Element previousProcessGroup = null;
        EA.Element previousProcess = null;
        EA.Element previousActivity = null;
        EA.Element previousTask = null;

        Element fromDatabase = null;

        Package processPackage = null;

        EAUtils.EAUtils eaUtils;

        ElementSize categoryElementSize = new ElementSize { width = 710, height = 33, offset = 5 };
        ElementPosition categoryInitPosition = new ElementPosition { left = 10, top = 0 };

        ElementSize processGroupElementSize = new ElementSize { width = 612, height = 31, offset = 5 };
        ElementPosition processGroupInitPosition = new ElementPosition { left = 53, top = 5 };

        ElementSize processElementSize = new ElementSize { width = 110, height = 60, offset = 5 };
        ElementPosition processInitPosition = new ElementPosition { left = 0, top = 50 };

        ElementSize activityElementSize = new ElementSize { width = 110, height = 60, offset = 50 };
        ElementPosition activityInitPosition = new ElementPosition { left = 0, top = 50 };

        ElementSize taskElementSize = new ElementSize { width = 110, height = 60, offset = 50 };
        ElementPosition taskInitPosition = new ElementPosition { left = 0, top = 50 };

        ElementSize endTaskElementSize = new ElementSize { width = 30, height = 30, offset = 50 };
        ElementPosition endTaskInitPosition = new ElementPosition { left = 0, top = 65 };

        ElementSize poolElementSize = new ElementSize { width = 800, height = 180, offset = 0 };
        ElementPosition poolInitPosition = new ElementPosition { left = 2, top = 2, right = 802, bottom = 182 };

        ElementSize startEventElementSize = new ElementSize { width = 30, height = 30, offset = 0 };
        ElementPosition startEventInitPosition = new ElementPosition { left = 64, top = 64, right = 94, bottom = 94 };

        Dictionary<string, List<string>> process2metrics = new Dictionary<string, List<string>>();
        Dictionary<string, Element> metricsById = new Dictionary<string, Element>();

        public ProcessImporter(Package processPackage, Dictionary<string, List<string>> process2metrics, Dictionary<string, Element> metricsById, EAUtils.EAUtils eaUtils)
        {
            this.processPackage = processPackage;
            this.eaUtils = eaUtils;
            this.metricsById = metricsById;
            this.process2metrics = process2metrics;
        }

        Element loadElementFromDatabaseByAlias(string processId)
        {
            Element element = null;

            string sql = "SELECT object_id FROM t_object WHERE alias = '" + processId + "' and package_id = " + this.processPackage.PackageID;

            string queryResult = this.eaUtils.repository.SQLQuery(sql);

            if (queryResult.Length > 0)
            {
                XmlNode currentNode;
                XmlDocument xmlDOM = new XmlDocument();
                try
                {
                    xmlDOM.LoadXml(queryResult);

                    XmlNode rootNode = xmlDOM.DocumentElement.SelectSingleNode("/EADATA/Dataset_0/Data");

                    for (var i = 0; i < rootNode.ChildNodes.Count; i++)
                    {
                        currentNode = rootNode.ChildNodes.Item(i);

                        element = this.eaUtils.repository.GetElementByID(int.Parse(currentNode.ChildNodes[0].InnerText));
                    }
                }
                catch (Exception w) { w.ToString(); }
            }
            return element;
        }

        public void import(ExcelWorksheet worksheet, int rowNum)
        {
            string hierarchy = null;
            string[] level = null;
            string notes = "";
            string id = null;

            try
            {
                id = worksheet.Cells["A" + rowNum].Value.ToString();

            }
            catch (Exception) { }


            if ( id == null )
            {
                // esta entrada se da al finalizar todo por lo que es necesario agregar el fin de la actividad si se trata de un proceso
                // en el caso que sea un proceso currentActivity tiene el valor de la última actividad.

                addEndProcessFinish(currentActivity);

                // Este sería el caso si lo que falta es cerrar un subproceso
                addEndProcessFinish(currentTask);
            }
            else
            {

                hierarchy = worksheet.Cells["B" + rowNum].Value.ToString();
                level = hierarchy.Split('.');

                ExcelRange cell = worksheet.Cells["C" + rowNum];

                try
                {
                    notes = cell.Comment.Text;
                }
                catch (Exception) { }

                string name = worksheet.Cells["C" + rowNum].Value.ToString();
                string metricsAvailable = worksheet.Cells["F" + rowNum].Value.ToString();

                this.fromDatabase = this.loadElementFromDatabaseByAlias(id);

                if (level[1] == "0")
                {
                    previousCategory = currentCategory;

                    if (fromDatabase != null)
                    {
                        currentCategory = fromDatabase;
                        diagram = processPackage.Diagrams.GetAt(0);
                    }
                    else
                    {
                        currentCategory = this.processPackage.Elements.AddNew(name, "Activity");
                        currentCategory.Alias = id;
                        currentCategory.Stereotype = "APQC-EA::PCF_Category";
                        currentCategory.IsComposite = true;

                        currentCategory.Notes = notes;

                        currentCategory.Update();

                        diagram = processPackage.Diagrams.GetAt(0);
                        diagram.ExtendedStyle = "HideRel=0;ShowTags=0;ShowReqs=0;ShowCons=0;OpParams=1;ShowSN=0;ScalePI=0;PPgs.cx=0;PPgs.cy=0;PSize=9;ShowIcons=1;SuppCN=0;HideProps=0;HideParents=0;UseAlias=0;HideAtts=0;HideOps=0;HideStereo=0;HideEStereo=1;ShowRec=1;ShowRes=0;ShowShape=1;FormName=;";
                        diagram.Update();

                        addToDiagram(diagram, currentCategory, true, categoryElementSize, categoryInitPosition, previousCategory);

                        addInfo(currentCategory, id, "1", "Category", hierarchy, metricsAvailable);
                    }

                    if (process2metrics.ContainsKey(id))
                    {
                        Element metric;
                        // recorrer la lista para agregarle las métricas.
                        foreach (string metricId in process2metrics[id])
                        {
                            metric = metricsById[metricId];

                            this.eaUtils.connectorUtils.addConnector(currentCategory, metric, EAUtils.ConnectorUtils.CONNECTOR__REALISATION, null, null, true);
                        }
                    }

                    // si es un subproceso agrego un final de proceso
                    if (addEndTask(previousActivity, currentTask))
                    {
                        currentTask = null;
                    }

                    // si cambia el grupo de procesos cierro las actividades del proceso anterior.
                    // agrego final de actividades 
                    if (addEndTask(previousProcess, currentActivity))
                    {
                        currentActivity = null;
                    }
                }
                else if (level.Length == 2)
                {
                    previousProcessGroup = currentProcessGroup;

                    if (fromDatabase != null)
                    {
                        currentProcessGroup = fromDatabase;
                    }
                    else
                    {
                        currentProcessGroup = currentCategory.Elements.AddNew(name, "Activity");
                        currentProcessGroup.Stereotype = "APQC-EA::PCF_ProcessGroup";
                        currentProcessGroup.Alias = id;
                        currentProcessGroup.IsComposite = true;

                        currentProcessGroup.Notes = notes;

                        currentProcessGroup.Update();

                        diagram = currentCategory.Diagrams.GetAt(0);
                        diagram.ExtendedStyle = "HideRel=0;ShowTags=0;ShowReqs=0;ShowCons=0;OpParams=1;ShowSN=0;ScalePI=0;PPgs.cx=0;PPgs.cy=0;PSize=9;ShowIcons=1;SuppCN=0;HideProps=0;HideParents=0;UseAlias=0;HideAtts=0;HideOps=0;HideStereo=0;HideEStereo=1;ShowRec=1;ShowRes=0;ShowShape=1;FormName=;";
                        diagram.Update();

                        addToDiagram(diagram, currentProcessGroup, true, processGroupElementSize, processGroupInitPosition, previousProcessGroup);

                        addInfo(currentProcessGroup, id, "2", "Process Group", hierarchy, metricsAvailable);
                    }

                    if (process2metrics.ContainsKey(id))
                    {
                        Element metric;
                        // recorrer la lista para agregarle las métricas.
                        foreach (string metricId in process2metrics[id])
                        {
                            metric = metricsById[metricId];

                            this.eaUtils.connectorUtils.addConnector(currentProcessGroup, metric, EAUtils.ConnectorUtils.CONNECTOR__REALISATION, null, null, true);
                        }
                    }

                    // si cambia el grupo de procesos cierro las actividades del proceso anterior.
                    // agrego final de actividades 

                    // si es un task al cambiar de nivel debo usar la actividad actual y no la anterior.
                    // agrego un final de proceso
                    if (addEndTask(currentActivity, currentTask))
                    {
                        currentTask = null;
                    }

                    if (addEndTask(previousProcess, currentActivity))
                    {
                        currentActivity = null;
                    }

                }
                else if (level.Length == 3)
                {
                    previousProcess = currentProcess;

                    if (fromDatabase != null)
                    {
                        currentProcess = fromDatabase;
                    }
                    else
                    {
                        currentProcess = currentProcessGroup.Elements.AddNew(name, "Activity");
                        currentProcess.Stereotype = "APQC-EA::PCF_Process";
                        currentProcess.Alias = id;

                        currentProcess.Notes = notes;

                        currentProcess.Update();

                        addToDiagram(currentProcessGroup.Diagrams.GetAt(0), currentProcess, false, processElementSize, processInitPosition, previousProcess);

                        addInfo(currentProcess, id, "3", "Process", hierarchy, metricsAvailable);
                    }

                    if (process2metrics.ContainsKey(id))
                    {
                        Element metric;
                        // recorrer la lista para agregarle las métricas.
                        foreach (string metricId in process2metrics[id])
                        {
                            metric = metricsById[metricId];

                            this.eaUtils.connectorUtils.addConnector(currentProcess, metric, EAUtils.ConnectorUtils.CONNECTOR__REALISATION, null, null, true);
                        }
                    }

                    // si es un subproceso agrego un final de proceso
                    if (addEndTask(currentActivity, currentTask))
                    {
                        currentTask = null;
                    }

                    // si cambia el proceso cierro las actividades
                    // agrego final de actividades 
                    if (addEndTask(previousProcess, currentActivity))
                    {
                        currentActivity = null;
                    }
                }
                else if (level.Length == 4)
                {
                    Element startEvent = null;

                    previousActivity = currentActivity;

                    if (fromDatabase != null)
                    {
                        currentActivity = fromDatabase;
                    }
                    else
                    {
                        currentActivity = currentProcess.Elements.AddNew(name, "Activity");
                        currentActivity.Stereotype = "APQC-EA::PCF_Activity";
                        currentActivity.Alias = id;

                        currentActivity.Notes = notes;

                        currentActivity.Update();

                        if (currentProcess.Diagrams.Count == 0)
                        {
                            currentProcess.IsComposite = true;
                            currentProcess.Update();
                            currentProcess.Diagrams.Refresh();
                            startEvent = initBusinessProcess(currentProcess);
                        }

                        addToDiagram(currentProcess.Diagrams.GetAt(0), currentActivity, false, activityElementSize, activityInitPosition, previousActivity == null ? startEvent : previousActivity);

                        addInfo(currentActivity, id, "4", "Activity", hierarchy, metricsAvailable);
                    }

                    if (process2metrics.ContainsKey(id))
                    {
                        Element metric;
                        // recorrer la lista para agregarle las métricas.
                        foreach (string metricId in process2metrics[id])
                        {
                            metric = metricsById[metricId];

                            this.eaUtils.connectorUtils.addConnector(currentActivity, metric, EAUtils.ConnectorUtils.CONNECTOR__REALISATION, null, null, true);
                        }
                    }

                    if (previousActivity != null)
                    {
                        this.eaUtils.connectorUtils.addConnector(previousActivity, currentActivity, "ControlFlow", null, "BPMN2.0::SequenceFlow", true);
                    }
                    else if (startEvent != null)
                    {
                        this.eaUtils.connectorUtils.addConnector(startEvent, currentActivity, "ControlFlow", null, "BPMN2.0::SequenceFlow", true);
                    }
                    // si es un subproceso y este cambia agrego un final a las actividades del subproceso anterior.
                    if (addEndTask(previousActivity, currentTask))
                    {
                        currentTask = null;
                    }
                }
                else if (level.Length == 5)
                {
                    previousTask = currentTask;

                    Element startEvent = null;

                    if (fromDatabase != null)
                    {
                        currentTask = fromDatabase;
                    }
                    else
                    {
                        currentTask = currentActivity.Elements.AddNew(name, "Activity");
                        currentTask.Stereotype = "APQC-EA::PCF_Task";
                        currentTask.Alias = id;

                        currentTask.Notes = notes;

                        currentTask.Update();

                        if (currentActivity.Diagrams.Count == 0)
                        {
                            this.eaUtils.taggedValuesUtils.set(currentActivity, "activityType", "Sub-Process");

                            currentActivity.IsComposite = true;
                            currentActivity.Update();
                            currentActivity.Diagrams.Refresh();
                            startEvent = initBusinessProcess(currentActivity);
                        }

                        addToDiagram(currentActivity.Diagrams.GetAt(0), currentTask, false, taskElementSize, taskInitPosition, previousTask == null ? startEvent : previousTask);

                        addInfo(currentTask, id, "5", "Task", hierarchy, metricsAvailable);
                    }

                    if (process2metrics.ContainsKey(id))
                    {
                        Element metric;
                        // recorrer la lista para agregarle las métricas.
                        foreach (string metricId in process2metrics[id])
                        {
                            metric = metricsById[metricId];

                            this.eaUtils.connectorUtils.addConnector(currentTask, metric, EAUtils.ConnectorUtils.CONNECTOR__REALISATION, null, null, true);
                        }
                    }

                    if (previousTask != null)
                    {
                        this.eaUtils.connectorUtils.addConnector(previousTask, currentTask, "ControlFlow", null, "BPMN2.0::SequenceFlow", true);
                    }
                    else if (startEvent != null)
                    {
                        this.eaUtils.connectorUtils.addConnector(startEvent, currentTask, "ControlFlow", null, "BPMN2.0::SequenceFlow", true);
                    }
                }
            }
        }

        /// <summary>
        /// Este método se invoca al finalizar el proceso de la planilla por lo que sólo se cuenta con la última actividad.
        /// </summary>
        /// <param name="lastTask"></param>
        /// <returns></returns>
        private void addEndProcessFinish( Element lastTask)
        {
            if( lastTask != null && lastTask.ParentID != 0)
            {
                Element parentElement = this.eaUtils.repository.GetElementByID(lastTask.ParentID);
                this.addEndTask(parentElement, lastTask);
            }
        }

        private bool addEndTask(Element parentElement, Element lastTask)
        {
            //Session.Output("OPEN addEndTask");
            Element pool = null;
            bool create = false;

            //Session.Output("parentElement != null " + (parentElement != null));
            if (parentElement != null)
            {
                parentElement.Elements.Refresh();
                //Session.Output("parentElement.Elements.Count > 0 " + (parentElement.Elements.Count > 0));
            }

            if (parentElement != null && parentElement.Elements.Count > 0)
            {
                //Session.Output("parentElement " + parentElement.Name);
                create = true;

                pool = parentElement.Elements.GetAt(0);
                if (pool.Stereotype == "Pool")
                {
                    // si el elemento fue traducido entonces el nombre del pool debería ser el mismo.
                    if (pool.Name != parentElement.Name)
                    {
                        pool.Name = parentElement.Name;
                        pool.Update();
                    }

                    for (short i = 0; i < pool.Elements.Count; i++)
                    {
                        if (pool.Elements.GetAt(i).Stereotype == "EndEvent")
                        {
                            create = false;
                            break;
                        }
                    }
                }
                else
                {
                    for (short i = 0; i < parentElement.Elements.Count; i++)
                    {
                        if (parentElement.Elements.GetAt(i).Stereotype == "EndEvent")
                        {
                            create = false;
                            break;
                        }
                    }
                }
            }
            //Session.Output("create " + create);

            if (create && parentElement != null)
            {
                Element endTask = parentElement.Elements.AddNew("", "Event");
                endTask.Stereotype = "BPMN2.0::EndEvent";
                endTask.Update();
                parentElement.Elements.Refresh();

                this.eaUtils.connectorUtils.addConnector(lastTask, endTask, "ControlFlow", null, "BPMN2.0::SequenceFlow", true);

                addToDiagram(parentElement.Diagrams.GetAt(0), endTask, false, endTaskElementSize, endTaskInitPosition, lastTask);

                insidePool(parentElement, endTask);
            }
            //Session.Output("END addEndTask");
            return create;
        }

        private void insidePool(Element parentElement, Element endTask )
        {
            Element pool = null;
            Element other = null;
            DiagramObject poolDiagramObject = null;
            DiagramObject endTaskDiagramObject = null;

            for (short i = 0; i < parentElement.Elements.Count; i++)
            {
                pool = parentElement.Elements.GetAt(i);

                if (pool.Stereotype == "Pool")
                {
                    // ajustar el tamaño del pool para entren todos los elementos
                    // buscamos el end task en el diagrama para tomar su posición y ajustar el tamaño del pool.
                    foreach( DiagramObject currentDiagramObject in parentElement.Diagrams.GetAt(0).DiagramObjects)
                    {
                        if(currentDiagramObject.ElementID == pool.ElementID)
                        {
                            poolDiagramObject = currentDiagramObject;
                        }
                        if (currentDiagramObject.ElementID == endTask.ElementID)
                        {
                            endTaskDiagramObject = currentDiagramObject;
                        }

                        if(poolDiagramObject != null && endTaskDiagramObject != null)
                        {
                            break;
                        }
                    }

                    poolDiagramObject.right = endTaskDiagramObject.right + 50;
                    poolDiagramObject.Update();

                    break;
                }
                else
                {
                    pool = null;
                }
            }

            if (pool != null)
            {

                for (short i = 0; i < parentElement.Elements.Count; i++)
                {
                    other = parentElement.Elements.GetAt(i);

                    if (other.Stereotype != "Pool")
                    {
                        other.ParentID = pool.ElementID;
                        other.Update();
                    }
                }
                parentElement.Elements.Refresh();
            }
        }

        private Element initBusinessProcess(Element processElement)
        {
            Element pool;
            pool = processElement.Elements.AddNew(currentProcess.Name, "ActivityPartition");
            pool.Stereotype = "BPMN2.0::Pool";
            pool.Update();

            processElement.Elements.Refresh();

            Element startEvent = processElement.Elements.AddNew("", "Event");
            startEvent.Stereotype = "BPMN2.0::StartEvent";
            startEvent.Update();
            processElement.Elements.Refresh();

            addToDiagram(processElement.Diagrams.GetAt(0), pool, false, poolElementSize, poolInitPosition, true, null);

            addToDiagram(processElement.Diagrams.GetAt(0), startEvent, false, startEventElementSize, startEventInitPosition, true, null);

            return startEvent;
        }

        private void addInfo(EA.Element element, string id, string index, string level, string hierarchy, string metricsAvailable)
        {
            this.eaUtils.taggedValuesUtils.set(element, "PCF-Level-index"      , index     );
            this.eaUtils.taggedValuesUtils.set(element, "PCF-Level"            , level     );
            this.eaUtils.taggedValuesUtils.set(element, "PCF-ID"               , id        );
            this.eaUtils.taggedValuesUtils.set(element, "PCF-Hierarchy"        , hierarchy );
            this.eaUtils.taggedValuesUtils.set(element, "PCF-Metrics-available", (metricsAvailable == "Y" ? "Yes" : "No"));
        }

        private void addToDiagram(Diagram diagram, Element currentProcessGroup, bool horizontal, ElementSize elementSize, ElementPosition initPosition, Element afterThat)
        {
            this.addToDiagram(diagram, currentProcessGroup, horizontal, elementSize, initPosition, false, afterThat);
        }

        private void addToDiagram(Diagram diagram, Element currentProcessGroup, bool horizontal, ElementSize elementSize, ElementPosition initPosition)
        {
            this.addToDiagram(diagram, currentProcessGroup, horizontal, elementSize, initPosition, false, null);
        }

        private void addToDiagram(Diagram diagram, Element element, bool horizontal, ElementSize elementSize, ElementPosition initPosition, bool isPositionAbsolute, Element afterThat)
        {
            DiagramObject newDiagramObject;
            DiagramObject lastDiagramObject;

            newDiagramObject = diagram.DiagramObjects.AddNew(element.Name, element.Type);

            newDiagramObject.ElementID = element.ElementID;
            newDiagramObject.Sequence = 1;

            newDiagramObject.Update();

            if ( ! isPositionAbsolute )
            {
                lastDiagramObject = null;
                if (diagram.DiagramObjects.Count > 0)
                {
                    if( afterThat != null)
                    {
                        foreach( DiagramObject currentDO in diagram.DiagramObjects)
                        {
                            if(currentDO.ElementID == afterThat.ElementID)
                            {
                                lastDiagramObject = currentDO;
                                break;
                            }
                        }
                    }
                    else
                    {
                        lastDiagramObject = diagram.DiagramObjects.GetAt((short)(diagram.DiagramObjects.Count - 1));
                        if (lastDiagramObject != null)
                        {
                            if (this.eaUtils.repository.GetElementByID(lastDiagramObject.ElementID).Stereotype == "Pool")
                            {
                                lastDiagramObject = null;

                                this.eaUtils.printOut(diagram.Name);

                                for( short i = 0; i < diagram.DiagramObjects.Count; i++)
                                {
                                    this.eaUtils.printOut("id elmt " + diagram.DiagramObjects.GetAt( i ).ElementID);
                                }
                            }
                        }
                    }
                }

                initPosition = getElementPosition(elementSize, diagram.DiagramObjects.Count, initPosition, horizontal, lastDiagramObject);
            }

            newDiagramObject.bottom = (initPosition.bottom <= 0 ? initPosition.bottom : initPosition.bottom * -1);
            newDiagramObject.left = initPosition.left;
            newDiagramObject.right = initPosition.right;
            newDiagramObject.top = (initPosition.top <= 0 ? initPosition.top : initPosition.top * -1);

            newDiagramObject.Sequence = 1;

            newDiagramObject.Update();
            diagram.DiagramObjects.Refresh();

        }

        private ElementPosition getElementPosition( ElementSize elementSize, int elementNumber, ElementPosition initPosition, bool horizontal, DiagramObject lastDiagramObject)
        {
            ElementPosition postition = new ElementPosition { left = 0, top = 0, right = 0, bottom = 0 };

            int offset = elementSize.offset;

            if (horizontal) // debajo del otro
            {
                //var plus = ( lastDiagramObject != null ?  (elementSize.height + (-1*(lastDiagramObject.bottom ))) : (elementSize.height * elementNumber)  );
                int plus = (lastDiagramObject != null ? -lastDiagramObject.bottom : (elementSize.height * elementNumber));

                //postition.top = initPosition.top + (elementSize.height * elementNumber) + offset;
                postition.top = initPosition.top + plus + offset;
                postition.left = initPosition.left;
            }
            else // al lado del otro
            {
                var plus = (lastDiagramObject != null ? lastDiagramObject.right : (elementSize.width * elementNumber));

                postition.top = initPosition.top;
                postition.left = ( initPosition.left + plus + offset );
            }

            postition.bottom = postition.top + elementSize.height;

            postition.right = postition.left + elementSize.width;

            return postition;
        }
    }
}
