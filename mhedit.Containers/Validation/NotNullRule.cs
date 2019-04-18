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
    public class NotNullRule : ValidationRule<object>
    {
        public NotNullRule( ValidationData data )
            : base( data )
        {}

#region Overrides of ValidationRule<object>

        public override IValidationResult Validate( object subject )
        {
            return subject == null ?
                       /// subject will always be null! What to pass to this function?
                       this.CreateResult( new object(), "Object is null." ) :
                       null;
        }

#endregion
    }
}
