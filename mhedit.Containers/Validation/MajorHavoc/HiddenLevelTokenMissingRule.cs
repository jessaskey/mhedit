using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using mhedit.Containers.MazeObjects;

namespace mhedit.Containers.Validation.MajorHavoc
{
    /// <summary>
    /// Validates that the Maze collection isn't missing a Hidden Level Token for each
    /// Hidden level.
    ///
    /// ValidationAttribute.Options (none)
    ///
    /// Composite format string: 
    ///     Missing Level is Index 0
    ///     Default Message is Index 1
    /// </summary>
    public class HiddenLevelTokenMissingRule : ValidationRule<IEnumerable<Maze>>
    {
        public HiddenLevelTokenMissingRule( ValidationData data )
            : base( data )
        { }

#region Overrides of ValidationRule<IEnumerable<Maze>>

        public override IValidationResult Validate( IEnumerable<Maze> mazes )
        {
            Hashtable tokens = new Hashtable();

            foreach ( Maze maze in mazes )
            {
                foreach ( HiddenLevelToken token in maze.MazeObjects.OfType<HiddenLevelToken>() )
                {
                    tokens[ token.TargetLevel ] = token;
                }
            }

            ValidationResults results = new ValidationResults() { Context = mazes };

            /// If there are unreferenced Hidden levels start generating validation results.
            foreach ( HiddenLevels en in Enum.GetValues( typeof( HiddenLevels ) ) )
            {
                if ( !tokens.ContainsKey( en ) )
                {
                    results.Add( this.CreateResult( en, $"Hidden Level {en} not referenced via Hidden Level Token." ) );
                }
            }

            return results;
        }

#endregion

    }

}
