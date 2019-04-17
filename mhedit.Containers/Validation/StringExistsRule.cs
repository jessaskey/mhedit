namespace mhedit.Containers.Validation
{
    /// <summary>
    /// Simply tests a string value for IsNullOrWhiteSpace.
    ///
    /// ValidationAttribute.Options is not supported (Ignored).
    /// 
    /// Composite format string:
    ///     Subject string is Index 0
    ///     Default Message is Index 1
    /// </summary>
    public class StringExistsRule : ValidationRule<string>
    {
        public StringExistsRule( ValidationData data )
            : base( data )
        {}

        public override IValidationResult Validate( string str )
        {
            return string.IsNullOrWhiteSpace( str ) ?
                       this.CreateResult( str, "String is null, empty, or whitespace." ) :
                       null;
        }

    }

}