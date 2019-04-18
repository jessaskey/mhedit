using mhedit.Containers.MazeEnemies;

namespace mhedit.Containers.Validation.MajorHavoc
{
    /// <summary>
    /// Validates that a Perkoid's Velocity and IncrementingVelocity are matched
    /// properly.
    ///
    /// ValidationAttribute Properties are not supported (Ignored).
    /// 
    /// Composite format string:
    ///     Value is Index 0
    ///     Default Message is Index 1
    /// </summary>
    public class PerkoidVelocityRule : ValidationRule<Perkoid>
    {
        public PerkoidVelocityRule( ValidationData data )
            : base( data )
        {}

#region Overrides of ValidationRule<Perkoid>

        public override IValidationResult Validate( Perkoid subject )
        {
            /// https://codereview.stackexchange.com/a/107652
            return this.IsSignOfVelocityMismatched( subject ) ?
                       this.CreateResult( subject,
                           $"Perkoid Velocity.Y [{subject.Velocity.Y}] and IncrementingVelocity.Y " +
                           $"[{subject.IncrementingVelocity.Y}] should have the same sign." ) :
                       null;
        }

#endregion

        private bool IsSignOfVelocityMismatched( Perkoid subject )
        {
            /// If velocities are non-zero then they need to have the same sign.
            /// https://codereview.stackexchange.com/a/107652
            return ( subject.Velocity.Y != 0 &&
                     subject.IncrementingVelocity.Y != 0 &&
                     ( subject.Velocity.Y ^ subject.IncrementingVelocity.Y ) < 0 );
        }
    }
}
