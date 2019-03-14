using System;
using System.Collections;

namespace mhedit.Containers.Validation
{

    public class CollectionSizeValidator : ValidationRule<ICollection>
    {
        private readonly int _expectedCount;

        public CollectionSizeValidator( ValidationData data )
            : base( data )
        {
            this._expectedCount = Int32.Parse( this._options[ "Count" ] );
        }

        public override IValidationResult Validate( ICollection collection )
        {
            return
                collection.Count != this._expectedCount ?
                    new ValidationResult
                    {
                        Level = this._data.Level,
                        Context = collection,
                        Message = this._data.Message + Environment.NewLine +
                                  $"Collection size {collection.Count} was expected to be {this._expectedCount}."
                    } :
                    null;
        }
    }

}