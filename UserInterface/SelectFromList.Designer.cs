namespace UserInterface
{
    partial class SelectFromList
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel_base = new System.Windows.Forms.TableLayoutPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.flowLayoutPanel_buttons = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel_base.SuspendLayout();
            this.flowLayoutPanel_buttons.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel_base
            // 
            this.tableLayoutPanel_base.ColumnCount = 2;
            this.tableLayoutPanel_base.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 81.81818F));
            this.tableLayoutPanel_base.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18.18182F));
            this.tableLayoutPanel_base.Controls.Add(this.button1, 1, 1);
            this.tableLayoutPanel_base.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel_base.Controls.Add(this.flowLayoutPanel_buttons, 0, 1);
            this.tableLayoutPanel_base.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_base.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel_base.Name = "tableLayoutPanel_base";
            this.tableLayoutPanel_base.RowCount = 2;
            this.tableLayoutPanel_base.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel_base.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90F));
            this.tableLayoutPanel_base.Size = new System.Drawing.Size(1150, 584);
            this.tableLayoutPanel_base.TabIndex = 4;
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button1.BackColor = System.Drawing.Color.Red;
            this.button1.Font = new System.Drawing.Font("Calibri", 9.900001F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.SystemColors.Window;
            this.button1.Location = new System.Drawing.Point(943, 511);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(204, 70);
            this.button1.TabIndex = 1;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.tableLayoutPanel_base.SetColumnSpan(this.label2, 2);
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Calibri", 14.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(1144, 58);
            this.label2.TabIndex = 2;
            this.label2.Text = "Choose from list";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // flowLayoutPanel_buttons
            // 
            this.flowLayoutPanel_buttons.Controls.Add(this.flowLayoutPanel1);
            this.flowLayoutPanel_buttons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel_buttons.Location = new System.Drawing.Point(3, 61);
            this.flowLayoutPanel_buttons.Name = "flowLayoutPanel_buttons";
            this.flowLayoutPanel_buttons.Size = new System.Drawing.Size(934, 520);
            this.flowLayoutPanel_buttons.TabIndex = 3;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1067, 0);
            this.flowLayoutPanel1.TabIndex = 4;
            // 
            // SelectFromList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tableLayoutPanel_base);
            this.Name = "SelectFromList";
            this.Size = new System.Drawing.Size(1150, 584);
            this.tableLayoutPanel_base.ResumeLayout(false);
            this.tableLayoutPanel_base.PerformLayout();
            this.flowLayoutPanel_buttons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_base;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel_buttons;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}
