using GameEditor.Core;
using GameEditor.Core.Serialization;
using System;
using System.Runtime.Serialization;

namespace GameEditor.Atari.MajorHavoc
{
    [Serializable]
    public abstract class MazeObject : IName, IRomSerializable
    {
        private string _name;
        private readonly MazePosition _position;

        public MazeObject( string name, MazePosition position )
        {
            this._name = name;

            this._position = position;
        }

        protected MazeObject( RomSerializationInfo si, StreamingContext context )
        {
            this._position = (MazePosition)si.GetValue( "Position", typeof( MazePosition ) );
        }

        public string Name
        {
            get
            {
                return this._name;
            }
        }

        public MazePosition Position
        {
            get
            {
                return this._position;
            }
        }

        public virtual void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            //si.AddValue( "Position", this._position, typeof( MazePosition ) );
            si.AddValue( "Position", this._position, this._position.GetType() );
        }
    }

}
