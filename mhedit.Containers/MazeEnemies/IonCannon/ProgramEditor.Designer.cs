namespace mhedit.Containers.MazeEnemies.IonCannon
{
    partial class CannonProgramEditor
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
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CannonProgramEditor));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonAddMove = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAddAngle = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAddPause = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAddRepeat = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonMoveUp = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonMoveDown = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonPreview = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripComboBoxLoadPreset = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripButtonSaveProgram = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.propertyGridProgram = new System.Windows.Forms.PropertyGrid();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.treeViewProgram = new System.Windows.Forms.TreeView();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonAddMove,
            this.toolStripButtonAddAngle,
            this.toolStripButtonAddPause,
            this.toolStripButtonAddRepeat,
            this.toolStripSeparator1,
            this.toolStripButtonDelete,
            this.toolStripButtonMoveUp,
            this.toolStripButtonMoveDown,
            this.toolStripSeparator2,
            this.toolStripButtonPreview,
            this.toolStripSeparator3,
            this.toolStripComboBoxLoadPreset,
            this.toolStripButtonSaveProgram});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(557, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonAddMove
            // 
            this.toolStripButtonAddMove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAddMove.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAddMove.Image")));
            this.toolStripButtonAddMove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAddMove.Name = "toolStripButtonAddMove";
            this.toolStripButtonAddMove.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAddMove.Text = "toolStripButtonProgram";
            this.toolStripButtonAddMove.ToolTipText = "Add Move";
            this.toolStripButtonAddMove.Click += new System.EventHandler(this.toolStripButtonAddMove_Click);
            // 
            // toolStripButtonAddAngle
            // 
            this.toolStripButtonAddAngle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAddAngle.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAddAngle.Image")));
            this.toolStripButtonAddAngle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAddAngle.Name = "toolStripButtonAddAngle";
            this.toolStripButtonAddAngle.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAddAngle.Text = "toolStripButtonPosition";
            this.toolStripButtonAddAngle.ToolTipText = "Add Gun Orientation/Fire Shot";
            this.toolStripButtonAddAngle.Click += new System.EventHandler(this.toolStripButtonAddAngle_Click);
            // 
            // toolStripButtonAddPause
            // 
            this.toolStripButtonAddPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAddPause.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAddPause.Image")));
            this.toolStripButtonAddPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAddPause.Name = "toolStripButtonAddPause";
            this.toolStripButtonAddPause.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAddPause.Text = "toolStripButton3";
            this.toolStripButtonAddPause.ToolTipText = "Add Pause";
            this.toolStripButtonAddPause.Click += new System.EventHandler(this.toolStripButtonAddPause_Click);
            // 
            // toolStripButtonAddRepeat
            // 
            this.toolStripButtonAddRepeat.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAddRepeat.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAddRepeat.Image")));
            this.toolStripButtonAddRepeat.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAddRepeat.Name = "toolStripButtonAddRepeat";
            this.toolStripButtonAddRepeat.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAddRepeat.Text = "toolStripButton4";
            this.toolStripButtonAddRepeat.ToolTipText = "Add Return To Start";
            this.toolStripButtonAddRepeat.Click += new System.EventHandler(this.toolStripButtonAddRepeat_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonDelete
            // 
            this.toolStripButtonDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDelete.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDelete.Image")));
            this.toolStripButtonDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDelete.Name = "toolStripButtonDelete";
            this.toolStripButtonDelete.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonDelete.Text = "toolStripButton5";
            this.toolStripButtonDelete.ToolTipText = "Delete Selected";
            this.toolStripButtonDelete.Click += new System.EventHandler(this.toolStripButtonDelete_Click);
            // 
            // toolStripButtonMoveUp
            // 
            this.toolStripButtonMoveUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonMoveUp.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonMoveUp.Image")));
            this.toolStripButtonMoveUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonMoveUp.Name = "toolStripButtonMoveUp";
            this.toolStripButtonMoveUp.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonMoveUp.Text = "toolStripButton6";
            this.toolStripButtonMoveUp.ToolTipText = "Move selected item up";
            this.toolStripButtonMoveUp.Click += new System.EventHandler(this.toolStripButtonMoveUp_Click);
            // 
            // toolStripButtonMoveDown
            // 
            this.toolStripButtonMoveDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonMoveDown.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonMoveDown.Image")));
            this.toolStripButtonMoveDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonMoveDown.Name = "toolStripButtonMoveDown";
            this.toolStripButtonMoveDown.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonMoveDown.Text = "toolStripButton7";
            this.toolStripButtonMoveDown.ToolTipText = "Move selected item down";
            this.toolStripButtonMoveDown.Click += new System.EventHandler(this.toolStripButtonMoveDown_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonPreview
            // 
            this.toolStripButtonPreview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPreview.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonPreview.Image")));
            this.toolStripButtonPreview.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPreview.Name = "toolStripButtonPreview";
            this.toolStripButtonPreview.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPreview.Text = "Preview";
            this.toolStripButtonPreview.ToolTipText = "Preview";
            this.toolStripButtonPreview.Click += new System.EventHandler(this.toolStripButtonPreview_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripComboBoxLoadPreset
            // 
            this.toolStripComboBoxLoadPreset.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripComboBoxLoadPreset.Name = "toolStripComboBoxLoadPreset";
            this.toolStripComboBoxLoadPreset.Size = new System.Drawing.Size(175, 25);
            this.toolStripComboBoxLoadPreset.Text = "<Load Preset Program>";
            this.toolStripComboBoxLoadPreset.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxLoadPreset_SelectedIndexChanged);
            // 
            // toolStripButtonSaveProgram
            // 
            this.toolStripButtonSaveProgram.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonSaveProgram.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSaveProgram.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSaveProgram.Image")));
            this.toolStripButtonSaveProgram.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSaveProgram.Name = "toolStripButtonSaveProgram";
            this.toolStripButtonSaveProgram.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSaveProgram.Text = "Save Program";
            this.toolStripButtonSaveProgram.ToolTipText = "Save Program";
            this.toolStripButtonSaveProgram.Click += new System.EventHandler(this.toolStripButtonSaveProgram_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeViewProgram);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertyGridProgram);
            this.splitContainer1.Size = new System.Drawing.Size(557, 382);
            this.splitContainer1.SplitterDistance = 297;
            this.splitContainer1.TabIndex = 2;
            // 
            // propertyGridProgram
            // 
            this.propertyGridProgram.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridProgram.Location = new System.Drawing.Point(0, 0);
            this.propertyGridProgram.Name = "propertyGridProgram";
            this.propertyGridProgram.Size = new System.Drawing.Size(256, 382);
            this.propertyGridProgram.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonOK);
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 407);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(557, 39);
            this.panel1.TabIndex = 3;
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(472, 8);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "Ok";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(375, 8);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 0;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // treeViewProgram
            // 
            this.treeViewProgram.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewProgram.Location = new System.Drawing.Point(0, 0);
            this.treeViewProgram.Name = "treeViewProgram";
            this.treeViewProgram.Size = new System.Drawing.Size(297, 382);
            this.treeViewProgram.TabIndex = 0;
            this.treeViewProgram.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewProgram_AfterSelect);
            // 
            // CannonProgramEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(557, 446);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "CannonProgramEditor";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ion Cannon Program Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CannonProgramEditor_FormClosing);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddMove;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddAngle;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddPause;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddRepeat;
        private System.Windows.Forms.ToolStripButton toolStripButtonDelete;
        private System.Windows.Forms.ToolStripButton toolStripButtonMoveUp;
        private System.Windows.Forms.ToolStripButton toolStripButtonMoveDown;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PropertyGrid propertyGridProgram;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonPreview;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxLoadPreset;
        private System.Windows.Forms.ToolStripButton toolStripButtonSaveProgram;
        private System.Windows.Forms.TreeView treeViewProgram;
    }
}