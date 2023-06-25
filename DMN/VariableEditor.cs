using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EA;
using UIResources;
using DMN.framework.domain;

namespace DMN
{
    public partial class VariableEditor : UserControl
    {
        //private const string OPTION_SEARCH = "<Select in ...>";

        public static string NAME = "DMN Editor";
        private Element currentVariable;
        private Element currentBusinessKnowledge;
        private EAUtils.EAUtils eaUtils;
        public Main main;
        public EA.TaggedValue currentVariableFormatTaggedValue;
        public EA.TaggedValue currentVariableDefaultTaggedValue;

        private bool building = false;

        private Dictionary<string, int> dataTypesInCombo = new Dictionary<string, int>();

        List<KeyValuePair<int, Object>> options = new List<KeyValuePair<int, Object>>();


        KeyValuePair<int, Object> optionSelected  = new KeyValuePair<int, object>(-1000, "");


        public VariableEditor()
        {
            InitializeComponent();

            this.label_attributeName.Text = Properties.Resources.variable_editor_label_attributeName;
            this.label_businessName.Text = Properties.Resources.variable_editor_label_businessName;
            this.label_default.Text = Properties.Resources.variable_editor_label_default;
            this.label_format.Text = Properties.Resources.variable_editor_label_format;
            this.label_type.Text = Properties.Resources.variable_editor_label_type;

            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip( );

            //ToolTip1.ToolTipIcon = DMN.Properties.Resources.icons8_Trash_64;
            ToolTip1.SetToolTip(this.borrar, Properties.Resources.variable_editor_tooltip_eliminar);

            System.Windows.Forms.ToolTip ToolTip2 = new System.Windows.Forms.ToolTip();
            ToolTip2.SetToolTip(this.agregar, Properties.Resources.variable_editor_tooltip_agregar);

            System.Windows.Forms.ToolTip ToolTip3 = new System.Windows.Forms.ToolTip();
            ToolTip3.SetToolTip(this.generarEnumeracion, Properties.Resources.variable_editor_tooltip_crear_enumeracion);

            System.Windows.Forms.ToolTip ToolTip4 = new System.Windows.Forms.ToolTip();
            ToolTip4.SetToolTip(this.actualizar, Properties.Resources.variable_editor_tooltip_guardar);

            System.Windows.Forms.ToolTip ToolTip5 = new System.Windows.Forms.ToolTip();
            ToolTip5.SetToolTip(this.controlButtonGenerarCodigo, Properties.Resources.variable_editor_tooltip_generar_codigo);

            System.Windows.Forms.ToolTip ToolTip6 = new System.Windows.Forms.ToolTip();
            ToolTip6.SetToolTip(this.controlButtonGenerarXml, Properties.Resources.variable_editor_tooltip_generar_dmn);

            System.Windows.Forms.ToolTip ToolTip7 = new System.Windows.Forms.ToolTip();
            ToolTip7.SetToolTip(this.controlButtonGenerarJson, Properties.Resources.variable_editor_tooltip_generar_json);
        }

        internal void show(Element e, EAUtils.EAUtils eaUtils, Main main)
        {
            this.eaUtils = eaUtils;
            this.main = main;
            this.currentVariable = null;
            this.currentBusinessKnowledge = null;
            this.currentVariableFormatTaggedValue = null;

            if (e.Type == "ActivityParameter")
            {
                currentVariable = e;

                this.attributeName.Text = e.Alias;
                this.businessName.Text = e.Name;

                this.currentVariableFormatTaggedValue  = this.eaUtils.taggedValuesUtils.getTaggedValue(currentVariable, "Format");
                this.currentVariableDefaultTaggedValue = this.eaUtils.taggedValuesUtils.getTaggedValue(currentVariable, "Default");
                //this.currentVariableFormatTaggedValue = this.eaUtils.taggedValuesUtils.otrometododelorto(currentVariable, "Format");
                //this.currentVariableDefaultTaggedValue = this.eaUtils.taggedValuesUtils.otrometododelorto(currentVariable, "Default");

                this.controlTextBoxFormat.Text = this.currentVariableFormatTaggedValue.Value;
                this.controlTextBoxDefault.Text = this.currentVariableDefaultTaggedValue.Value;

                this.actualizar.Enabled = true;
                this.actualizar.Show();
                this.agregar.Enabled = true;
                this.agregar.Show();
                this.borrar.Enabled = true;
                this.borrar.Show();
                this.generarEnumeracion.Enabled = true;
                this.generarEnumeracion.Show();

                // TODO : pensar mejor esto, no me cierra.
                if (this.main.framework.isEnableToGenerateCode())
                {
                    this.controlButtonGenerarCodigo.Show();
                    this.controlButtonGenerarJson.Show();
                }
                this.controlButtonGenerarXml.Show();

                this.dataType.Enabled = true;
                this.businessName.Enabled = true;
                this.attributeName.Enabled = true;
                this.controlTextBoxDefault.Enabled = true;
                this.controlTextBoxFormat.Enabled = true;

                this.building = true;
                this.makeOptions();
                this.building = false;
            }
            else if( e.Stereotype == "BusinessKnowledge")
            {
                this.disable();
                this.currentBusinessKnowledge = e;
                this.agregar.Show();
                this.agregar.Enabled = true;
                //if(this.main.framework.isEnableToGenerateCode())
                //{
                    this.controlButtonGenerarCodigo.Show();
                    this.controlButtonGenerarJson.Show();
                //}
                this.controlButtonGenerarXml.Show();
            }
            else
            {
                this.disable();
            }
        }

        public void disable()
        {
            this.actualizar.Enabled = false;
            this.actualizar.Hide();
            this.agregar.Enabled = false;
            this.agregar.Hide();
            this.borrar.Enabled = false;
            this.borrar.Hide();
            this.generarEnumeracion.Enabled = false;
            this.generarEnumeracion.Hide();
            this.controlButtonGenerarCodigo.Hide();
            this.controlButtonGenerarJson.Hide();
            this.controlButtonGenerarXml.Hide();

            this.dataType.Enabled = false;
            this.businessName.Enabled = false;
            this.attributeName.Enabled = false;
            this.controlTextBoxDefault.Enabled = false;
            this.controlTextBoxFormat.Enabled = false;

            /*
            this.dataType.Text = "";
            this.businessName.Text = "";
            this.attributeName.Text = "";
            this.controlTextBoxDefault.Text = "";
            this.controlTextBoxFormat.Text = "";
            */

        }
        private void makeOptions()
        {
            this.dataType.DataSource = null;

            this.dataType.Items.Clear();
            options.Clear();

            Option option = Option.EmptyOption();

            options.Add(new KeyValuePair<int, Object>(option.id, option));

            // cargar los tipos de datos del lenguaje
            bool isDataTypeLanguague = this.makeOptionsLanguage();

            if( ! isDataTypeLanguague)
            {
                makeOptionFromInstanceType();

            }
            option = Option.ChooseOption();
            options.Add(new KeyValuePair<int, Object>(option.id, option));

            this.dataType.DataSource = new BindingSource(options, null);
            this.dataType.DisplayMember = "Value";
            this.dataType.ValueMember = "Key";

            this.dataType.SelectedIndex = 0;
            
            if( optionSelected.Key != -1000 )
            {
                this.dataType.SelectedItem = optionSelected;
            }
        }

        private void makeOptionFromInstanceType()
        {
            if (currentVariable.ClassifierID != 0)
            {
                // - Instancia de una Enumeración
                // - Instancia de un Attribute.
                if ( ! makeOptionFromInstanceTypeElement())
                {
                    // - Instancia de un atributo de una clase
                    if( ! makeOptionFromInstanceTypeAttribute())
                    {
                        // - Instancia de un método de una clase
                        makeOptionFromInstanceTypeMethod();
                    }
                }
            }
        }

        private bool makeOptionFromInstanceTypeAttribute()
        {
            bool canMake = false;
            Option option;

            try
            {
                string attrGuid = this.eaUtils.taggedValuesUtils.get(this.currentVariable, "instanceOf", "").asString();

                if( attrGuid != "")
                {
                    EA.Attribute dataTypeClassifier = this.eaUtils.repository.GetAttributeByGuid(attrGuid);
                    Element dataTypeClassifierParent = this.eaUtils.repository.GetElementByID(dataTypeClassifier.ParentID);

                    option = Option.AttributeOption(dataTypeClassifier, dataTypeClassifierParent);

                    optionSelected = new KeyValuePair<int, Object>(option.id, option);

                    options.Add(optionSelected);

                    canMake = true;
                }
            }
            catch (Exception) { }

            return canMake;
        }

        private void makeOptionFromInstanceTypeMethod()
        {
            Option option;

            try
            {
                string methodGuid = this.eaUtils.taggedValuesUtils.get(this.currentVariable, "instanceOf", "").asString();

                EA.Method dataTypeClassifier = this.eaUtils.repository.GetMethodByGuid(methodGuid);
                Element dataTypeClassifierParent = this.eaUtils.repository.GetElementByID(dataTypeClassifier.ParentID);

                option = Option.MethodOption(dataTypeClassifier, dataTypeClassifierParent);

                optionSelected = new KeyValuePair<int, Object>(option.id, option);

                options.Add(optionSelected);
            }
            catch (Exception) { }
        }
        /// <summary>
        /// Instancia de DIT Attribute o de Enumeration
        /// </summary>
        /// <returns></returns>
        private bool makeOptionFromInstanceTypeElement()
        {
            bool canMake = false;
            Option option;

            try
            {
                Element dataTypeClassifier = this.eaUtils.repository.GetElementByID(currentVariable.ClassifierID);

                if(dataTypeClassifier.Type == "Enumeration")
                {
                    if( dataTypeClassifier.Attributes.Count != 0)
                    {
                        option = Option.EnumerationOption(dataTypeClassifier, dataTypeClassifier.Attributes.GetAt(0).Type);

                        optionSelected = new KeyValuePair<int, Object>(option.id, option);

                        options.Add(optionSelected);

                        canMake = true;
                    }
                }
                else if (dataTypeClassifier.Stereotype == "Data")
                {
                    if (dataTypeClassifier.ParentID != 0)
                    {
                        Element dataTypeClassifierParent = this.eaUtils.repository.GetElementByID(dataTypeClassifier.ParentID);

                        string dataType = "";

                        if (eaUtils.repositoryConfiguration.getLanguage() == "en")
                        {
                            dataType = eaUtils.taggedValuesUtils.get(dataTypeClassifier, EAUtils.TaggedValuesUtils.EN__TIPO_DATO, "").asString();
                        }
                        else
                        {
                            dataType = eaUtils.taggedValuesUtils.get(dataTypeClassifier, EAUtils.TaggedValuesUtils.ES__TIPO_DATO, "").asString();
                        }

                        //string dataType = this.eaUtils.taggedValuesUtils.get(dataTypeClassifier, "Tipo de dato", "").asString();

                        if( dataType.Length == 0 )
                        {
                            Alert.Warning( String.Format( Properties.Resources.error_tipo__dato_no_definido ,dataTypeClassifier.Name ));
                        }

                        option = Option.DitAttributeOption(dataTypeClassifier, dataType, dataTypeClassifierParent);

                        optionSelected = new KeyValuePair<int, Object>(option.id, option);

                        options.Add(optionSelected);

                        canMake = true;
                    }
                    else
                    {
                        Alert.Error( String.Format( Properties.Resources.error_no_pertence_a_dataContainer ,dataTypeClassifier.Name ));
                    }
                }
            }
            catch (Exception) { }

            return canMake;
        }
        private bool makeOptionsLanguage()
        {
            bool isDataTypeLanguage = false;
            Option option;

            if (this.main.framework.currentLanguage != null)
            {

                foreach (DataType dataType in this.main.framework.currentLanguage.dataTypes)
                {
                    option = Option.LanguageOption(dataType.element, this.main.framework.currentLanguage.element);

                    options.Add(new KeyValuePair<int, Object>(option.id, option));

                    if (this.currentVariable.ClassifierID == dataType.elementId)
                    {
                        optionSelected = new KeyValuePair<int, Object>(option.id, option);

                        options.Add(optionSelected);

                        isDataTypeLanguage = true;
                    }
                }
            }
            return isDataTypeLanguage;
        }

        private void elementChanged()
        {
            if( currentVariable != null)
            {
                currentVariable.Update();
                if( this.currentVariableDefaultTaggedValue.Value != this.controlTextBoxDefault.Text)
                {
                    this.currentVariableDefaultTaggedValue.Value = this.controlTextBoxDefault.Text;
                    this.currentVariableDefaultTaggedValue.Update();
                }
                if (this.currentVariableFormatTaggedValue.Value != this.controlTextBoxFormat.Text)
                {
                    this.currentVariableFormatTaggedValue.Value = this.controlTextBoxFormat.Text;
                    this.currentVariableFormatTaggedValue.Update();
                }
                if (currentVariable.ParentID != 0)
                {
                    Element parent = this.eaUtils.repository.GetElementByID(currentVariable.ParentID);
                    parent.Elements.Refresh();
                }
            }
        }

        private void attributeName_TextChanged(object sender, EventArgs e)
        {
                currentVariable.Alias = this.attributeName.Text;
        }

        private void businessName_TextChanged(object sender, EventArgs e)
        {
            this.currentVariable.Name = this.businessName.Text;
        }

        private void borrar_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("¿Confirma eliminar " + this.currentVariable.Name +" (" + this.currentVariable.Alias +")", "Eliminar atributo", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                if (currentVariable.ParentID != 0)
                {
                    Element parent = this.eaUtils.repository.GetElementByID(currentVariable.ParentID);
                    Element element;
                    for( short i = 0; i < parent.Elements.Count; i++)
                    {
                        element = parent.Elements.GetAt(i);
                        if( element.ElementID == currentVariable.ElementID )
                        {
                            parent.Elements.DeleteAt(i, true);
                            break;
                        }
                    }
                }
            }
        }

        private void agregar_Click(object sender, EventArgs e)
        {
            //Element parent = this.main.getBusinessKnowledgeSelected(this.eaUtils.repository, MainUtils.MENU_LOCATION__TREEVIEW);

            if ( this.currentBusinessKnowledge != null )
            {
                this.main.addVar(this.currentBusinessKnowledge);
            }
            else
            {
                if(this.currentVariable != null && this.currentVariable.ParentID != 0 )
                {
                    this.main.addVar(this.eaUtils.repository.GetElementByID(this.currentVariable.ParentID));
                }
            }
        }

        private void actualizar_Click(object sender, EventArgs e)
        {
            elementChanged();
        }

        private void generarEnumeracion_Click(object sender, EventArgs e)
        {
            this.main.crearEnum(this.currentVariable);
        }

        private void onSelect()
        {
            try
            {
                KeyValuePair<int, object> selectedPair = (KeyValuePair<int, object>)this.dataType.SelectedItem;

                Option option = (Option)selectedPair.Value;

                if (option.id == Option.emptyOptionId)
                {
                    this.currentVariable.ClassifierID = 0;
                    this.currentVariable.ClassifierName = "";
                    this.currentVariable.Update();
                }
                else if (option.id == Option.chooseOptionId)
                {

                    int eaId = this.eaUtils.repository.InvokeConstructPicker("IncludedTypes=Enumeration;MultiSelect=false;");

                    this.currentVariable.ClassifierID = eaId;

                    try
                    {
                        makeOptionFromInstanceType();

                        this.currentVariable.ClassifierName = ((Option)optionSelected.Value).getSimpleName();

                        this.currentVariable.Update();

                        if (optionSelected.Key != -1000)
                        {
                            this.dataType.SelectedItem = optionSelected;
                        }

                    }
                    catch (Exception) { }
                }
                else
                {
                    this.currentVariable.ClassifierID = option.id;
                    this.currentVariable.ClassifierName = option.getSimpleName();
                    this.currentVariable.Update();
                }
            }
            catch (Exception) { }
        }
        private void dataType_SelectedValueChanged(object sender, EventArgs e)
        {
            if( ! this.building )
            {
                this.onSelect();
            }
        }

        private void controlButtonGenerarCodigo_Click(object sender, EventArgs e)
        {
            
            chooseLanguageForm form = new chooseLanguageForm();
            form.setCaller( this, true );
            form.ShowDialog();
            if( form.languageSelected != null)
            {
                Language language = this.main.framework.getByName(form.languageSelected);

                Package LanguageTargetPackage = this.eaUtils.repository.GetPackageByID(language.element.PackageID);

                Element selected = null;
                if (this.currentVariable != null && this.currentVariable.ParentID != 0)
                {
                    selected = this.eaUtils.repository.GetElementByID(this.currentVariable.ParentID);
                }
                this.main.generarCodigo(LanguageTargetPackage, selected);

            }
            /*
            int eaId = this.eaUtils.repository.InvokeConstructPicker("IncludedTypes=Class;MultiSelect=false;StereoType=Language");

            //Alert.Warning("Esto es un muestra de cómo debería ser, el InvokeConstructPicker no funciona para paquetes");
            if( eaId != 0)
            {
                Element languageElement = this.eaUtils.repository.GetElementByID(eaId);
                Package LanguageTargetPackage = this.eaUtils.repository.GetPackageByID(languageElement.PackageID);

                Element selected = null;
                if ( this.current != null && this.current.ParentID != 0)
                {
                    selected = this.eaUtils.repository.GetElementByID(this.current.ParentID);
                }
                
                this.main.generarCodigo(LanguageTargetPackage, selected);
            }
            else
            {
                Alert.Alert("Debe seleccionar el lenguaje para generar código");
            }
            */
        }

        private void controlButtonGenerarXml_Click(object sender, EventArgs e)
        {
            Alert.Error(Properties.Resources.no_implementada);
        }

        private void controlButtonGenerarJson_Click(object sender, EventArgs e)
        {
            chooseLanguageForm form = new chooseLanguageForm();
            form.setCaller(this, true);
            form.ShowDialog();
            if (form.languageSelected != null)
            {
                Language language = this.main.framework.getByName(form.languageSelected);

                Package LanguageTargetPackage = this.eaUtils.repository.GetPackageByID(language.element.PackageID);

                if (this.currentVariable != null && this.currentVariable.ParentID != 0)
                {
                    this.currentBusinessKnowledge = this.eaUtils.repository.GetElementByID(this.currentVariable.ParentID);
                }

                this.main.showAsJson(this.currentBusinessKnowledge, LanguageTargetPackage);

            }
        }
        private void agregar_MouseHover(object sender, EventArgs e)
        {
            this.agregar.Image = DMN.Properties.Resources.icons8_Plus_64;
        }

        private void agregar_MouseLeave(object sender, EventArgs e)
        {
            this.agregar.Image = DMN.Properties.Resources.icons8_Plus_32;
        }

        private void borrar_MouseHover(object sender, EventArgs e)
        {
            this.borrar.Image = DMN.Properties.Resources.icons8_Trash_64;
        }

        private void borrar_MouseLeave(object sender, EventArgs e)
        {
            this.borrar.Image = DMN.Properties.Resources.icons8_Trash_32;
        }

        private void actualizar_MouseHover(object sender, EventArgs e)
        {
            this.actualizar.Image = DMN.Properties.Resources.icons8_Save_64;
        }

        private void actualizar_MouseLeave(object sender, EventArgs e)
        {
            this.actualizar.Image = DMN.Properties.Resources.icons8_Save_32;

        }

        private void generarEnumeracion_MouseHover(object sender, EventArgs e)
        {
            this.generarEnumeracion.Image = DMN.Properties.Resources.icons8_Add_Property_64;
        }

        private void generarEnumeracion_MouseLeave(object sender, EventArgs e)
        {
            this.generarEnumeracion.Image = DMN.Properties.Resources.icons8_Add_Property_32;
        }

        private void controlButtonGenerarCodigo_MouseHover(object sender, EventArgs e)
        {
            this.controlButtonGenerarCodigo.Image = DMN.Properties.Resources.icons8_Source_Code_64;
        }

        private void controlButtonGenerarCodigo_MouseLeave(object sender, EventArgs e)
        {
            this.controlButtonGenerarCodigo.Image = DMN.Properties.Resources.icons8_Source_Code_32;
        }

        private void controlButtonGenerarXml_MouseHover(object sender, EventArgs e)
        {
            this.controlButtonGenerarXml.Image = DMN.Properties.Resources.icons8_XML_64;
        }

        private void controlButtonGenerarXml_MouseLeave(object sender, EventArgs e)
        {
            this.controlButtonGenerarXml.Image = DMN.Properties.Resources.icons8_XML_32;

        }

        private void controlButtonGenerarJson_MouseHover(object sender, EventArgs e)
        {
            this.controlButtonGenerarJson.Image = DMN.Properties.Resources.icons8_JSON_64;
        }

        private void controlButtonGenerarJson_MouseLeave(object sender, EventArgs e)
        {
            this.controlButtonGenerarJson.Image = DMN.Properties.Resources.icons8_JSON_32;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void controlTextBoxFormat_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void controlTextBoxDefault_TextChanged(object sender, EventArgs e)
        {

        }

        private void VariableEditor_Load(object sender, EventArgs e)
        {

        }

        private void dataType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void controlLabelDefault_Click(object sender, EventArgs e)
        {

        }
    }
}
