using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EAUtils.saveUtils
{
    class SaveDialogResolverUtil
    {
        public bool resolve(SaveFileInfo info)
        {
            bool saveFile = false;
            // pregunto si se quiere al porta papeles o al disco
            bool doSave = true;

            if( ! info.saveAlways )
            {
                DialogResult dialogResult = MessageBox.Show(
                    Properties.Resources.OPCIONES_GENERACION_TITLE
                    , Properties.Resources.GUARDAR_EN_DISCO_TITLE + " " + info.artifactName
                    , MessageBoxButtons.YesNo);

                doSave = (dialogResult == DialogResult.Yes);
            }

            bool showFileDialog = false;

            if (doSave)
            {
                if( File.Exists(info.fileName() ) && info.oneTime )
                {
                    doSave = false;
                }

                if( doSave && info.showFileDialogAlways )
                {
                    showFileDialog = true;
                }

                if (showFileDialog || info.fileName() == null || info.fileName() == "" || ! Directory.Exists(Path.GetDirectoryName(info.fileName())) )
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.InitialDirectory = info.initialDirectory();

                    saveFileDialog1.FileName = info.fileName();

                    saveFileDialog1.Title = Properties.Resources.GUARDAR_TITLE;

                    saveFileDialog1.CheckFileExists = false;

                    saveFileDialog1.CheckPathExists = true;

                    saveFileDialog1.DefaultExt = info.defaultExtension();

                    saveFileDialog1.Filter = info.filter();

                    saveFileDialog1.FilterIndex = 1;

                    saveFileDialog1.RestoreDirectory = true; // esto estaba en false.

                    doSave = (saveFileDialog1.ShowDialog() == DialogResult.OK);

                    if( doSave)
                    {
                        info.fileName(saveFileDialog1.FileName);
                    }
                }

                if (doSave)
                {
                    saveFile = true;
                    System.IO.File.WriteAllText(info.path() + info.fileName(), info.fileContent, Encoding.UTF8);
                }
            }
            return saveFile;
        }
    }
}
