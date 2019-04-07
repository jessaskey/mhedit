using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

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

            ValidationAttribute[] rules =
                Array.ConvertAll(
                    subject.GetType().GetCustomAttributes( typeof( ValidationAttribute ), true ),
                    item => (ValidationAttribute)item );

            if ( rules.Length > 0 )
            {
                IEnumerable<IValidationResult> failures =
                    rules.Select(
                             r => r.HasRule ?
                                      r.Validate( subject ) :
                                      throw new InvalidOperationException(
                                          $"Validation Rule is required for Class. Review {subject.GetType().FullName}" ) )
                         .Where( f => f != null ).ToList();

                foreach ( IValidationResult validationResult in failures )
                {
#if DEBUG
                    Debug.WriteLine( validationResult.ToString() );
#else
                        /// No sense in looping if we've already found an error.
                        if ( results.Level == ValidationLevel.Error )
                        {
                            break;
                        }
#endif
                    if ( results.Level > validationResult.Level )
                    {
                        results.Level = validationResult.Level;
                    }
                }
                results.AddRange( failures );
            }

            PropertyInfo[] properties =
                subject.GetType().GetProperties( BindingFlags.Public | BindingFlags.Instance );

            foreach ( PropertyInfo property in properties )
            {
                object propertyValue = property.GetValue( subject );

                rules =
                    Array.ConvertAll(
                        property.GetCustomAttributes( typeof( ValidationAttribute ), true ),
                        item => (ValidationAttribute)item );

                if ( rules.Length > 0 )
                {
                    IEnumerable<IValidationResult> failures =
                        rules.Select( r => r.Validate( propertyValue ) )
                             .Where( f => f != null ).ToList();

                    foreach ( IValidationResult validationResult in failures )
                    {
#if DEBUG
                        Debug.WriteLine( validationResult.ToString() );
#else
                        /// No sense in looping if we've already found an error.
                        if ( results.Level == ValidationLevel.Error )
                        {
                            break;
                        }
#endif
                        if ( results.Level > validationResult.Level )
                        {
                            results.Level = validationResult.Level;
                        }
                    }

                    results.AddRange( failures );
                }
            }

            return results;
        }

//        private static IEnumerable<IValidationResult> GetResults( this MemberInfo memberInfo,
//            object subject )
//        {
//            var rules =
//                Array.ConvertAll(
//                    memberInfo.GetCustomAttributes( typeof( ValidationAttribute ), true ),
//                    item => (ValidationAttribute)item );

//            if ( rules.Length > 0 )
//            {
//                IEnumerable<IValidationResult> failures =
//                    rules.Select( r => r.Validate( subject ) )
//                         .Where( f => f != null ).ToList();

//                foreach ( IValidationResult validationResult in failures )
//                {
//#if DEBUG
//                    Debug.WriteLine( validationResult.ToString() );
//#else
//                        /// No sense in looping if we've already found an error.
//                        if ( results.Level == ValidationLevel.Error )
//                        {
//                            break;
//                        }
//#endif
//                    if ( results.Level > validationResult.Level )
//                    {
//                        results.Level = validationResult.Level;
//                    }
//                }
//                results.AddRange( failures );
//            }
//        }
    }

}