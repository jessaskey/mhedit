using GameEditor.Core.Serialization;
using System;
using System.Runtime.Serialization;

namespace GameEditor.Atari.MajorHavoc.Maze.Features
{
    /// <summary>
    /// ArrowDirection defines which way the current arrow is pointing.
    /// </summary>
    public enum ArrowDirection : byte
    {
        Right,
        Left,
        Up,
        Down,
        UpRight,
        DownLeft,
        UpLeft,
        DownRight,
        QuestionMark
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [ConcreteType( typeof( LowResolutionPosition ) )]
    [TerminationObject( (byte)0x00 )]
    public sealed class Arrow : MazeObject
    {
        private ArrowDirection _direction;

        public Arrow()
            : base( "Arrow", new LowResolutionPosition() )
        { }

        private Arrow( RomSerializationInfo si, StreamingContext context )
            : base( si, context )
        {
            this._direction = (ArrowDirection)si.GetByte( "Direction" );
        }

        public ArrowDirection Direction
        {
            get
            {
                return this._direction;
            }
            set
            {
                this._direction = value;
            }
        }

        public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            base.GetObjectData( si, context );

            si.AddValue( "Direction", (byte)this._direction );
        }
    }
}
