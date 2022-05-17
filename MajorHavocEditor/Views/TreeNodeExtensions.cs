using System.Linq;
using System.Windows.Forms;

namespace MajorHavocEditor.Views
{

    public static class TreeNodeExtensions
    {
        /// <summary>
        /// This is enough to look through 2 levels of node hierarchy. 
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static TreeNode FindNodeOrDefault(this TreeView tree, object itemId)
        {
            var result = tree.Nodes.Cast<TreeNode>()
                             .FirstOrDefault(node => node.Tag.Equals(itemId));

            return result ?? tree.Nodes.Cast<TreeNode>()
                                 .SelectMany( n => n.Nodes.Cast<TreeNode>() )
                                 .FirstOrDefault(node => node.Tag.Equals(itemId));
        }
    }

}