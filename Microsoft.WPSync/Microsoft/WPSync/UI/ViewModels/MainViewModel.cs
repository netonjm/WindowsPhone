namespace Microsoft.WPSync.UI.ViewModels
{
    using Microsoft.WPSync.Device;
    using Microsoft.WPSync.Logging;
    using Microsoft.WPSync.Settings;
    using Microsoft.WPSync.Shared;
    using Microsoft.WPSync.UI;
    using Microsoft.WPSync.UI.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    public class MainViewModel : PropChangeNotifier, IMainViewModel, INotifyPropertyChanged
    {
        private RelayCommand cancelSyncCommand;
        private RelayCommand commitSettingsCommand;
        private readonly IMainController controller;
        private MainViewState currentClientView;
        private ContentViewState currentContentViewState = ContentViewState.MusicPanel;
        private IDevice currentDevice;
        private ISyncPartnership currentPartnership;
        private RelayCommand deleteCommand;
        private RelayCommand deviceChooserCommand;
        private RelayCommand doSyncCommand;
        private RelayCommand eraseContentCommand;
        private string filterString;
        private RelayCommand findYourPhoneCommand;
        private RelayCommand forgetPhoneCommand;
        private RelayCommand helpCommand;
        private Cursor mainWindowCursor;
        private RelayCommand nextDeviceCommand;
        private ContentViewState previousContentState = ContentViewState.MusicPanel;
        private RelayCommand saveToPCCommand;
        private RelayCommand setContentViewCommand;
        private RelayCommand setMainViewCommand;
        private RelayCommand settingsCommand;
        private bool showConnectingToDevice;
        private bool showDeviceChooser;

        public MainViewModel(IMainController controller, IViewModelFactory factory)
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }
            this.controller = controller;
            if (factory == null)
            {
                throw new ArgumentNullException(typeof(IMainFactory).ToString());
            }
            this.SyncProgressViewModel = factory.CreateISyncProgressViewModel(controller);
            if (this.SyncProgressViewModel == null)
            {
                throw new ApplicationInitializationException(typeof(ISyncProgressViewModel).ToString());
            }
            this.StorageGaugeViewModel = factory.CreateIStorageGaugeViewModel(controller);
            if (this.StorageGaugeViewModel == null)
            {
                throw new ApplicationInitializationException(typeof(IStorageGaugeViewModel).ToString());
            }
            this.ContentViewModel = factory.CreateIMediaContentViewModel(controller);
            if (this.ContentViewModel == null)
            {
                throw new ApplicationInitializationException(typeof(IMediaContentViewModel).ToString());
            }
            this.DeviceViewModel = factory.CreateIDeviceViewModel(controller);
            if (this.DeviceViewModel == null)
            {
                throw new ApplicationInitializationException(typeof(IDeviceViewModel).ToString());
            }
            this.DeviceSettingsViewModel = factory.CreateDeviceSettingsViewModel(controller);
            if (this.DeviceSettingsViewModel == null)
            {
                throw new ApplicationInitializationException(typeof(Microsoft.WPSync.UI.ViewModels.DeviceSettingsViewModel).ToString());
            }
            this.AppSettingsViewModel = factory.CreateAppSettingsViewModel(controller);
            if (this.AppSettingsViewModel == null)
            {
                throw new ApplicationInitializationException(typeof(Microsoft.WPSync.UI.ViewModels.AppSettingsViewModel).ToString());
            }
            this.AppSettingsViewModel.Init();
            controller.PropertyChanged += new PropertyChangedEventHandler(this.OnControllerPropertyChanged);
        }

        private static bool IsSpecialContentState(ContentViewState state)
        {
            return (state == ContentViewState.SettingsPanel);
        }

        private void OnContentViewChanged()
        {
            this.UpdateFilterString();
            this.OnPropertyChanged("ShowContentHeader");
            this.OnPropertyChanged("ShowContentFooter");
            CommandManager.InvalidateRequerySuggested();
            if (this.CurrentContentViewState == ContentViewState.SettingsPanel)
            {
                this.DeviceSettingsViewModel.Init();
                this.AppSettingsViewModel.Init();
            }
        }

        private void OnControllerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Action method = null;
            Action action2 = null;
            string propertyName = e.PropertyName;
            if (propertyName != null)
            {
                if (!(propertyName == "CurrentSyncPartnership"))
                {
                    if (!(propertyName == "CurrentDevice"))
                    {
                        return;
                    }
                }
                else
                {
                    if (method == null)
                    {
                        method = delegate {
                            this.OnPartnershipChanged();
                        };
                    }
                    this.controller.Dispatcher.BeginInvoke(method, new object[0]);
                    return;
                }
                if (action2 == null)
                {
                    action2 = delegate {
                        this.OnDeviceChanged();
                    };
                }
                this.controller.Dispatcher.BeginInvoke(action2, new object[0]);
            }
        }

        private void OnDeviceChanged()
        {
            if (this.currentDevice != null)
            {
                this.currentDevice.PropertyChanged -= new PropertyChangedEventHandler(this.OnDevicePropertyChanged);
            }
            this.currentDevice = this.controller.CurrentDevice;
            if (this.currentDevice != null)
            {
                this.currentDevice.PropertyChanged += new PropertyChangedEventHandler(this.OnDevicePropertyChanged);
            }
            this.ResetDeviceState();
        }

        private void OnDevicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Action method = null;
            IDevice device = sender as IDevice;
            if (e.PropertyName == "CachedIsLocked")
            {
                if (method == null)
                {
                    method = delegate {
                        this.OnLockChange(device);
                    };
                }
                this.controller.Dispatcher.BeginInvoke(method, new object[0]);
            }
        }

        private void OnFilterStringChanged()
        {
            ContentViewState currentContentViewState = this.controller.MainViewModel.CurrentContentViewState;
            switch (this.controller.MainViewModel.CurrentViewState)
            {
                case MainViewState.PhoneTab:
                {
                    IDeviceViewModel deviceViewModel = this.controller.MainViewModel.DeviceViewModel;
                    switch (currentContentViewState)
                    {
                        case ContentViewState.MusicPanel:
                            deviceViewModel.MusicFilterString = this.FilterString;
                            return;

                        case ContentViewState.PhotosVideosPanel:
                            deviceViewModel.PhotosVideosFilterString = this.FilterString;
                            return;

                        case ContentViewState.MoviesTVPanel:
                            deviceViewModel.MoviesTVSeriesFilterString = this.FilterString;
                            return;

                        case ContentViewState.PodcastsPanel:
                            deviceViewModel.PodcastsFilterString = this.FilterString;
                            return;
                    }
                    return;
                }
                case MainViewState.PCTab:
                {
                    IMediaContentViewModel contentViewModel = this.controller.MainViewModel.ContentViewModel;
                    switch (currentContentViewState)
                    {
                        case ContentViewState.MusicPanel:
                            contentViewModel.MusicFilterString = this.FilterString;
                            return;

                        case ContentViewState.PhotosVideosPanel:
                            contentViewModel.PhotosVideosFilterString = this.FilterString;
                            return;

                        case ContentViewState.MoviesTVPanel:
                            contentViewModel.MoviesTVSeriesFilterString = this.FilterString;
                            return;

                        case ContentViewState.PodcastsPanel:
                            contentViewModel.PodcastsFilterString = this.FilterString;
                            return;

                        case ContentViewState.RingtonesPanel:
                            contentViewModel.RingtonesFilterString = this.FilterString;
                            return;
                    }
                    return;
                }
            }
        }

        private void OnLockChange(IDevice device)
        {
            if (device == this.currentDevice)
            {
                this.ResetDeviceState();
            }
        }

        private void OnPartnershipChanged()
        {
            if (this.currentPartnership != null)
            {
                this.currentPartnership.StateChanged -= new EventHandler<PartnershipStateChangeEventArgs>(this.OnPartnershipStateChanged);
            }
            this.currentPartnership = this.controller.CurrentSyncPartnership;
            if (this.currentPartnership != null)
            {
                this.currentPartnership.StateChanged += new EventHandler<PartnershipStateChangeEventArgs>(this.OnPartnershipStateChanged);
            }
        }

        private void OnPartnershipStateChanged(object sender, PartnershipStateChangeEventArgs args)
        {
            this.OnPropertyChanged("CanDoSync");
            this.OnPropertyChanged("CanCancelSync");
            this.OnPropertyChanged("CanCancelSync");
            this.OnPropertyChanged("CanCancelSync");
            CommandManager.InvalidateRequerySuggested();
        }

        public void OnUserChangingMainView(MainViewState newState)
        {
            if (((this.CurrentContentViewState == ContentViewState.SettingsPanel) && ((this.CurrentViewState == MainViewState.PCTab) || (this.CurrentViewState == MainViewState.PhoneTab))) && ((newState == MainViewState.PCTab) || (newState == MainViewState.PhoneTab)))
            {
                this.CommitSettingsCommand.Execute(null);
            }
        }

        private void ResetDeviceState()
        {
            if (this.currentDevice == null)
            {
                this.SetMainView(MainViewState.UnconnectedState);
            }
            else
            {
                this.currentDevice.PropertyChanged += new PropertyChangedEventHandler(this.OnDevicePropertyChanged);
                if (this.currentDevice.CachedIsLocked)
                {
                    this.SetMainView(MainViewState.LockedPanel);
                }
                else if (!this.controller.IsDevicePartnered(this.currentDevice))
                {
                    this.SetMainView(MainViewState.FirstConnectPanel);
                }
                else
                {
                    this.SetMainView(MainViewState.PCTab);
                    if (IsSpecialContentState(this.CurrentContentViewState))
                    {
                        this.SetContentView(this.previousContentState);
                    }
                }
            }
            this.ShowDeviceChooser = false;
        }

        public void SetContentView(ContentViewState contentView)
        {
            if (contentView == ContentViewState.LastMediaPanel)
            {
                contentView = this.previousContentState;
            }
            if (contentView != this.CurrentContentViewState)
            {
                this.CurrentContentViewState = contentView;
            }
        }

        public void SetMainView(MainViewState viewState)
        {
            using (new OperationLogger())
            {
                this.CurrentViewState = viewState;
                if ((this.CurrentContentViewState == ContentViewState.RingtonesPanel) && (this.CurrentViewState == MainViewState.PhoneTab))
                {
                    this.SetContentView(ContentViewState.MusicPanel);
                }
                if (((this.CurrentContentViewState == ContentViewState.PodcastsPanel) && (this.CurrentViewState == MainViewState.PCTab)) && GlobalSetting.IsMusicSourceWindowsLibraries())
                {
                    this.SetContentView(ContentViewState.MusicPanel);
                }
            }
            this.UpdateFilterString();
        }

        private void UpdateFilterString()
        {
            ContentViewState currentContentViewState = this.controller.MainViewModel.CurrentContentViewState;
            switch (this.controller.MainViewModel.CurrentViewState)
            {
                case MainViewState.PhoneTab:
                {
                    IDeviceViewModel deviceViewModel = this.controller.MainViewModel.DeviceViewModel;
                    switch (currentContentViewState)
                    {
                        case ContentViewState.MusicPanel:
                            this.FilterString = deviceViewModel.MusicFilterString;
                            return;

                        case ContentViewState.PhotosVideosPanel:
                            this.FilterString = deviceViewModel.PhotosVideosFilterString;
                            return;

                        case ContentViewState.MoviesTVPanel:
                            this.FilterString = deviceViewModel.MoviesTVSeriesFilterString;
                            return;

                        case ContentViewState.PodcastsPanel:
                            this.FilterString = deviceViewModel.PodcastsFilterString;
                            return;
                    }
                    return;
                }
                case MainViewState.PCTab:
                {
                    IMediaContentViewModel contentViewModel = this.controller.MainViewModel.ContentViewModel;
                    switch (currentContentViewState)
                    {
                        case ContentViewState.MusicPanel:
                            this.FilterString = contentViewModel.MusicFilterString;
                            return;

                        case ContentViewState.PhotosVideosPanel:
                            this.FilterString = contentViewModel.PhotosVideosFilterString;
                            return;

                        case ContentViewState.MoviesTVPanel:
                            this.FilterString = contentViewModel.MoviesTVSeriesFilterString;
                            return;

                        case ContentViewState.PodcastsPanel:
                            this.FilterString = contentViewModel.PodcastsFilterString;
                            return;

                        case ContentViewState.RingtonesPanel:
                            this.FilterString = contentViewModel.RingtonesFilterString;
                            return;
                    }
                    return;
                }
            }
        }

        public Microsoft.WPSync.UI.ViewModels.AppSettingsViewModel AppSettingsViewModel { get; private set; }

        public bool CanCancelSync
        {
            get
            {
                if (this.controller.CurrentSyncPartnership == null)
                {
                    return false;
                }
                return (this.controller.CurrentSyncPartnership.CurrentState == PartnershipState.Syncing);
            }
        }

        public ICommand CancelSyncCommand
        {
            get
            {
                Action<object> execute = null;
                Predicate<object> canExecute = null;
                if (this.cancelSyncCommand == null)
                {
                    if (execute == null)
                    {
                        execute = delegate (object param) {
                            this.controller.CancelSync();
                        };
                    }
                    if (canExecute == null)
                    {
                        canExecute = param => this.CanCancelSync;
                    }
                    this.cancelSyncCommand = new RelayCommand(execute, canExecute);
                }
                return this.cancelSyncCommand;
            }
        }

        public bool CanDoSync
        {
            get
            {
                if (this.controller.CurrentSyncPartnership == null)
                {
                    return false;
                }
                return (this.controller.CurrentSyncPartnership.CurrentState == PartnershipState.Idle);
            }
        }

        public bool CanSelectContentTab
        {
            get
            {
                if (this.controller.CurrentSyncPartnership == null)
                {
                    return false;
                }
                if (IsSpecialContentState(this.CurrentContentViewState))
                {
                    return (this.CurrentContentViewState == ContentViewState.SettingsPanel);
                }
                return true;
            }
        }

        public bool CanSelectMainTab
        {
            get
            {
                if (this.controller.CurrentSyncPartnership == null)
                {
                    return false;
                }
                if (IsSpecialContentState(this.CurrentContentViewState))
                {
                    return (this.CurrentContentViewState == ContentViewState.SettingsPanel);
                }
                return true;
            }
        }

        public bool CanSelectSettings
        {
            get
            {
                return (((!IsSpecialContentState(this.CurrentContentViewState) || (this.CurrentContentViewState == ContentViewState.SettingsPanel)) && ((this.controller.CurrentSyncPartnership != null) && (this.controller.CurrentSyncPartnership.CurrentState != PartnershipState.PreparingSync))) && (this.controller.CurrentSyncPartnership.CurrentState != PartnershipState.Syncing));
            }
        }

        public ICommand CommitSettingsCommand
        {
            get
            {
                Action<object> execute = null;
                if (this.commitSettingsCommand == null)
                {
                    if (execute == null)
                    {
                        execute = delegate (object param) {
                            if (this.DeviceSettingsViewModel.Validate() && this.AppSettingsViewModel.Validate())
                            {
                                this.DeviceSettingsViewModel.Commit();
                                this.AppSettingsViewModel.Commit();
                                if ((this.AppSettingsViewModel.SyncSourceIsWindowsLibraries && (this.previousContentState == ContentViewState.PodcastsPanel)) && (this.CurrentViewState == MainViewState.PCTab))
                                {
                                    this.SetContentView(ContentViewState.MusicPanel);
                                }
                                else
                                {
                                    this.SetContentView(ContentViewState.LastMediaPanel);
                                }
                            }
                        };
                    }
                    this.commitSettingsCommand = new RelayCommand(execute);
                }
                return this.commitSettingsCommand;
            }
        }

        public IMediaContentViewModel ContentViewModel { get; private set; }

        public ContentViewState CurrentContentViewState
        {
            get
            {
                return this.currentContentViewState;
            }
            set
            {
                this.currentContentViewState = value;
                if (!IsSpecialContentState(this.currentContentViewState))
                {
                    this.previousContentState = value;
                }
                this.OnContentViewChanged();
                this.OnPropertyChanged("CurrentContentViewState");
            }
        }

        public MainViewState CurrentViewState
        {
            get
            {
                return this.currentClientView;
            }
            set
            {
                if (value == MainViewState.FirstConnectPanel)
                {
                    this.DeviceSettingsViewModel.Init();
                    this.AppSettingsViewModel.Init();
                }
                this.currentClientView = value;
                this.OnPropertyChanged("CurrentViewState");
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                Action<object> execute = null;
                Predicate<object> canExecute = null;
                if (this.deleteCommand == null)
                {
                    if (execute == null)
                    {
                        execute = delegate (object param) {
                            IEnumerable<DeviceItemViewModel> source = param as IEnumerable<DeviceItemViewModel>;
                            if (source != null)
                            {
                                this.DeviceViewModel.DeleteItems(from item in source
                                    where item.IsChecked
                                    select item);
                            }
                        };
                    }
                    if (canExecute == null)
                    {
                        canExecute = delegate (object param) {
                            IEnumerable<DeviceItemViewModel> source = param as IEnumerable<DeviceItemViewModel>;
                            if (((source == null) || (this.controller.CurrentSyncPartnership == null)) || (this.controller.CurrentSyncPartnership.CurrentState != PartnershipState.Idle))
                            {
                                return false;
                            }
                            return source.Any<DeviceItemViewModel>(item => item.IsChecked);
                        };
                    }
                    this.deleteCommand = new RelayCommand(execute, canExecute);
                }
                return this.deleteCommand;
            }
        }

        public ICommand DeviceChooserCommand
        {
            get
            {
                Action<object> execute = null;
                if (this.deviceChooserCommand == null)
                {
                    if (execute == null)
                    {
                        execute = delegate (object param) {
                            this.ShowDeviceChooser = !this.ShowDeviceChooser;
                        };
                    }
                    this.deviceChooserCommand = new RelayCommand(execute);
                }
                return this.deviceChooserCommand;
            }
        }

        public Microsoft.WPSync.UI.ViewModels.DeviceSettingsViewModel DeviceSettingsViewModel { get; private set; }

        public IDeviceViewModel DeviceViewModel { get; private set; }

        public ICommand DoSyncCommand
        {
            get
            {
                Action<object> execute = null;
                Predicate<object> canExecute = null;
                if (this.doSyncCommand == null)
                {
                    if (execute == null)
                    {
                        execute = delegate (object param) {
                            this.controller.DoSync((SyncStartType) param);
                        };
                    }
                    if (canExecute == null)
                    {
                        canExecute = param => this.CanDoSync;
                    }
                    this.doSyncCommand = new RelayCommand(execute, canExecute);
                }
                return this.doSyncCommand;
            }
        }

        public ICommand EraseContentCommand
        {
            get
            {
                Action<object> execute = null;
                if (this.eraseContentCommand == null)
                {
                    if (execute == null)
                    {
                        execute = delegate (object param) {
                            this.controller.MainViewModel.DeviceSettingsViewModel.EraseContent();
                        };
                    }
                    this.eraseContentCommand = new RelayCommand(execute);
                }
                return this.eraseContentCommand;
            }
        }

        public string FilterString
        {
            get
            {
                return this.filterString;
            }
            set
            {
                this.filterString = value;
                this.OnFilterStringChanged();
                this.OnPropertyChanged("FilterString");
            }
        }

        public ICommand FindYourPhoneCommand
        {
            get
            {
                Action<object> execute = null;
                if (this.findYourPhoneCommand == null)
                {
                    if (execute == null)
                    {
                        execute = delegate (object param) {
                            this.controller.OpenBrowserLink("http://go.microsoft.com/fwlink/?LinkID=265475");
                        };
                    }
                    this.findYourPhoneCommand = new RelayCommand(execute);
                }
                return this.findYourPhoneCommand;
            }
        }

        public ICommand ForgetPhoneCommand
        {
            get
            {
                Action<object> execute = null;
                if (this.forgetPhoneCommand == null)
                {
                    if (execute == null)
                    {
                        execute = delegate (object param) {
                            this.controller.UnpartnerWithPhone();
                        };
                    }
                    this.forgetPhoneCommand = new RelayCommand(execute);
                }
                return this.forgetPhoneCommand;
            }
        }

        public ICommand HelpCommand
        {
            get
            {
                Action<object> execute = null;
                if (this.helpCommand == null)
                {
                    if (execute == null)
                    {
                        execute = delegate (object param) {
                            this.controller.OpenBrowserLink("http://go.microsoft.com/fwlink/?LinkID=260953");
                        };
                    }
                    this.helpCommand = new RelayCommand(execute);
                }
                return this.helpCommand;
            }
        }

        public Cursor MainWindowCursor
        {
            get
            {
                return this.mainWindowCursor;
            }
            set
            {
                this.mainWindowCursor = value;
                this.OnPropertyChanged("MainWindowCursor");
            }
        }

        public ICommand NextDeviceCommand
        {
            get
            {
                Action<object> execute = null;
                if (this.nextDeviceCommand == null)
                {
                    if (execute == null)
                    {
                        execute = delegate (object param) {
                            this.controller.SwitchToNextDevice();
                        };
                    }
                    this.nextDeviceCommand = new RelayCommand(execute);
                }
                return this.nextDeviceCommand;
            }
        }

        public ICommand SaveToPCCommand
        {
            get
            {
                Action<object> execute = null;
                Predicate<object> canExecute = null;
                if (this.saveToPCCommand == null)
                {
                    if (execute == null)
                    {
                        execute = delegate (object param) {
                            IEnumerable<DeviceItemViewModel> source = param as IEnumerable<DeviceItemViewModel>;
                            if (source != null)
                            {
                                this.DeviceViewModel.CopyItemsToPC(from item in source
                                    where item.IsChecked
                                    select item);
                            }
                        };
                    }
                    if (canExecute == null)
                    {
                        canExecute = delegate (object param) {
                            IEnumerable<DeviceItemViewModel> source = param as IEnumerable<DeviceItemViewModel>;
                            if (((source == null) || (this.controller.CurrentSyncPartnership == null)) || (this.controller.CurrentSyncPartnership.CurrentState != PartnershipState.Idle))
                            {
                                return false;
                            }
                            return source.Any<DeviceItemViewModel>(item => item.IsChecked);
                        };
                    }
                    this.saveToPCCommand = new RelayCommand(execute, canExecute);
                }
                return this.saveToPCCommand;
            }
        }

        public ICommand SetContentViewCommand
        {
            get
            {
                Action<object> execute = null;
                Predicate<object> canExecute = null;
                if (this.setContentViewCommand == null)
                {
                    if (execute == null)
                    {
                        execute = delegate (object param) {
                            this.controller.MainViewModel.SetContentView((ContentViewState) param);
                        };
                    }
                    if (canExecute == null)
                    {
                        canExecute = param => this.CanSelectContentTab;
                    }
                    this.setContentViewCommand = new RelayCommand(execute, canExecute);
                }
                return this.setContentViewCommand;
            }
        }

        public ICommand SetMainViewCommand
        {
            get
            {
                Action<object> execute = null;
                Predicate<object> canExecute = null;
                if (this.setMainViewCommand == null)
                {
                    if (execute == null)
                    {
                        execute = delegate (object param) {
                            this.controller.MainViewModel.SetMainView((MainViewState) param);
                        };
                    }
                    if (canExecute == null)
                    {
                        canExecute = param => this.CanSelectMainTab;
                    }
                    this.setMainViewCommand = new RelayCommand(execute, canExecute);
                }
                return this.setMainViewCommand;
            }
        }

        public ICommand SettingsCommand
        {
            get
            {
                Action<object> execute = null;
                Predicate<object> canExecute = null;
                if (this.settingsCommand == null)
                {
                    if (execute == null)
                    {
                        execute = delegate (object param) {
                            this.controller.MainViewModel.SetContentView(ContentViewState.SettingsPanel);
                        };
                    }
                    if (canExecute == null)
                    {
                        canExecute = param => this.CanSelectSettings;
                    }
                    this.settingsCommand = new RelayCommand(execute, canExecute);
                }
                return this.settingsCommand;
            }
        }

        public bool ShowConnectingToDevice
        {
            get
            {
                return this.showConnectingToDevice;
            }
            set
            {
                this.showConnectingToDevice = value;
                this.OnPropertyChanged("ShowConnectingToDevice");
            }
        }

        public bool ShowContentFooter
        {
            get
            {
                return true;
            }
        }

        public bool ShowContentHeader
        {
            get
            {
                return !IsSpecialContentState(this.CurrentContentViewState);
            }
        }

        public bool ShowDeviceChooser
        {
            get
            {
                return this.showDeviceChooser;
            }
            set
            {
                this.showDeviceChooser = value;
                this.OnPropertyChanged("ShowDeviceChooser");
            }
        }

        public IStorageGaugeViewModel StorageGaugeViewModel { get; private set; }

        public ISyncProgressViewModel SyncProgressViewModel { get; private set; }
    }
}

