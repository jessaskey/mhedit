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

        private IonCannonProgram _program = new IonCannonProgram();

        public IonCannon()
            : base( 4,
                    ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.cannon_obj.png" ),
                    Point.Empty,
                    new Point( 32, 32 ) )
        {
            ( (INotifyPropertyChanged)this._program ).PropertyChanged +=
                this.ForwardIsDirtyPropertyChanged;
        }

        [BrowsableAttribute( false )]
        public override Point SnapSize
        {
            get { return _snapSize; }
        }

        [XmlIgnore]
        public override bool IsDirty
        {
            get
            {
                return base.IsDirty | this._program.IsDirty;
            }
            set
            {
                base.IsDirty = this._program.IsDirty = value;
            }
        }

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
