namespace Microsoft.WPSync.UI.Design
{
    using Microsoft.WPSync.UI;
    using Microsoft.WPSync.UI.ViewModels;
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows.Input;

    public class DesignMainViewModel : IMainViewModel, INotifyPropertyChanged
    {
        private bool canCancel;
        private bool canSync = true;
        private ContentViewState currentContentViewState;
        private MainViewState currentViewState;
        private PropertyChangedEventHandler _propertyChanged;
        private bool showDeviceChooser;

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

        public DesignMainViewModel(IMainController controller)
        {
            this.Controller = controller;
            this.StorageGaugeViewModel = new DesignStorageGaugeViewModel(controller);
            this.SyncProgressViewModel = new DesignSyncProgressViewModel(controller, false);
            this.ContentViewModel = new DesignContentViewModel();
            this.DeviceViewModel = new DesignDeviceViewModel();
            this.DeviceSettingsViewModel = new Microsoft.WPSync.UI.ViewModels.DeviceSettingsViewModel(controller);
            this.DeviceSettingsViewModel.Init();
            this.AppSettingsViewModel = new Microsoft.WPSync.UI.ViewModels.AppSettingsViewModel(controller);
            this.AppSettingsViewModel.Init();
            this.CurrentViewState = MainViewState.PCTab;
            this.CurrentContentViewState = ContentViewState.MusicPanel;
        }

        public static void EmptyAction()
        {
        }

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        public void FirePropertyChanged(string prop)
        {
            if (this._propertyChanged != null)
            {
                this._propertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        private void OnContentViewChanged()
        {
            this.FirePropertyChanged("ShowContentHeader");
            this.FirePropertyChanged("ShowContentFooter");
            CommandManager.InvalidateRequerySuggested();
            if (this.CurrentContentViewState == ContentViewState.SettingsPanel)
            {
                this.DeviceSettingsViewModel.Init();
                this.AppSettingsViewModel.Init();
            }
        }

        public void OnUserChangingMainView(MainViewState newState)
        {
        }

        public void SetContentView(ContentViewState contentView)
        {
            this.CurrentContentViewState = contentView;
        }

        public void SetMainView(MainViewState viewState)
        {
            this.CurrentViewState = viewState;
        }

        public Microsoft.WPSync.UI.ViewModels.AppSettingsViewModel AppSettingsViewModel { get; private set; }

        public bool CanCancel
        {
            get
            {
                return this.canCancel;
            }
            set
            {
                this.canCancel = value;
            }
        }

        public ICommand CancelSyncCommand
        {
            get
            {
                return new RelayCommand(delegate (object param) {
                    EmptyAction();
                }, param => this.CanCancel);
            }
        }

        public bool CanSync
        {
            get
            {
                return this.canSync;
            }
            set
            {
                this.canSync = value;
            }
        }

        public ICommand CommitSettingsCommand
        {
            get
            {
                return new RelayCommand(delegate (object param) {
                    EmptyAction();
                });
            }
        }

        public IMediaContentViewModel ContentViewModel { get; private set; }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private IMainController Controller { get; set; }

        public ContentViewState CurrentContentViewState
        {
            get
            {
                return this.currentContentViewState;
            }
            private set
            {
                this.currentContentViewState = value;
                this.OnContentViewChanged();
                this.FirePropertyChanged("CurrentContentViewState");
            }
        }

        public MainViewState CurrentViewState
        {
            get
            {
                return this.currentViewState;
            }
            private set
            {
                this.currentViewState = value;
                this.OnContentViewChanged();
                this.FirePropertyChanged("CurrentViewState");
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                return new RelayCommand(delegate (object param) {
                    EmptyAction();
                }, param => this.CanCancel);
            }
        }

        public ICommand DeviceChooserCommand
        {
            get
            {
                return new RelayCommand(delegate (object param) {
                    this.ShowDeviceChooser = !this.ShowDeviceChooser;
                });
            }
        }

        public Microsoft.WPSync.UI.ViewModels.DeviceSettingsViewModel DeviceSettingsViewModel { get; private set; }

        public IDeviceViewModel DeviceViewModel { get; private set; }

        public ICommand DoSyncCommand
        {
            get
            {
                return new RelayCommand(delegate (object param) {
                    EmptyAction();
                }, param => this.CanSync);
            }
        }

        public ICommand EraseContentCommand
        {
            get
            {
                return new RelayCommand(delegate (object param) {
                    EmptyAction();
                });
            }
        }

        public string FilterString
        {
            get
            {
                return "";
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public ICommand FindYourPhoneCommand
        {
            get
            {
                return new RelayCommand(delegate (object param) {
                    this.Controller.OpenBrowserLink("http://go.microsoft.com/fwlink/?LinkID=265475");
                });
            }
        }

        public ICommand ForgetPhoneCommand
        {
            get
            {
                return new RelayCommand(delegate (object param) {
                    EmptyAction();
                });
            }
        }

        public ICommand HelpCommand
        {
            get
            {
                return new RelayCommand(delegate (object param) {
                    this.Controller.OpenBrowserLink("http://go.microsoft.com/fwlink/?LinkID=260953");
                });
            }
        }

        public Cursor MainWindowCursor
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public ICommand NextDeviceCommand
        {
            get
            {
                return new RelayCommand(delegate (object param) {
                    EmptyAction();
                });
            }
        }

        public ICommand SaveToPCCommand
        {
            get
            {
                return new RelayCommand(delegate (object param) {
                    EmptyAction();
                }, param => this.CanSync);
            }
        }

        public ICommand SetContentViewCommand
        {
            get
            {
                return new RelayCommand(delegate (object param) {
                    this.SetContentView((ContentViewState) param);
                });
            }
        }

        public ICommand SetMainViewCommand
        {
            get
            {
                return new RelayCommand(delegate (object param) {
                    this.SetMainView((MainViewState) param);
                });
            }
        }

        public ICommand SettingsCommand
        {
            get
            {
                return new RelayCommand(delegate (object param) {
                    this.SetContentView(ContentViewState.SettingsPanel);
                });
            }
        }

        public bool ShowConnectingToDevice
        {
            get
            {
                return true;
            }
            set
            {
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
                return true;
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
                this.FirePropertyChanged("ShowDeviceChooser");
            }
        }

        public IStorageGaugeViewModel StorageGaugeViewModel { get; private set; }

        public ISyncProgressViewModel SyncProgressViewModel { get; private set; }
    }
}

