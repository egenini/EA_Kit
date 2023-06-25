using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LicenceManager
{
    public partial class FormLicences : Form
    {
        private Manager manager;

        public FormLicences()
        {
            InitializeComponent();
        }

        public FormLicences(Manager manager)
        {
            this.manager = manager;
            InitializeComponent();
        }

        private void aceptar_Click(object sender, EventArgs e)
        {
            if( ! string.IsNullOrWhiteSpace( this.textBoxLicences.Text ) )
            {
                this.manager.onEnter(this.textBoxLicences.Text);

                this.Close();
            }
        }

        private void cancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
