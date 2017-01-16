//----------------------------------------------------------------------------
// File    : Collections.cs
// Date    : 02/10/2005
// Author  : Aju George
// Email   : aju_george_2002@yahoo.co.in ; george.aju@gmail.com
// 
// Updates :
//           See ToolBox.cs
//
// Legal   : See ToolBox.cs
//----------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;

namespace Silver.UI
{
    #region ToolBoxTabCollection Class

    [Serializable]
    public class ToolBoxTabCollection : CollectionBase
    {
        #region Constructor
        public ToolBoxTabCollection()
        {
        }
        #endregion //Constructor

        #region Properties
        public ToolBoxTab this[int index]
        {
            get{return (ToolBoxTab)base.List[index];}
            set{base.List[index]=value;}
        }
        #endregion //Properties

        #region Public Methods
        public int Add(ToolBoxTab tab)
        {
            return base.InnerList.Add(tab);
        }

        public void Insert(int index, ToolBoxTab tab)
        {
            base.InnerList.Insert(index,tab);
        }

        public void Remove(ToolBoxTab tab)
        {
            base.InnerList.Remove(tab);
        }

        public int IndexOf(ToolBoxTab tab)
        {
            return base.InnerList.IndexOf(tab);
        }

        public bool Contains(ToolBoxTab tab)
        {
            return base.InnerList.Contains(tab);
        }

        #endregion //Public Methods
    }

    #endregion //ToolBoxTabCollection Class

    #region ToolBoxItemCollection Class

    [Serializable]
    public class ToolBoxItemCollection : CollectionBase
    {
        #region Constructor
        public ToolBoxItemCollection()
        {
        }
        #endregion //Constructor

        #region Properties
        public ToolBoxItem this[int index]
        {
            get{return (ToolBoxItem)base.List[index];}
            set{base.List[index]=value;}
        }
        #endregion //Properties

        #region Public Methods
        public int Add(ToolBoxItem item)
        {
            return base.InnerList.Add(item);
        }

        public void Insert(int index, ToolBoxItem item)
        {
            base.InnerList.Insert(index,item);
        }

        public void Remove(ToolBoxItem item)
        {
            base.InnerList.Remove(item);
        }

        public int IndexOf(ToolBoxItem item)
        {
            return base.InnerList.IndexOf(item);
        }

        public bool Contains(ToolBoxItem item)
        {
            return base.InnerList.Contains(item);
        }

        #endregion //Public Methods
    }

    #endregion //ToolBoxTabCollection Class

}
