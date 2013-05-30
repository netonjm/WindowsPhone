namespace Microsoft.WPSync.UI.Design
{
    using Microsoft.WPSync.Device;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public class DesignDevice : BaseDevice
    {
        public DesignDevice() : this("Design Device")
        {
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public DesignDevice(string name)
        {
            base.Name = name;
            base.BatteryLevel = 50;
            base.PhoneNumber = "(012) 345-6789";
            base.WinMoDeviceId = "ABCDEF123456";
            base.Manufacturer = "Faux";
            base.Description = "3000";
            StorageInfo info = new StorageInfo {
                TotalBytes = 0x2710L,
                UsedStorageMusic = 500L,
                UsedStoragePictures = 0x5dcL,
                UsedStoragePodcasts = 300L,
                UsedStorageVideos = 500L,
                UsedStorageApps = 0x3e8L,
                UsedStorageSystem = 0x3e8L,
                FreeBytes = 0x1388L
            };
            base.StorageDevices = new Dictionary<string, StorageInfo>();
            base.StorageDevices["built-in"] = info;
            info = new StorageInfo {
                TotalBytes = 0x2710L,
                UsedStorageMusic = 500L,
                UsedStoragePictures = 0x5dcL,
                UsedStoragePodcasts = 300L,
                UsedStorageVideos = 500L,
                UsedStorageApps = 0x3e8L,
                UsedStorageSystem = 0x3e8L,
                FreeBytes = 0x1388L
            };
            base.StorageDevices["sd card"] = info;
        }
    }
}

