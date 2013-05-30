namespace Microsoft.WPSync.UI
{
    using Microsoft.WPSync.Device;
    using Microsoft.WPSync.UI.Models;
    using Microsoft.WPSync.UI.Utilities;
    using Microsoft.WPSync.UI.ViewModels;
    using System;

    internal class MainFactory : IMainFactory
    {
        public IDeviceEnumerationListener CreateIDeviceEnumerationListener()
        {
            return DependencyContainer.ResolveIDeviceEnumerationListener();
        }

        public IDeviceManager CreateIDeviceManager()
        {
            return DependencyContainer.ResolveIDeviceManager();
        }

        public IErrorLogger CreateIErrorLogger()
        {
            return DependencyContainer.ResolveIErrorLogger();
        }

        //public IMainViewModel CreateIMainViewModel(IMainController controller)
        //{
        //    IViewModelFactory factory = DependencyContainer.ResolveIViewModelFactory();
        //    return DependencyContainer.ResolveIMainViewModel(controller, factory);
        //}

        public ISqmHelper CreateISqmHelper()
        {
            return DependencyContainer.ResolveISqmHelper();
        }

        public ISyncRepository CreateISyncRepository()
        {
            return DependencyContainer.ResolveISyncRepository();
        }

        public ISyncSourcePreloader CreateISyncSourcePreloader()
        {
            return DependencyContainer.ResolveISyncSourcePreloader();
        }
    }
}

