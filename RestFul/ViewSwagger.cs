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
    public partial class ViewSwagger : UserControl
    {
        FormSwagger formSwagger;
        public static string NAME = "Ver Swagger";

        public ViewSwagger()
        {
            InitializeComponent();
            
        }

        public void setSwagger(Swagger swagger)
        {
            this.formSwagger = new FormSwagger();
            formSwagger.setSwagger(swagger);
            this.Controls.Add(formSwagger);
        }
    }
}
