using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EA;
using MDGBuilder.mdg;
using UIResources;
using EAUtils;

namespace MDGBuilder.ui
{
    public partial class RenameMDG : UserControl
    {
        public static string NAME = "MDG Editor";

        private EAUtils.EAUtils eaUtils;
        private EA.Attribute useSemanticVersioningAttribute;
        private bool useSemanticVersioning = false;
        private Element mtsElement;
        private Package mdgPackage;
        private MDGRenamer renamer;
        private EAUtils.SemanticVersioning semanticVersioning;

        public RenameMDG()
        {
            InitializeComponent();

            System.Windows.Forms.ToolTip toolTip01 = new System.Windows.Forms.ToolTip();
            toolTip01.SetToolTip( this.button_incrementMajor, Properties.Resources.editor_configuracion_boton_incrementMajor );

            System.Windows.Forms.ToolTip toolTip02 = new System.Windows.Forms.ToolTip();
            toolTip02.SetToolTip( this.button_incrementMinor, Properties.Resources.editor_configuracion_boton_incrementMinor );

            System.Windows.Forms.ToolTip toolTip03 = new System.Windows.Forms.ToolTip();
            toolTip02.SetToolTip(this.button_buildMDG, Properties.Resources.editor_configuracion_boton_buildMDG);

            this.label_nombreMDG.Text = Properties.Resources.nombre_MDG;
            this.label_nuevoNombreMDG.Text = Properties.Resources.nuevo_nombre_MDG;
            this.label_version.Text = Properties.Resources.version_MDG;
            this.checkBox_usarVersionadoSemantico.Text = Properties.Resources.editor_configuracion_usar_versionado_semántico;
            this.label_buildInfo.Text = Properties.Resources.build_info;
        }

        public void show( Package mdgPackage, SemanticVersioning semanticVersioning, EAUtils.EAUtils eaUtils)
        {
            this.eaUtils                        = eaUtils;
            this.renamer                        = new MDGRenamer();
            this.semanticVersioning             = semanticVersioning;
            this.mdgPackage                     = mdgPackage;
            this.mtsElement                     = renamer.getMtsElement(mdgPackage);            
            this.useSemanticVersioningAttribute = EAUtils.AttributeUtils.get(mtsElement, "useSemanticVersioning");
            this.textBox_nombreMDG.Text         = this.mtsElement.Name;
            this.textBox_nuevoNombreMDG.Text    = this.mtsElement.Name;

            if(useSemanticVersioningAttribute != null )
            {
                useSemanticVersioning = useSemanticVersioningAttribute.Default == "true";
                parseVersion();
            }
            else
            {
                this.checkBox_usarVersionadoSemantico.Checked = false;
                this.useSemanticVersioningAttribute = mtsElement.Attributes.AddNew("useSemanticVersioning", "boolean");
                this.useSemanticVersioningAttribute.Default = "false";

                this.useSemanticVersioningAttribute.Update();
            }
        }

        private void parseVersion() {

            semanticVersioning.parseVersion( this.mtsElement.Version );

            setVersionUIValues();
        }

        private void setVersionUIValues()
        {
            this.label_majorMinorPatch.Text    = semanticVersioning.buildStringWithoutBuildPart();
            this.textBox_versionBuildInfo.Text = semanticVersioning.build;
            mtsElement.Version = this.textBox_versionBuildInfo.Text;
            mtsElement.Update();
        }

        private void toggleVersionado()
        {
            if ( ! useSemanticVersioning )
            {
                this.button_incrementMajor.Enabled = false;
                this.button_incrementMinor.Enabled = false;
            }
            else
            {
                parseVersion();

                this.button_incrementMajor.Enabled = true;
                this.button_incrementMinor.Enabled = true;
            }
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            // guardar los datos y renombrar el mdg.
            this.useSemanticVersioningAttribute.Default = useSemanticVersioning ? "true" : "false";
            this.useSemanticVersioningAttribute.Update();

            if (useSemanticVersioning)
            {
                semanticVersioning.build = this.textBox_versionBuildInfo.Text.Trim();

                this.mtsElement.Version = semanticVersioning.buildString();
                this.mtsElement.Update();
            }
            else if( this.textBox_versionBuildInfo.Text.Trim().Length != 0)
            {
                this.mtsElement.Version = this.mtsElement.Version + this.textBox_versionBuildInfo.Text.Trim();
                this.mtsElement.Update();
            }

            string newName = this.textBox_nuevoNombreMDG.Text.Trim();

            if (newName != "" && this.mtsElement.Name != newName)
            {
                renamer.oldMdgName = this.mtsElement.Name;
                renamer.oldmdgNameU = this.mtsElement.Alias.Replace(" ", "_");
                renamer.newMdgName = newName;
                renamer.newMdgNameU = newName.Replace(" ", "_");

                this.mtsElement.Name = newName;
                this.mtsElement.Alias = renamer.newMdgNameU;

                this.mtsElement.Update();

                renamer.doIt(this.mdgPackage);

                MtsUtil mtsUtil = new MtsUtil();

                mtsUtil.eaUtils = this.eaUtils;

                mtsUtil.rename( this.mdgPackage, renamer.mtsElement, renamer.oldMdgName, renamer.oldmdgNameU, renamer.newMdgName, renamer.newMdgNameU);

            }
            Alert.Success(Properties.Resources.MESSAGE_CONIFGURACION_SAVED);
        }

        private void checkBox_usarVersionadoSemantico_CheckedChanged(object sender, EventArgs e)
        {
            useSemanticVersioning = checkBox_usarVersionadoSemantico.Checked;
            toggleVersionado();
        }

        private void button_save_MouseHover(object sender, EventArgs e)
        {
            button_save.Image = MDGBuilder.Properties.Resources.icons8_Save_64;
        }

        private void button_save_MouseLeave(object sender, EventArgs e)
        {
            button_save.Image = MDGBuilder.Properties.Resources.icons8_Save_32;
        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://semver.org/");
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button_refresh_Click(object sender, EventArgs e)
        {
            this.mtsElement                     = renamer.getMtsElement(mdgPackage);
            this.useSemanticVersioningAttribute = EAUtils.AttributeUtils.get(mtsElement, "useSemanticVersioning");

            parseVersion();

            Alert.Success(Properties.Resources.MESSAGE_CONIFGURACION_RELOAD);
        }

        private void button_incrementMajor_MouseHover(object sender, EventArgs e)
        {
            this.button_incrementMajor.Image = Properties.Resources.icons8_Plus_Math_64;
        }

        private void button_incrementMajor_MouseLeave(object sender, EventArgs e)
        {
            this.button_incrementMajor.Image = Properties.Resources.icons8_plus_math_32;
        }

        private void button_incrementMinor_MouseHover(object sender, EventArgs e)
        {
            this.button_incrementMinor.Image = Properties.Resources.icons8_Plus_Math_64;
        }

        private void button_incrementMinor_MouseLeave(object sender, EventArgs e)
        {
            this.button_incrementMinor.Image = Properties.Resources.icons8_plus_math_32;
        }

        private void button_refresh_MouseHover(object sender, EventArgs e)
        {
            this.button_refresh.Image = Properties.Resources.icons8_refresh_64;
        }

        private void button_refresh_MouseLeave(object sender, EventArgs e)
        {
            this.button_refresh.Image = Properties.Resources.icons8_Refresh_32;
        }

        private void button_incrementMajor_Click_1(object sender, EventArgs e)
        {
            semanticVersioning.incrementMajor();
            setVersionUIValues();
        }

        private void button_incrementMinor_Click_1(object sender, EventArgs e)
        {
            semanticVersioning.incrementMinor();
            setVersionUIValues();
        }

        private void button_buildMDG_Click(object sender, EventArgs e)
        {
            Builder builder = new Builder(mdgPackage, semanticVersioning, this.eaUtils);

            try
            {
                builder.build();
            }
            catch (Exception ex)
            {
                Clipboard.SetText(ex.ToString());
                Alert.Error("Error en el portapapeles");
            }
        }

        private void button_buildMDG_MouseHover(object sender, EventArgs e)
        {
            this.button_buildMDG.Image = Properties.Resources.icons8_construction_64;
        }

        private void button_buildMDG_MouseLeave(object sender, EventArgs e)
        {
            this.button_buildMDG.Image = Properties.Resources.icons8_construction_32;
        }
    }
}
