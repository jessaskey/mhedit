using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;
using mhedit.Containers.MazeObjects;
using mhedit.Containers.Validation;

namespace mhedit.Containers.MazeEnemies.IonCannon
{
    /// <summary>
    /// The IonCannon class shows the Ion IonCannon in the maze.
    /// 
    /// IonCannons are a Hybrid.. they have a High Resolution X component and a Low
    /// Resolution Y component but are stored in High Resolution format. Several
    /// cannons in the factory ROMs have High Resolution position data in the Y/Low
    /// Resolution component. This shows as visual errors in the Editor but has no
    /// effect on the actual game. This object corrects for those on load by using
    /// a StaticLSB value of Point( 0, 0x80 ).
    /// </summary>
    [Serializable]
    public class IonCannon : MazeObject
    {
        private static readonly Point _snapSize = new Point( 1, 64 );
        private bool _isShotTransportable;
        private IonCannonProgram _program;

        public IonCannon()
            : base( Constants.MAXOBJECTS_CANNON,
                    ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.cannon_obj.png" ),
                    new Point( 0, 0x80 ),
                    new Point( 31, 32 ) )
        {
            this.Program = new IonCannonProgram();
        }

        [BrowsableAttribute( false )]
        public override Point SnapSize
        {
            get { return _snapSize; }
        }

        #region Implementation of IChangeTracking

        [BrowsableAttribute( false )]
        [XmlIgnore]
        public override bool IsChanged
        {
            get
            {
                return base.IsChanged |
                    this._program.IsChanged;
            }
        }

        public override void AcceptChanges()
        {
            /// clear composite member first.
            this._program.AcceptChanges();

            base.AcceptChanges();
        }

        #endregion

        [CategoryAttribute("Custom")]
        [DescriptionAttribute("The movement script for the cannon")]
        [EditorAttribute(typeof(IonCannonEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [XmlElement( "ReturnToStart", typeof( ReturnToStart ) )]
        [XmlElement( "OrientAndFire", typeof( OrientAndFire ) )]
        [XmlElement( "Move", typeof( Move ) )]
        [XmlElement( "Pause", typeof( Pause ) )]
        [Validation]
        public IonCannonProgram Program
        {
            get { return _program; }
            set
            {
                /// Note, this set field serves a special purpose. When an IonCannon program is edited
                /// it will replace the previous version here. This will result in the IonCannon.IsChanged
                /// property being set to True and allows the Maze to track edits to the Program seperately
                /// from the IonCannonProgram object itself.
                /// Setting IsChanged locally is important, because the user can save the IonCannon program
                /// in the program editor (clears the IonCannonProgram.IsChanged property) and we want
                /// the Maze to know it was edited at some point.
                this.SetField( ref this._program, value );

                ( (INotifyPropertyChanged)this._program ).PropertyChanged +=
                    this.ForwardPropertyChanged;
            }
        }

        [DescriptionAttribute("Defines if the object needs to be checked for Transporter collisions. If this object must transport, then set to True, otherwise leave at False.")]
        public bool IsShotTransportable
        {
            get { return _isShotTransportable; }
            set
            {
                this.SetField(ref this._isShotTransportable, value);
            }
        }

        public override Point GetAdjustedPosition( Point point )
        {
            Point adjusted = base.GetAdjustedPosition( point );

            /// Make a special adjustment for drag/drop operations to make the drop 
            /// behavior/location logical from the Users perspective. This is due 
            /// to the Image being the same size as a maze stamp AND the image needs
            /// to be displayed between 2 maze stamps.
            /// Thus, make adjustments based upon the cursor being in the lower or
            /// upper range of a maze stamp
            adjusted.Y +=
                ( ( point.Y - DataConverter.PADDING ) % DataConverter.CanvasGridSize ) < 32 ?
                    -32 : 32;

            return adjusted;
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(DataConverter.PointToByteArrayLong(DataConverter.ConvertPixelsToVector(this.Position)));

            //now cannon commands
            this._program.GetObjectData( bytes );

            return bytes.ToArray();
        }

        public override byte[] ToBytes(object obj)
        {
            return ToBytes();
        }
    }
}
