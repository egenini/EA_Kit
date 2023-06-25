using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Newtonsoft.Json;
using UIResources;
using EAUtils;

namespace Productividad
{
    public partial class JsonXmlViewer : UserControl
    {
        public static string JSON_WINDOW_NAME = "Ver JSON";
        public static string XML_WINDOW_NAME = "Ver XML";
        private Main caller;
        private Boolean asJson = false;
        private System.Xml.Linq.XNode xmlDoc;
        private string jsonSource;
        private bool fromText = false;

        public JsonXmlViewer()
        {
            InitializeComponent();
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            
        }

        public void setJson( string json, Main caller )
        {
            this.caller           = caller;
            this.asJson           = true;
            this.jsonSource       = json;
            //this.toPascal.Checked = true;
            this.refreshView();
        }

        public void setXml(string json, Main caller )
        {
            this.caller           = caller;
            this.jsonSource       = json;
            //this.toPascal.Checked = true;
            this.refreshView();
        }

        private void toPascal_CheckedChanged(object sender, EventArgs e)
        {
            if (this.toPascal.Checked)
            {
                this.jsonSource = caller.jsonViewerRefreshCallback(false, getStringCase());
                this.refreshView();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.toCamel.Checked)
            {
                this.jsonSource = caller.jsonViewerRefreshCallback(false, getStringCase());
                this.refreshView();
            }
        }

        private void toSnake_CheckedChanged(object sender, EventArgs e)
        {
            if (this.toSnake.Checked)
            {
                this.jsonSource = caller.jsonViewerRefreshCallback(false, getStringCase());
                this.refreshView();
            }
        }

        private void toDot_CheckedChanged(object sender, EventArgs e)
        {
            if (this.toDot.Checked)
            {
                this.jsonSource = caller.jsonViewerRefreshCallback(false, getStringCase());
                this.refreshView();
            }
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            this.jsonSource = caller.jsonViewerRefreshCallback(true,getStringCase());

            if( this.jsonSource.Length != 0)
            {
                this.refreshView();
            }
        }

        private short getStringCase()
        {
            short stringCase = StringUtils.NONE;

            if (this.toPascal.Checked)
            {
                stringCase = StringUtils.PASCAL;
            }
            else if (this.toCamel.Checked)
            {
                stringCase = StringUtils.CAMEL;
            }
            else if (this.toSnake.Checked)
            {
                stringCase = StringUtils.SNAKE;
            }
            else if (this.toDot.Checked)
            {
                stringCase = StringUtils.DOT;
            }
            return stringCase;
        }

        private void refreshView()
        {
            treeView1.Nodes.Clear();

            if ( asJson )
            {
                if( ! this.fromText )
                {
                    this.textBox1.Text = jsonSource;
                    this.textBox1.Refresh();
                }

                JsonTreeViewLoader.LoadJsonToTreeView(this.treeView1, jsonSource);

                this.fromText = false;
            }
            else
            {
                xmlDoc = JsonConvert.DeserializeXNode(jsonSource, "Root");

                string xml = xmlDoc.ToString();

                this.textBox1.Text = xml;
                this.textBox1.Refresh();

                XmlUtils.xmlUtils(this.treeView1, xmlDoc);
            }
        }

        private void copiar_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.textBox1.Text);
            Alert.Success("Copiado al porta papeles");
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.fromText = true;

            this.setJson(textBox1.Text, this.caller);
        }
    }
}
