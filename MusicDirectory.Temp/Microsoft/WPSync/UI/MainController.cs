using Microsoft.WPSync.Sync.Rules;

namespace Microsoft.WPSync.UI
{
    using Microsoft.WPSync.Device;
    using Microsoft.WPSync.Logging;
    using Microsoft.WPSync.Settings;
    using Microsoft.WPSync.Shared;
    using Microsoft.WPSync.Sync.Engine;
    using Microsoft.WPSync.Sync.Source.iTunes;
    using Microsoft.WPSync.Sync.Source.Zmdb;
    using Microsoft.WPSync.UI.Models;
    using Microsoft.WPSync.UI.Properties;
    using Microsoft.WPSync.UI.Utilities;
    using Microsoft.WPSync.UI.ViewModels;
    using Microsoft.WPSync.UI.Views;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Management;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    public class MainController : PropChangeNotifier, IMainController, INotifyPropertyChanged, IPartnershipClient
    {
        private bool checkingConnectedDevices;
        private IList<IDevicePropertiesViewModel> connectedDevices = new List<IDevicePropertiesViewModel>();
        private IDevice currentDevice;
        private ISyncPartnership currentPartnership;
        private readonly IDeviceEnumerationListener deviceEnumerationListener;
        private readonly IDeviceManager deviceManager;
        private readonly IErrorLogger errorLogger;
        private object lockObjectForCheckingConnections = new object();
        private bool needToCheckDevicesAgain;
        private readonly ISyncSourcePreloader preloader;
        private readonly ISyncRepository repository;
        private readonly ISqmHelper sqmHelper;
        private List<string> uniquelyConnectedDeviceIds = new List<string>();
        private readonly IWindow view;

        public MainController(IWindow view, IMainFactory factory)
        {
            if (view == null)
            {
                throw new ArgumentNullException(typeof(IWindow).ToString());
            }
            this.view = view;
            this.Dispatcher = view.Window.Dispatcher;
            if (factory == null)
            {
                throw new ArgumentNullException(typeof(IMainFactory).ToString());
            }
            this.repository = factory.CreateISyncRepository();
            if (this.repository == null)
            {
                throw new ApplicationInitializationException(typeof(ISyncRepository).ToString());
            }
            this.MainViewModel = factory.CreateIMainViewModel(this);
            if (this.MainViewModel == null)
            {
                throw new ApplicationInitializationException(typeof(IMainViewModel).ToString());
            }
            this.deviceManager = factory.CreateIDeviceManager();
            if (this.deviceManager == null)
            {
                throw new ApplicationInitializationException(typeof(IDeviceManager).ToString());
            }
            this.deviceEnumerationListener = factory.CreateIDeviceEnumerationListener();
            if (this.deviceEnumerationListener == null)
            {
                throw new ApplicationInitializationException(typeof(IDeviceEnumerationListener).ToString());
            }
            this.errorLogger = factory.CreateIErrorLogger();
            if (this.errorLogger == null)
            {
                throw new ApplicationInitializationException(typeof(IErrorLogger).ToString());
            }
            this.sqmHelper = factory.CreateISqmHelper();
            if (this.sqmHelper == null)
            {
                throw new ApplicationInitializationException(typeof(ISqmHelper).ToString());
            }
            this.preloader = factory.CreateISyncSourcePreloader();
            if (this.preloader == null)
            {
                throw new ApplicationInitializationException(typeof(ISyncSourcePreloader).ToString());
            }
            bool flag = ((string) GlobalSetting.GetApplicationSetting("MusicSyncSource")) == "ITunes";
            bool flag2 = ((string) GlobalSetting.GetApplicationSetting("MusicSyncSource")) == "WindowsLibraries";
            if (!flag && !flag2)
            {
                GlobalSetting.SetApplicationSetting("MusicSyncSource", "WindowsLibraries");
            }
            bool flag3 = ITunesMusicSyncSource.IsITunesInstalled();
            if (flag && !flag3)
            {
                if (!((bool) GlobalSetting.GetApplicationSetting("FirstRun")))
                {
                    MessageBox.Show(Microsoft.WPSync.UI.Properties.Resources.iTunesMissingWillSwitchMessage, Microsoft.WPSync.UI.Properties.Resources.iTunesMissingWillSwitchTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                GlobalSetting.SetApplicationSetting("MusicSyncSource", "WindowsLibraries");
                this.MainViewModel.AppSettingsViewModel.Init();
            }
            this.deviceEnumerationListener.EventArrived += new EventArrivedEventHandler(this.OnConnectedDevicesChanged);
            GlobalSetting.SettingChange = (EventHandler<ApplicationSettingsChangeEventArgs>) Delegate.Combine(GlobalSetting.SettingChange, new EventHandler<ApplicationSettingsChangeEventArgs>(this.OnApplicationSettingChanged));
        }

        public void CancelSync()
        {
            using (new OperationLogger())
            {
                if ((this.CurrentSyncPartnership != null) && (this.CurrentSyncPartnership.CurrentState == PartnershipState.Syncing))
                {
                    this.CurrentSyncPartnership.Engine.CancelSync();
                }
            }
        }

        public bool CanResetMusicSyncSourceType()
        {
            foreach (SyncPartnership partnership in this.repository.SyncPartnershipList())
            {
                if ((partnership.CurrentState == PartnershipState.Syncing) || (partnership.CurrentState == PartnershipState.PreparingSync))
                {
                    return false;
                }
            }
            return true;
        }

        private void CheckConnectedDevices(bool InitialCheck = false)
        {
            lock (this.lockObjectForCheckingConnections)
            {
                if (this.checkingConnectedDevices)
                {
                    this.needToCheckDevicesAgain = true;
                    return;
                }
                this.checkingConnectedDevices = true;
                this.needToCheckDevicesAgain = false;
            }
        Label_0041:
            using (new OperationLogger())
            {
                int count = this.deviceManager.Devices.Count;
                DateTime now = DateTime.Now;
                do
                {
                    this.deviceManager.BuildDeviceList();
                }
                while ((!InitialCheck && (this.deviceManager.Devices.Count == count)) && ((DateTime.Now - now) < TimeSpan.FromMinutes(1.0)));
                this.MainViewModel.ShowConnectingToDevice = false;
                this.MainViewModel.MainWindowCursor = null;
                if (!this.needToCheckDevicesAgain)
                {
                    this.OnDeviceListChanged();
                }
            }
            lock (this.lockObjectForCheckingConnections)
            {
                if (!this.needToCheckDevicesAgain)
                {
                    this.checkingConnectedDevices = false;
                }
                else
                {
                    this.needToCheckDevicesAgain = false;
                    goto Label_0041;
                }
            }
        }

        private bool DoStorageCardChangedCheck()
        {
            return (((ZmdbSyncSource) this.CurrentSyncPartnership.Engine.Targets.First<ISyncSource>()).StorageCardHasChanged() && (MessageBox.Show(Microsoft.WPSync.UI.Properties.Resources.StorageCardHasChanged, Microsoft.WPSync.UI.Properties.Resources.ApplicationTitle, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel));
        }

        public void DoSync(SyncStartType syncStartType)
        {
            using (new OperationLogger())
            {
                ISyncEngine engine;
                SyncResult syncResult;
                if ((this.CurrentSyncPartnership == null) || !this.CurrentSyncPartnership.LockForSyncing())
                {
                    Logger.TraceInformation("Managed:UI", "Not syncing.  Partnership null or refused to sync.", syncStartType.ToString());
                }
                else if (this.DoStorageCardChangedCheck())
                {
                    Logger.TraceInformation("Managed:UI", "Not syncing.  User canceled due to sd card changed.", syncStartType.ToString());
                    this.CurrentSyncPartnership.UnlockForSyncing();
                }
                else
                {
                    engine = this.CurrentSyncPartnership.Engine;
                    new Thread(delegate {
                        using (new OperationLogger("MainController sync"))
                        {
                            Thread.CurrentThread.SetApartmentState(ApartmentState.MTA);
                            SyncType syncType = SyncType.Calculated;
                            if ((syncStartType != SyncStartType.AutoSync) && (syncStartType != SyncStartType.ManualSync))
                            {
                                syncType = SyncType.Specific;
                            }
                            this.CurrentSyncPartnership.SyncStartType = syncStartType;
                            syncResult = engine.CalculateSyncOperations(syncType);
                            if (syncResult == null)
                            {
                                DateTime now = DateTime.Now;
                                SyncStoppedReason reason = engine.DoSyncOperations();
                                TimeSpan syncDuration = (TimeSpan) (DateTime.Now - now);
                                if (reason != SyncStoppedReason.Paused)
                                {
                                    this.sqmHelper.CreateSyncContentStream(this.CurrentSyncPartnership, syncStartType, reason, syncDuration);
                                }
                            }
                            else
                            {
                                Logger.TraceInformation("Managed:UI", "Didn't do sync.  CalculateSyncOperations returned error.", syncResult.Details);
                            }
                        }
                    }).Start();
                }
            }
        }

        private bool EnsurePartnershipForDevice(IDevice device, out ISyncPartnership partnership)
        {
            bool flag = this.repository.EnsureSyncPartnershipForDevice(device, out partnership);
            if (flag)
            {
                partnership.PartnershipClient = this;
            }
            return flag;
        }

        public void HandleException(Exception exception)
        {
            if (exception != null)
            {
                this.errorLogger.LogException(exception);
            }
        }

        public void HandleSourceErrors(ISyncPartnership partnership, ICollection<SourceResult> errors)
        {
            if ((partnership != null) && ((bool) GlobalSetting.GetApplicationSetting("ShowSyncErrors")))
            {
                this.ShowSourceErrorsPanel(errors);
            }
        }

        public void HandleSyncErrors(ISyncPartnership partnership, ICollection<SyncResult> errors)
        {
            if ((partnership != null) && ((bool) GlobalSetting.GetApplicationSetting("ShowSyncErrors")))
            {
                this.ShowSyncErrorsPanel(errors);
            }
        }

        public bool IsDevicePartnered(IDevice device)
        {
            if (device == null)
            {
                return false;
            }
            return (DeviceSettings.SettingsFileExists(device.WinMoDeviceId) && device.IsPartnered());
        }

        private void OnApplicationSettingChanged(object src, ApplicationSettingsChangeEventArgs e)
        {
            if (e.SettingName == "SendSqmInfo")
            {
                this.sqmHelper.OnSqmOptinChanged((bool) e.NewValue);
            }
        }

        public void OnClosing(CancelEventArgs e)
        {
            if (e != null)
            {
                if (this.repository.SyncPartnershipList().Any<ISyncPartnership>(p => (p.CurrentState == PartnershipState.Syncing)) && (MessageBox.Show(Microsoft.WPSync.UI.Properties.Resources.CancelSyncText, Microsoft.WPSync.UI.Properties.Resources.CancelSyncTitle, MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.Yes) == MessageBoxResult.No))
                {
                    e.Cancel = true;
                }
                else
                {
                    GlobalSetting.SetApplicationSetting("FirstRun", false);
                    this.deviceEnumerationListener.Stop();
                    this.deviceEnumerationListener.Dispose();
                    foreach (ISyncPartnership partnership in this.repository.SyncPartnershipList())
                    {
                        int index = this.uniquelyConnectedDeviceIds.IndexOf(partnership.Device.WinMoDeviceId);
                        this.sqmHelper.CreateDeviceInfoStream(partnership, index);
                        partnership.PartnershipClient = null;
                        partnership.Dispose();
                    }
                    this.deviceManager.Dispose();
                    this.sqmHelper.EndSqmSession((uint) this.uniquelyConnectedDeviceIds.Count);
                    DependencyContainer.OnShutdown();
                }
            }
        }

        private void OnConnectedDevicesChanged(object sender, EventArrivedEventArgs e)
        {
            this.CheckConnectedDevices(false);
        }

        private void OnDeviceListChanged()
        {
            this.UpdateConnectedDeviceViewModel();
            foreach (IDevice device in this.deviceManager.Devices)
            {
                if (!string.IsNullOrWhiteSpace(device.WinMoDeviceId) && !this.uniquelyConnectedDeviceIds.Contains(device.WinMoDeviceId))
                {
                    this.uniquelyConnectedDeviceIds.Add(device.WinMoDeviceId);
                    this.sqmHelper.OnDeviceConnected(device, this.uniquelyConnectedDeviceIds.IndexOf(device.WinMoDeviceId));
                }
            }
            List<string> list = (from d in this.deviceManager.Devices select d.WinMoDeviceId).ToList<string>();
            foreach (string str in this.repository.DeviceList())
            {
                if (!list.Contains(str))
                {
                    int index = this.uniquelyConnectedDeviceIds.IndexOf(str);
                    this.sqmHelper.CreateDeviceInfoStream(this.repository.SyncPartnershipForDevice(str), index);
                    this.repository.RemoveSyncPartnership(str);
                    DeviceSettings.RemoveDeviceSettings(str);
                }
            }
            if ((this.CurrentDevice == null) || !list.Contains(this.CurrentDevice.WinMoDeviceId))
            {
                IDevice device2 = this.deviceManager.Devices.LastOrDefault<IDevice>();
                this.SetCurrentDevice(device2);
            }
        }

        public void OnDeviceLocked(ISyncPartnership partnership)
        {
        }

        protected void OnDeviceUnlockedEvent(object sender, EventArgs e)
        {
            IDevice device = (IDevice) sender;
            if (!device.CachedIsLocked)
            {
                device.DeviceUnlockedEvent -= new EventHandler(this.OnDeviceUnlockedEvent);
            }
        }

        public void OnKeyUp(KeyEventArgs e)
        {
            if (e != null)
            {
                switch (e.Key)
                {
                    case Key.L:
                        if ((Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl)) && (Keyboard.IsKeyDown(Key.RightAlt) || Keyboard.IsKeyDown(Key.LeftAlt)))
                        {
                            Logger.SaveOperationalLog(Path.Combine(GlobalSetting.SettingsDirectoryForApplication(), "wpsync_etw_log.xml"));
                        }
                        break;
                }
            }
        }

        public void OnStartup()
        {
            this.MainViewModel.ShowConnectingToDevice = true;
            this.MainViewModel.MainWindowCursor = Cursors.Wait;
            this.sqmHelper.InitSqm();
            Task.Factory.StartNew(delegate {
                this.CheckConnectedDevices(true);
            });
            Task.Factory.StartNew(delegate {
                this.preloader.PreloadSources();
            });
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void OpenBrowserLink(string link)
        {
            try
            {
                Process.Start(link);
            }
            catch (Win32Exception exception)
            {
                if (exception.ErrorCode == -2147467259)
                {
                    MessageBox.Show(exception.Message);
                }
            }
            catch (Exception exception2)
            {
                MessageBox.Show(exception2.Message);
            }
        }

        public bool PartnerWithPhone()
        {
            if ((this.CurrentDevice.SetSyncPartner() == ResultCode.Success) && !string.IsNullOrEmpty(this.CurrentDevice.SyncPartner))
            {
                DeviceSettings.EnsureSettingsForDevice(this.CurrentDevice.WinMoDeviceId);
                this.CurrentDevice.IsFirstConnect = true;
                this.ResetCurrentDeviceTo(this.CurrentDevice);
                return true;
            }
            return false;
        }

        [Conditional("PERF_TESTING"), SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void PerfTesting()
        {
            string title = this.view.Window.Title;
            this.view.Window.Title = "Perf testing...";
            try
            {
                for (int i = 0; i < 2; i++)
                {
                    this.ResetMusicSyncSourceType();
                }
            }
            catch
            {
            }
            this.view.Window.Title = title;
        }

        private static void ReloadPartnershipSources(ISyncPartnership partnership)
        {
            Action action = null;
            using (new OperationLogger("Partnership reloading sources"))
            {
                if (action == null)
                {
                    action = delegate {
                        partnership.LoadSourceContent();
                    };
                }
                Task.Factory.StartNew(action);
            }
        }

        private static void ReloadSourcesAndTargets(ISyncPartnership partnership)
        {
            ReloadPartnershipSources(partnership);
            ResetAndReloadPartnershipTargets(partnership);
        }

        private static void ResetAndReloadPartnershipTargets(ISyncPartnership partnership)
        {
            Action action = null;
            using (new OperationLogger("Partnership targets resetting and reloading"))
            {
                if (action == null)
                {
                    action = delegate {
                        partnership.ResetAndReloadTargets();
                    };
                }
                Task.Factory.StartNew(action);
            }
        }

        private void ResetCurrentDeviceTo(IDevice device)
        {
            Action action = null;
            using (new OperationLogger())
            {
                if ((device != null) && device.RefreshLockStatus())
                {
                    device.DeviceUnlockedEvent += new EventHandler(this.OnDeviceUnlockedEvent);
                    if (action == null)
                    {
                        action = delegate {
                            device.NotifyWhenUnlocked();
                        };
                    }
                    Task.Factory.StartNew(action);
                }
                bool flag = this.IsDevicePartnered(device);
                if (flag)
                {
                    DeviceSettings.EnsureSettingsForDevice(device.WinMoDeviceId);
                }
                Trace.WriteLine("\n\n");
                this.CurrentDevice = device;
                if (flag)
                {
                    ISyncPartnership partnership;
                    this.EnsurePartnershipForDevice(this.CurrentDevice, out partnership);
                    this.CurrentSyncPartnership = partnership;
                    ReloadSourcesAndTargets(partnership);
                }
                else
                {
                    this.CurrentSyncPartnership = null;
                }
                this.OnPropertyChanged("CurrentDeviceIndex");
            }
        }

        public void ResetMusicSyncSourceType()
        {
            foreach (SyncPartnership partnership in this.repository.SyncPartnershipList())
            {
                Task.Factory.StartNew(delegate (object obj) {
                    using (new OperationLogger("Music library reset"))
                    {
                        ((ISyncPartnership) obj).OnSystemMusicSyncSourceChanged();
                    }
                }, partnership);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId="0")]
        public void SetCurrentDevice(IDevice device)
        {
            if (device != this.CurrentDevice)
            {
                if (this.CurrentDevice != null)
                {
                    this.CurrentDevice.DeviceUnlockedEvent -= new EventHandler(this.OnDeviceUnlockedEvent);
                }
                this.ResetCurrentDeviceTo(device);
            }
        }

        public void SetCurrentDeviceById(string deviceId)
        {
            IDevice device = (from d in this.deviceManager.Devices
                where d.WinMoDeviceId == deviceId
                select d).FirstOrDefault<IDevice>();
            if (device != null)
            {
                this.SetCurrentDevice(device);
            }
        }

        public void ShowSourceErrorsPanel(ICollection<SourceResult> sourceErrors)
        {
            ErrorsViewModel viewModel = new ErrorsViewModel(sourceErrors.Cast<IOperationResult>());
            this.view.Window.Dispatcher.Invoke(delegate {
                this.view.ShowDialogWithModel(new SyncErrorsPanel(viewModel), DialogType.Modal, viewModel);
            }, new object[0]);
        }

        public void ShowSyncErrorsPanel(ICollection<SyncResult> syncErrors)
        {
            ErrorsViewModel viewModel = new ErrorsViewModel(syncErrors.Cast<IOperationResult>());
            this.view.Window.Dispatcher.Invoke(delegate {
                this.view.ShowDialogWithModel(new SyncErrorsPanel(viewModel), DialogType.Modal, viewModel);
            }, new object[0]);
        }

        public void SwitchToNextDevice()
        {
            int index = this.deviceManager.Devices.IndexOf(this.currentDevice);
            int num2 = ((index + 1) < this.deviceManager.Devices.Count) ? (index + 1) : 0;
            if (this.deviceManager.Devices.Count > 0)
            {
                this.SetCurrentDevice(this.deviceManager.Devices[num2]);
            }
        }

        public void UnpartnerWithPhone()
        {
            if (((this.CurrentSyncPartnership.CurrentState == PartnershipState.Idle) && (MessageBox.Show(Microsoft.WPSync.UI.Properties.Resources.ForgetDeviceWarningText, Microsoft.WPSync.UI.Properties.Resources.ForgetDeviceWarningTitle, MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.Yes)) && (this.CurrentDevice != null))
            {
                if (DeviceSettings.ForgetDeviceSettings(this.CurrentDevice.WinMoDeviceId))
                {
                    this.repository.RemoveSyncPartnership(this.CurrentDevice.WinMoDeviceId);
                    this.MainViewModel.SetMainView(MainViewState.FirstConnectPanel);
                    this.MainViewModel.NextDeviceCommand.Execute(null);
                }
                else
                {
                    Errors.ShowError(Microsoft.WPSync.UI.Properties.Resources.ForgetPhoneFilesError, new object[0]);
                }
            }
        }

        private void UpdateConnectedDeviceViewModel()
        {
            this.Dispatcher.Invoke(delegate {
                this.ConnectedDevices = (from d in this.deviceManager.Devices select new DevicePropertiesViewModel(d)).ToList<IDevicePropertiesViewModel>();
                this.OnPropertyChanged("CurrentDeviceIndex");
            }, new object[0]);
        }

        public IList<IDevicePropertiesViewModel> ConnectedDevices
        {
            get
            {
                return this.connectedDevices;
            }
            private set
            {
                this.connectedDevices = value;
                this.OnPropertyChanged("ConnectedDevices");
            }
        }

        public IDevice CurrentDevice
        {
            get
            {
                return this.currentDevice;
            }
            private set
            {
                this.currentDevice = value;
                this.OnPropertyChanged("CurrentDevice");
            }
        }

        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId="Microsoft.WPSync.Logging.Logger.TraceWarning(System.String,System.String,System.String)")]
        public int CurrentDeviceIndex
        {
            get
            {
                for (int i = 0; i < this.connectedDevices.Count; i++)
                {
                    if ((this.currentDevice != null) && (this.connectedDevices[i].DeviceId == this.currentDevice.WinMoDeviceId))
                    {
                        return i;
                    }
                }
                return -1;
            }
            set
            {
                if (value != this.CurrentDeviceIndex)
                {
                    string deviceId = null;
                    try
                    {
                        deviceId = this.ConnectedDevices[value].DeviceId;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Logger.TraceWarning("Managed:UI", "Fail to switch devices. Request index is out of range.", this.CurrentDeviceIndex.ToString(CultureInfo.InvariantCulture));
                    }
                    this.SetCurrentDeviceById(deviceId);
                    this.OnPropertyChanged("CurrentDeviceIndex");
                }
            }
        }

        public ISyncPartnership CurrentSyncPartnership
        {
            get
            {
                return this.currentPartnership;
            }
            set
            {
                if (this.currentPartnership != value)
                {
                    if (this.currentPartnership != null)
                    {
                        this.currentPartnership.IsCurrent = false;
                    }
                    this.currentPartnership = value;
                    if (this.currentPartnership != null)
                    {
                        this.currentPartnership.IsCurrent = true;
                    }
                    this.OnPropertyChanged("CurrentSyncPartnership");
                }
            }
        }

        public System.Windows.Threading.Dispatcher Dispatcher { get; set; }

        public IMainViewModel MainViewModel { get; private set; }
    }
}

