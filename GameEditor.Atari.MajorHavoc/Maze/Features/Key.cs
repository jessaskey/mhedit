using GameEditor.Core.Serialization;
using System;
using System.Runtime.Serialization;

namespace GameEditor.Atari.MajorHavoc.Maze.Features
{
    /// <summary>
    /// The colors that are available for Lock and Key maze objects.
    /// </summary>
    public enum Color : byte
    {
        Black,
        Blue,
        Green,
        Cyan,
        Red2,
        Purple,
        Yellow,
        White,

        /// <summary>
        /// Whiter
        /// </summary>
        Whiter,

        Pink,
        Orange,
        Red,

        /// <summary>
        /// Sparkle maybe?
        /// </summary>
        Flash,

        /// <summary>
        /// Cyanr
        /// </summary>
        Cyanr,

        /// <summary>
        /// Bluer
        /// </summary>
        Bluer,

        /// <summary>
        /// Greenr
        /// </summary>
        Greenr
    }


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [ConcreteType( typeof( LowResolutionPosition ) )]
    [TerminationObject( (byte)0x00 )]
    public sealed class Key : MazeObject
    {
        private Color _color = Color.White;
        private readonly Lock _lock;

        public Key()
            : base( "Key", new LowResolutionPosition() )
        {
            this._lock = new Lock
            {
                Color = this._color
            };
        }

        /// call into local constructor so that we can pull color before
        /// base class position.
        private Key( RomSerializationInfo si, StreamingContext context )
            : this( (Color)si.GetByte( "Color" ), si, context )
        {
            this._lock = (Lock)si.GetValue( "Lock", this._lock.GetType() );

            this._lock.Color = this._color;
        }

        private Key( Color color,  RomSerializationInfo si, StreamingContext context )
            : base( si, context )
        {
            this._color = color;
        }

        public Color Color
        {
            get
            {
                return this._color;
            }
            set
            {
                this._color = value;
            }
        }

        public Lock Lock
        {
            get
            {
                return this._lock;
            }
        }

        public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            si.AddValue( "Color", (byte)this._color );

            base.GetObjectData( si, context );

            si.AddValue( "Lock", this._lock, this._lock.GetType() );
        }
    }
}
