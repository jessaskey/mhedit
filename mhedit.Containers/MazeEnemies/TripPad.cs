using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

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
            : base( 8,
                    ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.trippad_obj.png" ),
                    new Point( 0x00, 0x08 ),
                    new Point( 0, 32 ) )
        { }

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

        [DescriptionAttribute("The pyroid associated with this trip pad.")]
        [TypeConverter(typeof(TypeConverters.TripPadPyroidTypeConverter))]
        public TripPadPyroid Pyroid
        {
            get { return _pyroid; }
            set
            {
                //if ( this._pyroid != null )
                //{
                //    this._pyroid.PropertyChanged -= this.OnTripPyroidChanged;
                //}

                _pyroid = value;

                //if ( this._pyroid != null )
                //{
                //    this._pyroid.PropertyChanged += this.OnTripPyroidChanged;
                //}
            }
        }

        //private void OnTripPyroidChanged( object sender, PropertyChangedEventArgs e )
        //{
        //    this.IsDirty |= ((TripPadPyroid)sender).IsDirty;
        //}
    }
}
