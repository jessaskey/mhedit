using GameEditor.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GameEditor.Atari.MajorHavoc.Maze.Features
{
    /// <summary>
    /// Describes the different wall types available.
    /// </summary>
    public enum MazeWallTypes : byte
    {
        Horizontal = 1,
        LeftDown,
        LeftUp,
        RightUp,
        RightDown,
        Vertical,
        Empty
    }

    [Serializable]
    [ConcreteType( typeof( WallPosition ) )]
    [TerminationObject( (byte)0x00 )]
    public class MazeWall : MazeObject
    {
        private MazeWallTypes _type = MazeWallTypes.Empty;

        public MazeWall()
            : base( "MazeWall", new WallPosition() )
        { }

        private MazeWall( RomSerializationInfo si, StreamingContext context )
            : base( si, context )
        {
            /// There is one wall in the Production ROMs, Level3 that provides
            /// 0x41 for index 50 (0x). The actual wall is MazeWallTypes.RightDown
            /// which is actually 0x05.. Maybe the upper nibble needs added to lower?
            /// Anyway it's why the Masking is here right now.
            this._type = (MazeWallTypes)( si.GetByte( "Type" ) & 0x07 );
        }

        public MazeWallTypes Type
        {
            get
            {
                return this._type;
            }
            set
            {
                this._type = value;
            }
        }

        public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            base.GetObjectData( si, context );

            si.AddValue( "Type", (byte)this._type );
        }
    }
}
