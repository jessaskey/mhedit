using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using mhedit.Containers;

namespace mhedit
{
    [DefaultPropertyAttribute("Name")]
    [Serializable]
    public class MazeCollectionController: ITreeObject, ICustomTypeDescriptor
    {
        #region Declarations

        private const int MAX_MAZES = 32;
        private readonly MazeCollection _mazeCollection;
        private string _fileName = null;
        private bool gridlines = true;
        private string exportPath = String.Empty;

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
            : this( NameFactory.Create( "MazeCollection" ) )
        {}

        public MazeCollectionController( string name )
            : this ( new MazeCollection( name ) )
        {}

        public MazeCollectionController( MazeCollection collection )
        {
            _mazeCollection = collection;

            this._mazeCollection.PropertyChanged += this.OnMazeCollectionPropertyChanged;
        }

        #endregion

        #region Properties

        [BrowsableAttribute(false)]
        public MazeCollection MazeCollection
        {
            get { return _mazeCollection; }
        }

        [ReadOnly(true)]
        [DescriptionAttribute("The filename of this collection on disk.")]
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        [ReadOnly( true )]
        [DescriptionAttribute( "The last ROM Export file location." )]
        public string ExportPath
        {
            get { return exportPath; }
            set { exportPath = value; }
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

        public TreeNode TreeRender(TreeView treeView, TreeNode currentNode, bool gridLines)
        {
            TreeNode parentNode = null;
            TreeNode collectionNode = null;
            if (treeView != null)
            {
                try
                {
                    treeView.BeginUpdate();

                    //see if it has a parent node...
                    if ( ( parentNode = currentNode?.Parent ) != null )
                    {
                        treeView.Nodes.Remove( currentNode );
                    }

                    /// Add node after selected
                    collectionNode = new TreeNode( _mazeCollection.Name )
                    {
                        Tag = this
                    };

                    treeView.Nodes.Insert( 
                        treeView.Nodes.IndexOf( treeView.SelectedNode?.Parent ?? treeView.SelectedNode ) + 1,
                        collectionNode );

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
                _mazeCollection.LastError = "Tree not defined.";
            }

            return collectionNode;
        }

        public void SetGridlines(bool grid)
        {
            gridlines = grid;
        }

        #endregion

        private void OnMazeCollectionPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
        }
    }
}
