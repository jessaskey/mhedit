using System;

namespace mhedit.Containers.Validation
{
    /// <summary>
    /// Validates that a value is within the range of a supplied Minimum and Maximum
    /// inclusive.
    ///
    /// ValidationAttribute.Options (Required)
    ///     Maximum
    ///     Minimum (If Maximum = Minimum then the exact count is expected)
    /// 
    /// Composite format string:
    ///     Value is Index 0
    ///     Default Message is Index 1
    ///     Minimum is Index 2
    ///     Maximum is Index 3
    /// </summary>
    public class RangeRule<T> : ValidationRule<T> where T : IComparable<T>
    {
        private readonly Range<T> _range = new Range<T>();

        public RangeRule( ValidationData data )
            : base( data )
        {
            this._range.Maximum = (T) Convert.ChangeType( this._options[ "Maximum" ], typeof( T ) );

            this._range.Minimum = (T) Convert.ChangeType( this._options[ "Minimum" ], typeof( T ) );
        }

        public override IValidationResult Validate( T value )
        {
            return !this._range.ContainsValue( value ) ?
                       this.CreateResult( value,
                           $"Invalid value: {value}, expected {this._range.Minimum} <= value <= {this._range.Maximum}.",
                           this._range.Minimum,
                           this._range.Maximum ) :
                       null;
        }
    }

}