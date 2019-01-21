using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using mhedit.Containers;
using System.Xml.Serialization;
using ICSharpCode.SharpZipLib.BZip2;
using System.Xml;

namespace mhedit
{
    [DefaultPropertyAttribute("Name")]
    [Serializable]
    public class MazeCollectionController: ITreeObject, ICustomTypeDescriptor
    {
        #region Declarations

        private const int MAX_MAZES = 32;
        private MazeCollection _mazeCollection = new MazeCollection();

        private string _fileName = null;

        private bool gridlines = true;

        private string exportPath = String.Empty;

        private PropertyGrid propertyGrid = null;

        private bool _error = false;
        private string _lastError = String.Empty;
        #endregion

        #region PropertyIncludes

        private string[] NamesToInclude =
        {
          "Name",
          "FileName"
        };

        #endregion

        #region Constructor

        public MazeCollectionController()
        {
        }

        public MazeCollectionController(string name)
        {
            _mazeCollection.Name = name;
        }

        public MazeCollectionController(MazeCollection collection)
        {
            _mazeCollection = collection;
        }

        #endregion

        #region Properties



        //[BrowsableAttribute(false)]
        //public PropertyGrid PropertyGrid
        //{
        //    //get { return propertyGrid; }
        //    set
        //    {
        //        //propertyGrid = value;
        //        foreach (MazeController maze in _mazeCollection.Mazes)
        //        {
        //            maze.PropertyGrid = value;
        //        }
        //    }
        //}

        [BrowsableAttribute(false)]
        public MazeCollection MazeCollection
        {
            get { return _mazeCollection; }
            set { _mazeCollection = value; }
        }

        [BrowsableAttribute(false)]
        public List<Maze> Mazes
        {
            get { return _mazeCollection.Mazes; }
            set { _mazeCollection.Mazes = value; }
        }

        [BrowsableAttribute(false)]
        public int MazeCount
        {
            get { return _mazeCollection.Mazes.Count; }
        }

        [BrowsableAttribute(false)]
        public bool IsValid
        {
            get
            {
                if (_mazeCollection.Mazes.Where(m => !m.IsValid).Count() > 0)
                {
                    return false;
                }
                return true;
            }
        }

        [BrowsableAttribute(false)]
        public string ValidationMessage
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach(Maze maze in _mazeCollection.Mazes.Where(m => !m.IsValid))
                {
                    sb.AppendLine(maze.Name + ": " + maze.ValidationMessage);
                }
                return sb.ToString();
            }
        }

        [BrowsableAttribute(false)]
        public string LastError
        {
            get { return _mazeCollection.LastError; }
        }

        [ReadOnly(true)]
        [DescriptionAttribute("The filename of this collection on disk.")]
        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                _mazeCollection.IsDirty = true;
            }
        }

        //[ReadOnly(true)]
        //[DescriptionAttribute("The last ROM Export file location.")]
        //public string ExportPath
        //{
        //    get { return exportPath; }
        //    set { 
        //        exportPath = value;
        //        isDirty = true;
        //    }
        //}

        [BrowsableAttribute(true)]
        [DescriptionAttribute("The name of this maze collection.")]
        public new string Name
        {
            get { return _mazeCollection.Name; }
            set 
            {
                _mazeCollection.Name = value;
                _mazeCollection.IsDirty = true;
            }
        }

        [DescriptionAttribute("The name of the person who created this maze.")]
        public string AuthorName
        {
            get { return _mazeCollection.AuthorName; }
            set 
            { 
                _mazeCollection.AuthorName = value;
                _mazeCollection.IsDirty = true;
            }
        }

        [DescriptionAttribute("The email address of the person who created this maze.")]
        public string AuthorEmail
        {
            get { return _mazeCollection.AuthorEmail; }
            set 
            {
                _mazeCollection.AuthorEmail = value;
                _mazeCollection.IsDirty = true;
            }
        }

        [BrowsableAttribute(false)]
        public bool IsDirty
        {
            get
            {
                if (_mazeCollection.IsDirty) return true;
                foreach(Maze maze in _mazeCollection.Mazes) { 
                    if (maze.IsDirty) return true;
                }
                return false;

            }
        }

        #endregion


        public static MazeCollection DeserializeFromFile(string fileName)
        {
            MazeCollection mazeCollection = null;
            using (FileStream fStream = new FileStream(fileName, FileMode.Open))
            {
                mazeCollection = DeserializeFromStream(fStream);
            }
            return mazeCollection;
        }

        public static MazeCollection DeserializeFromStream(Stream inputStream)
        {
            MazeCollection mazeCollection = null;
            using (MemoryStream mStream = new MemoryStream())
            {
                BZip2.Decompress(inputStream, mStream, false);
                mStream.Position = 0;
                var serializer = new XmlSerializer(typeof(MazeCollection));
                using (var reader = XmlReader.Create(mStream))
                {
                    mazeCollection = (MazeCollection)serializer.Deserialize(reader);
                }
            }
            return mazeCollection;
        }

        public static bool SerializeToFile(MazeCollection mazeCollection, string fileName)
        {
            bool result = false;
            using (FileStream fStream = new FileStream(fileName, FileMode.Create))
            {
                using (MemoryStream mStream = new MemoryStream())
                {
                    var serializer = new XmlSerializer(typeof(MazeCollection));
                    using (var writer = XmlWriter.Create(mStream))
                    {
                        serializer.Serialize(writer, mazeCollection, Constants.XmlNamespace);
                    }
                    mStream.Position = 0;
                    BZip2.Compress(mStream, fStream, true, 4096);
                    result = true;
                }
            }
            return result;
        }

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
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(this, true);
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

        #region ITreeObject

        public TreeNode TreeRender(TreeView treeView, TreeNode currentNode, bool gridLines)
        {
            TreeNode parentNode = null;
            TreeNode collectionNode = null;
            if (treeView != null)
            {
                try
                {
                    //treeView.BeginUpdate();
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
                        collectionNode = parentNode.Nodes.Add(_mazeCollection.Name);
                    }
                    else
                    {
                        collectionNode = treeView.Nodes.Add(_mazeCollection.Name);
                    }
                    collectionNode.Name = this.Name;
                    collectionNode.Tag = this;
                    for (int i = 0; i < _mazeCollection.Mazes.Count; i++)
                    {
                        MazeController mazeController = new MazeController(_mazeCollection.Mazes[i]);
                        mazeController.GridLines = gridLines;
                        TreeNode mazeNode = new TreeNode(((int)i + 1).ToString() + ": <unassigned>");
                        mazeNode.ForeColor = Color.Gray;
                        mazeNode.Text = _mazeCollection.Mazes[i].Name;
                        mazeNode.Tag = mazeController;
                        mazeNode.ForeColor = Color.Black;
                        mazeNode.ImageIndex = ((int)_mazeCollection.Mazes[i].MazeType) + 1;
                        mazeNode.SelectedImageIndex = mazeNode.ImageIndex;
                        collectionNode.Nodes.Add(mazeNode);
                    }
                    collectionNode.Expand();
                }
                catch (Exception ex)
                {
                    _mazeCollection.Error = true;
                    _mazeCollection.LastError = ex.Message;
                }
                finally
                {
                    treeView.EndUpdate();
                }
            }
            else
            {
                _error = true;
                _lastError = "Tree not defined.";
            }
            return collectionNode;
        }

        public void SetGridlines(bool grid)
        {
            gridlines = grid;
        }

        public ContextMenu GetTreeContextMenu()
        {
            ContextMenu menu = new ContextMenu();
            menu.MenuItems.Add("Test");
            menu.MenuItems.Add("Test2");
            return menu;
        }

        #endregion

        #region Overrides

        //protected override void OnPaint(PaintEventArgs e)
        //{

        //}

        //protected override void OnDragOver(DragEventArgs drgevent)
        //{
        //    //this object does not accept drag/drop
        //    drgevent.Effect = DragDropEffects.None;
        //}

        #endregion

        #region Public Methods

        //public int FindMaze(Maze maze)
        //{
        //    for (int i = 0; i < mazes.Count; i++)
        //    {
        //        if (mazes[i] == maze)
        //        {
        //            return i;
        //        }
        //    }
        //    return -1;
        //}



        public bool AddMaze(Maze maze)
        {
            if (maze != null)
            {
                Mazes.Add(maze);
                return true;
            }
            return false;
        }

        public void Validate()
        {
            foreach (Maze maze in Mazes)
            {
                maze.Validate();
            }
        }

        #endregion


    }
}
