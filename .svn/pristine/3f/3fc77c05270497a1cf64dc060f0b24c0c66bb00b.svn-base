using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UIResources;

namespace DMN.ui
{
    public partial class JsonXmlViewer : UserControl
    {
        public const string WINDOW_NAME = "As Json";

        private JsonProvider jsonProvider;
        private string jsonSource;

        public JsonXmlViewer()
        {
            InitializeComponent();
        }

        public void setJson(JsonProvider jsonProvider)
        {
            this.jsonProvider = jsonProvider;
            this.refresh();
        }

        private void refresh()
        {
            this.controlTreeView.Nodes.Clear();

            this.jsonSource = jsonProvider.getJson();

            if (this.jsonSource.Length != 0)
            {
                this.controlTextBoxTextViewer.Text = jsonSource;
                JsonTreeViewLoader.LoadJsonToTreeView(this.controlTreeView, this.jsonSource);
            }
            else
            {
                this.controlTextBoxTextViewer.Text = "";
            }

            this.controlTextBoxTextViewer.Refresh();

        }

        private void controlButtonRefresh_Click(object sender, EventArgs e)
        {
            this.refresh();
        }

        private void controlButtonCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.controlTextBoxTextViewer.Text);
            Alert.Success("Copiado al porta papeles");
        }

        private void controlButtonTreeViewExpand_Click(object sender, EventArgs e)
        {
            if(this.controlTreeView.SelectedNode == null)
            {
                this.controlTreeView.Nodes[0].ExpandAll();
            }
            else
            {
                this.controlTreeView.SelectedNode.ExpandAll();
            }
        }

        private void controlButtonTreeViewCollapse_Click(object sender, EventArgs e)
        {
            if (this.controlTreeView.SelectedNode == null)
            {
                this.controlTreeView.Nodes[0].Collapse();
            }
            else
            {
                this.controlTreeView.SelectedNode.Collapse();
            }
        }
    }
}
