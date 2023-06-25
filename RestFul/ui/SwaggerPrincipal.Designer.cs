﻿namespace RestFul.ui
{
    partial class SwaggerPrincipal
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
            this.main_tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.tabControl_main = new System.Windows.Forms.TabControl();
            this.control_service_tabPage = new System.Windows.Forms.TabPage();
            this.control_info_tabPage = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.control_button_guardar = new System.Windows.Forms.Button();
            this.swaggerMain = new RestFul.ui.SwaggerMain();
            this.swaggerInfo = new RestFul.ui.SwaggerInfo();
            this.main_tableLayoutPanel.SuspendLayout();
            this.tabControl_main.SuspendLayout();
            this.control_service_tabPage.SuspendLayout();
            this.control_info_tabPage.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // main_tableLayoutPanel
            // 
            this.main_tableLayoutPanel.AutoSize = true;
            this.main_tableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.main_tableLayoutPanel.ColumnCount = 1;
            this.main_tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.main_tableLayoutPanel.Controls.Add(this.tabControl_main, 0, 1);
            this.main_tableLayoutPanel.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.main_tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.main_tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.main_tableLayoutPanel.Name = "main_tableLayoutPanel";
            this.main_tableLayoutPanel.RowCount = 2;
            this.main_tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.07194F));
            this.main_tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 89.92805F));
            this.main_tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.main_tableLayoutPanel.Size = new System.Drawing.Size(853, 1082);
            this.main_tableLayoutPanel.TabIndex = 1;
            this.main_tableLayoutPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.main_tableLayoutPanel_Paint);
            // 
            // tabControl_main
            // 
            this.tabControl_main.Controls.Add(this.control_service_tabPage);
            this.tabControl_main.Controls.Add(this.control_info_tabPage);
            this.tabControl_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_main.Font = new System.Drawing.Font("Calibri", 9.900001F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl_main.Location = new System.Drawing.Point(3, 111);
            this.tabControl_main.Name = "tabControl_main";
            this.tabControl_main.SelectedIndex = 0;
            this.tabControl_main.Size = new System.Drawing.Size(847, 968);
            this.tabControl_main.TabIndex = 1;
            // 
            // control_service_tabPage
            // 
            this.control_service_tabPage.Controls.Add(this.swaggerMain);
            this.control_service_tabPage.Font = new System.Drawing.Font("Calibri", 8.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.control_service_tabPage.Location = new System.Drawing.Point(10, 57);
            this.control_service_tabPage.Name = "control_service_tabPage";
            this.control_service_tabPage.Padding = new System.Windows.Forms.Padding(3);
            this.control_service_tabPage.Size = new System.Drawing.Size(827, 901);
            this.control_service_tabPage.TabIndex = 0;
            this.control_service_tabPage.Text = "Servicio";
            this.control_service_tabPage.UseVisualStyleBackColor = true;
            this.control_service_tabPage.Click += new System.EventHandler(this.control_service_tabPage_Click_1);
            // 
            // control_info_tabPage
            // 
            this.control_info_tabPage.AutoScroll = true;
            this.control_info_tabPage.BackColor = System.Drawing.SystemColors.Window;
            this.control_info_tabPage.Controls.Add(this.swaggerInfo);
            this.control_info_tabPage.Location = new System.Drawing.Point(10, 57);
            this.control_info_tabPage.Name = "control_info_tabPage";
            this.control_info_tabPage.Padding = new System.Windows.Forms.Padding(3);
            this.control_info_tabPage.Size = new System.Drawing.Size(827, 901);
            this.control_info_tabPage.TabIndex = 1;
            this.control_info_tabPage.Text = "Alert";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Controls.Add(this.control_button_guardar, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(847, 102);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // control_button_guardar
            // 
            this.control_button_guardar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.control_button_guardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.control_button_guardar.Location = new System.Drawing.Point(341, 3);
            this.control_button_guardar.Name = "control_button_guardar";
            this.control_button_guardar.Size = new System.Drawing.Size(163, 96);
            this.control_button_guardar.TabIndex = 0;
            this.control_button_guardar.Text = "Guardar";
            this.control_button_guardar.UseVisualStyleBackColor = true;
            this.control_button_guardar.Click += new System.EventHandler(this.control_button_guardar_Click);
            // 
            // swaggerMain
            // 
            this.swaggerMain.AutoSize = true;
            this.swaggerMain.BackColor = System.Drawing.SystemColors.Window;
            this.swaggerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.swaggerMain.Location = new System.Drawing.Point(3, 3);
            this.swaggerMain.Name = "swaggerMain";
            this.swaggerMain.Size = new System.Drawing.Size(821, 895);
            this.swaggerMain.TabIndex = 0;
            // 
            // swaggerInfo
            // 
            this.swaggerInfo.AutoScroll = true;
            this.swaggerInfo.AutoSize = true;
            this.swaggerInfo.BackColor = System.Drawing.SystemColors.Window;
            this.swaggerInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.swaggerInfo.Font = new System.Drawing.Font("Calibri", 8.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.swaggerInfo.Location = new System.Drawing.Point(3, 3);
            this.swaggerInfo.Name = "swaggerInfo";
            this.swaggerInfo.Size = new System.Drawing.Size(821, 895);
            this.swaggerInfo.TabIndex = 0;
            // 
            // SwaggerPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 33F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.main_tableLayoutPanel);
            this.Font = new System.Drawing.Font("Calibri", 8.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SwaggerPrincipal";
            this.Size = new System.Drawing.Size(853, 1082);
            this.main_tableLayoutPanel.ResumeLayout(false);
            this.tabControl_main.ResumeLayout(false);
            this.control_service_tabPage.ResumeLayout(false);
            this.control_service_tabPage.PerformLayout();
            this.control_info_tabPage.ResumeLayout(false);
            this.control_info_tabPage.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel main_tableLayoutPanel;
        private System.Windows.Forms.TabControl tabControl_main;
        private System.Windows.Forms.TabPage control_service_tabPage;
        private System.Windows.Forms.TabPage control_info_tabPage;
        private SwaggerInfo swaggerInfo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button control_button_guardar;
        private SwaggerMain swaggerMain;
    }
}
