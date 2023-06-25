using DocumentFormat.OpenXml.Packaging;
using EAUtils;
using EAUtils.flow;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proceso4Doc
{
    public class Recorre4Doc : RecorreFlow
    {
        string archivoAnterior = null;
        public string directorio = null;

        public Recorre4Doc(EAUtils.EAUtils eaUtils) : base(eaUtils)
        {           
        }        
        public override void enElemento(EA.Element element)
        {
            string doc = element.GetLinkedDocument();

            if (doc != "")
            {
                string archivoActual = element.ElementGUID.Replace("{", "doc").Replace("}", ".docx");

                archivoActual = Path.Combine(directorio, archivoActual);

                element.SaveLinkedDocument(archivoActual);

                if (archivoAnterior != null)
                {
                    try
                    {
                        combinarDocs(archivoAnterior, archivoActual);
                    }
                    catch(Exception ex)
                    {
                        this.eaUtils.printOut("No se pudo combinar el documento");

                        this.eaUtils.printOut(ex.ToString());
                    }
                }

                archivoAnterior = archivoActual;
            }

            if ( element.Stereotype == "Activity")
            {
                this.eaUtils.printOut("Recorriendo "+ element.Name);
            }
        }

        [STAThreadAttribute]
        private void combinarDocs(string file1, string file2)
        {
            // Abrir el primer archivo
            using (WordprocessingDocument doc1 = WordprocessingDocument.Open(file1, false))
            {
                // Crear una copia del primer archivo
                //System.IO.File.Copy(file1, outputFile, true);

                // Abrir el segundo archivo para lectura
                using (WordprocessingDocument doc2 = WordprocessingDocument.Open(file2, false))
                {
                    // Clonar el contenido del segundo archivo en el archivo combinado
                    doc2.MainDocumentPart.Document.Body.Elements().ToList()
                        .ForEach(element => doc1.MainDocumentPart.Document.Body.AppendChild(element));

                    // Guardar el archivo combinado
                    doc1.MainDocumentPart.Document.Save();
                }
            }
        }
    }
}
