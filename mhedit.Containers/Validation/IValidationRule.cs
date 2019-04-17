namespace mhedit.Containers.Validation
{

    /// <summary>
    /// The validation structure was based off of a pattern discussed here:
    /// https://softwareengineering.stackexchange.com/a/360439
    /// </summary>
    /// <typeparam name="T">The type this rule applies to.</typeparam>
    public interface IValidationRule<in T>
    {
        /// <summary>
        /// Apply this Rule to the subject.
        /// </summary>
        /// <param name="subject">The subject of the validation.</param>
        /// <returns>A IValidationResult that details the result of the operation.</returns>
        IValidationResult Validate( T subject );
    }
}
