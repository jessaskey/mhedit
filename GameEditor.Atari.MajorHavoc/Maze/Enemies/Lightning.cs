using GameEditor.Core.Serialization;
using System;
using System.Runtime.Serialization;

namespace GameEditor.Atari.MajorHavoc.Maze.Enemies
{
    public enum LightningOrientation : byte
    {
        Horizontal,
        Verticle = 0xFF
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [ConcreteType( typeof( LowResolutionPosition ) )]
    [CollectionType( typeof( LightningCollection ) )]
    [TerminationObject( (byte)0x00 )]
    public sealed class Lightning : MazeObject
    {
        private LightningOrientation _orientation = LightningOrientation.Verticle;

        public Lightning()
            : base( "Lightning", new LowResolutionPosition() )
        { }

        private Lightning( RomSerializationInfo si, StreamingContext context )
            : base( si, context )
        {}

        public LightningOrientation Orientation
        {
            get
            {
                return this._orientation;
            }
            set
            {
                this._orientation = value;
            }
        }
    }
}
