using System.Collections.Generic;
using mhedit.Containers.MazeObjects;

namespace mhedit.Containers.Validation.MajorHavoc
{

    /// <summary>
    /// Validates that the MazeObject collection contains Lock/Key color matched
    /// pairs. Any individual Lock/Key that doesn't have a match based on color
    /// fails this test is processed as an individual error (called once for each
    /// for each Lock/Key that fails).
    ///
    /// ValidationAttribute.Options (none)
    ///
    /// Composite format string: 
    ///     Unmatched Key/Lock is Index 0
    ///     Default Message is Index 1
    /// </summary>
    public class KeyLockPairsRule : ValidationRule<IEnumerable<MazeObject>>
    {
        public KeyLockPairsRule( ValidationData data )
            : base( data )
        {}

        public override IValidationResult Validate( IEnumerable<MazeObject> mazeObjects )
        {
            Dictionary<ObjectColor, Key> keys = new Dictionary<ObjectColor, Key>();
            Dictionary<ObjectColor, Lock> locks = new Dictionary<ObjectColor, Lock>();

            foreach ( MazeObject mazeObject in mazeObjects )
            {
                /// DON'T use Add() method as it will throw on duplicates. That's a different
                /// rule. Just look to ensure pairs exist.
                if ( mazeObject is Key key )
                {
                    keys[ key.KeyColor ] = key;
                }
                else if ( mazeObject is Lock @lock )
                {
                    locks[ @lock.LockColor ] = @lock;
                }
            }

            List<ObjectColor> colorPairs = new List<ObjectColor>();

            /// Create a collection of Colors that represent pairs.
            foreach ( ObjectColor color in keys.Keys )
            {
                if ( locks.ContainsKey( color ) )
                {
                    colorPairs.Add( color );
                }
            }

            ValidationResults results = null;

            /// If there are unmatched pairs start generating validation results.
            if ( colorPairs.Count != keys.Count ||
                 colorPairs.Count != locks.Count )
            {
                /// Eliminate the valid pairs so only errors are left.
                foreach ( ObjectColor color in colorPairs )
                {
                    keys.Remove( color );

                    locks.Remove( color );
                }

                results = new ValidationResults() { Context = mazeObjects };

                foreach ( KeyValuePair<ObjectColor, Key> pair in keys )
                {
                    results.Add( this.CreateResult( pair.Value,
                        $"Unmatched Key: {pair.Value.Name}. Expected matching {pair.Key} Lock in Maze." ) );
                }

                foreach ( KeyValuePair<ObjectColor, Lock> pair in locks )
                {
                    results.Add( this.CreateResult( pair.Value,
                        $"Unmatched Lock: {pair.Value.Name}. Expected matching {pair.Key} Key in Maze." ) );
                }
            }

            return results;
        }
    }

}
