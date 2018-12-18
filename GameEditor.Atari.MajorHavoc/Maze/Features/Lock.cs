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
    public sealed class Lock : MazeObject
    {
        private Color _color = Color.White;

        public Lock()
            : base( "Lock", new LowResolutionPosition() )
        { }

        private Lock( RomSerializationInfo si, StreamingContext context )
            : base( si, context )
        {
            /// Color is set by the parent Key object.
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

        public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            base.GetObjectData( si, context );
        }
    }
}
