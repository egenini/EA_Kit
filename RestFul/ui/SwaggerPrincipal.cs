using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EA;
using EAUtils;
using RestFul.modelo;

namespace RestFul.ui
{
    public partial class SwaggerPrincipal : UserControl
    {
        public SwaggerPrincipal()
        {
            InitializeComponent();
        }

        public const string NAME = "Swagger service";

        public Package definitionPackage = null;
        public Package rootPackage = null;
        public Element swaggerClass = null;

        public void show(Element servicio, EAUtils.EAUtils eaUtils, Main main)
        {
            // busco la clase Swagger para obtener los datos para info.
            // recorro las interfaces embebidas para buscar la/s uri's (todas deberían apuntar a las misma/s)
            try
            {
                definitionPackage = eaUtils.repository.GetPackageByID(servicio.PackageID);
                rootPackage       = eaUtils.repository.GetPackageByID(definitionPackage.ParentID);

                SwaggerMaster swaggerMaster = new SwaggerMaster(eaUtils);

                swaggerMaster.readServiceFromEA(rootPackage);

                this.swaggerMain.setSwaggerMaster(swaggerMaster);
                this.swaggerInfo.setSwaggerMaster(swaggerMaster);

            }
            catch (Exception e ) { eaUtils.printOut(e.ToString()); }

        }

        internal void disable()
        {
            this.swaggerMain.disable();
            this.swaggerInfo.disable();
        }

        private void swaggerInfo_Load(object sender, EventArgs e)
        {

        }

        private void control_service_tabPage_Click(object sender, EventArgs e)
        {

        }

        private void main_tableLayoutPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void control_service_tabPage_Click_1(object sender, EventArgs e)
        {

        }

        private void control_button_guardar_Click(object sender, EventArgs e)
        {
            this.swaggerMain.guardar();
            this.swaggerInfo.guardar();

        }
    }
}
