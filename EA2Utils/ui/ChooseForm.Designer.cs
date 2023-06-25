namespace EAUtils.ui
{
    partial class ChooseForm
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
            this.chooseUC = new ChooseUC();
            this.SuspendLayout();
            // 
            // chooseUC
            // 
            this.chooseUC.BackColor = System.Drawing.SystemColors.Window;
            this.chooseUC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chooseUC.Location = new System.Drawing.Point(0, 0);
            this.chooseUC.Name = "chooseUC";
            this.chooseUC.Size = new System.Drawing.Size(1192, 669);
            this.chooseUC.TabIndex = 0;
            this.chooseUC.Load += new System.EventHandler(this.chooseUC1_Load);
            // 
            // ChooseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1192, 669);
            this.Controls.Add(this.chooseUC);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ChooseForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ChooseForm";
            this.ResumeLayout(false);

        }

        #endregion

        private ChooseUC chooseUC;
    }
}