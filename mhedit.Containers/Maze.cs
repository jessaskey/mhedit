using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using mhedit.Containers.MazeObjects;
using mhedit.Containers.MazeEnemies;
using System.Xml.Serialization;
using mhedit.Containers.MazeEnemies.IonCannon;
using mhedit.Containers.Validation;

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
    public class Maze : ChangeTrackingBase, IName
    {

        #region Declarations

        private Guid _id = Guid.NewGuid();
        private string _mazeName = null;
        private string _mazeDescription = String.Empty;
        private bool _isValid = false;
        private List<string> _validationMessage = new List<string>();
        private MazeType _mazeType;
        private string _mazeHint = String.Empty;
        private string _mazeHint2 = String.Empty;
        private int _oxygenReward = 16;
        private List<MazeWall> _mazeWallBase;
        private ExtendedObservableCollection<MazeObject> _mazeObjects =
            new ExtendedObservableCollection<MazeObject>();
        private List<bool> _transportabilityFlags = new List<bool>();
        private int _mazeStampsX = 0;
        private int _mazeStampsY = 0;

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
            _mazeName = name;

            this.MazeType = type;

            ( (INotifyPropertyChanged)this._mazeObjects ).PropertyChanged +=
                this.ForwardPropertyChanged;
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

        [Validation( typeof( CollectionContentValidator<Reactoid,MazeObject> ),
            Message = "Every Maze requires a single reactor. {0} were found.",
            Options = "Count=1")]
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
        public bool IsValid
        {
            get { return _isValid; }
            set { _isValid = value; }
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

        [BrowsableAttribute(false)]
        [XmlIgnore]
        public List<string> ValidationMessage
        {
            get { return _validationMessage; }
            set { _validationMessage = value; }
        }

        [Validation( typeof( RegexValidator ) )]
        [BrowsableAttribute(true)]
        [DescriptionAttribute("The name of the maze.")]
        public string Name
        {
            get { return this._mazeName; }
            set { this.SetField( ref this._mazeName, value ); }
        }

        [Validation( typeof( RegexValidator ),
            Options = "Pattern=^[a-zA-Z0-9 .!-,%:]*$;" )]
        [BrowsableAttribute(true)]
        [DescriptionAttribute("The text shown at the top of the screen when entering the maze. Valid characters are ' 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ..!-,%:'")]
        public string Hint
        {
            get { return _mazeHint; }
            set { this.SetField( ref this._mazeHint, value ); }
        }

        [Validation( typeof( RegexValidator ),
            Options = "Pattern=^[a-zA-Z0-9 .!-,%:]*$;" )]
        [BrowsableAttribute(true)]
        [DescriptionAttribute("The second line of text shown at the top of the screen when entering the maze. Valid characters are ' 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ..!-,%:'")]
        public string Hint2
        {
            get { return _mazeHint2; }
            set { this.SetField( ref this._mazeHint2, value ); }
        }

        [BrowsableAttribute(true)]
        [DescriptionAttribute("A description of gameplay and strategy for this maze.")]
        public string Description
        {
            get { return _mazeDescription; }
            set { this.SetField( ref this._mazeDescription, value ); }
        }

        [BrowsableAttribute(true)]
        [DescriptionAttribute("The bitflags defining which objects are able to be transported.")]
        public List<bool> TransportabilityFlags
        {
            get { return _transportabilityFlags; }
            set { _transportabilityFlags = value; }
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
        //    if (_mazeObjects != null && _mazeObjects.Count > 0)
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
