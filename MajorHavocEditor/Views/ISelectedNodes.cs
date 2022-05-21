using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MajorHavocEditor.Views
{

    /// <summary>
    /// A delegate that provides a search algo for the ISelectedNodes
    /// implementation.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public delegate bool TryFindNode( object key, out TreeNode found );

    /// <summary>
    /// Defines the interface between user code that needs a SelectedItems
    /// collection and MultiselectTreeView which requires SelectedNodes.  
    /// </summary>
    internal interface ISelectedNodes : IEnumerable<TreeNode>
    {
        /// <summary>
        /// The number of TreeNodes in the Selected collection.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Add a TreeNode to Selected Nodes collection.
        /// </summary>
        /// <param name="node"></param>
        void Add( TreeNode node );

        /// <summary>
        /// Remove all TreeNodes from the collection.
        /// </summary>
        void Clear();

        /// <summary>
        /// Returns true if the TreeNode exists in the collection. False otherwise.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        bool Contains( TreeNode node );

        /// <summary>
        /// Remove the first occurrence of an object from the collection
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        bool Remove( TreeNode node );

        /// <summary>
        /// Exposes the underlying selected object collection, not the nodes that
        /// contain them in the tree.
        /// </summary>
        IList SelectedItems { get; }
    }

}