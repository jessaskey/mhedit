using GameEditor.Core.Serialization;
using System;
using System.Runtime.Serialization;

namespace GameEditor.Atari.MajorHavoc.Maze.Features
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [ConcreteType( typeof( LowResolutionPosition ) )]
    [TerminationObject( (byte)0x00 )]
    public sealed class Stalactite : MazeObject
    {
        public Stalactite()
            : base( "Stalactite", new LowResolutionPosition() )
        { }

        private Stalactite( RomSerializationInfo si, StreamingContext context )
            : base( si, context )
        { }

        public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            base.GetObjectData( si, context );
        }
    }
}
