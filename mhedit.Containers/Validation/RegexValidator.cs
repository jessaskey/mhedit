using System;
using System.Text.RegularExpressions;

namespace mhedit.Containers.Validation
{
    //        protected string _validText = " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ..!-,%:";

    public class RegexValidator : ValidationRule<string>
    {
        private readonly string _pattern;
        private readonly RegexOptions _regexOptions;

        public RegexValidator( ValidationData data )
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

        public override IValidationResult Validate( string input )
        {
            return
                !new Regex( this._pattern, this._regexOptions ).IsMatch( input ) ?
                    new ValidationResult
                    {
                        Level = this._data.Level,
                        Context = input,
                        Message = string.IsNullOrWhiteSpace( this._data.Message ) ?
                                      $"\"{input}\" contains unsupported characters." :
                                      string.Format( this._data.Message,
                                          new object[]
                                          {
                                              input
                                          } )
                    } :
                    null;
        }

    }

}