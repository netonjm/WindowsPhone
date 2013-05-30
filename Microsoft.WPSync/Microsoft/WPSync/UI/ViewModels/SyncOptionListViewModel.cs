namespace Microsoft.WPSync.UI.ViewModels
{
    using Microsoft.WPSync.Shared;
    using Microsoft.WPSync.Sync.Rules;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;

    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public sealed class SyncOptionListViewModel : AsyncObservableCollection<SelectableOptionViewModel>, IDisposable
    {
        private bool autoSelectionValue;
        private BatchingTimer buffer;
        private bool disposed;
        private bool isAutoSelected;
        private object lockObject;
        private readonly ISyncOptionList model;
        private readonly SelectableOptionViewModel parent;

        public SyncOptionListViewModel(ISyncOptionList list, SelectableOptionViewModel parent) : this(list, parent, null)
        {
        }

        public SyncOptionListViewModel(ISyncOptionList list, string filter) : this(list, null, filter)
        {
        }

        public SyncOptionListViewModel(ISyncOptionList list, SelectableOptionViewModel parent, string filter)
        {
            Action<ISyncSelectionOption> action = null;
            this.lockObject = new object();
            this.parent = parent;
            this.FilterString = filter;
            if (list == null)
            {
                throw new ArgumentNullException(typeof(ISyncOptionList).ToString());
            }
            this.model = list;
            if (action == null)
            {
                action = delegate (ISyncSelectionOption o) {
                    this.Add(new SelectableOptionViewModel(o, this, parent, filter));
                };
            }
            list.Options.ToList<ISyncSelectionOption>().ForEach(action);
            this.buffer = new BatchingTimer(new Action<List<object>>(this.ProcessChanges), 0x3e8);
            this.model.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnModelCollectionChanged);
        }

        private void AddNewOption(ISyncSelectionOption newOption)
        {
            base.Add(new SelectableOptionViewModel(newOption, this, this.parent, this.FilterString));
        }

        private void ClearAll()
        {
            base.Clear();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isManualDisposing)
        {
            if (!this.disposed && isManualDisposing)
            {
                lock (this.lockObject)
                {
                    this.model.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnModelCollectionChanged);
                    this.buffer.Dispose();
                    foreach (SelectableOptionViewModel model in this)
                    {
                        model.Dispose();
                    }
                    this.ClearAll();
                }
            }
            this.disposed = true;
        }

        ~SyncOptionListViewModel()
        {
            this.Dispose(false);
        }

        private void HandleChanges(List<object> changes)
        {
            Action<object> action = null;
            lock (this.lockObject)
            {
                if (action == null)
                {
                    action = delegate (object c) {
                        this.ProcessChange((NotifyCollectionChangedEventArgs) c);
                    };
                }
                changes.ForEach(action);
            }
        }

        private void OnModelCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.buffer.AddToBatch(e);
        }

        private void OnProcessedChanges(object e)
        {
            this.HandleChanges((List<object>) e);
        }

        private void ProcessChange(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (ISyncSelectionOption option in e.NewItems)
                    {
                        this.AddNewOption(option);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (ISyncSelectionOption option2 in e.OldItems)
                    {
                        this.RemoveOption(option2);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.ClearAll();
                    break;

                default:
                    return;
            }
        }

        private void ProcessChanges(List<object> changes)
        {
            if (SynchronizationContext.Current == base._synchronizationContext)
            {
                this.HandleChanges(changes);
            }
            else
            {
                base._synchronizationContext.Post(new SendOrPostCallback(this.OnProcessedChanges), changes);
            }
        }

        private void RemoveOption(ISyncSelectionOption option)
        {
            SelectableOptionViewModel item = (from vmo in this
                where vmo.Option == option
                select vmo).FirstOrDefault<SelectableOptionViewModel>();
            if (item != null)
            {
                base.Remove(item);
                item.Dispose();
            }
        }

        public void SetAutoSelect(bool autoSelectOn, bool autoSelectValue)
        {
            this.autoSelectionValue = autoSelectValue;
            this.isAutoSelected = autoSelectOn;
            foreach (SelectableOptionViewModel model in this)
            {
                model.SetAutoSelect(autoSelectOn, autoSelectValue);
            }
        }

        public bool AutoSelectionValue
        {
            get
            {
                return this.autoSelectionValue;
            }
            set
            {
                this.autoSelectionValue = value;
            }
        }

        public string FilterString { get; set; }

        public bool IsAutoSelected
        {
            get
            {
                return this.isAutoSelected;
            }
            set
            {
                this.isAutoSelected = value;
            }
        }
    }
}

