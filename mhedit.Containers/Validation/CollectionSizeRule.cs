using System;
using System.Collections;

namespace mhedit.Containers.Validation
{
    /// <summary>
    /// Validates that a collection contains a predetermined number of elements.
    /// Validates that a value is within the range of a supplied Minimum and
    /// Maximum inclusive. For a fixed count/size set Minimum = Maximum.
    ///
    /// ValidationAttribute.Options (Required)
    ///     Maximum
    ///     Minimum (If Maximum = Minimum then the exact count is expected)
    ///
    /// Composite format string:
    ///     Collection object is Index 0
    ///     Default Message is Index 1
    ///     Minimum is Index 2
    ///     Maximum is Index 3
    ///     Collection.Count is Index 4
    /// </summary>
    public class CollectionSizeRule : ValidationRule<ICollection>
    {
        private readonly Range<int> _range = new Range<int>();

        public CollectionSizeRule( ValidationData data )
            : base( data )
        {
            try
            {
                this._range.Maximum = int.Parse( this._options[ "Maximum" ] );

                this._range.Minimum = int.Parse( this._options[ "Minimum" ] );
            }
            catch ( Exception e )
            {
                throw new InvalidOperationException( "Required option is missing.", e );
            }
        }

        public override IValidationResult Validate( ICollection collection )
        {
            return !this._range.ContainsValue( collection.Count ) ?
                       this.CreateResult( collection,
                           $"Collection size: {collection.Count} is outside limits, " +
                           $"expected {this._range.Minimum} <= Size <= {this._range.Maximum}.",
                           this._range.Minimum,
                           this._range.Maximum,
                           collection.Count ) :
                       null;
        }
    }

}