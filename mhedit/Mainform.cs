using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
//using System.Runtime.Serialization.Formatters.Binary;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Zip;

using mhedit.Containers;
using mhedit.Containers.MazeEnemies;
using mhedit.Containers.MazeEnemies.IonCannon;
using mhedit.Containers.MazeObjects;
using mhedit.Controllers;

namespace mhedit
{
    /// <summary>
    /// The main application form.
    /// </summary>
    public partial class Mainform : Form
    {
        /// <summary>
        /// This is the default caption for all message boxes in the application
        /// </summary>
        public const string MESSAGEBOX_CAPTION = "The Homeworld Is Near";
        private MazeController _currentMazeController = null;
        private MazeCollectionController _currentMazeCollectionController = null;
        private TreeNode _draggedNode;

        #region Constructor

        /// <summary>
        /// Our default constructor for the application mainform
        /// </summary>
        public Mainform()
        {
            InitializeComponent();

            treeView.ContextMenuStrip = contextMenuStripTree;

            string versionString = String.Empty;
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                Version version = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
                versionString = version.ToString();
            }

            this.Text = this.Text + " - " + versionString + " BETA VERSION";
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);

            LoadToolbox();

            //look at command line args..
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                string file = args[1];
                switch (Path.GetExtension(file))
                {
                    case ".mhz":
                        //open a maze 
                        OpenMaze(file);
                        break;
                    case ".mhc":
                        //open a maze collection
                        OpenCollection(file);
                        break;
                }
            }

            timerMain.Enabled = true;
        }

        #endregion

        #region Utility Methods

        private void LoadToolbox()
        {
            toolBox.AllowSwappingByDragDrop = false;
            //create our image lists...
            ImageList toolboxImageList = new ImageList();
            Size iconSize = new Size(32,32);
            toolboxImageList.ImageSize = iconSize;
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.wall_horizontal_32.png"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.wall_leftup_32.png"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.wall_rightup_32.png"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.wall_leftdown_32.png"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.wall_rightdown_32.png"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.wall_vertical_32.png"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.wall_empty_32.png"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.pyroid_32.png"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.reactoid_32.png"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.perkoid_32.png"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.roboid_32.png"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.oxoid_32.png"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.key_32.png"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.arrow_32.png"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.lightning_32.png"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.lightning_v_32.png"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.clock_32.png"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.cannon_32.png"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.oneway_32.png"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.trippad_32.png"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.lock_32.png"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.hand_32.png"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.spikes_32.png"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.booties_32.png"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.transporter_32.png"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.pod_32.png"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.roboid_32.png"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.arrow_out_32.png"));
            toolBox.SmallImageList = toolboxImageList;

            int tabIndex;
            int itemIndex;
            tabIndex = toolBox.AddTab("Maze Walls", -1);
            itemIndex = toolBox[tabIndex].AddItem("Horizontal", 0, true, new MazeWall(MazeWallType.Horizontal));
            itemIndex = toolBox[tabIndex].AddItem("LeftUp", 1, true, new MazeWall(MazeWallType.LeftUp));
            itemIndex = toolBox[tabIndex].AddItem("RightUp", 2, true, new MazeWall(MazeWallType.RightUp));
            itemIndex = toolBox[tabIndex].AddItem("LeftDown", 3, true, new MazeWall(MazeWallType.LeftDown));
            itemIndex = toolBox[tabIndex].AddItem("RightDown", 4, true, new MazeWall(MazeWallType.RightDown));
            itemIndex = toolBox[tabIndex].AddItem("Vertical", 5, true, new MazeWall(MazeWallType.Vertical));
            itemIndex = toolBox[tabIndex].AddItem("Empty", 6, true, new MazeWall(MazeWallType.Empty));
            tabIndex = toolBox.AddTab("Maze Enemies", -1);
            itemIndex = toolBox[tabIndex].AddItem("Pyroid", 7, true, new Pyroid());
            itemIndex = toolBox[tabIndex].AddItem("Perkoid", 9, true, new Perkoid());
            itemIndex = toolBox[tabIndex].AddItem("Maxoid", 26, true, new Maxoid());
            itemIndex = toolBox[tabIndex].AddItem("Force Field", 14, true, new LightningH());
            itemIndex = toolBox[tabIndex].AddItem("Force Field", 15, true, new LightningV());
            itemIndex = toolBox[tabIndex].AddItem("Ion IonCannon", 17, true, new IonCannon());
            itemIndex = toolBox[tabIndex].AddItem("Trip Pad", 19, true, new TripPad());
            //toolBox[tabIndex].AddItem("Roboid", 10, true, null);
            tabIndex = toolBox.AddTab("Maze Objects", -1);
            itemIndex = toolBox[tabIndex].AddItem("Reactoid", 8, true, new Reactoid());
            itemIndex = toolBox[tabIndex].AddItem("Arrow", 13, true, new Arrow());
            itemIndex = toolBox[tabIndex].AddItem("Out Arrow", 27, true, new ArrowOut());
            itemIndex = toolBox[tabIndex].AddItem("Oxoid", 11, true, new Oxoid());
            itemIndex = toolBox[tabIndex].AddItem("One Way", 18, true, new OneWay());
            itemIndex = toolBox[tabIndex].AddItem("Stalactites", 22, true, new Spikes());
            itemIndex = toolBox[tabIndex].AddItem("Transporter", 24, true, new Transporter());
            itemIndex = toolBox[tabIndex].AddItem("Booties", 23, true, new Boots());
            itemIndex = toolBox[tabIndex].AddItem("Lock", 20, true, new Lock());
            itemIndex = toolBox[tabIndex].AddItem("Key", 12, true, new Key());
            itemIndex = toolBox[tabIndex].AddItem("De Hand", 21, true, new Hand());
            itemIndex = toolBox[tabIndex].AddItem("Clock", 16, true, new Clock());
            itemIndex = toolBox[tabIndex].AddItem("Escape Pod", 25, true, new EscapePod());

            toolBox.SelectedTabIndex = 2;
        }

        private string GetNewName(string prefix)
        {
            for (int i = 1; i > 0; i++)
            {
                TreeNode[] array = treeView.Nodes.Find(prefix + i.ToString(), false);
                if (array.Length == 0)
                {
                    return prefix + i.ToString();
                }
            }
            return prefix;
        }

        #endregion

        #region Tree Methods

        private void treeView_AfterSelect( object sender, TreeViewEventArgs e )
        {
            this.RefreshTree();
        }

        private void RefreshTree()
        {
            propertyGrid.SelectedObject = treeView.SelectedNode?.Tag;

            if ( treeView.SelectedNode?.Tag != null )
            {
                toolStripButtonSave.Enabled = true;

                if ( treeView.SelectedNode?.Tag is MazeController mazeController )
                {
                    toolStripButtonMAME.Enabled = true;

                    panelContent.Controls.Clear();
                    panelContent.Controls.Add( mazeController );

                    mazeController.Left = Math.Max( ( panelContent.Width - mazeController.Width ) / 2, 0 );
                    mazeController.Top = ( panelContent.Height - mazeController.Height ) / 2;
                    mazeController.PropertyGrid = propertyGrid;
                    mazeController.ComboBoxObjects = comboBoxMazeObjects;

                    _currentMazeController = mazeController;
                }
                else if ( treeView.SelectedNode?.Tag is MazeCollectionController mazeCollectionController )
                {
                    toolStripButtonMAME.Enabled = false;
                }
            }
            else
            {
                toolStripButtonSave.Enabled = false;

                toolStripButtonMAME.Enabled = false;

                panelContent.Controls.Clear();
            }

        }

        private void treeView_ItemDrag( object sender, ItemDragEventArgs e )
        {
            _draggedNode = (TreeNode)e.Item;

            //we can only drag maze objects..
            if ( _draggedNode?.Tag is MazeController mazeController )
            {
                DoDragDrop( mazeController, DragDropEffects.Copy | DragDropEffects.Move );
            }
        }

        private void treeView_DragOver(object sender, DragEventArgs e)
        {
            TreeNode targetNode = this.treeView.GetNodeAt(
                treeView.PointToClient( new Point( e.X, e.Y ) ) );

            e.Effect = DragDropEffects.None;

            if ( targetNode != _draggedNode && targetNode?.Tag is MazeController targetMaze )
            {
                MazeController draggedMaze = _draggedNode.Tag as MazeController;

                if ( targetMaze.MazeType == draggedMaze.MazeType )
                {
                    e.Effect = DragDropEffects.Move;
                }
            }
        }

        private void treeView_DragDrop(object sender, DragEventArgs e)
        {
            TreeNode targetNode = this.treeView.GetNodeAt(
                treeView.PointToClient( new Point( e.X, e.Y ) ) );

            if ( targetNode != _draggedNode && targetNode?.Tag is MazeController targetMaze )
            {
                MazeController draggedMaze = _draggedNode.Tag as MazeController;

                if ( targetMaze.MazeType == draggedMaze.MazeType )
                {
                    string msg = $"Replace {targetMaze.Name} with {draggedMaze.Name}?";

                    DialogResult dr = MessageBox.Show(
                        msg, MESSAGEBOX_CAPTION,
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Question );

                    if ( dr == DialogResult.OK )
                    {
                        //get the parent collection of the target maze
                        if ( targetNode.Parent?.Tag is MazeCollectionController controller )
                        {
                            MessageBox.Show( "Maze dragging not supported." );
                            //MazeCollection collection = (MazeCollection)node.Parent.Tag;
                            //int mazeindex = collection.FindMaze(maze);
                            //if (mazeindex > -1)
                            //{
                            //    collection.AddMaze(mazeindex, draggedmaze);
                            //    collection.TreeRender(treeView, node.Parent);
                            //}
                        }
                    }
                }
            }
        }


        #endregion

        #region Mainform Methods

        private void Mainform_Load( object sender, EventArgs e )
        {
            // https://stackoverflow.com/a/32561014
            if ( Properties.Settings.Default.IsMaximized )
            {
                WindowState = FormWindowState.Maximized;
            }
            else if ( Screen.AllScreens.Any( screen => screen.WorkingArea.IntersectsWith( Properties.Settings.Default.WindowPosition ) ) )
            {
                StartPosition = FormStartPosition.Manual;
                DesktopBounds = Properties.Settings.Default.WindowPosition;
                WindowState = FormWindowState.Normal;
            }
        }

        private void Mainform_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.IsMaximized = WindowState == FormWindowState.Maximized;
            Properties.Settings.Default.WindowPosition = DesktopBounds;
            Properties.Settings.Default.Save();

            DialogResult result = DialogResult.OK;

            try
            {
                foreach (TreeNode node in treeView.Nodes)
                {
                    /// allow short circuit when cancel is hit.
                    if ( node.Parent == null && result != DialogResult.Cancel )
                    {
                        if ( node.Tag is MazeCollectionController mazeCollectionController )
                        {
                            if ( mazeCollectionController.IsDirty )
                            {
                                result = MessageBox.Show(
                                    $"Would you like to save changes to the {mazeCollectionController.Name} collection before closing?",
                                    "Save changes and Exit",
                                    MessageBoxButtons.YesNoCancel,
                                    MessageBoxIcon.Question );

                                if ( result == DialogResult.Yes )
                                {
                                    SaveFileDialog sfd = new SaveFileDialog
                                    {
                                        InitialDirectory =
                                            string.IsNullOrWhiteSpace( mazeCollectionController.FileName ) ?
                                            Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ) :
                                            Path.GetDirectoryName( mazeCollectionController.FileName ),
                                        FileName = string.IsNullOrWhiteSpace( mazeCollectionController.FileName ) ?
                                            $"{mazeCollectionController.Name}.mhc" :
                                            Path.GetFileName( mazeCollectionController.FileName ),
                                        Filter = "Maze Files (*.mhc)|*.mhc|All files (*.*)|*.*",
                                        AddExtension = true,
                                        OverwritePrompt = true
                                    };

                                    /// capture user choice to update exit 
                                    result = sfd.ShowDialog();

                                    if ( result == DialogResult.OK )
                                    {
                                        Cursor.Current = Cursors.WaitCursor;

                                        mazeCollectionController.FileName = sfd.FileName;

                                        Application.DoEvents();

                                        MazeCollectionController.SerializeToFile(
                                            mazeCollectionController.MazeCollection,
                                            mazeCollectionController.FileName );

                                        mazeCollectionController.MazeCollection.IsDirty = false;
                                    }
                                }
                            }
                        }
                        else if ( node.Tag is MazeController mazeController )
                        {
                            result = MessageBox.Show(
                                $"Would you like to save changes to the {mazeController.Name} maze before closing?",
                                "Save changes and Exit",
                                MessageBoxButtons.YesNoCancel,
                                MessageBoxIcon.Question );

                            if ( result == DialogResult.Yes )
                            {
                                SaveFileDialog sfd = new SaveFileDialog
                                {
                                    InitialDirectory =
                                        string.IsNullOrWhiteSpace( mazeController.FileName ) ?
                                        Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ) :
                                        Path.GetDirectoryName( mazeController.FileName ),
                                    FileName = string.IsNullOrWhiteSpace( mazeController.FileName ) ?
                                        $"{mazeController.Name}.mhz" :
                                        Path.GetFileName( mazeController.FileName ),
                                    Filter = "Maze Files (*.mhz)|*.mhz|All files (*.*)|*.*",
                                    AddExtension = true,
                                    OverwritePrompt = true
                                };

                                /// capture user choice to update exit 
                                result = sfd.ShowDialog();

                                if ( result == DialogResult.OK )
                                {
                                    Cursor.Current = Cursors.WaitCursor;

                                    mazeController.FileName = sfd.FileName;

                                    Application.DoEvents();

                                    MazeController.SerializeToFile(
                                        mazeController.Maze, mazeController.FileName );

                                    mazeController.IsDirty = false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                /// Allow the user to try again if they feel so inclined.
                result = MessageBox.Show(
                    $"An error has occurred while trying to save: {ex.Message}" +
                    $"Press OK to exit or Cancel to try again.",
                    "An Error Occurred",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Error );
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

            /// cancel exiting based upon user choice.
            e.Cancel = result == DialogResult.Cancel;
        }

        #endregion

        #region ToolStrip Methods

        private void toolStripButtonGrid_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                if (treeView.SelectedNode.Tag != null)
                {
                    ((ITreeObject)treeView.SelectedNode.Tag).SetGridlines(toolStripButtonGrid.Checked);
                }
            }
            panelContent.Invalidate();
        }

        private void toolStripButtonZoomIn_Click(object sender, EventArgs e)
        {
            if (_currentMazeController != null)
            {
                _currentMazeController.Zoom += .1M;
                panelContent.Invalidate();
            }
        }

        private void toolStripButtonZoomOut_Click(object sender, EventArgs e)
        {
            if (_currentMazeController != null)
            {
                _currentMazeController.Zoom -= .1M;
                panelContent.ClientSize = new Size((int)(panelContent.Width * _currentMazeController.Zoom), (int)(panelContent.Height * _currentMazeController.Zoom));
                panelContent.Invalidate();
            }
        }

        private void toolStripButtonAbout_Click(object sender, EventArgs e)
        {
            DialogAbout about = new DialogAbout();
            about.ShowDialog();
        }

        #endregion

        #region Panel Methods

        private void panelContent_Paint(object sender, PaintEventArgs e)
        {
            foreach (Control control in panelContent.Controls)
            {
                control.Invalidate();
            }
        }

        #endregion

        #region TreeRightMenu Methods

        private void contextMenuStripTree_Opening(object sender, CancelEventArgs e)
        {
            if ( treeView.SelectedNode?.Tag is MazeController mazeController )
            {
                toolStripMenuItemClose.Enabled = true;
                toolStripMenuItemSave.Enabled = true;
                toolStripMenuItemSaveAs.Enabled = true;
                toolStripMenuItemDelete.Enabled = true;
                toolStripMenuItemRename.Enabled = true;
                toolStripMenuItemPreview.Enabled = true;
            }
            else if ( treeView.SelectedNode?.Tag is MazeCollectionController mazeCollectionController )
            {
                toolStripMenuItemClose.Enabled = true;
                toolStripMenuItemSave.Enabled = true;
                toolStripMenuItemSaveAs.Enabled = true;
                toolStripMenuItemDelete.Enabled = true;
                toolStripMenuItemRename.Enabled = true;
                toolStripMenuItemPreview.Enabled = false;
            }
            else
            {
                toolStripMenuItemClose.Enabled = false;
                toolStripMenuItemSave.Enabled = false;
                toolStripMenuItemSaveAs.Enabled = false;
                toolStripMenuItemDelete.Enabled = false;
                toolStripMenuItemRename.Enabled = false;
                toolStripMenuItemPreview.Enabled = false;
            }
        }

        #endregion

        private void toolStripButtonHome_Click(object sender, EventArgs e)
        {
            //take me to your leader...
            System.Diagnostics.Process.Start(Globals.MHPHomepage);
        }

        private void toolStripButtonMHP_Click(object sender, EventArgs e)
        {
            DialogConfiguration mhp = new DialogConfiguration();
            mhp.ShowDialog();
        }

        private void timerMain_Tick(object sender, EventArgs e)
        {
            timerMain.Enabled = false;
            //Show dialog...
            //string msg = @"This is a Pre-Release BETA version of the Major Havoc Level Editor. It is not complete and has many missing features. The purpose of this release is simply to allow users to preview the current functionality and submit bugs. Enjoy... The homeworld is near!";
            //MessageBox.Show(msg, "BETA Information");
        }


        private void toolStripButtonConfiguration_Click(object sender, EventArgs e)
        {
            DialogConfiguration d = new DialogConfiguration();
            d.ShowDialog();
        }

        private void toolStripButtonLoadFromROM_Click(object sender, EventArgs e)
        {

            DialogLoadROM dlr = new DialogLoadROM( Path.GetFullPath(
                Properties.Settings.Default.TemplatesLocation ) );

            DialogResult dr = dlr.ShowDialog();

            if (dr == DialogResult.OK)
            {
                MazeCollectionController collectionController = new MazeCollectionController(dlr.Mazes);
                //foreach (Maze maze in collectionController.Mazes)
                //{
                //    collectionController.OnMazePropertiesUpdated += new MazePropertiesUpdated(RefreshMazeName);
                //}

                TreeNode node = treeView.Nodes.Add(collectionController.MazeCollection.Name);
                //collectionController.PropertyGrid = propertyGrid;
                node.Tag = collectionController;
                collectionController.TreeRender(treeView, node, toolStripButtonGrid.Checked);
                node.ImageIndex = 0;
                node.SelectedImageIndex = node.ImageIndex;
                treeView.SelectedNode = node;
                _currentMazeCollectionController = collectionController;
                
            }
        }

        private void toolStripButtonContestUpload_Click(object sender, EventArgs e)
        {
            if (_currentMazeController == null)
            {
                MessageBox.Show("You must select a maze to upload.", "Missing Maze", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Image mazeImage = _currentMazeController.GetImage();

                if (_currentMazeController.Maze != null && mazeImage != null)
                {
                    DialogMHPLogin mhpDialog = new DialogMHPLogin();
                    if (!String.IsNullOrEmpty(Properties.Settings.Default.MHPUsername))
                    {
                        mhpDialog.Username = Properties.Settings.Default.MHPUsername;
                    }
                    if (!String.IsNullOrEmpty(Properties.Settings.Default.MHPPassword)
                        && !String.IsNullOrEmpty(Properties.Settings.Default.MHPKey))
                    {
                        mhpDialog.Password = Properties.Settings.Default.MHPPassword;
                        mhpDialog.PasswordKey = Properties.Settings.Default.MHPKey;
                    }
                    mhpDialog.SavePassword = Properties.Settings.Default.MHPSavePassword;
                    mhpDialog.MazePreview = mazeImage;
                    mhpDialog.MazeController = _currentMazeController;
                    mhpDialog.ShowDialog();
                }
            }
        }

        private void toolStripButtonSaveAll_Click( object sender, EventArgs e )
        {
            foreach ( TreeNode node in treeView.Nodes )
            {
                if ( node.Tag is MazeController mazeController &&
                    mazeController.IsDirty )
                {
                    this.SaveMaze( mazeController,
                        treeView.SelectedNode.Parent?.Tag as MazeCollectionController );
                }
                else if ( node.Tag is MazeCollectionController mazeCollectionController &&
                    mazeCollectionController.MazeCollection.IsDirty )
                {
                    this.SaveCollection( mazeCollectionController );
                }
            }
        }



        private void treeView_AfterLabelEdit( object sender, NodeLabelEditEventArgs e )
        {
            ///https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.nodelabelediteventargs?view=netframework-4.7.2
            if ( e.Label != null )
            {
                if ( e.Label.Length > 0 )
                {
                    if ( e.Label.IndexOfAny( Path.GetInvalidFileNameChars() ) == -1 )
                    {
                        // Stop editing without canceling the label change.
                        e.Node.EndEdit( false );

                        if ( e.Node.Tag is MazeController controller )
                        {
                            //controller.Name = controller.IsDirty ?
                            //    $"{e.Label} *" : e.Label;
                            controller.Name = e.Label;
                            propertyGrid.Refresh();
                        }
                        else if ( e.Node.Tag is MazeCollectionController collectionController )
                        {
                            //collectionController.Name = collectionController.IsDirty ?
                            //    $"{e.Label} *" : e.Label;
                            collectionController.Name = e.Label;
                            propertyGrid.Refresh();
                        }
                    }
                    else
                    {
                        /* Cancel the label edit action, inform the user, and 
                           place the node in edit mode again. */
                        e.CancelEdit = true;
                        MessageBox.Show( "Invalid tree node label.\n" +
                           "The invalid characters are: '@','.', ',', '!'",
                           "Node Label Edit" );
                        e.Node.BeginEdit();
                    }
                }
                else
                {
                    /* Cancel the label edit action, inform the user, and 
                       place the node in edit mode again. */
                    e.CancelEdit = true;
                    MessageBox.Show( "Invalid tree node label.\nThe label cannot be blank",
                       "Node Label Edit" );
                    e.Node.BeginEdit();
                }
            }
        }


        private void toolStripMenuItemRename_Click( object sender, EventArgs e )
        {
            if ( treeView.SelectedNode != null )
            {
                treeView.SelectedNode.BeginEdit();
            }
        }

        private void toolStripMenuItemDelete_Click( object sender, EventArgs e )
        {
            if ( treeView.SelectedNode != null )
            {
                DialogResult result = MessageBox.Show(
                    $"{treeView.SelectedNode.Text} will be deleted permanently?",
                    "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation );

                if ( result == DialogResult.OK )
                {
                    treeView.SelectedNode.Remove();

                    this.RefreshTree();
                }
            }
        }

        private void toolStripMenuItemClose_Click( object sender, EventArgs e )
        {
            bool safeToRemove = true;

            if ( treeView.SelectedNode?.Tag is MazeController mazeController &&
                mazeController.IsDirty )
            {
                safeToRemove = this.SaveMaze( mazeController,
                    treeView.SelectedNode.Parent?.Tag as MazeCollectionController );
            }
            else if ( treeView.SelectedNode?.Tag is MazeCollectionController mazeCollectionController &&
                mazeCollectionController.MazeCollection.IsDirty )
            {
                safeToRemove = this.SaveCollection( mazeCollectionController );
            }

            if ( safeToRemove )
            {
                treeView.SelectedNode.Remove();
                this.RefreshTree();
            }
        }

        private void toolStripMenuItemOpen_Click( object sender, EventArgs e )
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Open Maze or Maze Collection",
                InitialDirectory = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ),
                Filter = "Editor Files (*.mhz;*.mhc)|*.mhz;*.mhc|Mazes (*.mhz)|*.mhz|Maze Collections (*.mhc)|*.mhc",
                CheckFileExists = true,
            };

            if ( ofd.ShowDialog() == DialogResult.OK )
            {
                string fileExtension = Path.GetExtension( ofd.FileName );

                if ( fileExtension.Equals( ".mhz", StringComparison.CurrentCultureIgnoreCase ) )
                {
                    this.OpenMaze( ofd.FileName );
                }
                else if ( fileExtension.Equals( ".mhc", StringComparison.CurrentCultureIgnoreCase ) )
                {
                    this.OpenCollection( ofd.FileName );
                }
            }
        }

        private void toolStripMenuItemSave_Click( object sender, EventArgs e )
        {
            if ( treeView.SelectedNode?.Tag is MazeController mazeController )
            {
                this.SaveMaze( mazeController,
                    treeView.SelectedNode.Parent?.Tag as MazeCollectionController );
            }
            else if ( treeView.SelectedNode?.Tag is MazeCollectionController mazeCollectionController )
            {
                this.SaveCollection( mazeCollectionController );
            }
        }

        private void toolStripMenuItemSaveAs_Click( object sender, EventArgs e )
        {
            if ( treeView.SelectedNode?.Tag is MazeController mazeController )
            {
                string savedFilePath = mazeController.FileName;

                /// Erase the filename if there is one to force OpenFileDialog
                mazeController.FileName = string.Empty;

                /// If the save was not performed then restore previous FileName
                if ( !this.SaveMaze( mazeController,
                        treeView.SelectedNode.Parent?.Tag as MazeCollectionController ) )
                {
                    mazeController.FileName = savedFilePath;
                }
            }
            else if ( treeView.SelectedNode?.Tag is MazeCollectionController mazeCollectionController )
            {
                string savedFilePath = mazeCollectionController.FileName;

                /// Erase the filename if there is one to force OpenFileDialog
                mazeCollectionController.FileName = string.Empty;

                /// If the save was not performed then restore previous FileName
                if ( !this.SaveCollection( mazeCollectionController ) )
                {
                    mazeCollectionController.FileName = savedFilePath;
                }
            }
        }

        private void toolStripMenuItemNewMazeCollection_Click( object sender, EventArgs e )
        {
            Cursor.Current = Cursors.WaitCursor;

            MazeCollectionController collectionController =
                new MazeCollectionController( GetNewName( "MazeCollection" ) );

            for ( int i = 0; i < 28; i++ )
            {
                MazeType mazeType = (MazeType)( i & 0x03 );
                Maze maze = new Maze( (MazeType)mazeType, "Level " + ( i + 1 ).ToString() );
                collectionController.Mazes.Add( maze );
            }
            TreeNode node = collectionController.TreeRender( treeView, null, toolStripButtonGrid.Checked );
            node.ImageIndex = 0;
            node.SelectedImageIndex = node.ImageIndex;

            treeView.SelectedNode = node;
            node.BeginEdit();
            Cursor.Current = Cursors.Default;
        }

        private void toolStripMenuItemNewMaze_Click( object sender, EventArgs e )
        {
            Cursor.Current = Cursors.WaitCursor;

            Maze maze = new Maze( GetNewName( "Maze" ) );
            MazeController mazeController = new MazeController( maze );
            mazeController.OnMazePropertiesUpdated += new MazePropertiesUpdated( RefreshMazeName );
            mazeController.ShowGridReferences = Properties.Settings.Default.ShowGridReferences;
            mazeController.PropertyGrid = propertyGrid;
            mazeController.ComboBoxObjects = comboBoxMazeObjects;
            TreeNode node = mazeController.TreeRender( treeView, null, toolStripButtonGrid.Checked );
            node.ImageIndex = ( (int)maze.MazeType ) + 1;
            node.SelectedImageIndex = node.ImageIndex;
            treeView.SelectedNode = node;
            _currentMazeController = mazeController;
            Cursor.Current = Cursors.Default;
            node.BeginEdit();
        }

        private void toolStripMenuItemPreview_Click( object sender, EventArgs e )
        {
            if ( _currentMazeCollectionController != null && _currentMazeController != null )
            {
                if ( MAMEHelper.SaveROM( _currentMazeCollectionController.MazeCollection, _currentMazeController.Maze ) )
                {
                    try
                    {
                        //now launch MAME for mhavoc..
                        string mameExe = Path.GetFullPath( Properties.Settings.Default.MameExecutable );

                        string args = "";

                        if ( Properties.Settings.Default.MameDebug )
                        {
                            args += "-debug ";
                        }

                        if ( Properties.Settings.Default.MameWindow )
                        {
                            args += "-window ";
                        }

                        args += Properties.Settings.Default.MameDriver;

                        ProcessStartInfo info = new ProcessStartInfo( mameExe, args );
                        info.ErrorDialog = true;
                        info.RedirectStandardError = true;
                        info.RedirectStandardOutput = true;
                        info.UseShellExecute = false;
                        info.WorkingDirectory = Path.GetDirectoryName( mameExe );

                        Process p = new Process();
                        p.EnableRaisingEvents = true;
                        p.StartInfo = info;
                        p.Exited += new EventHandler( ProcessExited );
                        p.Start();
                    }
                    catch ( Exception ex )
                    {
                        MessageBox.Show( $"There was an error launching MAME," +
                            $" verify your MAME paths in the configuration. {ex.Message}" );
                    }
                }
            }
            else
            {
                MessageBox.Show( "The current maze is not loaded correctly." +
                    " This is most likely an issue with the application. Please contact Jess." );
            }
        }

        private void ProcessExited( object sender, EventArgs e )
        {
            if ( sender is Process )
            {
                Process p = (Process)sender;
                if ( p.ExitCode != 0 )
                {
                    if ( p.StandardError != null )
                    {
                        string errorResult = p.StandardError.ReadToEnd();
                        if ( !String.IsNullOrEmpty( errorResult ) )
                        {
                            MessageBox.Show( errorResult, "MAME Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        }
                    }
                }
            }
            //put back the ROM's from backup
            string mamePath = Path.GetDirectoryName( Properties.Settings.Default.MameExecutable ) + "\\roms\\" + Properties.Settings.Default.MameDriver + "\\";
            string backupPath = mamePath + "_backup\\";

            foreach ( string file in Directory.GetFiles( backupPath ) )
            {
                File.Copy( file, mamePath + Path.GetFileName( file ), true );
            }
        }

        private void treeView_MouseDown( object sender, MouseEventArgs args )
        {
            if ( args.Button == MouseButtons.Right )
            {
                TreeNode node = treeView.GetNodeAt( args.Location );

                if ( node != null && node.Bounds.Contains( args.Location ) )
                {
                    treeView.SelectedNode = node;
                }
            }
        }

        private void OpenMaze( string fileName )
        {
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();
            try
            {
                MazeController mazeController = new MazeController( MazeController.DeserializeFromFile( fileName ) );
                TreeNode node = treeView.Nodes.Add( mazeController.Maze.Name );
                node.Tag = mazeController;
                node.ImageIndex = ( (int)mazeController.Maze.MazeType ) + 1;
                node.SelectedImageIndex = node.ImageIndex;
                treeView.SelectedNode = node;
                mazeController.FileName = fileName;
                mazeController.TreeRender( treeView, node, toolStripButtonGrid.Checked );
                mazeController.PropertyGrid = propertyGrid;
                mazeController.ComboBoxObjects = comboBoxMazeObjects;
                _currentMazeController = mazeController;
            }
            catch ( Exception ex )
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show( $"Maze could not be opened: {ex.Message}",
                    "File Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void OpenCollection( string fileName )
        {
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();
            try
            {
                MazeCollection mazeCollection = MazeCollectionController.DeserializeFromFile( fileName );
                MazeCollectionController mazeCollectionControl = new MazeCollectionController( mazeCollection );
                mazeCollectionControl.FileName = fileName;
                TreeNode node = treeView.Nodes.Add( mazeCollection.Name );
                node.Tag = mazeCollection;
                mazeCollectionControl.TreeRender( treeView, node, toolStripButtonGrid.Checked );
                node.ImageIndex = 0;
                node.SelectedImageIndex = node.ImageIndex;
                treeView.SelectedNode = node;
                _currentMazeCollectionController = mazeCollectionControl;
            }
            catch ( Exception ex )
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show( $"Maze Collection could not be opened: {ex.Message}",
                    "File Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private bool SaveCollection( MazeCollectionController mazeCollectionController )
        {
            DialogResult result = DialogResult.OK;

            /// if there isn't a file associated with this MazeCollection then ask
            /// for the fileName. 
            if ( string.IsNullOrWhiteSpace( mazeCollectionController.FileName ) )
            {
                SaveFileDialog sfd = new SaveFileDialog
                {
                    InitialDirectory = Environment.GetFolderPath(
                        Environment.SpecialFolder.MyDocuments ),
                    FileName = $"{mazeCollectionController.Name}.mhc",
                    Filter = "Maze Files (*.mhc)|*.mhc|All files (*.*)|*.*",
                    AddExtension = true,
                    OverwritePrompt = true
                };

                /// capture user choice for save operation below.
                result = sfd.ShowDialog();

                if ( result == DialogResult.OK )
                {
                    mazeCollectionController.FileName = sfd.FileName;
                }
            }

            try
            {
                if ( result == DialogResult.OK )
                {
                    Cursor.Current = Cursors.WaitCursor;

                    Application.DoEvents();

                    MazeCollectionController.SerializeToFile(
                        mazeCollectionController.MazeCollection,
                        mazeCollectionController.FileName );

                    mazeCollectionController.MazeCollection.IsDirty = false;
                }
            }
            catch ( Exception ex )
            {
                result = DialogResult.Cancel;

                MessageBox.Show(
                    $"An error has occurred while trying to save: {ex.Message}",
                    "An Error Occurred",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error );
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

            return result == DialogResult.OK;
        }

        private bool SaveMaze( MazeController mazeController, MazeCollectionController mazeCollectionController )
        {
            DialogResult result = DialogResult.OK;

            /// if there isn't a file associated with this MazeCollection then ask
            /// for the fileName. 
            if ( string.IsNullOrWhiteSpace( mazeController.FileName ) )
            {
                SaveFileDialog sfd = new SaveFileDialog
                {
                    InitialDirectory = Environment.GetFolderPath(
                        Environment.SpecialFolder.MyDocuments ),
                    /// Have FileName include parent collection if not already set.
                    FileName = mazeCollectionController != null ?
                        $"{mazeCollectionController.Name}.{mazeController.Name}.mhz" :
                        $"{mazeController.Name}.mhz",
                    Filter = "Maze Files (*.mhz)|*.mhz|All files (*.*)|*.*",
                    AddExtension = true,
                    OverwritePrompt = true
                };

                /// capture user choice for save operation below.
                result = sfd.ShowDialog();

                if ( result == DialogResult.OK )
                {
                    mazeController.FileName = sfd.FileName;
                }
            }

            try
            {
                if ( result == DialogResult.OK )
                {
                    Cursor.Current = Cursors.WaitCursor;

                    Application.DoEvents();

                    MazeController.SerializeToFile(
                        mazeController.Maze, mazeController.FileName );

                    mazeController.IsDirty = false;
                }
            }
            catch ( Exception ex )
            {
                result = DialogResult.Cancel;

                MessageBox.Show(
                    $"An error has occurred while trying to save: {ex.Message}",
                    "An Error Occurred",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error );
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

            return result == DialogResult.OK;
        }

        private void RefreshMazeName( object sender )
        {
            if ( treeView.SelectedNode != null )
            {
                treeView.SelectedNode.Text = ( (MazeController)sender ).Name;
            }
        }
    }
}