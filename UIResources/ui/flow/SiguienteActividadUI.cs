using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace UIResources.ui.flow
{
    
    public partial class SiguienteActividadUI : UserControl
    {
        public const string NAME = "Siguiente Actividad";

        public ManualResetEvent caller = null;

        public SiguienteActividadUI()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        public int select = -1;

        public void prepararOpctiones(EA.Element actual, List<string> posiblesSiguientesGuardas, List<object> posiblesSiguiente)
        {
            this.BeginInvoke(new Action(() =>
            {
                int i = 0;

                select = -1;

                reestablecerTableLayout();

                this.buttonActual.Text = actual.Name;

                foreach (object o in posiblesSiguiente)
                {
                    this.tableLayoutPanel3.RowCount += 1;
                    this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
                    
                    // columna, fila
                    this.tableLayoutPanel3.Controls.Add(buildButton(posiblesSiguientesGuardas[i], i), 0, this.tableLayoutPanel3.RowCount - 1);
                    this.tableLayoutPanel3.Controls.Add(buildButton(((EA.Element)o).Name, i), 1, this.tableLayoutPanel3.RowCount - 1);

                    i++;
                }
            }));
        }

        private void reestablecerTableLayout()
        {
            while(tableLayoutPanel3.Controls.Count != 1)
            {
                tableLayoutPanel3.Controls.Remove(tableLayoutPanel3.Controls[tableLayoutPanel3.Controls.Count - 1]);
            }

            while (tableLayoutPanel3.RowStyles.Count != 1)
            {
                tableLayoutPanel3.RowStyles.Remove(tableLayoutPanel3.RowStyles[tableLayoutPanel3.RowStyles.Count - 1]);

                this.tableLayoutPanel3.RowCount -= 1;
            }
        }
        private Button buildButton(string name, int index)
        {
            Button button = new Button();

            button.BackColor = System.Drawing.SystemColors.Window;
            button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            button.Dock = System.Windows.Forms.DockStyle.Fill;
            button.Location = new System.Drawing.Point(3, 3);
            button.Name = ""+ index;
            button.Padding = new System.Windows.Forms.Padding(5);
            button.Size = new System.Drawing.Size(1632, 94);
            button.TabIndex = 0;
            button.Text = name;
            button.UseVisualStyleBackColor = false;
            button.Click += new System.EventHandler(this.onSiguienteSelected);
            //button.DoubleClick += this.onSiguienteSelected;

            return button;
        }
        private void onCancelar(object sender, EventArgs e)
        {
            caller.Set();
        }

        private void onSiguienteSelected(object sender, EventArgs e)
        {
            select = int.Parse(((Button)sender).Name);
            caller.Set();
        }

        private void onSuspender(object sender, EventArgs e)
        {
            select = -10;
            caller.Set();
        }
    }
}
