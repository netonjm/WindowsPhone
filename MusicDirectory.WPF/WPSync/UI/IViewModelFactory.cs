namespace Microsoft.WPSync.UI
{
    using Microsoft.WPSync.UI.ViewModels;

    public interface IViewModelFactory
    {
        AppSettingsViewModel CreateAppSettingsViewModel(IMainController controller);
        DeviceSettingsViewModel CreateDeviceSettingsViewModel(IMainController controller);
        IDeviceViewModel CreateIDeviceViewModel(IMainController controller);
        IMediaContentViewModel CreateIMediaContentViewModel(IMainController controller);
        IStorageGaugeViewModel CreateIStorageGaugeViewModel(IMainController controller);
        ISyncProgressViewModel CreateISyncProgressViewModel(IMainController controller);
    }
}

