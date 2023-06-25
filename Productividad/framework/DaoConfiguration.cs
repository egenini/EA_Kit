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

namespace Productividad.framework.ui
{
    public partial class DaoConfiguration : UserControl
    {
        int rowIndex;
        private JooqDaoGenerator caller;
        private bool enableEvents = false;

        public DaoConfiguration()
        {
            InitializeComponent();
        }

        public void Show(bool enableEvents )
        {
            this.enableEvents = enableEvents;
        }

        public void setCaller(JooqDaoGenerator caller )
        {
            this.caller = caller;
        }

        public void setFromTableName( Element table )
        {
            string tableName = table.Name;
            this.control_table_constant.Text = tableName.ToUpper();
            this.control_table_class.Text = EAUtils.StringUtils.toPascal(tableName);
            this.control_entity.Text = control_table_class.Text;
        }

        public void set(string entity, string package, string package_entity, string package_tables, string table_class, string table_constant)
        {
            this.control_entity.Text = entity;
            this.control_package.Text = package;
            this.control_package_entity.Text = package_entity;
            this.control_package_tables.Text = package_tables;
            this.control_table_class.Text = table_class;
            this.control_table_constant.Text = table_constant;

        }
        /// <summary>
        /// Se usa en la primera vez cuando aún no se ha onfigurado nunca.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="attributeName"></param>
        public void add( string columnName, string attributeName )
        {
            rowIndex = this.control_dataGridView.Rows.Add();

            this.control_dataGridView.Rows[rowIndex].Cells["column"].Value = columnName;
            this.control_dataGridView.Rows[rowIndex].Cells["attribute"].Value = attributeName;
        }

        public void add(string columnName, string attributeName, bool pk, bool search)
        {
            rowIndex = this.control_dataGridView.Rows.Add();

            this.control_dataGridView.Rows[rowIndex].Cells["pk"].Value = pk;
            this.control_dataGridView.Rows[rowIndex].Cells["search"].Value = search;
            this.control_dataGridView.Rows[rowIndex].Cells["column"].Value    = columnName;
            this.control_dataGridView.Rows[rowIndex].Cells["attribute"].Value = attributeName;

        }

        private void control_button_aceptar_Click(object sender, EventArgs e)
        {
            JooqDaoInfo info = new JooqDaoInfo();
            info.package = this.control_package.Text;
            info.package_entity = this.control_package_entity.Text;
            info.package_tables = this.control_package_tables.Text;
            info.entity = this.control_entity.Text;
            info.table_class = this.control_table_class.Text;
            info.table_constant = this.control_table_constant.Text;

            string columnName;
            string attributeName;
            bool pk = false;
            bool search = false;

            foreach ( DataGridViewRow row in this.control_dataGridView.Rows)
            {
                pk = row.Cells[0].Value != null ? (bool)row.Cells[0].Value : false;
                search = row.Cells[1].Value != null ? (bool)row.Cells[1].Value : false;
                columnName = (string) row.Cells[2].Value;
                attributeName = (string) row.Cells[3].Value;

                info.add(columnName, attributeName, pk, search);
            }

            this.caller.onAceptConfigure(info);
        }

        private void control_button_cancelar_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void control_dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if( enableEvents )
            {
                if (e.ColumnIndex == 0)
                {
                    bool value = (bool)this.control_dataGridView.Rows[e.RowIndex].Cells[0].Value;

                    enableEvents = false;

                    this.control_dataGridView.Rows[e.RowIndex].Cells[1].ReadOnly = value;

                    this.control_dataGridView.Rows[e.RowIndex].Cells[1].Value = false;

                    this.control_dataGridView.Rows[e.RowIndex].DefaultCellStyle.ForeColor = value ? Color.Gray : Color.Black;

                    enableEvents = true;
                }
            }
        }
    }
}
