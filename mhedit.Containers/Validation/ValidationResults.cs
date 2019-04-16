using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace mhedit.Containers.Validation
{

    public class ValidationResults : IEnumerable<IValidationResult>, IValidationResult
    {
        private List<IValidationResult> _results = new List<IValidationResult>();
        private ValidationLevel _level = ValidationLevel.NoResults;
        private object _context;

#region Implementation of IValidationResult

        public ValidationLevel Level
        {
            get { return this._level; }
        }

        public object Context
        {
            get { return this._context; }
            set { this._context = value; }
        }

#endregion

#region Implementation of IEnumerable

        public IEnumerator<IValidationResult> GetEnumerator()
        {
            return this._results.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

#endregion

        public void Add( IValidationResult item )
        {
            if ( item != null && item.Level > ValidationLevel.NoResults )
            {
                if ( this._level < item.Level )
                {
                    this._level = item.Level;
                }

                this._results.Add( item );
            }
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