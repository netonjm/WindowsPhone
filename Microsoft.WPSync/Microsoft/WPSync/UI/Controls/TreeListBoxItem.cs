namespace Microsoft.WPSync.UI.Controls
{
    using Microsoft.WPSync.UI.Utilities;
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;

    [TemplatePart(Name="PART_Header", Type=typeof(ToggleButton))]
    public class TreeListBoxItem : ListBoxItem
    {
        private PropertyInfo childCollectionProperty;
        private IEnumerable childrenList;
        private INotifyCollectionChanged childrenNotificationList;
        private TreeListBoxInfo currentInfo;
        public static readonly DependencyProperty HasItemsProperty;
        private const double IndentSize = 19.0;
        public static readonly DependencyProperty IsExpandedProperty;
        private bool isInitializing;
        private int lastChildItemCreatedIndex;
        private int level;
        private NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler;
        private readonly TreeListBox treeListBox;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static TreeListBoxItem()
        {
            IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(TreeListBoxItem), new UIPropertyMetadata(false, delegate (DependencyObject sender, DependencyPropertyChangedEventArgs e) {
                TreeListBoxItem item = (TreeListBoxItem) sender;
                if (!item.isInitializing)
                {
                    item.ExpandCollapseItem();
                }
            }));
            HasItemsProperty = DependencyProperty.Register("HasItems", typeof(bool), typeof(TreeListBoxItem), new UIPropertyMetadata(false, delegate (DependencyObject sender, DependencyPropertyChangedEventArgs e) {
                TreeListBoxItem item = (TreeListBoxItem) sender;
                if (!item.isInitializing)
                {
                    item.ExpandCollapseItem();
                }
            }));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListBoxItem), new FrameworkPropertyMetadata(typeof(TreeListBoxItem)));
        }

        public TreeListBoxItem(TreeListBox parent)
        {
            RoutedEventHandler handler = null;
            this.lastChildItemCreatedIndex = 1;
            this.isInitializing = true;
            this.treeListBox = parent;
            if (handler == null)
            {
                handler = delegate {
                    if (!this.currentInfo.IsExpanded)
                    {
                        this.UnRegisterCollectionNotification();
                    }
                };
            }
            base.Unloaded += handler;
            base.PreviewKeyDown += new KeyEventHandler(this.TreeListBoxItem_PreviewKeyDown);
        }

        private bool DeleteSingleItem(TreeListBoxInfo info)
        {
            int index = this.treeListBox.CompositeChildCollection.IndexOf(info);
            if (index == -1)
            {
                return false;
            }
            for (int i = 0; i < info.DescendentCount; i++)
            {
                this.treeListBox.CompositeChildCollection[index].IsExpanded = false;
                this.treeListBox.CompositeChildCollection.RemoveAt(index);
            }
            this.treeListBox.CompositeChildCollection[index].IsExpanded = false;
            this.treeListBox.CompositeChildCollection.RemoveAt(index);
            this.currentInfo.ChildItems.Remove(info);
            this.lastChildItemCreatedIndex--;
            return true;
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId="ChildItemSourcePath"), SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId="element"), SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId="IEnumerable")]
        private bool DoesDataItemHaveChildren()
        {
            if (this.childrenList != null)
            {
                IEnumerator enumerator = this.childrenList.GetEnumerator();
               
                    while (enumerator.MoveNext())
                    {
                        object current = enumerator.Current;
                        return true;
                    }
               
            }
            return false;
        }

        internal void DropAllListItems()
        {
            if (this.currentInfo.ChildItems != null)
            {
                for (int i = 0; i < this.currentInfo.ChildItems.Count; i++)
                {
                    if (this.DeleteSingleItem(this.currentInfo.ChildItems[i]))
                    {
                        i--;
                    }
                }
                this.UnRegisterCollectionNotification();
                this.lastChildItemCreatedIndex = 1;
            }
        }

        private void ExpandCollapseItem()
        {
            if (this.currentInfo.IsExpanded != this.IsExpanded)
            {
                this.currentInfo.IsExpanded = this.IsExpanded;
                if (this.IsExpanded)
                {
                    this.RegisterCollectionNotification();
                    this.GenerateAllListItems();
                }
                else
                {
                    this.UnRegisterCollectionNotification();
                    this.DropAllListItems();
                }
            }
        }

        [Conditional("UNIT_TESTS")]
        public void ExposePrepareItem(TreeListBoxInfo info)
        {
            this.PrepareItem(info);
        }

        private void GenerateAllListItems()
        {
            IEnumerable childList = this.GetChildList();
            if (childList != null)
            {
                foreach (object obj2 in childList)
                {
                    this.GenerateSingleItem(obj2);
                }
            }
        }

        private void GenerateSingleItem(object item)
        {
            int index = this.treeListBox.CompositeChildCollection.IndexOf(this.currentInfo);
            if (index != -1)
            {
                TreeListBoxInfo info = new TreeListBoxInfo(this.level + 1, item);
                this.currentInfo.ChildItems.Add(info);
                this.treeListBox.CompositeChildCollection.Insert(index + this.lastChildItemCreatedIndex, info);
                this.lastChildItemCreatedIndex++;
            }
        }

        private IEnumerable GetChildList()
        {
            return this.childrenList;
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId="ChildItemSourcePath"), SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId="IEnumerable")]
        private void InitChildListeners()
        {
            this.childrenList = null;
            this.childrenNotificationList = null;
            if (!string.IsNullOrEmpty(this.treeListBox.ChildItemSourcePath))
            {
                if (this.childCollectionProperty == null)
                {
                    this.childCollectionProperty = ReflectionHelper.GetPropertyForObject(this.currentInfo.DataItem, this.treeListBox.ChildItemSourcePath);
                    if (this.childCollectionProperty == null)
                    {
                        throw new InvalidOperationException("The ChildItemSourcePath specified is invalid");
                    }
                    if (!ReflectionHelper.InterfacePresentInType(this.childCollectionProperty.PropertyType, typeof(IEnumerable)))
                    {
                        throw new InvalidOperationException("The ChildItemSourcePath must be of type IEnumerable");
                    }
                }
                object obj2 = this.childCollectionProperty.GetValue(this.currentInfo.DataItem, null);
                if (obj2 != null)
                {
                    this.childrenList = obj2 as IEnumerable;
                    this.childrenNotificationList = obj2 as INotifyCollectionChanged;
                    if ((this.childrenNotificationList != null) && (this.notifyCollectionChangedEventHandler == null))
                    {
                        this.notifyCollectionChangedEventHandler = new NotifyCollectionChangedEventHandler(this.OnChildCollectionChanged);
                    }
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ToggleButton templateChild = base.GetTemplateChild("PART_Header") as ToggleButton;
            if (templateChild != null)
            {
                Binding binding = new Binding("IsExpanded") {
                    Source = this,
                    Mode = BindingMode.TwoWay
                };
                templateChild.SetBinding(ToggleButton.IsCheckedProperty, binding);
                templateChild.Margin = new Thickness(this.level * 19.0, 0.0, 0.0, 0.0);
            }
        }

        private void OnChildCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.HasItems = this.DoesDataItemHaveChildren();
            lock (this.treeListBox.CompositeChildCollection)
            {
                if (!this.currentInfo.IsExpanded)
                {
                    this.UnRegisterCollectionNotification();
                }
                else
                {
                    int index = this.treeListBox.CompositeChildCollection.IndexOf(this.currentInfo);
                    if (index < 0)
                    {
                        this.UnRegisterCollectionNotification();
                    }
                    else
                    {
                        switch (e.Action)
                        {
                            case NotifyCollectionChangedAction.Add:
                                if (e.NewItems != null)
                                {
                                    int num2 = 0;
                                    foreach (object obj2 in e.NewItems)
                                    {
                                        int num3 = Math.Min(e.NewStartingIndex + num2, this.currentInfo.ChildItems.Count);
                                        int num4 = 1;
                                        for (int i = num3 - 1; i >= 0; i--)
                                        {
                                            num4 += this.currentInfo.ChildItems[i].DescendentCount + 1;
                                        }
                                        int num6 = num4 + index;
                                        TreeListBoxInfo item = new TreeListBoxInfo(this.level + 1, obj2);
                                        this.currentInfo.ChildItems.Insert(Math.Min(e.NewStartingIndex + num2, this.currentInfo.ChildItems.Count), item);
                                        this.treeListBox.CompositeChildCollection.Insert(num6, item);
                                        num2++;
                                    }
                                }
                                goto Label_01D2;

                            case NotifyCollectionChangedAction.Remove:
                                goto Label_0182;

                            case NotifyCollectionChangedAction.Replace:
                            case NotifyCollectionChangedAction.Move:
                            case NotifyCollectionChangedAction.Reset:
                                this.treeListBox.RecalculateList();
                                goto Label_01D2;
                        }
                    }
                }
                goto Label_01D2;
            Label_0182:
                if (this.currentInfo.ChildItems.Count > e.OldStartingIndex)
                {
                    this.DeleteSingleItem(this.currentInfo.ChildItems[e.OldStartingIndex]);
                }
            Label_01D2:;
            }
        }

        internal virtual void PrepareItem(TreeListBoxInfo info)
        {
            this.currentInfo = info;
            this.level = info.Level;
            if (info.IsExpanded)
            {
                this.IsExpanded = true;
            }
            this.isInitializing = false;
            this.InitChildListeners();
            this.HasItems = this.DoesDataItemHaveChildren();
            if (info.IsExpanded)
            {
                this.RegisterCollectionNotification();
            }
            if (this.HasItems && !info.CreatedDuringReset)
            {
                this.treeListBox.RecalculateList();
            }
        }

        private void RegisterCollectionNotification()
        {
            if ((this.notifyCollectionChangedEventHandler != null) && (this.childrenNotificationList != null))
            {
                this.childrenNotificationList.CollectionChanged += this.notifyCollectionChangedEventHandler;
            }
        }

        private void TreeListBoxItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((sender == this) && (e != null))
            {
                switch (e.Key)
                {
                    case Key.Left:
                    case Key.Subtract:
                        this.IsExpanded = false;
                        e.Handled = true;
                        return;

                    case Key.Up:
                    case Key.Separator:
                        return;

                    case Key.Right:
                    case Key.Add:
                        this.IsExpanded = true;
                        e.Handled = true;
                        return;
                }
            }
        }

        private void UnRegisterCollectionNotification()
        {
            if ((this.notifyCollectionChangedEventHandler != null) && (this.childrenNotificationList != null))
            {
                this.childrenNotificationList.CollectionChanged -= this.notifyCollectionChangedEventHandler;
            }
        }

        public bool HasItems
        {
            get
            {
                return (bool) base.GetValue(HasItemsProperty);
            }
            set
            {
                base.SetValue(HasItemsProperty, value);
            }
        }

        public bool IsExpanded
        {
            get
            {
                return (bool) base.GetValue(IsExpandedProperty);
            }
            set
            {
                base.SetValue(IsExpandedProperty, value);
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

