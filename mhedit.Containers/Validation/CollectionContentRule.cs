using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace mhedit.Containers.Validation
{
    /// <summary>
    /// Validates that a collection contains a predetermined number of elements of a
    /// specified type. Validates that a value is within the range of a supplied
    /// Minimum and Maximum inclusive. For a fixed count/size set Minimum = Maximum.
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
    ///     TContent.Name is Index 5
    /// </summary>
    public class CollectionContentRule<TContent> : ValidationRule<IEnumerable>
    {
        private readonly Range<int> _range = new Range<int>();

        public CollectionContentRule( ValidationData data )
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

        public override IValidationResult Validate( IEnumerable collection )
        {
            List<TContent> actual = collection.OfType<TContent>().ToList();

            return !this._range.ContainsValue( actual.Count ) ?
                       this.CreateResult( collection,
                           $"Expected {this._range} {typeof( TContent ).Name} objects, found {actual.Count}.",
                           this._range.Minimum,
                           this._range.Maximum,
                           actual.Count,
                           typeof( TContent ).Name ) :
                       null;
        }
    }

}