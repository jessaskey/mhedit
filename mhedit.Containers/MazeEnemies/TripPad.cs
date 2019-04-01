using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;

namespace mhedit.Containers.MazeEnemies
{
    /// <summary>
    /// TripPad objects are maze enemies that when stepped on by the player will launch a pyroid from 
    /// a predefined location at a specific speed and velocity.
    /// </summary>
    [Serializable]
    public class TripPad : MazeObject
    {
        private TripPadPyroid _pyroid;

        public TripPad()
            : base(Constants.MAXOBJECTS_TRIPPAD,
                    ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.trippad_obj.png" ),
                    new Point( 0x80, 0x08 ),
                    new Point( 32, 32 ) )
        { }

        #region Implementation of IChangeTracking

        [BrowsableAttribute( false )]
        [XmlIgnore]
        public override bool IsChanged
        {
            get
            {
                return base.IsChanged |
                    this._pyroid.IsChanged;
            }
        }

        public override void AcceptChanges()
        {
            /// clear composite member first.
            this._pyroid.AcceptChanges();

            base.AcceptChanges();
        }

        #endregion

        [BrowsableAttribute( false )]
        [XmlIgnore]
        protected override bool IsHighlighted
        {
            get { return this._pyroid.Selected; }
        }

        [DescriptionAttribute( "The pyroid associated with this trip pad." )]
        [TypeConverter( typeof( TypeConverters.TripPadPyroidTypeConverter ) )]
        public TripPadPyroid Pyroid
        {
            get { return _pyroid; }
            set
            {
                if ( this._pyroid != null )
                {
                    this._pyroid.PropertyChanged -= this.ForwardPropertyChanged;
                }

                this.SetField( ref this._pyroid, value );

                if ( this._pyroid != null )
                {
                    this._pyroid.TripPad = this;

                    this._pyroid.PropertyChanged += this.ForwardPropertyChanged;
                }
            }
        }

        public override Point GetAdjustedPosition( Point point )
        {
            Point adjusted = base.GetAdjustedPosition( point );

            adjusted.Y += 28;

            return adjusted;
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(DataConverter.PointToByteArrayPacked(this.Position));
            return bytes.ToArray();
        }

        [BrowsableAttribute(false)]
        public override byte[] ToBytes(object obj)
        {
            return ToBytes();
        }
    }
}
