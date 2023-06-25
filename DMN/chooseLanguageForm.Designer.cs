namespace DMN
{
    partial class chooseLanguageForm
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
            this.chooseLanguage1 = new DMN.ChooseLanguage();
            this.SuspendLayout();
            // 
            // chooseLanguage1
            // 
            this.chooseLanguage1.AutoSize = true;
            this.chooseLanguage1.BackColor = System.Drawing.SystemColors.Window;
            this.chooseLanguage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chooseLanguage1.Location = new System.Drawing.Point(0, 0);
            this.chooseLanguage1.Name = "chooseLanguage1";
            this.chooseLanguage1.Size = new System.Drawing.Size(1311, 733);
            this.chooseLanguage1.TabIndex = 0;
            this.chooseLanguage1.Load += new System.EventHandler(this.chooseLanguage1_Load);
            // 
            // chooseLanguageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1311, 733);
            this.Controls.Add(this.chooseLanguage1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "chooseLanguageForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "chooseLanguageForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ChooseLanguage chooseLanguage1;
    }
}