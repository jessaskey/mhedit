using System;
using System.Collections;

namespace mhedit.Containers.Validation
{
    /// <summary>
    /// Validates that a collection has a fixed number of elements. The "Expected" option
    /// specifies the expected.
    ///
    /// Composite format string:
    ///     Expected is Index 0
    ///     Actual is Index 1
    ///     CollectionType.Name is Index 2
    /// </summary>
    public class CollectionSizeRule : ValidationRule<ICollection>
    {
        private readonly int _expectedCount;

        public CollectionSizeRule( ValidationData data )
            : base( data )
        {
            this._expectedCount = this._options.ContainsKey( "Expected" ) ?
                                      int.Parse( this._options[ "Expected" ] ) :
                                      this._expectedCount;
        }

        public override IValidationResult Validate( ICollection collection )
        {
            return
                collection.Count != this._expectedCount ?
                    new ValidationResult
                    {
                        Level = this._data.Level,
                        Context = collection,
                        Message = string.IsNullOrWhiteSpace( this._data.Message ) ?
                                      $"Expected {this._expectedCount}, Actual {collection.Count}." :
                                      string.Format( this._data.Message,
                                          new object[]
                                          {
                                              this._expectedCount,
                                              collection.Count,
                                              collection.GetType().Name
                                          } )
                    } :
                    null;
        }
    }

}