using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DMN.framework;
using DMN.framework.domain;

namespace DMN
{
    public partial class ChooseLanguage : UserControl
    {
        private chooseLanguageForm chooseLanguageForm;
        List<Button> buttons = new List<Button>();

        public ChooseLanguage()
        {
            InitializeComponent();
            this.label1.Text = Properties.Resources.elegir_lenguaje_window_title;
            this.button1.Text = Properties.Resources.elegir_lenguaje_window_button_cancel_label;
        }

        public void addButtonLanguage( string name)
        {
            Button button = new Button();

            button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            button.Name = name;
            button.Size = new System.Drawing.Size(1061, 62);
            button.TabIndex = buttons.Count + 1;
            button.Text = name;
            button.UseVisualStyleBackColor = true;
            button.Click += languageSelected;

            button.MouseHover += (s, e) =>
            {
                button.BackColor = Color.LightSkyBlue;
            };
            button.MouseLeave += (s, e) =>
            {
                button.BackColor = Color.White;
            };
            this.flowLayoutPanel_buttons.Controls.Add(button);

            buttons.Add(button);

        }

        public void languageSelected(object sender, EventArgs e)
        {
            this.chooseLanguageForm.setLanguageSelected(((Button)sender).Name);
            
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.chooseLanguageForm != null)
            {
                this.chooseLanguageForm.Close();
            }
        }

        internal void setFramework(Framework framework, bool generateSourceCode)
        {
            foreach( Language language in framework.languages)
            {
                if( generateSourceCode && language.name != framework.feelLanguage.name)
                {
                    this.addButtonLanguage(language.name);
                }
            }
        }

        internal void setParent(chooseLanguageForm chooseLanguageForm)
        {
            this.chooseLanguageForm = chooseLanguageForm;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel_base_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
