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
using mhedit.Containers.MazeEnemies;
using System.Xml.Serialization;
using mhedit.Containers.MazeEnemies.IonCannon;

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
    [XmlInclude(typeof( MazeWall ))]
    public class Maze 
    {

        #region Declarations

        public const int MAXWALLS = 209;

        private const int GRIDUNITS = 8;
        private const int GRIDUNITSTAMPS = 8;
        //private Point objectOffset = new Point(-16, 16);

        private Guid _id = Guid.NewGuid();
        private string _mazeName = null;
        private string _mazeDescription = String.Empty;
        private bool _isDirty = false;
        private bool _isValid = false;
        private List<string> _validationMessage = new List<string>();
        private MazeType _mazeType;
        private string _mazeHint = String.Empty;
        private string _mazeHint2 = String.Empty;
        private int _oxygenReward = 16;
        private List<MazeWall> _mazeWallBase;
        private List<MazeObject> _mazeObjects;
        private List<bool> _transportabilityFlags = new List<bool>();
        private int _mazeStampsX = 0;
        private int _mazeStampsY = 0;

        #endregion

        #region Constructors

        public Maze()
        {
            _mazeType = MazeType.TypeA;
            Init();
        }

        public Maze(string name)
        {
            _mazeName = name;
            _mazeType = MazeType.TypeA;
            Init();
        }

        public Maze(MazeType type, string name)
        {
            _mazeType = type;
            _mazeName = name;
            Init();
        }

        #endregion

        #region Public Properties

        [BrowsableAttribute(false)]
        public List<MazeObject> MazeObjects
        {
            get
            {
                return _mazeObjects;
            }
            set
            {
                _mazeObjects = value;
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
            set
            {
                _mazeWallBase = value;
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
        public bool IsDirty
        {
            get {return _isDirty;}
            set { _isDirty = value; }
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

        [BrowsableAttribute(true)]
        [DescriptionAttribute("The name of the maze.")]
        public string Name
        {
            get { return _mazeName; }
            set { _mazeName = value; }
        }

        [BrowsableAttribute(true)]
        [DescriptionAttribute("The text shown at the top of the screen when entering the maze. Valid characters are ' 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ..!-,%:'")]
        public string Hint
        {
            get { return _mazeHint; }
            set { _mazeHint = value; }
        }

        [BrowsableAttribute(true)]
        [DescriptionAttribute("The second line of text shown at the top of the screen when entering the maze. Valid characters are ' 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ..!-,%:'")]
        public string Hint2
        {
            get { return _mazeHint2; }
            set { _mazeHint2 = value; }
        }

        [BrowsableAttribute(true)]
        [DescriptionAttribute("A description of gameplay and strategy for this maze.")]
        public string Description
        {
            get { return _mazeDescription; }
            set { _mazeDescription = value; }
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
                _mazeType = value;
                InitBaseMap();
            }
        }

        [DescriptionAttribute("The Oxygen reward value on this maze.")]
        public int OxygenReward
        {
            get { return _oxygenReward; }
            set
            {
                _oxygenReward = value ;
            }
        }

        #endregion

       
       
        #region Public Methods

        public void Validate()
        {
            _isValid = false;
            _validationMessage.Clear();

            //validate here...
            if (_mazeObjects != null && _mazeObjects.Count > 0)
            {
                if (_mazeObjects.Where(o => o is MazeObjects.Reactoid).FirstOrDefault() == null)
                {
                    _validationMessage.Add("ERROR/REQUIRED: Maze does not contain a reactor.");
                }
            }

            _isValid = (_validationMessage.Count == 0);
        }

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

        private void Init()
        {
            _mazeWallBase = new List<MazeWall>();
            _mazeObjects = new List<MazeObject>();
            InitBaseMap();
        }

        private void InitBaseMap()
        {
            //initialize our base maze maps
            _mazeWallBase = null;
            MazeFactory.MazeBaseData mazeBaseData = MazeFactory.GetBaseMap(_mazeType);
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
                    string type = obj.GetType().ToString();
                    string[] typeString = type.Split('.');

                    MazeWall wall = obj as MazeWall;
                    if (wall != null)
                    {
                        wall.Name = GetNextName(typeString[typeString.Length - 1].ToLower());
                        wall.Position = GetAdjustedPosition((MazeObject)wall, wall.Position);
                        _mazeObjects.Add((MazeObject)obj);
                        wasAdded = true;
                    }
                    else
                    {
                        mazeObject.Name = GetNextName(typeString[typeString.Length - 1].ToLower());
                        _mazeObjects.Add((MazeObject)obj);
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
            return Math.Max(Math.Min((col * _mazeStampsX) + row, MAXWALLS), 0);
        }

        public Point PointFromStamp(int stamp)
        {
            int col = stamp % _mazeStampsX;
            int row = stamp / _mazeStampsX;
            return new Point(col * GRIDUNITS * GRIDUNITSTAMPS, row * GRIDUNITS * GRIDUNITSTAMPS);
        }
        
        private string GetNextName(string prefix)
        {
            for (int i = 1; i > 0; i++)
            {
                if (_mazeObjects.Where(o=>o.Name == prefix + i.ToString()).FirstOrDefault() == null)
                {
                    return prefix + i.ToString();
                }
            }
            return prefix;
        }

        
    }
}
