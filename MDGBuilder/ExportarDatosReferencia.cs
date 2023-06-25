using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using EA;
using EAUtils;

namespace MDGBuilder
{
    public class ExportarDatosReferencia
    {
        private const string template = @"<?xml version=""1.0"" encoding=""Windows-1252"" standalone=""no"" ?><RefData version=""1.0"" exporter=""EA.25""></RefData>";

        EAUtils.EAUtils eaUtils;
        EstructuraSolucion estructuraSolucion;
        DbUtils dbUtils;
        XmlUtils xmlUtils = new XmlUtils();
        XmlDocument exportXmlDocument;
        public ExportarDatosReferencia(EAUtils.EAUtils eaUtils)
        {
            this.eaUtils = eaUtils;

            estructuraSolucion = new EstructuraSolucion(eaUtils);
            estructuraSolucion.establecer();

            dbUtils = new DbUtils(this.eaUtils.repository);

            exportXmlDocument = xmlUtils.createDOM();

            exportXmlDocument.LoadXml(template);
        }

        public void exportar()
        {
            string query;
            Dictionary<string, EA.Attribute> attributeByName = new Dictionary<string, EA.Attribute>();
            List<string> base64Columns = new List<string>();
            string datosReferenciaFileName = this.estructuraSolucion.mtsElement.Name + "_datosReferencia_" + this.estructuraSolucion.mtsElement.Version + ".xml";


            DirectorioArchivoUsuarioHelper directorioUsuario = new DirectorioArchivoUsuarioHelper(this.eaUtils)
                .withPaquete(this.estructuraSolucion.datosReferencia);

            directorioUsuario.initSaveFileInfo().fileName(datosReferenciaFileName);
                                
            string directorio = directorioUsuario.directorio();

            foreach ( EA.Element element in this.estructuraSolucion.datosReferencia.Elements)
            {
                if( ! element.IsActive)
                {
                    this.eaUtils.printOut(element.Name +" no está activo");
                    continue;
                }

                attributeByName.Clear();
                base64Columns.Clear();

                foreach ( EA.Attribute attribute in element.Attributes)
                {
                    attributeByName.Add(attribute.Name, attribute);
                }

                if( attributeByName.ContainsKey("base64Columns"))
                {
                    base64Columns.AddRange(attributeByName["base64Columns"].Default.Trim().Split(','));
                }

                query = this.eaUtils.notes2Txt(element.Notes).Trim();

                if ( query.Length > 0)
                {
                    this.executeQuery(query, element, attributeByName, base64Columns);
                }
                else
                {
                    query = attributeByName["table"].Default.Trim();

                    if(query.Length > 0)
                    {
                        query = "select " + attributeByName["columns"].Default.Trim() +" from "+ query;

                        this.executeQuery(query, element, attributeByName, base64Columns);
                    }
                }
            }

            //System.IO.File.WriteAllText( directorio +"\\"+ datosReferenciaFileName, this.exportXmlDocument.OuterXml, Encoding.Default);

            byte[] bytes = Encoding.ASCII.GetBytes(this.exportXmlDocument.OuterXml);

            string zipFileName = directorio + "\\" + datosReferenciaFileName.Replace(".xml", ".zip");

            FileInfo fileInfo = new FileInfo(zipFileName);

            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            using (var fileStream = new FileStream(zipFileName, FileMode.OpenOrCreate))
            {
                using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create, true))
                {
                    var zipArchiveEntry = archive.CreateEntry(datosReferenciaFileName, CompressionLevel.Fastest);
                    using (var zipStream = zipArchiveEntry.Open())
                    {
                        zipStream.Write(bytes, 0, bytes.Length);
                    }
                }
            }
            this.eaUtils.printOut("Fin");
        }

        private void executeQuery(string query, EA.Element element, Dictionary<string, EA.Attribute> attributeByName, List<string> base64Columns)
        {
            List<List<string>> rows;
            XmlElement rowXmlElelemnt;
            XmlElement xmlDataRow;
            XmlElement xmlColumn;
            XmlAttribute xmlAttribute;

            if (dbUtils.execute(query))
            {
                rows = dbUtils.asList();

                rowXmlElelemnt = this.addDataSet(element.Name, attributeByName);

                foreach (List<string> row in rows)
                {

                    xmlDataRow = this.exportXmlDocument.CreateElement("DataRow");

                    rowXmlElelemnt.AppendChild(xmlDataRow);

                    for (int i = 0; i < row.Count; i++)
                    {
                        xmlColumn = this.exportXmlDocument.CreateElement("Column");

                        xmlDataRow.AppendChild(xmlColumn);
                        xmlColumn.SetAttribute("name", dbUtils.getHeader()[i]);

                        if( base64Columns.Contains(dbUtils.getHeader()[i] )) {

                            xmlAttribute = this.exportXmlDocument.CreateAttribute("dt:dt", "urn:schemas-microsoft-com:datatypes");
                            xmlColumn.Attributes.Append(xmlAttribute);
                            xmlColumn.SetAttribute("dt:dt", "bin.base64");
                            xmlColumn.InnerText= row[i].ToString();
                        }
                        else
                        {
                            xmlColumn.SetAttribute("value", row[i]);
                        }
                    }
                }
            }
            else
            {
                this.eaUtils.printOut("No hay resultados para la query");
                this.eaUtils.printOut(query);
            }
        }
        private XmlElement addDataSet(string name, Dictionary<string, EA.Attribute> attributeByName)
        {
            XmlElement xmlElement = this.exportXmlDocument.CreateElement("DataSet");

            xmlElement.SetAttribute("name", name);
            xmlElement.SetAttribute("table", attributeByName["table"].Default);
            xmlElement.SetAttribute("filter", attributeByName["filter"].Default);

            if(attributeByName.ContainsKey("stoplist") && attributeByName["stoplist"].Default.Trim().Length > 0) 
            {
                xmlElement.SetAttribute("stoplist", attributeByName["stoplist"].Default);
            }

            this.exportXmlDocument.DocumentElement.AppendChild(xmlElement);

            return xmlElement;
        }
    }
}
