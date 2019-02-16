using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace mhedit
{

    public interface ITreeObject
    {
        TreeNode TreeRender(TreeView treeView, TreeNode parentNode, bool gridLines);
        void SetGridlines(bool gridlines);
    }
}
