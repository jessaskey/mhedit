using GameEditor.Atari.MajorHavoc.Maze.Enemies;
using GameEditor.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GameEditor.Atari.MajorHavoc.Maze.Features
{
    [Serializable]
    [ConcreteType( typeof( HighResolutionPosition ) )]
    [SerializationSurrogate( typeof( CoreMazeObjects ) )]
    public sealed class Reactoid : MazeObject
    {
        private int _countdownTime;

        public Reactoid()
            : base( "Reactoid", new LowResolutionPosition() )
        { }

        private Reactoid( RomSerializationInfo si, StreamingContext context )
            : base( si, context )
        {
            /// BUG: TODO: This time isn't stored in with the Reactoid position data
            /// but in it's own array.. Need to make a way to load this.
            throw new NotImplementedException();

            this._countdownTime = si.GetByte( "CountdownTime" );
        }

        public int CountdownTime
        {
            get
            {
                return this._countdownTime;
            }
            set
            {
                if ( value > 0x7f || value < 0 )
                {
                    throw new ArgumentOutOfRangeException( nameof( CountdownTime ),
                        value,  $"Must be > 1  and < 128." );
                }

                this._countdownTime = value;
            }
        }

        public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            /// BUG: TODO: This time isn't stored in with the Reactoid position data
            /// but in it's own array.. Need to make a way to load this.
            throw new NotImplementedException();

            base.GetObjectData( si, context );

            si.AddValue( "CountdownTime", (byte)this._countdownTime );
        }
    }
}