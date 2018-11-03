using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using Silver.UI;
using mhedit.Containers.MazeObjects;

namespace mhedit.Containers
{
    public delegate void MazePropertiesUpdated(object sender);

    [DefaultPropertyAttribute("Name")]
    [Serializable]
    public class Maze : Panel, ISerializable, ITreeObject, ICustomTypeDescriptor
    {

        #region Declarations

        public const int MAXWALLS = 209;

        private const int GRIDUNITS = 8;
        private const int GRIDUNITSTAMPS = 8;
        //private Point objectOffset = new Point(-16, 16);

        private string mazeName = null;
        private string fileName = null;
        private bool isDirty = false;
        private bool isValid = false;
        private List<string> validationMessage = new List<string>();
        private MazeType mazeType;
        private bool error = false;
        private bool gridLines = true;
        private string lastError = "";
        private int mazeStampsX = 0;
        private int mazeStampsY = 0;
        private decimal zoom = 1;
        private string _mazeHint = String.Empty;
        private bool _repainted = false;

        private PropertyGrid propertyGrid = null;
        private ComboBox comboBoxObjects = null;

        //maze objects
        private MazeWall[] mazeWallBase;
        private List<MazeObject> mazeObjects;

        #endregion

        #region PropertyIncludes

        private string[] NamesToInclude =
        {
          "Name",
          "MazeType",
          "FileName",
          "Hint"
        }; 

        #endregion

        #region Constructors

        public Maze()
        {
            mazeType = MazeType.TypeA;
            Init();
        }

        public Maze(string name)
        {
            mazeName = name;
            mazeType = MazeType.TypeA;
            Init();
        }

        public Maze(MazeType type, string name)
        {
            mazeType = type;
            mazeName = name;
            Init();
        }

        #endregion

        #region Public Properties

        [BrowsableAttribute(false)]
        public List<MazeObject> MazeObjects
        {
            get
            {
                return mazeObjects;
            }
        }

        [BrowsableAttribute(false)]
        public bool ShowGridReferences { get; set; }

        [BrowsableAttribute(false)]
        public bool IsDirty
        {
            get {return isDirty;}
            set { isDirty = value; }
        }

        [BrowsableAttribute(false)]
        public bool IsValid
        {
            get { return isValid; }
            set { isValid = value; }
        }

        [BrowsableAttribute(false)]
        public List<string> ValidationMessage
        {
            get { return validationMessage; }
            set { validationMessage = value; }
        }

        [ReadOnly(true)]
        [DescriptionAttribute("The filename of this maze on disk.")]
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        [BrowsableAttribute(false)]
        public bool GridLines
        {
            get { return gridLines; }
            set { gridLines = value; }
        }

        [BrowsableAttribute(false)]
        public PropertyGrid PropertyGrid
        {
            get { return propertyGrid; }
            set { propertyGrid = value; }
        }

        [BrowsableAttribute(false)]
        public ComboBox ComboBoxObjects
        {
            get { return comboBoxObjects; }
            set 
            { 
                comboBoxObjects = value;
                BindComboBoxObjects(null);
                comboBoxObjects.SelectedIndexChanged += new EventHandler(comboBoxObjects_SelectedIndexChanged);
            }
        }

        [BrowsableAttribute(false)]
        public decimal Zoom
        {
            get { return zoom; }
            set { zoom = value; }
        }

        [BrowsableAttribute(true)]
        [DescriptionAttribute("The name of the maze.")]
        public new string Name
        {
            get { return mazeName; }
            set {
                mazeName = value;
                DataChanged();
            }
        }

        [BrowsableAttribute(true)]
        [DescriptionAttribute("The text shown at the top of the screen when entering the maze. Valid characters are ' 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ..!-,%:'")]
        public string Hint
        {
            get { return _mazeHint; }
            set { _mazeHint = value; }
        }

        [DescriptionAttribute("The structure type of the maze.")]
        public MazeType MazeType
        {
            get { return mazeType; }
            set 
            { 
                mazeType = value;
                InitBaseMap();
                DataChanged();
            }
        }

        public event MazePropertiesUpdated OnMazePropertiesUpdated;

        #endregion

        #region ITreeObject

        public void SetGridlines(bool grid)
        {
            isDirty = true;
            gridLines = grid;
        }

        #endregion

        #region ISerializable

        //Deserialization constructor.
        public Maze(SerializationInfo info, StreamingContext ctxt)
        {
            mazeName = (string)info.GetValue("Name", typeof(string));
            try
            {
                _mazeHint = (string)info.GetValue("Hint", typeof(string));
            }
            catch { };
            fileName = (string)info.GetValue("FileName", typeof(string));
            base.Width = (int)info.GetValue("Width", typeof(int));
            base.Height = (int)info.GetValue("Height", typeof(int));
            base.AllowDrop = (bool)info.GetValue("AllowDrop", typeof(bool));
            isDirty = (bool)info.GetValue("IsDirty", typeof(bool));
            mazeType = (MazeType)info.GetValue("MazeType", typeof(MazeType));
            mazeStampsX = (int)info.GetValue("MazeStampsX", typeof(int));
            mazeStampsY = (int)info.GetValue("MazeStampsY", typeof(int));
            mazeWallBase = (MazeWall[])info.GetValue("MazeWallBase", typeof(MazeWall[]));
            mazeObjects = (List<MazeObject>)info.GetValue("MazeObjects", typeof(List<MazeObject>));
        }
                
        //Serialization function.
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Name", mazeName);
            info.AddValue("Hint", _mazeHint);
            info.AddValue("FileName", fileName);
            info.AddValue("Width", base.Width);
            info.AddValue("Height", base.Height);
            info.AddValue("AllowDrop", base.AllowDrop);
            info.AddValue("IsDirty", false);
            info.AddValue("MazeType", mazeType);
            info.AddValue("MazeStampsX", mazeStampsX);
            info.AddValue("MazeStampsY", mazeStampsY);
            info.AddValue("MazeWallBase", mazeWallBase);
            info.AddValue("MazeObjects", mazeObjects);
        }

        #endregion

        #region ICustomTypeDescriptor

        private PropertyDescriptorCollection FilterProperties(PropertyDescriptorCollection pdc)
        {
            ArrayList toInclude = new ArrayList();
            foreach (string s in NamesToInclude)
                toInclude.Add(s); 

            PropertyDescriptorCollection adjustedProps = new PropertyDescriptorCollection(new PropertyDescriptor[] { });
            foreach (PropertyDescriptor pd in pdc)
                if (toInclude.Contains(pd.Name))
                    adjustedProps.Add(pd);

            return adjustedProps;
        } 

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(this, attributes, true);
            return FilterProperties(pdc);
        }

        PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties()
        {
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(this,true);
            return FilterProperties(pdc);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        #endregion

        # region Overrides

        protected override void OnPaint(PaintEventArgs e)
        {
            //Stopwatch stopwatch = new Stopwatch();
            //long time = stopwatch.ElapsedMilliseconds;
            //stopwatch.Start();
            Console.Write("OnPaint Begin\n");

 	        //base.OnPaint(e);

            Pen bigGridPen = new Pen(Color.DimGray, 1);
            Pen smallGridPen = new Pen(Color.DimGray, 1);
            Pen backgroundPen = new Pen(Color.Gray, 1);
            Brush referenceBrush = Brushes.Yellow;

            int mazeWidth;
            int mazeHeight;
            int currentStamp;

            //base.DisplayRectangle = new Rectangle(base.Left, base.Top,(int)(base.Width * zoom),(int)(base.Height * zoom));
            base.Height = (int)(GRIDUNITS * mazeStampsY * GRIDUNITSTAMPS * zoom);
            base.Width = (int)(GRIDUNITS * mazeStampsX * GRIDUNITSTAMPS * zoom);

            mazeHeight = (int)(base.Height);
            mazeWidth = (int)(base.Width);

            //Console.Write("Init Complete - " + stopwatch.ElapsedMilliseconds.ToString() + "\n");
            //stopwatch.Reset();
            //stopwatch.Start();

            Graphics g = e.Graphics;
            //g.RenderingOrigin = objectOffset;
            g.Clear(Color.Black);

            if (GridLines)
            {
                //now draw the major grid lines
                //vertical
                for (int i = (int)(GRIDUNITS * GRIDUNITSTAMPS * zoom); i <= mazeWidth; i += (int)(GRIDUNITS * GRIDUNITSTAMPS * zoom))
                {
                    g.DrawLine(bigGridPen, (int)(i), 0, (int)(i), mazeHeight);
                }
                //horizontal
                for (int i = (int)(GRIDUNITS * GRIDUNITSTAMPS * zoom); i <= mazeHeight; i += (int)(GRIDUNITS * GRIDUNITSTAMPS * zoom))
                {
                    g.DrawLine(bigGridPen, 0, (int)(i), mazeWidth, (int)(i));
                }
            }

            if (ShowGridReferences)
            {
                //now draw the major grid lines
                //X
                int xOffset = -3;
                for (int i = Math.Abs(xOffset); i <= mazeStampsX; i++)
                {
                    int gridValue = i + xOffset;
                    g.DrawString(gridValue.ToString("X"), Font, referenceBrush, new Point(i * (int)(GRIDUNITS * GRIDUNITSTAMPS * zoom), 1));
                }
                //Y
                for (int i = 0; i <= mazeStampsY; i ++)
                {
                    int gridValue = ((-i) + 12);
                    g.DrawString(gridValue.ToString("X"), Font, referenceBrush, new Point(1, i * (int)(GRIDUNITS * GRIDUNITSTAMPS * zoom)));
                }
            }

            //Console.Write("Grid Complete - " + stopwatch.ElapsedMilliseconds.ToString() + "\n");
            //stopwatch.Reset();
            //stopwatch.Start();

            //in order for our user walls to work we need to not draw
            //the base walls if a user wall exists in that stamp
            //we will use the base wall 'selected' property to denote
            //if there is not a wall in that stamp location
            //first reset all 'selected properties to 0
            for (int i = 0; i < MAXWALLS; i++)
            {
                if (mazeWallBase[i] != null)
                {
                    mazeWallBase[i].Selected = false;
                }
            }
            //set wall to 'selected' of there is a user defined wall at that location
            for (int i = 0; i < mazeObjects.Count; i++)
            {
                MazeObject mazeObject = (MazeObject)mazeObjects[i];
                if (mazeObject.GetType() == typeof(MazeWall))
                {
                    currentStamp = PointToStamp(mazeObject.Position);
                    if (currentStamp >= 0 && currentStamp < mazeWallBase.Length)
                    {
                        if (mazeWallBase[currentStamp] != null)
                        {
                            mazeWallBase[currentStamp].Selected = true;
                        }
                    }
                }
            }
            //now draw all walls that don't have a user defined wall at that location
            for (int rows = 0; rows < mazeStampsY; rows++)
            {
                for (int cols = 0; cols < mazeStampsX; cols++)
                {
                    currentStamp = (rows * mazeStampsX) + cols;
                    if (currentStamp < MAXWALLS)
                    {
                        if (mazeWallBase[currentStamp] != null)
                        {
                            if (mazeWallBase[currentStamp].Selected == false)
                            {
                                if (mazeWallBase[currentStamp].WallType != MazeWallType.Empty)
                                {
                                    Image currentImage = mazeWallBase[currentStamp].Image;
                                    if (currentImage != null)
                                    {
                                        Image scaledImage = currentImage.GetThumbnailImage((int)(currentImage.Width * zoom), (int)(currentImage.Height * zoom), null, System.IntPtr.Zero);
                                        if (scaledImage != null)
                                        {
                                            g.DrawImage(scaledImage, new Point((int)((cols * GRIDUNITS * GRIDUNITSTAMPS * zoom)), (int)((rows * GRIDUNITS * GRIDUNITSTAMPS * zoom))));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }



            //Console.Write("Walls #1 Complete - " + stopwatch.ElapsedMilliseconds.ToString() + "\n");
           // stopwatch.Reset();
            //stopwatch.Start();

            //draw all wall objects
            for (int i = 0; i < mazeObjects.Count; i++)
            {
                MazeObject mazeObject = (MazeObject)mazeObjects[i];
                if (mazeObject.GetType() == typeof(MazeWall))
                {
                    Image scaledImage = mazeObject.Image.GetThumbnailImage((int)(mazeObject.Image.Width * zoom), (int)(mazeObject.Image.Height * zoom), null, System.IntPtr.Zero);
                    g.DrawImage(scaledImage, new Point((int)((mazeObject.Position.X * zoom)), (int)((mazeObject.Position.Y * zoom))));
                }
            }

            //Console.Write("Walls #2 Complete - " + stopwatch.ElapsedMilliseconds.ToString() + "\n");
            //stopwatch.Reset();
            //stopwatch.Start();

            //draw all non-wall objects
            for (int i = 0; i < mazeObjects.Count; i++)
            {
                MazeObject mazeObject = (MazeObject)mazeObjects[i];
                if (mazeObject.GetType() != typeof(MazeWall))
                {
                    Image scaledImage = mazeObject.Image.GetThumbnailImage((int)(mazeObject.Image.Width * zoom), (int)(mazeObject.Image.Height * zoom), null, System.IntPtr.Zero);
                    g.DrawImage(scaledImage, new Point((int)(mazeObject.RenderPosition.X * zoom), (int)(mazeObject.RenderPosition.Y * zoom)));
                }
            }

            _repainted = true;
            //Console.Write("Objects Complete - " + stopwatch.ElapsedMilliseconds.ToString() + "\n");
            //stopwatch.Stop();

            //if (propertyGrid != null) propertyGrid.Refresh();
        }

        protected override bool IsInputKey(Keys keyData)
        {
            return true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Tab:
                    int selectedIndex = GetSelectedObjectIndex();
                    selectedIndex++;
                    selectedIndex %= mazeObjects.Count;
                    SetSelectedObject(selectedIndex);
                    RefreshMaze();
                    break;
                case Keys.Delete:
                    MazeObject obj = GetSelectedObject();
                    if (obj != null)
                    {
                        if (obj is MazeEnemies.TripPadPyroid && ((MazeEnemies.TripPadPyroid)obj).TripPad != null)
                        {
                            MessageBox.Show("This Pyroid is part of a Trip Pad and cannot be deleted. Delete the related Trip Pad to remove this Pyroid.");
                            return;
                        }
                        DialogResult dr = MessageBox.Show("Are you sure you want to remove this object from the Maze?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes)
                        {
                            if (obj is MazeEnemies.TripPad && ((MazeEnemies.TripPad)obj).Pyroid != null)
                            {
                                mazeObjects.Remove(((MazeEnemies.TripPad)obj).Pyroid); 
                            }
                            mazeObjects.Remove(obj);
                            isDirty = true;
                            RefreshMaze();
                        }
                    }
                    break;
                case Keys.Up:
                    MoveSelectedObject(0, -1);
                    isDirty = true;
                    RefreshMaze();
                    break;
                case Keys.Down:
                    MoveSelectedObject(0, 1);
                    isDirty = true;
                    RefreshMaze();
                    break;
                case Keys.Left:
                    MoveSelectedObject(-1, 0);
                    isDirty = true;
                    RefreshMaze();
                    break;
                case Keys.Right:
                    MoveSelectedObject(1, 0);
                    isDirty = true;
                    RefreshMaze();
                    break;

            }
            base.OnKeyDown(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MazeObject obj = GetSelectedObject();
                if (obj != null)
                {
                    DragDropEffects effect = DoDragDrop(obj, DragDropEffects.Copy);

                }
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            this.Select();
            //look for objects here...
            MazeObject mObject = SelectObject(new Point(e.X, e.Y));
            RefreshMaze();
            base.OnMouseClick(e);
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            if (drgevent.Data.GetDataPresent(typeof(Silver.UI.ToolBoxItem)))
            {
                ToolBoxItem item = (ToolBoxItem)drgevent.Data.GetData(typeof(Silver.UI.ToolBoxItem));
                if (item.Object != null)
                {
                    if (item.Object is MazeWall)
                    {

                        MazeObject mObject = SelectObject(this.PointToClient(new Point(drgevent.X, drgevent.Y)));
                        
                        if (mObject != null && mObject is MazeWall)
                        {
                            drgevent.Effect = DragDropEffects.None;
                        }
                        else
                        {
                            drgevent.Effect = DragDropEffects.Copy;
                        }
                    }
                    else
                    {
                        drgevent.Effect = DragDropEffects.Copy;
                    }
                }
            }
            else 
            {
                drgevent.Effect = DragDropEffects.Copy;
            }
            base.OnDragOver(drgevent);
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            //this.Select();
            ToolBoxItem dragItem = null;
            Point panelXY = PointToClient(new Point(drgevent.X, drgevent.Y));
            //Point adjustedXY = new Point(panelXY.X - ((Panel)this.Parent).AutoScrollPosition.X, panelXY.Y - ((Panel)this.Parent).AutoScrollPosition.Y);
            if (drgevent.Data.GetDataPresent(typeof(Silver.UI.ToolBoxItem)))
            {
                dragItem = drgevent.Data.GetData(typeof(Silver.UI.ToolBoxItem)) as ToolBoxItem;

                if (null != dragItem && null != dragItem.Object)
                {
                    if (this.mazeType != MazeType.TypeB && dragItem.Object.GetType() == typeof(EscapePod))
                    {
                        MessageBox.Show("The Escape pod can only be added to 'B' Type mazes.");
                    }
                    else
                    {
                        if (AddObjectClone(dragItem.Object, panelXY))
                        {
                            isDirty = true; 
                            RefreshMaze();
                        }
                    }
                }
            }
            else
            {
                MoveSelectedObject(panelXY);
                RefreshMaze();
            }
            this.Focus();   //this is required so that when dropping an object, the maze panel gets focus so keys work immediately with out clicking on the maze first.
            base.OnDragDrop(drgevent);
        }

        #endregion

        #region Public Methods

        public void Validate()
        {
            isValid = false;
            validationMessage.Clear();

            //
            //validate here...

            if (mazeObjects.Where(o => o is MazeObjects.Reactoid).FirstOrDefault() == null)
            {
                validationMessage.Add("ERROR/REQUIRED: Maze does not contain a reactor");
            }


            isValid = (validationMessage.Count == 0);
        }

        public Image GetImage()
        {
            bool gridLineState = GridLines;
            GridLines = false;
            _repainted = false;
            Invalidate(true);

            while(!_repainted)
            {
                Application.DoEvents();
            }

            SuspendLayout();
            // reverse control z-index
            ReverseControlZIndex(this);

            int xTrim = 200;
            Bitmap bitmap = new Bitmap(this.Width, this.Height);

            this.DrawToBitmap(bitmap, new Rectangle(0, 0, this.Width, this.Height));

            Bitmap trimmedBitmap = new Bitmap(bitmap.Width - xTrim, bitmap.Height);
            Rectangle cropRect = new Rectangle(xTrim, 0, bitmap.Width - xTrim, bitmap.Height);
            using (Graphics g = Graphics.FromImage(trimmedBitmap))
            {
                g.DrawImage(bitmap, new Rectangle(0, 0, trimmedBitmap.Width, trimmedBitmap.Height),cropRect,GraphicsUnit.Pixel);
            }

            // reverse control z-index back
            ReverseControlZIndex(this);
            ResumeLayout(true);

            GridLines = gridLineState;
            Invalidate(true);

            return AdjustContrast(trimmedBitmap, (ushort) 100);
        }

        public static Bitmap AdjustContrast(Bitmap Image, ushort increase)
        {

            Bitmap NewBitmap = (Bitmap)Image.Clone();
            BitmapData data = NewBitmap.LockBits(
                new Rectangle(0, 0, NewBitmap.Width, NewBitmap.Height),
                ImageLockMode.ReadWrite,
                NewBitmap.PixelFormat);
            int Height = NewBitmap.Height;
            int Width = NewBitmap.Width;

            unsafe
            {
                for (int y = 0; y < Height; ++y)
                {
                    byte* row = (byte*)data.Scan0 + (y * data.Stride);
                    int columnOffset = 0;
                    for (int x = 0; x < Width; ++x)
                    {
                        int B = row[columnOffset];
                        int G = row[columnOffset + 1];
                        int R = row[columnOffset + 2];

                        if (R > 0) R += increase;
                        if (G > 0) G += increase;
                        if (B > 0) B += increase;

                        if (R > 255) R = 255;
                        if (G > 255) G = 255;
                        if (B > 255) B = 255;

                        row[columnOffset] = (byte)B;
                        row[columnOffset + 1] = (byte)G;
                        row[columnOffset + 2] = (byte)R;

                        columnOffset += 4;
                    }
                }
            }
            NewBitmap.UnlockBits(data);
            return NewBitmap;
        }

        private void ReverseControlZIndex(Control parentControl)
        {
            var list = new List<Control>();
            foreach (Control i in parentControl.Controls)
            {
                list.Add(i);
            }
            var total = list.Count;
            for (int i = 0; i < total / 2; i++)
            {
                var left = parentControl.Controls.GetChildIndex(list[i]);
                var right = parentControl.Controls.GetChildIndex(list[total - 1 - i]);

                parentControl.Controls.SetChildIndex(list[i], right);
                parentControl.Controls.SetChildIndex(list[total - 1 - i], left);
            }
        }

        public ContextMenu GetTreeContextMenu()
        {
            ContextMenu menu = new ContextMenu();
            return menu;
        }

        public TreeNode TreeRender(TreeView treeView, TreeNode currentNode)
        {
            TreeNode parentNode = null;
            TreeNode mazeNode = null;
            if (treeView != null)
            {
                try
                {
                    treeView.BeginUpdate();
                    //see if it has a parent node...
                    if (currentNode != null)
                    {
                        if (currentNode.Parent != null)
                        {
                            parentNode = currentNode.Parent;
                        }
                    }
                    //now, remove this node
                    if (currentNode != null)
                    {
                        treeView.Nodes.Remove(currentNode);
                    }
                    //create the node now..
                    if (parentNode != null)
                    {
                        mazeNode = parentNode.Nodes.Add(this.Name);
                    }
                    else
                    {
                        mazeNode = treeView.Nodes.Add(this.Name);
                    }
                    mazeNode.Name = this.Name;
                    mazeNode.ImageIndex = ((int)mazeType) + 1;
                    mazeNode.SelectedImageIndex = mazeNode.ImageIndex;
                    mazeNode.Tag = this;

                }
                catch (Exception ex)
                {
                    error = true;
                    lastError = ex.Message;
                }
                finally
                {
                    treeView.EndUpdate();
                }
            }
            else
            {
                error = true;
                lastError = "Tree not defined.";
            }
            return mazeNode;
        }

        #endregion

        #region Private Methods

        private void Init()
        {
            mazeWallBase = new MazeWall[MAXWALLS];
            mazeObjects = new List<MazeObject>();

            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

            InitBaseMap();

            //event methods
            base.AllowDrop = true;
            base.TabStop = true;

            //DragOver += new DragEventHandler(Maze_DragOver);
        }

        private void InitBaseMap()
        {
            //initialize our base maze maps
            mazeWallBase = null;
            MazeFactory.MazeBaseData mazeBaseData = MazeFactory.GetBaseMap(mazeType);
            mazeStampsX = mazeBaseData.mazeStampsX;
            mazeStampsY = mazeBaseData.mazeStampsY;
            mazeWallBase = mazeBaseData.mazeWallBase;
            base.Height = GRIDUNITS * mazeStampsY * GRIDUNITSTAMPS;
            base.Width = GRIDUNITS * mazeStampsX * GRIDUNITSTAMPS;
        }

        private void DataChanged()
        {
            OnMazePropertiesUpdated?.Invoke(this);
        }

        public bool AddObject(object obj)
        {
            bool wasAdded = false;
            MazeObject mazeObject = obj as MazeObject;

            if (mazeObject != null)
            {
                if (((MazeObject)obj).MaxObjects > GetObjectTypeCount(obj.GetType()))
                {
                    ClearSelectedObjects();
                    string type = obj.GetType().ToString();
                    string[] typeString = type.Split('.');

                    MazeWall wall = obj as MazeWall;
                    if (wall != null)
                    {
                        wall.Name = GetNextName(typeString[typeString.Length - 1].ToLower());
                        wall.Position = GetAdjustedPosition((MazeObject)wall, wall.Position);
                        mazeObjects.Add((MazeObject)obj);
                        BindComboBoxObjects((MazeObject)obj);
                        if (propertyGrid != null) propertyGrid.SelectedObject = (MazeObject)obj;
                        wasAdded = true;
                    }
                    else
                    {
                        mazeObject.Name = GetNextName(typeString[typeString.Length - 1].ToLower());
                        mazeObjects.Add((MazeObject)obj);
                        BindComboBoxObjects((MazeObject)obj);
                        if (propertyGrid != null) propertyGrid.SelectedObject = (MazeObject)obj;
                        wasAdded = true;
                    }
                }
                else
                {
                    MessageBox.Show("You can't add any more objects of this type.", "The Homeworld is near", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            return wasAdded;
        }


        public bool AddObjectClone(object obj, Point point)
        {
            bool wasAdded = false;
            if (((MazeObject)obj).MaxObjects > GetObjectTypeCount(obj.GetType()))
            {
                ClearSelectedObjects();
                string type = obj.GetType().ToString();

                MazeWall mazeWall = obj as MazeWall;
                if (mazeWall != null)
                {

                    MazeWall wall = new MazeWall(((MazeWall)obj).WallType);
                    wall.UserWall = true;
                    wall.Selected = true;
                    wall.Position = GetAdjustedPosition((MazeObject)wall, point);
                    wall.Name = GetNextName("wall");
                    mazeObjects.Add(wall);
                    BindComboBoxObjects(wall);
                    if (propertyGrid != null) propertyGrid.SelectedObject = wall;
                    wasAdded = true;
                }
                else
                {
                    MazeObject mazeObject = (MazeObject)Activator.CreateInstance(obj.GetType(), true);
                    mazeObject.Position = GetAdjustedPosition((MazeObject)mazeObject, point);
                    mazeObject.Selected = true;
                    string[] typeString = type.Split('.');
                    mazeObject.Name = GetNextName(typeString[typeString.Length - 1].ToLower());
                    mazeObjects.Add(mazeObject);
                    if (mazeObject is MazeEnemies.TripPad)
                    {
                        //special case for Trip Pads, must create a pyroid too
                        MazeEnemies.TripPadPyroid tripPyroid = new MazeEnemies.TripPadPyroid();
                        tripPyroid.Position = GetAdjustedPosition(tripPyroid, mazeObject.Position);
                        tripPyroid.TripPad = (MazeEnemies.TripPad)mazeObject;
                        tripPyroid.Name = GetNextName("trippyroid");
                        mazeObjects.Add(tripPyroid);
                        ((MazeEnemies.TripPad)mazeObject).Pyroid = tripPyroid;
                    }
                    BindComboBoxObjects(mazeObject);
                    if (propertyGrid != null) propertyGrid.SelectedObject = mazeObject;
                    wasAdded = true;
                }
            }
            else
            {
                MessageBox.Show("You can't add any more objects of this type.", "The Homeworld is near", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return wasAdded;
        }

        private void BindComboBoxObjects(MazeObject obj)
        {
            if (comboBoxObjects != null)
            {
                comboBoxObjects.DataSource = mazeObjects.OrderBy(o => o.Name).ToList();
                comboBoxObjects.DisplayMember = "Name";
                comboBoxObjects.ValueMember = "Name";
                if (obj != null)
                {
                    comboBoxObjects.SelectedIndex = comboBoxObjects.FindStringExact(obj.Name);
                }
                else if (comboBoxObjects.Items.Count > 0)
                {
                    comboBoxObjects.SelectedIndex = 0;
                }
            }
        }

        private void MoveSelectedObject(Point newpos)
        {
            MazeObject obj = GetSelectedObject();
            if (obj != null)
            {
                obj.Position = GetAdjustedPosition(obj, newpos);
            }
        }

        private void MoveSelectedObject(int x, int y)
        {
            MazeObject obj = GetSelectedObject();
            if (obj != null)
            {
                int new_x = obj.Position.X + (x * obj.SnapSize.X);
                int new_y = obj.Position.Y + (y * obj.SnapSize.Y);

                if (new_x >= 0)
                {
                    if (new_y >= 0)
                    {
                        if (new_x <= base.Width - obj.Size.Width)
                        {
                            if (new_y <= base.Height - obj.Size.Height)
                            {
                                if (new_x != obj.Position.X || new_y != obj.Position.Y)
                                {
                                    isDirty = true;
                                    obj.Position = new Point(new_x, new_y);
                                }
                            }
                        }
                    }
                }
            }
        }

        private Point GetAdjustedPosition(MazeObject obj, Point point)
        {
            //Point snapPosition = new Point();
            Point finalPosition = new Point();
            //adjust for size of object so mouse appears to be at center point
            finalPosition.X = point.X - (obj.Size.Width / 2);
            finalPosition.Y = point.Y - (obj.Size.Height / 2);
            //apply the objects snapto grid
            finalPosition.X = finalPosition.X - (finalPosition.X % obj.SnapSize.X);
            finalPosition.Y = finalPosition.Y - (finalPosition.Y % obj.SnapSize.Y);

            //apply any offset
            finalPosition.X = finalPosition.X + obj.RenderOffset.X;
            finalPosition.Y = finalPosition.Y + obj.RenderOffset.Y;
            
            //bounds check
            if (finalPosition.X < 0) finalPosition.X = 0;
            if (finalPosition.Y < 0) finalPosition.Y = 0;
            
            
            return finalPosition;
        }

        public int PointToStamp(Point point)
        {
            int row = point.X / (GRIDUNITS * GRIDUNITSTAMPS);
            int col = point.Y / (GRIDUNITS * GRIDUNITSTAMPS);
            return Math.Max(Math.Min((col * mazeStampsX) + row, MAXWALLS), 0);
        }

        public Point PointFromStamp(int stamp)
        {
            int col = stamp % mazeStampsX;
            int row = stamp / mazeStampsX;
            return new Point(col * GRIDUNITS * GRIDUNITSTAMPS, row * GRIDUNITS * GRIDUNITSTAMPS);
        }

        private void RefreshMaze()
        {
            if (propertyGrid != null)
            {
                MazeObject obj = GetSelectedObject();
                propertyGrid.SelectedObject = obj;
                //BindComboBoxObjects(null);
                if (comboBoxObjects != null)
                {
                    if (obj != null)
                    {
                        int itemIndex = ComboBoxObjects.FindStringExact(obj.Name);
                        if (itemIndex >= 0)
                        {
                            ComboBoxObjects.SelectedIndex = itemIndex;
                        }
                    }
                    else
                    {
                        comboBoxObjects.SelectedIndex = 0;
                    }
                }
            }
            Invalidate();
        }

        private string GetNextName(string prefix)
        {
            for (int i = 1; i > 0; i++)
            {
                if (FindObject(prefix + i.ToString()) == null)
                {
                    return prefix + i.ToString();
                }
            }
            return prefix;
        }

        private MazeObject FindObject(string name)
        {
            for (int i = 0; i < mazeObjects.Count; i++)
            {
                if (mazeObjects[i].Name == name) return mazeObjects[i];
            }
            return null;
        }

        private MazeObject SelectObject(Point location)
        {
            //go through each object from the top down
            //and see if we clicked on it's area...
            for (int i = mazeObjects.Count - 1; i >= 0; i--)
            {
                if (PointInObject((MazeObject)mazeObjects[i],location))
                {
                    if (((MazeObject)mazeObjects[i]).Selected == false)
                    {
                        ClearSelectedObjects();
                        //SetSelectedWall(-1);
                        ((MazeObject)mazeObjects[i]).Selected = true;
                        return (MazeObject)mazeObjects[i];
                    }
                }
            }
            return null;
        }

        private bool PointInObject(MazeObject obj, Point location)
        {
            if (location.X >= obj.RenderPosition.X && location.X <= obj.RenderPosition.X + obj.Size.Width)
            {
                if (location.Y >= obj.RenderPosition.Y && location.Y <= obj.RenderPosition.Y + obj.Size.Height)
                {
                    return true;
                }
            }
            return false;
        }

        private MazeObject GetSelectedObject()
        {
            //unselect all other walls
            for (int i = 0; i < mazeObjects.Count; i++)
            {
                if (mazeObjects[i].Selected) return mazeObjects[i];
            }
            return null;
        }

        private int GetSelectedObjectIndex()
        {
            //unselect all other walls
            for (int i = 0; i < mazeObjects.Count; i++)
            {
                if (mazeObjects[i].Selected) return i;
            }
            return -1;
        }

        private void ClearSelectedObjects()
        {
            for (int i = 0; i < mazeObjects.Count; i++)
            {
                MazeObject mazeObject = (MazeObject)mazeObjects[i];
                mazeObject.Selected = false;
            }
        }

        private int GetObjectTypeCount(Type type)
        {
            int count = 0;
            for (int i = 0; i < mazeObjects.Count; i++)
            {
                if (mazeObjects[i].GetType() == type) count++;
            }
            return count;
        }

        private void SetSelectedObject(int index)
        {
            if (index < mazeObjects.Count && index >= 0)
            {
                ClearSelectedObjects();
                mazeObjects[index].Selected = true;
            }
        }

        private void comboBoxObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            //go through each object from the top down
            //and see if we clicked on it's area...
            for (int i = mazeObjects.Count - 1; i >= 0; i--)
            {
                if (comboBoxObjects.SelectedValue != null)
                {
                    if (comboBoxObjects.SelectedValue.ToString() == mazeObjects[i].Name)
                    {
                        ClearSelectedObjects();
                        mazeObjects[i].Selected = true;
                        RefreshMaze();
                        return;
                    }
                }
            }
        }

        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Maze
            // 
            this.Font = new System.Drawing.Font("Courier New", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResumeLayout(false);

        }

    }
}