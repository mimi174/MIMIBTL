using System.Windows.Forms;

namespace N4_BTCM
{
    partial class UCThongKe
    {
        ///  <summary>
        ///  Required designer variable.
        ///  </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel panelThongKe;
        private System.Windows.Forms.Button btnXemThongKe;

        ///  <summary>
        ///  Clean up any resources being used.
        ///  </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        ///  <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        ///  </summary>
        private void InitializeComponent()
        {
            this.panelThongKe = new System.Windows.Forms.Panel();
            this.btnXemThongKe = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // 
            // panelThongKe
            // 
            this.panelThongKe.Location = new System.Drawing.Point(20, 20);
            this.panelThongKe.Size = new System.Drawing.Size(600, 250);
            this.panelThongKe.BorderStyle = BorderStyle.FixedSingle;
            this.panelThongKe.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));


            // 
            // btnXemThongKe
            // 
            this.btnXemThongKe.Location = new System.Drawing.Point(250, 290);
            this.btnXemThongKe.Size = new System.Drawing.Size(150, 40);
            this.btnXemThongKe.Text = "Xem Thống Kê";
            this.btnXemThongKe.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnXemThongKe.Click += new System.EventHandler(this.btnXemThongKe_Click);
            this.btnXemThongKe.Anchor = System.Windows.Forms.AnchorStyles.Bottom;

            // 
            // UCThongKe
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelThongKe);
            this.Controls.Add(this.btnXemThongKe);
            this.Name = "UCThongKe";
            this.Size = new System.Drawing.Size(650, 350);
            this.ResumeLayout(false);
        }

        #endregion
    }
}