using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RestFul.modelo;
using RestFul.ea;

namespace RestFul.ui
{
    public partial class SwaggerInfo : UserControl
    {
        public SwaggerInfo()
        {
            InitializeComponent();
        }

        Swagger swagger = null;
        SwaggerMaster swaggerMaster = null;

        internal void setSwaggerMaster(SwaggerMaster swaggerMaster)
        {
            this.swagger       = swaggerMaster.swagger;
            this.swaggerMaster = swaggerMaster;

            this.control_infoServicio_descripcion_textBox.Text = swagger.info.description;
            this.control_infoServicio_terminos_textBox.Text    = swagger.info.termsOfService;
            this.control_infoServicio_titulo_textBox.Text      = swagger.info.title;
            this.control_infoServicio_version_textBox.Text     = swagger.info.version;

            this.control_contacto_correo_textBox.Text = swagger.info.contact.email;
            this.control_contacto_nombre_textBox.Text = swagger.info.contact.name;
            this.control_contacto_url_textBox.Text    = swagger.info.contact.url;

            this.control_licencia_nombre_textBox.Text = swagger.info.license.name;
            this.control_licencia_url_textBox.Text    = swagger.info.license.url;

            this.control_infoServicio_descripcion_textBox.Enabled = true;
            this.control_infoServicio_terminos_textBox.Enabled    = true;
            this.control_infoServicio_titulo_textBox.Enabled      = true;
            this.control_infoServicio_version_textBox.Enabled     = true;

            this.control_contacto_correo_textBox.Enabled = true;
            this.control_contacto_nombre_textBox.Enabled = true;
            this.control_contacto_url_textBox.Enabled    = true;

            this.control_licencia_nombre_textBox.Enabled = true;
            this.control_licencia_url_textBox.Enabled    = true;
        }

        public void disable()
        {
            this.control_infoServicio_descripcion_textBox.Enabled = false;
            this.control_infoServicio_terminos_textBox.Enabled    = false;
            this.control_infoServicio_titulo_textBox.Enabled      = false;
            this.control_infoServicio_version_textBox.Enabled     = false;

            this.control_contacto_correo_textBox.Enabled = false;
            this.control_contacto_nombre_textBox.Enabled = false;
            this.control_contacto_url_textBox.Enabled    = false;

            this.control_licencia_nombre_textBox.Enabled = false;
            this.control_licencia_url_textBox.Enabled    = false;
        }

        public bool isDescriptionChanged()
        {
            return this.control_infoServicio_descripcion_textBox.Text != swagger.info.description;
        }
        public bool isTerminosChanged()
        {
            return this.control_infoServicio_terminos_textBox.Text != swagger.info.termsOfService;
        }
        public bool isTituloChanged()
        {
            return this.control_infoServicio_titulo_textBox.Text != swagger.info.title;
        }
        public bool isVersionChangedd()
        {
            return this.control_infoServicio_version_textBox.Text != swagger.info.version;
        }
        public bool isCorreoChanged()
        {
            return this.control_contacto_correo_textBox.Text != swagger.info.contact.email;
        }
        public bool isContactoNombreChanged()
        {
            return this.control_contacto_nombre_textBox.Text != swagger.info.contact.name;
        }
        public bool isContactoUrlChanded()
        {
            return this.control_contacto_url_textBox.Text != swagger.info.contact.url;
        }
        public bool isLicenciaNombreChanged()
        {
            return this.control_licencia_nombre_textBox.Text != swagger.info.license.name;
        }
        public bool isLicenciaUrlChanged()
        {
            return this.control_licencia_url_textBox.Text != swagger.info.license.url;
        }

        public void guardar()
        {
            if( isDescriptionChanged())
            {
                this.swaggerMaster.serviceModel.synchronizeInfoAttribute(ServiceModel.INFO__DESCRIPTION, this.control_infoServicio_descripcion_textBox.Text);
            }
            if( isTerminosChanged() )
            {
                this.swaggerMaster.serviceModel.synchronizeInfoAttribute(ServiceModel.INFO__TERMS_OF_SERVICE, this.control_infoServicio_terminos_textBox.Text);
            }
            if (isTituloChanged())
            {
                this.swaggerMaster.serviceModel.synchronizeInfoAttribute(ServiceModel.INFO__TITLE , this.control_infoServicio_titulo_textBox.Text);
            }
            if (isVersionChangedd())
            {
                this.swaggerMaster.serviceModel.synchronizeInfoAttribute(ServiceModel.INFO__VERSION ,this.control_infoServicio_version_textBox.Text);
            }
            if (isCorreoChanged())
            {
                this.swaggerMaster.serviceModel.synchronizeContactAttribute(ServiceModel.CONTACT__EMAIL, this.control_contacto_correo_textBox.Text);
            }
            if (isContactoNombreChanged())
            {
                this.swaggerMaster.serviceModel.synchronizeContactAttribute(ServiceModel.CONTACT__NAME, this.control_contacto_nombre_textBox.Text);
            }
            if (isContactoUrlChanded())
            {
                this.swaggerMaster.serviceModel.synchronizeContactAttribute(ServiceModel.CONTACT__URL, this.control_contacto_url_textBox.Text);
            }
            if( isLicenciaNombreChanged())
            {
                this.swaggerMaster.serviceModel.synchronizeLicenseAttribute(ServiceModel.LICENSE__NAME , this.control_contacto_nombre_textBox.Text);
            }
            if( isLicenciaUrlChanged())
            {
                this.swaggerMaster.serviceModel.synchronizeLicenseAttribute(ServiceModel.LICENSE__URL, this.control_licencia_url_textBox.Text);
            }
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void control_contacto_groupBox_Enter(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void control_infoServicio_descripcion_textBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void control_infoServicio_descripcion_label_Click(object sender, EventArgs e)
        {

        }
    }
}
