namespace MajorHavocEditor.Views.Dialogs
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
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxROMPath = new System.Windows.Forms.TextBox();
            this.buttonBrowseFolder = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.comboBoxGameDriver = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Folder Location:";
            // 
            // textBoxROMPath
            // 
            this.textBoxROMPath.Location = new System.Drawing.Point(101, 53);
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
            this.buttonBrowseFolder.Location = new System.Drawing.Point(373, 51);
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
            this.buttonOK.Location = new System.Drawing.Point(231, 97);
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
            this.buttonCancel.Location = new System.Drawing.Point(324, 97);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // comboBoxGameDriver
            // 
            this.comboBoxGameDriver.FormattingEnabled = true;
            this.comboBoxGameDriver.Items.AddRange(new object[] {
            "Major Havoc - The Promised End",
            "Major Havoc v3",
            "Major Havoc - Return to Vaxx"});
            this.comboBoxGameDriver.Location = new System.Drawing.Point(101, 24);
            this.comboBoxGameDriver.Name = "comboBoxGameDriver";
            this.comboBoxGameDriver.Size = new System.Drawing.Size(270, 21);
            this.comboBoxGameDriver.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Game Driver:";
            // 
            // DialogLoadROM
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(411, 133);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxGameDriver);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonBrowseFolder);
            this.Controls.Add(this.textBoxROMPath);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogLoadROM";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Load Mazes from ROM Images";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxROMPath;
        private System.Windows.Forms.Button buttonBrowseFolder;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ComboBox comboBoxGameDriver;
        private System.Windows.Forms.Label label2;
    }
}