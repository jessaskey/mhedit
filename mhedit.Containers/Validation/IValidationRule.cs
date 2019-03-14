namespace mhedit.Containers.Validation
{

    /// <summary>
    /// The validation structure was based off of a pattern discussed here:
    /// https://softwareengineering.stackexchange.com/a/360439
    /// </summary>
    public interface IValidationRule<T>
    {
        IValidationResult Validate( T input );
    }
}
