using System;
using System.Collections.Generic;
using mhedit.Containers.Validation;

namespace mhedit.Containers.MazeEnemies.IonCannon
{
    /// <summary>
    /// Base class for cannon behavior programming.
    /// </summary>
    [Serializable]
    public sealed class Pause : IonCannonInstruction
    {
        private int _waitFrames;

        public Pause()
            : base( Commands.Pause )
        { }

        [Validation( typeof( RangeRule<int> ),
            Options = "Minimum=0;Maximum=63" )]
        public int WaitFrames
        {
            get
            {
                return _waitFrames;
            }
            set
            {
                this.SetField( ref this._waitFrames, value);
            }
        }

        public override void GetObjectData( List<byte> bytes )
        {
            bytes.Add( this.SerializeCommand( (byte)( this._waitFrames) ) );
        }

        /// <inheritdoc />
        protected override IonCannonInstruction InternalClone()
        {
            return new Pause { _waitFrames = this._waitFrames};
        }

        //public override string ToString()
        //{
        //    //return $"Pause  [ WaitFrames:{this.WaitFrames} ]";
        //}
    }
}
