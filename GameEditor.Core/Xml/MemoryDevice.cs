using System.Xml.Serialization;

namespace GameEditor.Core.Xml
{
    /// <summary>
    /// The point behind a base class here was to allow for different
    /// types of memory devices in the XML. This has some basis in 
    /// making the XML easier to associate with the hardware configuration
    /// as well as providing for a path to add different devices in the
    /// future which might need more options.
    /// </summary>
    public class MemoryDevice
    {
        private string _id;

        private string _path;

        private int _size;

        private int _width;

        /// <remarks />
        [XmlAttribute]
        public string Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public string Path
        {
            get { return this._path; }
            set { this._path = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public int Size
        {
            get { return this._size; }
            set { this._size = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public int Width
        {
            get { return this._width; }
            set { this._width = value; }
        }
    }
}