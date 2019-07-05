using System.Collections.Generic;
using mhedit.Containers.MazeObjects;

namespace mhedit.Containers.Validation.MajorHavoc
{
    /// <summary>
    /// Validates that any given pair of Transporters ( Paired by color )
    /// face opposite directions. Any Transporter pair that doesn't is
    /// processed as an individual error.
    ///
    /// ValidationAttribute.Options (none)
    ///
    /// Composite format string: 
    ///     Transporter 1 is Index 0
    ///     Default Message is Index 1
    ///     Transporter 2 is Index 2
    /// </summary>
    public class TransporterDirectionRule : ValidationRule<IEnumerable<MazeObject>>
    {
        public TransporterDirectionRule( ValidationData data )
            : base( data )
        { }

        public override IValidationResult Validate( IEnumerable<MazeObject> mazeObjects )
        {
            Dictionary<ObjectColor, List<Transporter>> transporters =
                new Dictionary<ObjectColor, List<Transporter>>();

            foreach ( MazeObject mazeObject in mazeObjects )
            {
                if ( mazeObject is Transporter transporter )
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
                /// Don't deal with Anything other than Transporter Pairs
                if ( pair.Value.Count == 2 &&
                     pair.Value[0].Direction == pair.Value[ 1 ].Direction )
                {
                    results.Add( this.CreateResult( pair.Value[ 0 ],
                        $"Transporter pairs must have different Direction: {pair.Value[ 0 ].Name}/{pair.Value[ 1 ].Name}.",
                        pair.Value[ 1 ] ) );
                }
            }

            return results;
        }
    }

}
