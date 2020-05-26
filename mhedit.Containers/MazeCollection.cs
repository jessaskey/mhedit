using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using mhedit.Containers.Validation;
using mhedit.Containers.Validation.MajorHavoc;

namespace mhedit.Containers
{
    [DefaultPropertyAttribute("Name")]
    [Serializable]
    public class MazeCollection : ChangeTrackingBase, IName
    {
        #region Declarations

        private readonly ExtendedObservableCollection<Maze> _mazes =
            new ExtendedObservableCollection<Maze>();
        private Guid _id = Guid.NewGuid();
        private string _collectionName;
        private string _authorName = string.Empty;
        private string _authorEmail = string.Empty;

        #endregion

        #region Constructor

        public MazeCollection()
            : this( NameFactory.Create( "MazeCollection" ) )
        {}

        public MazeCollection( string name )
        {
            _collectionName = name;

            ( (INotifyPropertyChanged)this._mazes ).PropertyChanged +=
                this.ForwardPropertyChanged;
        }

        #endregion

        #region Properties

        [BrowsableAttribute(false)]
        [XmlIgnore]
        public Guid Id
        {
            get { return _id; }
        }

        [BrowsableAttribute(false)]
        [Validation( typeof( CollectionSizeRule ),
            Options = "Minimum=1;Maximum=32" )]
        //[Validation( typeof( MazeTypeOrderRule) )]
        [Validation( typeof(HiddenLevelTokensRule) )]
        [Validation( typeof(HiddenLevelTokensCountRule),
            Options = "Minimum=0;Maximum=4" )]
        //[Validation( typeof( HiddenLevelTokenMissingRule ),
        //    Level = ValidationLevel.Warning )]
        [Validation( typeof( ElementsRule ) )]
        public ExtendedObservableCollection<Maze> Mazes
        {
            get { return _mazes; }
        }

        [BrowsableAttribute(true)]
        [DescriptionAttribute("The name of this maze collection.")]
        public string Name
        {
            get { return _collectionName; }
            set { this.SetField( ref this._collectionName, value ); }
        }

        [DescriptionAttribute("The name of the person who created this maze.")]
        public string AuthorName
        {
            get { return _authorName; }
            set { this.SetField( ref this._authorName, value ); }
        }

        [DescriptionAttribute("The email address of the person who created this maze.")]
        public string AuthorEmail
        {
            get { return _authorEmail; }
            set { this.SetField( ref this._authorEmail, value ); }
        }

        #endregion

        #region Implementation of IChangeTracking

        [BrowsableAttribute( false )]
        [XmlIgnore]
        public override bool IsChanged
        {
            get
            {
                return base.IsChanged |
                    this._mazes.IsChanged;
            }
        }

        public override void AcceptChanges()
        {
            /// clear composite member first.
            this._mazes.AcceptChanges();

            base.AcceptChanges();
        }

        #endregion
        
    }
}
