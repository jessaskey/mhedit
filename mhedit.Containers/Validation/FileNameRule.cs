using System.Text.RegularExpressions;

namespace mhedit.Containers.Validation
{
    /// <summary>
    /// Windows file names only support a specific set of characters and must be
    /// less than 254 characters long.
    ///
    /// ValidationAttribute.Options is not supported (Ignored).
    /// 
    /// Composite format string:
    ///     Filename string is Index 0
    ///     Default Message is Index 1
    /// </summary>
    public class FileNameRule : ValidationRule<string>
    {
        private static readonly Regex FilenameRegex =
            new Regex( @"^[0-9a-zA-Z_\-. ]{1,254}$" );

        public FileNameRule( ValidationData data )
            : base( data )
        { }

        public override IValidationResult Validate( string str )
        {
            return str != null && !FilenameRegex.IsMatch( str ) ?
                       this.CreateResult( str,
                           $"Filename \"{str}\" contains invalid characters or is > 254 characters" ) :
                       null;
        }
    }

}