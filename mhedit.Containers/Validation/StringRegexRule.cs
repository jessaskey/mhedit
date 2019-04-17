using System;
using System.Text.RegularExpressions;

namespace mhedit.Containers.Validation
{

    /// <summary>
    /// Tests a string using a Regular Expression provided via the Pattern Option
    /// which is Required. By default the Regex operation is executed with
    /// RegexOptions.IgnoreCase. Null strings are ignored (assumed to be valid).
    ///
    /// ValidationAttribute.Options:
    /// "Pattern" provides the Regex pattern.
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

        //private static readonly string Pattern = "^[a-zA-Z0-9 ]*$";

        public StringRegexRule( ValidationData data )
            : base( data )
        {
            this._pattern = this._options[ "Pattern" ];

            this._regexOptions =
                this._options.ContainsKey( "RegexOptions" ) ?
                    (RegexOptions)Enum.Parse( typeof( RegexOptions ),
                        this._options[ "RegexOptions" ] ) :
                    RegexOptions.IgnoreCase;
        }

        public override IValidationResult Validate( string str )
        {
            return str != null && !Regex.IsMatch( str, this._pattern, this._regexOptions ) ?
                       this.CreateResult( str, $"\"{str}\" contains invalid characters." ) :
                       null;
        }
    }

}