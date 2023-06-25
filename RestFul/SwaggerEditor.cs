using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RestFul.modelo;

namespace RestFul
{
    public partial class SwaggerEditor : UserControl
    {
        public static string NAME = "Swagger Editor";

        public SwaggerEditor()
        {
            InitializeComponent();
        }

        internal void setSwagger(Swagger swagger)
        {
            this.control_swagger_textBox.Text  = swagger.version;
            this.control_host_textBox.Text     = swagger.host;
            this.control_basePath_textBox.Text = swagger.basePath;

            /* esquemas */
            this.control_http_checkBox.Checked = swagger.schemes.Contains(Swagger.SCHEMA__HTTP);
            this.control_https_checkBox.Checked = swagger.schemes.Contains(Swagger.SCHEMA__HTTPS);
            this.control_ws_checkBox.Checked = swagger.schemes.Contains(Swagger.SCHEMA__WS);
            this.control_wss_checkBox.Checked = swagger.schemes.Contains(Swagger.SCHEMA__WSS);

            /* info servicio */
            this.control_infoServicio_titulo_textBox.Text = swagger.info.title;
            this.control_infoServicio_descripcion_textBox.Text = swagger.info.description;
            this.control_infoServicio_terminos_textBox.Text = swagger.info.termsOfService;
            this.control_infoServicio_version_textBox.Text = swagger.info.version;

            /* contacto */

            this.control_contacto_correo_textBox.Text = swagger.info.contact.email;
            this.control_contacto_nombre_textBox.Text = swagger.info.contact.name;
            this.control_contacto_url_textBox.Text = swagger.info.contact.url;

            foreach( ExternalDoc externalDoc in swagger.externalDocs)
            {
                this.control_documentos_externos_listBox.Text = externalDoc.url +"\r\n";
            }

            /* árbol de recursos */
            var rootNode = new TreeNode(swagger.basePath);
            this.control_recursos_treeView.Nodes.Add(rootNode);

            

            foreach ( var item in swagger.paths)
            {
                addTreeNodeAsPath(item, rootNode);
            }

            
        }

        public void addTreeNodeAsPath(KeyValuePair<string, PathItem> path, TreeNode parent)
        {
            var node = new TreeNode(path.Key);
            parent.Nodes.Add(node);

            addTreeNodeAsMethod(path.Value.delete, node);
            addTreeNodeAsMethod(path.Value.get, node);
            addTreeNodeAsMethod(path.Value.head, node);
            addTreeNodeAsMethod(path.Value.options, node);
            addTreeNodeAsMethod(path.Value.patch, node);
            addTreeNodeAsMethod(path.Value.post, node);
            addTreeNodeAsMethod(path.Value.put, node);

        }

        public void addTreeNodeAsMethod(Operation operation, TreeNode parent)
        {
            if( operation != null)
            {
                var node = new TreeNode(operation._name);
                parent.Nodes.Add(node);
            }
        }

        public void show()
        {
            //FormSwagger formSwagger = new FormSwagger();

            //formSwagger.Show();
        }

        private void SwaggerEditor_Load(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void documentos_expernos_tableLayoutPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void control_listBox_documentos_externos_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
