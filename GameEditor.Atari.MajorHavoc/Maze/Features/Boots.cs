using GameEditor.Core.Serialization;
using System;
using System.Runtime.Serialization;

namespace GameEditor.Atari.MajorHavoc.Maze.Features
{

    /// <summary>
    /// Boots is enabled if it's placed in/on the maze - has a position
    /// that's not zero (the origin).
    /// </summary>
    [Serializable]
    [ConcreteType( typeof( LowResolutionPosition ) )]
    public sealed class Boots : MazeObject
    {
        public Boots()
            : base( "Boots", new LowResolutionPosition() )
        { }

        private Boots( RomSerializationInfo si, StreamingContext context )
            : base( si, context )
        { }

        public bool Enabled
        {
            get
            {
                return this.Position.X != 0 || this.Position.Y != 0;
            }
            set
            {
                /// Only allow a false set, which clears position. To Enable
                /// the MazeObject needs placed on the maze.
                if ( !value )
                {
                    this.Position.Reset();
                }
            }
        }

        public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            base.GetObjectData( si, context );
        }
    }
}