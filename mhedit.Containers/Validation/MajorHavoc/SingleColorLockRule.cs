using System.Collections.Generic;
using System.Linq;
using mhedit.Containers.MazeObjects;

namespace mhedit.Containers.Validation.MajorHavoc
{
    /// <summary>
    /// Validates that the MazeObject collection contains only one Lock of any given
    /// color. An individual error is generated for each duplicate lock.
    ///
    /// ValidationAttribute.Options (none)
    ///
    /// Composite format string: 
    ///     Unmatched Transporter is Index 0
    ///     Default Message is Index 1
    /// </summary>
    public class SingleColorLockRule : ValidationRule<IEnumerable<MazeObject>>
    {
        public SingleColorLockRule( ValidationData data )
            : base( data )
        { }

        public override IValidationResult Validate( IEnumerable<MazeObject> mazeObjects )
        {
            Dictionary<ObjectColor, List<Lock>> locks =
                new Dictionary<ObjectColor, List<Lock>>();

            foreach ( MazeObject mazeObject in mazeObjects )
            {
                if ( mazeObject is Lock @lock )
                {
                    if ( !locks.ContainsKey( @lock.LockColor ) )
                    {
                        locks[ @lock.LockColor ] = new List<Lock>();
                    }

                    locks[ @lock.LockColor ].Add( @lock );
                }
            }

            ValidationResults results = new ValidationResults() { Context = mazeObjects };

            /// If there are unmatched pairs start generating validation results.
            foreach ( KeyValuePair<ObjectColor, List<Lock>> pair in locks )
            {
                if ( pair.Value.Count > 1 )
                {
                    results.Add( this.CreateResult( pair.Value,
                        $"Duplicate {pair.Key} Lock in Maze: {pair.Value.LastOrDefault()?.Name}. Only one Lock of any given color is allowed." ) );
                }
            }

            return results;
        }
    }

}
