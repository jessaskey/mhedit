using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace mhedit.Containers.Validation
{
    public class StringValidator : ValidationRule<string>
    {
        public StringValidator( ValidationData data )
            : base( data )
        {
            //this._regexOptions =
            //    this._options.ContainsKey( "RegexOptions" ) ?
            //        (RegexOptions)Enum.Parse( typeof( RegexOptions ),
            //            this._options[ "RegexOptions" ] ) :
            //        RegexOptions.IgnoreCase;
        }

        public override IValidationResult Validate( string input )
        {
            return
                string.IsNullOrWhiteSpace( input ) ?
                    new ValidationResult
                    {
                        Level = this._data.Level,
                        Context = input,
                        Message = string.IsNullOrWhiteSpace( this._data.Message ) ?
                                      "String is null, empty, or whitespace." :
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