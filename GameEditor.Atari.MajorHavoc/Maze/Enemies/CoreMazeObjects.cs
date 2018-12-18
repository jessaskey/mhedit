using GameEditor.Atari.MajorHavoc.Maze.Features;
using GameEditor.Core.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameEditor.Atari.MajorHavoc.Maze.Enemies
{
    /// <summary>
    /// Implements the funky pattern required to pull out Pyroid and Perkoid
    /// objects. I believe MaxBots will end up in here too.
    /// </summary>
    [Serializable]
    public class CoreMazeObjects : IRomSerializable
    {
        private Reactoid _reactoid;
        private List<Pyroid> _pyroids = new List<Pyroid>();
        private List<Perkoid> _perkoids = new List<Perkoid>();

        public CoreMazeObjects()
        { }

        private CoreMazeObjects( RomSerializationInfo si, StreamingContext context )
        {
            this._reactoid = (Reactoid)si.GetValue( "Reactoid", typeof( Reactoid ) );

            IList list = this._pyroids;
            Type oidType = typeof( Pyroid );

            while ( !si.TryGet<byte>( "Termination", b => b == 0xFF ) )
            {
                /// Note: In Production ROMs their is ALWAYS at least 1 Pyroid!
                /// Note2: No Perkoids means no marker
                /// Note3: When Max shows we need to add another bump..
                if ( si.TryGet<byte>( "PerkoidMarker", b => b == 0xFE ) )
                {
                    list = this._perkoids;

                    oidType = typeof( Perkoid );

                    break;
                }

                list.Add( si.GetValue( $"{oidType.Name}{list.Count}", oidType ) );
            }
        }

        public List<Pyroid> Pyroids
        {
            get { return this._pyroids; }
            //set { this._pyroids = value; }
        }

        public List<Perkoid> Perkoids
        {
            get { return this._perkoids; }
            //set { this._perkoids = value; }
        }

        public void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            si.AddValue( "Reactoid", this._reactoid );

            int count = 0;

            foreach ( Pyroid pyroid in this._pyroids )
            {
                si.AddValue( $"Pyroid{count++}", pyroid );
            }

            /// No Perkoids means no marker
            if ( this._perkoids.Count > 0 )
            {
                si.AddValue( "PerkoidDelimiter", (byte)0xFE );

                count = 0;

                foreach ( Perkoid perkoid in this._perkoids )
                {
                    si.AddValue( $"Perkoid{count++}", perkoid );
                }
            }

            ///// No Maxbots means no marker
            //if ( this._maxbots.Count > 0 )
            //{
            //    si.AddValue( "MaxbotDelimiter", (byte)0xFE );

            //    count = 0;

            //    foreach ( Maxbot maxbot in this._maxbots )
            //    {
            //        si.AddValue( $"Maxbot{count++}", maxbot );
            //    }
            //}

            si.AddValue( "Termination", (byte)0xFF );
        }
    }
}
