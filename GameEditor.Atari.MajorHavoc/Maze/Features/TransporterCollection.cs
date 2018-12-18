using GameEditor.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameEditor.Atari.MajorHavoc.Maze.Features
{
    /// <summary>
    /// Implements the funky pattern required to serialize Transporter objects.
    /// </summary>
    [Serializable]
    public class TransporterCollection : List<Transporter>, IRomSerializable
    {
        public TransporterCollection()
        {}

        private TransporterCollection( RomSerializationInfo si, StreamingContext context )
        {
            while ( !si.TryGet<byte>( "Termination", b => b == 0x00 ) )
            {
                this.Add( (Transporter)
                    si.GetValue( $"Transporter{this.Count}", typeof( Transporter ) ) );
            }
        }

        public void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            List<Transporter> copy = new List<Transporter>( this );

            int count = 0;

            while ( copy.Count > 0 )
            {
                Color color = copy[ 0 ].Color;

                /// sort the transporters by color. Should be 2 of each color, left
                /// and right.
                List<Transporter> transporterPair = this.FindAll( t => t.Color == color );

                /// validate the pair of transformers exist and are of opposite direction.
                if ( transporterPair.Count != 2 )
                {
                    throw new SerializationException(
                        $"Transporters should exist in pairs! There are " +
                        $"{transporterPair.Count} {color}!" );
                }

                if ( transporterPair[0].Direction == transporterPair[ 1 ].Direction )
                {
                    throw new SerializationException(
                        $"Transporters should exist in pairs! There are " +
                        $"2 {transporterPair[ 0 ].Direction}!" );
                }

                foreach ( var transporter in transporterPair )
                {
                    si.AddValue( $"Transporter{count++}", transporter, transporter.GetType() );

                    this.Remove( transporter );
                }
            }

            si.AddValue( "Termination", (byte)0x00 );
        }
    }
}
