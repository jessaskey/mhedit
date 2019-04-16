namespace mhedit.Containers.Validation
{

    public class ValidationResult : IValidationResult
    {
        private ValidationLevel _level;
        private string _message;
        private object _context;

#region Implementation of IValidationResult

        public ValidationLevel Level
        {
            get { return this._level; }
            set { this._level = value; }
        }

        public object Context
        {
            get { return this._context; }
            set { this._context = value; }
        }

        public string Message
        {
            get { return this._message; }
            set { this._message = value; }
        }

        public override string ToString()
        {
            return $"[{this._level}] {this._message}";
        }

#endregion
    }

}