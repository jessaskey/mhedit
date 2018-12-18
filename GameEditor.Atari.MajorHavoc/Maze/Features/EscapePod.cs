using GameEditor.Core.Serialization;
using System;
using System.Runtime.Serialization;

namespace GameEditor.Atari.MajorHavoc.Maze.Features
{
    /// <summary>
    /// Describes the way the Escape Pod effects the players exit strategy.
    /// </summary>
    public enum EscapePodDirectives : byte
    {
        /// <summary>
        /// Means not on this wave.
        /// </summary>
        Disabled,

        /// <summary>
        /// Means shows up but player doesn't have to use it.
        /// </summary>
        EnabledButOptional,

        /// <summary>
        /// Means show up and doors don't open, player must use pod to escape.
        /// </summary>
        EnabledAndRequired
    }

    /// <summary>
    /// Escape pods are only supported on Maze Type A. The Escape Pod has a fixed
    /// location on the maze so the position doesn't support changes and isn't
    /// serialized.
    /// </summary>
    [Serializable]
    public sealed class EscapePod : MazeObject
    {
        /// <summary>
        /// Should probably find a way to set position to it's fixed
        /// location in the maze (where it's supported)
        /// </summary>
        private class FixedPosition : MazePosition
        {
            /// Don't allow changing X/Y
            public override short X { set { } }
            public override short Y { set { } }

            public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
            {}
        }

        private EscapePodDirectives _directive = EscapePodDirectives.Disabled;

        public EscapePod()
            : base( "EscapePod", new FixedPosition() )
        { }

        private EscapePod( RomSerializationInfo si, StreamingContext context )
            : base( "EscapePod", new FixedPosition() )
        {
            this._directive = (EscapePodDirectives)si.GetByte( "Directive" );
        }

        public EscapePodDirectives Directive
        {
            get
            {
                return this._directive;
            }
            set
            {
                this._directive = value;
            }
        }

        public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            si.AddValue( "Directive", (byte)this._directive );
        }
    }
}