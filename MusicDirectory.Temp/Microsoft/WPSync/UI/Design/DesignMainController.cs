namespace Microsoft.WPSync.UI.Design
{
    using Microsoft.WPSync.Device;
    using Microsoft.WPSync.Settings;
    using Microsoft.WPSync.Shared;
    using Microsoft.WPSync.UI;
    using Microsoft.WPSync.UI.Models;
    using Microsoft.WPSync.UI.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;
    using System.Windows.Threading;

    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    public class DesignMainController : PropChangeNotifier, IMainController, INotifyPropertyChanged
    {
        public DesignMainController()
        {
            this.CurrentDevice = new DesignDevice();
            DeviceSettings.InitSettingsModel(this.CurrentDevice.WinMoDeviceId);
            this.CurrentSyncPartnership = new DesignSyncPartnership();
            this.MainViewModel = new DesignMainViewModel(this);
            this.MainViewModel.DeviceViewModel.Device = this.CurrentDevice;
            this.ConnectedDevices = new List<IDevicePropertiesViewModel>();
            this.ConnectedDevices.Add(new DevicePropertiesViewModel(this.CurrentDevice));
            this.CurrentDeviceIndex = 0;
            DesignDevice device = new DesignDevice("Second Device");
            this.ConnectedDevices.Add(new DevicePropertiesViewModel(device));
        }

        public void CancelSync()
        {
            throw new NotImplementedException();
        }

        public bool CanResetMusicSyncSourceType()
        {
            throw new NotImplementedException();
        }

        public void DoSync(SyncStartType syncStartType)
        {
            throw new NotImplementedException();
        }

        public bool IsDevicePartnered(IDevice device)
        {
            return true;
        }

        public void OnClosing(CancelEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnKeyUp(KeyEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnStartup()
        {
            throw new NotImplementedException();
        }

        public void OpenBrowserLink(string link)
        {
            throw new NotImplementedException();
        }

        public bool PartnerWithPhone()
        {
            throw new NotImplementedException();
        }

        public void ResetMusicSyncSourceType()
        {
            throw new NotImplementedException();
        }

        public void SetCurrentDevice(IDevice device)
        {
            throw new NotImplementedException();
        }

        public void SetCurrentDeviceById(string deviceId)
        {
            throw new NotImplementedException();
        }

        public void SwitchToNextDevice()
        {
            throw new NotImplementedException();
        }

        public void UnpartnerWithPhone()
        {
            throw new NotImplementedException();
        }

        public IList<IDevicePropertiesViewModel> ConnectedDevices { get; private set; }

        public IDevice CurrentDevice { get; private set; }

        public int CurrentDeviceIndex { get; set; }

        public ISyncPartnership CurrentSyncPartnership { get; private set; }

        public System.Windows.Threading.Dispatcher Dispatcher { get; set; }

        public bool IsLoadingContent { get; set; }

        public bool IsSyncing { get; set; }

        public IMainViewModel MainViewModel { get; private set; }
    }
}

