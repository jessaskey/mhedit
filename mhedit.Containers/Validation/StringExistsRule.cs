namespace mhedit.Containers.Validation
{
    /// <summary>
    /// Simply tests a string value for IsNullOrWhiteSpace.
    ///
    /// Doesn't support Composite format string
    /// </summary>
    public class StringExistsRule : ValidationRule<string>
    {
        public StringExistsRule( ValidationData data )
            : base( data )
        {}

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
                                      this._data.Message
                    } :
                    null;
        }

    }

}