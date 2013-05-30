using Microsoft.WPSync.Shared;
using Microsoft.WPSync.Sync.Engine;
using Microsoft.WPSync.Sync.Rules;
using Microsoft.WPSync.Sync.Source.WindowsLibraries;
using Microsoft.WPSync.UI.Models;

namespace MusicDirectory.WindowsStore
{
    class DependencyContainer
    {

        private static IShellFactory shellFactory;

        private static ISyncSelectionsModelFactory syncSelectionsModelFactory;

        private static IMusicSyncSource windowsLibraryMusicSource;

        private static IWindowsLibraryFactory windowsLibraryFactory;

        public static IMusicSyncSource WindowsLibraryMusicSource
        {
            get
            {
                return windowsLibraryMusicSource;
            }
        }

        private static IFileSystemHelper fileSystemHelper;


        public static ISyncSelectionsModel ResolveISyncSelectionsModel(ISyncSelectionsModelClient client)
        {
            return new SyncSelectionsModel(client);
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

        public static IFileSystemHelper ResolveIFileSystemHelper()
        {
            if (fileSystemHelper == null)
            {
                fileSystemHelper = new FileSystemHelper();
            }
            return fileSystemHelper;
        }

        public static IShellFactory ResolveIShellFactory()
        {
            if (shellFactory == null)
            {
                shellFactory = new ShellFactory();
            }
            return shellFactory;
        }

        public static IWindowsLibraryFactory ResolveIWindowsLibraryFactory()
        {
            if (windowsLibraryFactory == null)
            {
                windowsLibraryFactory = new WindowsLibraryFactory(ResolveIShellFactory());
            }
            return windowsLibraryFactory;
        }


        public static ISyncSelectionsModelFactory ResolveISyncSelectionsModelFactory()
        {
            if (syncSelectionsModelFactory == null)
            {
                syncSelectionsModelFactory = new SyncSelectionsModelFactory();
            }
            return syncSelectionsModelFactory;
        }

        public static ISyncEngine ResolveISyncEngine()
        {
            return new SyncEngine();
        }
    }
}
