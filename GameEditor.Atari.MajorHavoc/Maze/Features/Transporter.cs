using GameEditor.Core.Serialization;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace GameEditor.Atari.MajorHavoc.Maze.Features
{

    /// <summary>
    /// Transporters are defined in pairs and matched through color.
    /// TODO: Create a custom collection to make sure that there are
    /// pairs of Transporters and that they are in the correct order.
    /// </summary>
    [Serializable]
    [ConcreteType( typeof( LowResolutionPosition ) )]
    [CollectionType( typeof( TransporterCollection ) )]
    [TerminationObject( (byte)0x00 )]
    public sealed class Transporter : MazeObject
    {
        private OneWayDirection _direction = OneWayDirection.Left;
        private Color _color = Color.White;

        public Transporter()
            : base( "Transporter", new LowResolutionPosition() )
        { }

        private Transporter( RomSerializationInfo si, StreamingContext context )
            /// pull color before position
            : this( si.GetByte( "ColorAndDirection" ), si, context )
        { }

        private Transporter( byte colorAndDirection, RomSerializationInfo si, StreamingContext context )
            : base( si, context )
        {
            /// Test the bit defining direction.
            this._direction = (colorAndDirection & 0x10 ) > 0 ?
                OneWayDirection.Right :
                OneWayDirection.Left;

            this._color = (Color)( colorAndDirection & 0x10);
        }

        public OneWayDirection Direction
        {
            get
            {
                return this._direction;
            }
            set
            {
                this._direction = value;
            }
        }

        public Color Color
        {
            get
            {
                return this._color;
            }
            set
            {
                this._color = value;
            }
        }

        public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            byte colorAndDirection = (byte)(
                ( this._direction == OneWayDirection.Right ? 0x10 : 0x00 ) | (int)this._color
                );

            si.AddValue( "ColorAndDirection", colorAndDirection );

            base.GetObjectData( si, context );
        }
    }
}
