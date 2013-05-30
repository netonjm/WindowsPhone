namespace Microsoft.WPSync.UI.ViewModels
{
    using Microsoft.WPSync.Shared;
    using Microsoft.WPSync.Sync.Rules;
    using Microsoft.WPSync.UI.Controls;
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows.Data;

    public sealed class SelectableOptionViewModel : PropChangeNotifier, IDisposable, IExpandable
    {
        private bool autoSelectionValue;
        private SyncOptionListViewModel childOptionListVm;
        private ListCollectionView childView;
        private bool disposed;
        private string filterString;
        private bool isAutoSelected;
        private bool isExpanded;
        private static IComparer labelSorter = new LabelSorter();
        private object lockObject;
        private readonly SyncOptionListViewModel parentListVm;
        private readonly SelectableOptionViewModel parentVm;
        private string sortString;

        public SelectableOptionViewModel(ISyncSelectionOption option, SyncOptionListViewModel parentList, SelectableOptionViewModel parent) : this(option, parentList, parent, null)
        {
        }

        public SelectableOptionViewModel(ISyncSelectionOption option, SyncOptionListViewModel parentList, string filter) : this(option, parentList, null, filter)
        {
        }

        public SelectableOptionViewModel(ISyncSelectionOption option, SyncOptionListViewModel parentList, SelectableOptionViewModel parent, string filter)
        {
            string str;
            this.isExpanded = true;
            this.lockObject = new object();
            if (option == null)
            {
                throw new ArgumentNullException("option");
            }
            this.Option = option;
            this.parentVm = parent;
            this.parentListVm = parentList;
            this.filterString = filter;
            if (parentList != null)
            {
                this.isAutoSelected = parentList.IsAutoSelected;
                this.autoSelectionValue = parentList.AutoSelectionValue;
            }
            if (StringUtilities.HasArticlePrefix(option.Label, out str))
            {
                this.sortString = str;
            }
            this.InitViewModel();
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
                this.Option.PropertyChanged -= new PropertyChangedEventHandler(this.OnOptionPropertyChanged);
                this.Option.ChildOptions.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnChildOptionsCollectionChanged);
                if (this.childOptionListVm != null)
                {
                    this.childOptionListVm.Dispose();
                }
            }
            this.disposed = true;
        }

        public static bool DoesMatchFilter(object value)
        {
            SelectableOptionViewModel model = (SelectableOptionViewModel) value;
            if (string.IsNullOrWhiteSpace(model.FilterString))
            {
                return true;
            }
            bool flag = model.Option.FilterMatchingString.Contains(model.FilterString);
            if (!flag && model.Option.HasChildOptions)
            {
                return (model.childView.Count > 0);
            }
            return flag;
        }

        ~SelectableOptionViewModel()
        {
            this.Dispose(false);
        }

        private void InitViewModel()
        {
            this.isExpanded = true;
            this.Option.PropertyChanged += new PropertyChangedEventHandler(this.OnOptionPropertyChanged);
            this.Option.ChildOptions.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnChildOptionsCollectionChanged);
            this.ResetView();
        }

        private void OnChildOptionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.parentListVm.SynchronizationContext.Post(new SendOrPostCallback(this.ResetViewCallback), null);
        }

        private void OnFilterStringChanged()
        {
            if (this.Option.HasChildOptions)
            {
                this.childOptionListVm.FilterString = this.FilterString;
                foreach (SelectableOptionViewModel model in this.childOptionListVm)
                {
                    model.OnFilterStringChanged();
                }
                if (this.childView != null)
                {
                    this.childView.Refresh();
                }
            }
        }

        private void OnOptionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName;
            if (propertyName != null)
            {
                if (!(propertyName == "IsSelectedForSync"))
                {
                    if (!(propertyName == "Label"))
                    {
                        if (propertyName == "Note")
                        {
                            this.OnPropertyChanged("Note");
                        }
                        return;
                    }
                }
                else
                {
                    this.OnPropertyChanged("IsSelected");
                    this.OnPropertyChanged("IsAutoSelected");
                    return;
                }
                this.OnPropertyChanged("Label");
            }
        }

        public void ResetView()
        {
            lock (this.lockObject)
            {
                if (this.childOptionListVm == null)
                {
                    this.childOptionListVm = new SyncOptionListViewModel(this.Option.ChildOptions, this);
                    this.childOptionListVm.SetAutoSelect(this.isAutoSelected, this.autoSelectionValue);
                    this.childView = new ListCollectionView(this.childOptionListVm);
                    using (this.childView.DeferRefresh())
                    {
                        this.childView.CustomSort = labelSorter;
                        this.childView.Filter = new Predicate<object>(SelectableOptionViewModel.DoesMatchFilter);
                        goto Label_00C9;
                    }
                }
                if (!this.Option.HasChildOptions)
                {
                    this.childOptionListVm.Dispose();
                    this.childOptionListVm = null;
                    this.ChildView = null;
                }
            Label_00C9:;
            }
        }

        private void ResetViewCallback(object notice)
        {
            this.ResetView();
        }

        public void SetAutoSelect(bool autoSelectOn, bool autoSelectValue)
        {
            this.autoSelectionValue = autoSelectValue;
            this.isAutoSelected = autoSelectOn;
            this.OnPropertyChanged("IsAutoSelected");
            this.OnPropertyChanged("IsSelected");
            if (this.childOptionListVm != null)
            {
                this.childOptionListVm.SetAutoSelect(autoSelectOn, autoSelectValue);
            }
        }

        public override string ToString()
        {
            return StringUtilities.MergeBiDiMixedStrings(this.Option.Name, this.Option.Note, " ");
        }

        public int ChildrenCount
        {
            get
            {
                if (this.Option.HasChildOptions)
                {
                    return this.Option.ChildOptions.Count;
                }
                return 0;
            }
        }

        public ListCollectionView ChildView
        {
            get
            {
                return this.childView;
            }
            set
            {
                this.childView = value;
                this.OnPropertyChanged("ChildView");
            }
        }

        public string FilterString
        {
            get
            {
                if (this.parentVm == null)
                {
                    return this.filterString;
                }
                return this.parentVm.FilterString;
            }
            set
            {
                if (this.filterString != value)
                {
                    this.filterString = value;
                    this.OnFilterStringChanged();
                }
            }
        }

        public bool IsAutoSelected
        {
            get
            {
                return this.isAutoSelected;
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
                this.OnPropertyChanged("IsExpanded");
            }
        }

        public bool? IsSelected
        {
            get
            {
                if (this.isAutoSelected)
                {
                    return new bool?(this.autoSelectionValue);
                }
                return this.Option.IsSelectedForSync;
            }
            set
            {
                this.Option.IsSelectedForSync = value;
            }
        }

        public string Label
        {
            get
            {
                return this.Option.Label;
            }
        }

        public string Note
        {
            get
            {
                return this.Option.Note;
            }
        }

        public ISyncSelectionOption Option { get; set; }

        public string SortString
        {
            get
            {
                return (this.sortString ?? this.Label);
            }
        }
    }
}

