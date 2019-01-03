using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Runtime.Serialization;
using System.Text;

namespace mhedit.Containers.MazeEnemies
{
    /// <summary>
    /// The Cannon class shows the Ion Cannon in the maze.
    /// </summary>
    [Serializable]
    public class Cannon : MazeObject, ISerializable
    {
        private const int _SNAP_X = 4;
        private const int _SNAP_Y = 4;
        private const int _MAXOBJECTS = 4;

        private Point _position;
        private Image _img;
        private List<iCannonMovement> _movements = null;

        public Cannon()
        {
            _movements = new List<iCannonMovement>();
            LoadDefaultImage();
            renderOffset.X = 32;
            renderOffset.Y = 32;
        }

        [BrowsableAttribute(false)]
        public override Size Size
        {
            get { return _img.Size; }
        }

        [CategoryAttribute("Location")]
        [DescriptionAttribute("The location of the object in the maze.")]
        public override Point Position
        {
            get { return _position; }
            set { _position = value; }
        }

        [DescriptionAttribute("Maximum number of cannon's allowed in this maze.")]
        public override int MaxObjects
        {
            get { return _MAXOBJECTS; }
        }

        [CategoryAttribute("Custom")]
        [DescriptionAttribute("The movement script for the cannon")]
        [EditorAttribute(typeof(CannonEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public List<iCannonMovement> Movements
        {
            get { return _movements; }
            set { _movements = value; }
        }

        [BrowsableAttribute(false)]
        public override Point SnapSize
        {
            get { return new Point(_SNAP_X, _SNAP_Y); }
        }

        [BrowsableAttribute(false)]
        public override Image Image
        {
            get
            {
                LoadDefaultImage();
                if (selected)
                {
                    _img = base.ImageSelected(_img);
                }
                return _img;
            }
        }

        [BrowsableAttribute(false)]
        public override byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(DataConverter.PointToByteArrayLong(DataConverter.ConvertPixelsToVector(_position)));
            //now cannon commands
            foreach (iCannonMovement movement in _movements)
            {
                byte command = 0;
                if (movement is CannonMovementMove)
                {
                    CannonMovementMove move = (CannonMovementMove)movement;
                    command = 0x80;
                    if (move.Velocity.X == 0 && move.Velocity.Y == 0)
                    {
                        bytes.Add(command);
                    }
                    else
                    {
                        command += (byte)(0x3F & ((move.WaitFrames) >> 2));
                        bytes.Add(command);
                        //write velocities
                        if (move.Velocity.X >= 0)
                        {
                            bytes.Add((byte)(move.Velocity.X & 0x3F));
                        }
                        else
                        {
                            bytes.Add((byte)(move.Velocity.X | 0xc0));
                        }
                        if (move.Velocity.Y >= 0)
                        {
                            bytes.Add((byte)(move.Velocity.Y & 0x3F));
                        }
                        else
                        {
                            bytes.Add((byte)(move.Velocity.Y | 0xc0));
                        }
                    }
                }
                else if (movement is CannonMovementPause)
                {
                    CannonMovementPause pause = (CannonMovementPause)movement;
                    command = 0xc0;
                    command += (byte)((pause.WaitFrames >> 2) & 0x3F);
                    bytes.Add(command);
                }
                else if (movement is CannonMovementReturn)
                {
                    CannonMovementReturn ret = (CannonMovementReturn)movement;
                    command = 0x00;
                    bytes.Add(0);
                }
                else if (movement is CannonMovementPosition)
                {
                    CannonMovementPosition position = (CannonMovementPosition)movement;
                    command = 0x40;
                    command += (byte)(((int)position.Position) << 3);
                    command += (byte)(((int)position.Speed) << 1);
                    if (position.ShotSpeed > 0)
                    {
                        command += 0x01;
                    }
                    bytes.Add(command);
                    if (position.ShotSpeed > 0)
                    {
                        //write velocity now too
                        bytes.Add(position.ShotSpeed);
                    }
                }
            }
            return bytes.ToArray();
        }

        [BrowsableAttribute(false)]
        public override byte[] ToBytes(object obj)
        {
            return ToBytes();
        }

        private void LoadDefaultImage()
        {
            _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.cannon_obj.ico");
        }

        #region ISerializable

        //Deserialization constructor.
        public Cannon(SerializationInfo info, StreamingContext ctxt)
        {
            _movements = (List<iCannonMovement>)info.GetValue("Movements", typeof(List<iCannonMovement>));
            _position = (Point)info.GetValue("Position", typeof(Point));
        }
                
        //Serialization function.
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Movements", _movements);
            info.AddValue("Position", _position);
        }

        #endregion
    }
}
