using System;
using System.Reflection;

namespace mhedit.Containers.Validation
{

    /// <summary>
    /// Specifies an implementation of IValidationRule<T> that is used to validate
    /// the Class instance or Property value. This class cannot be inherited.
    /// </summary>
    [ AttributeUsage( AttributeTargets.Property | AttributeTargets.Class,
        AllowMultiple = true ) ]
    public sealed class ValidationAttribute : Attribute
    {
        //public readonly Guid G = Guid.NewGuid();

        private readonly Type _ruleType;
        private readonly ValidationData _data = new ValidationData();

        /// <summary>
        /// Initializes a new instance of the ValidationAttribute class with no parameters
        /// indicating that the Property value should be subject to its Type's validation.
        /// This constructor is not valid for a ValidationAttribute decorating a Class.
        /// </summary>
        public ValidationAttribute()
        {}

        /// <summary>
        /// Initializes a new instance of the ValidationAttribute class with a type that
        /// implements IValidationRule<T> and is used to validate the Class instance or
        /// Property value.
        /// </summary>
        /// <param name="validationRuleType">The Type object for the ValidationRule to
        /// be applied during validation.</param>
        public ValidationAttribute( Type validationRuleType )
        {
            this._ruleType = validationRuleType;
        }

        /// <summary>
        /// The severity level of the Validation result in the case of failure.
        /// </summary>
        public ValidationLevel Level
        {
            get { return this._data.Level; }
            set { this._data.Level = value; }
        }

        /// <summary>
        /// An alternate message to use in the validation results. This value can be
        /// a composite format string to provide for more meaningful validation feedback.
        /// Reference the object represented by the ValidatorType for the available
        /// objects/indexes that can be used in the composite format string.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting?view=netframework-4.7.2#composite-format-string"/>
        /// </summary>
        public string Message
        {
            get { return this._data.Message; }
            set { this._data.Message = value; }
        }

        /// <summary>
        /// A string of key/value pairs of the format "key1=value1;key2=value2;key3=value3"
        /// that is converted to a collection of options of type Dictionary<string,string>
        /// for use by the IValidationRule<T>.
        /// </summary>
        public string Options
        {
            get { return this._data.Options; }
            set { this._data.Options = value; }
        }

#region Overrides of Attribute

        /// <summary>
        /// Indicates whether the value of this instance is the default value for the
        /// derived class.
        /// </summary>
        public override bool IsDefaultAttribute()
        {
            return this._ruleType == null;
        }

#endregion

        /// <summary>
        /// Apply the ValidationRule to the subject returning any validation results.
        /// </summary>
        /// <param name="subject">The subject of the validation.</param>
        /// <returns>A IValidationResult that details the results of the operation.</returns>
        public IValidationResult Validate( object subject )
        {
            if ( this._ruleType != null )
            {
                try
                {
                    object x = Activator.CreateInstance( this._ruleType, this._data );

                    MethodInfo method = this._ruleType.GetMethod( "Validate" );

                    return (IValidationResult)method.Invoke( x, new[] { subject } );
                }
                catch ( Exception e ) 
                {
                    throw new InvalidOperationException(
                        $"There was an error with ValidationRule {this._ruleType.FullName}. Does it implement {typeof( IValidationRule<> ).Name}?",
                        e );
                }
            }

            /// If a validator wasn't provided then forward validation to the subject itself.
            return subject.Validate();
        }
    }
}