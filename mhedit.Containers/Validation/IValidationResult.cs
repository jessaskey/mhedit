namespace mhedit.Containers.Validation
{

    /// <summary>
    /// An object returned if there was a Validation outcome.
    /// </summary>
    public interface IValidationResult
    {
        /// <summary>
        /// The validation result level.
        /// </summary>
        ValidationLevel Level { get; }

        /// <summary>
        /// Returns the object that was the subject of the validation
        /// </summary>
        object Context { get; }
    }

}