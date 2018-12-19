using GameEditor.Core.Serialization;
using System;
using System.Runtime.Serialization;

namespace GameEditor.Atari.MajorHavoc.Maze.Features
{
    /// <summary>
    /// The Text a the top of several of the first 12 levels used to provide Hints to
    /// the player. Note that there is a leading byte on all strings (the stringPtr
    /// points to this as the starting byte) that isn't used/decoded. This byte is 
    /// captured in the unknown property.
    /// </summary>
    [Serializable]
    public sealed class MazeHint : IRomSerializable
    {
        /// <summary>
        /// The first byte of a MazeHint string is unknown (not encoded) but it
        /// needs processed to get the string.
        /// </summary>
        private readonly byte _unknown;
        private string _text;

        public MazeHint()
        { }

        private MazeHint( RomSerializationInfo si, StreamingContext context )
        {
            this._unknown = si.GetByte( "Unknown" );

            this._text = si.GetString( "Text" );
        }

        /// <summary>
        /// The hint string.
        /// </summary>
        public string Text
        {
            get
            {
                return this._text;
            }
            set
            {
                if ( value == null || value.Length > 254 )
                {
                    throw new ArgumentOutOfRangeException(
                        $"{nameof( Text )} value must be > 1  and < 128." );
                }

                this._text = value;
            }
        }

        /// <summary>
        /// An unknown byte at the beginning of every Hint.
        /// </summary>
        //public byte Unknown
        //{
        //    get
        //    {
        //        return this._unknown;
        //    }
        //}

        public void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            si.AddValue( "Unknown", this._unknown );

            si.AddValue( "Text", this._text );
        }
    }
}
