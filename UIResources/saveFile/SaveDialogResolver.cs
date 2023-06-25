using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UIResources.saveFile
{
    public class SaveDialogResolver
    {
        public bool resolve(SaveFileInfo info)
        {
            bool saveFile = false;
            // pregunto si se quiere al porta papeles o al disco
            DialogResult dialogResult = MessageBox.Show(Properties.Resources.guardarEnDiscoTitle, Properties.Resources.OpcionesGeneracionTitle, MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                saveFileDialog1.InitialDirectory = info.initialDirectory();

                saveFileDialog1.FileName = info.fileName();

                saveFileDialog1.Title = Properties.Resources.guardarTitle;

                saveFileDialog1.CheckFileExists = false;

                saveFileDialog1.CheckPathExists = true;

                saveFileDialog1.DefaultExt = info.defaultExtension();

                saveFileDialog1.Filter = info.filter();

                saveFileDialog1.FilterIndex = 1;

                saveFileDialog1.RestoreDirectory = false;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)

                {
                    info.fileName( saveFileDialog1.FileName );
                    saveFile = true;
                    System.IO.File.WriteAllText(info.path() + info.fileName(), Clipboard.GetText(), Encoding.UTF8);

                }
            }
            return saveFile;
        }
    }
}
