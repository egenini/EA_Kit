﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UIResources
{
    public partial class SelectFromList : UserControl
    {
        private SelectFromListForm selectFromListForm;

        List<Button> buttons = new List<Button>();

        public SelectFromList()
        {
            InitializeComponent();
        }

        public void addButtonChoose(string name)
        {
            Button button = new Button();

            button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            button.Name = name;
            button.Size = new System.Drawing.Size(1061, 62);
            button.TabIndex = buttons.Count + 1;
            button.Text = name;
            button.UseVisualStyleBackColor = true;
            button.Click += selected;

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
        public void selected(object sender, EventArgs e)
        {
            this.selectFromListForm.setSelected(((Button)sender).Name);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.selectFromListForm != null)
            {
                this.selectFromListForm.Close();
            }
        }

        internal void setOptionList(List<String> names)
        {
            foreach (String name in names)
            {
                    this.addButtonChoose(name);
            }
        }

        internal void setParent(SelectFromListForm selectFromListForm)
        {
            this.selectFromListForm = selectFromListForm;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
