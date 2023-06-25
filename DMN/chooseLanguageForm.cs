using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMN
{
    public partial class chooseLanguageForm : Form
    {
        public VariableEditor variableEditor = null;
        public string languageSelected = null;
        private bool generateSourceCode = false;

        public chooseLanguageForm()
        {
            InitializeComponent();
        }

        private void chooseLanguage1_Load(object sender, EventArgs e)
        {
            // este es para que el user control cierre la ventana.
            this.chooseLanguage1.setParent(this);
            // este es para que el user control complete con botones los lenguajes
            this.chooseLanguage1.setFramework(this.variableEditor.main.framework, this.generateSourceCode);
        }

        private void chooseLanguage2_Load(object sender, EventArgs e)
        {

        }

        internal void setCaller(VariableEditor variableEditor, bool generateSourceCode)
        {
            this.generateSourceCode = generateSourceCode;
            this.variableEditor = variableEditor;
        }

        internal void setLanguageSelected(string name)
        {
            this.languageSelected = name;
            this.Close();
        }
    }
}
