using GameEditor.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GameEditor.Atari.MajorHavoc.Maze.Features
{

    [Serializable]
    [ConcreteType( typeof( WallPosition ) )]
    [TerminationObject( (byte)0x00 )]
    public sealed class DynamicMazeWall : MazeObject
    {
        private readonly WallOption _base = new WallOption();
        private readonly WallOption _alternate = new WallOption();

        public DynamicMazeWall()
            : base( "DynamicMazeWall", new WallPosition() )
        { }

        private DynamicMazeWall( RomSerializationInfo si, StreamingContext context )
            : base( si, context )
        {
            this._base.Duration = si.GetByte( "Base.Duration" );

            this._alternate.Duration = si.GetByte( "Alternate.Duration" );

            /// Don't need to mask the dynamic wall types for some reason.
            this._base.Type = (MazeWallTypes)si.GetByte( "Base.Type" );

            this._alternate.Type = (MazeWallTypes)si.GetByte( "Alternate.Type" );
        }

        public WallOption Base
        {
            get
            {
                return this._base;
            }
        }

        public WallOption Alternate
        {
            get
            {
                return this._alternate;
            }
        }

        public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            base.GetObjectData( si, context );

            si.AddValue( "Base.Duration", (byte)this._base.Duration );

            si.AddValue( "Alternate.Duration", (byte)this._alternate.Duration );

            si.AddValue( "Base.Type", (byte)this._base.Type );

            si.AddValue( "Alternate.Type", (byte)this._alternate.Type );
        }
    }
}

