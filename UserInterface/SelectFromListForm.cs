using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UserInterface
{
    public partial class SelectFromListForm : Form
    {
        public string selected = null;
        List<String> optionList = new List<string>();

        public SelectFromListForm()
        {
            InitializeComponent();
        }

        public void addOption(String option)
        {
            this.optionList.Add( option );
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

        private void selectFromList1_Load(object sender, EventArgs e)
        {
            this.selectFromList1.setParent(this);

            this.selectFromList1.setOptionList(optionList);
        }
    }
}
