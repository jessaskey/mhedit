using System.Collections;

namespace mhedit.Containers.Validation
{
    /// <summary>
    /// Validates that a collection contains an elements of type TContent at index
    /// "Index". If the element should be at the end of the collection set Index=-1.
    /// Assumes that an empty collection is an error.
    ///
    /// ValidationAttribute.Options
    ///     Index (Required)
    ///
    /// Composite format string:
    ///     Collection object is Index 0
    ///     Default Message is Index 1
    ///     Index is Index 2
    ///     Actual Index is Index 3
    ///     TContent.Name is Index 5
    /// </summary>
    public class CollectionIndexRule<TContent> : ValidationRule<ICollection>
    {
        private int _expectedIndex;

        public CollectionIndexRule( ValidationData data )
            : base( data )
        {
            this._expectedIndex = int.Parse( this._options[ "Index" ] );
        }

#region Overrides of ValidationRule<ICollection>

        public override IValidationResult Validate( ICollection collection )
        {
            return collection.Count == 0 ?
                       this.CreateResult( collection,
                           $"Expected {typeof( TContent ).Name} object but collection was empty.",
                           this._expectedIndex,
                           -1,
                           typeof( TContent ).Name ) :
                       this.TestForContent( collection );
        }

#endregion

        private IValidationResult TestForContent( ICollection collection )
        {
            int actualIndex = 0;

            foreach ( object obj in collection )
            {
                if ( obj.GetType() == typeof( TContent ) )
                {
                    break;
                }

                actualIndex++;
            }

            /// if -1 then choose collection size.
            this._expectedIndex = this._expectedIndex == -1 ?
                                      collection.Count - 1 : this._expectedIndex;

            return actualIndex != this._expectedIndex ?
                       this.CreateResult( collection,
                           $"Expected {typeof( TContent ).Name} object at index {this._expectedIndex}, was found at {actualIndex}.",
                           this._expectedIndex,
                           actualIndex,
                           typeof( TContent ).Name ) :
                       null;
        }
    }

}