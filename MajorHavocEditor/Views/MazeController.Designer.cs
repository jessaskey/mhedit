
namespace MajorHavocEditor.Views
{
    partial class MazeController
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

#region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MazeController));
            this.mazeControllerContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mazeControllerContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Enabled = false;
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.selectAllToolStripMenuItem.Text = "Select All";
            this.selectAllToolStripMenuItem.ToolTipText = "Select All Maze Objects";
            this.selectAllToolStripMenuItem.DropDownOpening += new System.EventHandler(this.selectAllToolStripMenuItem_DropDownOpening);
            // 
            // MazeController
            // 
            this.AllowDrop = true;
            this.Size = new System.Drawing.Size(447, 152);
            this.TabStop = true;
            this.mazeControllerContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip mazeControllerContextMenu;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
    }
}
