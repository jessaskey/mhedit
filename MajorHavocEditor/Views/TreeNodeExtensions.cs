using System;
using System.Linq;
using System.Windows.Forms;

namespace MajorHavocEditor.Views
{

    public static class TreeNodeExtensions
    {
        /// <summary>
        /// BUG: Only sorts 2 levels of hierarchy!!
        /// 
        /// This is enough to look through 2 levels of node hierarchy which is sufficient
        /// for now.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TreeNode FindNodeOrDefault( this TreeView tree,
            Func<TreeNode, bool> predicate )
        {
            var result = tree.Nodes.Cast<TreeNode>()
                             .FirstOrDefault( predicate );

            return result ?? tree.Nodes.Cast<TreeNode>()
                                 .SelectMany( n => n.Nodes.Cast<TreeNode>() )
                                 .FirstOrDefault( predicate );
        }

        /// <summary>
        /// BUG: Only sorts 2 levels of hierarchy!!
        /// 
        /// This is enough to look through 2 levels of node hierarchy which is sufficient
        /// for now.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static TreeNode FindNodeOrDefault( this TreeView tree, object itemId )
        {
            return tree.FindNodeOrDefault( n => n.Tag.Equals( itemId ) );
        }
    }

}