namespace Microsoft.WPSync.UI
{
    using Microsoft.WPSync.Device;
    using Microsoft.WPSync.UI.Models;
    using Microsoft.WPSync.UI.Utilities;
    using Microsoft.WPSync.UI.ViewModels;

    public interface IMainFactory
    {
        IDeviceEnumerationListener CreateIDeviceEnumerationListener();
        IDeviceManager CreateIDeviceManager();
        IErrorLogger CreateIErrorLogger();
        IMainViewModel CreateIMainViewModel(IMainController controller);
        ISqmHelper CreateISqmHelper();
        ISyncRepository CreateISyncRepository();
        ISyncSourcePreloader CreateISyncSourcePreloader();
    }
}

