namespace CertComplete
{
    partial class Preferences
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.BackgroundColor = new System.Windows.Forms.Button();
            this.TextColor = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.Dark_Mode = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.JSONSymbolColor = new System.Windows.Forms.Button();
            this.JSONNumberColor = new System.Windows.Forms.Button();
            this.JSONStringColor = new System.Windows.Forms.Button();
            this.JSONVariableColor = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Save = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Background Color";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Text Color";
            // 
            // BackgroundColor
            // 
            this.BackgroundColor.Location = new System.Drawing.Point(119, 58);
            this.BackgroundColor.Name = "BackgroundColor";
            this.BackgroundColor.Size = new System.Drawing.Size(75, 23);
            this.BackgroundColor.TabIndex = 2;
            this.BackgroundColor.Text = "Pick";
            this.BackgroundColor.UseVisualStyleBackColor = true;
            this.BackgroundColor.Click += new System.EventHandler(this.BackgroundColor_Click);
            // 
            // TextColor
            // 
            this.TextColor.Location = new System.Drawing.Point(119, 96);
            this.TextColor.Name = "TextColor";
            this.TextColor.Size = new System.Drawing.Size(75, 23);
            this.TextColor.TabIndex = 3;
            this.TextColor.Text = "Pick";
            this.TextColor.UseVisualStyleBackColor = true;
            this.TextColor.Click += new System.EventHandler(this.TextColor_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Dark Mode";
            // 
            // Dark_Mode
            // 
            this.Dark_Mode.Location = new System.Drawing.Point(119, 19);
            this.Dark_Mode.Name = "Dark_Mode";
            this.Dark_Mode.Size = new System.Drawing.Size(75, 23);
            this.Dark_Mode.TabIndex = 5;
            this.Dark_Mode.Text = "Enable";
            this.Dark_Mode.UseVisualStyleBackColor = true;
            this.Dark_Mode.Click += new System.EventHandler(this.Dark_Mode_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.JSONSymbolColor);
            this.groupBox1.Controls.Add(this.JSONNumberColor);
            this.groupBox1.Controls.Add(this.JSONStringColor);
            this.groupBox1.Controls.Add(this.JSONVariableColor);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(12, 174);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(206, 168);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "JSON Colors";
            // 
            // JSONSymbolColor
            // 
            this.JSONSymbolColor.Location = new System.Drawing.Point(115, 131);
            this.JSONSymbolColor.Name = "JSONSymbolColor";
            this.JSONSymbolColor.Size = new System.Drawing.Size(75, 23);
            this.JSONSymbolColor.TabIndex = 7;
            this.JSONSymbolColor.Text = "Pick";
            this.JSONSymbolColor.UseVisualStyleBackColor = true;
            this.JSONSymbolColor.Click += new System.EventHandler(this.JSONSymbolColor_Click);
            // 
            // JSONNumberColor
            // 
            this.JSONNumberColor.Location = new System.Drawing.Point(115, 93);
            this.JSONNumberColor.Name = "JSONNumberColor";
            this.JSONNumberColor.Size = new System.Drawing.Size(75, 23);
            this.JSONNumberColor.TabIndex = 6;
            this.JSONNumberColor.Text = "Pick";
            this.JSONNumberColor.UseVisualStyleBackColor = true;
            this.JSONNumberColor.Click += new System.EventHandler(this.JSONNumberColor_Click);
            // 
            // JSONStringColor
            // 
            this.JSONStringColor.Location = new System.Drawing.Point(115, 55);
            this.JSONStringColor.Name = "JSONStringColor";
            this.JSONStringColor.Size = new System.Drawing.Size(75, 23);
            this.JSONStringColor.TabIndex = 5;
            this.JSONStringColor.Text = "Pick";
            this.JSONStringColor.UseVisualStyleBackColor = true;
            this.JSONStringColor.Click += new System.EventHandler(this.JSONStringColor_Click);
            // 
            // JSONVariableColor
            // 
            this.JSONVariableColor.Location = new System.Drawing.Point(115, 19);
            this.JSONVariableColor.Name = "JSONVariableColor";
            this.JSONVariableColor.Size = new System.Drawing.Size(75, 23);
            this.JSONVariableColor.TabIndex = 4;
            this.JSONVariableColor.Text = "Pick";
            this.JSONVariableColor.UseVisualStyleBackColor = true;
            this.JSONVariableColor.Click += new System.EventHandler(this.JSONVariableColor_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 136);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Symbol Color";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 98);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Number Color";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 60);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "String Color";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Variable Color";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Dark_Mode);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.TextColor);
            this.groupBox2.Controls.Add(this.BackgroundColor);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(12, 38);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(206, 130);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Basic Colors";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(449, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.Save_Click);
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(318, 267);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(106, 61);
            this.Save.TabIndex = 9;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // Preferences
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 370);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Preferences";
            this.Text = "Preferences";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button BackgroundColor;
        private System.Windows.Forms.Button TextColor;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button Dark_Mode;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button JSONSymbolColor;
        private System.Windows.Forms.Button JSONNumberColor;
        private System.Windows.Forms.Button JSONStringColor;
        private System.Windows.Forms.Button JSONVariableColor;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.Button Save;
    }
}