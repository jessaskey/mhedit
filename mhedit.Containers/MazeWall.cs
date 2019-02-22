using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Xml.Serialization;

namespace mhedit.Containers
{
    [Serializable]
    public class MazeWall : MazeObject
    {
        private MazeWallType _wallType = MazeWallType.Empty;
        private bool _userWall = false;
        private bool _dynamicWall = false;
        private int _dynamicWallTimeout;
        private MazeWallType _wallTypeDynamic = MazeWallType.Empty;
        private int _alternateWallTimeout;
        private int _wallIndex;

        public MazeWall()
            : this( MazeWallType.Empty )
        {}

        public MazeWall(MazeWallType type)
            : this( type, Point.Empty, 0 )
        {}

        public MazeWall( MazeWallType type, Point position, int wallIndex )
            : base( 64, ImageFactory.Create( type, false ) )
        {
            this._wallIndex = wallIndex;

            this._wallType = type;

            this.Position = position;
        }

        [DescriptionAttribute("Description of the wall type.")]
        public MazeWallType WallType
        {
            get { return _wallType; }
            set
            {
                if ( this._wallType != value )
                {
                    /// Must change Image first then property so any UX updates get proper
                    /// image.
                    this.Image = ResourceFactory.ReplaceColor(
                        ImageFactory.Create( value, this._dynamicWall ),
                        Color.Green, this._userWall ? Color.Blue : Color.Green );

                    this.SetField( ref this._wallType, value );
                }
            }
        }

        [BrowsableAttribute(false)]
        public int WallIndex
        {
            get { return _wallIndex; }
            set { _wallIndex = value; }
        }

        [BrowsableAttribute(false)]
        public bool UserWall
        {
            get { return _userWall; }
            set
            {
                if ( this._userWall != value )
                {
                    /// Must change Image first then property so any UX updates get proper
                    /// image.
                    this.Image = ResourceFactory.ReplaceColor(
                        ImageFactory.Create( this._wallType, this._dynamicWall ),
                        Color.Green, value ? Color.Blue : Color.Green );

                    this.SetField( ref this._userWall, value );
                }
            }
        }

        [CategoryAttribute("Dynamic Wall")]
        [DescriptionAttribute("A Dynamic Wall will alternate between two different wall types based upon timers for each wall.")]
        public bool IsDynamicWall
        {
            get { return _dynamicWall; }
            set
            {
                if ( this._dynamicWall != value )
                {
                    /// Must change Image first then property so any UX updates get proper
                    /// image.
                    this.Image = ResourceFactory.ReplaceColor(
                        ImageFactory.Create( this._wallType, value ),
                        Color.Green, this._userWall ? Color.Blue : Color.Green );

                    this.SetField( ref this._dynamicWall, value );
                }
            }
        }

        [CategoryAttribute("Dynamic Wall")]
        [DescriptionAttribute("Alternate Wall Type, this is the wall style that will show when the Dynamic Wall Timeout expires.")]
        public MazeWallType AlternateWallType
        {
            get { return _wallTypeDynamic; }
            set { this.SetField( ref this._wallTypeDynamic, value ); }
        }

        [CategoryAttribute("Dynamic Wall")]
        [DescriptionAttribute("Dynamic Wall Timeout. This value is in game 'frames' with 0 being a valid value and equal to about 2-3 seconds.")]
        public int DynamicWallTimout
        {
            get { return _dynamicWallTimeout; }
            set { this.SetField( ref this._dynamicWallTimeout, value ); }
        }

        [CategoryAttribute("Dynamic Wall")]
        [DescriptionAttribute("Alternate Wall Timeout. This value is in game 'frames' with 0 being a valid value and equal to about 2-3 seconds.")]
        public int AlternateWallTimeout
        {
            get { return _alternateWallTimeout; }
            set { this.SetField( ref this._alternateWallTimeout, value ); }
        }

        //public List<Vector> GetWallLines()
        //{
        //    List<Vector> wallLines = new List<Vector>();
        //    switch (_wallType)
        //    {
        //        case MazeWallType.Empty:

        //            break;
        //        case MazeWallType.Horizontal:
        //            wallLines.Add(new Vector() { Start= new Point(0 + this.Position.X, 32 + this.Position.Y), End = new Point(64 + this.Position.X, 32 + this.Position.Y)});
        //            break;
        //        case MazeWallType.LeftDown:
        //            wallLines.Add(new Vector() { Start= new Point(0 + this.Position.X, 32 + this.Position.Y), End = new Point(32 + this.Position.X, 32 + this.Position.Y)});
        //            wallLines.Add(new Vector() { Start= new Point(32 + this.Position.X, 32 + this.Position.Y), End = new Point(32 + this.Position.X, 64 + this.Position.Y)});
        //            break;
        //        case MazeWallType.LeftUp:
        //            wallLines.Add(new Vector() { Start= new Point(0 + this.Position.X, 32 + this.Position.Y), End = new Point(32 + this.Position.X, 32 + this.Position.Y)});
        //            wallLines.Add(new Vector() { Start= new Point(32 + this.Position.X, 32 + this.Position.Y), End = new Point(32 + this.Position.X, 0 + this.Position.Y)});
        //            break;
        //        case MazeWallType.RightDown:
        //            wallLines.Add(new Vector() { Start= new Point(32 + this.Position.X, 32 + this.Position.Y), End = new Point(64 + this.Position.X, 32 + this.Position.Y)});
        //            wallLines.Add(new Vector() { Start= new Point(32 + this.Position.X, 32 + this.Position.Y), End = new Point(32 + this.Position.X, 64 + this.Position.Y)});
        //            break;
        //        case MazeWallType.RightUp:
        //            wallLines.Add(new Vector() { Start= new Point(32 + this.Position.X, 32 + this.Position.Y), End = new Point(64 + this.Position.X, 32 + this.Position.Y)});
        //            wallLines.Add(new Vector() { Start= new Point(32 + this.Position.X, 32 + this.Position.Y), End = new Point(32 + this.Position.X, 0 + this.Position.Y)});
        //            break;
        //        case MazeWallType.Vertical:
        //            wallLines.Add(new Vector() { Start = new Point(32 + this.Position.X, 0 + this.Position.Y), End = new Point(32 + this.Position.X, 64 + this.Position.Y) });
        //            break;
        //    }
        //    return wallLines;
        //}

        private class ImageFactory
        {
            public static Image Create( MazeWallType wallType, bool dynamicWall )
            {
                Image image = null;

                if ( dynamicWall )
                {
                    switch ( wallType )
                    {
                        case MazeWallType.Empty:
                            image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.wallc_empty_64.png" );
                            break;
                        case MazeWallType.Horizontal:
                            image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.wallc_horizontal_64.png" );
                            break;
                        case MazeWallType.LeftDown:
                            image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.wallc_leftdown_64.png" );
                            break;
                        case MazeWallType.LeftUp:
                            image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.wallc_leftup_64.png" );
                            break;
                        case MazeWallType.RightDown:
                            image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.wallc_rightdown_64.png" );
                            break;
                        case MazeWallType.RightUp:
                            image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.wallc_rightup_64.png" );
                            break;
                        case MazeWallType.Vertical:
                            image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.wallc_vertical_64.png" );
                            break;
                        default:
                            //just in case
                            image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.wallc_empty_64.png" );
                            break;
                    }
                }
                else
                {
                    switch ( wallType )
                    {
                        case MazeWallType.Empty:
                            image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.wall_empty_64.png" );
                            break;
                        case MazeWallType.Horizontal:
                            image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.wall_horizontal_64.png" );
                            break;
                        case MazeWallType.LeftDown:
                            image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.wall_leftdown_64.png" );
                            break;
                        case MazeWallType.LeftUp:
                            image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.wall_leftup_64.png" );
                            break;
                        case MazeWallType.RightDown:
                            image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.wall_rightdown_64.png" );
                            break;
                        case MazeWallType.RightUp:
                            image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.wall_rightup_64.png" );
                            break;
                        case MazeWallType.Vertical:
                            image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.wall_vertical_64.png" );
                            break;
                    }
                }

                return image;
            }
        }

        public override byte[] ToBytes()
        {
            throw new Exception("Serialization of Wall requires the parent Maze, use the ToBytes(object) method instead.");
        }

        public override byte[] ToBytes(object obj)
        {
            List<byte> bytes = new List<byte>();
            if (obj is Maze)
            {
                int wallDataOffset = 18; //this is a set of blank data offsets defined in the mhavoc source for some reason
                if (_dynamicWall)
                {
                    bytes.Add((byte)(wallDataOffset + ((Maze)obj).PointToStamp(this.Position)));
                    bytes.Add((byte)_dynamicWallTimeout);
                    bytes.Add((byte)_alternateWallTimeout);
                    bytes.Add((byte)_wallType);
                    bytes.Add((byte)_wallTypeDynamic);
                }
                else
                {
                    bytes.Add((byte)(wallDataOffset + ((Maze)obj).PointToStamp(this.Position)));
                    bytes.Add((byte)_wallType);
                }
            }
            else
            {
                throw new Exception("Walls are serialized via the maze object.");
            }
            return bytes.ToArray();
        }

        protected override Image AddSelectedDecoration( Image image )
        {
            //draw little brackets in each corner
            Graphics g = Graphics.FromImage( image );
            Pen redPen = new Pen( Color.Red, 3 );
            //top left
            g.DrawLine( redPen, 0, 0, 0, 10 );
            g.DrawLine( redPen, 0, 0, 10, 0 );
            //top right
            g.DrawLine( redPen, image.Width, 0, image.Width, 10 );
            g.DrawLine( redPen, image.Width, 0, image.Width - 10, 0 );
            //bottom left
            g.DrawLine( redPen, 0, image.Height, 0, image.Height - 10 );
            g.DrawLine( redPen, 0, image.Height, 10, image.Height );
            //bottom right
            g.DrawLine( redPen, image.Width, image.Height, image.Width, image.Height - 10 );
            g.DrawLine( redPen, image.Width, image.Height, image.Width - 10, image.Height );
            return image;
        }
    }
}
