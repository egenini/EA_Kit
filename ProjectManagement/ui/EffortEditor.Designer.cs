namespace ProjectManagement.ui
{
    partial class EffortEditor
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
            this.button_guardar = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.dateTimePicker_fechaFin = new System.Windows.Forms.DateTimePicker();
            this.numericUpDown_completado = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_asignado = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_utilizado = new System.Windows.Forms.NumericUpDown();
            this.comboBox_esfuerzoTipo = new System.Windows.Forms.ComboBox();
            this.comboBox_dificultad = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label_tipo = new System.Windows.Forms.Label();
            this.label_esperado = new System.Windows.Forms.Label();
            this.label_utilizado = new System.Windows.Forms.Label();
            this.label_asignado = new System.Windows.Forms.Label();
            this.label_completado = new System.Windows.Forms.Label();
            this.comboBox_complejidad = new System.Windows.Forms.ComboBox();
            this.numericUpDown_esperado = new System.Windows.Forms.NumericUpDown();
            this.dateTimePicker_fechaInicio = new System.Windows.Forms.DateTimePicker();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPage.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_completado)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_asignado)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_utilizado)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_esperado)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tabControl, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90.90909F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(774, 875);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.Controls.Add(this.button_guardar, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(768, 73);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // button_guardar
            // 
            this.button_guardar.AutoSize = true;
            this.button_guardar.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button_guardar.BackColor = System.Drawing.SystemColors.Window;
            this.button_guardar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_guardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_guardar.ForeColor = System.Drawing.Color.Transparent;
            this.button_guardar.Image = global::ProjectManagement.Properties.Resources.icons8_Save_32;
            this.button_guardar.Location = new System.Drawing.Point(3, 3);
            this.button_guardar.Name = "button_guardar";
            this.button_guardar.Size = new System.Drawing.Size(250, 67);
            this.button_guardar.TabIndex = 0;
            this.button_guardar.UseVisualStyleBackColor = false;
            this.button_guardar.Click += new System.EventHandler(this.button_guardar_Click);
            this.button_guardar.MouseLeave += new System.EventHandler(this.button_guardar_MouseLeave);
            this.button_guardar.MouseHover += new System.EventHandler(this.button_guardar_MouseHover);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(3, 82);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(768, 790);
            this.tabControl.TabIndex = 1;
            // 
            // tabPage
            // 
            this.tabPage.Controls.Add(this.tableLayoutPanel3);
            this.tabPage.Location = new System.Drawing.Point(10, 48);
            this.tabPage.Name = "tabPage";
            this.tabPage.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage.Size = new System.Drawing.Size(748, 732);
            this.tabPage.TabIndex = 0;
            this.tabPage.Text = "tabPage";
            this.tabPage.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoScroll = true;
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.dateTimePicker_fechaFin, 1, 9);
            this.tableLayoutPanel3.Controls.Add(this.numericUpDown_completado, 1, 7);
            this.tableLayoutPanel3.Controls.Add(this.numericUpDown_asignado, 1, 6);
            this.tableLayoutPanel3.Controls.Add(this.numericUpDown_utilizado, 1, 5);
            this.tableLayoutPanel3.Controls.Add(this.comboBox_esfuerzoTipo, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.comboBox_dificultad, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.label10, 0, 9);
            this.tableLayoutPanel3.Controls.Add(this.label9, 0, 8);
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.label_tipo, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.label_esperado, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this.label_utilizado, 0, 5);
            this.tableLayoutPanel3.Controls.Add(this.label_asignado, 0, 6);
            this.tableLayoutPanel3.Controls.Add(this.label_completado, 0, 7);
            this.tableLayoutPanel3.Controls.Add(this.comboBox_complejidad, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.numericUpDown_esperado, 1, 4);
            this.tableLayoutPanel3.Controls.Add(this.dateTimePicker_fechaInicio, 1, 8);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 10;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(742, 726);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // dateTimePicker_fechaFin
            // 
            this.dateTimePicker_fechaFin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dateTimePicker_fechaFin.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker_fechaFin.Location = new System.Drawing.Point(216, 651);
            this.dateTimePicker_fechaFin.Name = "dateTimePicker_fechaFin";
            this.dateTimePicker_fechaFin.Size = new System.Drawing.Size(523, 38);
            this.dateTimePicker_fechaFin.TabIndex = 18;
            this.dateTimePicker_fechaFin.ValueChanged += new System.EventHandler(this.dateTimePicker_fechaFin_ValueChanged);
            // 
            // numericUpDown_completado
            // 
            this.numericUpDown_completado.AutoSize = true;
            this.numericUpDown_completado.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numericUpDown_completado.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericUpDown_completado.Location = new System.Drawing.Point(216, 507);
            this.numericUpDown_completado.Name = "numericUpDown_completado";
            this.numericUpDown_completado.Size = new System.Drawing.Size(523, 34);
            this.numericUpDown_completado.TabIndex = 16;
            this.numericUpDown_completado.ValueChanged += new System.EventHandler(this.numericUpDown_completado_ValueChanged);
            // 
            // numericUpDown_asignado
            // 
            this.numericUpDown_asignado.AutoSize = true;
            this.numericUpDown_asignado.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numericUpDown_asignado.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericUpDown_asignado.Location = new System.Drawing.Point(216, 435);
            this.numericUpDown_asignado.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown_asignado.Name = "numericUpDown_asignado";
            this.numericUpDown_asignado.Size = new System.Drawing.Size(523, 34);
            this.numericUpDown_asignado.TabIndex = 15;
            this.numericUpDown_asignado.ValueChanged += new System.EventHandler(this.numericUpDown_asignado_ValueChanged);
            // 
            // numericUpDown_utilizado
            // 
            this.numericUpDown_utilizado.AutoSize = true;
            this.numericUpDown_utilizado.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numericUpDown_utilizado.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericUpDown_utilizado.Location = new System.Drawing.Point(216, 363);
            this.numericUpDown_utilizado.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown_utilizado.Name = "numericUpDown_utilizado";
            this.numericUpDown_utilizado.Size = new System.Drawing.Size(523, 34);
            this.numericUpDown_utilizado.TabIndex = 14;
            this.numericUpDown_utilizado.ValueChanged += new System.EventHandler(this.numericUpDown_utilizado_ValueChanged);
            // 
            // comboBox_esfuerzoTipo
            // 
            this.comboBox_esfuerzoTipo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBox_esfuerzoTipo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBox_esfuerzoTipo.FormattingEnabled = true;
            this.comboBox_esfuerzoTipo.Location = new System.Drawing.Point(216, 219);
            this.comboBox_esfuerzoTipo.Name = "comboBox_esfuerzoTipo";
            this.comboBox_esfuerzoTipo.Size = new System.Drawing.Size(523, 39);
            this.comboBox_esfuerzoTipo.TabIndex = 12;
            this.comboBox_esfuerzoTipo.SelectedIndexChanged += new System.EventHandler(this.comboBox_esfuerzoTipo_SelectedIndexChanged);
            // 
            // comboBox_dificultad
            // 
            this.comboBox_dificultad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBox_dificultad.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_dificultad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox_dificultad.FormattingEnabled = true;
            this.comboBox_dificultad.Location = new System.Drawing.Point(216, 75);
            this.comboBox_dificultad.Name = "comboBox_dificultad";
            this.comboBox_dificultad.Size = new System.Drawing.Size(523, 39);
            this.comboBox_dificultad.TabIndex = 11;
            this.comboBox_dificultad.SelectedIndexChanged += new System.EventHandler(this.comboBox_dificultad_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.AutoSize = true;
            this.label10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label10.Location = new System.Drawing.Point(3, 648);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(207, 78);
            this.label10.TabIndex = 9;
            this.label10.Text = "Fecha de fin";
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label9.Location = new System.Drawing.Point(3, 576);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(207, 72);
            this.label9.TabIndex = 8;
            this.label9.Text = "Fecha de inicio";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(207, 72);
            this.label1.TabIndex = 0;
            this.label1.Text = "Complejidad";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label2.Location = new System.Drawing.Point(3, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(207, 72);
            this.label2.TabIndex = 1;
            this.label2.Text = "Dificultad";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel3.SetColumnSpan(this.label4, 2);
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label4.Location = new System.Drawing.Point(3, 144);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(736, 72);
            this.label4.TabIndex = 3;
            this.label4.Text = "Esfuerzo";
            // 
            // label_tipo
            // 
            this.label_tipo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_tipo.AutoSize = true;
            this.label_tipo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label_tipo.Location = new System.Drawing.Point(3, 216);
            this.label_tipo.Name = "label_tipo";
            this.label_tipo.Size = new System.Drawing.Size(207, 72);
            this.label_tipo.TabIndex = 4;
            this.label_tipo.Text = "Tipo";
            // 
            // label_esperado
            // 
            this.label_esperado.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_esperado.AutoSize = true;
            this.label_esperado.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label_esperado.Location = new System.Drawing.Point(3, 288);
            this.label_esperado.Name = "label_esperado";
            this.label_esperado.Size = new System.Drawing.Size(207, 72);
            this.label_esperado.TabIndex = 6;
            this.label_esperado.Text = "Esperado";
            // 
            // label_utilizado
            // 
            this.label_utilizado.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_utilizado.AutoSize = true;
            this.label_utilizado.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label_utilizado.Location = new System.Drawing.Point(3, 360);
            this.label_utilizado.Name = "label_utilizado";
            this.label_utilizado.Size = new System.Drawing.Size(207, 72);
            this.label_utilizado.TabIndex = 5;
            this.label_utilizado.Text = "Utilizado";
            // 
            // label_asignado
            // 
            this.label_asignado.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_asignado.AutoSize = true;
            this.label_asignado.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label_asignado.Location = new System.Drawing.Point(3, 432);
            this.label_asignado.Name = "label_asignado";
            this.label_asignado.Size = new System.Drawing.Size(207, 72);
            this.label_asignado.TabIndex = 7;
            this.label_asignado.Text = "Asignado";
            // 
            // label_completado
            // 
            this.label_completado.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_completado.AutoSize = true;
            this.label_completado.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label_completado.Location = new System.Drawing.Point(3, 504);
            this.label_completado.Name = "label_completado";
            this.label_completado.Size = new System.Drawing.Size(207, 72);
            this.label_completado.TabIndex = 2;
            this.label_completado.Text = "% completado";
            // 
            // comboBox_complejidad
            // 
            this.comboBox_complejidad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBox_complejidad.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_complejidad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox_complejidad.FormattingEnabled = true;
            this.comboBox_complejidad.Location = new System.Drawing.Point(216, 3);
            this.comboBox_complejidad.Name = "comboBox_complejidad";
            this.comboBox_complejidad.Size = new System.Drawing.Size(523, 39);
            this.comboBox_complejidad.TabIndex = 10;
            this.comboBox_complejidad.SelectedIndexChanged += new System.EventHandler(this.comboBox_complejidad_SelectedIndexChanged);
            // 
            // numericUpDown_esperado
            // 
            this.numericUpDown_esperado.AutoSize = true;
            this.numericUpDown_esperado.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numericUpDown_esperado.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericUpDown_esperado.Location = new System.Drawing.Point(216, 291);
            this.numericUpDown_esperado.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown_esperado.Name = "numericUpDown_esperado";
            this.numericUpDown_esperado.Size = new System.Drawing.Size(523, 34);
            this.numericUpDown_esperado.TabIndex = 13;
            this.numericUpDown_esperado.ValueChanged += new System.EventHandler(this.numericUpDown_esperado_ValueChanged);
            // 
            // dateTimePicker_fechaInicio
            // 
            this.dateTimePicker_fechaInicio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dateTimePicker_fechaInicio.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker_fechaInicio.Location = new System.Drawing.Point(216, 579);
            this.dateTimePicker_fechaInicio.Name = "dateTimePicker_fechaInicio";
            this.dateTimePicker_fechaInicio.Size = new System.Drawing.Size(523, 38);
            this.dateTimePicker_fechaInicio.TabIndex = 17;
            this.dateTimePicker_fechaInicio.ValueChanged += new System.EventHandler(this.dateTimePicker_fechaInicio_ValueChanged);
            // 
            // EffortEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "EffortEditor";
            this.Size = new System.Drawing.Size(774, 875);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabPage.ResumeLayout(false);
            this.tabPage.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_completado)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_asignado)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_utilizado)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_esperado)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button button_guardar;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.DateTimePicker dateTimePicker_fechaFin;
        private System.Windows.Forms.NumericUpDown numericUpDown_completado;
        private System.Windows.Forms.NumericUpDown numericUpDown_asignado;
        private System.Windows.Forms.NumericUpDown numericUpDown_utilizado;
        private System.Windows.Forms.ComboBox comboBox_esfuerzoTipo;
        private System.Windows.Forms.ComboBox comboBox_dificultad;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label_tipo;
        private System.Windows.Forms.Label label_esperado;
        private System.Windows.Forms.Label label_utilizado;
        private System.Windows.Forms.Label label_asignado;
        private System.Windows.Forms.Label label_completado;
        private System.Windows.Forms.ComboBox comboBox_complejidad;
        private System.Windows.Forms.NumericUpDown numericUpDown_esperado;
        private System.Windows.Forms.DateTimePicker dateTimePicker_fechaInicio;
    }
}
