namespace RestFul
{
    partial class FormSwagger
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSwagger));
            this.basePath = new System.Windows.Forms.TextBox();
            this.http = new System.Windows.Forms.CheckBox();
            this.host = new System.Windows.Forms.TextBox();
            this.swaggerVersion = new System.Windows.Forms.TextBox();
            this.https = new System.Windows.Forms.CheckBox();
            this.ws = new System.Windows.Forms.CheckBox();
            this.wss = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.licenceUrl = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.licenceName = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.contacotGroupBox = new System.Windows.Forms.GroupBox();
            this.contactUrl = new System.Windows.Forms.TextBox();
            this.contactEmail = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.contactName = new System.Windows.Forms.TextBox();
            this.infoVersion = new System.Windows.Forms.TextBox();
            this.infoTermsOfService = new System.Windows.Forms.TextBox();
            this.infoDescription = new System.Windows.Forms.TextBox();
            this.infoTitulo = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.Swagger = new System.Windows.Forms.TabControl();
            this.general = new System.Windows.Forms.TabPage();
            this.label13 = new System.Windows.Forms.Label();
            this.externalDocsPanel = new System.Windows.Forms.Panel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.aceptar = new System.Windows.Forms.Button();
            this.cancelar = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.contacotGroupBox.SuspendLayout();
            this.Swagger.SuspendLayout();
            this.general.SuspendLayout();
            this.SuspendLayout();
            // 
            // basePath
            // 
            this.basePath.Location = new System.Drawing.Point(192, 199);
            this.basePath.Name = "basePath";
            this.basePath.Size = new System.Drawing.Size(513, 38);
            this.basePath.TabIndex = 1;
            this.basePath.Text = "/rootResource/V1";
            this.toolTip1.SetToolTip(this.basePath, resources.GetString("basePath.ToolTip"));
            // 
            // http
            // 
            this.http.AutoSize = true;
            this.http.Location = new System.Drawing.Point(92, 340);
            this.http.Name = "http";
            this.http.Size = new System.Drawing.Size(101, 36);
            this.http.TabIndex = 2;
            this.http.Text = "http";
            this.http.UseVisualStyleBackColor = true;
            // 
            // host
            // 
            this.host.Location = new System.Drawing.Point(192, 117);
            this.host.Name = "host";
            this.host.Size = new System.Drawing.Size(513, 38);
            this.host.TabIndex = 3;
            this.host.Text = "xxx.com";
            this.toolTip1.SetToolTip(this.host, resources.GetString("host.ToolTip"));
            // 
            // swaggerVersion
            // 
            this.swaggerVersion.Location = new System.Drawing.Point(192, 31);
            this.swaggerVersion.Name = "swaggerVersion";
            this.swaggerVersion.Size = new System.Drawing.Size(513, 38);
            this.swaggerVersion.TabIndex = 4;
            this.swaggerVersion.Text = "2.0";
            this.toolTip1.SetToolTip(this.swaggerVersion, "Especifica la versión de Swagger Specification que se está utilizando. Puede ser " +
        "utilizado por la interfaz de Swagger y otros clientes para interpretar la lista " +
        "de API. El valor DEBE ser \"2.0\".");
            // 
            // https
            // 
            this.https.AutoSize = true;
            this.https.Location = new System.Drawing.Point(212, 60);
            this.https.Name = "https";
            this.https.Size = new System.Drawing.Size(115, 36);
            this.https.TabIndex = 5;
            this.https.Text = "https";
            this.https.UseVisualStyleBackColor = true;
            // 
            // ws
            // 
            this.ws.AutoSize = true;
            this.ws.Location = new System.Drawing.Point(387, 60);
            this.ws.Name = "ws";
            this.ws.Size = new System.Drawing.Size(87, 36);
            this.ws.TabIndex = 6;
            this.ws.Text = "ws";
            this.ws.UseVisualStyleBackColor = true;
            // 
            // wss
            // 
            this.wss.AutoSize = true;
            this.wss.Location = new System.Drawing.Point(552, 60);
            this.wss.Name = "wss";
            this.wss.Size = new System.Drawing.Size(101, 36);
            this.wss.TabIndex = 7;
            this.wss.Text = "wss";
            this.wss.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.wss);
            this.groupBox1.Controls.Add(this.ws);
            this.groupBox1.Controls.Add(this.https);
            this.groupBox1.Location = new System.Drawing.Point(46, 280);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(659, 150);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Esquemas";
            this.toolTip1.SetToolTip(this.groupBox1, resources.GetString("groupBox1.ToolTip"));
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 32);
            this.label1.TabIndex = 9;
            this.label1.Text = "Swagger";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 120);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 32);
            this.label2.TabIndex = 10;
            this.label2.Text = "Host";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(40, 199);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(143, 32);
            this.label3.TabIndex = 11;
            this.label3.Text = "Base path";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.contacotGroupBox);
            this.groupBox2.Controls.Add(this.infoVersion);
            this.groupBox2.Controls.Add(this.infoTermsOfService);
            this.groupBox2.Controls.Add(this.infoDescription);
            this.groupBox2.Controls.Add(this.infoTitulo);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(728, 16);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1258, 424);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Alert del servicio";
            this.toolTip1.SetToolTip(this.groupBox2, "Proporciona metadatos sobre la API. Los metadatos pueden ser utilizados por los c" +
        "lientes si es necesario.");
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.licenceUrl);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.licenceName);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Location = new System.Drawing.Point(625, 243);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(610, 171);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Licencia";
            // 
            // licenceUrl
            // 
            this.licenceUrl.Location = new System.Drawing.Point(171, 96);
            this.licenceUrl.Name = "licenceUrl";
            this.licenceUrl.Size = new System.Drawing.Size(420, 38);
            this.licenceUrl.TabIndex = 4;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(21, 99);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(71, 32);
            this.label12.TabIndex = 3;
            this.label12.Text = "URL";
            // 
            // licenceName
            // 
            this.licenceName.Location = new System.Drawing.Point(171, 46);
            this.licenceName.Name = "licenceName";
            this.licenceName.Size = new System.Drawing.Size(420, 38);
            this.licenceName.TabIndex = 1;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(21, 52);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(115, 32);
            this.label11.TabIndex = 0;
            this.label11.Text = "Nombre";
            // 
            // contacotGroupBox
            // 
            this.contacotGroupBox.Controls.Add(this.contactUrl);
            this.contacotGroupBox.Controls.Add(this.contactEmail);
            this.contacotGroupBox.Controls.Add(this.label10);
            this.contacotGroupBox.Controls.Add(this.label9);
            this.contacotGroupBox.Controls.Add(this.label8);
            this.contacotGroupBox.Controls.Add(this.contactName);
            this.contacotGroupBox.Location = new System.Drawing.Point(625, 21);
            this.contacotGroupBox.Name = "contacotGroupBox";
            this.contacotGroupBox.Size = new System.Drawing.Size(610, 200);
            this.contacotGroupBox.TabIndex = 8;
            this.contacotGroupBox.TabStop = false;
            this.contacotGroupBox.Text = "Contacto";
            // 
            // contactUrl
            // 
            this.contactUrl.Location = new System.Drawing.Point(156, 156);
            this.contactUrl.Name = "contactUrl";
            this.contactUrl.Size = new System.Drawing.Size(448, 38);
            this.contactUrl.TabIndex = 5;
            // 
            // contactEmail
            // 
            this.contactEmail.Location = new System.Drawing.Point(156, 104);
            this.contactEmail.Name = "contactEmail";
            this.contactEmail.Size = new System.Drawing.Size(448, 38);
            this.contactEmail.TabIndex = 4;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(22, 107);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(101, 32);
            this.label10.TabIndex = 3;
            this.label10.Text = "Correo";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(22, 159);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(71, 32);
            this.label9.TabIndex = 2;
            this.label9.Text = "URL";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(22, 55);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(115, 32);
            this.label8.TabIndex = 1;
            this.label8.Text = "Nombre";
            // 
            // contactName
            // 
            this.contactName.Location = new System.Drawing.Point(156, 46);
            this.contactName.Name = "contactName";
            this.contactName.Size = new System.Drawing.Size(448, 38);
            this.contactName.TabIndex = 0;
            // 
            // infoVersion
            // 
            this.infoVersion.Location = new System.Drawing.Point(193, 367);
            this.infoVersion.Name = "infoVersion";
            this.infoVersion.Size = new System.Drawing.Size(409, 38);
            this.infoVersion.TabIndex = 7;
            this.infoVersion.Text = "1.0";
            this.toolTip1.SetToolTip(this.infoVersion, "Proporciona la versión de la API de la aplicación (que no debe confundirse con la" +
        " versión de la especificación).");
            // 
            // infoTermsOfService
            // 
            this.infoTermsOfService.Location = new System.Drawing.Point(193, 316);
            this.infoTermsOfService.Name = "infoTermsOfService";
            this.infoTermsOfService.Size = new System.Drawing.Size(409, 38);
            this.infoTermsOfService.TabIndex = 6;
            this.infoTermsOfService.Text = "http://swagger.io/terms/";
            this.toolTip1.SetToolTip(this.infoTermsOfService, "Los términos de servicio de la API.");
            // 
            // infoDescription
            // 
            this.infoDescription.Location = new System.Drawing.Point(12, 157);
            this.infoDescription.Multiline = true;
            this.infoDescription.Name = "infoDescription";
            this.infoDescription.Size = new System.Drawing.Size(590, 147);
            this.infoDescription.TabIndex = 5;
            this.toolTip1.SetToolTip(this.infoDescription, "Una breve descripción de la aplicación. La sintaxis de GFM se puede utilizar para" +
        " la representación de texto enriquecido.");
            // 
            // infoTitulo
            // 
            this.infoTitulo.Location = new System.Drawing.Point(179, 76);
            this.infoTitulo.Name = "infoTitulo";
            this.infoTitulo.Size = new System.Drawing.Size(423, 38);
            this.infoTitulo.TabIndex = 4;
            this.toolTip1.SetToolTip(this.infoTitulo, "El título de la solicitud.");
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 367);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(112, 32);
            this.label7.TabIndex = 3;
            this.label7.Text = "Versión";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 316);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(133, 32);
            this.label6.TabIndex = 2;
            this.label6.Text = "Términos";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 122);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(164, 32);
            this.label5.TabIndex = 1;
            this.label5.Text = "Descripción";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 32);
            this.label4.TabIndex = 0;
            this.label4.Text = "Título";
            // 
            // Swagger
            // 
            this.Swagger.Controls.Add(this.general);
            this.Swagger.Controls.Add(this.tabPage2);
            this.Swagger.Controls.Add(this.tabPage1);
            this.Swagger.Controls.Add(this.tabPage3);
            this.Swagger.Location = new System.Drawing.Point(31, 42);
            this.Swagger.Name = "Swagger";
            this.Swagger.SelectedIndex = 0;
            this.Swagger.ShowToolTips = true;
            this.Swagger.Size = new System.Drawing.Size(2009, 972);
            this.Swagger.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.Swagger.TabIndex = 13;
            // 
            // general
            // 
            this.general.Controls.Add(this.label13);
            this.general.Controls.Add(this.externalDocsPanel);
            this.general.Controls.Add(this.label1);
            this.general.Controls.Add(this.groupBox2);
            this.general.Controls.Add(this.label3);
            this.general.Controls.Add(this.basePath);
            this.general.Controls.Add(this.label2);
            this.general.Controls.Add(this.http);
            this.general.Controls.Add(this.host);
            this.general.Controls.Add(this.swaggerVersion);
            this.general.Controls.Add(this.groupBox1);
            this.general.Location = new System.Drawing.Point(10, 48);
            this.general.Name = "general";
            this.general.Padding = new System.Windows.Forms.Padding(3);
            this.general.Size = new System.Drawing.Size(1989, 914);
            this.general.TabIndex = 0;
            this.general.Text = "Principal";
            this.general.UseVisualStyleBackColor = true;
            this.general.Click += new System.EventHandler(this.general_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(40, 450);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(290, 32);
            this.label13.TabIndex = 14;
            this.label13.Text = "Documentos externos";
            // 
            // externalDocsPanel
            // 
            this.externalDocsPanel.Location = new System.Drawing.Point(46, 528);
            this.externalDocsPanel.Name = "externalDocsPanel";
            this.externalDocsPanel.Size = new System.Drawing.Size(1917, 349);
            this.externalDocsPanel.TabIndex = 13;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(10, 48);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1989, 914);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Seguridad";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(10, 48);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1989, 914);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Tags";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // aceptar
            // 
            this.aceptar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.aceptar.Location = new System.Drawing.Point(1309, 1050);
            this.aceptar.Name = "aceptar";
            this.aceptar.Size = new System.Drawing.Size(338, 55);
            this.aceptar.TabIndex = 14;
            this.aceptar.Text = "Aceptar";
            this.aceptar.UseVisualStyleBackColor = true;
            this.aceptar.Click += new System.EventHandler(this.aceptar_Click);
            // 
            // cancelar
            // 
            this.cancelar.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.cancelar.Location = new System.Drawing.Point(1692, 1050);
            this.cancelar.Name = "cancelar";
            this.cancelar.Size = new System.Drawing.Size(338, 55);
            this.cancelar.TabIndex = 15;
            this.cancelar.Text = "Cancelar";
            this.cancelar.UseVisualStyleBackColor = true;
            this.cancelar.Click += new System.EventHandler(this.cancelar_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(10, 48);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1989, 914);
            this.tabPage3.TabIndex = 3;
            this.tabPage3.Text = "Recursos";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // FormSwagger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2071, 1117);
            this.Controls.Add(this.cancelar);
            this.Controls.Add(this.aceptar);
            this.Controls.Add(this.Swagger);
            this.Name = "FormSwagger";
            this.Text = "RestFul Alert";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.contacotGroupBox.ResumeLayout(false);
            this.contacotGroupBox.PerformLayout();
            this.Swagger.ResumeLayout(false);
            this.general.ResumeLayout(false);
            this.general.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox basePath;
        private System.Windows.Forms.CheckBox http;
        private System.Windows.Forms.TextBox host;
        private System.Windows.Forms.TextBox swaggerVersion;
        private System.Windows.Forms.CheckBox https;
        private System.Windows.Forms.CheckBox ws;
        private System.Windows.Forms.CheckBox wss;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox infoTermsOfService;
        private System.Windows.Forms.TextBox infoDescription;
        private System.Windows.Forms.TextBox infoTitulo;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox infoVersion;
        private System.Windows.Forms.TabControl Swagger;
        private System.Windows.Forms.TabPage general;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox licenceUrl;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox licenceName;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox contacotGroupBox;
        private System.Windows.Forms.TextBox contactUrl;
        private System.Windows.Forms.TextBox contactEmail;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox contactName;
        private System.Windows.Forms.Panel externalDocsPanel;
        private System.Windows.Forms.Button aceptar;
        private System.Windows.Forms.Button cancelar;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TabPage tabPage3;
    }
}