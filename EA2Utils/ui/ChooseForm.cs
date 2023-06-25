using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EAUtils.ui
{
    public partial class ChooseForm : Form
    {
        public string selected = null;
        List<String> optionList = new List<string>();

        public ChooseForm()
        {
            InitializeComponent();
        }

        public void addOption(String option)
        {
            this.optionList.Add(option);
        }

        internal void setSelected(string name)
        {
            this.selected = name;
            this.Close();
        }

        public string getSelected()
        {
            return this.selected;
        }

        private void chooseUC1_Load(object sender, EventArgs e)
        {
            this.chooseUC.setParent(this);

            this.chooseUC.setOptionList(optionList);

        }
    }
}
