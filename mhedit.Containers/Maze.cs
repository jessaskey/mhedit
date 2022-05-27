using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using mhedit.Containers.MazeObjects;
using mhedit.Containers.MazeEnemies;
using System.Xml.Serialization;
using mhedit.Containers.MazeEnemies.IonCannon;
using mhedit.Containers.Validation;
using mhedit.Containers.Validation.MajorHavoc;

namespace mhedit.Containers
{
    public delegate void MazePropertiesUpdated(object sender);

    [DefaultPropertyAttribute("Name")]
    [Serializable]
    [XmlInclude(typeof(IonCannon))]
    [XmlInclude(typeof(LightningH))]
    [XmlInclude(typeof(LightningV))]
    [XmlInclude(typeof(Maxoid))]
    [XmlInclude(typeof(Perkoid))]
    [XmlInclude(typeof(Pyroid))]
    [XmlInclude(typeof(TripPad))]
    [XmlInclude(typeof(TripPadPyroid))]
    [XmlInclude(typeof(Arrow))]
    [XmlInclude(typeof(ArrowOut))]
    [XmlInclude(typeof(Boots))]
    [XmlInclude(typeof(KeyPouch))]
    [XmlInclude(typeof(Clock))]
    [XmlInclude(typeof(EscapePod))]
    [XmlInclude(typeof(Hand))]
    [XmlInclude(typeof(Key))]
    [XmlInclude(typeof(Lock))]
    [XmlInclude(typeof(OneWay))]
    [XmlInclude(typeof(Oxoid))]
    [XmlInclude(typeof(Reactoid))]
    [XmlInclude(typeof(Spikes))]
    [XmlInclude(typeof(Transporter))]
    [XmlInclude(typeof(MazeWall))]
    [XmlInclude(typeof(HiddenLevelToken))]
    public class Maze : ChangeTrackingBase, IFileProperties, IName
    {

        #region Declarations

        private Guid _id = Guid.NewGuid();
        private string _mazeName;
        private string _mazeDescription = String.Empty;
        private MazeType _mazeType;
        private string _mazeHint = String.Empty;
        private string _mazeHint2 = String.Empty;
        private int _oxygenReward = 16;
        private List<MazeWall> _mazeWallBase;
        private readonly ExtendedObservableCollection<MazeObject> _mazeObjects = new ExtendedObservableCollection<MazeObject>();
        private List<bool> _transportabilityFlags = new List<bool>();
        private int _mazeStampsX = 0;
        private int _mazeStampsY = 0;
        private EditInfo _created;
        private EditInfo _modified;
        private string _filename;
        private string _path;

        #endregion

        #region Constructors

        public Maze()
            : this( MazeType.TypeA, NameFactory.Create( "Maze" ) )
        { }

        public Maze(string name)
            : this( MazeType.TypeA, name )
        { }

        public Maze(MazeType type, string name)
        {
            this._mazeName = name;

            this.MazeType = type;

            this._created = new EditInfo( DateTime.Now, VersionInformation.ApplicationVersion );

            /// Must be different objects!!!
            this._modified = new EditInfo( this._created.TimeStamp, VersionInformation.ApplicationVersion );

            ( (INotifyPropertyChanged)this._mazeObjects ).PropertyChanged +=
                this.ForwardPropertyChanged;
        }

        #endregion

#region Implementation of IFileProperties

        /// <inheritdoc />
        string IFileProperties.Extension
        {
            get { return ".mhz"; }
        }

        /// <inheritdoc />
        string IFileProperties.Name
        {
            get
            {
                string name = this._filename ?? this.Name;

                return Path.HasExtension(name) ?
                           name : $"{name}{((IFileProperties)this).Extension}";
            }
            set
            {
                if ( Path.HasExtension( value ) &&
                     Path.GetExtension(value) != ((IFileProperties)this).Extension )
                {
                    throw new ArgumentException();
                }

                this._filename = value;
            }
        }

        /// <inheritdoc />
        string IFileProperties.Path
        {
            get { return this._path; }
            set { this._path = value; }
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
                    this._mazeObjects.IsChanged;
            }
        }

        public override void AcceptChanges()
        {
            /// clear composite member first.
            this._mazeObjects.AcceptChanges();

            base.AcceptChanges();
        }

        #endregion

        #region Public Properties

        [Validation( typeof( CollectionContentRule<Reactoid> ),
            Message = "Every Maze requires a single Reactoid. {4} were found.",
            Options = "Maximum=1;Minimum=1" )]
        [Validation( typeof( HandReactorLocationRule ) )]
        [Validation( typeof( KeyLockPairsRule ) )]
        [Validation( typeof( SingleColorKeyRule ) )]
        [Validation( typeof( SingleColorLockRule ) )]
        [Validation( typeof( TransporterPairsRule ) )]
        [Validation( typeof( TransporterDirectionRule ) )]
        [Validation( typeof( ElementsRule ) )]
        [BrowsableAttribute(false)]
        public ExtendedObservableCollection<MazeObject> MazeObjects
        {
            get
            {
                return _mazeObjects;
            }
        }

        [BrowsableAttribute(false)]
        [XmlIgnore]
        public List<MazeWall> MazeWallBase
        {
            get
            {
                return _mazeWallBase;
            }
        }

        [BrowsableAttribute(false)]
        [XmlIgnore]
        public Guid Id
        {
            get { return _id; }
        }

        [BrowsableAttribute(false)]
        [XmlIgnore]
        public int MazeStampsX
        {
            get { return _mazeStampsX; }
            set { _mazeStampsX = value; }
        }

        [BrowsableAttribute(false)]
        [XmlIgnore]
        public int MazeStampsY
        {
            get { return _mazeStampsY; }
            set { _mazeStampsY = value; }
        }

        /// <summary>
        /// Information about when this maze was created.
        /// </summary>
        [BrowsableAttribute( false )]
        public EditInfo Created
        {
            get { return this._created; }
            set
            {
                /// This property isn't available to the user. We first set with the constructor. If
                /// it's saved/serialized to file then the deserialization will overwrite the ctor's
                /// set and the creation data will be preserved over time/saves/loads.
                this._created = value;
            }
        }

        /// <summary>
        /// Information about when this maze was last modified.
        /// </summary>
        [BrowsableAttribute( false )]
        public EditInfo Modified
        {
            get { return this._modified; }
            set
            {
                /// This property isn't available to the user. We set with any edits. If it's
                /// changed and saved/serialized to file then the modifed time will be saved.
                /// Deserialization will overwrite but new modification data will be written
                /// for each edit and subsequently saved with the file.
                this._modified = value;
            }
        }

        [Validation( typeof( FileNameRule ) )]
        [Validation(typeof(StringLengthRule),
            Options = "Minimum=1;Maximum=50")]
        [BrowsableAttribute(true)]
        [DescriptionAttribute("The name of the maze.")]
        public string Name
        {
            get { return this._mazeName; }
            set
            {
                if (string.IsNullOrWhiteSpace(value) )
                {
                    throw new ArgumentException("Is null or empty.", nameof(this.Name));
                }
                if (value.Length > 50)
                {
                    throw new ArgumentException("Is longer than 50 characters.", nameof(this.Name));
                }
                if ( value.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                {
                    string invalid = new string(Path.GetInvalidFileNameChars()
                                                    .Where(c => !char.IsControl(c))
                                                    .ToArray());

                    throw new ArgumentException($"\"{value}\" contains invalid characters: {invalid}", nameof(this.Name));
                }

                this.SetField( ref this._mazeName, value );
            }
        }

        [Validation( typeof( MazeHintRule ),
            Message = "Maze Hint: {1}" )]
        //[Validation( typeof( StringExistsRule ),
        //    Level = ValidationLevel.Warning,
        //    Message = "Maze Hint: {1}" )]
        [Validation(typeof(StringLengthRule),
            Options = "Minimum=0;Maximum=50")]
        [BrowsableAttribute(true)]
        [DescriptionAttribute("The text shown at the top of the screen when entering the maze. Valid characters are ' 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ..!-,%:'")]
        public string Hint
        {
            get { return _mazeHint; }
            set { this.SetField( ref this._mazeHint, value ); }
        }

        [Validation( typeof( MazeHintRule ),
            Message = "Maze Hint2: {1}" )]
        [Validation( typeof( StringExistsRule ),
            Level = ValidationLevel.Warning,
            Message = "Maze Hint2: {1}" )]
        [Validation(typeof(StringLengthRule),
            Options = "Minimum=0;Maximum=50")]
        [BrowsableAttribute(true)]
        [DescriptionAttribute("The second line of text shown at the top of the screen when entering the maze. Valid characters are ' 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ..!-,%:'")]
        public string Hint2
        {
            get { return _mazeHint2; }
            set { this.SetField( ref this._mazeHint2, value ); }
        }

        [Validation(typeof(StringLengthRule),
            Options = "Minimum=0;Maximum=500")]
        [BrowsableAttribute(true)]
        [DescriptionAttribute("A description of gameplay and strategy for this maze.")]
        public string Description
        {
            get { return _mazeDescription; }
            set { this.SetField( ref this._mazeDescription, value ); }
        }

        [DescriptionAttribute("The structure type of the maze.")]
        public MazeType MazeType
        {
            get { return _mazeType; }
            set
            {
                InitBaseMap( value );
                this.SetField( ref this._mazeType, value );
            }
        }

        [DescriptionAttribute("The Oxygen reward value on this maze.")]
        [Validation( typeof( RangeRule<int> ),
            Options = "Minimum=1;Maximum=32" )]
        public int OxygenReward
        {
            get { return _oxygenReward; }
            set { this.SetField( ref this._oxygenReward, value ); }
        }

        #endregion

        #region Public Methods

        //public void Validate()
        //{
        //    _isValid = false;
        //    _validationMessage.Clear();

        //    //validate here...
        //    if (_mazeObjects != null)
        //    {
        //        if (_mazeObjects.Where(o => o is MazeObjects.Reactoid).FirstOrDefault() == null)
        //        {
        //            _validationMessage.Add("ERROR/REQUIRED: Maze does not contain a reactor.");
        //        }
        //    }

        //    _isValid = (_validationMessage.Count == 0);
        //}

        public int GetObjectTypeCount(Type type)
        {
            int count = 0;
            for (int i = 0; i < _mazeObjects.Count; i++)
            {
                if (_mazeObjects[i].GetType() == type) count++;
            }
            return count;
        }

        #endregion

#region Overrides of Object

        /// <inheritdoc />
        public override string ToString()
        {
            return this.Name;
        }

#endregion

        private void InitBaseMap( MazeType type )
        {
            //initialize our base maze maps
            _mazeWallBase = null;
            MazeFactory.MazeBaseData mazeBaseData = MazeFactory.GetBaseMap( type );
            _mazeStampsX = mazeBaseData.mazeStampsX;
            _mazeStampsY = mazeBaseData.mazeStampsY;
            _mazeWallBase = mazeBaseData.mazeWallBase;
        }

        public bool AddObject(object obj)
        {
            bool wasAdded = false;
            MazeObject mazeObject = obj as MazeObject;

            if (mazeObject != null)
            {
                if (((MazeObject)obj).MaxObjects > GetObjectTypeCount(obj.GetType()))
                {
                    MazeWall wall = obj as MazeWall;
                    if (wall != null)
                    {
                        wall.Name = NameFactory.Create( obj.GetType().Name );
                        wall.Position = wall.GetAdjustedPosition( wall.Position);
                        _mazeObjects.Add((MazeObject)obj);
                        wasAdded = true;
                    }
                    else
                    {
                        mazeObject.Name = NameFactory.Create( obj.GetType().Name );
                        _mazeObjects.Add((MazeObject)obj);
                        wasAdded = true;
                    }
                }
                else
                {
                    MessageBox.Show( $"You can't add any more {obj.GetType().Name} objects.",
                        "The Homeworld is near", MessageBoxButtons.OK, MessageBoxIcon.Warning );
                }
            }
            return wasAdded;
        }

        public int PointToStamp( Point point )
        {
            int row = point.X / ( DataConverter.CanvasGridSize );
            int col = point.Y / ( DataConverter.CanvasGridSize );
            return Math.Max( Math.Min( ( col * this.MazeStampsX ) + row, this.MazeWallBase.Count ), 0 );
        }

        public Point PointFromStamp( int stamp )
        {
            int col = stamp % this.MazeStampsX;
            int row = stamp / this.MazeStampsX;
            return new Point( col * DataConverter.CanvasGridSize, row * DataConverter.CanvasGridSize );
        }

    }
}
