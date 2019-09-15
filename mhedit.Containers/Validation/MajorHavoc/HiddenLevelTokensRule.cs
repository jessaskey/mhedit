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
            Dictionary<TokenStyle, List<Tuple<HiddenLevelToken, Maze>>> tokens = new Dictionary<TokenStyle, List<Tuple<HiddenLevelToken, Maze>>>();

            foreach (Maze maze in mazes)
            {
                foreach (var token in maze.MazeObjects.OfType<HiddenLevelToken>())
                {
                    if (!tokens.ContainsKey(token.TokenStyle))
                    {
                        tokens[token.TokenStyle] = new List<Tuple<HiddenLevelToken, Maze>>();
                    }
                    tokens[token.TokenStyle].Add(Tuple.Create(token, maze));
                }
            }

            ValidationResults results = new ValidationResults() { Context = mazes };

            /// Look for duplicates.
            foreach (KeyValuePair<TokenStyle, List<Tuple<HiddenLevelToken, Maze>>> pair in tokens)
            {
                if (pair.Value.Count > 1)
                {
                    results.Add(this.CreateResult(pair.Value,
                        $"Duplicate Token Style in Maze Collection {pair.Key} in Maze: {pair.Value.LastOrDefault()?.Item2.Name}. Each Token Style may only be used once in the entire maze collection."));
                }
            }

            return results;
        }
    }

#endregion

}
