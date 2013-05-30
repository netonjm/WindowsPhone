namespace Microsoft.WPSync.UI
{
    using Microsoft.WPSync.UI.ViewModels;
    using System;

    internal class ViewModelFactory : IViewModelFactory
    {
        public AppSettingsViewModel CreateAppSettingsViewModel(IMainController controller)
        {
            return DependencyContainer.ResolveAppSettingsViewModel(controller);
        }

        public DeviceSettingsViewModel CreateDeviceSettingsViewModel(IMainController controller)
        {
            return DependencyContainer.ResolveDeviceSettingsViewModel(controller);
        }

        public IDeviceViewModel CreateIDeviceViewModel(IMainController controller)
        {
            return DependencyContainer.ResolveIDeviceViewModel(controller);
        }

        public IMediaContentViewModel CreateIMediaContentViewModel(IMainController controller)
        {
            return DependencyContainer.ResolveIMediaContentViewModel(controller);
        }

        public IStorageGaugeViewModel CreateIStorageGaugeViewModel(IMainController controller)
        {
            return DependencyContainer.ResolveIStorageGaugeViewModel(controller);
        }

        public ISyncProgressViewModel CreateISyncProgressViewModel(IMainController controller)
        {
            return DependencyContainer.ResolveISyncProgressViewModel(controller);
        }
    }
}

