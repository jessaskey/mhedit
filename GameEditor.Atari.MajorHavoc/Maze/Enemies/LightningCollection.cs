using GameEditor.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameEditor.Atari.MajorHavoc.Maze.Enemies
{
    /// <summary>
    /// Implements the funky pattern required to pull out Lightning objects.
    /// </summary>
    [Serializable]
    public sealed class LightningCollection : List<Lightning>, IRomSerializable
    {
        public LightningCollection()
        {}

        private LightningCollection( RomSerializationInfo si, StreamingContext context )
        {
            LightningOrientation orientation = LightningOrientation.Horizontal;

            while ( !si.TryGet<byte>( "Termination", b => b == 0x00 ) )
            {
                if ( si.TryGet<byte>(
                    "Delimiter", b => b == (byte)LightningOrientation.Verticle ) )
                {
                    orientation = LightningOrientation.Verticle;
                }

                Lightning li = (Lightning)si.GetValue(
                        $"Lightning{this.Count}", typeof( Lightning ) );

                li.Orientation = orientation;

                this.Add( li );
            }
        }

        public void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            /// sort the horizontal orientation elements to the beginning of the list.
            this.Sort( ( a, b ) => (int)a.Orientation );

            bool delimiterWritten = false;
            int count = 0;

            foreach( var lightning in this )
            {
                if( lightning.Orientation == LightningOrientation.Verticle &&
                    !delimiterWritten )
                {
                    si.AddValue( "Delimiter", (byte)0xFF );

                    delimiterWritten = true;
                }

                si.AddValue( $"Lightning{count++}", lightning, lightning.GetType() );
            }

            si.AddValue( "Termination", (byte)0x00 );
        }
    }
}
