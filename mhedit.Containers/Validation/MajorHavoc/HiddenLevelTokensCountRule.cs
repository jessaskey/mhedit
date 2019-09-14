using System;
using System.Collections.Generic;
using System.Linq;
using mhedit.Containers.MazeObjects;

namespace mhedit.Containers.Validation.MajorHavoc
{
    /// <summary>
    /// Validates that the Maze collection contains 0 to 4 HiddenLevelTokens.
    ///
    /// ValidationAttribute.Options (Required)
    ///     Maximum
    ///     Minimum (If Maximum = Minimum then the exact length is expected)
    ///
    /// Composite format string: 
    ///     Value is Index 0
    ///     Default Message is Index 1
    ///     Minimum is Index 2
    ///     Maximum is Index 3
    /// </summary>
    public class HiddenLevelTokensCountRule : ValidationRule<IEnumerable<Maze>>
    {
        private readonly Range<int> _range = new Range<int>();

        public HiddenLevelTokensCountRule( ValidationData data )
            : base( data )
        {
            this._range.Maximum = (int)Convert.ChangeType( this._options[ "Maximum" ], typeof( int ) );

            this._range.Minimum = (int)Convert.ChangeType( this._options[ "Minimum" ], typeof( int ) );
        }

#region Overrides of ValidationRule<IEnumerable<Maze>>

        public override IValidationResult Validate( IEnumerable<Maze> mazes )
        {
            List<HiddenLevelToken> tokens = new List<HiddenLevelToken>();

            foreach ( Maze maze in mazes )
            {
                tokens.AddRange( maze.MazeObjects.OfType<HiddenLevelToken>() );
            }

            return !this._range.ContainsValue( tokens.Count ) ?
                       this.CreateResult( tokens.Count,
                           $"Hidden Level Token count: {tokens.Count}, expected {this._range.Minimum} <= value <= {this._range.Maximum}.",
                           this._range.Minimum,
                           this._range.Maximum ) :
                       null;

        }

#endregion

    }

}
