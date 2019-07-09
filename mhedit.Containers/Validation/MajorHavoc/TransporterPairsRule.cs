using System.Collections.Generic;
using System.Linq;
using mhedit.Containers.MazeObjects;

namespace mhedit.Containers.Validation.MajorHavoc
{
    /// <summary>
    /// Validates that the MazeObject collection contains Transporter color matched
    /// pairs. This does NOT validate Intra-Level Transporters, it ignores them. Please
    /// see IntraLevelTransporterRule.
    /// Any individual Transporter that doesn't have a match based on color
    /// fails this test is processed as an individual error.
    ///
    /// ValidationAttribute.Options (none)
    ///
    /// Composite format string: 
    ///     Unmatched Transporter is Index 0
    ///     Default Message is Index 1
    /// </summary>
    class TransporterPairsRule : ValidationRule<IEnumerable<MazeObject>>
    {
        public TransporterPairsRule( ValidationData data )
            : base( data )
        { }

        public override IValidationResult Validate( IEnumerable<MazeObject> mazeObjects )
        {
            Dictionary<ObjectColor, List<Transporter>> transporters =
                new Dictionary<ObjectColor, List<Transporter>>();

            foreach ( MazeObject mazeObject in mazeObjects )
            {
                /// Ignore Intra-Level Transporters!
                if ( mazeObject is Transporter transporter &&
                     !transporter.IsSpecial )
                {
                    if ( !transporters.ContainsKey( transporter.Color ) )
                    {
                        transporters[ transporter.Color ] = new List<Transporter>();
                    }

                    transporters[ transporter.Color ].Add( transporter );
                }
            }

            ValidationResults results = new ValidationResults() { Context = mazeObjects };

            /// If there are unmatched pairs start generating validation results.
            foreach ( KeyValuePair<ObjectColor, List<Transporter>> pair in transporters )
            {
                if ( pair.Value.Count < 2 )
                {
                    results.Add( this.CreateResult( pair.Value,
                        $"Unmatched Transporter: {pair.Value.FirstOrDefault()?.Name}. Expected matching {pair.Key} Transporter in Maze." ) );
                }
                else if ( pair.Value.Count > 2 )
                {
                    results.Add( this.CreateResult( pair.Value,
                        $"One or more extra {pair.Key} Transporters in Maze: {pair.Value.LastOrDefault()?.Name}. Transporters must be in pairs by color." ) );
                }
            }

            return results;
        }
    }

}
