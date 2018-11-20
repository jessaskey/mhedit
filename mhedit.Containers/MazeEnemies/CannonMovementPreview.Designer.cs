namespace mhedit.Containers.MazeEnemies
{
    partial class CannonMovementPreview
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CannonMovementPreview));
            this.panelPreview = new System.Windows.Forms.Panel();
            this.timerMain = new System.Windows.Forms.Timer(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonRepeat = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabelSequenceCounter = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelPreview
            // 
            this.panelPreview.BackColor = System.Drawing.Color.Black;
            this.panelPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPreview.Location = new System.Drawing.Point(0, 0);
            this.panelPreview.Name = "panelPreview";
            this.panelPreview.Size = new System.Drawing.Size(636, 119);
            this.panelPreview.TabIndex = 0;
            // 
            // timerMain
            // 
            this.timerMain.Tick += new System.EventHandler(this.timerMain_Tick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonRepeat,
            this.toolStripLabelSequenceCounter});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(636, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonRepeat
            // 
            this.toolStripButtonRepeat.Checked = true;
            this.toolStripButtonRepeat.CheckOnClick = true;
            this.toolStripButtonRepeat.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButtonRepeat.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonRepeat.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRepeat.Image")));
            this.toolStripButtonRepeat.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRepeat.Name = "toolStripButtonRepeat";
            this.toolStripButtonRepeat.Size = new System.Drawing.Size(101, 22);
            this.toolStripButtonRepeat.Text = "Repeat Sequence";
            this.toolStripButtonRepeat.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            // 
            // toolStripLabelSequenceCounter
            // 
            this.toolStripLabelSequenceCounter.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripLabelSequenceCounter.Name = "toolStripLabelSequenceCounter";
            this.toolStripLabelSequenceCounter.Size = new System.Drawing.Size(24, 22);
            this.toolStripLabelSequenceCounter.Text = "1/1";
            // 
            // CannonMovementPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 119);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.panelPreview);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CannonMovementPreview";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cannon Preview";
            this.TopMost = true;
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelPreview;
        private System.Windows.Forms.Timer timerMain;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonRepeat;
        private System.Windows.Forms.ToolStripLabel toolStripLabelSequenceCounter;
    }
}