﻿using GameEditor.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GameEditor.Atari.MajorHavoc.Maze.Enemies
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [ConcreteType( typeof( HighResolutionPosition ) )]
    [SerializationSurrogate( typeof( CoreMazeObjects ) )]
    public sealed class Perkoid : MazeObject
    {
        private readonly Vector _velocity;

        public Perkoid()
            : base( "Perkoid", new HighResolutionPosition() )
        {
            this._velocity = new Vector();
        }

        private Perkoid( RomSerializationInfo si, StreamingContext context )
            : base( si, context )
        {
            this._velocity = (Vector)si.GetValue( "Velocity", typeof( Vector ) );
        }

        public Vector Velocity
        {
            get
            {
                return this._velocity;
            }
        }

        public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            base.GetObjectData( si, context );

            si.AddValue( "Velocity", this._velocity );
        }
    }
}