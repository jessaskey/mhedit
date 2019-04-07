using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;

namespace mhedit.Containers.MazeEnemies.IonCannon
{
    /// <summary>
    /// The IonCannon class shows the Ion IonCannon in the maze.
    /// </summary>
    [Serializable]
    public class IonCannon : MazeObject
    {
        private static readonly Point _snapSize = new Point( 1, 1 );
        private bool _isShotTransportable;
        private IonCannonProgram _program;

        public IonCannon()
            : base( 4,
                    ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.cannon_obj.png" ),
                    Point.Empty,
                    new Point( 32, 32 ) )
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
