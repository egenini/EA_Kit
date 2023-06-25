namespace UIResources
{
    partial class SelectFromListForm
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
            this.selectFromList1 = new UIResources.SelectFromList();
            this.SuspendLayout();
            // 
            // selectFromList1
            // 
            this.selectFromList1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.selectFromList1.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.selectFromList1.BackColor = System.Drawing.SystemColors.Window;
            this.selectFromList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectFromList1.Location = new System.Drawing.Point(0, 0);
            this.selectFromList1.Name = "selectFromList1";
            this.selectFromList1.Size = new System.Drawing.Size(1162, 612);
            this.selectFromList1.TabIndex = 0;
            this.selectFromList1.Load += new System.EventHandler(this.selectFromList1_Load);
            // 
            // SelectFromListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1162, 612);
            this.Controls.Add(this.selectFromList1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SelectFromListForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SelectFromListForm";
            this.ResumeLayout(false);

        }

        #endregion

        private SelectFromList selectFromList1;
    }
}