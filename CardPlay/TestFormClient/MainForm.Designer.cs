namespace TestFormClient
{
    partial class MainForm
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
            this.panelD = new System.Windows.Forms.Panel();
            this.panelC = new System.Windows.Forms.Panel();
            this.panelB = new System.Windows.Forms.Panel();
            this.panelE = new System.Windows.Forms.Panel();
            this.panelA = new System.Windows.Forms.Panel();
            this.txtConsole = new System.Windows.Forms.TextBox();
            this.panelInfo = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.lb_pot = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.panelInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelD
            // 
            this.panelD.Location = new System.Drawing.Point(259, 12);
            this.panelD.Name = "panelD";
            this.panelD.Size = new System.Drawing.Size(200, 146);
            this.panelD.TabIndex = 0;
            // 
            // panelC
            // 
            this.panelC.Location = new System.Drawing.Point(569, 12);
            this.panelC.Name = "panelC";
            this.panelC.Size = new System.Drawing.Size(200, 146);
            this.panelC.TabIndex = 0;
            // 
            // panelB
            // 
            this.panelB.Location = new System.Drawing.Point(793, 157);
            this.panelB.Name = "panelB";
            this.panelB.Size = new System.Drawing.Size(200, 157);
            this.panelB.TabIndex = 0;
            // 
            // panelE
            // 
            this.panelE.Location = new System.Drawing.Point(33, 157);
            this.panelE.Name = "panelE";
            this.panelE.Size = new System.Drawing.Size(200, 157);
            this.panelE.TabIndex = 0;
            // 
            // panelA
            // 
            this.panelA.Location = new System.Drawing.Point(259, 334);
            this.panelA.Name = "panelA";
            this.panelA.Size = new System.Drawing.Size(510, 173);
            this.panelA.TabIndex = 0;
            // 
            // txtConsole
            // 
            this.txtConsole.Location = new System.Drawing.Point(793, 334);
            this.txtConsole.Multiline = true;
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.Size = new System.Drawing.Size(200, 173);
            this.txtConsole.TabIndex = 1;
            // 
            // panelInfo
            // 
            this.panelInfo.Controls.Add(this.label1);
            this.panelInfo.Location = new System.Drawing.Point(33, 12);
            this.panelInfo.Name = "panelInfo";
            this.panelInfo.Size = new System.Drawing.Size(200, 136);
            this.panelInfo.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // lb_pot
            // 
            this.lb_pot.AutoSize = true;
            this.lb_pot.Font = new System.Drawing.Font("宋体", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_pot.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lb_pot.Location = new System.Drawing.Point(476, 237);
            this.lb_pot.Name = "lb_pot";
            this.lb_pot.Size = new System.Drawing.Size(22, 21);
            this.lb_pot.TabIndex = 2;
            this.lb_pot.Text = "0";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button1.Location = new System.Drawing.Point(480, 281);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 33);
            this.button1.TabIndex = 3;
            this.button1.Text = "准备";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1045, 535);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lb_pot);
            this.Controls.Add(this.txtConsole);
            this.Controls.Add(this.panelE);
            this.Controls.Add(this.panelB);
            this.Controls.Add(this.panelC);
            this.Controls.Add(this.panelA);
            this.Controls.Add(this.panelInfo);
            this.Controls.Add(this.panelD);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.panelInfo.ResumeLayout(false);
            this.panelInfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelD;
        private System.Windows.Forms.Panel panelC;
        private System.Windows.Forms.Panel panelB;
        private System.Windows.Forms.Panel panelE;
        private System.Windows.Forms.Panel panelA;
        private System.Windows.Forms.TextBox txtConsole;
        private System.Windows.Forms.Panel panelInfo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lb_pot;
        private System.Windows.Forms.Button button1;
    }
}