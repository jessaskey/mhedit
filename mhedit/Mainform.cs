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
using Silver.UI;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Zip;

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
        private Maze _currentMaze = null;
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
            //create our image lists...
            ImageList toolboxImageList = new ImageList();
            Size iconSize = new Size(32,32);
            toolboxImageList.ImageSize = iconSize;
            toolboxImageList.Images.Add(Resource.GetResourceImage("images.buttons.wall_horizontal_32.ico"));
            toolboxImageList.Images.Add(Resource.GetResourceImage("images.buttons.wall_leftup_32.ico"));
            toolboxImageList.Images.Add(Resource.GetResourceImage("images.buttons.wall_rightup_32.ico"));
            toolboxImageList.Images.Add(Resource.GetResourceImage("images.buttons.wall_leftdown_32.ico"));
            toolboxImageList.Images.Add(Resource.GetResourceImage("images.buttons.wall_rightdown_32.ico"));
            toolboxImageList.Images.Add(Resource.GetResourceImage("images.buttons.wall_vertical_32.ico"));
            toolboxImageList.Images.Add(Resource.GetResourceImage("images.buttons.wall_empty_32.ico"));
            toolboxImageList.Images.Add(Resource.GetResourceImage("images.buttons.pyroid_32.ico"));
            toolboxImageList.Images.Add(Resource.GetResourceImage("images.buttons.reactoid_32.ico"));
            toolboxImageList.Images.Add(Resource.GetResourceImage("images.buttons.perkoid_32.ico"));
            toolboxImageList.Images.Add(Resource.GetResourceImage("images.buttons.roboid_32.ico"));
            toolboxImageList.Images.Add(Resource.GetResourceImage("images.buttons.oxoid_32.ico"));
            toolboxImageList.Images.Add(Resource.GetResourceImage("images.buttons.key_32.ico"));
            toolboxImageList.Images.Add(Resource.GetResourceImage("images.buttons.arrow_32.ico"));
            toolboxImageList.Images.Add(Resource.GetResourceImage("images.buttons.lightning_32.ico"));
            toolboxImageList.Images.Add(Resource.GetResourceImage("images.buttons.lightning_v_32.ico"));
            toolboxImageList.Images.Add(Resource.GetResourceImage("images.buttons.clock_32.ico"));
            toolboxImageList.Images.Add(Resource.GetResourceImage("images.buttons.cannon_32.ico"));
            toolboxImageList.Images.Add(Resource.GetResourceImage("images.buttons.oneway_32.ico"));
            toolboxImageList.Images.Add(Resource.GetResourceImage("images.buttons.trippad_32.ico"));
            toolboxImageList.Images.Add(Resource.GetResourceImage("images.buttons.lock_32.ico"));
            toolboxImageList.Images.Add(Resource.GetResourceImage("images.buttons.hand_32.ico"));
            toolboxImageList.Images.Add(Resource.GetResourceImage("images.buttons.spikes_32.ico"));
            toolboxImageList.Images.Add(Resource.GetResourceImage("images.buttons.booties_32.ico"));
            toolboxImageList.Images.Add(Resource.GetResourceImage("images.buttons.transporter_32.ico"));
            toolboxImageList.Images.Add(Resource.GetResourceImage("images.buttons.pod_32.ico"));
            toolboxImageList.Images.Add(Resource.GetResourceImage("images.buttons.roboid_32.ico"));
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
            itemIndex = toolBox[tabIndex].AddItem("Pyroid", 7, true, new MazeEnemies.Pyroid());
            itemIndex = toolBox[tabIndex].AddItem("Perkoid", 9, true, new MazeEnemies.Perkoid());
            itemIndex = toolBox[tabIndex].AddItem("Maxoid", 26, true, new MazeEnemies.Maxoid());
            itemIndex = toolBox[tabIndex].AddItem("Force Field", 14, true, new MazeEnemies.LightningH());
            itemIndex = toolBox[tabIndex].AddItem("Force Field", 15, true, new MazeEnemies.LightningV());
            itemIndex = toolBox[tabIndex].AddItem("Ion Cannon", 17, true, new MazeEnemies.Cannon());
            itemIndex = toolBox[tabIndex].AddItem("Trip Pad", 19, true, new MazeEnemies.TripPad());
            //toolBox[tabIndex].AddItem("Roboid", 10, true, null);
            tabIndex = toolBox.AddTab("Maze Objects", -1);
            itemIndex = toolBox[tabIndex].AddItem("Reactoid", 8, true, new MazeObjects.Reactoid());
            itemIndex = toolBox[tabIndex].AddItem("Arrow", 13, true, new MazeObjects.Arrow());
            itemIndex = toolBox[tabIndex].AddItem("Oxoid", 11, true, new MazeObjects.Oxoid());
            itemIndex = toolBox[tabIndex].AddItem("One Way", 18, true, new MazeObjects.OneWay());
            itemIndex = toolBox[tabIndex].AddItem("Stalactites", 22, true, new MazeObjects.Spikes());
            itemIndex = toolBox[tabIndex].AddItem("Transporter", 24, true, new MazeObjects.Transporter());
            itemIndex = toolBox[tabIndex].AddItem("Booties", 23, true, new MazeObjects.Boots());
            itemIndex = toolBox[tabIndex].AddItem("Lock", 20, true, new MazeObjects.Lock());
            itemIndex = toolBox[tabIndex].AddItem("Key", 12, true, new MazeObjects.Key());
            itemIndex = toolBox[tabIndex].AddItem("De Hand", 21, true, new MazeObjects.Hand());
            itemIndex = toolBox[tabIndex].AddItem("Clock", 16, true, new MazeObjects.Clock());
            itemIndex = toolBox[tabIndex].AddItem("Escape Pod", 25, true, new MazeObjects.EscapePod());

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

            if (treeView.SelectedNode != null)
            {
                if (treeView.SelectedNode.Tag != null)
                {
                    switch (treeView.SelectedNode.Tag.GetType().ToString())
                    {
                        case "mhedit.Maze":
                            Maze maze = (Maze)treeView.SelectedNode.Tag;
                            if (maze != null)
                            {
                                treeView.ContextMenu = null;
                                treeView.ContextMenu = maze.GetTreeContextMenu();
                                propertyGrid.SelectedObject = maze;
                                panelContent.Controls.Clear();
                                panelContent.Controls.Add(maze);
                                maze.Left = Math.Max((panelContent.Width - maze.Width) / 2, 0);
                                maze.Top = (panelContent.Height - maze.Height) / 2;
                            }
                            toolStripButtonSaveMaze.Enabled = true;
                            if (treeView.SelectedNode.Parent != null)
                            {
                                toolStripButtonSaveCollection.Enabled = true;
                                toolStripButtonConfiguration.Enabled = true;
                            }
                            break;
                        case "mhedit.MazeCollection":
                            MazeCollection collection = (MazeCollection)treeView.SelectedNode.Tag;
                            propertyGrid.SelectedObject = collection;
                            toolStripButtonSaveCollection.Enabled = true;
                            toolStripButtonConfiguration.Enabled = true;
                            break;
                    }
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
                                        MazeCollection collection = (MazeCollection)node.Parent.Tag;
                                        int mazeindex = collection.FindMaze(maze);
                                        if (mazeindex > -1)
                                        {
                                            collection.InsertMaze(mazeindex, draggedmaze);
                                            collection.TreeRender(treeView, node.Parent);
                                        }
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

        private void Mainform_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                foreach (TreeNode node in treeView.Nodes)
                {
                    if (node.Parent == null)
                    {
                        if (node.Tag != null)
                        {
                            switch (node.Tag.GetType().ToString())
                            {
                                case "mhedit.MazeCollection":
                                    MazeCollection collection = (MazeCollection)node.Tag;
                                    if (collection.IsDirty)
                                    {
                                        DialogResult dr = MessageBox.Show("Would you like to save changes to the '" + collection.Name + "' collection before closing?", "Confirm Exit", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                                        if (dr == DialogResult.Yes)
                                        {
                                            FileStream fStream = null;
                                            MemoryStream mStream = null;
                                            if (collection.FileName == null)
                                            {
                                                SaveFileDialog sd = new SaveFileDialog();
                                                sd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                                                sd.Filter = "Maze Files (*.mhc)|*.mhc|All files (*.*)|*.*";
                                                sd.AddExtension = true;
                                                sd.ShowDialog();
                                                collection.FileName = sd.FileName;
                                            }
                                            Cursor.Current = Cursors.WaitCursor;
                                            Application.DoEvents();
                                            try
                                            {
                                                using (fStream = new FileStream(collection.FileName, FileMode.Create))
                                                {
                                                    using (mStream = new MemoryStream())
                                                    {
                                                        BinaryFormatter b = new BinaryFormatter();
                                                        b.Serialize(mStream, collection);
                                                        mStream.Position = 0;
                                                        BZip2.Compress(mStream, fStream, true, 4096);
                                                    }
                                                }
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
                                    break;
                                case "mhedit.Maze":
                                    Maze maze = (Maze)node.Tag;
                                    if (maze.IsDirty)
                                    {
                                        DialogResult dr = MessageBox.Show("Would you like to save changes to '" + maze.Name + " ' before closing?", "Confirm Exit", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                                        if (dr == DialogResult.Yes)
                                        {
                                            FileStream fStream = null;
                                            MemoryStream mStream = null;
                                            if (maze.FileName == null)
                                            {
                                                SaveFileDialog sd = new SaveFileDialog();
                                                sd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                                                sd.Filter = "Maze Files (*.mhz)|*.mhz|All files (*.*)|*.*";
                                                sd.AddExtension = true;
                                                sd.ShowDialog();
                                                maze.FileName = sd.FileName;
                                            }
                                            Cursor.Current = Cursors.WaitCursor;
                                            Application.DoEvents();
                                            try
                                            {
                                                using (fStream = new FileStream(maze.FileName, FileMode.Create))
                                                {
                                                    using (mStream = new MemoryStream())
                                                    {
                                                        BinaryFormatter b = new BinaryFormatter();
                                                        b.Serialize(mStream, maze);
                                                        mStream.Position = 0;
                                                        BZip2.Compress(mStream, fStream, true, 4096);
                                                    }
                                                }
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
                                    break;
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

        #region PropertyGrid Methods

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            PropertyGrid grid = (PropertyGrid)s;
            if (treeView.SelectedNode != null)
            {
                if (treeView.SelectedNode.Tag != null)
                {
                    switch (grid.SelectedObject.GetType().ToString())
                    {
                        case "mhedit.MazeCollection":
                        case "mhedit.Maze":
                            ((ITreeObject)grid.SelectedObject).TreeRender(treeView, treeView.SelectedNode);
                            break;
                        default:
                            //zoomPanImageBox.Image = ((ITreeObject)treeView.SelectedNode.Tag).GetImage();
                            //zoomPanImageBox.Invalidate();
                            break;
                    }
                }
            }
        }

        #endregion

        #region ToolStrip Methods

        private void toolStripButtonNewCollection_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            MazeCollection collection = new MazeCollection(GetNewName("MazeCollection"));
            TreeNode node = collection.TreeRender(treeView, null);
            node.ImageIndex = 0;
            node.SelectedImageIndex = node.ImageIndex;
            collection.PropertyGrid = propertyGrid;
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
            FileStream fStream = null;
            Maze[] mazes = new Maze[16];
            Application.DoEvents();
            MazeCollection mazeCollection = null;
            //try
            //{
            fStream = new FileStream(fileName, FileMode.Open);

            ZipInputStream zip = new ZipInputStream(fStream);
            ZipEntry entry;
            while ((entry = zip.GetNextEntry()) != null)
            {
                MemoryStream mStream;
                switch (Path.GetExtension(entry.Name))
                {
                    case ".mhz":
                        mStream = new MemoryStream();
                        Byte[] buffer = new Byte[2048];
                        int size = 2048;
                        while (true)
                        {
                            size = zip.Read(buffer, 0, buffer.Length);
                            if (size > 0)
                            {
                                mStream.Write(buffer, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                        //memorystream has our maze objects..
                        mStream.Position = 0;
                        BinaryFormatter b = new BinaryFormatter();
                        Maze maze = (Maze)b.Deserialize(mStream);
                        long i = zip.Position;
                        string filename = Path.GetFileNameWithoutExtension(entry.Name);
                        int mazeIndex = int.Parse(filename);
                        mazes[mazeIndex] = maze;
                        break;
                    case ".dat":
                        MemoryStream mStreamCollection = new MemoryStream();
                        Byte[] bufferCollection = new Byte[2048];
                        int sizeCollection = 2048;
                        while (true)
                        {
                            sizeCollection = zip.Read(bufferCollection, 0, bufferCollection.Length);
                            if (sizeCollection > 0)
                            {
                                mStreamCollection.Write(bufferCollection, 0, sizeCollection);
                            }
                            else
                            {
                                break;
                            }
                        }
                        //memorystream has our maze objects..
                        mStreamCollection.Position = 0;
                        BinaryFormatter bCollection = new BinaryFormatter();
                        mazeCollection = (MazeCollection)bCollection.Deserialize(mStreamCollection);
                        break;
                }
            }
            zip.Close();

            mazeCollection.Mazes = mazes;
            mazeCollection.FileName = fileName;
            TreeNode node = treeView.Nodes.Add(mazeCollection.Name);
            node.Tag = mazeCollection;
            mazeCollection.TreeRender(treeView, node);
            node.ImageIndex = 0;
            node.SelectedImageIndex = node.ImageIndex;
            treeView.SelectedNode = node;
            mazeCollection.PropertyGrid = propertyGrid;
            //}
            //catch (Exception ex)
            //{
            //    Cursor.Current = Cursors.Default;
            //    MessageBox.Show("Maze could not be opened: " + ex.Message, "File Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            //finally
            //{
            //    //if (mStream != null) mStream.Close();
            //    if (fStream != null) fStream.Close();
            //}
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
            if (treeView.SelectedNode != null)
            {
                if (treeView.SelectedNode.Tag != null)
                {
                    MazeCollection collection = null;
                    if (treeView.SelectedNode.Tag.GetType().ToString() == "mhedit.MazeCollection")
                    {
                        collection = (MazeCollection)treeView.SelectedNode.Tag;
                    }
                    if (treeView.SelectedNode.Parent != null)
                    {
                        if (treeView.SelectedNode.Parent.Tag.GetType().ToString() == "mhedit.MazeCollection")
                        {
                            collection = (MazeCollection)treeView.SelectedNode.Parent.Tag;
                        }
                    }
                    if (collection != null)
                    {
                        SaveCollection(collection);
                    }
                    else
                    {
                        MessageBox.Show("Please select a maze collection to save.");
                    }
                }
            }
        }

        private void SaveCollection(MazeCollection collection)
        {
            FileStream fStream = null;
            //MemoryStream mStream = null;
            if (collection.FileName == null)
            {
                SaveFileDialog sd = new SaveFileDialog();
                sd.FileName = collection.Name + ".mhc";
                sd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                sd.Filter = "Maze Collection Files (*.mhc)|*.mhc|All files (*.*)|*.*";
                sd.AddExtension = true;
                DialogResult result = sd.ShowDialog();
                if (result == DialogResult.Cancel) return;
                collection.FileName = sd.FileName;
            }
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();
            try
            {
                //since we can serialize a collection, we need to create a .zip archive with 
                //each maze included by a certain file name... in this case, we will use
                // 0.mhz thru 15.mhz as our file names.
                fStream = new FileStream(collection.FileName, FileMode.Create);
                ZipOutputStream zip = new ZipOutputStream(fStream);
                zip.SetLevel(5);
                zip.SetComment("The Homeworld is Near!");
                for (int i = 0; i < collection.MazeCount; i++)
                {
                    Maze currentMaze = collection.Mazes[i];
                    MemoryStream mStream = new MemoryStream();
                    BinaryFormatter b = new BinaryFormatter();
                    b.Serialize(mStream, currentMaze);
                    //System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(currentMaze.GetType());
                    //x.Serialize(mStream, currentMaze);
                    mStream.Position = 0;
                    byte[] buffer = new byte[mStream.Length];
                    mStream.Read(buffer, 0, buffer.Length);
                    ZipEntry entry = new ZipEntry(i.ToString() + ".mhz");
                    zip.PutNextEntry(entry);
                    zip.Write(buffer, 0, buffer.Length);
                }
                //now, serialize the actual collection object into a .dat file
                MemoryStream mStreamCollection = new MemoryStream();
                BinaryFormatter bCollection = new BinaryFormatter();
                bCollection.Serialize(mStreamCollection, collection);
                mStreamCollection.Position = 0;
                byte[] bufferCollection = new byte[mStreamCollection.Length];
                mStreamCollection.Read(bufferCollection, 0, bufferCollection.Length);
                ZipEntry entryCollection = new ZipEntry("collection.dat");
                zip.PutNextEntry(entryCollection);
                zip.Write(bufferCollection, 0, bufferCollection.Length);
                zip.Finish();
                zip.Close();
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show("Maze could not be saved: " + ex.Message, "File Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                //if (mStream != null) mStream.Close();
                if (fStream != null) fStream.Close();
            }
            Cursor.Current = Cursors.Default;
        }

        private void toolStripButtonNewMaze_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Maze maze = new Maze(GetNewName("Maze"));
            TreeNode node = maze.TreeRender(treeView, null);
            node.ImageIndex = ((int)maze.MazeType) + 1;
            node.SelectedImageIndex = node.ImageIndex;
            maze.PropertyGrid = propertyGrid;
            maze.ComboBoxObjects = comboBoxMazeObjects;
            treeView.SelectedNode = node;
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
            FileStream fStream = null;
            MemoryStream mStream = null;
            Application.DoEvents();
            try
            {
                using (fStream = new FileStream(fileName, FileMode.Open))
                {
                    using (mStream = new MemoryStream())
                    {
                        BZip2.Decompress(fStream, mStream, true);
                        mStream.Position = 0;
                        BinaryFormatter b = new BinaryFormatter();
                        Maze maze = (Maze)b.Deserialize(mStream);
                        maze.FileName = fileName;
                        TreeNode node = treeView.Nodes.Add(maze.Name);
                        node.Tag = maze;
                        treeView.SelectedNode = node;
                        maze.TreeRender(treeView, node);
                        node.ImageIndex = ((int)maze.MazeType) + 1;
                        node.SelectedImageIndex = node.ImageIndex;
                        maze.PropertyGrid = propertyGrid;
                        maze.ComboBoxObjects = comboBoxMazeObjects;
                    }
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show("Maze could not be opened: " + ex.Message, "File Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (mStream != null) mStream.Close();
                if (fStream != null) fStream.Close();
            }
            Cursor.Current = Cursors.Default;
        }

        private void toolStripButtonSaveMaze_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                if (treeView.SelectedNode.Tag != null)
                {
                    if (treeView.SelectedNode.Tag.GetType().ToString() == "mhedit.Maze")
                    {
                        Maze maze = (Maze)treeView.SelectedNode.Tag;
                        SaveMaze(maze);
                    }
                }
            }
        }

        private void SaveSelectedNode()
        {
            if (treeView.SelectedNode != null)
            {
                if (treeView.SelectedNode.Tag != null)
                {
                    if (treeView.SelectedNode.Tag.GetType().ToString() == "mhedit.Maze")
                    {
                        Maze maze = (Maze)treeView.SelectedNode.Tag;
                        SaveMaze(maze);
                    }
                    if (treeView.SelectedNode.Tag.GetType().ToString() == "mhedit.MazeCollection")
                    {
                        MazeCollection collection = (MazeCollection)treeView.SelectedNode.Tag;
                        SaveCollection(collection);
                    }
                }
            }
        }

        private void SaveMaze(Maze maze)
        {
            FileStream fStream = null;
            MemoryStream mStream = null;
            if (maze.FileName == null)
            {
                SaveFileDialog sd = new SaveFileDialog();
                sd.FileName = maze.Name + ".mhz";
                sd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                sd.Filter = "Maze Files (*.mhz)|*.mhz|All files (*.*)|*.*";
                sd.AddExtension = true;
                DialogResult result = sd.ShowDialog();
                if (result == DialogResult.Cancel) return;
                maze.FileName = sd.FileName;
            }
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();
            try
            {
                using (fStream = new FileStream(maze.FileName, FileMode.Create))
                {
                    using (mStream = new MemoryStream())
                    {
                        BinaryFormatter b = new BinaryFormatter();
                        b.Serialize(mStream, maze);
                        mStream.Position = 0;
                        BZip2.Compress(mStream, fStream, true, 4096);
                    }
                }
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
                maze.IsDirty = false;
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
            if (treeView.SelectedNode != null)
            {
                if (treeView.SelectedNode.Tag != null)
                {
                    if (treeView.SelectedNode.Tag.GetType() == typeof(Maze))
                    {
                        Maze maze = (Maze)treeView.SelectedNode.Tag;
                        maze.Zoom += .1M;
                        panelContent.Invalidate();
                    }
                }
            }
        }

        private void toolStripButtonZoomOut_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                if (treeView.SelectedNode.Tag != null)
                {
                    if (treeView.SelectedNode.Tag.GetType() == typeof(Maze))
                    {
                        Maze maze = (Maze)treeView.SelectedNode.Tag;
                        maze.Zoom -= .1M;
                        panelContent.ClientSize = new Size((int)(panelContent.Width * maze.Zoom), (int)(panelContent.Height * maze.Zoom));
                        panelContent.Invalidate();
                    }
                }
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

        private void CloseMaze(Maze maze)
        {
            if (maze.IsDirty)
            {
                SaveMaze(maze);
            }
        }

        private void CloseCollection(MazeCollection collection)
        {
            if (collection.IsDirty)
            {
                SaveCollection(collection);
            }
        }
        
        private void closeToolStripMenuItemClose_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                if (treeView.SelectedNode.Tag != null)
                {
                    if (treeView.SelectedNode.Parent == null)
                    {
                        if (treeView.SelectedNode.Tag.GetType().ToString() == "mhedit.Maze")
                        {
                            Maze maze = (Maze)treeView.SelectedNode.Tag;
                            CloseMaze(maze);
                            treeView.SelectedNode.Remove();
                            RefreshTree();
                        }
                        else if (treeView.SelectedNode.Tag.GetType().ToString() == "mhedit.MazeCollection")
                        {
                            MazeCollection collection = (MazeCollection)treeView.SelectedNode.Tag;
                            CloseCollection(collection);
                            treeView.SelectedNode.Remove();
                            RefreshTree();
                        }
                    }
                    else
                    {
                        //this node has a parent... which should be a collection, so save that
                        if (treeView.SelectedNode.Parent.Tag.GetType().ToString() == "mhedit.MazeCollection")
                        {
                            MazeCollection collection = (MazeCollection)treeView.SelectedNode.Parent.Tag;
                            CloseCollection(collection);
                            treeView.SelectedNode.Parent.Remove();
                            RefreshTree();
                        }
                    }
                }
            }
        }

        #region TreeRightMenu Methods
        private void saveMazeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                if (treeView.SelectedNode.Tag != null)
                {
                    if (treeView.SelectedNode.Tag.GetType().ToString() == "mhedit.Maze")
                    {
                        Maze maze = (Maze)treeView.SelectedNode.Tag;
                        SaveMaze(maze);
                        maze.FileName = null;
                    }
                }
            }
        }

        private void contextMenuStripTree_Opening(object sender, CancelEventArgs e)
        {
            return;
            if (treeView.SelectedNode != null)
            {
                if (treeView.SelectedNode.Tag != null)
                {
                    if (treeView.SelectedNode.Parent == null)
                    {
                        if (treeView.SelectedNode.Tag.GetType() == typeof(Maze))
                        {
                            Maze maze = (Maze)treeView.SelectedNode.Tag;
                            saveToolStripMenuItemSave.Enabled = maze.IsDirty;
                        }
                    }
                    else
                    {
                        if (treeView.SelectedNode.Parent.Tag.GetType() == typeof(MazeCollection))
                        {
                            MazeCollection collection = (MazeCollection)treeView.SelectedNode.Parent.Tag;
                            saveToolStripMenuItemSave.Enabled = collection.IsDirty;
                            saveToolStripMenuItemSave.Text = "Save Collection";
                        }
                    }
                }
            }
        }

        #endregion


        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //if (treeView.SelectedNode.Tag.GetType() == typeof(Maze))
            //{
            //    e.Node.BeginEdit();
            //}

        }

        private void treeView_Click(object sender, EventArgs e)
        {
            MouseEventArgs args = (MouseEventArgs)e;

            if (args.Button == MouseButtons.Right)
            {
                TreeNode node = treeView.GetNodeAt(new Point(args.X, args.Y));
                if (node != null)
                {
                    treeView.SelectedNode = node;
                    if (node.Parent == null)
                    {
                        if (treeView.SelectedNode.Tag.GetType() == typeof(Maze))
                        {
                            saveToolStripMenuItemSave.Text = "Save Maze";
                            closeToolStripMenuItemClose.Text = "Close Maze";
                            saveMazeToolStripMenuItem.Visible = false;
                        }
                        if (treeView.SelectedNode.Tag.GetType() == typeof(MazeCollection))
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
                    contextMenuStripTree.Show(new Point(args.X, args.Y), ToolStripDropDownDirection.BelowRight);
                }
            }
            else if (args.Button == MouseButtons.Left)
            {
                //typical selection
                TreeNode node = treeView.GetNodeAt(new Point(args.X, args.Y));
                if (node != null)
                {
                    treeView.SelectedNode = node;

                    if (treeView.SelectedNode.Tag.GetType() == typeof(Maze))
                    {
                        toolStripButtonMAME.Enabled = true;
                        _currentMaze = (Maze)treeView.SelectedNode.Tag;
                        _currentMaze.PropertyGrid = propertyGrid;
                        _currentMaze.ComboBoxObjects = comboBoxMazeObjects;
                        return;
                    }
                }
                _currentMaze = null;
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
            if (_currentMaze != null)
            {
                if (CreateRom())
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
                        MessageBox.Show("There was an error launching MAME, verify your MAME paths in the configuration." + ex.Message );
                    }
                }
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
        }

        private bool CreateRom()
        {
            bool success = false;

            string mamePath = Path.GetDirectoryName(Properties.Settings.Default.MameExecutable) + "\\roms\\" + Properties.Settings.Default.MameDriver + "\\";
            
            string templatePath = Path.GetDirectoryName(Application.ExecutablePath) + "\\template\\";

            if (!File.Exists(Properties.Settings.Default.MameExecutable))
            {
                MessageBox.Show("MAME Executable not found. Check path in Preferences", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }

            if (!Directory.Exists(mamePath))
            {
                Directory.CreateDirectory(mamePath);
            }

            if (File.Exists(Path.GetDirectoryName(Properties.Settings.Default.MameExecutable) + "\\roms\\" + Properties.Settings.Default.MameDriver + ".zip"))
            {
                MessageBox.Show("The MAME driver you have specified is using a zipped ROM archive. The level editor does not support zipped ROM's. Please extract your '" + Properties.Settings.Default.MameDriver + ".zip' file to a '" + Properties.Settings.Default.MameDriver + "' folder under the 'roms' folder and delete the .zip file. The level editor will then overwrite your mhavoc ROM's as needed in order to run the level you have created.", "MAME Configuration Issue", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            _currentMaze.Validate();

            if (_currentMaze.IsValid)
            {
                ROMDump rom = new ROMDump(templatePath, mamePath, templatePath);

                MazeObjects.Reactoid reactor = null;
                MazeObjects.EscapePod pod = null;
                MazeObjects.Boots boots = null;
                MazeObjects.Clock clock = null;
                List<MazeEnemies.Pyroid> pyroids = new List<MazeEnemies.Pyroid>();
                List<MazeEnemies.Perkoid> perkoids = new List<MazeEnemies.Perkoid>();
                List<MazeObjects.Oxoid> oxoids = new List<MazeObjects.Oxoid>();
                List<MazeEnemies.LightningH> lightningHorizontal = new List<MazeEnemies.LightningH>();
                List<MazeEnemies.LightningV> lightningVertical = new List<MazeEnemies.LightningV>();
                List<MazeObjects.Arrow> arrows = new List<mhedit.MazeObjects.Arrow>();
                List<MazeWall> staticWalls = new List<MazeWall>();
                List<MazeWall> dynamicWalls = new List<MazeWall>();
                List<MazeObjects.OneWay> oneWayRights = new List<mhedit.MazeObjects.OneWay>();
                List<MazeObjects.OneWay> oneWayLefts = new List<mhedit.MazeObjects.OneWay>();
                List<MazeObjects.Spikes> spikes = new List<mhedit.MazeObjects.Spikes>();
                List<MazeObjects.Lock> locks = new List<mhedit.MazeObjects.Lock>();
                List<MazeObjects.Key> keys = new List<mhedit.MazeObjects.Key>();
                List<MazeObjects.Transporter> transporters = new List<mhedit.MazeObjects.Transporter>();
                List<MazeEnemies.Cannon> cannons = new List<mhedit.MazeEnemies.Cannon>();
                List<MazeEnemies.TripPad> tripPads = new List<mhedit.MazeEnemies.TripPad>();
                MazeObjects.Hand hand = null;

                foreach (MazeObject obj in _currentMaze.MazeObjects)
                {
                    if (obj is MazeObjects.Reactoid)
                    {
                        reactor = (MazeObjects.Reactoid)obj;
                    }
                    else if (obj is MazeEnemies.Pyroid)
                    {
                        pyroids.Add((MazeEnemies.Pyroid)obj);
                    }
                    else if (obj is MazeEnemies.Perkoid)
                    {
                        perkoids.Add((MazeEnemies.Perkoid)obj);
                    }
                    else if (obj is MazeObjects.Oxoid)
                    {
                        oxoids.Add((MazeObjects.Oxoid)obj);
                    }
                    else if (obj is MazeEnemies.LightningH)
                    {
                        lightningHorizontal.Add((MazeEnemies.LightningH)obj);
                    }
                    else if (obj is MazeEnemies.LightningV)
                    {
                        lightningVertical.Add((MazeEnemies.LightningV)obj);
                    }
                    else if (obj is MazeObjects.Arrow)
                    {
                        arrows.Add((MazeObjects.Arrow)obj);
                    }
                    else if (obj is MazeWall)
                    {
                        if (((MazeWall)obj).IsDynamicWall)
                        {
                            dynamicWalls.Add((MazeWall)obj);
                        }
                        else
                        {
                            staticWalls.Add((MazeWall)obj);
                        }
                    }
                    else if (obj is MazeObjects.OneWay)
                    {
                        if (((MazeObjects.OneWay)obj).Direction == MazeObjects.OneWayDirection.Right)
                        {
                            oneWayRights.Add((MazeObjects.OneWay)obj);
                        }
                        else if (((MazeObjects.OneWay)obj).Direction == MazeObjects.OneWayDirection.Left)
                        {
                            oneWayLefts.Add((MazeObjects.OneWay)obj);
                        }
                    }
                    else if (obj is MazeObjects.Spikes)
                    {
                        spikes.Add((MazeObjects.Spikes)obj);
                    }
                    else if (obj is MazeObjects.Lock)
                    {
                        locks.Add((MazeObjects.Lock)obj);
                    }
                    else if (obj is MazeObjects.Key)
                    {
                        keys.Add((MazeObjects.Key)obj);
                    }
                    else if (obj is MazeObjects.EscapePod)
                    {
                        pod = (MazeObjects.EscapePod)obj;
                    }
                    else if (obj is MazeObjects.Boots)
                    {
                        boots = (MazeObjects.Boots)obj;
                    }
                    else if (obj is MazeObjects.Clock)
                    {
                        clock = (MazeObjects.Clock)obj;
                    }
                    else if (obj is MazeObjects.Transporter)
                    {
                        transporters.Add((MazeObjects.Transporter)obj);
                    }
                    else if (obj is MazeEnemies.Cannon)
                    {
                        cannons.Add((MazeEnemies.Cannon)obj);
                    }
                    else if (obj is MazeEnemies.TripPad)
                    {
                        tripPads.Add((MazeEnemies.TripPad)obj);
                    }
                    else if (obj is MazeObjects.Hand)
                    {
                        hand = (MazeObjects.Hand)obj;
                    }
                }

                /////////////////////////////
                // Start building ROM here //
                /////////////////////////////
                //first is the level selection
                rom.Write(ROMAddress.levelst, (byte)_currentMaze.MazeType, 1);

                //next hint text
                rom.Write(ROMAddress.mzh0, rom.GetText(_currentMaze.Hint), 0);

                //build reactor, pyroids and perkoids now...
                //write reactor
                int offset = 0;
                offset += rom.Write(ROMAddress.mzsc0, Context.PointToByteArrayLong(Context.ConvertPixelsToVector(reactor.Position)), offset);
                foreach (MazeEnemies.Pyroid pyroid in pyroids)
                {
                    offset += rom.Write(ROMAddress.mzsc0, Context.PointToByteArrayLong(Context.ConvertPixelsToVector(pyroid.Position)), offset);
                    offset += rom.Write(ROMAddress.mzsc0, new byte[] { (byte)pyroid.Velocity.X, (byte)pyroid.Velocity.Y }, offset);
                }
                if (perkoids.Count > 0)
                {
                    offset += rom.Write(ROMAddress.mzsc0, (byte)0xfe, offset);
                    foreach (MazeEnemies.Perkoid perkoid in perkoids)
                    {
                        offset += rom.Write(ROMAddress.mzsc0, Context.PointToByteArrayLong(Context.ConvertPixelsToVector(perkoid.Position)), offset);
                        offset += rom.Write(ROMAddress.mzsc0, new byte[] { (byte)perkoid.Velocity.X, (byte)perkoid.Velocity.Y }, offset);
                    }
                }
                rom.Write(ROMAddress.mzsc0, (byte)0xff, offset);
                //reactor timer, we will write all 4 entries for now...
                rom.Write(ROMAddress.outime, new byte[] { GetDecimalByte(reactor.Timer), GetDecimalByte(reactor.Timer), GetDecimalByte(reactor.Timer), GetDecimalByte(reactor.Timer) }, 0);

                //do oxygens now
                offset = 0;
                foreach (MazeObjects.Oxoid oxoid in oxoids)
                {
                    byte[] oxoidPositionBytes = Context.PointToByteArrayPacked(oxoid.Position);
                    offset += rom.Write(ROMAddress.mzdc0, oxoidPositionBytes, offset);
                    //offset += rom.Write(ROMAddress.mzdc0, (byte)oxoid.OxoidType, offset);
                    //Point oxoidVectorPoint2 = rom.ByteArrayPackedToPoint(oxoidPositionBytes[0]);
                    //Point oxoidPixelPoint2 = rom.ConvertVectorToPixels(oxoidVectorPoint2);
                }
                rom.Write(ROMAddress.mzdc0, 0, offset);

                //do lightning (Force Fields)
                offset = 0;
                if (lightningHorizontal.Count > 0)
                {
                    foreach (MazeEnemies.LightningH lightning in lightningHorizontal)
                    {
                        Point fixedPosition = new Point(lightning.Position.X + 64, lightning.Position.Y + 64);
                        offset += rom.Write(ROMAddress.mzlg0, Context.PointToByteArrayPacked(fixedPosition), offset);
                    }
                    offset += rom.Write(ROMAddress.mzlg0, (byte)0xff, offset);
                }
                foreach (MazeEnemies.LightningV lightning in lightningVertical)
                {
                    //lighning has a weird positioning issue, lets kludge
                    Point fixedPosition = new Point(lightning.Position.X, lightning.Position.Y + 64);
                    offset += rom.Write(ROMAddress.mzlg0, Context.PointToByteArrayPacked(fixedPosition), offset);
                }
                rom.Write(ROMAddress.mzlg0, (byte)0, offset);

                //build arrows now
                offset = 0;
                foreach (MazeObjects.Arrow arrow in arrows)
                {
                    offset += rom.Write(ROMAddress.mzar0, Context.PointToByteArrayPacked(arrow.Position), offset);
                    offset += rom.Write(ROMAddress.mzar0, (byte)arrow.ArrowDirection , offset);
                }
                rom.Write(ROMAddress.mzar0, (byte)0, offset);

                //maze walls
                //static first
                offset = 0;
                foreach (MazeWall wall in staticWalls)
                {
                    offset += rom.Write(ROMAddress.mzta0, (byte)_currentMaze.PointToStamp(new Point(wall.Position.X + 128, wall.Position.Y + 64)), offset);
                    offset += rom.Write(ROMAddress.mzta0, (byte)wall.WallType, offset);
                }
                rom.Write(ROMAddress.mzta0, (byte)0, offset);

                //then dynamic
                offset = 0;
                foreach (MazeWall wall in dynamicWalls)
                {
                    offset += rom.Write(ROMAddress.mztd0, (byte)_currentMaze.PointToStamp(new Point(wall.Position.X + 128, wall.Position.Y + 64)), offset);
                    offset += rom.Write(ROMAddress.mztd0, (byte)wall.DynamicWallTimout, offset);
                    offset += rom.Write(ROMAddress.mztd0, (byte)wall.AlternateWallTimeout, offset);
                    offset += rom.Write(ROMAddress.mztd0, (byte)wall.WallType, offset);
                    offset += rom.Write(ROMAddress.mztd0, (byte)wall.AlternateWallType, offset);
                }
                rom.Write(ROMAddress.mztd0, (byte)0, offset);

                //one way walls
                offset = 0;
                if (oneWayRights.Count > 0)
                {
                    foreach (MazeObjects.OneWay oneway in oneWayRights)
                    {
                        offset += rom.Write(ROMAddress.mone0, Context.PointToByteArrayPacked(new Point(oneway.Position.X, oneway.Position.Y + 64)), offset);
                    }
                    offset += rom.Write(ROMAddress.mone0, (byte)0xff, offset);
                }
                foreach (MazeObjects.OneWay oneway in oneWayLefts)
                {
                    offset += rom.Write(ROMAddress.mone0, Context.PointToByteArrayPacked(new Point(oneway.Position.X, oneway.Position.Y + 64)), offset);
                }
                rom.Write(ROMAddress.mone0, (byte)0, offset);

                //build spikes now
                offset = 0;
                foreach (MazeObjects.Spikes spike in spikes)
                {
                    offset += rom.Write(ROMAddress.tite0, Context.PointToByteArrayPacked(new Point(spike.Position.X, spike.Position.Y + 64)), offset);
                }
                rom.Write(ROMAddress.tite0, (byte)0, offset);

                //locks and keys, for now, there has to be an even number of locks and keys
                offset = 0;
                for( int i = 0; i < locks.Count; i++)
                {
                    MazeObjects.Lock thisLock = locks[i];
                    MazeObjects.Key thisKey = keys.Where(k => k.KeyColor == thisLock.LockColor).FirstOrDefault();
                    if (thisKey != null)
                    {
                        offset += rom.Write(ROMAddress.lock0, (byte)thisLock.LockColor, offset);
                        offset += rom.Write(ROMAddress.lock0, Context.PointToByteArrayPacked(thisKey.Position), offset);
                        offset += rom.Write(ROMAddress.lock0, Context.PointToByteArrayPacked(new Point(thisLock.Position.X, thisLock.Position.Y + 64)), offset);
                    }
                }
                rom.Write(ROMAddress.lock0, (byte)0, offset);

                //Escape pod
                if (pod != null)
                {
                    rom.Write(ROMAddress.mpod, (byte)pod.Option, 0);
                }

                //clock & boots
                if (clock != null)
                {
                    rom.Write(ROMAddress.mclock, Context.PointToByteArrayPacked(new Point(clock.Position.X + 64, clock.Position.Y + 64)), 0);
                }
                if (boots != null)
                {
                    rom.Write(ROMAddress.mclock, Context.PointToByteArrayPacked(boots.Position), 0x10);
                }

                //transporters
                offset = 0;
                var tpairs = transporters.GroupBy(t => t.Color).Select(group => new { Color = group.Key, Count = keys.Count() });
                if (tpairs.Count() > 0)
                {
                    foreach (var i in tpairs)
                    {
                        if (i.Count == 2)
                        {
                            //there are two... move ahead
                            List<MazeObjects.Transporter> colorT = transporters.Where(t => t.Color == i.Color).ToList();
                            foreach (MazeObjects.Transporter t in colorT)
                            {
                                byte colorByte = (byte)(((byte)t.Color) & 0x0F);
                                if (t.Direction == mhedit.MazeObjects.OneWayDirection.Right)
                                {
                                    colorByte += 0x10;
                                }
                                offset += rom.Write(ROMAddress.tran0, colorByte, offset);
                                offset += rom.Write(ROMAddress.tran0, Context.PointToByteArrayPacked(new Point(t.Position.X, t.Position.Y + 64)), offset);
                            }
                        }
                    }
                    //write end of transports
                    offset += rom.Write(ROMAddress.tran0, 0, offset);
                    //write transportability data
                    offset += rom.Write(ROMAddress.tran0, new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xee }, offset);
                }

                //Laser Cannon
                offset = 0;
                int pointer = 0;
                if (cannons.Count > 0)
                {
                    rom.Write(ROMAddress.mcan, new byte[] { 0x02, 0x02, 0x02, 0x02 }, 0);
                }
                for (int i = 0; i < cannons.Count; i++)
                {
                    MazeEnemies.Cannon cannon = cannons[i];
                    pointer += rom.Write(ROMAddress.mcan0, (UInt16)(rom.GetAddress(ROMAddress.mcand) + offset), pointer);
                    //cannon location first...
                    offset += rom.Write(ROMAddress.mcand, Context.PointToByteArrayLong(Context.ConvertPixelsToVector(cannon.Position)), offset);
                    //now cannon commands
                    foreach(MazeEnemies.iCannonMovement movement in cannon.Movements)
                    {
                        byte command = 0;
                        if (movement is MazeEnemies.CannonMovementMove)
                        {
                            MazeEnemies.CannonMovementMove move = (MazeEnemies.CannonMovementMove)movement;
                            command = 0x80;
                            if (move.Velocity.X == 0 && move.Velocity.Y == 0)
                            {
                                offset += rom.Write(ROMAddress.mcand, command, offset);
                            }
                            else
                            {
                                command += (byte)(move.WaitFrames & 0x3F);
                                offset += rom.Write(ROMAddress.mcand, command, offset);
                                //write velocities
                                if (move.Velocity.X >= 0)
                                {
                                    offset += rom.Write(ROMAddress.mcand, (byte)(move.Velocity.X & 0x3F), offset);
                                }
                                else
                                {
                                    offset += rom.Write(ROMAddress.mcand, (byte)(move.Velocity.X | 0xc0), offset);
                                }
                                if (move.Velocity.Y >= 0)
                                {
                                    offset += rom.Write(ROMAddress.mcand, (byte)(move.Velocity.Y & 0x3F), offset);
                                }
                                else
                                {
                                    offset += rom.Write(ROMAddress.mcand, (byte)(move.Velocity.Y | 0xc0), offset);
                                }
                            }
                        }
                        else if (movement is MazeEnemies.CannonMovementPause)
                        {
                            MazeEnemies.CannonMovementPause pause = (MazeEnemies.CannonMovementPause)movement;
                            command = 0xc0;
                            command += (byte)(pause.WaitFrames & 0x3F );
                            offset += rom.Write(ROMAddress.mcand, command, offset);
                        }
                        else if (movement is MazeEnemies.CannonMovementReturn)
                        {
                            MazeEnemies.CannonMovementReturn ret = (MazeEnemies.CannonMovementReturn)movement;
                            command = 0x00;
                            offset += rom.Write(ROMAddress.mcand, 0, offset);
                        }
                        else if (movement is MazeEnemies.CannonMovementAngle)
                        {
                            MazeEnemies.CannonMovementAngle angle = (MazeEnemies.CannonMovementAngle)movement;
                            command = 0x40;
                            command += (byte)(((int)angle.Angle) << 3);
                            command += (byte)(((int)angle.Speed) << 1);
                            if (angle.FireShot > 0)
                            {
                                command += 0x01;
                            }
                            offset += rom.Write(ROMAddress.mcand, command, offset);
                            if (angle.FireShot > 0)
                            {
                                //write velocity now too
                                offset += rom.Write(ROMAddress.mcand, angle.FireShot, offset);
                            }
                        }
                    }
                }
                //build trips now
                offset = 0;
                int tripoffset = 0;
                foreach (MazeEnemies.TripPad trip in tripPads)
                {
                    offset += rom.Write(ROMAddress.mztr0, Context.PointToByteArrayPacked(new Point(trip.Position.X, trip.Position.Y + 64)), offset);
                    byte[] position = Context.PointToByteArrayShort(new Point(trip.Pyroid.Position.X, trip.Pyroid.Position.Y + 64));
                    if (trip.Pyroid.PyroidStyle == mhedit.MazeEnemies.PyroidStyle.Single)
                    {
                        position[0] += 0x80;
                    }
                    rom.Write(ROMAddress.trtbl, position, tripoffset + 0x18);
                    rom.Write(ROMAddress.trtbl, position, tripoffset + 0x30);
                    rom.Write(ROMAddress.trtbl, position, tripoffset + 0x48);
                    tripoffset += rom.Write(ROMAddress.trtbl, position, tripoffset);
                    
                    rom.Write(ROMAddress.trtbl, new byte[] { (byte)trip.Pyroid.Velocity }, tripoffset + 0x18);
                    rom.Write(ROMAddress.trtbl, new byte[] { (byte)trip.Pyroid.Velocity }, tripoffset + 0x30);
                    rom.Write(ROMAddress.trtbl, new byte[] { (byte)trip.Pyroid.Velocity }, tripoffset + 0x48);
                    tripoffset += rom.Write(ROMAddress.trtbl, new byte[] { (byte)trip.Pyroid.Velocity }, tripoffset);
                    
                }
                rom.Write(ROMAddress.mztr0, (byte)0, offset);
                //de hand finally
                offset = 0;
                if (hand != null)
                {
                    byte[] handLocation = Context.PointToByteArrayShort(new Point(hand.Position.X + 64, hand.Position.Y));
                    offset += rom.Write(ROMAddress.hand0, handLocation , offset);
                    byte[] reactoidLocation = Context.PointToByteArrayShort(new Point(reactor.Position.X, reactor.Position.Y + 64));
                    int xAccordians = Math.Abs(reactoidLocation[0] - handLocation[0]);
                    int yAccordians = Math.Abs(handLocation[1] - reactoidLocation[1]);
                    offset += rom.Write(ROMAddress.hand0, new byte[] { (byte)((xAccordians * 2) + 1), (byte)(yAccordians * 2), 0x3F, 0x0B, 0x1F, 0x05, 0x03 }, offset);
                }

                //write it BABY!!!!
                if (rom.Save())
                {
                    success = true;
                }
            }
            else
            {
                MessageBox.Show(String.Join("\r\n",_currentMaze.ValidationMessage.ToArray()), "Validation Errors", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                success = false;
            }
            return success;
        }

        private byte GetDecimalByte(int value)
        {
            byte outValue = 0;
            string valueString = value.ToString();
            for(int i = 0; i < valueString.Length; i++)
            {
                if (i > 0) outValue = (byte)(outValue << 4);
                outValue += (byte)(((byte)0xF) & byte.Parse(valueString[i].ToString()));
            }
            return outValue;
        }

        private void toolStripButtonConfiguration_Click(object sender, EventArgs e)
        {
            DialogConfiguration d = new DialogConfiguration();
            d.ShowDialog();
        }

        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node.Tag.GetType() == typeof(Maze))
            {
                ((Maze)e.Node.Tag).Name = e.Label;
                propertyGrid.Refresh();
            }
            if (e.Node.Tag.GetType() == typeof(MazeCollection))
            {   
                ((MazeCollection)e.Node.Tag).Name = e.Label;
                propertyGrid.Refresh();
            }
        }

        private void toolStripButtonLoadFromROM_Click(object sender, EventArgs e)
        {

            string romPath = "Z:\\Files\\ROM Archive\\Vid ROM's\\Atari\\MajorHavoc\\";

            if (!Directory.Exists(romPath))
            {
                FolderBrowserDialog fb = new FolderBrowserDialog();
                fb.Description = "Take me to your production ROMs";
                DialogResult dr = fb.ShowDialog();
                if (dr == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }
                romPath = fb.SelectedPath + "\\";
            }

            ROMDump rom = new ROMDump(romPath, romPath, romPath);

            MazeCollection mazeCollection = new MazeCollection("Production Mazes");
            mazeCollection.AuthorEmail = "Owen Rubin";
           
            for (int i = 0; i < 16; i++)
            {

                byte mazeType = (byte) (i & 0x03);
   
                Maze maze = new Maze((MazeType)mazeType, "Level " + (i+1).ToString());

                //hint text - only first 12 mazes tho
                if (i < 12)
                {
                    byte messageIndex = rom.ReadByte(0x93FE, i);
                    maze.Hint = rom.GetMessage(messageIndex);
                }
                //build reactor
                ushort mazeInitIndex = rom.ReadWord(0x2581, i*2);
                MazeObjects.Reactoid reactor = new MazeObjects.Reactoid();
                reactor.LoadPosition(rom.ReadBytes(mazeInitIndex, 4));
                mazeInitIndex += 4;
                int timer = rom.HexToDecimal((int) rom.ReadByte(0x3355, i));
                reactor.Timer = timer;
                maze.AddObject(reactor);



                //pyroids
                byte firstValue = rom.ReadByte(mazeInitIndex, 0);
                while (firstValue != 0xff)
                {
                    MazeEnemies.Pyroid pyroid = new MazeEnemies.Pyroid();
                    pyroid.LoadPosition(rom.ReadBytes(mazeInitIndex, 4));
                    mazeInitIndex += 4;
                    byte fireballVelX = rom.ReadByte(mazeInitIndex, 0);
                    if (fireballVelX > 0x70 && fireballVelX < 0x90)
                    {
                        //incrementing velocity
                        mazeInitIndex++;
                        byte fireballVelXIncrement = fireballVelX;
                        fireballVelX = rom.ReadByte(mazeInitIndex, 0);
                    }
                    mazeInitIndex++;
                    byte fireballVelY = rom.ReadByte(mazeInitIndex, 0);
                    if (fireballVelY > 0x70 && fireballVelY < 0x90)
                    {
                        //incrementing velocity
                        mazeInitIndex++;
                        byte fireballVelYIncrement = fireballVelY;
                        fireballVelY = rom.ReadByte(mazeInitIndex, 0);
                    }
                    mazeInitIndex++;

                    pyroid.Velocity.X = fireballVelX;
                    pyroid.Velocity.Y = fireballVelY;
                    maze.AddObject(pyroid);

                    firstValue = rom.ReadByte(mazeInitIndex, 0);

                    if (firstValue == 0xfe)
                    {
                        mazeInitIndex++;
                        //perkoids now...
                        MazeEnemies.Perkoid perkoid = new MazeEnemies.Perkoid();
                        perkoid.LoadPosition(rom.ReadBytes(mazeInitIndex, 4));
                        mazeInitIndex += 4;
                        byte perkoidVelX = rom.ReadByte(mazeInitIndex, 0);
                        if (perkoidVelX > 0x70 && perkoidVelX < 0x90)
                        {
                            //incrementing velocity
                            mazeInitIndex++;
                            byte perkoidVelXIncrement = perkoidVelX;
                            perkoidVelX = rom.ReadByte(mazeInitIndex, 0);
                        }
                        mazeInitIndex++;
                        byte perkoidVelY = rom.ReadByte(mazeInitIndex, 0);
                        if (perkoidVelY > 0x70 && perkoidVelY < 0x90)
                        {
                            //incrementing velocity
                            mazeInitIndex++;
                            byte perkoidVelYIncrement = perkoidVelY;
                            perkoidVelY = rom.ReadByte(mazeInitIndex, 0);
                        }
                        mazeInitIndex++;

                        perkoid.Velocity.X = perkoidVelX;
                        perkoid.Velocity.Y = perkoidVelY;
                        maze.AddObject(perkoid);

                        firstValue = rom.ReadByte(mazeInitIndex, 0);
                    }
                }             
      
                //do oxygens now
                ushort oxygenBaseAddress = rom.ReadWord(0x25A9, i * 2);

                byte oxoidValue = rom.ReadByte(oxygenBaseAddress, 0);
                while (oxoidValue != 0x00)
                {
                    MazeObjects.Oxoid oxoid = new MazeObjects.Oxoid();
                    oxoid.LoadPosition(oxoidValue);
                    maze.AddObject(oxoid);

                    oxygenBaseAddress++;
                    oxoidValue = rom.ReadByte(oxygenBaseAddress, 0);
                }


                //do lightning (Force Fields)
                ushort lightningBaseAddress = rom.ReadWord(0x25D1, i * 2);

                byte lightningValue = rom.ReadByte(lightningBaseAddress, 0);
                bool isHorizontal = true;

                if (lightningValue == 0xff)
                {
                    isHorizontal = false;
                    lightningBaseAddress++;
                }

                while (lightningValue != 0x00)
                {

                    if (isHorizontal)
                    {
                        MazeEnemies.LightningH lightningh = new MazeEnemies.LightningH();
                        lightningh.LoadPosition(lightningValue);
                        maze.AddObject(lightningh);
                    }
                    else
                    {
                        MazeEnemies.LightningV lightningv = new MazeEnemies.LightningV();
                        lightningv.LoadPosition(lightningValue);
                        maze.AddObject(lightningv);

                    }

                    lightningBaseAddress++;
                    lightningValue = rom.ReadByte(lightningBaseAddress, 0);
                    if (lightningValue == 0xff)
                    {
                        isHorizontal = false;
                        lightningBaseAddress++;
                    }
                    lightningValue = rom.ReadByte(lightningBaseAddress, 0);
                }

                //build arrows now
                ushort arrowBaseAddress = rom.ReadWord(0x25F9, i * 2);
                byte arrowValue = rom.ReadByte(arrowBaseAddress, 0);

                while (arrowValue != 0x00)
                {
                    MazeObjects.Arrow arrow = new MazeObjects.Arrow();
                    arrow.LoadPosition(arrowValue);
                    arrowBaseAddress++;
                    arrowValue = rom.ReadByte(arrowBaseAddress, 0);
                    arrow.ArrowDirection = (MazeObjects.ArrowDirection)arrowValue;
                    maze.AddObject(arrow);
                    arrowBaseAddress++;
                    arrowValue = rom.ReadByte(arrowBaseAddress, 0);
                }

                //maze walls
                //static first
                ushort wallBaseAddress = rom.ReadWord(0x2647, i * 2);
                byte wallValue = rom.ReadByte(wallBaseAddress, 0);

                while (wallValue != 0x00)
                {
                    int relativeWallValue = wallValue - 1;
                    if (mazeType == 0x01)
                    {
                        relativeWallValue = wallValue + 4;
                    }
                    else if (mazeType == 0x02)
                    {
                        relativeWallValue = wallValue + 4;
                    }
                    else if (mazeType == 0x03)
                    {
                        relativeWallValue = wallValue + 2;
                    }
                    Point stampPoint = maze.PointFromStamp(relativeWallValue);
                    wallBaseAddress++;
                    wallValue = rom.ReadByte(wallBaseAddress, 0);
                    MazeWall wall = new MazeWall((MazeWallType)(wallValue & 0x07), stampPoint);
                    wall.UserWall = true;
                    maze.AddObject(wall);
                    wallBaseAddress++;
                    wallValue = rom.ReadByte(wallBaseAddress, 0);
                }

                //then dynamic walls




                //one way walls
                ushort onewayBaseAddress = rom.ReadWord(0x2677, i * 2);

                byte onewayValue = rom.ReadByte(onewayBaseAddress, 0);
                MazeObjects.OneWayDirection onewayOrientation = MazeObjects.OneWayDirection.Right;

                while (onewayValue != 0x00)
                {
                    MazeObjects.OneWay oneway = new MazeObjects.OneWay();
                    oneway.LoadPosition(onewayValue);
                    oneway.Direction = onewayOrientation;
                    maze.AddObject(oneway);

                    onewayBaseAddress++;
                    onewayValue = rom.ReadByte(onewayBaseAddress, 0);
                    if (onewayValue == 0xff)
                    {
                        onewayOrientation = MazeObjects.OneWayDirection.Left;
                        onewayBaseAddress++;
                    }
                    onewayValue = rom.ReadByte(onewayBaseAddress, 0);
                }

                //locks and keys
                ushort lockBaseAddress = rom.ReadWord(0x26D1, i * 2);
                byte lockValue = rom.ReadByte(lockBaseAddress, 0);

                while (lockValue != 0x00)
                {
                    byte lockColor = lockValue;
                    lockBaseAddress++;

                    MazeObjects.Key key = new MazeObjects.Key();
                    key.LoadPosition(rom.ReadByte(lockBaseAddress, 0));
                    key.KeyColor = (MazeObjects.ObjectColor)lockColor;
                    maze.AddObject(key);

                    lockBaseAddress++;

                    MazeObjects.Lock keylock = new MazeObjects.Lock();
                    keylock.LoadPosition(rom.ReadByte(lockBaseAddress, 0));
                    keylock.LockColor = (MazeObjects.ObjectColor)lockColor;
                    maze.AddObject(keylock);

                    lockBaseAddress++;
                    lockValue = rom.ReadByte(lockBaseAddress, 0);
                }

                //Escape pod

                if (mazeType == 0x01)
                {
                    ushort podBaseAddress = 0x32FF;
                    byte podValue = rom.ReadByte(podBaseAddress, i >> 2);

                    if (podValue > 0)
                    {
                        MazeObjects.EscapePod pod = new MazeObjects.EscapePod();
                        maze.AddObject(pod);
                    }
                }

                //clock & boots
                byte clockData = rom.ReadByte(0x3290, i);
                byte bootsData = rom.ReadByte(0x3290, i + 0x10);

                if (clockData != 0)
                {
                    MazeObjects.Clock clock = new MazeObjects.Clock();
                    clock.LoadPosition(clockData);
                    maze.AddObject(clock);
                }

                if (bootsData != 0)
                {
                    MazeObjects.Boots boots = new MazeObjects.Boots();
                    boots.LoadPosition(bootsData);
                    maze.AddObject(boots);
                }
               
                //transporters
                ushort transporterBaseAddress = rom.ReadWord(0x26F9, i * 2);
                byte colorValue = rom.ReadByte(transporterBaseAddress, 0);

                while (colorValue != 0x00)
                {
                    transporterBaseAddress++;
                    MazeObjects.Transporter transporter = new MazeObjects.Transporter();
                    transporter.LoadPosition(rom.ReadByte(transporterBaseAddress, 0));
                    transporter.Direction = MazeObjects.OneWayDirection.Left;
                    if ((colorValue & 0x10) > 0)
                    {
                        transporter.Direction = MazeObjects.OneWayDirection.Right;
                    }
                    transporter.Color = (MazeObjects.ObjectColor)(colorValue & 0x07);
                    maze.AddObject(transporter);
                    transporterBaseAddress++;
                    colorValue = rom.ReadByte(transporterBaseAddress, 0);
                }

                //Laser Cannon
                byte cannonAddressOffset = rom.ReadByte(0x269F, i );
                if (cannonAddressOffset != 0)
                {
                    ushort cannonBaseAddress = (ushort)(0x30B1 + cannonAddressOffset);
                    ushort cannonPointerAddress = rom.ReadWord(cannonBaseAddress, 0);
                    
                    while (cannonPointerAddress != 0)
                    {
                        MazeEnemies.Cannon cannon = new MazeEnemies.Cannon();
                        cannon.LoadPosition(rom.ReadBytes(cannonPointerAddress, 4));
                        maze.AddObject(cannon);

                        cannonBaseAddress += 2;
                        cannonPointerAddress = rom.ReadWord(cannonBaseAddress, 0);
                    }
                }

                //build trips now

                if (i > 3)
                {
                    ushort tripBaseAddress = rom.ReadWord( (ushort)0x2627, ((i-4)*2));
                    ushort tripPyroidBaseAddress = 0x2D36;

                    byte tripX = rom.ReadByte(tripBaseAddress, 0);

                    while (tripX != 0)
                    {
                        MazeEnemies.TripPad trip = new MazeEnemies.TripPad();
                        trip.LoadPosition(tripX);
                        maze.AddObject(trip);

                        tripBaseAddress++;
                        tripX = rom.ReadByte(tripBaseAddress, 0);

                        //trip pyroid too
                        byte bx = (byte) (0x7f & rom.ReadByte(tripPyroidBaseAddress++, 0));
                        byte by = rom.ReadByte(tripPyroidBaseAddress++, 0);
                        byte bv = rom.ReadByte(tripPyroidBaseAddress++, 0);

                        byte[] longBytes = new byte[4];

                        longBytes[0] = 0;
                        longBytes[1] = (byte)((bx & 0x1f)+1);
                        longBytes[2] = 0x80;
                        longBytes[3] = by;

                        MazeEnemies.TripPadPyroid tpp = new MazeEnemies.TripPadPyroid();
                        tpp.LoadPosition(longBytes);
                        maze.AddObject(tpp);

                        trip.Pyroid = tpp;
                    } 
                }

                //finally... de hand
                if (i > 5)
                {
                    ushort handBaseAddress = rom.ReadWord((ushort)(0x2721 + ((i - 6) * 2)), 0);
                    byte handX = rom.ReadByte(handBaseAddress, 0);
                    if (handX != 0)
                    {
                        handBaseAddress++;
                        byte handY = rom.ReadByte(handBaseAddress, 0);
                        MazeObjects.Hand hand = new MazeObjects.Hand();
                        hand.LoadPosition(handY);
                        maze.AddObject(hand);
                    }
                }
                mazeCollection.InsertMaze(i, maze);
            }

            TreeNode node = treeView.Nodes.Add(mazeCollection.Name);
            node.Tag = mazeCollection;
            mazeCollection.TreeRender(treeView, node);
            node.ImageIndex = 0;
            node.SelectedImageIndex = node.ImageIndex;
            treeView.SelectedNode = node;
            mazeCollection.PropertyGrid = propertyGrid;
        }

        private void toolStripButtonContestLogin_Click(object sender, EventArgs e)
        {
            DialogMHPLogin mhpLogin = new DialogMHPLogin();
            mhpLogin.ShowDialog();
        }

        private void toolStripButtonContestUpload_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                if (treeView.SelectedNode.Tag != null)
                {
                    if (treeView.SelectedNode.Tag.GetType().ToString() == "mhedit.Maze")
                    {
                        Maze maze = (Maze)treeView.SelectedNode.Tag;

                        using (MemoryStream oStream = new MemoryStream())
                        {
                            using (MemoryStream mStream = new MemoryStream())
                            {
                                BinaryFormatter b = new BinaryFormatter();
                                b.Serialize(mStream, maze);
                                mStream.Position = 0;
                                BZip2.Compress(mStream, oStream, true, 4096);
                            }
                        }
                    }
                }
            }
        }
    }
}