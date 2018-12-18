using GameEditor.Core.Serialization;
using System;
using System.Runtime.Serialization;

namespace GameEditor.Atari.MajorHavoc.Maze.Features
{

    /// <summary>
    /// Oxygen for MH as he moves through the maze. In Production ROMs it has
    /// a fixed o2 value.
    /// MHPe will have a variable value attribute.
    /// </summary>
    [Serializable]
    [ConcreteType( typeof( LowResolutionPosition ) )]
    [TerminationObject( (byte)0x00 )]
    public sealed class Oxoid : MazeObject
    {
        //private int _value;

        public Oxoid()
            : base( "Oxoid", new LowResolutionPosition() )
        { }

        private Oxoid( RomSerializationInfo si, StreamingContext context )
            : base( si, context )
        {
            //this._value = (int)si.GetByte( "Value" );
        }

        //public int Value
        //{
        //    get
        //    {
        //        return this._value;
        //    }
        //    set
        //    {
        //        if ( value > 0x7f || value < 0 )
        //        {
        //            throw new ArgumentOutOfRangeException( nameof( Value ),
        //                $"value n[{value}] is not > 1  and < 128." );
        //        }

        //        this._value = value;
        //    }
        //}

        //public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
        //{
        //    base.GetObjectData( si, context );

        //    si.AddValue( "Value", (byte)this._value );
        //}
    }
}
