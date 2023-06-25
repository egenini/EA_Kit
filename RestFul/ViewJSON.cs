using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RestFul
{
    public partial class ViewJSON : UserControl
    {
        public const string NAME = "Ver JSON";
        public ViewJSON()
        {
            InitializeComponent();
        }
        public void setXML(string xml)
        {
            viewXML.Text = xml;
            viewXML.Refresh();
        }

        public void setJSON( string json )
        {
            textBox1.Text = json;
            textBox1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = EAUtils.StringUtils.toSnake(textBox1.Text);
            textBox1.Refresh();

            viewXML.Text = EAUtils.StringUtils.toSnake(viewXML.Text);
            viewXML.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = EAUtils.StringUtils.toCamel(textBox1.Text);
            textBox1.Refresh();

            viewXML.Text = EAUtils.StringUtils.toCamel(viewXML.Text);
            viewXML.Refresh();
        }

        private void ViewJSON_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            textBox1.Text = EAUtils.StringUtils.toPascal(textBox1.Text);
            textBox1.Refresh();

            viewXML.Text = EAUtils.StringUtils.toPascal(viewXML.Text);
            viewXML.Refresh();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            textBox1.Text = EAUtils.StringUtils.toPoint(textBox1.Text);
            textBox1.Refresh();

            viewXML.Text = EAUtils.StringUtils.toPoint(viewXML.Text);
            viewXML.Refresh();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
