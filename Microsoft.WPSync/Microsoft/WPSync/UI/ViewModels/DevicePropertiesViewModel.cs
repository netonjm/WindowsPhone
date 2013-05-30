namespace Microsoft.WPSync.UI.ViewModels
{
    using Microsoft.WPSync.Device;
    using Microsoft.WPSync.Settings;
    using Microsoft.WPSync.Shared;
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Windows.Media.Imaging;

    public class DevicePropertiesViewModel : PropChangeNotifier, IDevicePropertiesViewModel, INotifyPropertyChanged
    {
        private static BitmapImage battery0Image = new BitmapImage(new Uri("pack://application:,,,/resources/Battery0.png"));
        private static BitmapImage battery100Image = new BitmapImage(new Uri("pack://application:,,,/resources/Battery100.png"));
        private static BitmapImage battery25Image = new BitmapImage(new Uri("pack://application:,,,/resources/Battery25.png"));
        private static BitmapImage battery50Image = new BitmapImage(new Uri("pack://application:,,,/resources/Battery50.png"));
        private static BitmapImage battery75Image = new BitmapImage(new Uri("pack://application:,,,/resources/Battery75.png"));
        private string batteryLevel;
        private BitmapImage batteryLevelImage;
        private static BitmapImage defaultImage = new BitmapImage(new Uri("pack://application:,,,/resources/DeviceImage_200.png"));
        private IDevice device;
        private BitmapImage image;
        public const string PhoneImageFileName = "DeviceImage_200.png";

        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DevicePropertiesViewModel(IDevice device)
        {
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }
            this.device = device;
            device.PropertyChanged += new PropertyChangedEventHandler(this.OnDevicePropertyChanged);
            this.EnsureImageIsLoaded();
            this.UpdateBatteryLevel();
        }

        public void EnsureImageIsLoaded()
        {
            if (this.device == null)
            {
                this.Image = null;
            }
            else if ((this.Image == null) || (this.Image == defaultImage))
            {
                if (this.device.WinMoDeviceId != null)
                {
                    string settingsDirectoryForDevice = DeviceSettings.GetSettingsDirectoryForDevice(this.device.WinMoDeviceId, true);
                    string path = Path.Combine(settingsDirectoryForDevice, "DeviceImage_200.png");
                    if (!File.Exists(path))
                    {
                        this.device.LoadDeviceImagesToDirectory(settingsDirectoryForDevice);
                    }
                    if (File.Exists(path))
                    {
                        this.Image = new BitmapImage();
                        this.Image.BeginInit();
                        this.Image.CacheOption = BitmapCacheOption.OnLoad;
                        this.Image.UriSource = new Uri(path, UriKind.Absolute);
                        this.Image.EndInit();
                        this.Image.Freeze();
                        return;
                    }
                }
                this.Image = defaultImage;
            }
        }

        private void OnDevicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName;
            if (propertyName != null)
            {
                if (!(propertyName == "Name"))
                {
                    if (!(propertyName == "BatteryLevel"))
                    {
                        return;
                    }
                }
                else
                {
                    this.OnPropertyChanged("Name");
                    return;
                }
                this.UpdateBatteryLevel();
            }
        }

        public void ResetBatteryLevelImage()
        {
            if ((this.device == null) || (this.device.BatteryLevel <= 0))
            {
                this.BatteryLevelImage = null;
            }
            else if (this.device.BatteryLevel < 13)
            {
                this.BatteryLevelImage = battery0Image;
            }
            else if (this.device.BatteryLevel < 0x26)
            {
                this.BatteryLevelImage = battery25Image;
            }
            else if (this.device.BatteryLevel < 0x3f)
            {
                this.BatteryLevelImage = battery50Image;
            }
            else if (this.device.BatteryLevel < 0x58)
            {
                this.BatteryLevelImage = battery75Image;
            }
            else
            {
                this.BatteryLevelImage = battery100Image;
            }
        }

        private void UpdateBatteryLevel()
        {
            if (this.device != null)
            {
                if (this.device.BatteryLevel == 0)
                {
                    this.BatteryLevel = "";
                }
                else
                {
                    this.BatteryLevel = string.Format(CultureInfo.CurrentUICulture, "{0:P0}", new object[] { ((float) this.device.BatteryLevel) / 100f });
                }
            }
            this.ResetBatteryLevelImage();
        }

        public string BatteryLevel
        {
            get
            {
                return this.batteryLevel;
            }
            set
            {
                this.batteryLevel = value;
                this.OnPropertyChanged("BatteryLevel");
            }
        }

        public BitmapImage BatteryLevelImage
        {
            get
            {
                return this.batteryLevelImage;
            }
            set
            {
                this.batteryLevelImage = value;
                this.OnPropertyChanged("BatteryLevelImage");
            }
        }

        public string DeviceId
        {
            get
            {
                if (this.device != null)
                {
                    return this.device.WinMoDeviceId;
                }
                return null;
            }
        }

        public BitmapImage Image
        {
            get
            {
                return this.image;
            }
            set
            {
                this.image = value;
                this.OnPropertyChanged("Image");
            }
        }

        public string ManufacturerModelId
        {
            get
            {
                if (this.device != null)
                {
                    return this.device.ManufacturerModelID;
                }
                return null;
            }
        }

        public string Name
        {
            get
            {
                if (this.device == null)
                {
                    return null;
                }
                if ((this.device.Name != null) && (this.device.Name.Length > 0x20))
                {
                    return (this.device.Name.Substring(0, 0x20) + "...");
                }
                return this.device.Name;
            }
            set
            {
                if (this.device != null)
                {
                    this.device.SetFriendlyName(value);
                }
            }
        }
    }
}

