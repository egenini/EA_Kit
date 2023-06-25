using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UIResources
{
    public partial class WebBrowser : UserControl
    {
        public static string NAME = "Web Browser";
        public WebBrowser()
        {
            /*
            string UserDataFolder;
            UserDataFolder = "C:\\MyAppUserDataFolder";
            var _task = CoreWebView2Environment.CreateAsync("BrowserExecutableFolder="+ UserDataFolder );
            */
            InitializeComponent();
        }

        private void onSourceChanged(object sender, Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs e)
        {
            // nada
        }
    }
}
