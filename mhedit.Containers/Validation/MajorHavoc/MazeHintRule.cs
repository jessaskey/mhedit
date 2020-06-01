using System.Text.RegularExpressions;

namespace mhedit.Containers.Validation.MajorHavoc
{
    /// <summary>
    /// Major Havoc Maze Hints only support a specific set of characters and must be
    /// less than 128 characters long.
    /// " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ..!-,%:"
    ///
    /// ValidationAttribute.Options is not supported (Ignored).
    /// 
    /// Composite format string:
    ///     Maze Hint string is Index 0
    ///     Default Message is Index 1
    /// </summary>
    public class MazeHintRule : ValidationRule<string>
    {
        private const int MaxLength = 128;
        private static readonly Regex HavocRegex =
            new Regex( $"^[A-Z0-9 .!\\-,%:?csho@]{{0,{MaxLength}}}$" );

        public MazeHintRule( ValidationData data )
            : base( data )
        {}

        public override IValidationResult Validate( string str )
        {
            return str != null && !HavocRegex.IsMatch( str ) ?
                       this.CreateResult( str,
                           $"\"{str}\", contains invalid characters or is > {MaxLength} characters long. " +
                           "Valid characters are \" 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ..!-,%:?\"" ) :
                       null;
        }
    }

}
