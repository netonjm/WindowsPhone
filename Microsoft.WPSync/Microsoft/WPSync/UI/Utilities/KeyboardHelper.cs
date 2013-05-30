namespace Microsoft.WPSync.UI.Utilities
{
    using Microsoft.WPSync.UI.Controls;
    using Microsoft.WPSync.UI.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    public static class KeyboardHelper
    {
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void DataGridPreviewKeyDown(DataGrid sender, KeyEventArgs e)
        {
            Func<DeviceItemViewModel, bool> predicate = null;
            if (((sender != null) && (e != null)) && (e.Key == Key.Space))
            {
                if (sender.SelectedItems.Count > 0)
                {
                    if (predicate == null)
                    {
                        predicate = item => sender.SelectedItems.Contains(item);
                    }
                    bool flag = !sender.Items.Cast<DeviceItemViewModel>().First<DeviceItemViewModel>(predicate).IsChecked;
                    foreach (DeviceItemViewModel model2 in sender.SelectedItems)
                    {
                        model2.IsChecked = flag;
                    }
                }
                e.Handled = true;
            }
        }

        public static void ListBoxPreviewKeyDown(ListBox sender, KeyEventArgs e)
        {
            if (((sender != null) && (e != null)) && (e.Key == Key.Space))
            {
                if (sender.SelectedItems.Count > 0)
                {
                    HashSet<SelectableOptionViewModel> selectedItemsSet = new HashSet<SelectableOptionViewModel>(sender.SelectedItems.Cast<SelectableOptionViewModel>());
                    SelectableOptionViewModel model = sender.Items.Cast<SelectableOptionViewModel>().First<SelectableOptionViewModel>(item => selectedItemsSet.Contains(item));
                    bool flag = model.IsSelected.HasValue ? !model.IsSelected.Value : false;
                    foreach (SelectableOptionViewModel model2 in sender.SelectedItems)
                    {
                        model2.IsSelected = new bool?(flag);
                    }
                }
                e.Handled = true;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void PopupPreviewKeyDown(Popup sender, KeyEventArgs e)
        {
            if (((sender != null) && (e != null)) && (e.Key == Key.Escape))
            {
                sender.IsOpen = false;
                e.Handled = true;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void TreeListBoxPreviewKeyDown(TreeListBox sender, KeyEventArgs e)
        {
            if (((sender != null) && (e != null)) && (e.Key == Key.Space))
            {
                TreeListBoxInfo selectedItem = sender.SelectedItem as TreeListBoxInfo;
                if (selectedItem != null)
                {
                    SelectableOptionViewModel dataItem = selectedItem.DataItem as SelectableOptionViewModel;
                    dataItem.IsSelected = new bool?(dataItem.IsSelected.HasValue ? !dataItem.IsSelected.Value : false);
                }
                e.Handled = true;
            }
        }
    }
}

