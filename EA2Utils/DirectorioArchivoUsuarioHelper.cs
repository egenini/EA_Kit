using EAUtils.saveUtils;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAUtils
{

    /// <summary>
    /// Esta clase provee directorio por usuario en un valor etiquetado en formato json.
    /// </summary>
    public class DirectorioArchivoUsuarioHelper
    {
        public const string TGV = "usersDir";

        string currentUser = "Anonymous";
        EAUtils esUtils;
        string dir = null;
        string directorioNombreArchivo = null;
        EA.Package package;
        string valorEtiquetado = TGV;
        SaveFileInfo info = new SaveFileInfo();
        public DirectorioArchivoUsuarioHelper(EAUtils eaUtils)
        {
            this.esUtils = eaUtils;
            if( this.esUtils.repository.IsSecurityEnabled)
            {
                currentUser = eaUtils.repository.GetCurrentLoginUser(false);
            }
            package = eaUtils.repository.GetTreeSelectedPackage();
        }

        public DirectorioArchivoUsuarioHelper withPaquete(EA.Package package)
        {
            this.package = package;

            return this;
        }
        public DirectorioArchivoUsuarioHelper withTaggedValue(string tgv)
        {
            this.valorEtiquetado = tgv;
            return this;
        }

        public SaveFileInfo initSaveFileInfo()
        {
            return info;
        }

        public string nombreCompletoArchivo()
        {
            string tgvDir = this.esUtils.taggedValuesUtils.get(this.package.Element, this.valorEtiquetado, "{}").asString();

            JObject o = JObject.Parse(tgvDir);

            if (o.ContainsKey(this.currentUser))
            {
                this.directorioNombreArchivo = o.GetValue(this.currentUser).ToString();
            }
            else
            {
                ChooseTarget2Save chooser = new ChooseTarget2Save();

                chooser.choose(info, "Elegir nombre de archivo y directorio");

                o.Add(this.currentUser, info.fileName());

                this.directorioNombreArchivo = info.fileName();

                this.esUtils.taggedValuesUtils.set(this.package.Element, this.valorEtiquetado, o.ToString());
            }
            return this.directorioNombreArchivo;

        }
        public string directorio()
        {
            string tgvDir = this.esUtils.taggedValuesUtils.get(this.package.Element, this.valorEtiquetado, "{}").asString();

            JObject o = JObject.Parse(tgvDir);

            if( o.ContainsKey(this.currentUser))
            {
                dir = o.GetValue(this.currentUser).ToString();
            }
            else
            {
                ChooseTarget2Save chooser = new ChooseTarget2Save();

                chooser.choose(info, "Elegir directorio");

                dir = Path.GetDirectoryName(info.fileName());

                o.Add(this.currentUser, dir);

                this.esUtils.taggedValuesUtils.set(this.package.Element, this.valorEtiquetado, o.ToString());
            }
            return dir;
        }
    }
}
