using System;
using System.Reflection;

namespace mhedit.Containers.Validation
{

    /// <summary>
    /// Validates that the value of the named property (which is of type T) on the
    /// Subject is within the range of a supplied Minimum and Maximum inclusive.
    ///
    /// ValidationAttribute.Options (Required)
    ///     PropertyName
    ///     Maximum
    ///     Minimum (If Maximum = Minimum then the exact count is expected)
    /// 
    /// Composite format string:
    ///     Value is Index 0
    ///     Default Message is Index 1
    ///     PropertyName is Index 2
    ///     Minimum is Index 3
    ///     Maximum is Index 4
    /// </summary>
    public class NamedPropertyRule<T> : ValidationRule<object> where T : IComparable<T>
    {
        private readonly string _propertyName;
        private readonly Range<T> _range = new Range<T>();

        public NamedPropertyRule( ValidationData data )
            : base( data )
        {
            this._propertyName = this._options[ "PropertyName" ];

            this._range.Maximum = (T) Convert.ChangeType( this._options[ "Maximum" ], typeof( T ) );

            this._range.Minimum = (T) Convert.ChangeType( this._options[ "Minimum" ], typeof( T ) );
        }

#region Overrides of ValidationRule<object>

        public override IValidationResult Validate( object subject )
        {
            PropertyInfo property = subject.GetType().GetProperty( this._propertyName,
                BindingFlags.Public | BindingFlags.Instance );

            object propertyValue = property.GetValue( subject );

            return !this._range.ContainsValue( (T) propertyValue ) ?
                       this.CreateResult( propertyValue,
                           $"Invalid {this._propertyName} value: {propertyValue}, expected {this._range.Minimum} <= value <= {this._range.Maximum}.",
                           this._propertyName,
                           this._range.Minimum,
                           this._range.Maximum ) :
                       null;
        }

#endregion
    }

}