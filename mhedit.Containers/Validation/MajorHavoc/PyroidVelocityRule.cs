using mhedit.Containers.MazeEnemies;

namespace mhedit.Containers.Validation.MajorHavoc
{
    /// <summary>
    /// Validates that a Pyroids's Velocity and IncrementingVelocity are matched
    /// properly.
    ///
    /// ValidationAttribute Properties are not supported (Ignored).
    /// 
    /// Composite format string:
    ///     Value is Index 0
    ///     Default Message is Index 1
    /// </summary>
    public class PyroidVelocityRule : ValidationRule<Pyroid>
    {
        public PyroidVelocityRule( ValidationData data )
            : base( data )
        { }

#region Overrides of ValidationRule<Pyroid>

        public override IValidationResult Validate( Pyroid subject )
        {
            /// https://codereview.stackexchange.com/a/107652
            return this.IsSignOfVelocityMismatched( subject ) ?
                       this.CreateResult( subject,
                           $"A Pyroid's Velocity [{subject.Velocity}] and IncrementingVelocity [{subject.IncrementingVelocity}]" +
                           " X and Y components should have the same sign. " ):
                       null;
        }

#endregion

        private bool IsSignOfVelocityMismatched( Pyroid subject )
        {
            /// If velocities are non-zero then they need to have the same sign.
            /// https://codereview.stackexchange.com/a/107652
            return ( subject.Velocity.X != 0 &&
                     subject.IncrementingVelocity.X != 0 &&
                     ( subject.Velocity.X ^ subject.IncrementingVelocity.X ) < 0 )
                   ||
                   ( subject.Velocity.Y != 0 &&
                     subject.IncrementingVelocity.Y != 0 &&
                     ( subject.Velocity.Y ^ subject.IncrementingVelocity.Y ) < 0 );
        }
    }
}
