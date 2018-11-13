namespace mhedit
{
    partial class Mainform
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Mainform));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonNewCollection = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonOpenCollection = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSaveCollection = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonNewMaze = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonOpenMaze = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonOpenMazeMHP = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSaveMaze = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSaveMazeMHP = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonGrid = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonZoomIn = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonZoomOut = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonMAME = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonHome = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAbout = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonLoadFromROM = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonConfiguration = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButtonContestUpload = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.treeView = new System.Windows.Forms.TreeView();
            this.contextMenuStripTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveMazeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItemSave = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItemClose = new System.Windows.Forms.ToolStripMenuItem();
            this.imageListTree = new System.Windows.Forms.ImageList(this.components);
            this.splitterLeft = new System.Windows.Forms.Splitter();
            this.panelUnderneath = new System.Windows.Forms.Panel();
            this.panelContent = new System.Windows.Forms.Panel();
            this.splitterRight = new System.Windows.Forms.Splitter();
            this.panelRight = new System.Windows.Forms.Panel();
            this.splitContainerRight = new System.Windows.Forms.SplitContainer();
            this.toolBox = new Silver.UI.ToolBox();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.comboBoxMazeObjects = new System.Windows.Forms.ComboBox();
            this.splitterUpDown = new System.Windows.Forms.Splitter();
            this.timerMain = new System.Windows.Forms.Timer(this.components);
            this.statusStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.contextMenuStripTree.SuspendLayout();
            this.panelUnderneath.SuspendLayout();
            this.panelRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerRight)).BeginInit();
            this.splitContainerRight.Panel1.SuspendLayout();
            this.splitContainerRight.Panel2.SuspendLayout();
            this.splitContainerRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 688);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1033, 22);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonNewCollection,
            this.toolStripButtonOpenCollection,
            this.toolStripButtonSaveCollection,
            this.toolStripSeparator1,
            this.toolStripButtonNewMaze,
            this.toolStripButtonOpenMaze,
            this.toolStripButtonOpenMazeMHP,
            this.toolStripButtonSaveMaze,
            this.toolStripButtonSaveMazeMHP,
            this.toolStripSeparator2,
            this.toolStripButtonGrid,
            this.toolStripButtonZoomIn,
            this.toolStripButtonZoomOut,
            this.toolStripButtonMAME,
            this.toolStripSeparator3,
            this.toolStripButtonHome,
            this.toolStripButtonAbout,
            this.toolStripButtonLoadFromROM,
            this.toolStripSeparator4,
            this.toolStripButtonConfiguration,
            this.toolStripSeparator5,
            this.toolStripLabel1,
            this.toolStripButtonContestUpload,
            this.toolStripSeparator6});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(1033, 25);
            this.toolStrip.TabIndex = 2;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStripButtonNewCollection
            // 
            this.toolStripButtonNewCollection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonNewCollection.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonNewCollection.Image")));
            this.toolStripButtonNewCollection.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.toolStripButtonNewCollection.Name = "toolStripButtonNewCollection";
            this.toolStripButtonNewCollection.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonNewCollection.Text = "toolStripButton1";
            this.toolStripButtonNewCollection.ToolTipText = "Create a new maze collection";
            this.toolStripButtonNewCollection.Click += new System.EventHandler(this.toolStripButtonNewCollection_Click);
            // 
            // toolStripButtonOpenCollection
            // 
            this.toolStripButtonOpenCollection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonOpenCollection.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonOpenCollection.Image")));
            this.toolStripButtonOpenCollection.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.toolStripButtonOpenCollection.Name = "toolStripButtonOpenCollection";
            this.toolStripButtonOpenCollection.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonOpenCollection.Text = "toolStripButton1";
            this.toolStripButtonOpenCollection.ToolTipText = "Open an existing maze collection";
            this.toolStripButtonOpenCollection.Click += new System.EventHandler(this.toolStripButtonOpenCollection_Click);
            // 
            // toolStripButtonSaveCollection
            // 
            this.toolStripButtonSaveCollection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSaveCollection.Enabled = false;
            this.toolStripButtonSaveCollection.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSaveCollection.Image")));
            this.toolStripButtonSaveCollection.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.toolStripButtonSaveCollection.Name = "toolStripButtonSaveCollection";
            this.toolStripButtonSaveCollection.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSaveCollection.Text = "toolStripButton2";
            this.toolStripButtonSaveCollection.ToolTipText = "Save a maze collection";
            this.toolStripButtonSaveCollection.Click += new System.EventHandler(this.toolStripButtonSaveCollection_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonNewMaze
            // 
            this.toolStripButtonNewMaze.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonNewMaze.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonNewMaze.Image")));
            this.toolStripButtonNewMaze.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.toolStripButtonNewMaze.Name = "toolStripButtonNewMaze";
            this.toolStripButtonNewMaze.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonNewMaze.Text = "toolStripButton1";
            this.toolStripButtonNewMaze.ToolTipText = "Create a new individual maze";
            this.toolStripButtonNewMaze.Click += new System.EventHandler(this.toolStripButtonNewMaze_Click);
            // 
            // toolStripButtonOpenMaze
            // 
            this.toolStripButtonOpenMaze.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonOpenMaze.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonOpenMaze.Image")));
            this.toolStripButtonOpenMaze.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.toolStripButtonOpenMaze.Name = "toolStripButtonOpenMaze";
            this.toolStripButtonOpenMaze.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonOpenMaze.Text = "toolStripButton1";
            this.toolStripButtonOpenMaze.ToolTipText = "Open an individual maze";
            this.toolStripButtonOpenMaze.Click += new System.EventHandler(this.toolStripButtonOpenMaze_Click);
            // 
            // toolStripButtonOpenMazeMHP
            // 
            this.toolStripButtonOpenMazeMHP.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonOpenMazeMHP.Enabled = false;
            this.toolStripButtonOpenMazeMHP.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonOpenMazeMHP.Image")));
            this.toolStripButtonOpenMazeMHP.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOpenMazeMHP.Name = "toolStripButtonOpenMazeMHP";
            this.toolStripButtonOpenMazeMHP.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonOpenMazeMHP.Text = "toolStripButton2";
            // 
            // toolStripButtonSaveMaze
            // 
            this.toolStripButtonSaveMaze.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSaveMaze.Enabled = false;
            this.toolStripButtonSaveMaze.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripButtonSaveMaze.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSaveMaze.Image")));
            this.toolStripButtonSaveMaze.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.toolStripButtonSaveMaze.Name = "toolStripButtonSaveMaze";
            this.toolStripButtonSaveMaze.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSaveMaze.Text = "toolStripButton2";
            this.toolStripButtonSaveMaze.ToolTipText = "Save an individual maze";
            this.toolStripButtonSaveMaze.Click += new System.EventHandler(this.toolStripButtonSaveMaze_Click);
            // 
            // toolStripButtonSaveMazeMHP
            // 
            this.toolStripButtonSaveMazeMHP.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSaveMazeMHP.Enabled = false;
            this.toolStripButtonSaveMazeMHP.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSaveMazeMHP.Image")));
            this.toolStripButtonSaveMazeMHP.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSaveMazeMHP.Name = "toolStripButtonSaveMazeMHP";
            this.toolStripButtonSaveMazeMHP.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSaveMazeMHP.Text = "toolStripButton3";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonGrid
            // 
            this.toolStripButtonGrid.Checked = true;
            this.toolStripButtonGrid.CheckOnClick = true;
            this.toolStripButtonGrid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButtonGrid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonGrid.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonGrid.Image")));
            this.toolStripButtonGrid.ImageTransparentColor = System.Drawing.Color.Black;
            this.toolStripButtonGrid.Name = "toolStripButtonGrid";
            this.toolStripButtonGrid.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonGrid.Text = "toolStripButton3";
            this.toolStripButtonGrid.ToolTipText = "Grid Lines On/Off";
            this.toolStripButtonGrid.Click += new System.EventHandler(this.toolStripButtonGrid_Click);
            // 
            // toolStripButtonZoomIn
            // 
            this.toolStripButtonZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonZoomIn.Enabled = false;
            this.toolStripButtonZoomIn.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonZoomIn.Image")));
            this.toolStripButtonZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonZoomIn.Name = "toolStripButtonZoomIn";
            this.toolStripButtonZoomIn.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonZoomIn.Text = "toolStripButton1";
            this.toolStripButtonZoomIn.ToolTipText = "Zoom In";
            this.toolStripButtonZoomIn.Click += new System.EventHandler(this.toolStripButtonZoomIn_Click);
            // 
            // toolStripButtonZoomOut
            // 
            this.toolStripButtonZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonZoomOut.Enabled = false;
            this.toolStripButtonZoomOut.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonZoomOut.Image")));
            this.toolStripButtonZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonZoomOut.Name = "toolStripButtonZoomOut";
            this.toolStripButtonZoomOut.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonZoomOut.Text = "toolStripButton2";
            this.toolStripButtonZoomOut.ToolTipText = "Zoom Out";
            this.toolStripButtonZoomOut.Click += new System.EventHandler(this.toolStripButtonZoomOut_Click);
            // 
            // toolStripButtonMAME
            // 
            this.toolStripButtonMAME.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonMAME.Enabled = false;
            this.toolStripButtonMAME.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonMAME.Image")));
            this.toolStripButtonMAME.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonMAME.Name = "toolStripButtonMAME";
            this.toolStripButtonMAME.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonMAME.Text = "Run Maze in MAME";
            this.toolStripButtonMAME.ToolTipText = "Run Maze in MAME - Hint, click on the maze in the tree first!";
            this.toolStripButtonMAME.Click += new System.EventHandler(this.toolStripButtonAnimate_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonHome
            // 
            this.toolStripButtonHome.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonHome.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonHome.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonHome.Image")));
            this.toolStripButtonHome.ImageTransparentColor = System.Drawing.Color.Black;
            this.toolStripButtonHome.Name = "toolStripButtonHome";
            this.toolStripButtonHome.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonHome.Text = "toolStripButton";
            this.toolStripButtonHome.ToolTipText = "Major Havoc Level Editor Homepage";
            this.toolStripButtonHome.Click += new System.EventHandler(this.toolStripButtonHome_Click);
            // 
            // toolStripButtonAbout
            // 
            this.toolStripButtonAbout.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonAbout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAbout.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAbout.Image")));
            this.toolStripButtonAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAbout.Name = "toolStripButtonAbout";
            this.toolStripButtonAbout.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAbout.Text = "toolStripButton1";
            this.toolStripButtonAbout.ToolTipText = "About this program";
            this.toolStripButtonAbout.Click += new System.EventHandler(this.toolStripButtonAbout_Click);
            // 
            // toolStripButtonLoadFromROM
            // 
            this.toolStripButtonLoadFromROM.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonLoadFromROM.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonLoadFromROM.Image")));
            this.toolStripButtonLoadFromROM.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonLoadFromROM.Name = "toolStripButtonLoadFromROM";
            this.toolStripButtonLoadFromROM.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonLoadFromROM.Text = "Load Mazes from ROMs";
            this.toolStripButtonLoadFromROM.Click += new System.EventHandler(this.toolStripButtonLoadFromROM_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonConfiguration
            // 
            this.toolStripButtonConfiguration.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonConfiguration.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonConfiguration.Image")));
            this.toolStripButtonConfiguration.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonConfiguration.Name = "toolStripButtonConfiguration";
            this.toolStripButtonConfiguration.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonConfiguration.Text = "toolStripButton1";
            this.toolStripButtonConfiguration.ToolTipText = "Configuration";
            this.toolStripButtonConfiguration.Click += new System.EventHandler(this.toolStripButtonConfiguration_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(118, 22);
            this.toolStripLabel1.Text = "Major Havoc Project:";
            // 
            // toolStripButtonContestUpload
            // 
            this.toolStripButtonContestUpload.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonContestUpload.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonContestUpload.Image")));
            this.toolStripButtonContestUpload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonContestUpload.Name = "toolStripButtonContestUpload";
            this.toolStripButtonContestUpload.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonContestUpload.Text = "Upload Maze to Major Havoc Project";
            this.toolStripButtonContestUpload.Click += new System.EventHandler(this.toolStripButtonContestUpload_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            // 
            // treeView
            // 
            this.treeView.AllowDrop = true;
            this.treeView.ContextMenuStrip = this.contextMenuStripTree;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView.FullRowSelect = true;
            this.treeView.HideSelection = false;
            this.treeView.ImageIndex = 0;
            this.treeView.ImageList = this.imageListTree;
            this.treeView.LabelEdit = true;
            this.treeView.Location = new System.Drawing.Point(0, 25);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = 0;
            this.treeView.Size = new System.Drawing.Size(145, 663);
            this.treeView.TabIndex = 2;
            this.treeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView_AfterLabelEdit);
            this.treeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView_ItemDrag);
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            this.treeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseDoubleClick);
            this.treeView.Click += new System.EventHandler(this.treeView_Click);
            this.treeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView_DragDrop);
            this.treeView.DragOver += new System.Windows.Forms.DragEventHandler(this.treeView_DragOver);
            // 
            // contextMenuStripTree
            // 
            this.contextMenuStripTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveMazeToolStripMenuItem,
            this.saveToolStripMenuItemSave,
            this.closeToolStripMenuItemClose});
            this.contextMenuStripTree.Name = "contextMenuStripTree";
            this.contextMenuStripTree.Size = new System.Drawing.Size(130, 70);
            this.contextMenuStripTree.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripTree_Opening);
            // 
            // saveMazeToolStripMenuItem
            // 
            this.saveMazeToolStripMenuItem.Name = "saveMazeToolStripMenuItem";
            this.saveMazeToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.saveMazeToolStripMenuItem.Text = "Save Maze";
            this.saveMazeToolStripMenuItem.Click += new System.EventHandler(this.saveMazeToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItemSave
            // 
            this.saveToolStripMenuItemSave.Name = "saveToolStripMenuItemSave";
            this.saveToolStripMenuItemSave.Size = new System.Drawing.Size(129, 22);
            this.saveToolStripMenuItemSave.Text = "Save";
            this.saveToolStripMenuItemSave.Click += new System.EventHandler(this.saveToolStripMenuItemSave_Click);
            // 
            // closeToolStripMenuItemClose
            // 
            this.closeToolStripMenuItemClose.Name = "closeToolStripMenuItemClose";
            this.closeToolStripMenuItemClose.Size = new System.Drawing.Size(129, 22);
            this.closeToolStripMenuItemClose.Text = "Close";
            this.closeToolStripMenuItemClose.Click += new System.EventHandler(this.closeToolStripMenuItemClose_Click);
            // 
            // imageListTree
            // 
            this.imageListTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTree.ImageStream")));
            this.imageListTree.TransparentColor = System.Drawing.Color.Fuchsia;
            this.imageListTree.Images.SetKeyName(0, "ThumbnailViewHS.bmp");
            this.imageListTree.Images.SetKeyName(1, "maze_a.bmp");
            this.imageListTree.Images.SetKeyName(2, "maze_b.bmp");
            this.imageListTree.Images.SetKeyName(3, "maze_c.bmp");
            this.imageListTree.Images.SetKeyName(4, "maze_d.bmp");
            // 
            // splitterLeft
            // 
            this.splitterLeft.Location = new System.Drawing.Point(145, 25);
            this.splitterLeft.Name = "splitterLeft";
            this.splitterLeft.Size = new System.Drawing.Size(3, 663);
            this.splitterLeft.TabIndex = 4;
            this.splitterLeft.TabStop = false;
            // 
            // panelUnderneath
            // 
            this.panelUnderneath.Controls.Add(this.panelContent);
            this.panelUnderneath.Controls.Add(this.splitterRight);
            this.panelUnderneath.Controls.Add(this.panelRight);
            this.panelUnderneath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelUnderneath.Location = new System.Drawing.Point(148, 25);
            this.panelUnderneath.Name = "panelUnderneath";
            this.panelUnderneath.Size = new System.Drawing.Size(885, 663);
            this.panelUnderneath.TabIndex = 5;
            // 
            // panelContent
            // 
            this.panelContent.AutoScroll = true;
            this.panelContent.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelContent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelContent.CausesValidation = false;
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(0, 0);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(632, 663);
            this.panelContent.TabIndex = 3;
            this.panelContent.Paint += new System.Windows.Forms.PaintEventHandler(this.panelContent_Paint);
            // 
            // splitterRight
            // 
            this.splitterRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitterRight.Location = new System.Drawing.Point(632, 0);
            this.splitterRight.Name = "splitterRight";
            this.splitterRight.Size = new System.Drawing.Size(3, 663);
            this.splitterRight.TabIndex = 1;
            this.splitterRight.TabStop = false;
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.splitContainerRight);
            this.panelRight.Controls.Add(this.splitterUpDown);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(635, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(250, 663);
            this.panelRight.TabIndex = 0;
            // 
            // splitContainerRight
            // 
            this.splitContainerRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerRight.Location = new System.Drawing.Point(0, 0);
            this.splitContainerRight.Name = "splitContainerRight";
            this.splitContainerRight.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerRight.Panel1
            // 
            this.splitContainerRight.Panel1.Controls.Add(this.toolBox);
            this.splitContainerRight.Panel1MinSize = 50;
            // 
            // splitContainerRight.Panel2
            // 
            this.splitContainerRight.Panel2.Controls.Add(this.propertyGrid);
            this.splitContainerRight.Panel2.Controls.Add(this.comboBoxMazeObjects);
            this.splitContainerRight.Panel2MinSize = 50;
            this.splitContainerRight.Size = new System.Drawing.Size(250, 660);
            this.splitContainerRight.SplitterDistance = 400;
            this.splitContainerRight.TabIndex = 4;
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
            this.toolBox.Size = new System.Drawing.Size(250, 400);
            this.toolBox.SmallItemSize = new System.Drawing.Size(32, 32);
            this.toolBox.TabHeight = 18;
            this.toolBox.TabHoverTextColor = System.Drawing.SystemColors.ControlText;
            this.toolBox.TabIndex = 0;
            this.toolBox.TabNormalTextColor = System.Drawing.SystemColors.ControlText;
            this.toolBox.TabSelectedTextColor = System.Drawing.SystemColors.ControlText;
            this.toolBox.TabSpacing = 1;
            this.toolBox.UseItemColorInRename = false;
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.LineColor = System.Drawing.SystemColors.ControlDark;
            this.propertyGrid.Location = new System.Drawing.Point(0, 21);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(250, 235);
            this.propertyGrid.TabIndex = 5;
            // 
            // comboBoxMazeObjects
            // 
            this.comboBoxMazeObjects.Dock = System.Windows.Forms.DockStyle.Top;
            this.comboBoxMazeObjects.FormattingEnabled = true;
            this.comboBoxMazeObjects.Location = new System.Drawing.Point(0, 0);
            this.comboBoxMazeObjects.Name = "comboBoxMazeObjects";
            this.comboBoxMazeObjects.Size = new System.Drawing.Size(250, 21);
            this.comboBoxMazeObjects.TabIndex = 4;
            // 
            // splitterUpDown
            // 
            this.splitterUpDown.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitterUpDown.Location = new System.Drawing.Point(0, 660);
            this.splitterUpDown.Name = "splitterUpDown";
            this.splitterUpDown.Size = new System.Drawing.Size(250, 3);
            this.splitterUpDown.TabIndex = 1;
            this.splitterUpDown.TabStop = false;
            // 
            // timerMain
            // 
            this.timerMain.Interval = 1000;
            this.timerMain.Tick += new System.EventHandler(this.timerMain_Tick);
            // 
            // Mainform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1033, 710);
            this.Controls.Add(this.panelUnderneath);
            this.Controls.Add(this.splitterLeft);
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.statusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Mainform";
            this.Text = "Major Havoc Level Editor";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Mainform_FormClosing);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.contextMenuStripTree.ResumeLayout(false);
            this.panelUnderneath.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.splitContainerRight.Panel1.ResumeLayout(false);
            this.splitContainerRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerRight)).EndInit();
            this.splitContainerRight.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.Splitter splitterLeft;
        private System.Windows.Forms.Panel panelUnderneath;
        private System.Windows.Forms.Splitter splitterRight;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Splitter splitterUpDown;
        private System.Windows.Forms.ToolStripButton toolStripButtonOpenCollection;
        private System.Windows.Forms.ToolStripButton toolStripButtonSaveCollection;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.ToolStripButton toolStripButtonNewCollection;
        private System.Windows.Forms.ToolStripButton toolStripButtonNewMaze;
        private System.Windows.Forms.ToolStripButton toolStripButtonOpenMaze;
        private System.Windows.Forms.ToolStripButton toolStripButtonSaveMaze;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonGrid;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButtonHome;
        private System.Windows.Forms.ToolStripButton toolStripButtonAbout;
        private System.Windows.Forms.ToolStripButton toolStripButtonZoomIn;
        private System.Windows.Forms.ToolStripButton toolStripButtonZoomOut;
        private System.Windows.Forms.ImageList imageListTree;
        private System.Windows.Forms.ToolStripButton toolStripButtonConfiguration;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripTree;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItemSave;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItemClose;
        private System.Windows.Forms.ToolStripMenuItem saveMazeToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButtonOpenMazeMHP;
        private System.Windows.Forms.ToolStripButton toolStripButtonSaveMazeMHP;
        private System.Windows.Forms.Timer timerMain;
        private System.Windows.Forms.ToolStripButton toolStripButtonMAME;
        private System.Windows.Forms.SplitContainer splitContainerRight;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.ComboBox comboBoxMazeObjects;
        private Silver.UI.ToolBox toolBox;
        private System.Windows.Forms.ToolStripButton toolStripButtonLoadFromROM;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton toolStripButtonContestUpload;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
    }
}

