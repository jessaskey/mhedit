using System.Collections.Generic;
using System.Text;

namespace mhedit.Containers.Validation
{

    public class ValidationResults : List<IValidationResult>, IValidationResult
    {
        private ValidationLevel _level = ValidationLevel.Message;
        private object _context;

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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach ( IValidationResult validationResult in this )
            {
                sb.AppendLine( validationResult.ToString() );
            }

            return sb.ToString();
        }
    }

}