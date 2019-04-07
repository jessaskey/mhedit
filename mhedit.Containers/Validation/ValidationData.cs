namespace mhedit.Containers.Validation
{
    /// <summary>
    /// Class that contains various user data that helps to define the results
    /// and behavior of a ValidationRule.
    /// </summary>
    public class ValidationData
    {
        public ValidationLevel Level = ValidationLevel.Error;
        public string Message = string.Empty;
        public string Options = string.Empty;
    }

}