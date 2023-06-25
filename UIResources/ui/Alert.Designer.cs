using System.Drawing;
using System;
using System.Windows.Forms;

namespace UIResources
{
    partial class Alert
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

        public Alert(string mensaje, AlertType type)
        {
            this.InitializeComponent();

            this.message.Text = mensaje;

            switch (type)
            {

                case AlertType.info:
                    this.BackColor = Color.FromArgb(204, 229, 255);
                    this.pictureBox1.Image = imageList1.Images[0];
                    // cambiar la imagen que se muestra
                    break;
                case AlertType.success:
                    this.BackColor = Color.FromArgb(212, 237, 218);
                    this.pictureBox1.Image = imageList1.Images[1];
                    break;
                case AlertType.warning:
                    this.BackColor = Color.FromArgb(255, 243, 205);
                    this.pictureBox1.Image = imageList1.Images[2];
                    break;
                case AlertType.error:
                    this.BackColor = Color.FromArgb(248, 215, 218);
                    this.pictureBox1.Image = imageList1.Images[3];
                    break;

            }

            this.message.BackColor = this.BackColor;
        }

        public enum AlertType
        {
            info, success, error, warning
        }


        public static void Info(string mensaje)
        {
            new Alert(mensaje, AlertType.info).Show();
        }
        public static Alert InfoInstance(string mensaje)
        {
            return new Alert(mensaje, AlertType.info);
        }

        public static void Success(string mensaje)
        {
            new Alert(mensaje, AlertType.success).Show();
        }

        public static Alert SuccessInstance(string mensaje)
        {
            return new Alert(mensaje, AlertType.success);
        }
        public static void Warning(string mensaje)
        {
            new Alert(mensaje, AlertType.warning).Show();
        }
        public static Alert WarningInstance(string mensaje)
        {
            return new Alert(mensaje, AlertType.warning);
        }
        public static void Error(string mensaje)
        {
            new Alert(mensaje, AlertType.error).Show();
        }
        public static Alert ErrorInstance(string mensaje)
        {
            return new Alert(mensaje, AlertType.error);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            closeAlert.Start();
        }

        private void Alert_Load(object sender, EventArgs e)
        {
            //this.Top = Screen.PrimaryScreen.Bounds.Height - this.Height - 100;
            this.Top = -1 * (this.Height);
            this.Left = Screen.PrimaryScreen.Bounds.Width - this.Width - 60;

            show.Start();
        }

        private void timeout_Tick(object sender, EventArgs e)
        {
            this.closeAlert.Start();
        }

        int interval = 0;

        private void show_Tick(object sender, EventArgs e)
        {
            if (this.Top < 60)
            {
                this.Top += interval;
                interval += 2;
            }
            else
            {
                show.Stop();
            }
        }

        private void close_Tick(object sender, EventArgs e)
        {
            if (this.Opacity > 0)
            {
                this.Opacity -= 0.1;
            }
            else
            {
                this.Close();
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Alert));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.message = new System.Windows.Forms.TextBox();
            this.buttonClose = new System.Windows.Forms.Button();
            this.timeout = new System.Windows.Forms.Timer(this.components);
            this.show = new System.Windows.Forms.Timer(this.components);
            this.closeAlert = new System.Windows.Forms.Timer(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(29, 34);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(162, 133);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // message
            // 
            this.message.BackColor = System.Drawing.SystemColors.Window;
            this.message.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.message.Location = new System.Drawing.Point(197, 34);
            this.message.Multiline = true;
            this.message.Name = "message";
            this.message.Size = new System.Drawing.Size(901, 133);
            this.message.TabIndex = 3;
            this.message.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // buttonClose
            // 
            this.buttonClose.BackColor = System.Drawing.Color.White;
            this.buttonClose.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.buttonClose.Image = global::UIResources.Properties.Resources.icons8_Delete_32;
            this.buttonClose.Location = new System.Drawing.Point(1120, 2);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(50, 41);
            this.buttonClose.TabIndex = 4;
            this.buttonClose.UseVisualStyleBackColor = false;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // timeout
            // 
            this.timeout.Enabled = true;
            this.timeout.Interval = 5000;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "icons8-Alert-64.png");
            this.imageList1.Images.SetKeyName(1, "icons8-Good Quality-64.png");
            this.imageList1.Images.SetKeyName(2, "icons8-Warning Shield-64.png");
            this.imageList1.Images.SetKeyName(3, "icons8-Error-64.png");
            // 
            // Alert
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1179, 206);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.message);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Alert";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Alert";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox message;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Timer timeout;
        private System.Windows.Forms.Timer show;
        private System.Windows.Forms.Timer closeAlert;
        private System.Windows.Forms.ImageList imageList1;
    }
}