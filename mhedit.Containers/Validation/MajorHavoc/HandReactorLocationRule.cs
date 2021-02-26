using System;
using System.Collections.Generic;
using mhedit.Containers.MazeObjects;

namespace mhedit.Containers.Validation.MajorHavoc
{

    public class HandReactorLocationRule : ValidationRule<IEnumerable<MazeObject>>
    {
        public HandReactorLocationRule( ValidationData data )
            : base( data )
        {
        }

        public override IValidationResult Validate( IEnumerable<MazeObject> mazeObjects )
        {
            Hand hand = null;
            Reactoid reactoid = null;

            foreach ( MazeObject mazeObject in mazeObjects )
            {
                /// look for the Hand and the Reactoid.
                if ( mazeObject is Hand h )
                {
                    hand = h;
                }
                else if ( mazeObject is Reactoid r )
                {
                    reactoid = r;
                }

                if ( hand != null && reactoid != null )
                {
                    return this.Validate( hand, reactoid );
                }
            }

            return null;
        }

        private IValidationResult Validate( Hand hand, Reactoid reactoid )
        {

            byte[] lowRezHandPosition = DataConverter.PointToByteArrayShort( hand.Position );

            byte[] lowRezReactoidPosition =
                DataConverter.PointToByteArrayShort( reactoid.Position );

            /// The Hand must be in the same row or higher and at least one stamp to the left
            ///  of the Reactoid or bad things happen.
            return lowRezHandPosition[ 0 ] >= lowRezReactoidPosition[ 0 ] ||
                   lowRezHandPosition[ 1 ] <= lowRezReactoidPosition[ 1 ] ?
                       this.CreateResult( new List<MazeObject> { hand, reactoid },
                           $"De Hand must be to the left and above the Reactoid!",
                           hand,
                           reactoid ) :
                       null;
        }
    }

}

