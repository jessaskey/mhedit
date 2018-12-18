using GameEditor.Core.Serialization;
using System;
using System.Runtime.Serialization;

namespace GameEditor.Atari.MajorHavoc.Maze.Features
{

    /// <summary>
    /// The direction that a OneWay allows the player to pass.
    /// </summary>
    public enum OneWayDirection : byte
    {
        /// <summary>
        /// One Way Arrows allowing travel left to right
        /// </summary>
        Right,

        /// <summary>
        /// One Way Arrows allowing travel right to left
        /// </summary>
        Left = 0xFF
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [ConcreteType( typeof( LowResolutionPosition ) )]
    [TerminationObject( (byte)0x00 )]
    public sealed class OneWay : MazeObject
    {
        private OneWayDirection _direction = OneWayDirection.Right;

        public OneWay()
            : base( "OneWay", new LowResolutionPosition() )
        { }

        private OneWay( RomSerializationInfo si, StreamingContext context )
            : this(
                  /// The direction is only embedded if it's left and it's before the position
                  /// information. So Test for 0xff and deserialize if present.
                  si.TryGet<byte>( "Direction", b => b == (byte)OneWayDirection.Left ),
                  si,
                  context )
        { }

        private OneWay( bool isLeft, RomSerializationInfo si, StreamingContext context )
            : base( si, context )
        {
            this._direction = isLeft ?
                OneWayDirection.Left : OneWayDirection.Right;
        }

        public OneWayDirection Direction
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
            if ( this._direction == OneWayDirection.Left )
            {
                si.AddValue( "Direction", (byte)this._direction );
            }

            base.GetObjectData( si, context );
        }
    }
}
