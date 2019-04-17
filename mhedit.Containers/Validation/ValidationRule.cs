using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace mhedit.Containers.Validation
{
    /// <summary>
    /// A Base class implementation of IValidationRule for convenience. 
    /// </summary>
    public abstract class ValidationRule<T> : IValidationRule<T>
    {
        private static readonly Regex OptionsRegex = new Regex( @"\s*(.*?)\s*=\s*(.*?)\s*(;|$)" );

        protected readonly ValidationData _data;
        protected readonly Dictionary<string, string> _options;

        protected ValidationRule( ValidationData data )
        {
            this._data = data;

            /// https://stackoverflow.com/a/4141535
            this._options = OptionsRegex.Matches( data.Options )
                                 .OfType<Match>()
                                 .ToDictionary( m => m.Groups[ 1 ].Value, m => m.Groups[ 2 ].Value );
        }

        public abstract IValidationResult Validate( T subject );

        /// <summary>
        /// This method will create a ValidationResult object using the user
        /// supplied information in the ValidationData object.
        /// </summary>
        /// <param name="args">An object array used to construct an IValidationResult with
        /// the following constraints: Element 0 must be the subject of the
        /// IValidationRule.Validate method and Element 1 must be the Default User Message.</param>
        /// <returns>A IValidationResult object derived from the corresponding objects in <paramref name="args" />.</returns>
        protected virtual IValidationResult CreateResult( params object[] args )
        {
            return new ValidationResult
                   {
                       Level = this._data.Level,
                       Context = args[ 0 ],
                       Message = string.IsNullOrWhiteSpace( this._data.Message ) ?
                                     (string) args[ 1 ] :
                                     string.Format( this._data.Message, args )
                   };
        }
    }

}