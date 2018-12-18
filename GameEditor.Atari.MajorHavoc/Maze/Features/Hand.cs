using GameEditor.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GameEditor.Atari.MajorHavoc.Maze.Features
{
    /// <summary>
    /// The Hand may be placed on any maze. If it is not disabled by the player,
    /// it will automatically turn off the reactoid if it is triggered.
    /// </summary>
    [Serializable]
    [ConcreteType( typeof( MediumResolutionPosition ) )]
    public class Hand : MazeObject
    {
        public Hand()
            : base( "Hand", new LowResolutionPosition() )
        { }

        private Hand( RomSerializationInfo si, StreamingContext context )
            : base( si, context )
        { }

        public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            base.GetObjectData( si, context );
        }
    }
}