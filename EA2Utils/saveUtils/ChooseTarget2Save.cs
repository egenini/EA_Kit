using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EAUtils.saveUtils
{
    public class ChooseTarget2Save
    {

        public bool choose(SaveFileInfo info, string title)
        {
            bool choosed = false;

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.InitialDirectory = info.initialDirectory();

            saveFileDialog1.FileName = info.fileName();

            //saveFileDialog1.Title = Properties.Resources.GUARDAR_TITLE;
            saveFileDialog1.Title = title;

            saveFileDialog1.CheckFileExists = false;

            saveFileDialog1.CheckPathExists = true;

            saveFileDialog1.DefaultExt = info.defaultExtension();

            saveFileDialog1.Filter = info.filter();

            saveFileDialog1.FilterIndex = 1;

            saveFileDialog1.RestoreDirectory = false;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)

            {
                info.fileName(saveFileDialog1.FileName);
                choosed = true;
            }
            return choosed;
        }
    }
}
