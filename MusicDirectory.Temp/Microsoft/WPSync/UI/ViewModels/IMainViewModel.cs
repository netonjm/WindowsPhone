namespace Microsoft.WPSync.UI.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Windows.Input;

    public interface IMainViewModel : INotifyPropertyChanged
    {
        void OnUserChangingMainView(MainViewState newState);
        void SetContentView(ContentViewState contentView);
        void SetMainView(MainViewState viewState);

        Microsoft.WPSync.UI.ViewModels.AppSettingsViewModel AppSettingsViewModel { get; }

        ICommand CancelSyncCommand { get; }

        ICommand CommitSettingsCommand { get; }

        IMediaContentViewModel ContentViewModel { get; }

        ContentViewState CurrentContentViewState { get; }

        MainViewState CurrentViewState { get; }

        ICommand DeleteCommand { get; }

        ICommand DeviceChooserCommand { get; }

        Microsoft.WPSync.UI.ViewModels.DeviceSettingsViewModel DeviceSettingsViewModel { get; }

        IDeviceViewModel DeviceViewModel { get; }

        ICommand DoSyncCommand { get; }

        ICommand EraseContentCommand { get; }

        string FilterString { get; set; }

        ICommand FindYourPhoneCommand { get; }

        ICommand ForgetPhoneCommand { get; }

        ICommand HelpCommand { get; }

        Cursor MainWindowCursor { get; set; }

        ICommand NextDeviceCommand { get; }

        ICommand SaveToPCCommand { get; }

        ICommand SetContentViewCommand { get; }

        ICommand SetMainViewCommand { get; }

        ICommand SettingsCommand { get; }

        bool ShowConnectingToDevice { get; set; }

        bool ShowContentFooter { get; }

        bool ShowContentHeader { get; }

        bool ShowDeviceChooser { get; set; }

        IStorageGaugeViewModel StorageGaugeViewModel { get; }

        ISyncProgressViewModel SyncProgressViewModel { get; }
    }
}

