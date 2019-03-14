using System;

namespace mhedit.Containers.Validation
{

    public class ValidationData
    {
        public ValidationLevel Level = ValidationLevel.Error;
        public string Message = string.Empty;
        public string Options = string.Empty;
    }

    [AttributeUsage( System.AttributeTargets.Property, AllowMultiple = true ) ]
    public class ValidationAttribute : Attribute
    {
        //public readonly Guid G = Guid.NewGuid();

        private readonly Type _validatorType;
        private readonly ValidationData _data = new ValidationData();

        public ValidationAttribute( Type validatorType )
        {
            this._validatorType = validatorType;
        }

        public ValidationLevel Level
        {
            get { return this._data.Level; }
            set { this._data.Level = value; }
        }

        public string Message
        {
            get { return this._data.Message; }
            set { this._data.Message = value; }
        }

        public string Options
        {
            get { return this._data.Options; }
            set { this._data.Options = value; }
        }

        public IValidationResult Validate( object input )
        {
            object x = Activator.CreateInstance( this._validatorType, this._data );

            var method = this._validatorType.GetMethod( "Validate" );

            return (IValidationResult) method.Invoke( x, new[] { input } );
        }
    }

}