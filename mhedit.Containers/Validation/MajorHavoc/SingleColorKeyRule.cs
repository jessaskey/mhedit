using System.Collections.Generic;
using System.Linq;
using mhedit.Containers.MazeObjects;

namespace mhedit.Containers.Validation.MajorHavoc
{
    /// <summary>
    /// Validates that the MazeObject collection contains only one Key of any given
    /// color. An individual error is generated for each duplicate key.
    ///
    /// ValidationAttribute.Options (none)
    ///
    /// Composite format string: 
    ///     Unmatched Transporter is Index 0
    ///     Default Message is Index 1
    /// </summary>
    public class SingleColorKeyRule : ValidationRule<IEnumerable<MazeObject>>
    {
        public SingleColorKeyRule( ValidationData data )
            : base( data )
        { }

        public override IValidationResult Validate( IEnumerable<MazeObject> mazeObjects )
        {
            Dictionary<ObjectColor, List<Key>> keys =
                new Dictionary<ObjectColor, List<Key>>();

            foreach ( MazeObject mazeObject in mazeObjects )
            {
                if ( mazeObject is Key key )
                {
                    if ( !keys.ContainsKey( key.KeyColor ) )
                    {
                        keys[ key.KeyColor ] = new List<Key>();
                    }

                    keys[ key.KeyColor ].Add( key );
                }
            }

            ValidationResults results = new ValidationResults() { Context = mazeObjects };

            /// If there are unmatched pairs start generating validation results.
            foreach ( KeyValuePair<ObjectColor, List<Key>> pair in keys )
            {
                if ( pair.Value.Count > 1 )
                {
                    results.Add( this.CreateResult( pair.Value,
                        $"Duplicate {pair.Key} Key in Maze: {pair.Value.LastOrDefault()?.Name}. Only one Key of any given color is allowed." ) );
                }
            }

            return results;
        }
    }

}
