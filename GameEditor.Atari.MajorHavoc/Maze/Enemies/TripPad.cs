using GameEditor.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GameEditor.Atari.MajorHavoc.Maze.Enemies
{
    [Serializable]
    [ConcreteType( typeof( LowResolutionPosition ) )]
    //[CollectionType( typeof( TripPadPyroidCollection ) )]
    [TerminationObject( (byte)0x00 )]
    public sealed class TripPad : MazeObject
    {
        private readonly TripPadPyroid _pyroid;

        public TripPad()
            : base( "TripPad", new LowResolutionPosition() )
        {
            this._pyroid = new TripPadPyroid();
        }

        private TripPad( RomSerializationInfo si, StreamingContext context )
            : base( si, context )
        {
            this._pyroid = (TripPadPyroid)si.GetValue( "TripPadPyroid", typeof( TripPadPyroid ) );
        }

        public TripPadPyroid Pyroid
        {
            get
            {
                return this._pyroid;
            }
        }

        public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            base.GetObjectData( si, context );

            si.AddValue( "TripPadPyroid", this._pyroid );
        }
    }
}
