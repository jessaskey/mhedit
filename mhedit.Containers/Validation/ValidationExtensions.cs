using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace mhedit.Containers.Validation
{

    public static class ValidationExtensions
    {
        public static IValidationResult Validate( this object subject )
        {
            ValidationResults results = new ValidationResults();

            PropertyInfo[] properties = subject.GetType().GetProperties();

            foreach ( PropertyInfo property in properties )
            {
                object propertyValue = property.GetValue( subject );

                var rules =
                    Array.ConvertAll(
                        property.GetCustomAttributes( typeof( ValidationAttribute ), true ),
                        item => (ValidationAttribute)item );

                if ( rules.Length > 0 )
                {
                    IEnumerable<IValidationResult> failures =
                        rules.Select( r => r.Validate( propertyValue ) )
                             .Where( f => f != null ).ToList();
#if DEBUG
                    foreach ( IValidationResult validationResult in failures )
                    {
                        Debug.WriteLine( validationResult.ToString() );
                    }
#endif
                    results.AddRange( failures );
                }
            }

            return results;
        }
    }

}