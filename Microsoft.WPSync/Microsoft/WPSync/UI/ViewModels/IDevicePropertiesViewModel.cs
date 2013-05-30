namespace Microsoft.WPSync.UI.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Windows.Media.Imaging;

    public interface IDevicePropertiesViewModel : INotifyPropertyChanged
    {
        void EnsureImageIsLoaded();

        string BatteryLevel { get; set; }

        string DeviceId { get; }

        BitmapImage Image { get; }

        string ManufacturerModelId { get; }

        string Name { get; set; }
    }
}

