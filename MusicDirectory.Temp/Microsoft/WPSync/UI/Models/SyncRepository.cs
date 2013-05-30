namespace Microsoft.WPSync.UI.Models
{
    using Microsoft.WPSync.Device;
    using Microsoft.WPSync.Logging;
    using Microsoft.WPSync.Sync.Engine;
    using Microsoft.WPSync.Sync.Rules;
    using Microsoft.WPSync.UI;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal class SyncRepository : ISyncRepository
    {
        public SyncRepository()
        {
            this.SyncPartnershipCache = new Dictionary<string, ISyncPartnership>();
        }

        private void AddSyncPartnershipForDevice(string deviceSerialNumber, ISyncPartnership partnership)
        {
            this.SyncPartnershipCache[deviceSerialNumber] = partnership;
        }

        public List<string> DeviceList()
        {
            return this.SyncPartnershipCache.Keys.ToList<string>();
        }

        public bool EnsureSyncPartnershipForDevice(IDevice device, out ISyncPartnership partnership)
        {
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }
            partnership = this.SyncPartnershipForDevice(device.WinMoDeviceId);
            if (partnership != null)
            {
                return false;
            }
            using (new OperationLogger())
            {
                partnership = DependencyContainer.ResolveISyncPartnership(device);
                this.AddSyncPartnershipForDevice(device.WinMoDeviceId, partnership);
            }
            return true;
        }

        ISyncPartnership ISyncRepository.SyncPartnershipForDevice(string deviceSerialNumber)
        {
            return (from p in this.SyncPartnershipCache.Values
                where p.Device.WinMoDeviceId == deviceSerialNumber
                select p).FirstOrDefault<ISyncPartnership>();
        }

        List<ISyncPartnership> ISyncRepository.SyncPartnershipList()
        {
            return this.SyncPartnershipCache.Values.ToList<ISyncPartnership>();
        }

        public IMusicLibraryProvider MusicLibraryForDevice(string deviceSerialNumber)
        {
            ISyncPartnership partnership = this.SyncPartnershipForDevice(deviceSerialNumber);
            if (partnership != null)
            {
                return partnership.MusicAndMovieProvider;
            }
            return null;
        }

        public IPictureLibraryProvider PictureLibraryForDevice(string deviceSerialNumber)
        {
            ISyncPartnership partnership = this.SyncPartnershipForDevice(deviceSerialNumber);
            if (partnership != null)
            {
                return partnership.PhotoAndVideoProvider;
            }
            return null;
        }

        public void RemoveSyncPartnership(string deviceSerialNumber)
        {
            if (this.SyncPartnershipCache.Keys.Contains<string>(deviceSerialNumber))
            {
                this.SyncPartnershipCache[deviceSerialNumber].Dispose();
                this.SyncPartnershipCache.Remove(deviceSerialNumber);
            }
        }

        public ISyncEngine SyncEngineForDevice(string deviceSerialNumber)
        {
            ISyncPartnership partnership = null;
            this.SyncPartnershipCache.TryGetValue(deviceSerialNumber, out partnership);
            if (partnership != null)
            {
                return partnership.Engine;
            }
            return null;
        }

        public List<ISyncEngine> SyncEngineList()
        {
            return (from p in this.SyncPartnershipCache.Values select p.Engine).ToList<ISyncEngine>();
        }

        public ISyncPartnership SyncPartnershipForDevice(string deviceSerialNumber)
        {
            ISyncPartnership partnership = null;
            this.SyncPartnershipCache.TryGetValue(deviceSerialNumber, out partnership);
            return partnership;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public List<ISyncPartnership> SyncPartnershipList()
        {
            return this.SyncPartnershipCache.Values.ToList<ISyncPartnership>();
        }

        public ISyncRules SyncRulesForDevice(string deviceSerialNumber)
        {
            ISyncPartnership partnership = null;
            this.SyncPartnershipCache.TryGetValue(deviceSerialNumber, out partnership);
            if (partnership != null)
            {
                return partnership.RuleManager;
            }
            return null;
        }

        private Dictionary<string, ISyncPartnership> SyncPartnershipCache { get; set; }
    }
}

