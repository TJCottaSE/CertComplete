namespace CertComplete
{
    partial class PAX_Settings_Form
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.save = new System.Windows.Forms.Button();
            this.destinationPort = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.destinationIP = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.baudRate = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.serialPort = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.timeOut = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.commType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.save);
            this.groupBox1.Controls.Add(this.destinationPort);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.destinationIP);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.baudRate);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.serialPort);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.timeOut);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.commType);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(325, 330);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Comm Settings";
            // 
            // save
            // 
            this.save.Location = new System.Drawing.Point(61, 269);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(213, 40);
            this.save.TabIndex = 13;
            this.save.Text = "Save";
            this.save.UseVisualStyleBackColor = true;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // destinationPort
            // 
            this.destinationPort.Location = new System.Drawing.Point(153, 229);
            this.destinationPort.Name = "destinationPort";
            this.destinationPort.Size = new System.Drawing.Size(121, 20);
            this.destinationPort.TabIndex = 12;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(58, 232);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Destination Port";
            // 
            // destinationIP
            // 
            this.destinationIP.Location = new System.Drawing.Point(153, 193);
            this.destinationIP.Name = "destinationIP";
            this.destinationIP.Size = new System.Drawing.Size(121, 20);
            this.destinationIP.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(58, 196);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(73, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Destination IP";
            // 
            // baudRate
            // 
            this.baudRate.FormattingEnabled = true;
            this.baudRate.Items.AddRange(new object[] {
            "9600",
            "115200"});
            this.baudRate.Location = new System.Drawing.Point(153, 155);
            this.baudRate.Name = "baudRate";
            this.baudRate.Size = new System.Drawing.Size(121, 21);
            this.baudRate.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(58, 158);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Baud Rate";
            // 
            // serialPort
            // 
            this.serialPort.Location = new System.Drawing.Point(153, 119);
            this.serialPort.Name = "serialPort";
            this.serialPort.Size = new System.Drawing.Size(121, 20);
            this.serialPort.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(58, 122);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Serial Port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(280, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "ms";
            // 
            // timeOut
            // 
            this.timeOut.Location = new System.Drawing.Point(153, 84);
            this.timeOut.Name = "timeOut";
            this.timeOut.Size = new System.Drawing.Size(121, 20);
            this.timeOut.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(58, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Time Out";
            // 
            // commType
            // 
            this.commType.FormattingEnabled = true;
            this.commType.Items.AddRange(new object[] {
            "UART",
            "TCP",
            "HTTP",
            "SSL",
            "HTTPS"});
            this.commType.Location = new System.Drawing.Point(153, 46);
            this.commType.Name = "commType";
            this.commType.Size = new System.Drawing.Size(121, 21);
            this.commType.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(58, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Comm Type";
            // 
            // PAX_Settings_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 350);
            this.Controls.Add(this.groupBox1);
            this.Name = "PAX_Settings_Form";
            this.Text = "Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox destinationPort;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox destinationIP;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox baudRate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox serialPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox timeOut;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox commType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button save;
    }
}