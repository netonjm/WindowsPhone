namespace Microsoft.WPSync.UI.Design
{
    using Microsoft.WPSync.Sync.Rules;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal class DesignSyncSelectionOption : ISyncSelectionOption, INotifyPropertyChanged
    {
        private ISyncOptionList childOptionList;
        private PropertyChangedEventHandler _propertyChanged;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                PropertyChangedEventHandler handler2;
                PropertyChangedEventHandler propertyChanged = this._propertyChanged;
                do
                {
                    handler2 = propertyChanged;
                    PropertyChangedEventHandler handler3 = (PropertyChangedEventHandler) Delegate.Combine(handler2, value);
                    propertyChanged = Interlocked.CompareExchange<PropertyChangedEventHandler>(ref this._propertyChanged, handler3, handler2);
                }
                while (propertyChanged != handler2);
            }
            remove
            {
                PropertyChangedEventHandler handler2;
                PropertyChangedEventHandler propertyChanged = this._propertyChanged;
                do
                {
                    handler2 = propertyChanged;
                    PropertyChangedEventHandler handler3 = (PropertyChangedEventHandler) Delegate.Remove(handler2, value);
                    propertyChanged = Interlocked.CompareExchange<PropertyChangedEventHandler>(ref this._propertyChanged, handler3, handler2);
                }
                while (propertyChanged != handler2);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public DesignSyncSelectionOption(string label)
        {
            this.Label = label;
            this.childOptionList = new DesignSyncOptionList();
        }

        public void AddChildSelectableOption(ISyncSelectionOption childOption)
        {
            throw new NotImplementedException();
        }

        public void AddSyncableMember(ISyncableMedia item)
        {
            throw new NotImplementedException();
        }

        public void ClearAllSelections()
        {
            throw new NotImplementedException();
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ISyncSelectionOption CreateChildSelectableOption(string type, string name)
        {
            SelectionOptionInfo info2 = new SelectionOptionInfo {
                Key = name,
                OptionType = type
            };
            SelectionOptionInfo info = info2;
            ISyncSelectionOption option = new SyncSelectionOption(info, this.childOptionList, AllowAutoSelectionFromChildren.Yes);
            Random random = new Random(10);
            option.IsSelectedForSync = new bool?(random.Next(10) > 6);
            return option;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void FirePropertyChanged()
        {
            if (this._propertyChanged != null)
            {
                this._propertyChanged(this, new PropertyChangedEventArgs("null"));
            }
        }

        public void RemoveChildSelectableOption(ISyncSelectionOption childOption)
        {
            throw new NotImplementedException();
        }

        public void RemoveSyncableMember(ISyncableMedia item)
        {
            throw new NotImplementedException();
        }

        public void SelectFromParent(bool value)
        {
            throw new NotImplementedException();
        }

        public void SelectOnlyThis(bool value)
        {
            throw new NotImplementedException();
        }

        public void UpdateToReflectChildren()
        {
            throw new NotImplementedException();
        }

        public AllowAutoSelectionFromChildren AllowAutoSelectionFromSelectingChildren
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ISyncOptionList ChildOptions
        {
            get
            {
                return new DesignSyncOptionList();
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ConcurrentDictionary<string, ISyncableMedia> ChildSyncableItems { get; set; }

        public string FilterMatchingString { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public bool HasChildOptions { get; set; }

        public bool? IsSelectedForSync { get; set; }

        public IEnumerable<ISyncableMedia> ItemsToSync
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Key { get; set; }

        public string Label { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="value")]
        public int Level
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Name { get; set; }

        public string Note { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ISyncSelectionOption ParentOption { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public string Type { get; set; }
    }
}

