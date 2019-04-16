using System.Collections;

namespace mhedit.Containers.Validation
{
    /// <summary>
    /// Validation Rule that performs validation on each element of a collection using the
    /// rules associated with the Type of the collection's elements.
    /// 
    /// ValidationAttribute Properties are not supported (Ignored).
    /// 
    /// </summary>
    public class ElementsRule : IValidationRule<IEnumerable>
    {
        public ElementsRule( ValidationData data )
        {}

#region Implementation of IValidationRule<in IEnumerable>

        public IValidationResult Validate( IEnumerable enumerable )
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

#endregion

    }

}