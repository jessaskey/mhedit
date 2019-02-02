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
        private TreeNode _dragNode;

        #region Constructor

        /// <summary>
        /// Our default constructor for the application mainform
        /// </summary>
        public Mainform()
        {
            InitializeComponent();

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

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            RefreshTree();
        }

        public void RefreshTree()
        {
            toolStripButtonSaveCollection.Enabled = false;
            toolStripButtonSaveMaze.Enabled = false;
            //toolStripButtonConfiguration.Enabled = false;

            if (treeView.SelectedNode?.Tag != null)
            {
                if ( treeView.SelectedNode.Tag is MazeController mazeController )
                {
                    treeView.ContextMenu = null;
                    treeView.ContextMenu = mazeController.GetTreeContextMenu();

                    panelContent.Controls.Clear();
                    panelContent.Controls.Add( mazeController );
                    mazeController.Left = Math.Max( ( panelContent.Width - mazeController.Width ) / 2, 0 );
                    mazeController.Top = ( panelContent.Height - mazeController.Height ) / 2;


                    toolStripButtonMAME.Enabled = true;
                    mazeController.PropertyGrid = propertyGrid;
                    mazeController.ComboBoxObjects = comboBoxMazeObjects;
                    //show the maze properties on tree click
                    _currentMazeController = mazeController;
                    propertyGrid.SelectedObject = _currentMazeController;

                    toolStripButtonSaveMaze.Enabled = true;

                    if ( treeView.SelectedNode.Parent != null )
                    {
                        toolStripButtonSaveCollection.Enabled = true;
                        toolStripButtonConfiguration.Enabled = true;
                    }
                }
                else if ( treeView.SelectedNode.Tag.GetType() == typeof(MazeCollectionController) )
                {
                    propertyGrid.SelectedObject = treeView.SelectedNode.Tag;
                    toolStripButtonSaveCollection.Enabled = true;
                    toolStripButtonConfiguration.Enabled = true;
                }
            }
            else
            {
                panelContent.Controls.Clear();
            }
        }

        private void treeView_DragOver(object sender, DragEventArgs e)
        {
            Point Position = new Point(e.X, e.Y);
            Position = treeView.PointToClient(Position);
            TreeNode node = this.treeView.GetNodeAt(Position);
            if (node.Tag != null)
            {
                if (node.Tag.GetType() == typeof(Maze))
                {
                    e.Effect = DragDropEffects.Move;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
            base.OnDragOver(e);
        }

        private void treeView_DragDrop(object sender, DragEventArgs e)
        {
            Point Position = new Point(e.X, e.Y);
            Position = treeView.PointToClient(Position);
            TreeNode node = this.treeView.GetNodeAt(Position);
            if (node.Tag != null)
            {
                if (node.Tag.GetType() == typeof(Maze))
                {
                    Maze maze = (Maze)node.Tag;
                    Maze draggedmaze = (Maze)_dragNode.Tag;
                    if (maze.MazeType == draggedmaze.MazeType)
                    {
                        string msg = "Are you sure you want to replace '" + maze.Name + "' with '" + draggedmaze.Name + "'?";
                        DialogResult dr = MessageBox.Show(msg, MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes)
                        {
                            //get the parent collection of the target maze
                            if (node.Parent != null)
                            {
                                if (node.Parent.Tag != null)
                                {
                                    if (node.Parent.Tag.GetType() == typeof(MazeCollection))
                                    {
                                        MessageBox.Show("Maze dragging not supported.");
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
                    else
                    {
                        MessageBox.Show("'" + draggedmaze.Name + "' must be Maze " + maze.MazeType.ToString());
                    }
                }
            }
            base.OnDragOver(e);
        }

        private void treeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            _dragNode = (TreeNode)e.Item;
            //we can only drag maze objects..
            if (_dragNode.Tag != null)
            {
                if (_dragNode.Tag.GetType() == typeof(Maze))
                {
                    Maze maze = (Maze)_dragNode.Tag;
                    DoDragDrop(maze, DragDropEffects.Copy | DragDropEffects.Move);
                }
            }
        }


        #endregion

        #region Mainform Methods

        private void Mainform_Load( object sender, EventArgs e )
        {
            // https://stackoverflow.com/a/32561014
            if ( Properties.Settings.Default.IsMaximized )
                WindowState = FormWindowState.Maximized;
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

            try
            {
                foreach (TreeNode node in treeView.Nodes)
                {
                    if (node.Parent == null)
                    {
                        if (node.Tag != null)
                        {
                            if (node.Tag.GetType() == typeof(MazeCollectionController))
                            {

                                MazeCollectionController mazeCollectionController = node.Tag as MazeCollectionController;
                                if (mazeCollectionController.IsDirty)
                                {
                                    DialogResult dr = MessageBox.Show("Would you like to save changes to the '" + mazeCollectionController.Name + "' collection before closing?", "Confirm Exit", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                                    if (dr == DialogResult.Yes)
                                    {
                                        FileStream fStream = null;
                                        MemoryStream mStream = null;
                                        if (mazeCollectionController.FileName == null)
                                        {
                                            SaveFileDialog sd = new SaveFileDialog();
                                            sd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                                            sd.Filter = "Maze Files (*.mhc)|*.mhc|All files (*.*)|*.*";
                                            sd.AddExtension = true;
                                            sd.ShowDialog();
                                            mazeCollectionController.FileName = sd.FileName;
                                        }
                                        Cursor.Current = Cursors.WaitCursor;
                                        Application.DoEvents();
                                        try
                                        {
                                            MazeCollectionController.SerializeToFile(mazeCollectionController.MazeCollection, mazeCollectionController.FileName);
                                        }
                                        catch (Exception ex)
                                        {
                                            Cursor.Current = Cursors.Default;
                                            MessageBox.Show("Maze could not be saved: " + ex.Message, "File Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                        finally
                                        {
                                            if (mStream != null) mStream.Close();
                                            if (fStream != null) fStream.Close();
                                        }
                                        Cursor.Current = Cursors.Default;
                                    }
                                    else if (dr == DialogResult.Cancel)
                                    {
                                        //dont close
                                        e.Cancel = true;
                                        return;
                                    }
                                }
                            }
                            else if (node.GetType() == typeof(Maze))
                            {
                                MazeController mazeController = new MazeController((Maze)node.Tag);
                                if (mazeController.IsDirty)
                                {
                                    DialogResult dr = MessageBox.Show("Would you like to save changes to '" + mazeController.Name + " ' before closing?", "Confirm Exit", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                                    if (dr == DialogResult.Yes)
                                    {
                                        FileStream fStream = null;
                                        MemoryStream mStream = null;
                                        if (mazeController.FileName == null)
                                        {
                                            SaveFileDialog sd = new SaveFileDialog();
                                            sd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                                            sd.Filter = "Maze Files (*.mhz)|*.mhz|All files (*.*)|*.*";
                                            sd.AddExtension = true;
                                            sd.ShowDialog();
                                            mazeController.FileName = sd.FileName;
                                        }
                                        Cursor.Current = Cursors.WaitCursor;
                                        Application.DoEvents();
                                        try
                                        {
                                            MazeController.SerializeToFile(mazeController.Maze, mazeController.FileName);
                                        }
                                        catch (Exception ex)
                                        {
                                            Cursor.Current = Cursors.Default;
                                            MessageBox.Show("Maze could not be saved: " + ex.Message, "File Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                        finally
                                        {
                                            if (mStream != null) mStream.Close();
                                            if (fStream != null) fStream.Close();
                                        }
                                        Cursor.Current = Cursors.Default;
                                    }
                                    else if (dr == DialogResult.Cancel)
                                    {
                                        //dont close
                                        e.Cancel = true;
                                        return;
                                    }
                                }
                            }
     
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            e.Cancel = false;
        }

        #endregion

        #region ToolStrip Methods

        private void toolStripButtonNewCollection_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            MazeCollectionController collectionController = new MazeCollectionController(GetNewName("MazeCollection"));
            //assign callbacks
            //foreach (Maze maze in collection.Mazes)
            //{
            //    maze.OnMazePropertiesUpdated += new MazePropertiesUpdated(RefreshMazeName);
            //}
            for(int i = 0; i < 28; i++)
            {
                MazeType mazeType = (MazeType)(i & 0x03);
                Maze maze = new Maze((MazeType)mazeType, "Level " + (i + 1).ToString());
                collectionController.Mazes.Add(maze);
            }
            TreeNode node = collectionController.TreeRender(treeView, null, toolStripButtonGrid.Checked);
            node.ImageIndex = 0;
            node.SelectedImageIndex = node.ImageIndex;
            //Removed
            //collection.PropertyGrid = propertyGrid;
            treeView.SelectedNode = node;
            Cursor.Current = Cursors.Default;
        }

        private void toolStripButtonOpenCollection_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            od.CheckFileExists = true;
            od.Filter = "Maze Collection Files (*.mhc)|*.mhc|All files (*.*)|*.*";
            DialogResult dr = od.ShowDialog();
            if (dr == DialogResult.OK)
            {
                OpenCollection(od.FileName);
            }
        }

        private void OpenCollection(string fileName)
        {
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();
            MazeCollection mazeCollection = MazeCollectionController.DeserializeFromFile(fileName);
            MazeCollectionController mazeCollectionControl = new MazeCollectionController(mazeCollection);
            mazeCollectionControl.FileName = fileName;
            TreeNode node = treeView.Nodes.Add(mazeCollection.Name);
            node.Tag = mazeCollection;
            mazeCollectionControl.TreeRender(treeView, node, toolStripButtonGrid.Checked);
            node.ImageIndex = 0;
            node.SelectedImageIndex = node.ImageIndex;
            treeView.SelectedNode = node;
            _currentMazeCollectionController = mazeCollectionControl;
            Cursor.Current = Cursors.Default;
        }


        private void saveToolStripMenuItemSave_Click(object sender, EventArgs e)
        {
            SaveCollectionCommand();
        }

        private void toolStripButtonSaveCollection_Click(object sender, EventArgs e)
        {
            SaveCollectionCommand();
        }

        private void SaveCollectionCommand()
        {
            if (_currentMazeCollectionController != null)
            { 
                SaveCollection(_currentMazeCollectionController);
            }
            else
            {
                MessageBox.Show("Please select a maze collection to save.");
            }
        }

        private void SaveCollection(MazeCollectionController collectionController)
        {
            //MemoryStream mStream = null;
            if (collectionController.FileName == null)
            {
                SaveFileDialog sd = new SaveFileDialog();
                sd.FileName = collectionController.Name + ".mhc";
                sd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                sd.Filter = "Maze Collection Files (*.mhc)|*.mhc|All files (*.*)|*.*";
                sd.AddExtension = true;
                DialogResult result = sd.ShowDialog();
                if (result == DialogResult.Cancel) return;
                collectionController.FileName = sd.FileName;
            }
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();
            try
            {
                MazeCollectionController.SerializeToFile(collectionController.MazeCollection, collectionController.FileName);
                ////since we can serialize a collection, we need to create a .zip archive with 
                ////each maze included by a certain file name... in this case, we will use
                //// 0.mhz thru 15.mhz as our file names.
                //fStream = new FileStream(collectionController.FileName, FileMode.Create);
                //ZipOutputStream zip = new ZipOutputStream(fStream);
                //zip.SetLevel(5);
                //zip.SetComment("The Homeworld is Near!");
                //for (int i = 0; i < collectionController.Mazes.Count; i++)
                //{
                //    Maze currentMaze = collectionController.Mazes[i];
                //    MemoryStream mStream = new MemoryStream();
                //    BinaryFormatter b = new BinaryFormatter();
                //    b.Serialize(mStream, currentMaze);
                //    //System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(currentMaze.GetType());
                //    //x.Serialize(mStream, currentMaze);
                //    mStream.Position = 0;
                //    byte[] buffer = new byte[mStream.Length];
                //    mStream.Read(buffer, 0, buffer.Length);
                //    ZipEntry entry = new ZipEntry(i.ToString() + ".mhz");
                //    zip.PutNextEntry(entry);
                //    zip.Write(buffer, 0, buffer.Length);
                //}
                ////now, serialize the actual collection object into a .dat file
                //MemoryStream mStreamCollection = new MemoryStream();
                //BinaryFormatter bCollection = new BinaryFormatter();
                //bCollection.Serialize(mStreamCollection, collectionController.MazeCollection);
                //mStreamCollection.Position = 0;
                //byte[] bufferCollection = new byte[mStreamCollection.Length];
                //mStreamCollection.Read(bufferCollection, 0, bufferCollection.Length);
                //ZipEntry entryCollection = new ZipEntry("collection.dat");
                //zip.PutNextEntry(entryCollection);
                //zip.Write(bufferCollection, 0, bufferCollection.Length);
                //zip.Finish();
                //zip.Close();
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show("Maze could not be saved: " + ex.Message, "File Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                //if (mStream != null) mStream.Close();
                //if (fStream != null) fStream.Close();
            }
            Cursor.Current = Cursors.Default;
        }

        private void RefreshMazeName(object sender)
        {
            if (treeView.SelectedNode != null)
            {
                treeView.SelectedNode.Text = ((MazeController)sender).Name;
            }
        }

        private void toolStripButtonNewMaze_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Maze maze = new Maze(GetNewName("Maze"));
            MazeController mazeController = new MazeController(maze);
            mazeController.OnMazePropertiesUpdated += new MazePropertiesUpdated(RefreshMazeName);
            mazeController.ShowGridReferences = Properties.Settings.Default.ShowGridReferences;
            mazeController.PropertyGrid = propertyGrid;
            mazeController.ComboBoxObjects = comboBoxMazeObjects;
            TreeNode node = mazeController.TreeRender(treeView, null, toolStripButtonGrid.Checked);
            node.ImageIndex = ((int)maze.MazeType) + 1;
            node.SelectedImageIndex = node.ImageIndex;
            treeView.SelectedNode = node;
            _currentMazeController = mazeController;
            Cursor.Current = Cursors.Default;
        }

        private void toolStripButtonOpenMaze_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            od.CheckFileExists = true;
            od.Filter = "Maze Files (*.mhz)|*.mhz|All files (*.*)|*.*";
            DialogResult dr = od.ShowDialog();
            if (dr == DialogResult.OK)
            {
                OpenMaze(od.FileName);
            }
        }

        private void OpenMaze(string fileName)
        {
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();
            try
            {
                MazeController mazeController = new MazeController(MazeController.DeserializeFromFile(fileName));
                TreeNode node = treeView.Nodes.Add(mazeController.Maze.Name);
                node.Tag = mazeController;
                node.ImageIndex = ((int)mazeController.Maze.MazeType) + 1;
                node.SelectedImageIndex = node.ImageIndex;
                treeView.SelectedNode = node;
                mazeController.FileName = fileName;
                mazeController.TreeRender(treeView, node, toolStripButtonGrid.Checked);
                mazeController.PropertyGrid = propertyGrid;
                mazeController.ComboBoxObjects = comboBoxMazeObjects;
                _currentMazeController = mazeController;
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show("Maze could not be opened: " + ex.Message, "File Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Cursor.Current = Cursors.Default;
        }

        private void toolStripButtonSaveMaze_Click(object sender, EventArgs e)
        {
            if (_currentMazeController != null)
            { 
                SaveMaze(_currentMazeController);
            }
        }

        private void SaveMaze(MazeController mazeController)
        {
            if (String.IsNullOrEmpty(mazeController.FileName))
            {
                SaveFileDialog sd = new SaveFileDialog();
                sd.FileName = mazeController.Name + ".mhz";
                sd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                sd.Filter = "Maze Files (*.mhz)|*.mhz|All files (*.*)|*.*";
                sd.AddExtension = true;
                DialogResult result = sd.ShowDialog();
                if (result == DialogResult.Cancel) return;
                mazeController.FileName = sd.FileName;
            }
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();
            try
            {
                MazeController.SerializeToFile(mazeController.Maze, mazeController.FileName);
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show("Maze could not be saved: " + ex.Message, "File Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                mazeController.IsDirty = false;
            }
            Cursor.Current = Cursors.Default;

        }

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

        private void CloseMaze(MazeController mazeController)
        {
            if (mazeController.IsDirty)
            {
                SaveMaze(mazeController);
            }
        }

        private void CloseCollection(MazeCollectionController collectionController)
        {
            if (collectionController.IsDirty)
            {
                SaveCollection(collectionController);
            }
        }
        
        private void closeToolStripMenuItemClose_Click(object sender, EventArgs e)
        {
            if (_currentMazeCollectionController != null)
            {
                CloseCollection(_currentMazeCollectionController);
                treeView.SelectedNode.Remove();
                RefreshTree();
            }
            else if (_currentMazeController != null)
            {
                CloseMaze(_currentMazeController);
                treeView.SelectedNode.Remove();
                RefreshTree();
            }
        }

        #region TreeRightMenu Methods
        private void saveMazeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentMazeController != null)
            {
                SaveMaze(_currentMazeController);
                _currentMazeController.FileName = null;
            }
        }

        private void contextMenuStripTree_Opening(object sender, CancelEventArgs e)
        {
            //if (treeView.SelectedNode != null)
            //{
            //    if (treeView.SelectedNode.Tag != null)
            //    {
            //        if (treeView.SelectedNode.Parent == null)
            //        {
            //            if (treeView.SelectedNode.Tag.GetType() == typeof(Maze))
            //            {
            //                Maze maze = (Maze)treeView.SelectedNode.Tag;
            //                saveToolStripMenuItemSave.Enabled = maze.IsDirty;
            //            }
            //        }
            //        else
            //        {
            //            if (treeView.SelectedNode.Parent.Tag.GetType() == typeof(MazeCollection))
            //            {
            //                MazeCollection collection = (MazeCollection)treeView.SelectedNode.Parent.Tag;
            //                saveToolStripMenuItemSave.Enabled = collection.IsDirty;
            //                saveToolStripMenuItemSave.Text = "Save Collection";
            //            }
            //        }
            //    }
            //}
        }

        #endregion


        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //if (treeView.SelectedNode.Tag.GetType() == typeof(Maze))
            //{
            //    e.Node.BeginEdit();
            //}

        }

        private void treeView_MouseUp( object sender, MouseEventArgs args )
        {
            if ( args.Button == MouseButtons.Right )
            {
                TreeNode node = treeView.GetNodeAt( args.Location );

                if ( node != null && node.Bounds.Contains( args.Location ) )
                {
                    treeView.SelectedNode = node;

                    if ( node.Parent == null )
                    {
                        if ( treeView.SelectedNode.Tag.GetType() == typeof( MazeController ) )
                        {
                            saveToolStripMenuItemSave.Text = "Save Maze";
                            closeToolStripMenuItemClose.Text = "Close Maze";
                            saveMazeToolStripMenuItem.Visible = false;
                        }
                        if ( treeView.SelectedNode.Tag.GetType() == typeof( MazeCollectionController ) )
                        {
                            saveToolStripMenuItemSave.Text = "Save Collection";
                            closeToolStripMenuItemClose.Text = "Close Collection";
                            saveMazeToolStripMenuItem.Enabled = false;
                            saveMazeToolStripMenuItem.Visible = true;
                        }
                    }
                    else
                    {
                        saveToolStripMenuItemSave.Text = "Save Collection";
                        closeToolStripMenuItemClose.Text = "Close Collection";
                        saveMazeToolStripMenuItem.Enabled = true;
                        saveMazeToolStripMenuItem.Visible = true;
                    }

                    contextMenuStripTree.Show( treeView, args.Location );
                }
            }
        }

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

        private void toolStripButtonAnimate_Click(object sender, EventArgs e)
        {
            if (_currentMazeCollectionController != null && _currentMazeController != null)
            {
                if (MAMEHelper.SaveROM(_currentMazeCollectionController.MazeCollection, _currentMazeController.Maze))
                {
                    try
                    {
                        //now launch MAME for mhavoc..
                        string mameExe = Path.GetFullPath(Properties.Settings.Default.MameExecutable);

                        string args = "";

                        if (Properties.Settings.Default.MameDebug)
                        {
                            args += "-debug ";
                        }

                        if (Properties.Settings.Default.MameWindow)
                        {
                            args += "-window ";
                        }

                        args += Properties.Settings.Default.MameDriver;

                        ProcessStartInfo info = new ProcessStartInfo(mameExe, args);
                        info.ErrorDialog = true;
                        info.RedirectStandardError = true;
                        info.RedirectStandardOutput = true;
                        info.UseShellExecute = false;
                        info.WorkingDirectory = Path.GetDirectoryName(mameExe);

                        Process p = new Process();
                        p.EnableRaisingEvents = true;
                        p.StartInfo = info;
                        p.Exited += new EventHandler(ProcessExited);
                        p.Start();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("There was an error launching MAME, verify your MAME paths in the configuration." + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("The current maze is not loaded correctly. This is most likely an issue with the application. Please contact Jess.");
            }
        }

        private void ProcessExited(object sender, EventArgs e)
        {
            if (sender is Process)
            {
                Process p = (Process)sender;
                if (p.ExitCode != 0)
                {
                    if (p.StandardError != null)
                    {
                        string errorResult = p.StandardError.ReadToEnd();
                        if (!String.IsNullOrEmpty(errorResult))
                        {
                            MessageBox.Show(errorResult, "MAME Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            //put back the ROM's from backup
            string mamePath = Path.GetDirectoryName(Properties.Settings.Default.MameExecutable) + "\\roms\\" + Properties.Settings.Default.MameDriver + "\\";
            string backupPath = mamePath + "_backup\\";
            
            foreach(string file in Directory.GetFiles(backupPath))
            {
                File.Copy(file, mamePath + Path.GetFileName(file), true);
            }
        }

        private void toolStripButtonConfiguration_Click(object sender, EventArgs e)
        {
            DialogConfiguration d = new DialogConfiguration();
            d.ShowDialog();
        }

        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if ( e.Node.Tag is MazeController controller )
            {
                controller.Name = e.Label;
                propertyGrid.Refresh();
            }
            else if ( e.Node.Tag is MazeCollectionController collectionController )
            {
                collectionController.Name = e.Label;
                propertyGrid.Refresh();
            }
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
    }
}