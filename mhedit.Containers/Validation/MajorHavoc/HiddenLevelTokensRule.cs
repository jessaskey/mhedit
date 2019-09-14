using System;
using System.Collections.Generic;
using System.Linq;
using mhedit.Containers.MazeObjects;

namespace mhedit.Containers.Validation.MajorHavoc
{
    /// <summary>
    /// Validates that the Maze collection contains only one HiddenLevelToken targeting
    /// any particular Hidden Level. An individual error is generated for each duplicate
    /// Target Level.
    ///
    /// ValidationAttribute.Options (none)
    ///
    /// Composite format string: 
    ///     Tuple<HiddenLevelToken,Maze> of duplicate Token is Index 0
    ///     Default Message is Index 1
    /// </summary>
    public class HiddenLevelTokensRule : ValidationRule<IEnumerable<Maze>>
    {
        public HiddenLevelTokensRule( ValidationData data )
            : base( data )
        { }

#region Overrides of ValidationRule<IEnumerable<Maze>>

        public override IValidationResult Validate( IEnumerable<Maze> mazes )
        {
            Dictionary<HiddenLevels, List<Tuple<HiddenLevelToken, Maze>>> tokens =
                new Dictionary<HiddenLevels, List<Tuple<HiddenLevelToken, Maze>>>();

            foreach ( Maze maze in mazes )
            {
                foreach ( var token in maze.MazeObjects.OfType<HiddenLevelToken>() )
                {
                    if ( !tokens.ContainsKey( token.TargetLevel ) )
                    {
                        tokens[ token.TargetLevel ] = new List<Tuple<HiddenLevelToken, Maze>>();
                    }

                    tokens[ token.TargetLevel ].Add( Tuple.Create( token, maze ) );
                }
            }

            ValidationResults results = new ValidationResults() { Context = mazes };

            /// Look for duplicates.
            foreach ( KeyValuePair<HiddenLevels, List<Tuple<HiddenLevelToken, Maze>>> pair in tokens )
            {
                if ( pair.Value.Count > 1 )
                {
                    results.Add( this.CreateResult( pair.Value,
                        $"Duplicate Token for Hidden Level {pair.Key} in Maze: {pair.Value.LastOrDefault()?.Item2.Name}. Only one Token for each Hidden Level is allowed." ) );
                }
            }

            return results;
        }
    }

#endregion

}
