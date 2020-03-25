namespace CertComplete
{
    partial class Innowi_Settings_Form
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
            this.destinationIP = new System.Windows.Forms.TextBox();
            this.destinationPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.save = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // destinationIP
            // 
            this.destinationIP.Location = new System.Drawing.Point(128, 32);
            this.destinationIP.Name = "destinationIP";
            this.destinationIP.Size = new System.Drawing.Size(140, 20);
            this.destinationIP.TabIndex = 0;
            // 
            // destinationPort
            // 
            this.destinationPort.Location = new System.Drawing.Point(128, 65);
            this.destinationPort.Name = "destinationPort";
            this.destinationPort.Size = new System.Drawing.Size(140, 20);
            this.destinationPort.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Destination IP";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Destination Port";
            // 
            // save
            // 
            this.save.Location = new System.Drawing.Point(33, 103);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(235, 55);
            this.save.TabIndex = 4;
            this.save.Text = "Save";
            this.save.UseVisualStyleBackColor = true;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // Innowi_Settings_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 170);
            this.Controls.Add(this.save);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.destinationPort);
            this.Controls.Add(this.destinationIP);
            this.Name = "Innowi_Settings_Form";
            this.Text = "Innowi_Settings_Form";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox destinationIP;
        private System.Windows.Forms.TextBox destinationPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button save;
    }
}