namespace Productividad
{
    partial class JsonXmlViewer
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.toPascal = new System.Windows.Forms.RadioButton();
            this.toCamel = new System.Windows.Forms.RadioButton();
            this.toSnake = new System.Windows.Forms.RadioButton();
            this.toDot = new System.Windows.Forms.RadioButton();
            this.refresh = new System.Windows.Forms.Button();
            this.copiar = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoScroll = true;
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.40817F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 79.59184F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(845, 975);
            this.tableLayoutPanel1.TabIndex = 0;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.Controls.Add(this.toPascal, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.toCamel, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.toSnake, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.toDot, 3, 1);
            this.tableLayoutPanel2.Controls.Add(this.refresh, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.copiar, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(4, 4);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(837, 190);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // toPascal
            // 
            this.toPascal.AutoSize = true;
            this.toPascal.Location = new System.Drawing.Point(4, 99);
            this.toPascal.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.toPascal.Name = "toPascal";
            this.toPascal.Size = new System.Drawing.Size(157, 43);
            this.toPascal.TabIndex = 1;
            this.toPascal.TabStop = true;
            this.toPascal.Text = "Pascal";
            this.toPascal.UseVisualStyleBackColor = true;
            this.toPascal.CheckedChanged += new System.EventHandler(this.toPascal_CheckedChanged);
            // 
            // toCamel
            // 
            this.toCamel.AutoSize = true;
            this.toCamel.Location = new System.Drawing.Point(213, 99);
            this.toCamel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.toCamel.Name = "toCamel";
            this.toCamel.Size = new System.Drawing.Size(145, 43);
            this.toCamel.TabIndex = 2;
            this.toCamel.TabStop = true;
            this.toCamel.Text = "camel";
            this.toCamel.UseVisualStyleBackColor = true;
            this.toCamel.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // toSnake
            // 
            this.toSnake.AutoSize = true;
            this.toSnake.Location = new System.Drawing.Point(422, 99);
            this.toSnake.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.toSnake.Name = "toSnake";
            this.toSnake.Size = new System.Drawing.Size(164, 43);
            this.toSnake.TabIndex = 3;
            this.toSnake.TabStop = true;
            this.toSnake.Text = "_snake";
            this.toSnake.UseVisualStyleBackColor = true;
            this.toSnake.CheckedChanged += new System.EventHandler(this.toSnake_CheckedChanged);
            // 
            // toDot
            // 
            this.toDot.AutoSize = true;
            this.toDot.Location = new System.Drawing.Point(631, 99);
            this.toDot.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.toDot.Name = "toDot";
            this.toDot.Size = new System.Drawing.Size(101, 43);
            this.toDot.TabIndex = 4;
            this.toDot.TabStop = true;
            this.toDot.Text = "dot";
            this.toDot.UseVisualStyleBackColor = true;
            this.toDot.CheckedChanged += new System.EventHandler(this.toDot_CheckedChanged);
            // 
            // refresh
            // 
            this.refresh.AutoSize = true;
            this.refresh.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.refresh.Dock = System.Windows.Forms.DockStyle.Fill;
            this.refresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refresh.Image = global::Productividad.Properties.Resources.icons8_Refresh_32;
            this.refresh.Location = new System.Drawing.Point(213, 4);
            this.refresh.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.refresh.Name = "refresh";
            this.refresh.Size = new System.Drawing.Size(201, 87);
            this.refresh.TabIndex = 0;
            this.refresh.UseVisualStyleBackColor = true;
            this.refresh.Click += new System.EventHandler(this.refresh_Click);
            // 
            // copiar
            // 
            this.copiar.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.copiar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.copiar.Image = global::Productividad.Properties.Resources.icons8_Copy_to_Clipboard_32;
            this.copiar.Location = new System.Drawing.Point(4, 4);
            this.copiar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.copiar.Name = "copiar";
            this.copiar.Size = new System.Drawing.Size(201, 87);
            this.copiar.TabIndex = 5;
            this.copiar.UseVisualStyleBackColor = true;
            this.copiar.Click += new System.EventHandler(this.copiar_Click);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(4, 202);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(837, 769);
            this.panel1.TabIndex = 3;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(837, 769);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Location = new System.Drawing.Point(10, 55);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage1.Size = new System.Drawing.Size(817, 704);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Texto";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.900001F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(4, 4);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(809, 696);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.treeView1);
            this.tabPage2.Location = new System.Drawing.Point(10, 48);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage2.Size = new System.Drawing.Size(817, 709);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Árbol";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(7, 7);
            this.treeView1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(796, 865);
            this.treeView1.TabIndex = 1;
            // 
            // JsonXmlViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(19F, 38F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.900001F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "JsonXmlViewer";
            this.Size = new System.Drawing.Size(845, 975);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button refresh;
        private System.Windows.Forms.RadioButton toPascal;
        private System.Windows.Forms.RadioButton toCamel;
        private System.Windows.Forms.RadioButton toSnake;
        private System.Windows.Forms.RadioButton toDot;
        private System.Windows.Forms.Button copiar;
        private System.Windows.Forms.Panel panel1;
    }
}
