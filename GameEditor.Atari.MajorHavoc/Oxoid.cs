using GameEditor.Core.Serialization;
using System;
using System.Runtime.Serialization;

namespace GameEditor.Atari.MajorHavoc
{
    /// <summary>
    /// </summary>
    [Serializable]
    [ConcreteType( typeof( LowResolutionMazePosition ) )]
    [CollectionTermination( 0xff )]
    public sealed class Oxoid : MazeObject
    {
        private int _value;

        public Oxoid()
            : base( "Oxoid", new LowResolutionMazePosition() )
        { }

        private Oxoid( RomSerializationInfo si, StreamingContext context )
            : base( si, context )
        {
            this._value = (int)si.GetByte( "Value" );
        }

        public int Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
            }
        }

        public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            base.GetObjectData( si, context );

            si.AddValue( "Value", (byte)this._value );
        }
    }

}
