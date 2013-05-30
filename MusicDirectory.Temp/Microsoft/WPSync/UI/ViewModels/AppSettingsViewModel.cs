namespace Microsoft.WPSync.UI.ViewModels
{
    using Microsoft.WPSync.Settings;
    using Microsoft.WPSync.Shared;
    using Microsoft.WPSync.Sync.Source.iTunes;
    using Microsoft.WPSync.UI;
    using Microsoft.WPSync.UI.Properties;
    using System;
    using System.Linq;
    using System.Windows;

    public class AppSettingsViewModel : PropChangeNotifier, ISettingsViewModel
    {
        private readonly IMainController controller;
        private string musicSyncSource;
        private bool playSoundOnSyncComplete;
        private bool sendSqmInfo;
        private bool showSyncErrors;
        private bool syncSourceITunesIsEnabled;

        public AppSettingsViewModel(IMainController controller)
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }
            this.controller = controller;
        }

        public void Commit()
        {
            string applicationSetting = (string) GlobalSetting.GetApplicationSetting("MusicSyncSource");
            if (applicationSetting != this.MusicSyncSource)
            {
                GlobalSetting.SetApplicationSetting("MusicSyncSource", this.MusicSyncSource);
                this.controller.ResetMusicSyncSourceType();
            }
            GlobalSetting.SetApplicationSetting("PlaySoundOnSyncComplete", this.PlaySoundOnSyncComplete);
            GlobalSetting.SetApplicationSetting("ShowSyncErrors", this.ShowSyncErrors);
            GlobalSetting.SetApplicationSetting("SendSqmInfo", this.SendSqmInfo);
        }

        public void Init()
        {
            bool? applicationSetting = (bool?) GlobalSetting.GetApplicationSetting("PlaySoundOnSyncComplete");
            this.PlaySoundOnSyncComplete = applicationSetting.HasValue ? applicationSetting.GetValueOrDefault() : false;
            bool? nullable2 = (bool?) GlobalSetting.GetApplicationSetting("ShowSyncErrors");
            this.ShowSyncErrors = nullable2.HasValue ? nullable2.GetValueOrDefault() : false;
            this.MusicSyncSource = (string) GlobalSetting.GetApplicationSetting("MusicSyncSource");
            bool? nullable3 = (bool?) GlobalSetting.GetApplicationSetting("SendSqmInfo");
            this.SendSqmInfo = nullable3.HasValue ? nullable3.GetValueOrDefault() : false;
            this.SyncSourceITunesIsEnabled = ITunesMusicSyncSource.IsITunesInstalled();
        }

        public bool Validate()
        {
            string applicationSetting = (string) GlobalSetting.GetApplicationSetting("MusicSyncSource");
            if (applicationSetting != this.MusicSyncSource)
            {
                if (!this.controller.CanResetMusicSyncSourceType())
                {
                    MessageBox.Show(Resources.CantSwitchSyncSourceText, Resources.CantSwitchSyncSourceTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return false;
                }
                if ((DeviceSettings.GetDeviceDirectories().FirstOrDefault<string>() != null) && (MessageBox.Show(Resources.SwitchSyncSourceWarningText, Resources.SwitchSyncSourceWarningTitle, MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.No))
                {
                    return false;
                }
            }
            return true;
        }

        private string MusicSyncSource
        {
            get
            {
                return this.musicSyncSource;
            }
            set
            {
                this.musicSyncSource = value;
                this.OnPropertyChanged("SyncSourceIsITunes");
                this.OnPropertyChanged("SyncSourceIsWindowsLibraries");
            }
        }

        public bool PlaySoundOnSyncComplete
        {
            get
            {
                return this.playSoundOnSyncComplete;
            }
            set
            {
                this.playSoundOnSyncComplete = value;
                this.OnPropertyChanged("PlaySoundOnSyncComplete");
            }
        }

        public bool SendSqmInfo
        {
            get
            {
                return this.sendSqmInfo;
            }
            set
            {
                this.sendSqmInfo = value;
                this.OnPropertyChanged("SendSqmInfo");
            }
        }

        public bool ShowSyncErrors
        {
            get
            {
                return this.showSyncErrors;
            }
            set
            {
                this.showSyncErrors = value;
                this.OnPropertyChanged("ShowSyncErrors");
            }
        }

        public bool SyncSourceIsITunes
        {
            get
            {
                return (this.MusicSyncSource == "ITunes");
            }
            set
            {
                if (value)
                {
                    this.MusicSyncSource = "ITunes";
                }
            }
        }

        public bool SyncSourceIsWindowsLibraries
        {
            get
            {
                return (this.MusicSyncSource == "WindowsLibraries");
            }
            set
            {
                if (value)
                {
                    this.MusicSyncSource = "WindowsLibraries";
                }
            }
        }

        public bool SyncSourceITunesIsEnabled
        {
            get
            {
                return this.syncSourceITunesIsEnabled;
            }
            set
            {
                this.syncSourceITunesIsEnabled = value;
                this.OnPropertyChanged("SyncSourceITunesIsEnabled");
            }
        }
    }
}

