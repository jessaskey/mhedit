using GameEditor.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GameEditor.Atari.MajorHavoc.Maze
{
    /// <summary>
    /// A vector class used to model Moving Oids in the Maze.
    /// </summary>
    [Serializable]
    public class Vector : IRomSerializable
    {
        private readonly Component _x;
        private readonly Component _y;

        public Vector()
        {
            this._x = new Component();

            this._y = new Component();
        }

        private Vector( RomSerializationInfo si, StreamingContext context )
        {
            this._x = (Component)si.GetValue( "X", typeof( Component ) );

            this._y = (Component)si.GetValue( "Y", typeof( Component ) );
        }

        public Component X
        {
            get { return this._x; }
        }

        public Component Y
        {
            get { return this._y; }
        }

        public void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            si.AddValue( "X", this._x );

            si.AddValue( "Y", this._x );
        }
    }
}
