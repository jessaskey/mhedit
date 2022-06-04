using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MajorHavocEditor.Views
{

    public static class TreeNodeExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static TreeNode FindNodeOrDefault(this TreeView tree, object itemId)
        {
            return tree.Nodes
                       .Descendants()
                       .FirstOrDefault(n => n.Tag.Equals(itemId));
        }

        /// <summary>
        /// https://stackoverflow.com/a/7063002
        /// </summary>
        /// <param name="root"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        static IEnumerable<TreeNode> Descendants(this TreeNodeCollection root)
        {
            var nodes = new Queue<TreeNode>( root.Cast<TreeNode>() );
            while (nodes.Any())
            {
                TreeNode node = nodes.Dequeue();
                yield return node;
                foreach (TreeNode n in node.Nodes) nodes.Enqueue(n);
            }
        }
    }

}