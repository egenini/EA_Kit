using RestFul.modelo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RestFul
{
    public partial class FormSwagger : Form
    {
        private Swagger swagger;

        public FormSwagger()
        {
            InitializeComponent();
        }

        public void setSwagger(modelo.Swagger swagger)
        {
            this.swagger = swagger;

            this.swaggerVersion.Text = swagger.version;
            this.host.Text = swagger.host;
            this.basePath.Text = swagger.basePath;

            this.infoTitulo.Text = swagger.info.title;
            this.infoDescription.Text = swagger.info.description;
            this.infoTermsOfService.Text = swagger.info.termsOfService;
            this.infoVersion.Text = swagger.info.version;
            this.contactName.Text = swagger.info.contact.name;
            this.contactEmail.Text = swagger.info.contact.email;
            this.contactUrl.Text = swagger.info.contact.url;
            this.licenceName.Text = swagger.info.license.name;
            this.licenceUrl.Text = swagger.info.license.url;

            this.http.Checked = (swagger.schemes.Contains("http"));
            this.https.Checked = (swagger.schemes.Contains("https"));
            this.ws.Checked = (swagger.schemes.Contains("ws"));
            this.wss.Checked = (swagger.schemes.Contains("wss"));
        }

        private void cancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aceptar_Click(object sender, EventArgs e)
        {
            swagger.version  = this.swaggerVersion.Text;
            swagger.host     = this.host.Text;
            swagger.basePath = this.basePath.Text;

            swagger.info.title          = this.infoTitulo.Text;
            swagger.info.description    = this.infoDescription.Text;
            swagger.info.termsOfService = this.infoTermsOfService.Text;
            swagger.info.version        = this.infoVersion.Text;
            swagger.info.contact.name   = this.contactName.Text;
            swagger.info.contact.email  = this.contactEmail.Text;
            swagger.info.contact.url    = this.contactUrl.Text;
            swagger.info.license.name   = this.licenceName.Text;
            swagger.info.license.url    = this.licenceUrl.Text;

        }

        private void general_Click(object sender, EventArgs e)
        {

        }
    }
}
