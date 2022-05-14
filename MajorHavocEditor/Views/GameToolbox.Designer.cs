
namespace MajorHavocEditor.Views
{
    partial class GameToolbox
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
            this.components = new System.ComponentModel.Container();
            this.toolBox = new Silver.UI.ToolBox();
            //this.kryptonPanel = new Krypton.Toolkit.KryptonPanel();
            //((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).BeginInit();
            //this.kryptonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolBox
            // 
            this.toolBox.AllowDrop = true;
            this.toolBox.AllowSwappingByDragDrop = true;
            this.toolBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolBox.InitialScrollDelay = 500;
            this.toolBox.ItemBackgroundColor = System.Drawing.Color.Empty;
            this.toolBox.ItemBorderColor = System.Drawing.Color.Empty;
            this.toolBox.ItemHeight = 20;
            this.toolBox.ItemHoverColor = System.Drawing.SystemColors.Control;
            this.toolBox.ItemHoverTextColor = System.Drawing.SystemColors.ControlText;
            this.toolBox.ItemNormalColor = System.Drawing.SystemColors.Control;
            this.toolBox.ItemNormalTextColor = System.Drawing.SystemColors.ControlText;
            this.toolBox.ItemSelectedColor = System.Drawing.SystemColors.Control;
            this.toolBox.ItemSelectedTextColor = System.Drawing.SystemColors.ControlText;
            this.toolBox.ItemSpacing = 2;
            this.toolBox.LargeItemSize = new System.Drawing.Size(64, 64);
            this.toolBox.LayoutDelay = 10;
            this.toolBox.Location = new System.Drawing.Point(0, 0);
            this.toolBox.Name = "toolBox";
            this.toolBox.ScrollDelay = 60;
            this.toolBox.SelectAllTextWhileRenaming = true;
            this.toolBox.SelectedTabIndex = -1;
            this.toolBox.ShowOnlyOneItemPerRow = false;
            this.toolBox.Size = new System.Drawing.Size(211, 415);
            this.toolBox.SmallItemSize = new System.Drawing.Size(32, 32);
            this.toolBox.TabHeight = 18;
            this.toolBox.TabHoverTextColor = System.Drawing.SystemColors.ControlText;
            this.toolBox.TabIndex = 0;
            this.toolBox.TabNormalTextColor = System.Drawing.SystemColors.ControlText;
            this.toolBox.TabSelectedTextColor = System.Drawing.SystemColors.ControlText;
            this.toolBox.TabSpacing = 1;
            this.toolBox.UseItemColorInRename = false;
            // 
            // kryptonPanel
            // 
            //this.kryptonPanel.Controls.Add(this.toolBox);
            //this.kryptonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.kryptonPanel.Location = new System.Drawing.Point(0, 0);
            //this.kryptonPanel.Name = "kryptonPanel";
            //this.kryptonPanel.Padding = new System.Windows.Forms.Padding(5);
            //this.kryptonPanel.PanelBackStyle = Krypton.Toolkit.PaletteBackStyle.ControlClient;
            //this.kryptonPanel.Size = new System.Drawing.Size(152, 111);
            //this.kryptonPanel.TabIndex = 1;
            // 
            // GameToolbox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.toolBox);
            //this.Controls.Add(this.kryptonPanel);
            this.Name = "GameToolbox";
            this.Size = new System.Drawing.Size(211, 415);
            //((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).EndInit();
            //this.kryptonPanel.ResumeLayout(false);
            //this.kryptonPanel.PerformLayout();
            this.ResumeLayout(false);

        }

#endregion

        //private Krypton.Toolkit.KryptonPanel kryptonPanel;
        private Silver.UI.ToolBox toolBox;
    }
}
