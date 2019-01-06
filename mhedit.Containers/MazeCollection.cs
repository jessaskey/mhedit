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


namespace mhedit.Containers
{
    [DefaultPropertyAttribute("Name")]
    [Serializable]
    public class MazeCollection 
    {
        #region Declarations

        private const int MAX_MAZES = 32;
        private List<Maze> _mazes = new List<Maze>();
        private Guid _id = Guid.NewGuid();
        private bool _error = false;
        private bool _isDirty = false;
        private string _lastError = "";
        private string collectionName = "<new maze collection>";
        private string authorName = null;
        private string authorEmail = null;

        //private PropertyGrid propertyGrid = null;
        #endregion



        #region Constructor

        public MazeCollection()
        {
        }

        public MazeCollection(string name)
        {
            collectionName = name;
        }

        #endregion

        #region Properties

        [BrowsableAttribute(false)]
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }


        [BrowsableAttribute(false)]
        public List<Maze> Mazes
        {
            get { return _mazes; }
            set { _mazes = value; }
        }

        [BrowsableAttribute(false)]
        public int MazeCount
        {
            get { return _mazes.Count; }
        }

        [BrowsableAttribute(false)]
        public bool IsValid
        {
            get
            {
                if (_mazes.Where(m => !m.IsValid).Count() > 0)
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
                foreach(Maze maze in _mazes.Where(m => !m.IsValid))
                {
                    sb.AppendLine(maze.Name + ": " + maze.ValidationMessage);
                }
                return sb.ToString();
            }
        }

        [BrowsableAttribute(false)]
        public string LastError
        {
            get { return _lastError; }
            set { _lastError = value; }
        }



        [BrowsableAttribute(true)]
        [DescriptionAttribute("The name of this maze collection.")]
        public new string Name
        {
            get { return collectionName; }
            set 
            { 
                collectionName = value;
                _isDirty = true;
            }
        }

        [DescriptionAttribute("The name of the person who created this maze.")]
        public string AuthorName
        {
            get { return authorName; }
            set 
            { 
                authorName = value;
                _isDirty = true;
            }
        }

        [DescriptionAttribute("The email address of the person who created this maze.")]
        public string AuthorEmail
        {
            get { return authorEmail; }
            set 
            { 
                authorEmail = value;
                _isDirty = true;
            }
        }

        [BrowsableAttribute(false)]
        public bool IsDirty
        {
            get
            {
                if (_isDirty) return true;
                foreach(Maze maze in _mazes) { 
                    if (maze.IsDirty) return true;
                }
                return false;
            }
            set
            {
                _isDirty = value;
            }
        }

        [BrowsableAttribute(false)]
        public bool Error
        {
            get { return _error; }
            set { _error = value; }
        }

        #endregion

        #region ISerializable

        //Deserialization constructor.
        public MazeCollection(SerializationInfo info, StreamingContext ctxt)
        {
            collectionName = (string)info.GetValue("Name", typeof(string));
            authorName = (string)info.GetValue("AuthorName", typeof(string));
            authorEmail = (string)info.GetValue("AuthorEmail", typeof(string));
            //fileName = (string)info.GetValue("FileName", typeof(string));
            //Width = (int)info.GetValue("Width", typeof(int));
            //Height = (int)info.GetValue("Height", typeof(int));
            //AllowDrop = (bool)info.GetValue("AllowDrop", typeof(bool));
            _isDirty = (bool)info.GetValue("IsDirty", typeof(bool));
        }
                
        ////Serialization function.
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Name", collectionName);
            info.AddValue("AuthorName", authorName);
            info.AddValue("AuthorEmail", authorEmail);
            //info.AddValue("FileName", fileName);
            //info.AddValue("Width", Width);
            //info.AddValue("Height", Height);
            //info.AddValue("AllowDrop", AllowDrop);
            info.AddValue("IsDirty", false);
        }

        #endregion

        //#region ICustomTypeDescriptor

        //private PropertyDescriptorCollection FilterProperties(PropertyDescriptorCollection pdc)
        //{
        //    ArrayList toInclude = new ArrayList();
        //    foreach (string s in NamesToInclude)
        //        toInclude.Add(s);

        //    PropertyDescriptorCollection adjustedProps = new PropertyDescriptorCollection(new PropertyDescriptor[] { });
        //    foreach (PropertyDescriptor pd in pdc)
        //        if (toInclude.Contains(pd.Name))
        //            adjustedProps.Add(pd);

        //    return adjustedProps;
        //}

        //public TypeConverter GetConverter()
        //{
        //    return TypeDescriptor.GetConverter(this, true);
        //}

        //public EventDescriptorCollection GetEvents(Attribute[] attributes)
        //{
        //    return TypeDescriptor.GetEvents(this, attributes, true);
        //}

        //EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
        //{
        //    return TypeDescriptor.GetEvents(this, true);
        //}

        //public string GetComponentName()
        //{
        //    return TypeDescriptor.GetComponentName(this, true);
        //}

        //public object GetPropertyOwner(PropertyDescriptor pd)
        //{
        //    return this;
        //}

        //public AttributeCollection GetAttributes()
        //{
        //    return TypeDescriptor.GetAttributes(this, true);
        //}

        //public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        //{
        //    PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(this, attributes, true);
        //    return FilterProperties(pdc);
        //}

        //PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties()
        //{
        //    PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(this, true);
        //    return FilterProperties(pdc);
        //}

        //public object GetEditor(Type editorBaseType)
        //{
        //    return TypeDescriptor.GetEditor(this, editorBaseType, true);
        //}

        //public PropertyDescriptor GetDefaultProperty()
        //{
        //    return TypeDescriptor.GetDefaultProperty(this, true);
        //}

        //public EventDescriptor GetDefaultEvent()
        //{
        //    return TypeDescriptor.GetDefaultEvent(this, true);
        //}

        //public string GetClassName()
        //{
        //    return TypeDescriptor.GetClassName(this, true);
        //}

        //#endregion

        //#region ITreeObject

        //public TreeNode TreeRender(TreeView treeView, TreeNode currentNode)
        //{
        //    TreeNode parentNode = null;
        //    TreeNode collectionNode = null;
        //    if (treeView != null)
        //    {
        //        try
        //        {
        //            //treeView.BeginUpdate();
        //            //see if it has a parent node...
        //            if (currentNode != null)
        //            {
        //                if (currentNode.Parent != null)
        //                {
        //                    parentNode = currentNode.Parent;
        //                }
        //            }
        //            //now, remove this node
        //            if (currentNode != null)
        //            {
        //                treeView.Nodes.Remove(currentNode);
        //            }
        //            //create the node now..
        //            if (parentNode != null)
        //            {
        //                collectionNode = parentNode.Nodes.Add(collectionName);
        //            }
        //            else
        //            {
        //                collectionNode = treeView.Nodes.Add(collectionName);
        //            }
        //            collectionNode.Name = this.Name;
        //            collectionNode.Tag = this;
        //            for(int i = 0; i < _mazes.Count; i++)
        //            {
        //                TreeNode mazeNode = new TreeNode(((int)i + 1).ToString() + ": <unassigned>");
        //                mazeNode.ForeColor = Color.Gray;
        //                mazeNode.Text = _mazes[i].Name;
        //                mazeNode.Tag = _mazes[i];
        //                mazeNode.ForeColor = Color.Black;
        //                mazeNode.ImageIndex = ((int)_mazes[i].MazeType) + 1;
        //                mazeNode.SelectedImageIndex = mazeNode.ImageIndex;
        //                collectionNode.Nodes.Add(mazeNode);
        //            }
        //            collectionNode.Expand();
        //        }
        //        catch (Exception ex)
        //        {
        //            error = true;
        //            lastError = ex.Message;
        //        }
        //        finally
        //        {
        //            treeView.EndUpdate();
        //        }
        //    }
        //    else
        //    {
        //        error = true;
        //        lastError = "Tree not defined.";
        //    }
        //    return collectionNode;
        //}

        //public void SetGridlines(bool grid)
        //{
        //    gridlines = grid;
        //}

        //public ContextMenu GetTreeContextMenu()
        //{
        //    ContextMenu menu = new ContextMenu();
        //    menu.MenuItems.Add("Test");
        //    menu.MenuItems.Add("Test2");
        //    return menu;
        //}

        //#endregion

        //#region Overrides

        ////protected override void OnPaint(PaintEventArgs e)
        ////{

        ////}

        ////protected override void OnDragOver(DragEventArgs drgevent)
        ////{
        ////    //this object does not accept drag/drop
        ////    drgevent.Effect = DragDropEffects.None;
        ////}

        //#endregion

        #region Public Methods





        public bool AddMaze(Maze maze)
        {
            if (maze != null)
            {
                    _mazes.Add(maze);
                    return true;
            }
            return false;
        }

        public void Validate()
        {
            foreach (Maze maze in _mazes)
            {
                maze.Validate();
            }
        }

        #endregion
        
    }
}
