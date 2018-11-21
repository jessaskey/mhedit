using GameEditor.Core.Serialization;
using System;
using System.Runtime.Serialization;

namespace GameEditor.Atari.MajorHavoc.Test
{
    [Serializable]
    [TerminationObjectAttribute( (byte)0xff )]
    public class TestEmbeddedArray : IRomSerializable
    {
        private ushort[] _array;

        public TestEmbeddedArray()
        {
            this._array = new ushort[ 12 ];
            this._array[ 1 ] = 1;
            this._array[ 3 ] = 3;
            this._array[ 5 ] = 5;
            this._array[ 7 ] = 7;
        }

        private TestEmbeddedArray( RomSerializationInfo si, StreamingContext context )
        {
            this._array = (ushort[])si.GetValue( "Array", typeof(ushort[]), 12 );
        }

        public void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            si.AddValue( "Array", this._array );
        }
    }

}
