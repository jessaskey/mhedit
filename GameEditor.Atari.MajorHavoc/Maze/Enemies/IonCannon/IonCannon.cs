using GameEditor.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameEditor.Atari.MajorHavoc.Maze.Enemies.IonCannon
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [ConcreteType( typeof( HighResolutionPosition ) )]
    [CollectionType( typeof( IonCannonProgram ) )]
    [TerminationObject( (byte)0x00 )]
    public sealed class IonCannon : MazeObject
    {
        private readonly IonCannonProgram _cannonProgram;

        public IonCannon()
            : base( "IonCannon", new LowResolutionPosition() )
        {
            this._cannonProgram = new IonCannonProgram();
        }

        private IonCannon( RomSerializationInfo si, StreamingContext context )
            : base( si, context )
        {
            this._cannonProgram = 
                (IonCannonProgram)si.GetValue( "IonCannonProgram", typeof( IonCannonProgram ) );
        }

        public List<IonCannonBehavior> Orientation
        {
            get
            {
                return this._cannonProgram;
            }
        }

        public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            base.GetObjectData( si, context );

            si.AddValue( "IonCannonProgram", this._cannonProgram );
        }
    }
}
