using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace APQC
{
    public partial class AboutBox : Form
    {
        string @productPageUrl;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productName">Nombre del addin</param>
        /// <param name="productPageUrl">URL de la página del addin</param>
        /// <param name="productVersion">Versión del addin</param>
        /// <param name="licenceTimeRemaining">El tiempo que queda de licencia, si es null queda en blanco</param>
        public AboutBox( string productName, string @productPageUrl, string productVersion, string licenceTimeRemaining)
        {
            InitializeComponent();
            this.Text = productName;
            this.@productPageUrl = @productPageUrl;
            this.linkLabelProduct.Text = productName;
            this.labelVersionValue.Text = productVersion;
            this.labelTimeRemainingValue.Text = licenceTimeRemaining == null ? "" : licenceTimeRemaining;
        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkLabel.LinkVisited = true;
            System.Diagnostics.Process.Start("http://www.proagile.com.ar");
        }

        private void linkLabelProduct_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(this.@productPageUrl);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
