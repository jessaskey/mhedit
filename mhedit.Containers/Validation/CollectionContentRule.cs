using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace mhedit.Containers.Validation
{
    /// <summary>
    /// Validates that a collection has a fixed number of elements of a specified type.
    /// The "Expected" option specifies the expected size/count.
    ///
    /// Composite format string:
    ///     Expected is Index 0
    ///     Actual is Index 1
    ///     TContent.Name is Index 2
    /// </summary>
    public class CollectionContentRule<TContent> : ValidationRule<IEnumerable>
    {
        private readonly int _expectedCount;

        public CollectionContentRule( ValidationData data )
            : base( data )
        {
            this._expectedCount = this._options.ContainsKey( "Expected" ) ?
                                      int.Parse( this._options[ "Expected" ] ) :
                                      this._expectedCount;
        }

        public override IValidationResult Validate( IEnumerable collection )
        {
            List<TContent> actual = collection.OfType<TContent>().ToList();

            return
                actual.Count != this._expectedCount ?
                    new ValidationResult
                    {
                        Level = this._data.Level,
                        Context = collection,
                        Message = string.IsNullOrWhiteSpace( this._data.Message ) ?
                                      $"Expected {this._expectedCount} {typeof( TContent ).Name} objects, found {actual.Count}." :
                                      string.Format( this._data.Message,
                                          new object[]
                                          {
                                              this._expectedCount,
                                              actual.Count,
                                              typeof( TContent ).Name
                                          } )
                    } :
                    null;
        }
    }

}