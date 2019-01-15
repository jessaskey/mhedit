using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

namespace mhedit.Containers
{
    [Serializable]
    public abstract class MazeObject : ICloneable
    {
        private List<string> _validationErrors = new List<string>();
        private bool _isValid = false;
        private Guid _objectId = Guid.NewGuid();


        //protected MazeObjectType mazeObjectType = MazeObjectType.None;
        protected bool selected = false;
        protected string name;
        protected Point renderOffset = new Point(0, 0);
        protected Point staticLsb = new Point(0,0);


        public abstract Point SnapSize { get; }
        public abstract Size Size { get; }
        public abstract Point Position { get; set; }
        public abstract Image Image { get; }
        public abstract int MaxObjects { get; }

        //Methods for Serialization
        public abstract byte[] ToBytes();
        public abstract byte[] ToBytes(object obj);


        [BrowsableAttribute(false)]
        public Guid ObjectId
        {
            get
            {
                return _objectId;
            }
        }

        [BrowsableAttribute(false)]
        public Point RenderOffset 
        {
            get
            {
                return renderOffset;
            }
        }

        [BrowsableAttribute(false)]
        public Point RenderPosition
        {
            get
            {
                return new Point(Position.X - renderOffset.X, Position.Y - renderOffset.Y);
            }
        }


        [TypeConverter(typeof(TypeConverters.VectorPositionTypeConverter))]
        [ReadOnly(true)]
        public Point VectorPosition
        {
            get
            {
                return DataConverter.ConvertPixelsToVector(Position);
            }
        }

        [CategoryAttribute("Validation")]
        [ReadOnly(true)]
        public bool IsValid
        {
            get
            {
                return _isValid;
            }
        }

        [CategoryAttribute("Validation")]
        [ReadOnly(true)]
        public List<string> ValidationErrors 
        {
            get
            {
                return _validationErrors;
            }
            set
            {
                _validationErrors = value;
            }
        }

        //[BrowsableAttribute(false)]
        //public Point CenterPosition 
        //{
        //    get
        //    {
        //        return new Point(Position.X + (Size.Width / 2), Position.Y + (Size.Height / 2));
        //    }
        //}

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [BrowsableAttribute(false)]
        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        [BrowsableAttribute(false)]
        [IgnoreDataMemberAttribute]
        public Point StaticLSB
        {
            get { return staticLsb; }
            set { staticLsb = value; }
        }

        public object Clone()
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, this);
            ms.Position = 0;
            object obj = bf.Deserialize(ms);
            ms.Close();
            return obj;
        }

        protected Image ImageSelected(Image img)
        {
            //draw little brackets in each corner
            Graphics g = Graphics.FromImage(img);
            Pen redPen = new Pen(Color.Orange, 1);
            //top left
            g.DrawLine(redPen, 0, 0, 0, 6);
            g.DrawLine(redPen, 0, 0, 6, 0);
            //top right
            g.DrawLine(redPen, img.Width - 1, 0, img.Width - 1, 6);
            g.DrawLine(redPen, img.Width - 1, 0, img.Width - 7, 0);
            //bottom left
            g.DrawLine(redPen, 0, img.Height - 1, 0, img.Height - 7);
            g.DrawLine(redPen, 0, img.Height - 1, 6, img.Height - 1);
            //bottom right
            g.DrawLine(redPen, img.Width - 1, img.Height - 1, img.Width - 1, img.Height - 7);
            g.DrawLine(redPen, img.Width - 1, img.Height - 1, img.Width - 7, img.Height - 1);
            return img;
        }

        public void LoadPosition(byte bytePosition)
        {
            Tuple<short, short> oxoidVector = DataConverter.BytePackedToVector(bytePosition, staticLsb);
            Position = DataConverter.ConvertVectorToPixels(oxoidVector);
        }

        public void LoadPosition(byte[] longPosition)
        {
            Position = DataConverter.ConvertVectorToPixels(DataConverter.ByteArrayLongToPoint(longPosition));
        }

    }
}
