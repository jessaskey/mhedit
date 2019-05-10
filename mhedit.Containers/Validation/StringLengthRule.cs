using System;

namespace mhedit.Containers.Validation
{
    /// <summary>
    ////Validates that a string length is within the range of a supplied Minimum and Maximum
    /// inclusive.
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
    public class StringLengthRule : ValidationRule<string>
    {
        private readonly Range<int> _range = new Range<int>();

        public StringLengthRule( ValidationData data )
            : base( data )
        {
            this._range.Maximum = (int)Convert.ChangeType(this._options["Maximum"], typeof(int));

            this._range.Minimum = (int)Convert.ChangeType(this._options["Minimum"], typeof(int));
        }

#region Overrides of ValidationRule<string>

        public override IValidationResult Validate( string str )
        {
            return !this._range.ContainsValue(str.Length) ?
                                   this.CreateResult(str.Length,
                                       $"Invalid Length: {str.Length}, expected {this._range.Minimum} <= value <= {this._range.Maximum}.",
                                       this._range.Minimum,
                                       this._range.Maximum) :
                                   null;
        }

#endregion

    }

}