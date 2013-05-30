namespace Microsoft.WPSync.UI
{
    using Microsoft.WPSync.Device;
    using Microsoft.WPSync.Logging;
    using Microsoft.WPSync.Settings;
    using Microsoft.WPSync.Shared;
    using Microsoft.WPSync.Shared.Sqm;
    using Microsoft.WPSync.Sync.Engine;
    using Microsoft.WPSync.Sync.Rules;
    using Microsoft.WPSync.Sync.Source.iTunes;
    using Microsoft.WPSync.Sync.Source.WindowsLibraries;
    using Microsoft.WPSync.Sync.Source.Zmdb;
    using Microsoft.WPSync.UI.Models;
    using Microsoft.WPSync.UI.Utilities;
    using Microsoft.WPSync.UI.ViewModels;
    using System;
    using System.Threading;
    using System.Windows;

    public static class DependencyContainer
    {
        private static AppSettingsViewModel appSettingsViewModel;
        private static DeviceSettingsViewModel deviceSettingsViewModel;
        private static IDeviceViewModel deviceViewModel;
        private static IErrorLogger errorLogger;
        private static IFileSystemHelper fileSystemHelper;
        private static IMusicSyncSource iTunesMusicSource;
        private static IDeviceEnumerationListener listener;
        private static MainControllerFactory mainControllerFactory;
        private static IMainFactory mainFactory;
        private static IWindow mainView;
        private static IMainViewModel mainViewModel;
        private static IMediaContentViewModel mediaContentViewModel;
        private static IPictureSyncSource photoSource;
        private static ISyncRepository repository;
        private static IShellFactory shellFactory;
        private static ISqmHelper sqmHelper;
        private static ISqmManager sqmManager;
        private static IStorageGaugeViewModel storageGaugeViewModel;
        private static ISyncFactory syncFactory;
        private static ISyncProgressViewModel syncProgressViewModel;
        private static ISyncSelectionsModelFactory syncSelectionsModelFactory;
        private static ISyncSourcePreloader syncSourcePreloader;
        private static IViewModelFactory viewModelFactory;
        private static IWindowsLibraryFactory windowsLibraryFactory;
        private static IMusicSyncSource windowsLibraryMusicSource;

        public static void OnShutdown()
        {
            if (iTunesMusicSource != null)
            {
                iTunesMusicSource.Dispose();
            }
            if (windowsLibraryMusicSource != null)
            {
                windowsLibraryMusicSource.Dispose();
            }
            if (photoSource != null)
            {
                photoSource.Dispose();
            }
        }

        public static AppSettingsViewModel ResolveAppSettingsViewModel(IMainController controller)
        {
            if (appSettingsViewModel == null)
            {
                appSettingsViewModel = new AppSettingsViewModel(controller);
            }
            return appSettingsViewModel;
        }

        public static DeviceSettingsViewModel ResolveDeviceSettingsViewModel(IMainController controller)
        {
            if (deviceSettingsViewModel == null)
            {
                deviceSettingsViewModel = new DeviceSettingsViewModel(controller);
            }
            return deviceSettingsViewModel;
        }

        public static IDeviceEnumerationListener ResolveIDeviceEnumerationListener()
        {
            if (listener == null)
            {
                listener = new DeviceEnumerationListener();
            }
            return listener;
        }

        public static IDeviceManager ResolveIDeviceManager()
        {
            IDeviceManager manager = null;
            Thread thread = new Thread( () => {
                Thread.CurrentThread.SetApartmentState(ApartmentState.MTA);
                try
                {
//GlobalSetting

                    var tmp = GlobalSetting.GetApplicationSetting("MockDeviceCount");
                    
                    manager = new DeviceManager(0);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.ToString());
                    throw;
                }
            });
            thread.Start();
            thread.Join();
            return manager;
        }

        public static IDeviceViewModel ResolveIDeviceViewModel(IMainController controller)
        {
            if (deviceViewModel == null)
            {
                deviceViewModel = new DeviceViewModel(controller);
            }
            return deviceViewModel;
        }

        public static IErrorLogger ResolveIErrorLogger()
        {
            if (errorLogger == null)
            {
                errorLogger = new ErrorLogger();
            }
            return errorLogger;
        }

        public static IFileSystemHelper ResolveIFileSystemHelper()
        {
            if (fileSystemHelper == null)
            {
                fileSystemHelper = new FileSystemHelper();
            }
            return fileSystemHelper;
        }

        public static IMainFactory ResolveIMainFactory()
        {
            if (mainFactory == null)
            {
                mainFactory = new MainFactory();
            }
            return mainFactory;
        }

        public static IMainViewModel ResolveIMainViewModel(IMainController controller, IViewModelFactory factory)
        {
            if (mainViewModel == null)
            {
                mainViewModel = new MainViewModel(controller, factory);
            }
            return mainViewModel;
        }

        public static IWindow ResolveIMainWindow()
        {
            if (mainView == null)
            {
                using (new OperationLogger())
                {
                    MainWindow window = new MainWindow();
                    MainControllerFactory factory = ResolveMainControllerFactory();
                    mainView = new MainWindowAdapter(window, factory);
                }
            }
            return mainView;
        }

        public static IMediaContentViewModel ResolveIMediaContentViewModel(IMainController controller)
        {
            if (mediaContentViewModel == null)
            {
                mediaContentViewModel = new MediaContentViewModel(controller);
            }
            return mediaContentViewModel;
        }

        public static IShellFactory ResolveIShellFactory()
        {
            if (shellFactory == null)
            {
                shellFactory = new ShellFactory();
            }
            return shellFactory;
        }

        public static ISqmHelper ResolveISqmHelper()
        {
            if (sqmHelper == null)
            {
                sqmHelper = new SqmHelper(ResolveISqmManager());
            }
            return sqmHelper;
        }

        public static ISqmManager ResolveISqmManager()
        {
            if (sqmManager == null)
            {
                sqmManager = new SqmManager();
            }
            return sqmManager;
        }

        public static IStorageGaugeViewModel ResolveIStorageGaugeViewModel(IMainController controller)
        {
            if (storageGaugeViewModel == null)
            {
                storageGaugeViewModel = new StorageGaugeViewModel(controller);
            }
            return storageGaugeViewModel;
        }

        public static ISyncEngine ResolveISyncEngine()
        {
            return new SyncEngine();
        }

        public static ISyncFactory ResolveISyncFactory()
        {
            if (syncFactory == null)
            {
                syncFactory = new SyncFactory();
            }
            return syncFactory;
        }

        public static ISyncPartnership ResolveISyncPartnership(IDevice device)
        {
            return new SyncPartnership(device, ResolveISyncFactory());
        }

        public static ISyncProgressViewModel ResolveISyncProgressViewModel(IMainController controller)
        {
            if (syncProgressViewModel == null)
            {
                syncProgressViewModel = new SyncProgressViewModel(controller);
            }
            return syncProgressViewModel;
        }

        public static ISyncRepository ResolveISyncRepository()
        {
            if (repository == null)
            {
                repository = new SyncRepository();
            }
            return repository;
        }

        public static ISyncRulesManager ResolveISyncRulesManager(string deviceId)
        {
            return new SyncRulesManager(deviceId);
        }

        public static ISyncSelectionsModel ResolveISyncSelectionsModel(ISyncSelectionsModelClient client)
        {
            return new SyncSelectionsModel(client);
        }

        public static ISyncSelectionsModelFactory ResolveISyncSelectionsModelFactory()
        {
            if (syncSelectionsModelFactory == null)
            {
                syncSelectionsModelFactory = new SyncSelectionsModelFactory();
            }
            return syncSelectionsModelFactory;
        }

        public static ISyncSourcePreloader ResolveISyncSourcePreloader()
        {
            if (syncSourcePreloader == null)
            {
                syncSourcePreloader = new SyncSourcePreloader();
            }
            return syncSourcePreloader;
        }

        public static IMusicSyncSource ResolveITunesMusicSyncSource()
        {
            if (iTunesMusicSource == null)
            {
                ISyncSelectionsModelFactory factory = ResolveISyncSelectionsModelFactory();
                IFileSystemHelper fileHelper = ResolveIFileSystemHelper();
                iTunesMusicSource = new ITunesMusicSyncSource(factory, fileHelper);
            }
            return iTunesMusicSource;
        }

        public static IViewModelFactory ResolveIViewModelFactory()
        {
            if (viewModelFactory == null)
            {
                viewModelFactory = new ViewModelFactory();
            }
            return viewModelFactory;
        }

        public static IWindowsLibraryFactory ResolveIWindowsLibraryFactory()
        {
            if (windowsLibraryFactory == null)
            {
                windowsLibraryFactory = new WindowsLibraryFactory(ResolveIShellFactory());
            }
            return windowsLibraryFactory;
        }

        public static MainControllerFactory ResolveMainControllerFactory()
        {
            if (mainControllerFactory == null)
            {
                mainControllerFactory = new MainControllerFactory(ResolveIMainFactory());
            }
            return mainControllerFactory;
        }

        public static IPictureSyncSource ResolvePhotosSyncSource()
        {
            if (photoSource == null)
            {
                ISyncSelectionsModelFactory modelFactory = ResolveISyncSelectionsModelFactory();
                IWindowsLibraryFactory winLibFactory = ResolveIWindowsLibraryFactory();
                IFileSystemHelper fileHelper = ResolveIFileSystemHelper();
                photoSource = new WindowsLibraryPictureSyncSource(modelFactory, fileHelper, winLibFactory);
            }
            return photoSource;
        }

        public static IMusicSyncSource ResolveWindowsLibraryMusicSyncSource()
        {
            if (windowsLibraryMusicSource == null)
            {
                ISyncSelectionsModelFactory modelFactory = ResolveISyncSelectionsModelFactory();
                IWindowsLibraryFactory winLibFactory = ResolveIWindowsLibraryFactory();
                IFileSystemHelper fileHelper = ResolveIFileSystemHelper();
                windowsLibraryMusicSource = new WindowsLibraryMusicSyncSource(modelFactory, fileHelper, winLibFactory);
            }
            return windowsLibraryMusicSource;
        }

        public static IZmdbSyncSource ResolveZmdbSyncSource(IDevice device)
        {
            ISyncSelectionsModelFactory factory = ResolveISyncSelectionsModelFactory();
            return new ZmdbSyncSource(device, factory, ResolveIFileSystemHelper());
        }

        public static IMusicSyncSource ITunesMusicSource
        {
            get
            {
                return iTunesMusicSource;
            }
        }

        public static IPictureSyncSource PhotoSource
        {
            get
            {
                return photoSource;
            }
        }

        public static IMusicSyncSource WindowsLibraryMusicSource
        {
            get
            {
                return windowsLibraryMusicSource;
            }
        }
    }
}

