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
        private static readonly Point _snapSize = new Point( 4, 4 );

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
                this.SetField( ref this._program, value );

                ( (INotifyPropertyChanged)this._program ).PropertyChanged +=
                    this.ForwardPropertyChanged;
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
