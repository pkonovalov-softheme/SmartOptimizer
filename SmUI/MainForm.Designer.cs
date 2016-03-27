namespace SmUI
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
            this.StealthModeBtn = new System.Windows.Forms.Button();
            this.IsOnlineLabel = new System.Windows.Forms.Label();
            this.IsOnlineValue = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // StealthModeBtn
            // 
            this.StealthModeBtn.Location = new System.Drawing.Point(72, 122);
            this.StealthModeBtn.Name = "StealthModeBtn";
            this.StealthModeBtn.Size = new System.Drawing.Size(120, 23);
            this.StealthModeBtn.TabIndex = 0;
            this.StealthModeBtn.Text = "Invalid State";
            this.StealthModeBtn.UseVisualStyleBackColor = true;
            this.StealthModeBtn.Click += new System.EventHandler(this.StealthModeBtn_Click);
            // 
            // IsOnlineLabel
            // 
            this.IsOnlineLabel.AutoSize = true;
            this.IsOnlineLabel.Location = new System.Drawing.Point(69, 89);
            this.IsOnlineLabel.Name = "IsOnlineLabel";
            this.IsOnlineLabel.Size = new System.Drawing.Size(48, 13);
            this.IsOnlineLabel.TabIndex = 1;
            this.IsOnlineLabel.Text = "IsOnline:";
            // 
            // IsOnlineValue
            // 
            this.IsOnlineValue.AutoSize = true;
            this.IsOnlineValue.Location = new System.Drawing.Point(124, 89);
            this.IsOnlineValue.Name = "IsOnlineValue";
            this.IsOnlineValue.Size = new System.Drawing.Size(29, 13);
            this.IsOnlineValue.TabIndex = 2;
            this.IsOnlineValue.Text = "false";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(307, 171);
            this.Controls.Add(this.IsOnlineValue);
            this.Controls.Add(this.IsOnlineLabel);
            this.Controls.Add(this.StealthModeBtn);
            this.Name = "MainForm";
            this.Text = "Smart Optimizer Manager";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button StealthModeBtn;
        private System.Windows.Forms.Label IsOnlineLabel;
        private System.Windows.Forms.Label IsOnlineValue;
    }
}

