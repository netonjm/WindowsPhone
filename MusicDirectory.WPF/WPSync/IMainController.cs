namespace Microsoft.WPSync.UI
{
    using Microsoft.WPSync.Device;
    using Microsoft.WPSync.UI.Models;
    using Microsoft.WPSync.UI.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Input;
    using System.Windows.Threading;

    public interface IMainController : INotifyPropertyChanged
    {
        void CancelSync();
        bool CanResetMusicSyncSourceType();
        void DoSync(SyncStartType syncStartType);
        bool IsDevicePartnered(IDevice device);
        void OnClosing(CancelEventArgs e);
        void OnKeyUp(KeyEventArgs e);
        void OnStartup();
        void OpenBrowserLink(string link);
        bool PartnerWithPhone();
        void ResetMusicSyncSourceType();
        void SetCurrentDevice(IDevice device);
        void SetCurrentDeviceById(string deviceId);
        void SwitchToNextDevice();
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Unpartner")]
        void UnpartnerWithPhone();

        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        IList<IDevicePropertiesViewModel> ConnectedDevices { get; }

        IDevice CurrentDevice { get; }

        int CurrentDeviceIndex { get; set; }

        ISyncPartnership CurrentSyncPartnership { get; }

        System.Windows.Threading.Dispatcher Dispatcher { get; }

        //IMainViewModel MainViewModel { get; }
    }
}

