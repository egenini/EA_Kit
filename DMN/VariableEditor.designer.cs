namespace DMN
{
    partial class VariableEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VariableEditor));
            this.tableLayoutPanel_level_0 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel_type = new System.Windows.Forms.TableLayoutPanel();
            this.dataType = new System.Windows.Forms.ComboBox();
            this.label_type = new System.Windows.Forms.Label();
            this.tableLayoutPanel_businessName = new System.Windows.Forms.TableLayoutPanel();
            this.businessName = new System.Windows.Forms.TextBox();
            this.label_businessName = new System.Windows.Forms.Label();
            this.tableLayoutPanel_attribute = new System.Windows.Forms.TableLayoutPanel();
            this.attributeName = new System.Windows.Forms.TextBox();
            this.label_attributeName = new System.Windows.Forms.Label();
            this.tableLayoutPanel_toolbar = new System.Windows.Forms.TableLayoutPanel();
            this.agregar = new System.Windows.Forms.Button();
            this.borrar = new System.Windows.Forms.Button();
            this.actualizar = new System.Windows.Forms.Button();
            this.generarEnumeracion = new System.Windows.Forms.Button();
            this.controlButtonGenerarCodigo = new System.Windows.Forms.Button();
            this.controlButtonGenerarXml = new System.Windows.Forms.Button();
            this.controlButtonGenerarJson = new System.Windows.Forms.Button();
            this.tableLayoutPanel_format = new System.Windows.Forms.TableLayoutPanel();
            this.label_format = new System.Windows.Forms.Label();
            this.controlTextBoxFormat = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel_default = new System.Windows.Forms.TableLayoutPanel();
            this.controlTextBoxDefault = new System.Windows.Forms.TextBox();
            this.label_default = new System.Windows.Forms.Label();
            this.tableLayoutPanel_level_0.SuspendLayout();
            this.tableLayoutPanel_type.SuspendLayout();
            this.tableLayoutPanel_businessName.SuspendLayout();
            this.tableLayoutPanel_attribute.SuspendLayout();
            this.tableLayoutPanel_toolbar.SuspendLayout();
            this.tableLayoutPanel_format.SuspendLayout();
            this.tableLayoutPanel_default.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel_level_0
            // 
            resources.ApplyResources(this.tableLayoutPanel_level_0, "tableLayoutPanel_level_0");
            this.tableLayoutPanel_level_0.Controls.Add(this.tableLayoutPanel_type, 0, 3);
            this.tableLayoutPanel_level_0.Controls.Add(this.tableLayoutPanel_businessName, 0, 2);
            this.tableLayoutPanel_level_0.Controls.Add(this.tableLayoutPanel_attribute, 0, 1);
            this.tableLayoutPanel_level_0.Controls.Add(this.tableLayoutPanel_toolbar, 0, 0);
            this.tableLayoutPanel_level_0.Controls.Add(this.tableLayoutPanel_format, 0, 4);
            this.tableLayoutPanel_level_0.Controls.Add(this.tableLayoutPanel_default, 0, 5);
            this.tableLayoutPanel_level_0.Name = "tableLayoutPanel_level_0";
            this.tableLayoutPanel_level_0.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // tableLayoutPanel_type
            // 
            resources.ApplyResources(this.tableLayoutPanel_type, "tableLayoutPanel_type");
            this.tableLayoutPanel_type.Controls.Add(this.dataType, 0, 1);
            this.tableLayoutPanel_type.Controls.Add(this.label_type, 0, 0);
            this.tableLayoutPanel_type.Name = "tableLayoutPanel_type";
            // 
            // dataType
            // 
            resources.ApplyResources(this.dataType, "dataType");
            this.dataType.FormattingEnabled = true;
            this.dataType.Name = "dataType";
            this.dataType.SelectedIndexChanged += new System.EventHandler(this.dataType_SelectedIndexChanged);
            this.dataType.SelectedValueChanged += new System.EventHandler(this.dataType_SelectedValueChanged);
            // 
            // label_type
            // 
            resources.ApplyResources(this.label_type, "label_type");
            this.label_type.Name = "label_type";
            this.label_type.Click += new System.EventHandler(this.label3_Click);
            // 
            // tableLayoutPanel_businessName
            // 
            resources.ApplyResources(this.tableLayoutPanel_businessName, "tableLayoutPanel_businessName");
            this.tableLayoutPanel_businessName.Controls.Add(this.businessName, 0, 1);
            this.tableLayoutPanel_businessName.Controls.Add(this.label_businessName, 0, 0);
            this.tableLayoutPanel_businessName.Name = "tableLayoutPanel_businessName";
            // 
            // businessName
            // 
            resources.ApplyResources(this.businessName, "businessName");
            this.businessName.Name = "businessName";
            this.businessName.TextChanged += new System.EventHandler(this.businessName_TextChanged);
            // 
            // label_businessName
            // 
            resources.ApplyResources(this.label_businessName, "label_businessName");
            this.label_businessName.Name = "label_businessName";
            // 
            // tableLayoutPanel_attribute
            // 
            resources.ApplyResources(this.tableLayoutPanel_attribute, "tableLayoutPanel_attribute");
            this.tableLayoutPanel_attribute.Controls.Add(this.attributeName, 0, 1);
            this.tableLayoutPanel_attribute.Controls.Add(this.label_attributeName, 0, 0);
            this.tableLayoutPanel_attribute.Name = "tableLayoutPanel_attribute";
            // 
            // attributeName
            // 
            resources.ApplyResources(this.attributeName, "attributeName");
            this.attributeName.Name = "attributeName";
            this.attributeName.TextChanged += new System.EventHandler(this.attributeName_TextChanged);
            // 
            // label_attributeName
            // 
            resources.ApplyResources(this.label_attributeName, "label_attributeName");
            this.label_attributeName.Name = "label_attributeName";
            // 
            // tableLayoutPanel_toolbar
            // 
            resources.ApplyResources(this.tableLayoutPanel_toolbar, "tableLayoutPanel_toolbar");
            this.tableLayoutPanel_toolbar.Controls.Add(this.agregar, 0, 0);
            this.tableLayoutPanel_toolbar.Controls.Add(this.borrar, 1, 0);
            this.tableLayoutPanel_toolbar.Controls.Add(this.actualizar, 2, 0);
            this.tableLayoutPanel_toolbar.Controls.Add(this.generarEnumeracion, 3, 0);
            this.tableLayoutPanel_toolbar.Controls.Add(this.controlButtonGenerarCodigo, 4, 0);
            this.tableLayoutPanel_toolbar.Controls.Add(this.controlButtonGenerarXml, 5, 0);
            this.tableLayoutPanel_toolbar.Controls.Add(this.controlButtonGenerarJson, 6, 0);
            this.tableLayoutPanel_toolbar.Name = "tableLayoutPanel_toolbar";
            // 
            // agregar
            // 
            resources.ApplyResources(this.agregar, "agregar");
            this.agregar.ForeColor = System.Drawing.Color.Transparent;
            this.agregar.Image = global::DMN.Properties.Resources.icons8_Plus_32;
            this.agregar.Name = "agregar";
            this.agregar.UseVisualStyleBackColor = true;
            this.agregar.Click += new System.EventHandler(this.agregar_Click);
            this.agregar.MouseLeave += new System.EventHandler(this.agregar_MouseLeave);
            this.agregar.MouseHover += new System.EventHandler(this.agregar_MouseHover);
            // 
            // borrar
            // 
            resources.ApplyResources(this.borrar, "borrar");
            this.borrar.ForeColor = System.Drawing.Color.Transparent;
            this.borrar.Image = global::DMN.Properties.Resources.icons8_Trash_32;
            this.borrar.Name = "borrar";
            this.borrar.UseVisualStyleBackColor = true;
            this.borrar.Click += new System.EventHandler(this.borrar_Click);
            this.borrar.MouseLeave += new System.EventHandler(this.borrar_MouseLeave);
            this.borrar.MouseHover += new System.EventHandler(this.borrar_MouseHover);
            // 
            // actualizar
            // 
            resources.ApplyResources(this.actualizar, "actualizar");
            this.actualizar.ForeColor = System.Drawing.Color.Transparent;
            this.actualizar.Image = global::DMN.Properties.Resources.icons8_Save_32;
            this.actualizar.Name = "actualizar";
            this.actualizar.UseVisualStyleBackColor = true;
            this.actualizar.Click += new System.EventHandler(this.actualizar_Click);
            this.actualizar.MouseLeave += new System.EventHandler(this.actualizar_MouseLeave);
            this.actualizar.MouseHover += new System.EventHandler(this.actualizar_MouseHover);
            // 
            // generarEnumeracion
            // 
            resources.ApplyResources(this.generarEnumeracion, "generarEnumeracion");
            this.generarEnumeracion.ForeColor = System.Drawing.Color.Transparent;
            this.generarEnumeracion.Image = global::DMN.Properties.Resources.icons8_Add_Property_32;
            this.generarEnumeracion.Name = "generarEnumeracion";
            this.generarEnumeracion.UseVisualStyleBackColor = true;
            this.generarEnumeracion.Click += new System.EventHandler(this.generarEnumeracion_Click);
            this.generarEnumeracion.MouseLeave += new System.EventHandler(this.generarEnumeracion_MouseLeave);
            this.generarEnumeracion.MouseHover += new System.EventHandler(this.generarEnumeracion_MouseHover);
            // 
            // controlButtonGenerarCodigo
            // 
            resources.ApplyResources(this.controlButtonGenerarCodigo, "controlButtonGenerarCodigo");
            this.controlButtonGenerarCodigo.ForeColor = System.Drawing.Color.Transparent;
            this.controlButtonGenerarCodigo.Image = global::DMN.Properties.Resources.icons8_Source_Code_32;
            this.controlButtonGenerarCodigo.Name = "controlButtonGenerarCodigo";
            this.controlButtonGenerarCodigo.UseVisualStyleBackColor = true;
            this.controlButtonGenerarCodigo.Click += new System.EventHandler(this.controlButtonGenerarCodigo_Click);
            this.controlButtonGenerarCodigo.MouseLeave += new System.EventHandler(this.controlButtonGenerarCodigo_MouseLeave);
            this.controlButtonGenerarCodigo.MouseHover += new System.EventHandler(this.controlButtonGenerarCodigo_MouseHover);
            // 
            // controlButtonGenerarXml
            // 
            resources.ApplyResources(this.controlButtonGenerarXml, "controlButtonGenerarXml");
            this.controlButtonGenerarXml.ForeColor = System.Drawing.Color.Transparent;
            this.controlButtonGenerarXml.Image = global::DMN.Properties.Resources.icons8_XML_32;
            this.controlButtonGenerarXml.Name = "controlButtonGenerarXml";
            this.controlButtonGenerarXml.UseVisualStyleBackColor = true;
            this.controlButtonGenerarXml.Click += new System.EventHandler(this.controlButtonGenerarXml_Click);
            this.controlButtonGenerarXml.MouseLeave += new System.EventHandler(this.controlButtonGenerarXml_MouseLeave);
            this.controlButtonGenerarXml.MouseHover += new System.EventHandler(this.controlButtonGenerarXml_MouseHover);
            // 
            // controlButtonGenerarJson
            // 
            resources.ApplyResources(this.controlButtonGenerarJson, "controlButtonGenerarJson");
            this.controlButtonGenerarJson.ForeColor = System.Drawing.Color.Transparent;
            this.controlButtonGenerarJson.Image = global::DMN.Properties.Resources.icons8_JSON_32;
            this.controlButtonGenerarJson.Name = "controlButtonGenerarJson";
            this.controlButtonGenerarJson.UseVisualStyleBackColor = true;
            this.controlButtonGenerarJson.Click += new System.EventHandler(this.controlButtonGenerarJson_Click);
            this.controlButtonGenerarJson.MouseLeave += new System.EventHandler(this.controlButtonGenerarJson_MouseLeave);
            this.controlButtonGenerarJson.MouseHover += new System.EventHandler(this.controlButtonGenerarJson_MouseHover);
            // 
            // tableLayoutPanel_format
            // 
            resources.ApplyResources(this.tableLayoutPanel_format, "tableLayoutPanel_format");
            this.tableLayoutPanel_format.Controls.Add(this.label_format, 0, 0);
            this.tableLayoutPanel_format.Controls.Add(this.controlTextBoxFormat, 0, 1);
            this.tableLayoutPanel_format.Name = "tableLayoutPanel_format";
            // 
            // label_format
            // 
            resources.ApplyResources(this.label_format, "label_format");
            this.label_format.Name = "label_format";
            // 
            // controlTextBoxFormat
            // 
            resources.ApplyResources(this.controlTextBoxFormat, "controlTextBoxFormat");
            this.controlTextBoxFormat.Name = "controlTextBoxFormat";
            this.controlTextBoxFormat.TextChanged += new System.EventHandler(this.controlTextBoxFormat_TextChanged);
            // 
            // tableLayoutPanel_default
            // 
            resources.ApplyResources(this.tableLayoutPanel_default, "tableLayoutPanel_default");
            this.tableLayoutPanel_default.Controls.Add(this.controlTextBoxDefault, 0, 1);
            this.tableLayoutPanel_default.Controls.Add(this.label_default, 0, 0);
            this.tableLayoutPanel_default.Name = "tableLayoutPanel_default";
            // 
            // controlTextBoxDefault
            // 
            resources.ApplyResources(this.controlTextBoxDefault, "controlTextBoxDefault");
            this.controlTextBoxDefault.Name = "controlTextBoxDefault";
            // 
            // label_default
            // 
            resources.ApplyResources(this.label_default, "label_default");
            this.label_default.Name = "label_default";
            this.label_default.Click += new System.EventHandler(this.controlLabelDefault_Click);
            // 
            // VariableEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tableLayoutPanel_level_0);
            this.Name = "VariableEditor";
            this.Load += new System.EventHandler(this.VariableEditor_Load);
            this.tableLayoutPanel_level_0.ResumeLayout(false);
            this.tableLayoutPanel_level_0.PerformLayout();
            this.tableLayoutPanel_type.ResumeLayout(false);
            this.tableLayoutPanel_type.PerformLayout();
            this.tableLayoutPanel_businessName.ResumeLayout(false);
            this.tableLayoutPanel_businessName.PerformLayout();
            this.tableLayoutPanel_attribute.ResumeLayout(false);
            this.tableLayoutPanel_attribute.PerformLayout();
            this.tableLayoutPanel_toolbar.ResumeLayout(false);
            this.tableLayoutPanel_format.ResumeLayout(false);
            this.tableLayoutPanel_format.PerformLayout();
            this.tableLayoutPanel_default.ResumeLayout(false);
            this.tableLayoutPanel_default.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_level_0;
        private System.Windows.Forms.TextBox attributeName;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_attribute;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_businessName;
        private System.Windows.Forms.TextBox businessName;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_type;
        private System.Windows.Forms.ComboBox dataType;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_toolbar;
        private System.Windows.Forms.Button agregar;
        private System.Windows.Forms.Button borrar;
        private System.Windows.Forms.Button actualizar;
        private System.Windows.Forms.Button generarEnumeracion;
        private System.Windows.Forms.Button controlButtonGenerarCodigo;
        private System.Windows.Forms.Button controlButtonGenerarXml;
        private System.Windows.Forms.Button controlButtonGenerarJson;
        private System.Windows.Forms.Label label_type;
        private System.Windows.Forms.Label label_businessName;
        private System.Windows.Forms.Label label_attributeName;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_format;
        private System.Windows.Forms.Label label_format;
        private System.Windows.Forms.TextBox controlTextBoxFormat;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_default;
        private System.Windows.Forms.TextBox controlTextBoxDefault;
        private System.Windows.Forms.Label label_default;
    }
}
