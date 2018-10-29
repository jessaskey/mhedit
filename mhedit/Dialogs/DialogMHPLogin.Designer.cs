namespace mhedit
{
    partial class DialogMHPLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogMHPLogin));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonOk = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.labelMazeName = new System.Windows.Forms.Label();
            this.pictureBoxMaze = new System.Windows.Forms.PictureBox();
            this.checkBoxSavePassword = new System.Windows.Forms.CheckBox();
            this.linkLabelWebLink = new System.Windows.Forms.LinkLabel();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMaze)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(25, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(442, 67);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 106);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "User Email:";
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Location = new System.Drawing.Point(119, 103);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(236, 20);
            this.textBoxUsername.TabIndex = 2;
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(119, 132);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(123, 20);
            this.textBoxPassword.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 135);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Password:";
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(373, 158);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 6;
            this.buttonOk.Text = "Upload";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 163);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Maze Name:";
            // 
            // labelMazeName
            // 
            this.labelMazeName.AutoSize = true;
            this.labelMazeName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMazeName.ForeColor = System.Drawing.Color.Blue;
            this.labelMazeName.Location = new System.Drawing.Point(126, 163);
            this.labelMazeName.Name = "labelMazeName";
            this.labelMazeName.Size = new System.Drawing.Size(113, 13);
            this.labelMazeName.TabIndex = 9;
            this.labelMazeName.Text = "<maze name here>";
            // 
            // pictureBoxMaze
            // 
            this.pictureBoxMaze.BackColor = System.Drawing.Color.Black;
            this.pictureBoxMaze.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBoxMaze.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxMaze.Location = new System.Drawing.Point(28, 191);
            this.pictureBoxMaze.Name = "pictureBoxMaze";
            this.pictureBoxMaze.Size = new System.Drawing.Size(420, 181);
            this.pictureBoxMaze.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxMaze.TabIndex = 11;
            this.pictureBoxMaze.TabStop = false;
            // 
            // checkBoxSavePassword
            // 
            this.checkBoxSavePassword.AutoSize = true;
            this.checkBoxSavePassword.Location = new System.Drawing.Point(255, 135);
            this.checkBoxSavePassword.Name = "checkBoxSavePassword";
            this.checkBoxSavePassword.Size = new System.Drawing.Size(100, 17);
            this.checkBoxSavePassword.TabIndex = 12;
            this.checkBoxSavePassword.Text = "Save Password";
            this.checkBoxSavePassword.UseVisualStyleBackColor = true;
            // 
            // linkLabelWebLink
            // 
            this.linkLabelWebLink.AutoSize = true;
            this.linkLabelWebLink.Location = new System.Drawing.Point(26, 74);
            this.linkLabelWebLink.Name = "linkLabelWebLink";
            this.linkLabelWebLink.Size = new System.Drawing.Size(107, 13);
            this.linkLabelWebLink.TabIndex = 13;
            this.linkLabelWebLink.TabStop = true;
            this.linkLabelWebLink.Text = "http://mhp.askey.org";
            this.linkLabelWebLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelWebLink_LinkClicked);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(139, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(201, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "<- Create an account here to enter below";
            // 
            // DialogMHPLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 386);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.linkLabelWebLink);
            this.Controls.Add(this.checkBoxSavePassword);
            this.Controls.Add(this.pictureBoxMaze);
            this.Controls.Add(this.labelMazeName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxUsername);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogMHPLogin";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Major Havoc Project Upload";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMaze)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelMazeName;
        private System.Windows.Forms.PictureBox pictureBoxMaze;
        private System.Windows.Forms.CheckBox checkBoxSavePassword;
        private System.Windows.Forms.LinkLabel linkLabelWebLink;
        private System.Windows.Forms.Label label5;
    }
}