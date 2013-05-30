namespace Microsoft.WPSync.UI.Design
{
    using Microsoft.WPSync.Sync.Rules;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    internal class DesignSyncOptionList : Dictionary<string, ISyncSelectionOption>, ISyncOptionList, INotifyCollectionChanged, IDictionary<string, ISyncSelectionOption>, ICollection<KeyValuePair<string, ISyncSelectionOption>>, IEnumerable<KeyValuePair<string, ISyncSelectionOption>>, IEnumerable
    {
        private NotifyCollectionChangedEventHandler _collectionChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                NotifyCollectionChangedEventHandler handler2;
                NotifyCollectionChangedEventHandler collectionChanged = this._collectionChanged;
                do
                {
                    handler2 = collectionChanged;
                    NotifyCollectionChangedEventHandler handler3 = (NotifyCollectionChangedEventHandler) Delegate.Combine(handler2, value);
                    collectionChanged = Interlocked.CompareExchange<NotifyCollectionChangedEventHandler>(ref this._collectionChanged, handler3, handler2);
                }
                while (collectionChanged != handler2);
            }
            remove
            {
                NotifyCollectionChangedEventHandler handler2;
                NotifyCollectionChangedEventHandler collectionChanged = this._collectionChanged;
                do
                {
                    handler2 = collectionChanged;
                    NotifyCollectionChangedEventHandler handler3 = (NotifyCollectionChangedEventHandler) Delegate.Remove(handler2, value);
                    collectionChanged = Interlocked.CompareExchange<NotifyCollectionChangedEventHandler>(ref this._collectionChanged, handler3, handler2);
                }
                while (collectionChanged != handler2);
            }
        }

        public bool Add(ISyncSelectionOption option)
        {
            if (option == null)
            {
                return false;
            }
            base[option.Key] = option;
            return true;
        }

        public void ClearAllSelections()
        {
            throw new NotImplementedException();
        }

        public ISyncSelectionOption Find(string key)
        {
            throw new NotImplementedException();
        }

        public ISyncSelectionOption FindByPersistentId(string key)
        {
            throw new NotImplementedException();
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void FireCollectionChanged()
        {
            if (this._collectionChanged != null)
            {
                this._collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add));
            }
        }

        public void Remove(ISyncSelectionOption option)
        {
            throw new NotImplementedException();
        }

        public void RemoveAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ISyncSelectionOption> Options
        {
            get
            {
                return base.Values;
            }
        }

        public ISyncSelectionOption ParentOption
        {
            get
            {
                return null;
            }
        }
    }
}

