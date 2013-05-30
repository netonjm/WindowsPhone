namespace Microsoft.WPSync.UI.ViewModels
{
    using Microsoft.WPSync.Device;
    using Microsoft.WPSync.Settings;
    using Microsoft.WPSync.Shared;
    using Microsoft.WPSync.UI;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;

    public class DeviceSettingsViewModel : PropChangeNotifier, ISettingsViewModel
    {
        private readonly IMainController controller;
        private bool importPictures;
        private string name;
        private bool resizePhotos;
        private bool syncAutomatically;
        private string videoOptimizationStrategy;

        public DeviceSettingsViewModel(IMainController controller)
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }
            this.controller = controller;
        }

        public void Commit()
        {
            if (this.Device != null)
            {
                DeviceSettings.Set(this.Device.WinMoDeviceId, "SyncAutomatically", this.SyncAutomatically);
                DeviceSettings.Set(this.Device.WinMoDeviceId, "ImportPictures", this.ImportPictures);
                DeviceSettings.Set(this.Device.WinMoDeviceId, "ResizePhotos", this.ResizePhotos);
                DeviceSettings.Set(this.Device.WinMoDeviceId, "VideoOptimizationStrategy", this.VideoOptimizationStrategy);
                if (this.Device.Name != this.Name)
                {
                    this.Device.SetFriendlyName(this.Name);
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void EraseContent()
        {
            if (MessageBox.Show(MusicDirectory.WPF.Properties.Resources.EraseContentWarningText, MusicDirectory.WPF.Properties.Resources.EraseContentWarningTitle, MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                throw new NotImplementedException();
            }
        }

        public void Init()
        {
            IDevice device = this.Device;
            if (device != null)
            {
                if (!this.controller.IsDevicePartnered(device))
                {
                    this.SyncAutomatically = true;
                    this.ImportPictures = DeviceSettings.ImportPicturesDefault;
                    this.ResizePhotos = false;
                    this.VideoOptimizationStrategy = "Quality";
                }
                else
                {
                    this.SyncAutomatically = (bool) DeviceSettings.Get(device.WinMoDeviceId, "SyncAutomatically");
                    this.ImportPictures = (bool) DeviceSettings.Get(device.WinMoDeviceId, "ImportPictures");
                    this.ResizePhotos = (bool) DeviceSettings.Get(device.WinMoDeviceId, "ResizePhotos");
                    this.VideoOptimizationStrategy = (string) DeviceSettings.Get(device.WinMoDeviceId, "VideoOptimizationStrategy");
                }
                this.Name = device.Name;
            }
        }

        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(this.Name))
            {
                Errors.ShowError(MusicDirectory.WPF.Properties.Resources.MissingValueText, new object[0]);
                return false;
            }
            return true;
        }

        private IDevice Device
        {
            get
            {
                return this.controller.CurrentDevice;
            }
        }

        public bool ImportPictures
        {
            get
            {
                return this.importPictures;
            }
            set
            {
                this.importPictures = value;
                this.OnPropertyChanged("ImportPictures");
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                this.OnPropertyChanged("Name");
            }
        }

        public bool ResizePhotos
        {
            get
            {
                return this.resizePhotos;
            }
            set
            {
                this.resizePhotos = value;
                this.OnPropertyChanged("ResizePhotos");
            }
        }

        public bool SyncAutomatically
        {
            get
            {
                return this.syncAutomatically;
            }
            set
            {
                this.syncAutomatically = value;
                this.OnPropertyChanged("SyncAutomatically");
            }
        }

        public bool VideoOptimizationIsQuality
        {
            get
            {
                return (this.VideoOptimizationStrategy == "Quality");
            }
            set
            {
                if (value)
                {
                    this.VideoOptimizationStrategy = "Quality";
                }
            }
        }

        public bool VideoOptimizationIsSize
        {
            get
            {
                return (this.VideoOptimizationStrategy == "Size");
            }
            set
            {
                if (value)
                {
                    this.VideoOptimizationStrategy = "Size";
                }
            }
        }

        private string VideoOptimizationStrategy
        {
            get
            {
                return this.videoOptimizationStrategy;
            }
            set
            {
                this.videoOptimizationStrategy = value;
                this.OnPropertyChanged("VideoOptimizationIsQuality");
                this.OnPropertyChanged("VideoOptimizationIsSize");
            }
        }
    }
}

