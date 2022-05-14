
namespace MajorHavocEditor.Views
{
    partial class MazeUi
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.kryptonPanel = new Krypton.Toolkit.KryptonPanel();
            //this.kryptonDockableWorkspace = new Krypton.Docking.KryptonDockableWorkspace();
            this.kryptonDockingManager = new Krypton.Docking.KryptonDockingManager();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).BeginInit();
            this.kryptonPanel.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(this.kryptonDockableWorkspace)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel
            // 
            //this.kryptonPanel.Controls.Add(this.kryptonDockableWorkspace);
            this.kryptonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel.Name = "kryptonPanel";
            this.kryptonPanel.Size = new System.Drawing.Size(150, 150);
            this.kryptonPanel.TabIndex = 0;
            // 
            // kryptonDockableWorkspace
            // 
            //this.kryptonDockableWorkspace.ActivePage = null;
            //this.kryptonDockableWorkspace.AutoHiddenHost = false;
            //this.kryptonDockableWorkspace.CompactFlags = ((Krypton.Workspace.CompactFlags)(((Krypton.Workspace.CompactFlags.RemoveEmptyCells | Krypton.Workspace.CompactFlags.RemoveEmptySequences) 
            //| Krypton.Workspace.CompactFlags.PromoteLeafs)));
            //this.kryptonDockableWorkspace.ContainerBackStyle = Krypton.Toolkit.PaletteBackStyle.PanelClient;
            //this.kryptonDockableWorkspace.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.kryptonDockableWorkspace.Location = new System.Drawing.Point(5, 5);
            //this.kryptonDockableWorkspace.Name = "kryptonDockableWorkspace";
            //// 
            //// 
            //// 
            //this.kryptonDockableWorkspace.Root.Orientation = System.Windows.Forms.Orientation.Horizontal;
            //this.kryptonDockableWorkspace.Root.UniqueName = "96116bd642a04c6bb2255bdb8bf68bf1";
            //this.kryptonDockableWorkspace.Root.WorkspaceControl = this.kryptonDockableWorkspace;
            //this.kryptonDockableWorkspace.SeparatorStyle = Krypton.Toolkit.SeparatorStyle.LowProfile;
            //this.kryptonDockableWorkspace.ShowMaximizeButton = false;
            ////this.kryptonDockableWorkspace.Size = new System.Drawing.Size(150, 150);
            //this.kryptonDockableWorkspace.SplitterWidth = 5;
            //this.kryptonDockableWorkspace.TabIndex = 0;
            //this.kryptonDockableWorkspace.TabStop = true;
            // 
            // MazeUi2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonPanel);
            this.Name = "MazeUi2";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).EndInit();
            this.kryptonPanel.ResumeLayout(false);
            //((System.ComponentModel.ISupportInitialize)(this.kryptonDockableWorkspace)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Krypton.Toolkit.KryptonPanel kryptonPanel;
        //private Krypton.Docking.KryptonDockableWorkspace kryptonDockableWorkspace;
        private Krypton.Docking.KryptonDockingManager kryptonDockingManager;
    }
}
