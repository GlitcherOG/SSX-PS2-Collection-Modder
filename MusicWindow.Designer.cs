namespace SSX_Modder
{
    partial class MusicWindow
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.SSX2000 = new System.Windows.Forms.TabPage();
            this.SSXTricky = new System.Windows.Forms.TabPage();
            this.SSX3 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button4 = new System.Windows.Forms.Button();
            this.General = new System.Windows.Forms.TabPage();
            this.GeneralMPFToWav = new System.Windows.Forms.Button();
            this.GeneralWavToMPF = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.SSX2000.SuspendLayout();
            this.General.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.SSX2000);
            this.tabControl1.Controls.Add(this.SSXTricky);
            this.tabControl1.Controls.Add(this.SSX3);
            this.tabControl1.Controls.Add(this.General);
            this.tabControl1.Location = new System.Drawing.Point(13, 13);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(775, 425);
            this.tabControl1.TabIndex = 0;
            // 
            // SSX2000
            // 
            this.SSX2000.Controls.Add(this.button4);
            this.SSX2000.Controls.Add(this.listBox1);
            this.SSX2000.Controls.Add(this.button3);
            this.SSX2000.Controls.Add(this.button2);
            this.SSX2000.Controls.Add(this.button1);
            this.SSX2000.Location = new System.Drawing.Point(4, 22);
            this.SSX2000.Name = "SSX2000";
            this.SSX2000.Padding = new System.Windows.Forms.Padding(3);
            this.SSX2000.Size = new System.Drawing.Size(767, 399);
            this.SSX2000.TabIndex = 0;
            this.SSX2000.Text = "SSX (2000)";
            this.SSX2000.UseVisualStyleBackColor = true;
            // 
            // SSXTricky
            // 
            this.SSXTricky.Location = new System.Drawing.Point(4, 22);
            this.SSXTricky.Name = "SSXTricky";
            this.SSXTricky.Padding = new System.Windows.Forms.Padding(3);
            this.SSXTricky.Size = new System.Drawing.Size(767, 399);
            this.SSXTricky.TabIndex = 1;
            this.SSXTricky.Text = "SSX Tricky";
            this.SSXTricky.UseVisualStyleBackColor = true;
            // 
            // SSX3
            // 
            this.SSX3.Location = new System.Drawing.Point(4, 22);
            this.SSX3.Name = "SSX3";
            this.SSX3.Size = new System.Drawing.Size(767, 399);
            this.SSX3.TabIndex = 2;
            this.SSX3.Text = "SSX 3";
            this.SSX3.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(165, 370);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Load .BIG";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(597, 370);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(246, 370);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(99, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "Convert To Wav";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(7, 7);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(155, 381);
            this.listBox1.TabIndex = 3;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(689, 370);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 4;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // General
            // 
            this.General.Controls.Add(this.GeneralWavToMPF);
            this.General.Controls.Add(this.GeneralMPFToWav);
            this.General.Location = new System.Drawing.Point(4, 22);
            this.General.Name = "General";
            this.General.Size = new System.Drawing.Size(767, 399);
            this.General.TabIndex = 3;
            this.General.Text = "General";
            this.General.UseVisualStyleBackColor = true;
            // 
            // GeneralMPFToWav
            // 
            this.GeneralMPFToWav.Location = new System.Drawing.Point(15, 19);
            this.GeneralMPFToWav.Name = "GeneralMPFToWav";
            this.GeneralMPFToWav.Size = new System.Drawing.Size(105, 23);
            this.GeneralMPFToWav.TabIndex = 0;
            this.GeneralMPFToWav.Text = "Mpf To Wav";
            this.GeneralMPFToWav.UseVisualStyleBackColor = true;
            this.GeneralMPFToWav.Click += new System.EventHandler(this.GeneralMPFToWav_Click);
            // 
            // GeneralWavToMPF
            // 
            this.GeneralWavToMPF.Location = new System.Drawing.Point(126, 19);
            this.GeneralWavToMPF.Name = "GeneralWavToMPF";
            this.GeneralWavToMPF.Size = new System.Drawing.Size(105, 23);
            this.GeneralWavToMPF.TabIndex = 1;
            this.GeneralWavToMPF.Text = "Wav To MPF";
            this.GeneralWavToMPF.UseVisualStyleBackColor = true;
            this.GeneralWavToMPF.Click += new System.EventHandler(this.GeneralWavToMPF_Click);
            // 
            // MusicWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabControl1);
            this.Name = "MusicWindow";
            this.Text = "MusicVideoWindow";
            this.tabControl1.ResumeLayout(false);
            this.SSX2000.ResumeLayout(false);
            this.General.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage SSX2000;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabPage SSXTricky;
        private System.Windows.Forms.TabPage SSX3;
        private System.Windows.Forms.TabPage General;
        private System.Windows.Forms.Button GeneralWavToMPF;
        private System.Windows.Forms.Button GeneralMPFToWav;
    }
}