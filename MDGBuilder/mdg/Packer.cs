using EA;
using EAUtils.saveUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MDGBuilder.mdg
{
    public class Packer
    {
        private EAUtils.EAUtils eaUtils;
        EstructuraSolucion estructuraSolucion;
        public Packer(EAUtils.EAUtils eaUtils) {
        
            this.eaUtils = eaUtils;
            estructuraSolucion = new EstructuraSolucion(eaUtils);
            estructuraSolucion.establecer();

        }

        public void pack()
        {
            string zipName = estructuraSolucion.mtsElement.Genfile;

            if ( zipName == "")
            {
                SaveFileInfo info = new SaveFileInfo();

                info.fileName(this.estructuraSolucion.mtsElement.Name + "_" + estructuraSolucion.mtsElement.Version + ".zip");

                info.defaultExtension( "zip" );

                info.filter( info.fileName() +" (*.zip)|*.zip");

                ChooseTarget2Save chooser = new ChooseTarget2Save();

                chooser.choose(info, "Elegir directorio de entregable");

                zipName = info.fileName();

                estructuraSolucion.mtsElement.Genfile = zipName;
                estructuraSolucion.mtsElement.Update();
            }
            FileInfo fileInfo= new FileInfo(zipName);

            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            using (var fileStream = new FileStream(zipName, FileMode.OpenOrCreate))
            {
                using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create, true))
                {
                    byte[] content = System.IO.File.ReadAllBytes(estructuraSolucion.principal.Element.Genfile.Replace("mts", "xml"));

                    add(archive, estructuraSolucion.mtsElement.Name + ".xml", content);

                    if( estructuraSolucion.frameworks != null)
                    {
                        addTemplates(archive, estructuraSolucion.frameworks);
                    }
                    if(estructuraSolucion.templates != null)
                    {
                        addTemplates(archive, estructuraSolucion.templates);
                    }
                }
                this.eaUtils.printOut("Entrgable generado " + zipName);
            }
        }

        private void add(ZipArchive archive, string fileName, byte[] bytes)
        {
            var zipArchiveEntry = archive.CreateEntry(fileName, CompressionLevel.Fastest);
            using (var zipStream = zipArchiveEntry.Open())
            {
                zipStream.Write(bytes, 0, bytes.Length);
            }
        }

        private void addTemplates(ZipArchive archive, Package parent) 
        {
            byte[] content;

            foreach (EA.Package child in parent.Packages)
            {
                string fileNameTaggedValueName = Builder.TV__GENFILE_PREFIX + parent.Name.ToUpper() +"_"+ child.Name;

                string genFile = this.eaUtils.taggedValuesUtils.get(parent.Element, fileNameTaggedValueName, "").asString();

                if (genFile != "")
                {
                    if( System.IO.File.Exists(genFile.Replace(".xml", ".rtf")))
                    {
                        content = System.IO.File.ReadAllBytes(genFile.Replace(".xml", ".rtf"));

                        add(archive, Path.GetFileName(genFile).Replace(".xml", ".rtf"), content);

                    }
                    content = System.IO.File.ReadAllBytes(genFile);

                    add(archive, Path.GetFileName(genFile), content);

                }
            }
        }
    }
}
