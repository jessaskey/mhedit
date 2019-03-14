using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using mhedit.Containers.Validation;

namespace mhedit.Containers
{
    [DefaultPropertyAttribute("Name")]
    [Serializable]
    public class MazeCollection : ChangeTrackingBase
    {
        #region Declarations

        private const int MAX_MAZES = 32;
        private ExtendedObservableCollection<Maze> _mazes =
            new ExtendedObservableCollection<Maze>();
        private Guid _id = Guid.NewGuid();
        private bool _error = false;
        private string _lastError = string.Empty;
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
        public ExtendedObservableCollection<Maze> Mazes
        {
            get { return _mazes; }
        }

        [BrowsableAttribute(false)]
        [XmlIgnore]
        public bool IsValid
        {
            get
            {
                /// if any maze is NOT valid then Any returns true, so return false..
                return !this._mazes.Any( m => !m.IsValid );
            }
        }

        [BrowsableAttribute(false)]
        [XmlIgnore]
        public string ValidationMessage
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                foreach(Maze maze in _mazes.Where(m => !m.IsValid) )
                {
                    sb.AppendLine(maze.Name + ": " + maze.ValidationMessage);
                }

                return sb.ToString();
            }
        }

        [BrowsableAttribute(false)]
        [XmlIgnore]
        public string LastError
        {
            get { return _lastError; }
            set { _lastError = value; }
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

        [BrowsableAttribute(false)]
        [XmlIgnore]
        public bool Error
        {
            get { return _error; }
            set { _error = value; }
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

        #region Public Methods

        public void Validate()
        {
            //foreach (Maze maze in _mazes)
            //{
            //    maze.Validate();
            //}
        }

        #endregion
        
    }
}
