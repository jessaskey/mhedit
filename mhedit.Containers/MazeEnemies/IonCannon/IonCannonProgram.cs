using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace mhedit.Containers.MazeEnemies.IonCannon
{
    [Serializable]
    public sealed class IonCannonProgram : List<IonCannonBehavior>
    {
        public IonCannonProgram()
            : base()
        {}

        //private IonCannonProgram( List<byte> bytes )
        //    : base()
        //{
        //    /// Must be at least one command!!
        //    Commands command = Commands.ReturnToStart;
        //    do
        //    {
        //        /// use lambda closure to update local while performing test.
        //        si.TryGet<byte>( $"CannonCommand{this.Count}",
        //            b => ( command = (Commands)( b >> 6 ) ) == Commands.ReturnToStart );

        //        switch ( command )
        //        {
        //            case Commands.ReturnToStart:
        //                this.Add( (IonCannonBehavior)
        //                    si.GetValue( "ReturnToStart", typeof(ReturnToStart) ) );
        //                break;

        //            case Commands.OrientAndFire:
        //                this.Add( (IonCannonBehavior)
        //                    si.GetValue( $"Aim{this.Count}", typeof( OrientAndFire ) ) );
        //                break;

        //            case Commands.Move:
        //                this.Add( (IonCannonBehavior)
        //                    si.GetValue( $"Move{this.Count}", typeof( Move ) ) );
        //                break;

        //            case Commands.Pause:
        //                this.Add( (IonCannonBehavior)
        //                    si.GetValue( $"Pause{this.Count}", typeof( Pause ) ) );
        //                break;

        //            default:
        //                throw new ArgumentOutOfRangeException(
        //                    nameof( Commands ), command, "Unrecognized command." );
        //        }
        //    }
        //    while ( command != Commands.ReturnToStart );

        //}

        public void GetObjectData( List<byte> bytes )
        {
            /// Ensure that there is only one ReturnToStart command and it's the
            /// last one in the Program.
            int index = this.FindIndex( b => b is ReturnToStart );

            if ( index != (this.Count -1) )
            {
                throw new InvalidOperationException(
                    "Expected IonCannon program to end with ReturnToStart command." );
            }

            foreach ( var command in this )
            {
                command.GetObjectData( bytes );
            }
        }
    }
}
