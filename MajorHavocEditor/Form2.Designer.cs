﻿
namespace MajorHavocEditor
{
    partial class Form2
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
            this.kryptonPanel = new Krypton.Toolkit.KryptonPanel();
            this.kryptonDockableWorkspace = new Krypton.Docking.KryptonDockableWorkspace();
            this.kryptonDockingManager = new Krypton.Docking.KryptonDockingManager();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).BeginInit();
            this.kryptonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonDockableWorkspace)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel
            // 
            this.kryptonPanel.Controls.Add(this.kryptonDockableWorkspace);
            this.kryptonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel.Name = "kryptonPanel";
            this.kryptonPanel.Padding = new System.Windows.Forms.Padding(5);
            this.kryptonPanel.Size = new System.Drawing.Size(800, 450);
            this.kryptonPanel.TabIndex = 1;
            // 
            // kryptonDockableWorkspace
            // 
            this.kryptonDockableWorkspace.ActivePage = null;
            this.kryptonDockableWorkspace.AutoHiddenHost = false;
            this.kryptonDockableWorkspace.CompactFlags = ((Krypton.Workspace.CompactFlags)(((Krypton.Workspace.CompactFlags.RemoveEmptyCells | Krypton.Workspace.CompactFlags.RemoveEmptySequences) 
            | Krypton.Workspace.CompactFlags.PromoteLeafs)));
            this.kryptonDockableWorkspace.ContainerBackStyle = Krypton.Toolkit.PaletteBackStyle.PanelClient;
            this.kryptonDockableWorkspace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonDockableWorkspace.Location = new System.Drawing.Point(5, 5);
            this.kryptonDockableWorkspace.Name = "kryptonDockableWorkspace";
            // 
            // 
            // 
            this.kryptonDockableWorkspace.Root.UniqueName = "D51970B3EA2C496AD51970B3EA2C496A";
            this.kryptonDockableWorkspace.Root.WorkspaceControl = this.kryptonDockableWorkspace;
            this.kryptonDockableWorkspace.SeparatorStyle = Krypton.Toolkit.SeparatorStyle.LowProfile;
            this.kryptonDockableWorkspace.ShowMaximizeButton = false;
            this.kryptonDockableWorkspace.Size = new System.Drawing.Size(790, 440);
            this.kryptonDockableWorkspace.SplitterWidth = 5;
            this.kryptonDockableWorkspace.TabIndex = 0;
            this.kryptonDockableWorkspace.TabStop = true;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.kryptonPanel);
            this.KeyPreview = true;
            this.Name = "Form2";
            this.Text = "Major Havoc Level Editor";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).EndInit();
            this.kryptonPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonDockableWorkspace)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Krypton.Toolkit.KryptonPanel kryptonPanel;
        private Krypton.Docking.KryptonDockingManager kryptonDockingManager;
        private Krypton.Docking.KryptonDockableWorkspace kryptonDockableWorkspace;

    }
}