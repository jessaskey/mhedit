using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace mhedit.Containers
{

    public interface ITreeObject
    {
        TreeNode TreeRender(TreeView treeView, TreeNode parentNode);
        ContextMenu GetTreeContextMenu();
        void SetGridlines(bool gridlines);
    }
}
