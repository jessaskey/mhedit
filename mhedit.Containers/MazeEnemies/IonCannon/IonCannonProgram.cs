using System;
using System.Collections.Generic;
using System.Linq;
using mhedit.Containers.Validation;

namespace mhedit.Containers.MazeEnemies.IonCannon
{

    [ Serializable ]
    [ Validation( typeof( CollectionContentRule<ReturnToStart> ),
        Message = "Every Ion Cannon Program requires a single ReturnToStart. {4} were found.",
        Options = "Maximum=1;Minimum=1" ) ]
    [ Validation( typeof( CollectionIndexRule<ReturnToStart> ),
        Message = "Ion Cannon Program requires ReturnToStart at end of program.",
        Options = "Index=-1" ) ]
    [ Validation( typeof( ElementsRule ) ) ]
    public sealed class IonCannonProgram : ExtendedObservableCollection<IonCannonInstruction>,
                                           ICloneable
    {
        public IonCannonProgram()
        {
        }

        public IonCannonProgram( IEnumerable<IonCannonInstruction> enumerable )
            : base( enumerable )
        {
        }

        public void GetObjectData( List<byte> bytes )
        {
            /// Ensure that there is only one ReturnToStart command and it's the
            /// last one in the Program.
            int index = this.IndexOf( this.FirstOrDefault( b => b is ReturnToStart ) );

            if ( index != ( this.Count - 1 ) )
            {
                throw new InvalidOperationException(
                    "Expected IonCannon program to end with ReturnToStart command." );
            }

            foreach ( var command in this )
            {
                command.GetObjectData( bytes );
            }
        }

#region Implementation of ICloneable

        /// <inheritdoc />
        public object Clone()
        {
            return new IonCannonProgram(
                   this.Select( o => o.Clone() ).Cast<IonCannonInstruction>() )
                   {
                       IsChanged = this.IsChanged
                   };
        }

#endregion
    }

}