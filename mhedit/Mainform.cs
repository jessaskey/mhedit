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
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Zip;

using mhedit.Containers;
using mhedit.Containers.MazeEnemies;
using mhedit.Containers.MazeObjects;
using mhedit.Containers.TypeConverters;
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
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.wall_horizontal_32.ico"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.wall_leftup_32.ico"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.wall_rightup_32.ico"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.wall_leftdown_32.ico"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.wall_rightdown_32.ico"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.wall_vertical_32.ico"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.wall_empty_32.ico"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.pyroid_32.ico"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.reactoid_32.ico"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.perkoid_32.ico"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.roboid_32.ico"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.oxoid_32.ico"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.key_32.ico"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.arrow_32.ico"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.lightning_32.ico"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.lightning_v_32.ico"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.clock_32.ico"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.cannon_32.ico"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.oneway_32.ico"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.trippad_32.ico"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.lock_32.ico"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.hand_32.ico"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.spikes_32.ico"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.booties_32.ico"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.transporter_32.ico"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.pod_32.ico"));
            toolboxImageList.Images.Add(ResourceFactory.GetResourceImage("mhedit.images.buttons.roboid_32.ico"));
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
            itemIndex = toolBox[tabIndex].AddItem("Ion Cannon", 17, true, new Cannon());
            itemIndex = toolBox[tabIndex].AddItem("Trip Pad", 19, true, new TripPad());
            //toolBox[tabIndex].AddItem("Roboid", 10, true, null);
            tabIndex = toolBox.AddTab("Maze Objects", -1);
            itemIndex = toolBox[tabIndex].AddItem("Reactoid", 8, true, new Reactoid());
            itemIndex = toolBox[tabIndex].AddItem("Arrow", 13, true, new Arrow());
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

            if (treeView.SelectedNode != null)
            {
                if (treeView.SelectedNode.Tag != null)
                {
                    if (treeView.SelectedNode.Tag.GetType() == typeof(Maze))
                    {
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
                    }
                    else if (treeView.SelectedNode.Tag.GetType() == typeof(MazeCollection))
                    { 
                            MazeCollection collection = (MazeCollection)treeView.SelectedNode.Tag;
                            propertyGrid.SelectedObject = collection;
                            toolStripButtonSaveCollection.Enabled = true;
                            toolStripButtonConfiguration.Enabled = true;
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
                            if (node.Tag.GetType() == typeof(MazeCollection))
                            {

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
                            }
                            else if (node.GetType() == typeof(Maze))
                            {
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
                    ITreeObject treeObject = grid.SelectedObject as ITreeObject;
                    if (treeObject != null)
                    {
                        treeObject.TreeRender(treeView, treeView.SelectedNode);
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
            //assign callbacks
            foreach (Maze maze in collection.Mazes)
            {
                maze.OnMazePropertiesUpdated += new MazePropertiesUpdated(RefreshMazeName);
            }
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
                    if (treeView.SelectedNode.Tag.GetType() == typeof(MazeCollection))
                    {
                        collection = (MazeCollection)treeView.SelectedNode.Tag;
                    }
                    if (treeView.SelectedNode.Parent != null)
                    {
                        if (treeView.SelectedNode.Parent.Tag.GetType() == typeof(MazeCollection))
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

        private void RefreshMazeName(object sender)
        {
            if (treeView.SelectedNode != null)
            {
                treeView.SelectedNode.Text = ((Maze)sender).Name;
            }
        }

        private void toolStripButtonNewMaze_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Maze maze = new Maze(GetNewName("Maze"));
            maze.OnMazePropertiesUpdated += new MazePropertiesUpdated(RefreshMazeName);
            maze.ShowGridReferences = Properties.Settings.Default.ShowGridReferences;
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
                        BZip2.Decompress(fStream, mStream, false);
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
                    if (treeView.SelectedNode.Tag.GetType() == typeof(Maze))
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
                    if (treeView.SelectedNode.Tag.GetType() == typeof(Maze))
                    {
                        Maze maze = (Maze)treeView.SelectedNode.Tag;
                        SaveMaze(maze);
                    }
                    if (treeView.SelectedNode.Tag.GetType()== typeof(MazeCollection))
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
                        if (treeView.SelectedNode.Tag.GetType() == typeof(Maze))
                        {
                            Maze maze = (Maze)treeView.SelectedNode.Tag;
                            CloseMaze(maze);
                            treeView.SelectedNode.Remove();
                            RefreshTree();
                        }
                        else if (treeView.SelectedNode.Tag.GetType() == typeof(MazeCollection))
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
                        if (treeView.SelectedNode.Parent.Tag.GetType() == typeof(MazeCollection))
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
                    if (treeView.SelectedNode.Tag.GetType() == typeof(Maze))
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
                _currentMaze = null;
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
                        //show the maze properties on tree click
                        propertyGrid.SelectedObject = _currentMaze;
                    }
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
            if (_currentMaze != null)
            {
                if (MAMEHelper.CreateMAMERom(_currentMaze))
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

            string romPath = @"C:\SVN\havoc\mame\roms\mhavoc\";

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

            MazeCollection mazeCollection = MAMEHelper.GetMazeCollectionFromROM(romPath, RefreshMazeName);

            TreeNode node = treeView.Nodes.Add(mazeCollection.Name);
            node.Tag = mazeCollection;
            mazeCollection.TreeRender(treeView, node);
            node.ImageIndex = 0;
            node.SelectedImageIndex = node.ImageIndex;
            treeView.SelectedNode = node;
            mazeCollection.PropertyGrid = propertyGrid;
        }

        private void toolStripButtonContestUpload_Click(object sender, EventArgs e)
        {

            if (treeView.SelectedNode == null)
            {
                MessageBox.Show("You must select a maze to upload.", "Missing Maze", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (treeView.SelectedNode.Tag == null)
                {
                    MessageBox.Show("There was an error getting the maze from the tree. Sorry!", "Missing Maze", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (treeView.SelectedNode.Tag.GetType() == typeof(Maze))
                    {
                        Maze maze = (Maze)treeView.SelectedNode.Tag;
                        byte[] mazeBytes = null;
                        Image mazeImage = maze.GetImage();
                        
                        using (MemoryStream oStream = new MemoryStream())
                        {
                            using (MemoryStream mStream = new MemoryStream())
                            {
                                BinaryFormatter b = new BinaryFormatter();
                                b.Serialize(mStream, maze);
                                mStream.Position = 0;
                                BZip2.Compress(mStream, oStream, false, 4096);

                                oStream.Position = 0;
                                mazeBytes = new byte[oStream.Length];
                                oStream.Read(mazeBytes, 0, (int)oStream.Length);
                            }
                        }

                        if (mazeBytes != null && mazeImage != null)
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
                            mhpDialog.MazeBytes = mazeBytes;
                            mhpDialog.MazeName = maze.Name;
                            mhpDialog.ShowDialog();
                        }
                    }
                }
            }
        }
    }
}