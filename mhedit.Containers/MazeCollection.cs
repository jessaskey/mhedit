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
    public class MazeCollection : ITreeObject, ICustomTypeDescriptor
    {
        #region Declarations

        private const int MAX_MAZES = 16;
        private List<Maze> mazes;
        private bool error = false;
        private bool isDirty = false;
        private string fileName = null;
        private string lastError = "";
        private string collectionName = "<new maze collection>";
        private string authorName = null;
        private string authorEmail = null;
        private bool gridlines = true;
        private string exportPath = String.Empty;
        //private PropertyGrid propertyGrid = null;
        #endregion

        #region PropertyIncludes

        private string[] NamesToInclude =
        {
          "Name",
          "FileName"
        };

        #endregion

        #region Constructor

        public MazeCollection()
        {
            Init();
        }

        public MazeCollection(string name)
        {
            collectionName = name;
            Init();
        }

        #endregion

        #region Properties

        [BrowsableAttribute(false)]
        public PropertyGrid PropertyGrid
        {
            //get { return propertyGrid; }
            set
            {
                //propertyGrid = value;
                for (int i = 0; i < MAX_MAZES; i++)
                {
                    mazes[i].PropertyGrid = value;
                }
            }
        }

        [BrowsableAttribute(false)]
        public List<Maze> Mazes
        {
            get { return mazes; }
            set { mazes = value; }
        }

        [BrowsableAttribute(false)]
        public int MazeCount
        {
            get { return MAX_MAZES; }
        }

        [BrowsableAttribute(false)]
        public bool IsValid
        {
            get
            {
                if (mazes.Where(m => !m.IsValid).Count() > 0)
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
                foreach(Maze maze in mazes.Where(m => !m.IsValid))
                {
                    sb.AppendLine(maze.Name + ": " + maze.ValidationMessage);
                }
                return sb.ToString();
            }
        }

        [BrowsableAttribute(false)]
        public string LastError
        {
            get { return lastError; }
        }

        [ReadOnly(true)]
        [DescriptionAttribute("The filename of this collection on disk.")]
        public string FileName
        {
            get { return fileName; }
            set { 
                fileName = value;
                isDirty = true;
            }
        }

        [ReadOnly(true)]
        [DescriptionAttribute("The last ROM Export file location.")]
        public string ExportPath
        {
            get { return exportPath; }
            set { 
                exportPath = value;
                isDirty = true;
            }
        }

        [BrowsableAttribute(true)]
        [DescriptionAttribute("The name of this maze collection.")]
        public new string Name
        {
            get { return collectionName; }
            set 
            { 
                collectionName = value;
                isDirty = true;
            }
        }

        [DescriptionAttribute("The name of the person who created this maze.")]
        public string AuthorName
        {
            get { return authorName; }
            set 
            { 
                authorName = value;
                isDirty = true;
            }
        }

        [DescriptionAttribute("The email address of the person who created this maze.")]
        public string AuthorEmail
        {
            get { return authorEmail; }
            set 
            { 
                authorEmail = value;
                isDirty = true;
            }
        }

        [BrowsableAttribute(false)]
        public bool IsDirty
        {
            get
            {
                if (isDirty) return true;
                for (int i = 0; i < MAX_MAZES; i++)
                {
                    if (mazes[i] != null)
                    {
                        if (mazes[i].IsDirty) return true;
                    }
                }
                return false;

            }
        }

        #endregion

        #region ISerializable

        //Deserialization constructor.
        public MazeCollection(SerializationInfo info, StreamingContext ctxt)
        {
            collectionName = (string)info.GetValue("Name", typeof(string));
            authorName = (string)info.GetValue("AuthorName", typeof(string));
            authorEmail = (string)info.GetValue("AuthorEmail", typeof(string));
            fileName = (string)info.GetValue("FileName", typeof(string));
            //Width = (int)info.GetValue("Width", typeof(int));
            //Height = (int)info.GetValue("Height", typeof(int));
            //AllowDrop = (bool)info.GetValue("AllowDrop", typeof(bool));
            isDirty = (bool)info.GetValue("IsDirty", typeof(bool));
        }
                
        ////Serialization function.
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Name", collectionName);
            info.AddValue("AuthorName", authorName);
            info.AddValue("AuthorEmail", authorEmail);
            info.AddValue("FileName", fileName);
            //info.AddValue("Width", Width);
            //info.AddValue("Height", Height);
            //info.AddValue("AllowDrop", AllowDrop);
            info.AddValue("IsDirty", false);
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

        public TreeNode TreeRender(TreeView treeView, TreeNode currentNode)
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
                        collectionNode = parentNode.Nodes.Add(collectionName);
                    }
                    else
                    {
                        collectionNode = treeView.Nodes.Add(collectionName);
                    }
                    collectionNode.Name = this.Name;
                    collectionNode.Tag = this;
                    for (int i = 0; i < MAX_MAZES; i++)
                    {
                        if (mazes[i] != null)
                        {
                            TreeNode mazeNode = new TreeNode(((int)i + 1).ToString() + ": <unassigned>");
                            mazeNode.ForeColor = Color.Gray;
                            mazeNode.Text = mazes[i].Name;
                            mazeNode.Tag = mazes[i];
                            mazeNode.ForeColor = Color.Black;
                            mazeNode.ImageIndex = ((int)mazes[i].MazeType) + 1;
                            mazeNode.SelectedImageIndex = mazeNode.ImageIndex;
                            collectionNode.Nodes.Add(mazeNode);
                        }

                    }
                    collectionNode.Expand();
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

        public int FindMaze(Maze maze)
        {
            for (int i = 0; i < MAX_MAZES; i++)
            {
                if (mazes[i] == maze)
                {
                    return i;
                }
            }
            return -1;
        }



        public bool InsertMaze(int index, Maze maze)
        {
            if (maze != null)
            {
                if (index >= 0 && index < MAX_MAZES)
                {
                    mazes[index] = null;
                    mazes[index] = maze;
                    return true;
                }
            }
            return false;
        }

        public void Validate()
        {
            foreach (Maze maze in mazes)
            {
                maze.Validate();
            }
        }

        #endregion

        #region Private Methods

        private void Init()
        {
            mazes = new List<Maze>();
            mazes.Add(new Maze(MazeType.TypeA, "Level 1"));
            mazes.Add(new Maze(MazeType.TypeB, "Level 2"));
            mazes.Add(new Maze(MazeType.TypeC, "Level 3"));
            mazes.Add(new Maze(MazeType.TypeD, "Level 4"));
            mazes.Add(new Maze(MazeType.TypeA, "Level 5"));
            mazes.Add(new Maze(MazeType.TypeB, "Level 6"));
            mazes.Add(new Maze(MazeType.TypeC, "Level 7"));
            mazes.Add(new Maze(MazeType.TypeD, "Level 8"));
            mazes.Add(new Maze(MazeType.TypeA, "Level 9"));
            mazes.Add(new Maze(MazeType.TypeB, "Level 10"));
            mazes.Add(new Maze(MazeType.TypeC, "Level 11"));
            mazes.Add(new Maze(MazeType.TypeD, "Level 12"));
            mazes.Add(new Maze(MazeType.TypeA, "Level 13"));
            mazes.Add(new Maze(MazeType.TypeB, "Level 14"));
            mazes.Add(new Maze(MazeType.TypeC, "Level 15"));
            mazes.Add(new Maze(MazeType.TypeD, "Level 16"));
            mazes.Add(new Maze(MazeType.TypeA, "Level 17"));
            mazes.Add(new Maze(MazeType.TypeB, "Level 18"));
            mazes.Add(new Maze(MazeType.TypeC, "Level 19"));
            mazes.Add(new Maze(MazeType.TypeD, "Level 20"));
            mazes.Add(new Maze(MazeType.TypeA, "Level 21"));
            mazes.Add(new Maze(MazeType.TypeB, "Level 22"));
            mazes.Add(new Maze(MazeType.TypeC, "Level 23"));
            mazes.Add(new Maze(MazeType.TypeD, "Level 24"));
        }

        #endregion

    }
}
