using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDGBuilder.mdg
{
    public class ImportFromMts
    {
        EAUtils.EAUtils eaUtils;
        Element mtsElement;
        Package mdgPackage;
        MDGRenamer renamer = new MDGRenamer();

        public ImportFromMts(EAUtils.EAUtils eaUtils, Package mdgPackage)
        {
            this.eaUtils    = eaUtils;
            this.mdgPackage = mdgPackage;
        }

        public void import()
        {
            this.mtsElement = renamer.getMtsElement(mdgPackage);

            OpenFileDialog opd = new OpenFileDialog();

            opd.Title = ".mts";
            opd.DefaultExt = "mts";
            opd.Filter = "mts (*.mts)|*.mts";
            opd.Multiselect = false;
            opd.CheckFileExists = true;
            opd.CheckPathExists = true;

            if ( opd.ShowDialog() == DialogResult.OK )
            {
                string mtsFileName = opd.FileName;

                if ( System.IO.File.Exists( mtsFileName ) )
                {
                    MtsUtil mtsUtil = new MtsUtil();

                    mtsUtil.importFromMts( mtsFileName, this.mdgPackage, mtsElement );
                }


            }
        }
    }
}
