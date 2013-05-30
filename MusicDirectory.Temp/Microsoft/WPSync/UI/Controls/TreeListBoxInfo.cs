namespace Microsoft.WPSync.UI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;

    public class TreeListBoxInfo
    {
        private List<TreeListBoxInfo> childItems;
        private object dataItem;
        private bool isExpanded;
        private int level;

        public TreeListBoxInfo(int level, object dataItem)
        {
            this.level = level;
            this.dataItem = dataItem;
            IExpandable expandable = dataItem as IExpandable;
            if (expandable != null)
            {
                this.isExpanded = expandable.IsExpanded;
            }
        }

        private static int GetDescendentCount(TreeListBoxInfo info)
        {
            if ((info.childItems == null) || (info.childItems.Count == 0))
            {
                return 0;
            }
            int num = 0;
            foreach (TreeListBoxInfo info2 in info.childItems)
            {
                num += GetDescendentCount(info2);
                num++;
            }
            return num;
        }

        public override string ToString()
        {
            return this.DataItem.ToString();
        }

        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public List<TreeListBoxInfo> ChildItems
        {
            get
            {
                if (this.childItems == null)
                {
                    this.childItems = new List<TreeListBoxInfo>();
                }
                return this.childItems;
            }
        }

        public bool CreatedDuringReset { get; set; }

        public object DataItem
        {
            get
            {
                return this.dataItem;
            }
        }

        public int DescendentCount
        {
            get
            {
                return GetDescendentCount(this);
            }
        }

        public bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }
            set
            {
                this.isExpanded = value;
                IExpandable dataItem = this.dataItem as IExpandable;
                if (dataItem != null)
                {
                    dataItem.IsExpanded = this.isExpanded;
                }
            }
        }

        public int Level
        {
            get
            {
                return this.level;
            }
        }
    }
}

