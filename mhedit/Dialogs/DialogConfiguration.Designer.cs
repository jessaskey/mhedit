namespace mhedit
{
    partial class DialogConfiguration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogConfiguration));
            this.buttonSave = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageOptions = new System.Windows.Forms.TabPage();
            this.checkBoxShowGridCoordinateReferences = new System.Windows.Forms.CheckBox();
            this.tabPageMAME = new System.Windows.Forms.TabPage();
            this.checkBoxMAMEWindow = new System.Windows.Forms.CheckBox();
            this.checkBoxDebug = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxMameDriver = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonBrowseMameExecutable = new System.Windows.Forms.Button();
            this.textBoxMameExecutable = new System.Windows.Forms.TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tabPageLocations = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxTemplatesLocation = new System.Windows.Forms.TextBox();
            this.buttonBrowseTemplatesFolder = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPageOptions.SuspendLayout();
            this.tabPageMAME.SuspendLayout();
            this.tabPageLocations.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(375, 188);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 7;
            this.buttonSave.Text = "OK";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageOptions);
            this.tabControl1.Controls.Add(this.tabPageMAME);
            this.tabControl1.Controls.Add(this.tabPageLocations);
            this.tabControl1.Location = new System.Drawing.Point(5, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(544, 158);
            this.tabControl1.TabIndex = 13;
            // 
            // tabPageOptions
            // 
            this.tabPageOptions.Controls.Add(this.checkBoxShowGridCoordinateReferences);
            this.tabPageOptions.Location = new System.Drawing.Point(4, 22);
            this.tabPageOptions.Name = "tabPageOptions";
            this.tabPageOptions.Size = new System.Drawing.Size(536, 132);
            this.tabPageOptions.TabIndex = 2;
            this.tabPageOptions.Text = "Options";
            this.tabPageOptions.UseVisualStyleBackColor = true;
            // 
            // checkBoxShowGridCoordinateReferences
            // 
            this.checkBoxShowGridCoordinateReferences.AutoSize = true;
            this.checkBoxShowGridCoordinateReferences.Location = new System.Drawing.Point(22, 17);
            this.checkBoxShowGridCoordinateReferences.Name = "checkBoxShowGridCoordinateReferences";
            this.checkBoxShowGridCoordinateReferences.Size = new System.Drawing.Size(182, 17);
            this.checkBoxShowGridCoordinateReferences.TabIndex = 0;
            this.checkBoxShowGridCoordinateReferences.Text = "Show Grid Coordinate Reference";
            this.checkBoxShowGridCoordinateReferences.UseVisualStyleBackColor = true;
            // 
            // tabPageMAME
            // 
            this.tabPageMAME.Controls.Add(this.checkBoxMAMEWindow);
            this.tabPageMAME.Controls.Add(this.checkBoxDebug);
            this.tabPageMAME.Controls.Add(this.label5);
            this.tabPageMAME.Controls.Add(this.textBoxMameDriver);
            this.tabPageMAME.Controls.Add(this.label6);
            this.tabPageMAME.Controls.Add(this.buttonBrowseMameExecutable);
            this.tabPageMAME.Controls.Add(this.textBoxMameExecutable);
            this.tabPageMAME.Location = new System.Drawing.Point(4, 22);
            this.tabPageMAME.Name = "tabPageMAME";
            this.tabPageMAME.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMAME.Size = new System.Drawing.Size(536, 132);
            this.tabPageMAME.TabIndex = 0;
            this.tabPageMAME.Text = "MAME";
            this.tabPageMAME.UseVisualStyleBackColor = true;
            // 
            // checkBoxMAMEWindow
            // 
            this.checkBoxMAMEWindow.AutoSize = true;
            this.checkBoxMAMEWindow.Location = new System.Drawing.Point(109, 97);
            this.checkBoxMAMEWindow.Name = "checkBoxMAMEWindow";
            this.checkBoxMAMEWindow.Size = new System.Drawing.Size(199, 17);
            this.checkBoxMAMEWindow.TabIndex = 19;
            this.checkBoxMAMEWindow.Text = "Run MAME with \'-window\' parameter";
            this.checkBoxMAMEWindow.UseVisualStyleBackColor = true;
            // 
            // checkBoxDebug
            // 
            this.checkBoxDebug.AutoSize = true;
            this.checkBoxDebug.Location = new System.Drawing.Point(109, 74);
            this.checkBoxDebug.Name = "checkBoxDebug";
            this.checkBoxDebug.Size = new System.Drawing.Size(193, 17);
            this.checkBoxDebug.TabIndex = 18;
            this.checkBoxDebug.Text = "Run MAME with \'-debug\' parameter";
            this.checkBoxDebug.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 51);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "MAME Driver:";
            // 
            // textBoxMameDriver
            // 
            this.textBoxMameDriver.Location = new System.Drawing.Point(109, 48);
            this.textBoxMameDriver.Name = "textBoxMameDriver";
            this.textBoxMameDriver.Size = new System.Drawing.Size(93, 20);
            this.textBoxMameDriver.TabIndex = 16;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 25);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(98, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "MAME Executable:";
            // 
            // buttonBrowseMameExecutable
            // 
            this.buttonBrowseMameExecutable.Image = global::mhedit.Properties.Resources.OpenSelectedItemHS;
            this.buttonBrowseMameExecutable.Location = new System.Drawing.Point(492, 19);
            this.buttonBrowseMameExecutable.Name = "buttonBrowseMameExecutable";
            this.buttonBrowseMameExecutable.Size = new System.Drawing.Size(29, 23);
            this.buttonBrowseMameExecutable.TabIndex = 14;
            this.buttonBrowseMameExecutable.Text = "button1";
            this.buttonBrowseMameExecutable.UseVisualStyleBackColor = true;
            this.buttonBrowseMameExecutable.Click += new System.EventHandler(this.buttonBrowseMameExecutable_Click);
            // 
            // textBoxMameExecutable
            // 
            this.textBoxMameExecutable.Location = new System.Drawing.Point(109, 22);
            this.textBoxMameExecutable.Name = "textBoxMameExecutable";
            this.textBoxMameExecutable.Size = new System.Drawing.Size(377, 20);
            this.textBoxMameExecutable.TabIndex = 13;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(469, 188);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 14;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // tabPageLocations
            // 
            this.tabPageLocations.Controls.Add(this.buttonBrowseTemplatesFolder);
            this.tabPageLocations.Controls.Add(this.textBoxTemplatesLocation);
            this.tabPageLocations.Controls.Add(this.label1);
            this.tabPageLocations.Location = new System.Drawing.Point(4, 22);
            this.tabPageLocations.Name = "tabPageLocations";
            this.tabPageLocations.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLocations.Size = new System.Drawing.Size(536, 132);
            this.tabPageLocations.TabIndex = 3;
            this.tabPageLocations.Text = "Locations";
            this.tabPageLocations.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Templates Location";
            // 
            // textBoxTemplatesLocation
            // 
            this.textBoxTemplatesLocation.Location = new System.Drawing.Point(19, 19);
            this.textBoxTemplatesLocation.Name = "textBoxTemplatesLocation";
            this.textBoxTemplatesLocation.Size = new System.Drawing.Size(472, 20);
            this.textBoxTemplatesLocation.TabIndex = 1;
            // 
            // buttonBrowseTemplatesFolder
            // 
            this.buttonBrowseTemplatesFolder.Location = new System.Drawing.Point(501, 19);
            this.buttonBrowseTemplatesFolder.Name = "buttonBrowseTemplatesFolder";
            this.buttonBrowseTemplatesFolder.Size = new System.Drawing.Size(29, 23);
            this.buttonBrowseTemplatesFolder.TabIndex = 15;
            this.buttonBrowseTemplatesFolder.Text = "...";
            this.buttonBrowseTemplatesFolder.UseVisualStyleBackColor = true;
            this.buttonBrowseTemplatesFolder.Click += new System.EventHandler(this.buttonBrowseTemplatesFolder_Click);
            // 
            // DialogConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(556, 224);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogConfiguration";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuration";
            this.tabControl1.ResumeLayout(false);
            this.tabPageOptions.ResumeLayout(false);
            this.tabPageOptions.PerformLayout();
            this.tabPageMAME.ResumeLayout(false);
            this.tabPageMAME.PerformLayout();
            this.tabPageLocations.ResumeLayout(false);
            this.tabPageLocations.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageMAME;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxMameDriver;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonBrowseMameExecutable;
        private System.Windows.Forms.TextBox textBoxMameExecutable;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.CheckBox checkBoxDebug;
        private System.Windows.Forms.TabPage tabPageOptions;
        private System.Windows.Forms.CheckBox checkBoxShowGridCoordinateReferences;
        private System.Windows.Forms.CheckBox checkBoxMAMEWindow;
        private System.Windows.Forms.TabPage tabPageLocations;
        private System.Windows.Forms.TextBox textBoxTemplatesLocation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonBrowseTemplatesFolder;
    }
}