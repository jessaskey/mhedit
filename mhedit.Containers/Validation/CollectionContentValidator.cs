using System.Collections.Generic;
using System.Linq;

namespace mhedit.Containers.Validation
{

    public class CollectionContentValidator<TContent, TElement> :
        ValidationRule<ICollection<TElement>>
    {
        private readonly int _expectedCount;

        public CollectionContentValidator( ValidationData data )
            : base( data )
        {
            this._expectedCount = int.Parse( this._options[ "Count" ] );
        }

        public override IValidationResult Validate( ICollection<TElement> collection )
        {
            IEnumerable<TElement> actual = collection.Where( o => o is TContent ).ToList();

            return
                actual.Count() != this._expectedCount ?
                    new ValidationResult
                    {
                        Level = this._data.Level,
                        Context = collection,
                        Message = string.IsNullOrWhiteSpace( this._data.Message ) ?
                                      $"Expected {this._expectedCount} {typeof( TContent ).Name} objects, found {actual.Count()}." :
                                      string.Format( this._data.Message,
                                          new object[]
                                          {
                                              actual.Count(),
                                              this._expectedCount,
                                              typeof( TContent ).Name
                                          } )
                    } :
                    null;
        }
    }

}