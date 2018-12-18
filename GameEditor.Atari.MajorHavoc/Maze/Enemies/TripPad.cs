using GameEditor.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GameEditor.Atari.MajorHavoc.Maze.Enemies
{
    /// <summary>
    /// I could have sworn that this enum isn't real!! When I looked at the ROMs
    /// I saw Owen put 2 trips in the same place to get 2 Pyroids launched??
    /// </summary>
    public enum PyroidStyle
    {
        Double = 0,
        Single = 1
    }

    [Serializable]
    [ConcreteType( typeof( LowResolutionPosition ) )]
    //[CollectionType( typeof( TripPadPyroidCollection ) )]
    [TerminationObject( (byte)0x00 )]
    public class TripPad : MazeObject
    {
        private readonly TripPadPyroid _pyroid;

        public TripPad()
            : base( "TripPad", new HighResolutionPosition() )
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
