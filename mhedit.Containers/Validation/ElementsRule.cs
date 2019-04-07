using System.Collections;

namespace mhedit.Containers.Validation
{
    /// <summary>
    /// Validation Rule that performs validation on each element of a collection using the
    /// rules associated with the Type of the collection's elements. 
    /// </summary>
    public class ElementsRule : ValidationRule<IEnumerable>
    {
        public ElementsRule( ValidationData data )
            : base( data )
        {}

        public override IValidationResult Validate( IEnumerable enumerable )
        {
            ValidationResults results = new ValidationResults()
                                        {
                                            Context = enumerable,
                                        };

            foreach ( object element in enumerable )
            {
                results.Add( element.Validate() );
            }

            return results;
        }
    }

}