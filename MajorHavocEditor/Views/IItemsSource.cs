using System.Collections;

namespace MajorHavocEditor.Views
{

    /// <summary>
    /// Public facing type for the TreeView.ItemsSource
    /// </summary>
    public interface IItemsSource : IList
    {
        IItemsSourceDelegate ItemsDelegate { get; set; }
    }

}