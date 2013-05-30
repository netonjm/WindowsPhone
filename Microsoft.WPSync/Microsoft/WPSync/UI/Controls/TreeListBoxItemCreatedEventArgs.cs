namespace Microsoft.WPSync.UI.Controls
{
    using System;

    public class TreeListBoxItemCreatedEventArgs : EventArgs
    {
        private TreeListBoxItem newItem;
        private TreeListBoxInfo newItemInfo;

        public TreeListBoxItemCreatedEventArgs(TreeListBoxItem item, TreeListBoxInfo info)
        {
            this.newItem = item;
            this.newItemInfo = info;
        }

        public TreeListBoxItem NewItem
        {
            get
            {
                return this.newItem;
            }
        }

        public TreeListBoxInfo NewItemInfo
        {
            get
            {
                return this.newItemInfo;
            }
        }
    }
}

