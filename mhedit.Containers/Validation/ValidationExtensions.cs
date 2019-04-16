using System;
using System.Reflection;

namespace mhedit.Containers.Validation
{

    public static class ValidationExtensions
    {
        public static IValidationResult Validate( this object subject )
        {
            ValidationResults results = new ValidationResults()
                                        {
                                            Context = subject,
                                       };

            /// Look for ValidationAttributes on the subject type itself.
            ValidationAttribute[] rules =
                Array.ConvertAll(
                    subject.GetType().GetCustomAttributes( typeof( ValidationAttribute ), true ),
                    item =>
                    {
                        ValidationAttribute attr = (ValidationAttribute) item;

                        /// Do not allow the default attribute (no ValidationRule) to decorate
                        /// a class as it's recursive.
                        if ( attr.IsDefaultAttribute() )
                        {
                            throw new InvalidOperationException(
                                $"Validation Rule required when decorating Class. Review {subject.GetType().FullName}" );
                        }

                        return attr;
                    } );

            foreach ( ValidationAttribute attribute in rules )
            {
                results.Add( attribute.Validate( subject ) );
            }

            PropertyInfo[] properties =
                subject.GetType().GetProperties( BindingFlags.Public | BindingFlags.Instance );

            foreach ( PropertyInfo property in properties )
            {
                object propertyValue = property.GetValue( subject );

                rules =
                    Array.ConvertAll(
                        property.GetCustomAttributes( typeof( ValidationAttribute ), true ),
                        item => (ValidationAttribute) item );

                foreach ( ValidationAttribute attribute in rules )
                {
                    results.Add( attribute.Validate( propertyValue ) );
                }
            }

            return results;
        }
    }

}