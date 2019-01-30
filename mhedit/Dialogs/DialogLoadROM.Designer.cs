namespace mhedit
{
    partial class DialogLoadROM
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogLoadROM));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonMHPE = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxROMPath = new System.Windows.Forms.TextBox();
            this.buttonBrowseFolder = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonMHPE);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(387, 72);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source Game";
            // 
            // radioButtonMHPE
            // 
            this.radioButtonMHPE.AutoSize = true;
            this.radioButtonMHPE.Checked = true;
            this.radioButtonMHPE.Location = new System.Drawing.Point(24, 31);
            this.radioButtonMHPE.Name = "radioButtonMHPE";
            this.radioButtonMHPE.Size = new System.Drawing.Size(182, 17);
            this.radioButtonMHPE.TabIndex = 1;
            this.radioButtonMHPE.TabStop = true;
            this.radioButtonMHPE.Text = "Major Havoc - The Promised End";
            this.radioButtonMHPE.UseVisualStyleBackColor = true;
            this.radioButtonMHPE.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 101);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Folder Location:";
            // 
            // textBoxROMPath
            // 
            this.textBoxROMPath.Location = new System.Drawing.Point(101, 98);
            this.textBoxROMPath.Name = "textBoxROMPath";
            this.textBoxROMPath.Size = new System.Drawing.Size(270, 20);
            this.textBoxROMPath.TabIndex = 2;
            this.textBoxROMPath.TextChanged += new System.EventHandler(this.Textbox_TextChanged);
            this.textBoxROMPath.MouseLeave += new System.EventHandler(this.textBoxTT_MouseLeave);
            this.textBoxROMPath.MouseHover += new System.EventHandler(this.textBoxTT_MouseHover);
            // 
            // buttonBrowseFolder
            // 
            this.buttonBrowseFolder.Image = ((System.Drawing.Image)(resources.GetObject("buttonBrowseFolder.Image")));
            this.buttonBrowseFolder.Location = new System.Drawing.Point(373, 96);
            this.buttonBrowseFolder.Name = "buttonBrowseFolder";
            this.buttonBrowseFolder.Size = new System.Drawing.Size(26, 23);
            this.buttonBrowseFolder.TabIndex = 3;
            this.buttonBrowseFolder.UseVisualStyleBackColor = true;
            this.buttonBrowseFolder.Click += new System.EventHandler(this.buttonBrowseFolder_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Enabled = false;
            this.buttonOK.Location = new System.Drawing.Point(231, 142);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(324, 142);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // DialogLoadROM
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(411, 177);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonBrowseFolder);
            this.Controls.Add(this.textBoxROMPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogLoadROM";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Load Mazes from ROM Images";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonMHPE;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxROMPath;
        private System.Windows.Forms.Button buttonBrowseFolder;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
    }
}