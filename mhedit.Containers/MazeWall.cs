using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace mhedit.Containers
{
    /// <summary>
    /// Describes the different wall types available.
    /// </summary>
    public enum MazeWallType : int
    {
        /// <summary>
        /// Horizontal Wall Type
        /// </summary>
        Horizontal = 1,
        /// <summary>
        /// Left Down Wall Type
        /// </summary>
        LeftDown,
        /// <summary>
        /// Left Up Wall Type
        /// </summary>
        LeftUp,
        /// <summary>
        /// Right Up Wall Type
        /// </summary>
        RightUp,
        /// <summary>
        /// Right Down Wall Type
        /// </summary>
        RightDown,
        /// <summary>
        /// Vertical Wall Type
        /// </summary>
        Vertical,
        /// <summary>
        /// Empty Wall Type 
        /// </summary>
        Empty
    }

    [Serializable]
    public class MazeWall : MazeObject
    {
        private const int _SNAP_X = 64;
        private const int _SNAP_Y = 64;
        private const int _MAXOBJECTS = 64;

        private MazeWallType _wallType = MazeWallType.Empty;
        private bool _userWall = false;
        private Image _img;
        private Point _position;
        private bool _dynamicWall = false;
        private int _dynamicWallTimeout;
        private MazeWallType _wallTypeDynamic = MazeWallType.Empty;
        private int _alternateWallTimeout;
        private int _wallIndex;

        public MazeWall(MazeWallType type, Point position, int wallIndex)
        {
            InitWall(type, position);
            _wallIndex = wallIndex;
        }

        public MazeWall(MazeWallType type)
        {
            InitWall(type, new Point(0,0));
        }

        private void InitWall(MazeWallType type, Point position)
        {
            _wallType = type;
            _position = position;
            //base.mazeObjectType = MazeObjectType.Wall;
            LoadDefaultImage();
        }

        [DescriptionAttribute("Maximum number of walls allowed in this maze.")]
        public override int MaxObjects
        {
            get { return _MAXOBJECTS; }
        }

        [DescriptionAttribute("Description of the wall type.")]
        public MazeWallType WallType
        {
            get { return _wallType; }
            set { _wallType = value; }
        }

        [BrowsableAttribute(false)]
        public override Size Size
        {
            get 
            {
                return _img.Size;
            }
        }

        [BrowsableAttribute(false)]
        public int WallIndex
        {
            get
            {
                return _wallIndex;
            }
            set
            {
                _wallIndex = value;
            }
        }

        [CategoryAttribute("Location")]
        [DescriptionAttribute("The location of the wall in the maze.")]
        public override Point Position
        {
            get { return _position; }
            set { _position = value; }
        }

        [BrowsableAttribute(false)]
        public override Point SnapSize
        {
            get { return new Point(_SNAP_X, _SNAP_Y); }
        }

        [BrowsableAttribute(false)]
        public bool UserWall
        {
            get { return _userWall; }
            set { _userWall = value; }
        }

        [CategoryAttribute("Dynamic Wall")]
        [DescriptionAttribute("A Dynamic Wall will alternate between two different wall types based upon timers for each wall.")]
        public bool IsDynamicWall
        {
            get { return _dynamicWall; }
            set { _dynamicWall = value; }
        }

        [CategoryAttribute("Dynamic Wall")]
        [DescriptionAttribute("Alternate Wall Type, this is the wall style that will show when the Dynamic Wall Timeout expires.")]
        public MazeWallType AlternateWallType
        {
            get { return _wallTypeDynamic; }
            set { _wallTypeDynamic = value; }
        }

        [CategoryAttribute("Dynamic Wall")]
        [DescriptionAttribute("Dynamic Wall Timeout. This value is in game 'frames' with 0 being a valid value and equal to about 2-3 seconds.")]
        public int DynamicWallTimout
        {
            get { return _dynamicWallTimeout; }
            set { _dynamicWallTimeout = value; }
        }

        [CategoryAttribute("Dynamic Wall")]
        [DescriptionAttribute("Alternate Wall Timeout. This value is in game 'frames' with 0 being a valid value and equal to about 2-3 seconds.")]
        public int AlternateWallTimeout
        {
            get { return _alternateWallTimeout; }
            set { _alternateWallTimeout = value; }
        }

        public List<Vector> GetWallLines()
        {
            List<Vector> wallLines = new List<Vector>();
            switch (_wallType)
            {
                case MazeWallType.Empty:

                    break;
                case MazeWallType.Horizontal:
                    wallLines.Add(new Vector() { Start= new Point(0 + _position.X, 32 + _position.Y), End = new Point(64 + _position.X, 32 + _position.Y)});
                    break;
                case MazeWallType.LeftDown:
                    wallLines.Add(new Vector() { Start= new Point(0 + _position.X, 32 + _position.Y), End = new Point(32 + _position.X, 32 + _position.Y)});
                    wallLines.Add(new Vector() { Start= new Point(32 + _position.X, 32 + _position.Y), End = new Point(32 + _position.X, 64 + _position.Y)});
                    break;
                case MazeWallType.LeftUp:
                    wallLines.Add(new Vector() { Start= new Point(0 + _position.X, 32 + _position.Y), End = new Point(32 + _position.X, 32 + _position.Y)});
                    wallLines.Add(new Vector() { Start= new Point(32 + _position.X, 32 + _position.Y), End = new Point(32 + _position.X, 0 + _position.Y)});
                    break;
                case MazeWallType.RightDown:
                    wallLines.Add(new Vector() { Start= new Point(32 + _position.X, 32 + _position.Y), End = new Point(64 + _position.X, 32 + _position.Y)});
                    wallLines.Add(new Vector() { Start= new Point(32 + _position.X, 32 + _position.Y), End = new Point(32 + _position.X, 64 + _position.Y)});
                    break;
                case MazeWallType.RightUp:
                    wallLines.Add(new Vector() { Start= new Point(32 + _position.X, 32 + _position.Y), End = new Point(64 + _position.X, 32 + _position.Y)});
                    wallLines.Add(new Vector() { Start= new Point(32 + _position.X, 32 + _position.Y), End = new Point(32 + _position.X, 0 + _position.Y)});
                    break;
                case MazeWallType.Vertical:
                    wallLines.Add(new Vector() { Start = new Point(32 + _position.X, 0 + _position.Y), End = new Point(32 + _position.X, 64 + _position.Y) });
                    break;
            }
            return wallLines;
        }

        private void LoadDefaultImage()
        {
            if (_dynamicWall)
            {
                switch (_wallType)
                {
                    case MazeWallType.Empty:
                        _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.wallc_empty_64.ico");
                        break;
                    case MazeWallType.Horizontal:
                        _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.wallc_horizontal_64.ico");
                        break;
                    case MazeWallType.LeftDown:
                        _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.wallc_leftdown_64.ico");
                        break;
                    case MazeWallType.LeftUp:
                        _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.wallc_leftup_64.ico");
                        break;
                    case MazeWallType.RightDown:
                        _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.wallc_rightdown_64.ico");
                        break;
                    case MazeWallType.RightUp:
                        _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.wallc_rightup_64.ico");
                        break;
                    case MazeWallType.Vertical:
                        _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.wallc_vertical_64.ico");
                        break;
                    default:
                        //just in case
                        _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.wallc_empty_64.ico");
                        break;
                }
            }
            else
            {
                switch (_wallType)
                {
                    case MazeWallType.Empty:
                        _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.wall_empty_64.ico");
                        break;
                    case MazeWallType.Horizontal:
                        _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.wall_horizontal_64.ico");
                        break;
                    case MazeWallType.LeftDown:
                        _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.wall_leftdown_64.ico");
                        break;
                    case MazeWallType.LeftUp:
                        _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.wall_leftup_64.ico");
                        break;
                    case MazeWallType.RightDown:
                        _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.wall_rightdown_64.ico");
                        break;
                    case MazeWallType.RightUp:
                        _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.wall_rightup_64.ico");
                        break;
                    case MazeWallType.Vertical:
                        _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.wall_vertical_64.ico");
                        break;
                }
            }
        }

        [BrowsableAttribute(false)]
        public override byte[] ToBytes()
        {
            throw new Exception("Serialization of Wall requires the parent Maze, use the ToBytes(object) method instead.");
        }

        [BrowsableAttribute(false)]
        public override byte[] ToBytes(object obj)
        {
            List<byte> bytes = new List<byte>();
            if (obj is Maze)
            {
                int wallDataOffset = 18; //this is a set of blank data offsets defined in the mhavoc source for some reason
                if (_dynamicWall)
                {
                    bytes.Add((byte)(wallDataOffset + ((Maze)obj).PointToStamp(_position)));
                    bytes.Add((byte)_dynamicWallTimeout);
                    bytes.Add((byte)_alternateWallTimeout);
                    bytes.Add((byte)_wallType);
                    bytes.Add((byte)_wallTypeDynamic);
                }
                else
                {
                    bytes.Add((byte)(wallDataOffset + ((Maze)obj).PointToStamp(_position)));
                    bytes.Add((byte)_wallType);
                }
            }
            else
            {
                throw new Exception("Walls are serialized via the maze object.");
            }
            return bytes.ToArray();
        }

        [BrowsableAttribute(false)]
        public override Image Image 
        {
            get
            {
                LoadDefaultImage();
                if (_userWall)
                {
                    _img = ResourceFactory.ReplaceColor(_img, Color.Green, Color.Blue);
                }

                if (selected)
                {
                    //draw little brackets in each corner
                    Graphics g = Graphics.FromImage(_img);
                    Pen redPen = new Pen(Color.Red, 3);
                    //top left
                    g.DrawLine(redPen, 0, 0, 0, 10);
                    g.DrawLine(redPen, 0, 0, 10, 0);
                    //top right
                    g.DrawLine(redPen, _img.Width, 0, _img.Width, 10);
                    g.DrawLine(redPen, _img.Width, 0, _img.Width - 10, 0);
                    //bottom left
                    g.DrawLine(redPen, 0, _img.Height, 0, _img.Height - 10);
                    g.DrawLine(redPen, 0, _img.Height, 10, _img.Height);
                    //bottom right
                    g.DrawLine(redPen, _img.Width, _img.Height, _img.Width, _img.Height - 10);
                    g.DrawLine(redPen, _img.Width, _img.Height, _img.Width - 10, _img.Height);
                }
                return _img;
            }
        }

        

    }
}
