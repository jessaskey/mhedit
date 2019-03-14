using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace mhedit.Containers.Validation
{

    public abstract class ValidationRule<T> : IValidationRule<T>
    {
        protected readonly ValidationData _data;
        protected readonly Dictionary<string, string> _options;

        protected ValidationRule( ValidationData data )
        {
            this._data = data;

            this._options = Regex.Matches( data.Options, @"\s*(.*?)\s*=\s*(.*?)\s*(;|$)" )
                                 .OfType<Match>()
                                 .ToDictionary( m => m.Groups[ 1 ].Value, m => m.Groups[ 2 ].Value );
        }

        public abstract IValidationResult Validate( T input );
    }

}