namespace MajorHavocEditor.Views
{
    partial class ValidationResultTab
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( this.components != null ) )
            {
                this.components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ValidationResultTab));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.SortingComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ErrorsButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.WarningsButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.MessagesButton = new System.Windows.Forms.ToolStripButton();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SortingComboBox,
            this.toolStripSeparator1,
            this.ErrorsButton,
            this.toolStripSeparator2,
            this.WarningsButton,
            this.toolStripSeparator3,
            this.MessagesButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Margin = new System.Windows.Forms.Padding(3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(579, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // SortingComboBox
            // 
            this.SortingComboBox.Name = "SortingComboBox";
            this.SortingComboBox.Size = new System.Drawing.Size(150, 25);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // ErrorsButton
            // 
            this.ErrorsButton.Checked = true;
            this.ErrorsButton.CheckOnClick = true;
            this.ErrorsButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ErrorsButton.Image = ((System.Drawing.Image)(resources.GetObject("ErrorsButton.Image")));
            this.ErrorsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ErrorsButton.Name = "ErrorsButton";
            this.ErrorsButton.Size = new System.Drawing.Size(66, 22);
            this.ErrorsButton.Tag = "Errors";
            this.ErrorsButton.Text = "0 Errors";
            this.ErrorsButton.CheckedChanged += new System.EventHandler(this.ErrorsButton_CheckedChanged);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // WarningsButton
            // 
            this.WarningsButton.Checked = true;
            this.WarningsButton.CheckOnClick = true;
            this.WarningsButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.WarningsButton.Image = ((System.Drawing.Image)(resources.GetObject("WarningsButton.Image")));
            this.WarningsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.WarningsButton.Name = "WarningsButton";
            this.WarningsButton.Size = new System.Drawing.Size(86, 22);
            this.WarningsButton.Tag = "Warnings";
            this.WarningsButton.Text = "0 Warnings";
            this.WarningsButton.CheckedChanged += new System.EventHandler(this.WarningsButton_CheckedChanged);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // MessagesButton
            // 
            this.MessagesButton.Checked = true;
            this.MessagesButton.CheckOnClick = true;
            this.MessagesButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MessagesButton.Image = ((System.Drawing.Image)(resources.GetObject("MessagesButton.Image")));
            this.MessagesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MessagesButton.Name = "MessagesButton";
            this.MessagesButton.Size = new System.Drawing.Size(87, 22);
            this.MessagesButton.Tag = "Messages";
            this.MessagesButton.Text = "0 Messages";
            this.MessagesButton.CheckedChanged += new System.EventHandler(this.MessagesButton_CheckedChanged);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.GridColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dataGridView1.Location = new System.Drawing.Point(0, 25);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(579, 212);
            this.dataGridView1.TabIndex = 2;
            // 
            // ValidationWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "ValidationResultTab";
            this.Size = new System.Drawing.Size(579, 237);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripComboBox SortingComboBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton ErrorsButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton WarningsButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton MessagesButton;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}
