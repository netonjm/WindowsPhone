namespace Microsoft.WPSync.UI.Controls
{
    using Microsoft.WPSync.Shared;
    using Microsoft.WPSync.UI.Utilities;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;

    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class TreeListBox : ListBox
    {
        public static readonly DependencyProperty ChildItemSourcePathProperty = DependencyProperty.Register("ChildItemSourcePath", typeof(string), typeof(TreeListBox), new UIPropertyMetadata(null));
        private ObservableCollection<TreeListBoxInfo> compositeChildCollection = new ObservableCollection<TreeListBoxInfo>();
        private IEnumerable currentDataSource;
        public static readonly DependencyProperty HierarchicalItemsSourceProperty;
        private int isResetting;
        private EventHandler<TreeListBoxItemCreatedEventArgs> _newItemCreated;
        private NotifyCollectionChangedEventHandler notificationEnableCollectionHandler;
        private BatchingTimer resetTimer;
        private List<TreeListBoxInfo> rootNodesInfo;

        public event EventHandler<TreeListBoxItemCreatedEventArgs> NewItemCreated
        {
            add
            {
                EventHandler<TreeListBoxItemCreatedEventArgs> handler2;
                EventHandler<TreeListBoxItemCreatedEventArgs> newItemCreated = this._newItemCreated;
                do
                {
                    handler2 = newItemCreated;
                    EventHandler<TreeListBoxItemCreatedEventArgs> handler3 = (EventHandler<TreeListBoxItemCreatedEventArgs>) Delegate.Combine(handler2, value);
                    newItemCreated = Interlocked.CompareExchange<EventHandler<TreeListBoxItemCreatedEventArgs>>(ref this._newItemCreated, handler3, handler2);
                }
                while (newItemCreated != handler2);
            }
            remove
            {
                EventHandler<TreeListBoxItemCreatedEventArgs> handler2;
                EventHandler<TreeListBoxItemCreatedEventArgs> newItemCreated = this._newItemCreated;
                do
                {
                    handler2 = newItemCreated;
                    EventHandler<TreeListBoxItemCreatedEventArgs> handler3 = (EventHandler<TreeListBoxItemCreatedEventArgs>) Delegate.Remove(handler2, value);
                    newItemCreated = Interlocked.CompareExchange<EventHandler<TreeListBoxItemCreatedEventArgs>>(ref this._newItemCreated, handler3, handler2);
                }
                while (newItemCreated != handler2);
            }
        }

        static TreeListBox()
        {
            HierarchicalItemsSourceProperty = DependencyProperty.Register("HierarchicalItemsSource", typeof(IEnumerable), typeof(TreeListBox), new UIPropertyMetadata(null, delegate (DependencyObject sender, DependencyPropertyChangedEventArgs e) {
                ((TreeListBox) sender).GenerateItemsSource(e.OldValue as IEnumerable, e.NewValue as IEnumerable);
            }));
        }

        public TreeListBox()
        {
            this.resetTimer = new BatchingTimer(new Action(this.ResetFlattenedListAction), 100);
        }

        private void AddChildHierarchy(TreeListBoxInfo parent, object item)
        {
            TreeListBoxInfo info = new TreeListBoxInfo(parent.Level + 1, item) {
                CreatedDuringReset = true
            };
            parent.ChildItems.Add(info);
            this.CompositeChildCollection.Add(info);
            if (info.IsExpanded)
            {
                this.AddDescendentsOnCreation(info);
            }
        }

        private void AddDescendentsOnCreation(TreeListBoxInfo parent)
        {
            IEnumerable dataChildList = this.GetDataChildList(parent.DataItem);
            if (dataChildList != null)
            {
                foreach (object obj2 in dataChildList)
                {
                    this.AddChildHierarchy(parent, obj2);
                }
            }
        }

        private void GenerateItemsSource(IEnumerable oldValue, IEnumerable newValue)
        {
            this.currentDataSource = newValue;
            this.ResetFlattenedList();
            if (this.notificationEnableCollectionHandler == null)
            {
                this.notificationEnableCollectionHandler = new NotifyCollectionChangedEventHandler(this.NotificationEnableCollectionCollectionChanged);
            }
            INotifyCollectionChanged changed = oldValue as INotifyCollectionChanged;
            if (changed != null)
            {
                changed.CollectionChanged -= this.notificationEnableCollectionHandler;
            }
            INotifyCollectionChanged changed2 = newValue as INotifyCollectionChanged;
            if (changed2 != null)
            {
                changed2.CollectionChanged += this.notificationEnableCollectionHandler;
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeListBoxItem(this);
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId="IEnumerable"), SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId="ChildItemSourcePath")]
        private IEnumerable GetDataChildList(object item)
        {
            IEnumerable enumerable = null;
            if (string.IsNullOrEmpty(this.ChildItemSourcePath))
            {
                return enumerable;
            }
            PropertyInfo propertyForObject = ReflectionHelper.GetPropertyForObject(item, this.ChildItemSourcePath);
            if (propertyForObject == null)
            {
                return null;
            }
            if (!ReflectionHelper.InterfacePresentInType(propertyForObject.PropertyType, typeof(IEnumerable)))
            {
                return null;
            }
            object obj2 = propertyForObject.GetValue(item, null);
            if (obj2 == null)
            {
                return null;
            }
            return (obj2 as IEnumerable);
        }

        private int GetFlattenedTreeDescendentCount(object item)
        {
            IEnumerable dataChildList = this.GetDataChildList(item);
            int num = 0;
            if (dataChildList != null)
            {
                foreach (object obj2 in dataChildList)
                {
                    num++;
                    IExpandable expandable = obj2 as IExpandable;
                    if ((expandable == null) || expandable.IsExpanded)
                    {
                        num += this.GetFlattenedTreeDescendentCount(obj2);
                    }
                }
            }
            return num;
        }

        private int GetFlattenedTreeItemCount()
        {
            if (this.currentDataSource == null)
            {
                return 0;
            }
            int num = 0;
            foreach (object obj2 in this.currentDataSource)
            {
                num++;
                IExpandable expandable = obj2 as IExpandable;
                if ((expandable == null) || expandable.IsExpanded)
                {
                    num += this.GetFlattenedTreeDescendentCount(obj2);
                }
            }
            return num;
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is TreeListBoxItem);
        }

        private void NotificationEnableCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Predicate<TreeListBoxInfo> match = null;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems == null)
                    {
                        return;
                    }
                    lock (this.CompositeChildCollection)
                    {
                        int num = 0;
                        foreach (object obj2 in e.NewItems)
                        {
                            int index = Math.Min(e.NewStartingIndex + num, this.rootNodesInfo.Count);
                            int num3 = 0;
                            for (int j = index - 1; j >= 0; j--)
                            {
                                num3 += this.rootNodesInfo[j].DescendentCount + 1;
                            }
                            index = num3;
                            TreeListBoxInfo item = new TreeListBoxInfo(0, obj2);
                            this.rootNodesInfo.Insert(Math.Min(e.NewStartingIndex + num, this.rootNodesInfo.Count), item);
                            this.compositeChildCollection.Insert(index, item);
                            num++;
                        }
                        return;
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    break;

                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                    goto Label_01E9;

                default:
                    throw new InvalidOperationException();
            }
            lock (this.CompositeChildCollection)
            {
                if (match == null)
                {
                    match = i => i.DataItem == e.OldItems[0];
                }
                TreeListBoxInfo info2 = this.rootNodesInfo.Find(match);
                if (info2 != null)
                {
                    int num5 = this.compositeChildCollection.IndexOf(info2);
                    if (num5 >= 0)
                    {
                        int descendentCount = info2.DescendentCount;
                        for (int k = 0; k < descendentCount; k++)
                        {
                            this.compositeChildCollection.RemoveAt(num5);
                        }
                        this.compositeChildCollection.RemoveAt(num5);
                        this.rootNodesInfo.Remove(info2);
                    }
                }
                return;
            }
        Label_01E9:
            this.RecalculateList();
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if ((newValue != null) && !(newValue is ObservableCollection<TreeListBoxInfo>))
            {
                throw new ArgumentException("Do not use the ItemsSource property for this control please use the HierarchalItemsSource", "newValue");
            }
            base.OnItemsSourceChanged(oldValue, newValue);
        }

        protected void OnNewItemCreated(TreeListBoxItemCreatedEventArgs e)
        {
            if (this.NewItemCreated != null)
            {
                this.NewItemCreated(this, e);
            }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            TreeListBoxItem item2 = element as TreeListBoxItem;
            TreeListBoxInfo info = item as TreeListBoxInfo;
            this.OnNewItemCreated(new TreeListBoxItemCreatedEventArgs(item2, info));
            item2.PrepareItem(info);
            base.PrepareContainerForItemOverride(element, info.DataItem);
        }

        public void RecalculateList()
        {
            this.resetTimer.PerformAction();
        }

        private void ResetFlattenedList()
        {
            if (Interlocked.Exchange(ref this.isResetting, 1) == 0)
            {
                lock (this.CompositeChildCollection)
                {
                    if (this.currentDataSource != null)
                    {
                        this.compositeChildCollection.Clear();
                        if (this.rootNodesInfo == null)
                        {
                            this.rootNodesInfo = new List<TreeListBoxInfo>();
                        }
                        else
                        {
                            this.rootNodesInfo.Clear();
                        }
                        foreach (object obj2 in this.currentDataSource)
                        {
                            TreeListBoxInfo item = new TreeListBoxInfo(0, obj2) {
                                CreatedDuringReset = true
                            };
                            this.compositeChildCollection.Add(item);
                            if (item.IsExpanded)
                            {
                                this.AddDescendentsOnCreation(item);
                            }
                            this.rootNodesInfo.Add(item);
                            if (this.resetTimer.IsBatching)
                            {
                                this.isResetting = 0;
                                return;
                            }
                        }
                        if (this.GetFlattenedTreeItemCount() != this.compositeChildCollection.Count)
                        {
                            this.isResetting = 0;
                            this.RecalculateList();
                            return;
                        }
                        if (!this.resetTimer.IsBatching)
                        {
                            base.ItemsSource = this.compositeChildCollection;
                        }
                    }
                    else
                    {
                        base.ItemsSource = null;
                    }
                    this.isResetting = 0;
                    return;
                }
            }
            this.RecalculateList();
        }

        private void ResetFlattenedListAction()
        {
            if (this.isResetting == 1)
            {
                this.RecalculateList();
            }
            else
            {
                base.Dispatcher.Invoke(delegate {
                    this.ResetFlattenedList();
                }, new object[0]);
            }
        }

        public string ChildItemSourcePath
        {
            get
            {
                return (string) base.GetValue(ChildItemSourcePathProperty);
            }
            set
            {
                base.SetValue(ChildItemSourcePathProperty, value);
            }
        }

        internal ObservableCollection<TreeListBoxInfo> CompositeChildCollection
        {
            get
            {
                return this.compositeChildCollection;
            }
        }

        public IEnumerable HierarchicalItemsSource
        {
            get
            {
                return (IEnumerable) base.GetValue(HierarchicalItemsSourceProperty);
            }
            set
            {
                base.SetValue(HierarchicalItemsSourceProperty, value);
            }
        }
    }
}

