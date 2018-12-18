using System;

namespace GameEditor.Atari.MajorHavoc.Maze.Features
{
    public class WallOption
    {
        private byte _duration = 20;
        private MazeWallTypes _type = MazeWallTypes.Empty;

        public WallOption( byte duration = 20, MazeWallTypes type = MazeWallTypes.Empty )
        {
            this._duration = duration;

            this._type = type;
        }

        public int Duration
        {
            get
            {
                return this._duration;
            }
            set
            {
                if ( value > 0x7f )
                {
                    throw new ArgumentOutOfRangeException(
                        nameof( this.Duration ), value, "Value must be < 128" );
                }

                this._duration = (byte)value;
            }
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
    }
}

