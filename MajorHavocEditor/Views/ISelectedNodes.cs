using System.Collections.Generic;
using System.Windows.Forms;

namespace MajorHavocEditor.Views
{
    public delegate bool TryFindNode(object key, out TreeNode found);

    internal interface ISelectedNodes : IEnumerable<TreeNode>
    {
        int Count { get; }
        void Add(TreeNode node);
        void Clear();
        bool Contains(TreeNode node);
        bool Remove(TreeNode node);
    }

}