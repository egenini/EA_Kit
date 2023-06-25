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
using UIResources;

namespace ProjectManagement.ui
{
    public partial class EffortEditor : UserControl
    {
        public static string NAME = Properties.Resources.ESFUERZO_EDITOR_NAME;

        private EAUtils.EAUtils eaUtils;
        private Element task;
        private List<string> dificultad = Properties.Resources.DIFICULTAD_ALTA_MEDIA_BAJA.Split(',').ToList();
        private Esfuerzo esfuerzo;
        private List<string> complejidad = new List<string>();
        private List<string> esfuerzoTipo = new List<string>(2);
        bool cargaInicial = true;

        public EffortEditor()
        {
            InitializeComponent();

            System.Windows.Forms.ToolTip toolTip1 = new System.Windows.Forms.ToolTip();
            toolTip1.SetToolTip(this.label_asignado, Properties.Resources.tooltip_asignado);

            System.Windows.Forms.ToolTip toolTip2 = new System.Windows.Forms.ToolTip();
            toolTip2.SetToolTip(this.label_completado, Properties.Resources.tooltip_completado);

            System.Windows.Forms.ToolTip toolTip3 = new System.Windows.Forms.ToolTip();
            toolTip3.SetToolTip(this.label_utilizado, Properties.Resources.tooltip_utilizado);

            System.Windows.Forms.ToolTip toolTip4 = new System.Windows.Forms.ToolTip();
            toolTip4.SetToolTip(this.label_esperado, Properties.Resources.tooltip_esperado);

            this.fillCombos();
        }

        public void show( Element task, EAUtils.EAUtils eaUtils)
        {
            this.eaUtils = eaUtils;
            this.task    = task;
            this.esfuerzo = new Esfuerzo(eaUtils, task);

            this.find();

            this.tabPage.Text = this.esfuerzo.ejecutorManager.ejecutor.Name;

            this.comboBox_esfuerzoTipo.SelectedIndex = this.comboBox_esfuerzoTipo.FindString( this.esfuerzo.esfuerzoManager.currentEsfuerzo.Name );

            this.comboBox_dificultad.SelectedIndex = this.comboBox_dificultad.FindString( this.translateDifficulty(this.task.Difficulty) );
            this.comboBox_complejidad.SelectedIndex = this.comboBox_complejidad.FindString(this.findComplexityId(this.task.Complexity));

            if((decimal)this.esfuerzo.esfuerzoManager.esfuerzo > this.numericUpDown_esperado.Maximum)
            {
                this.esfuerzo.esfuerzoManager.esfuerzo = (float)this.numericUpDown_esperado.Maximum;
            }
            else if((decimal)this.esfuerzo.esfuerzoManager.esfuerzo < this.numericUpDown_esperado.Minimum)
            {
                this.esfuerzo.esfuerzoManager.esfuerzo = (float)this.numericUpDown_esperado.Minimum;
            }
            this.numericUpDown_esperado.Value = (decimal)this.esfuerzo.esfuerzoManager.esfuerzo;

            this.numericUpDown_asignado.Value     = this.esfuerzo.ejecutorManager.ejecutor.Time;
            this.numericUpDown_utilizado.Value    = this.esfuerzo.ejecutorManager.ejecutor.ActualHours;
            this.numericUpDown_completado.Value   = this.esfuerzo.ejecutorManager.ejecutor.PercentComplete;
            this.dateTimePicker_fechaInicio.Value = this.esfuerzo.ejecutorManager.ejecutor.DateStart;
            this.dateTimePicker_fechaFin.Value    = this.esfuerzo.ejecutorManager.ejecutor.DateEnd;

            cargaInicial = false;
        }

        private void fillCombos()
        {
            this.comboBox_dificultad.Items.Clear();

            foreach ( string v in dificultad)
            {
                this.comboBox_dificultad.Items.Add(v);
            }

            this.comboBox_complejidad.Items.Clear();

            this.comboBox_complejidad.Items.Add(Properties.Resources.COMPLEJIDAD_BAJA_LABEL);
            this.comboBox_complejidad.Items.Add(Properties.Resources.COMPLEJIDAD_MEDIA_LABEL);
            this.comboBox_complejidad.Items.Add(Properties.Resources.COMPLEJIDAD_ALTA_LABEL);

            this.comboBox_complejidad.DropDownStyle = ComboBoxStyle.DropDownList;

            this.complejidad.Clear();

            this.complejidad.Add(Properties.Resources.COMPLEJIDAD_BAJA_LABEL);
            this.complejidad.Add(Properties.Resources.COMPLEJIDAD_MEDIA_LABEL);
            this.complejidad.Add(Properties.Resources.COMPLEJIDAD_ALTA_LABEL);

            this.comboBox_esfuerzoTipo.Items.Clear();
            this.comboBox_esfuerzoTipo.Items.Add(Properties.Resources.ESFUERZO_CALCULADO);
            this.comboBox_esfuerzoTipo.Items.Add(Properties.Resources.ESFUERZO_MANUAL);

        }

        private void find()
        {
            if( ! this.esfuerzo.esfuerzoManager.localizar(task))
            {
                this.esfuerzo.esfuerzoManager.agregar(task, true);
            }

            if( ! this.esfuerzo.ejecutorManager.localizar(this.task))
            {
                this.esfuerzo.ejecutorManager.agregar(this.task);
            }
        }

        private void button_guardar_MouseHover(object sender, EventArgs e)
        {
            this.button_guardar.Image = Properties.Resources.icons8_Save_64;
        }

        private void button_guardar_MouseLeave(object sender, EventArgs e)
        {
            this.button_guardar.Image = Properties.Resources.icons8_Save_32;
        }

        private void comboBox_dificultad_SelectedIndexChanged(object sender, EventArgs e)
        {
            if( ! cargaInicial && this.task.Difficulty != dificultad[this.comboBox_dificultad.SelectedIndex])
            {
                this.task.Difficulty = dificultad[this.comboBox_dificultad.SelectedIndex];

                this.esfuerzo.calcular(this.task);

                this.numericUpDown_esperado.Value = (decimal)this.esfuerzo.esfuerzoManager.esfuerzo;

                this.numericUpDown_asignado.Value = this.esfuerzo.ejecutorManager.ejecutor.Time;
                this.numericUpDown_utilizado.Value = this.esfuerzo.ejecutorManager.ejecutor.ActualHours;
                this.numericUpDown_completado.Value = this.esfuerzo.ejecutorManager.ejecutor.PercentComplete;
                this.dateTimePicker_fechaInicio.Value = this.esfuerzo.ejecutorManager.ejecutor.DateStart;
                this.dateTimePicker_fechaFin.Value = this.esfuerzo.ejecutorManager.ejecutor.DateEnd;
            }
        }

        private void comboBox_complejidad_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cargaInicial && this.task.Complexity != (this.comboBox_complejidad.SelectedIndex + 1).ToString())
            {
                this.task.Complexity = (this.comboBox_complejidad.SelectedIndex + 1).ToString();

                this.esfuerzo.calcular(this.task);

                this.numericUpDown_esperado.Value = (decimal)this.esfuerzo.esfuerzoManager.esfuerzo;

                this.numericUpDown_asignado.Value = this.esfuerzo.ejecutorManager.ejecutor.Time;
                this.numericUpDown_utilizado.Value = this.esfuerzo.ejecutorManager.ejecutor.ActualHours;
                this.numericUpDown_completado.Value = this.esfuerzo.ejecutorManager.ejecutor.PercentComplete;
                this.dateTimePicker_fechaInicio.Value = this.esfuerzo.ejecutorManager.ejecutor.DateStart;
                this.dateTimePicker_fechaFin.Value = this.esfuerzo.ejecutorManager.ejecutor.DateEnd;
            }
        }

        private void numericUpDown_completado_ValueChanged(object sender, EventArgs e)
        {
            // si cambia este valor cambia el utilizado
            if ( !cargaInicial && this.numericUpDown_completado.Value != this.esfuerzo.ejecutorManager.ejecutor.PercentComplete)
            {
                this.esfuerzo.ejecutorManager.ejecutor.PercentComplete = (int)this.numericUpDown_completado.Value;

                this.esfuerzo.ejecutorManager.actualizarPorcentualAvance(this.numericUpDown_completado.Value);

                this.numericUpDown_utilizado.Value = this.esfuerzo.ejecutorManager.ejecutor.ActualHours;
            }
        }

        private string translateDifficulty( string toTranslate )
        {
            string d = this.dificultad.FirstOrDefault(stringToCheck => stringToCheck.Contains(toTranslate));

            return d;
        }

        private string findComplexityId( string c )
        {
            string d = "";

            if (c == "1")
            {
                d = this.complejidad[0];
            }
            else if (c == "2")
            {
                d = this.complejidad[1];
            }
            else if (c == "3")
            {
                d = this.complejidad[2];
            }
            return d;
        }

        private void numericUpDown_esperado_ValueChanged(object sender, EventArgs e)
        {
            if ( !cargaInicial && this.numericUpDown_esperado.Value != (decimal)this.esfuerzo.esfuerzoManager.esfuerzo)
            {
                if( ! this.esfuerzo.esfuerzoManager.inferir(this.task, this.numericUpDown_esperado.Value.ToString()))
                {
                    this.comboBox_esfuerzoTipo.SelectedIndex = this.comboBox_esfuerzoTipo.FindString(Properties.Resources.ESFUERZO_MANUAL);

                    // si cambia este valor deja de ser calculado.
                    this.esfuerzo.esfuerzoManager.setNoCalculado(this.numericUpDown_esperado.Value);
                }
                else
                {
                    this.comboBox_esfuerzoTipo.SelectedIndex = this.comboBox_esfuerzoTipo.FindString(Properties.Resources.ESFUERZO_CALCULADO);
                }

                this.esfuerzo.calcular(this.task);

                this.numericUpDown_esperado.Value = (decimal)this.esfuerzo.esfuerzoManager.esfuerzo;

                this.numericUpDown_asignado.Value     = this.esfuerzo.ejecutorManager.ejecutor.Time;
                this.numericUpDown_utilizado.Value    = this.esfuerzo.ejecutorManager.ejecutor.ActualHours;
                this.numericUpDown_completado.Value   = this.esfuerzo.ejecutorManager.ejecutor.PercentComplete;
                this.dateTimePicker_fechaInicio.Value = this.esfuerzo.ejecutorManager.ejecutor.DateStart;
                this.dateTimePicker_fechaFin.Value    = this.esfuerzo.ejecutorManager.ejecutor.DateEnd;
            }
        }

        private void comboBox_esfuerzoTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tipoEsfuerzo = (string)comboBox_esfuerzoTipo.SelectedItem;

            if (!cargaInicial && this.esfuerzo.esfuerzoManager.currentEsfuerzo.Name != tipoEsfuerzo 
                && tipoEsfuerzo == Properties.Resources.ESFUERZO_CALCULADO)
            {
                this.esfuerzo.esfuerzoManager.currentEsfuerzo.Name = Properties.Resources.ESFUERZO_CALCULADO;

                this.esfuerzo.calcular(this.task);

                this.numericUpDown_esperado.Value = (decimal)this.esfuerzo.esfuerzoManager.esfuerzo;

                this.numericUpDown_esperado.Value = this.esfuerzo.ejecutorManager.ejecutor.ExpectedHours;
                this.numericUpDown_utilizado.Value = this.esfuerzo.ejecutorManager.ejecutor.ActualHours;
                this.numericUpDown_completado.Value = this.esfuerzo.ejecutorManager.ejecutor.PercentComplete;
                this.dateTimePicker_fechaInicio.Value = this.esfuerzo.ejecutorManager.ejecutor.DateStart;
                this.dateTimePicker_fechaFin.Value = this.esfuerzo.ejecutorManager.ejecutor.DateEnd;
            }
        }

        private void numericUpDown_utilizado_ValueChanged(object sender, EventArgs e)
        {
            // cuando modifico el tiempo utilizado debo modificar el porcentual de completitud

            if (!cargaInicial && this.numericUpDown_utilizado.Value != this.esfuerzo.ejecutorManager.ejecutor.ActualHours)
            {
                this.esfuerzo.ejecutorManager.actualizarHorasUtilizadas(this.numericUpDown_utilizado.Value);

                this.numericUpDown_utilizado.Value  = this.esfuerzo.ejecutorManager.ejecutor.ActualHours;
                this.numericUpDown_completado.Value = this.esfuerzo.ejecutorManager.ejecutor.PercentComplete;
            }
        }

        private void dateTimePicker_fechaInicio_ValueChanged(object sender, EventArgs e)
        {
            // si cambio la fecha de inicio calculo la fecha final.
            if (!cargaInicial && this.esfuerzo.ejecutorManager.ejecutor.DateStart != this.dateTimePicker_fechaInicio.Value)
            {
                this.esfuerzo.ejecutorManager.progamar(this.dateTimePicker_fechaInicio.Value);

                dateTimePicker_fechaFin.Value = this.esfuerzo.ejecutorManager.ejecutor.DateEnd;
            }
        }

        private void dateTimePicker_fechaFin_ValueChanged(object sender, EventArgs e)
        {
            if (!cargaInicial && this.esfuerzo.ejecutorManager.ejecutor.DateEnd != dateTimePicker_fechaFin.Value ){

                this.esfuerzo.ejecutorManager.ejecutor.DateEnd = dateTimePicker_fechaFin.Value;
            }
        }

        private void button_guardar_Click(object sender, EventArgs e)
        {
            this.esfuerzo.persistir();
            Alert.Success(Properties.Resources.GUARDADO);
        }

        private void numericUpDown_asignado_ValueChanged(object sender, EventArgs e)
        {
            if (!cargaInicial && this.numericUpDown_asignado.Value != this.esfuerzo.ejecutorManager.ejecutor.Time)
            {
                this.esfuerzo.ejecutorManager.actualizarTiempoAsignado(this.numericUpDown_asignado.Value);

                this.dateTimePicker_fechaFin.Value = this.esfuerzo.ejecutorManager.ejecutor.DateEnd;
            }
        }
    }
}
