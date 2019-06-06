using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace mhedit.Containers.Validation
{

    public static class ValidationExtensions
    {
        public static ISystemWindows SystemWindows;

        public static void ValidateAndDisplayResults( this object subject )
        {
            SystemWindows.Add( new ValidationWindow( subject.Validate() ) );
        }

        public static void DisplayResults( this IValidationResult results )
        {
            SystemWindows.Add( new ValidationWindow( results ) );
        }

        public static void ValidateToMessageBox( this object subject )
        {
            IValidationResult validationResult = subject.Validate();

            if ( validationResult.Level > ValidationLevel.Message )
            {
                MessageBox.Show( validationResult.ToString(), "Validation Errors", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
            }
        }

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
                rules =
                    Array.ConvertAll(
                        property.GetCustomAttributes( typeof( ValidationAttribute ), true ),
                        item => (ValidationAttribute) item );

                if ( rules.Length > 0 )
                {
                    /// Get the value of the property only when a validation attribute
                    /// is found. This eliminates issues with GetProperties() returning
                    /// indexer "properties" on collection classes which require a method
                    /// parameter (the index) and result in a throw. I don't know how to
                    /// test for that situation. Besides, probably better to get the
                    /// property value only when it's going to be used.
                    object propertyValue = property.GetValue( subject );

                    foreach ( ValidationAttribute attribute in rules )
                    {
                        results.Add( attribute.Validate( propertyValue ) );
                    }
                }
            }

            return results;
        }
    }

}