using System;
using System.Text.RegularExpressions;

namespace mhedit.Containers.Validation
{

    /// <summary>
    /// Tests that a string only includes 0-9 and aA-zZ and spaces using the default
    /// Regex pattern,"^[a-zA-Z0-9 ]*$".
    ///
    /// Options:
    /// "Pattern" to override the Regex pattern.
    /// "RegexOptions" to set the RegexOptions flags. Set as an integer value.
    ///     E.g. "RegexOptions=3" 
    /// 
    /// Composite format string:
    ///     Subject string is Index 0
    ///     Default Message is Index 1
    /// </summary>
    public class StringRegexRule : ValidationRule<string>
    {
        protected string _pattern;
        protected RegexOptions _regexOptions;

        public StringRegexRule( ValidationData data )
            : base( data )
        {
            this._pattern = this._options.ContainsKey( "Pattern" ) ?
                                this._options[ "Pattern" ] :
                                "^[a-zA-Z0-9 ]*$";

            this._regexOptions =
                this._options.ContainsKey( "RegexOptions" ) ?
                    (RegexOptions) Enum.Parse( typeof( RegexOptions ),
                        this._options[ "RegexOptions" ] ) :
                    RegexOptions.IgnoreCase;
        }

        public override IValidationResult Validate( string str )
        {
            IValidationResult result = null;

            if ( str != null
                 && !new Regex( this._pattern, this._regexOptions ).IsMatch( str ) )
            {
                string defaultMessage = $"\"{str}\" contains invalid characters.";

                result = new ValidationResult
                         {
                             Level = this._data.Level,
                             Context = str,
                             Message = string.IsNullOrWhiteSpace( this._data.Message ) ?
                                           defaultMessage :
                                           string.Format( this._data.Message,
                                               new object[]
                                               {
                                                   str,
                                                   defaultMessage
                                               } )
                         };
            }

            return result;
        }
    }

}