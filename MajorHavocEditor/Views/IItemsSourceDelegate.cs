using System.Collections;
using System.Windows.Forms;

namespace MajorHavocEditor.Views
{

    /// <summary>
    /// Provides TreeNode implementation details for the objects contained
    /// in the TreeView's ItemsSource. 
    /// </summary>
    public interface IItemsSourceDelegate
    {
        /// <summary>
        /// Creates a TreeNode for the item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        TreeNode CreateNode( object item );

        /// <summary>
        /// ree.
        /// </summary>
        /// <param name="node"></param>
        void OnRemoveNode( TreeNode node );

        /// <summary>
        /// Determines if the TreeNode is the node containing the Item.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Equals( TreeNode node, object item );

        /// <summary>
        /// Returns an Enumerable for the children of the item, or null
        /// if the item has no children. 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        IEnumerable GetEnumerable( TreeNode item );
    }

}