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

using mhedit.Containers;
using mhedit.Containers.MazeEnemies;
using mhedit.Containers.MazeEnemies.IonCannon;
using mhedit.Containers.MazeObjects;
using mhedit.Containers.Validation;
using mhedit.Controllers;
using mhedit.GameControllers;
using mhedit.Extensions;

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
        private TreeNode _draggedNode;

        #region Constructor

        /// <summary>
        /// Our default constructor for the application mainform
        /// </summary>
        public Mainform()
        {
            InitializeComponent();

            treeView.ContextMenuStrip = contextMenuStripTree;

            /// Hack to make "system wide UX" component.
            ValidationExtensions.SystemWindows = this.tabControlSystemWindows;

            this.Text =
                $"{this.Text} - {Containers.VersionInformation.ApplicationVersion}  BETA VERSION";

            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);

            LoadToolbox();
            ParseCommandLine();

            timerMain.Enabled = true;
        }

        #endregion

        #region Utility Methods

        private void LoadToolbox()
        {
            toolBox.AllowSwappingByDragDrop = false;
            //create our image lists...
            ImageList toolboxImageList = new ImageList();
            Size iconSize = new Size(32, 32);
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
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.keypouch_32.png"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.token_32.png"));
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
            itemIndex = toolBox[tabIndex].AddItem("KeyPouch", 28, true, new KeyPouch());
            itemIndex = toolBox[tabIndex].AddItem("Lock", 20, true, new Lock());
            itemIndex = toolBox[tabIndex].AddItem("Key", 12, true, new Key());
            itemIndex = toolBox[tabIndex].AddItem("De Hand", 21, true, new Hand());
            itemIndex = toolBox[tabIndex].AddItem("Clock", 16, true, new Clock());
            itemIndex = toolBox[tabIndex].AddItem("Escape Pod", 25, true, new EscapePod());
            itemIndex = toolBox[tabIndex].AddItem("Hidden Level Token", 29, true, new HiddenLevelToken());

            toolBox.SelectedTabIndex = 2;
        }

        private void ParseCommandLine()
        {
            //look at command line args..
            string passedFilename = "";
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                passedFilename = args[0];
            }
            else
            {
                if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                {
                    if (AppDomain.CurrentDomain != null)
                    {
                        if (AppDomain.CurrentDomain.SetupInformation != null)
                        {
                            if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments != null)
                            {
                                if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData != null)
                                {
                                    if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData.Length > 0)
                                    {
                                        Uri uri = new Uri(AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData[0]);
                                        passedFilename = uri.LocalPath.ToString();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (!String.IsNullOrEmpty(passedFilename))
            {
                if (File.Exists(passedFilename))
                {
                    switch (Path.GetExtension(passedFilename))
                    {
                        case ".mhz":
                            //open a maze 
                            OpenMaze(passedFilename);
                            break;
                        case ".mhc":
                            //open a maze collection
                            OpenCollection(passedFilename);
                            break;
                    }
                }
            }
        }

        #endregion

        #region Tree Methods

        private void treeView_BeforeSelect( object sender, TreeViewCancelEventArgs e )
        {
            propertyGrid.SelectedObject = null;

            if ( treeView.SelectedNode?.Tag is MazeController mazeController )
            {
                mazeController.PropertyGrid = null;
                mazeController.ComboBoxObjects = null;
            }
        }

        private void treeView_AfterSelect( object sender, TreeViewEventArgs e )
        {
            this.RefreshTree();
        }

        private void RefreshTree()
        {
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

                    propertyGrid.SelectedObject = mazeController.Maze;
                }
                else if ( treeView.SelectedNode?.Tag is MazeCollectionController mazeCollectionController )
                {
                    toolStripButtonMAME.Enabled = false;

                    propertyGrid.SelectedObject = mazeCollectionController.MazeCollection;
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

                if ( targetMaze.Maze.MazeType == draggedMaze.Maze.MazeType )
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

                if ( targetMaze.Maze.MazeType == draggedMaze.Maze.MazeType )
                {
                    string msg = $"Replace {targetMaze.Maze.Name} with {draggedMaze.Maze.Name}?";

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

        private void treeView_BeforeLabelEdit( object sender, NodeLabelEditEventArgs e )
        {
            if ( ( ModifierKeys & ( Keys.Control | Keys.Shift ) ) > 0  )
            {
                e.CancelEdit = true;
            }
        }

        private void treeView_AfterLabelEdit( object sender, NodeLabelEditEventArgs e )
        {
            //https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.nodelabelediteventargs?view=netframework-4.7.2
            if ( e.Label != null )
            {
                if ( e.Label.Length > 0 )
                {
                    if (e.Label.Length <= 50)
                    {
                        if (e.Label.IndexOfAny(Path.GetInvalidFileNameChars()) == -1)
                        {
                            // Stop editing without canceling the label change.
                            e.Node.EndEdit(false);

                            if (e.Node.Tag is MazeController controller)
                            {
                                controller.Maze.Name = e.Label;

                                propertyGrid.Refresh();
                            }
                            else if (e.Node.Tag is MazeCollectionController collectionController)
                            {
                                collectionController.MazeCollection.Name = e.Label;

                                propertyGrid.Refresh();
                            }
                        }
                        else
                        {
                            /* Cancel the label edit action, inform the user, and 
                               place the node in edit mode again. */
                            e.CancelEdit = true;

                            string invalid = new string(Path.GetInvalidFileNameChars()
                                                         .Where(c => !char.IsControl(c))
                                                         .ToArray());

                            MessageBox.Show($"The name \"{e.Label}\" contains invalid characters: {invalid}",
                                "Invalid Name");

                            e.Node.BeginEdit();
                        }
                    }
                    else
                    {
                        /* Cancel the label edit action, inform the user, and 
                       place the node in edit mode again. */
                        e.CancelEdit = true;
                        MessageBox.Show("Maze Name cannot be longer than 50 characters.",
                           "Node Label Edit");
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

        /// <summary>
        /// https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.treeview.drawnode?redirectedfrom=MSDN&view=netframework-4.7.2
        /// </summary>
        private void treeView_DrawNode( object sender, DrawTreeNodeEventArgs e )
        {
            // Use the default background and node text.
            e.DrawDefault = !e.Node.IsEditing;

            if ( e.Node.IsEditing )
            {
                // While editing the Node Text don't paint the existing name behind.
                e.Graphics.FillRectangle( new SolidBrush( SystemColors.Window ), e.Bounds );

                return;
            }

            // Extract the set font/color from the tree.
            Font nodeFont =
                new Font( e.Node.NodeFont ?? e.Node.TreeView.Font, FontStyle.Bold );

            SolidBrush nodeBrush = new SolidBrush( e.Node.TreeView.ForeColor );

            // If a node tag is present, draw the IChangeTracking info if necessary.
            if ( e.Node.Tag is MazeController controller )
            {
                e.Graphics.DrawString(
                    controller.Maze.IsChanged ? ChangeTrackingBase.ModifiedBullet : "",
                    nodeFont, nodeBrush, e.Node.Bounds.Right + 4, e.Node.Bounds.Top );
            }
            else if ( e.Node.Tag is MazeCollectionController collection )
            {
                e.Graphics.DrawString(
                    collection.MazeCollection.IsChanged ? ChangeTrackingBase.ModifiedBullet : "",
                    nodeFont, nodeBrush, e.Node.Bounds.Right + 4, e.Node.Bounds.Top );
            }
        }

        private void treeView_MouseDown( object sender, MouseEventArgs args )
        {
            if ( args.Button == MouseButtons.Right )
            {
                TreeNode node = this.treeView.GetNodeAt( args.Location );

                /// only force selection if someone right clicks on the node's text
                /// and the node isn't already selected.
                if ( node != null  &&
                     node.Bounds.Contains( args.Location ) &&
                     !this.treeView.SelectedNodes.Contains( node ) )
                {
                    treeView.SelectedNode = node;
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
                    // allow short circuit when cancel is hit.
                    if ( node.Parent == null && result != DialogResult.Cancel )
                    {
                        if ( node.Tag is MazeCollectionController mazeCollectionController &&
                             mazeCollectionController.MazeCollection.IsChanged )
                        {
                            result = MessageBox.Show(
                                $"Would you like to save changes to the {mazeCollectionController.MazeCollection.Name} collection before closing?",
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
                                        $"{mazeCollectionController.MazeCollection.Name}.mhc" :
                                        Path.GetFileName( mazeCollectionController.FileName ),
                                    Filter = "Maze Files (*.mhc)|*.mhc|All files (*.*)|*.*",
                                    AddExtension = true,
                                    OverwritePrompt = true
                                };

                                // capture user choice to update exit 
                                result = sfd.ShowDialog();

                                if ( result == DialogResult.OK )
                                {
                                    Cursor.Current = Cursors.WaitCursor;

                                    mazeCollectionController.FileName = sfd.FileName;

                                    Application.DoEvents();

                                    mazeCollectionController.MazeCollection
                                                            .SerializeAndCompress( mazeCollectionController.FileName );

                                    mazeCollectionController.MazeCollection.AcceptChanges();
                                }
                            }
                        }
                        else if ( node.Tag is MazeController mazeController &&
                                  mazeController.Maze.IsChanged )
                        {
                            result = MessageBox.Show(
                                $"Would you like to save changes to the {mazeController.Maze.Name} maze before closing?",
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
                                        $"{mazeController.Maze.Name}.mhz" :
                                        Path.GetFileName( mazeController.FileName ),
                                    Filter = "Maze Files (*.mhz)|*.mhz|All files (*.*)|*.*",
                                    AddExtension = true,
                                    OverwritePrompt = true
                                };

                                // capture user choice to update exit 
                                result = sfd.ShowDialog();

                                if ( result == DialogResult.OK )
                                {
                                    Cursor.Current = Cursors.WaitCursor;

                                    mazeController.FileName = sfd.FileName;

                                    Application.DoEvents();

                                    mazeController.Maze.SerializeAndCompress( mazeController.FileName );

                                    mazeController.Maze.AcceptChanges();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Allow the user to try again if they feel so inclined.
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

            // cancel exiting based upon user choice.
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
            //if (_currentMazeController != null)
            //{
            //    _currentMazeController.Zoom += .1M;
            //    panelContent.Invalidate();
            //}
        }

        private void toolStripButtonZoomOut_Click(object sender, EventArgs e)
        {
            //if (_currentMazeController != null)
            //{
            //    _currentMazeController.Zoom -= .1M;
            //    panelContent.ClientSize = new Size((int)(panelContent.Width * _currentMazeController.Zoom), (int)(panelContent.Height * _currentMazeController.Zoom));
            //    panelContent.Invalidate();
            //}
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
                toolStripMenuItemValidate.Enabled = true;
                toolStripMenuItemDelete.Enabled = true;

                if ( this.treeView.SelectedNodes.Count > 1 )
                {
                    toolStripMenuItemSave.Enabled = false;
                    toolStripMenuItemSaveAs.Enabled = false;
                    toolStripMenuItemRename.Enabled = false;
                    toolStripMenuItemPreview.Enabled = false;
                }
                else
                {
                    toolStripMenuItemSave.Enabled = true;
                    toolStripMenuItemSaveAs.Enabled = true;
                    toolStripMenuItemRename.Enabled = true;
                    toolStripMenuItemPreview.Enabled = true;
                }
            }
            else if ( treeView.SelectedNode?.Tag is MazeCollectionController mazeCollectionController )
            {
                toolStripMenuItemClose.Enabled = true;
                toolStripMenuItemValidate.Enabled = true;
                toolStripMenuItemDelete.Enabled = true;
                toolStripMenuItemPreview.Enabled = false;

                if ( this.treeView.SelectedNodes.Count > 1 )
                {
                    toolStripMenuItemSave.Enabled = false;
                    toolStripMenuItemSaveAs.Enabled = false;
                    toolStripMenuItemRename.Enabled = false;
                }
                else
                {
                    toolStripMenuItemSave.Enabled = true;
                    toolStripMenuItemSaveAs.Enabled = true;
                    toolStripMenuItemRename.Enabled = true;
                }
            }
            else
            {
                toolStripMenuItemClose.Enabled = false;
                toolStripMenuItemSave.Enabled = false;
                toolStripMenuItemSaveAs.Enabled = false;
                toolStripMenuItemValidate.Enabled = false;
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
                MazeCollectionController collectionController =
                    new MazeCollectionController(dlr.MazeCollection);

                TreeNode node = collectionController.TreeRender( treeView, null, toolStripButtonGrid.Checked );
                node.ImageIndex = 0;
                node.SelectedImageIndex = node.ImageIndex;

                collectionController.MazeCollection.PropertyChanged += this.OnInstructionPropertyChanged;

                treeView.SelectedNode = node;
            }
        }

        private void toolStripButtonContestUpload_Click(object sender, EventArgs e)
        {
            if ( this.treeView.SelectedNode?.Tag is MazeController mazeController )
            {
                ValidationResults validationResults = (ValidationResults)mazeController.Maze.Validate();

                if (validationResults.Where(v => v.Level == ValidationLevel.Error).Count() > 0)
                {
                    MessageBox.Show("There are validation errors on the maze selected for upload. Please correct all Validation errors before uploading. \r\n\r\nYou can " +
                        "view all validation errors by selecting the maze in the tree, right clicking and selecting 'Validate'.", "Validation Errors", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    Image mazeImage = mazeController.GetImage();

                    if (mazeController.Maze != null && mazeImage != null)
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
                        mhpDialog.MazeController = mazeController;
                        mhpDialog.ShowDialog();
                    }
                }
            }
            else
            {
                MessageBox.Show( "You must select a maze to upload.", "Missing Maze",
                    MessageBoxButtons.OK, MessageBoxIcon.Information );
            }
        }

        private void toolStripButtonSaveAll_Click( object sender, EventArgs e )
        {
            foreach ( TreeNode node in treeView.Nodes )
            {
                if ( node.Tag is MazeController mazeController &&
                    mazeController.Maze.IsChanged )
                {
                    this.SaveMaze( mazeController,
                        treeView.SelectedNode.Parent?.Tag as MazeCollectionController );
                }
                else if ( node.Tag is MazeCollectionController mazeCollectionController &&
                    mazeCollectionController.MazeCollection.IsChanged )
                {
                    this.SaveCollection( mazeCollectionController );
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
            if ( treeView.SelectedNodes.Count > 0 )
            {
                DialogResult result = MessageBox.Show(
                    this.treeView.SelectedNodes.Count == 1 ?
                        $"{this.treeView.SelectedNodes.First().Name} will be deleted permanently!" :
                        $"All Selected nodes will be deleted permanently!",
                    "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation );

                if ( result == DialogResult.OK )
                {
                    List<TreeNode> toDelete = new List<TreeNode>( this.treeView.SelectedNodes );

                    foreach ( TreeNode node in toDelete )
                    {
                        // If maze is part of a collection must remove it too.
                        if ( node.Tag is MazeController controller &&
                             node.Parent?.Tag is MazeCollectionController collectionController )
                        {
                            collectionController.MazeCollection.Mazes.Remove( controller.Maze );
                        }

                        node.Remove();
                    }

                    /// HACK... need to fix the multiselect treeview to handle this stuff.
                    this.treeView.SelectedNodes.Clear();

                    if ( this.treeView.SelectedNode != null )
                    {
                        this.treeView.SelectedNodes.Add( this.treeView.SelectedNode );
                    }
                    /// HACK

                    this.RefreshTree();
                }
            }
        }

        private void toolStripMenuItemClose_Click( object sender, EventArgs e )
        {
            bool safeToRemove = true;

            if ( this.treeView.SelectedNode?.Tag is IChangeTracking changeTracking )
            {
                if ( changeTracking.IsChanged )
                {
                    DialogResult result = MessageBox.Show(
                        $"Save changes to {this.treeView.SelectedNode.Text}?",
                        "Close", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation );

                    if ( result == DialogResult.Yes )
                    {
                        if ( this.treeView.SelectedNode?.Tag is MazeController mazeController )
                        {
                            safeToRemove = this.SaveMaze( mazeController,
                                this.treeView.SelectedNode.Parent?.Tag as MazeCollectionController );
                        }
                        else if ( this.treeView.SelectedNode?.Tag is MazeCollectionController mazeCollectionController )
                        {
                            safeToRemove = this.SaveCollection( mazeCollectionController );
                        }
                    }
                    else if ( result == DialogResult.Cancel )
                    {
                        safeToRemove = false;
                    }
                }
            }

            if ( safeToRemove && this.treeView.SelectedNode != null )
            {
                this.treeView.SelectedNode.Remove();

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

                // Erase the filename if there is one to force OpenFileDialog
                mazeController.FileName = string.Empty;

                // If the save was not performed then restore previous FileName
                if ( !this.SaveMaze( mazeController,
                        treeView.SelectedNode.Parent?.Tag as MazeCollectionController ) )
                {
                    mazeController.FileName = savedFilePath;
                }
            }
            else if ( treeView.SelectedNode?.Tag is MazeCollectionController mazeCollectionController )
            {
                string savedFilePath = mazeCollectionController.FileName;

                // Erase the filename if there is one to force OpenFileDialog
                mazeCollectionController.FileName = string.Empty;

                // If the save was not performed then restore previous FileName
                if ( !this.SaveCollection( mazeCollectionController ) )
                {
                    mazeCollectionController.FileName = savedFilePath;
                }
            }
        }

        private void toolStripMenuItemNewMazeCollection_Click( object sender, EventArgs e )
        {
            Cursor.Current = Cursors.WaitCursor;

            Application.DoEvents();
            try
            {

                MazeCollectionController collectionController =
                new MazeCollectionController( NameFactory.Create( "MazeCollection" ) );

                for ( int i = 0; i < 28; i++ )
                {
                    MazeType mazeType = (MazeType)( i & 0x03 );
                    Maze maze = new Maze( (MazeType)mazeType, "Level " + ( i + 1 ).ToString() );
                    collectionController.MazeCollection.Mazes.Add( maze );
                }

                collectionController.MazeCollection.AcceptChanges();

                TreeNode node = collectionController.TreeRender( treeView, null, toolStripButtonGrid.Checked );
                node.ImageIndex = 0;
                node.SelectedImageIndex = node.ImageIndex;

                collectionController.MazeCollection.PropertyChanged += this.OnInstructionPropertyChanged;

                treeView.SelectedNode = node;

                node.BeginEdit();
            }
            catch ( Exception ex )
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show( $"Maze could not be created: {ex.Message}",
                    "Create Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void toolStripMenuItemNewMaze_Click( object sender, EventArgs e )
        {
            Cursor.Current = Cursors.WaitCursor;

            Application.DoEvents();
            try
            {

                MazeController mazeController = new MazeController( new Maze( NameFactory.Create( "Maze" ) ) );

                TreeNode node = mazeController.TreeRender( treeView, null, toolStripButtonGrid.Checked );

                node.ImageIndex = ( (int)mazeController.Maze.MazeType ) + 1;
                node.SelectedImageIndex = node.ImageIndex;
                treeView.SelectedNode = node;

                // Only connect PropertyChanged event if the maze isn't part of a collection
                // (it doesn't have a parent) as it's fed through the collection otherwise.
                if ( node.Parent == null )
                {
                    mazeController.Maze.PropertyChanged += this.OnInstructionPropertyChanged;
                }

                mazeController.ShowGridReferences = Properties.Settings.Default.ShowGridReferences;
                mazeController.PropertyGrid = propertyGrid;
                mazeController.ComboBoxObjects = comboBoxMazeObjects;

                node.BeginEdit();
            }
            catch ( Exception ex )
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show( $"Maze could not be created: {ex.Message}",
                    "Create Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void toolStripMenuItemPreview_Click( object sender, EventArgs e )
        {

                // Get the selected node's MazeController/Maze so we can make it the preview target.
                if ( this.treeView.SelectedNode?.Tag is MazeController mazeController )
                {
                    // Always create a MazeCollection for the preview of a Maze. Otherwise, any
                    // existing validation errors in the parent MazeCollection will keep us from
                    // being able to run the Preview of this level. So build a temporary collection
                    // from the template.
                    //IGameController mhpe = new MajorHavocPromisedEnd();

                    //if (mhpe.LoadTemplate(Path.GetFullPath(Properties.Settings.Default.TemplatesLocation)))
                    //{
                        //MazeCollection mazeCollection = mhpe.LoadMazes(new List<string>());

                        // inject our single maze based upon the maze type A=0,B=1,C=2,D=3
                        //mazeCollection.Mazes[(int)mazeController.Maze.MazeType] = mazeController.Maze;
                    if (this.treeView.SelectedNode.Parent != null && this.treeView.SelectedNode.Parent.Tag is MazeCollectionController mazeCollectionController)
                    {
                        // Now that we have a Maze in a MazeCollection we can build a ROM set to preview from.
                        if (MAMEHelper.SaveROM(mazeCollectionController.MazeCollection, mazeController.Maze))
                        {
                            //now launch MAME for mhavoc..
                            string mameExe =
                                Path.GetFullPath(Properties.Settings.Default.MameExecutable);

                            string args = "";

                            if (Properties.Settings.Default.MameDebug)
                            {
                                args += "-debug ";
                            }
                            if (Properties.Settings.Default.MameWindow)
                            {
                                args += "-window ";
                            }
                            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.MameCommandLineOptions))
                            {
                                // force at least 1 space on end.
                                args += $"{Properties.Settings.Default.MameCommandLineOptions} ";
                            }

                            args += Properties.Settings.Default.MameDriver;

                        try
                        {
                            ProcessStartInfo info = new ProcessStartInfo(mameExe, args)
                            {
                                ErrorDialog = true,
                                RedirectStandardError = true,
                                RedirectStandardOutput = true,
                                UseShellExecute = false,
                                WorkingDirectory = Path.GetDirectoryName(mameExe)
                            };

                            Process p = new Process
                            {
                                EnableRaisingEvents = true,
                                StartInfo = info
                            };
                            p.Exited += this.ProcessExited;
                            p.Start();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"There was an error launching HBMAME (" + mameExe + " " + args + ")." +
                                             $" Verify your HBMAME paths in the configuration. {ex.Message}");
                        }
                    }
                    }

                        
                    //}
                    //else
                    //{
                    //    MessageBox.Show("There was an issue loading the maze objects: " + mhpe.LastError, "ROM Load Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //}
                    
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

        private void OpenMaze( string fileName )
        {
            Cursor.Current = Cursors.WaitCursor;

            Application.DoEvents();
            try
            {
                MazeController mazeController =
                    new MazeController( fileName.ExpandAndDeserialize<Maze>() )
                    {
                        FileName = fileName
                    };
                
                if (!String.IsNullOrEmpty(SerializationExtensions.LastError))
                {
                    MessageBox.Show( SerializationExtensions.LastError, "Maze Load Issues", MessageBoxButtons.OK,
                        MessageBoxIcon.Information );
                }

                mazeController.Maze.AcceptChanges();

                TreeNode node = mazeController.TreeRender( treeView, null, toolStripButtonGrid.Checked );

                node.ImageIndex = ( (int)mazeController.Maze.MazeType ) + 1;
                node.SelectedImageIndex = node.ImageIndex;
                treeView.SelectedNode = node;

                // Only connect PropertyChanged event if the maze isn't part of a collection
                // (it doesn't have a parent) as it's fed through the collection otherwise.
                if ( node.Parent == null )
                {
                    mazeController.Maze.PropertyChanged += this.OnInstructionPropertyChanged;
                }

                mazeController.ShowGridReferences = Properties.Settings.Default.ShowGridReferences;
                mazeController.PropertyGrid = propertyGrid;
                mazeController.ComboBoxObjects = comboBoxMazeObjects;
            }
            catch ( Exception ex )
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show($@"Maze could not be opened: {(ex.InnerException != null ? ex.InnerException.Message : ex.Message)}",
                    "File Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                //Bryan, I put this here as an example of how to report Exceptions that are caught, but you still
                //may want to log them. All un-handled exceptions will still log.
                Program.SendException("MazeOpen", ex);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void OpenCollection( string fileName )
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Application.DoEvents();

                MazeCollectionController collectionController =
                    new MazeCollectionController( fileName.ExpandAndDeserialize<MazeCollection>() )
                    {
                        FileName = fileName
                    };

                if (!String.IsNullOrEmpty(SerializationExtensions.LastError))
                {
                    MessageBox.Show(SerializationExtensions.LastError, "Maze Load Issues", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }

                collectionController.MazeCollection.AcceptChanges();

                TreeNode node = collectionController.TreeRender( treeView, null, toolStripButtonGrid.Checked );
                node.ImageIndex = 0;
                node.SelectedImageIndex = node.ImageIndex;

                collectionController.MazeCollection.PropertyChanged += this.OnInstructionPropertyChanged;

                treeView.SelectedNode = node;
            }
            catch ( Exception ex )
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show($@"Maze Collection could not be opened: {(ex.InnerException != null ? ex.InnerException.Message : ex.Message)}",
                    "File Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private bool SaveCollection( MazeCollectionController mazeCollectionController )
        {
            DialogResult result = DialogResult.OK;

            // if there isn't a file associated with this MazeCollection then ask
            // for the fileName. 
            if ( string.IsNullOrWhiteSpace( mazeCollectionController.FileName ) )
            {
                SaveFileDialog sfd = new SaveFileDialog
                {
                    InitialDirectory = Environment.GetFolderPath(
                        Environment.SpecialFolder.MyDocuments ),
                    FileName = $"{mazeCollectionController.MazeCollection.Name}.mhc",
                    Filter = "Maze Files (*.mhc)|*.mhc|All files (*.*)|*.*",
                    AddExtension = true,
                    OverwritePrompt = true
                };

                // capture user choice for save operation below.
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

                    mazeCollectionController.MazeCollection.SerializeAndCompress(
                        mazeCollectionController.FileName );

                    mazeCollectionController.MazeCollection.AcceptChanges();

                    this.treeView.Invalidate();
                }
            }
            catch ( Exception ex )
            {
                result = DialogResult.Cancel;

                MessageBox.Show(
                    $@"An error has occurred while trying to save: {(ex.InnerException != null ? ex.InnerException.Message : ex.Message)}",
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

            // if there isn't a file associated with this MazeCollection then ask
            // for the fileName. 
            if ( string.IsNullOrWhiteSpace( mazeController.FileName ) )
            {
                SaveFileDialog sfd = new SaveFileDialog
                {
                    InitialDirectory = Environment.GetFolderPath(
                        Environment.SpecialFolder.MyDocuments ),
                    // Have FileName include parent collection if not already set.
                    FileName = mazeCollectionController != null ?
                        $"{mazeCollectionController.MazeCollection.Name}.{mazeController.Maze.Name}.mhz" :
                        $"{mazeController.Maze.Name}.mhz",
                    Filter = "Maze Files (*.mhz)|*.mhz|All files (*.*)|*.*",
                    AddExtension = true,
                    OverwritePrompt = true
                };

                // capture user choice for save operation below.
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

                    mazeController.Maze.SerializeAndCompress( mazeController.FileName );

                    mazeController.Maze.AcceptChanges();

                    this.treeView.Invalidate();
                }
            }
            catch ( Exception ex )
            {
                result = DialogResult.Cancel;

                MessageBox.Show(
                    $@"An error has occurred while trying to save: {(ex.InnerException != null ? ex.InnerException.Message : ex.Message)}",
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

        private void OnInstructionPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            if ( e.PropertyName.Equals( "Name" ) )
            {
                if ( treeView.SelectedNode?.Tag is MazeController mazeController )
                {
                    treeView.SelectedNode.Text = mazeController.Maze.Name;
                }
                else if ( treeView.SelectedNode?.Tag is MazeCollectionController mazeCollectionController )
                {
                    treeView.SelectedNode.Text = mazeCollectionController.MazeCollection.Name;
                }
            }

            this.treeView.Invalidate();
        }

        private void treeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F10)
            {
                MajorHavocPromisedEnd mhpe = new MajorHavocPromisedEnd();
                if (mhpe.LoadTemplate(Path.GetFullPath(Properties.Settings.Default.TemplatesLocation)))
                {
                    List<Tuple<Maze, int>> mazeInfo = new List<Tuple<Maze, int>>();
                    foreach (TreeNode node in treeView.SelectedNodes)
                    {
                        if (node.Tag is MazeController mazeController)
                        {
                            if (node.Parent?.Tag is MazeCollectionController mazeCollectionController)
                            {
                                int level = mazeCollectionController.MazeCollection.Mazes.IndexOf(mazeController.Maze);
                                mazeInfo.Add(new Tuple<Maze, int>(mazeController.Maze, level + 1));
                            }
                        }
                    }
                    string source = mhpe.ExtractSource(mazeInfo);
                    Clipboard.SetText(source);
                }
                else
                {
                    MessageBox.Show("There was an issue loading the maze objects: " + mhpe.LastError, "ROM Load Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void toolStripMenuItemValidate_Click( object sender, EventArgs e )
        {
            if ( this.treeView.SelectedNodes.Count == 1 )
            {
                if ( this.treeView.SelectedNode?.Tag is MazeController mazeController )
                {
                    mazeController.Maze.ValidateAndDisplayResults();
                }
                else if ( this.treeView.SelectedNode?.Tag is MazeCollectionController mazeCollectionController )
                {
                    mazeCollectionController.MazeCollection.ValidateAndDisplayResults();
                }
            }
            else
            {
                Collection mazesToValidate = new Collection();

                foreach ( TreeNode selectedNode in this.treeView.SelectedNodes )
                {
                    if ( selectedNode?.Tag is MazeController mazeController )
                    {
                        mazesToValidate.Add( mazeController.Maze );
                    }
                    else if ( selectedNode?.Tag is MazeCollectionController mazeCollectionController )
                    {
                        mazesToValidate.Add( mazeCollectionController.MazeCollection );
                    }
                }

                mazesToValidate.ValidateAndDisplayResults();
            }
        }
    }
}