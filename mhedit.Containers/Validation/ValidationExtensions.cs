using System;
using System.Reflection;
using System.Windows.Forms;

namespace mhedit.Containers.Validation
{

    public static class ValidationExtensions
    {

        public static void ValidateToMessageBox( this object subject, string caption = "" )
        {
            //if ( !subject.GetType().Namespace.Contains( "mhedit" ) )
            //{
            //    throw new ArgumentException("Validation subject not an EditorObject");
            //}

            IValidationResult validationResult = subject.Validate();

            string resultsMessage = validationResult.ToString();

            MessageBox.Show(
                string.IsNullOrWhiteSpace( resultsMessage ) ?
                    "There were no errors" : resultsMessage,
                string.IsNullOrWhiteSpace( caption ) ?
                    "Validation Errors" : caption,
                MessageBoxButtons.OK,
                validationResult.Level > ValidationLevel.Message ?
                    MessageBoxIcon.Exclamation :
                    MessageBoxIcon.Information );
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