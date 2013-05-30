namespace Microsoft.WPSync.UI.Utilities
{
    using Microsoft.WPSync.Device;
    using Microsoft.WPSync.ITunesWrapper;
    using Microsoft.WPSync.Settings;
    using Microsoft.WPSync.Shared.Sqm;
    using Microsoft.WPSync.Sync.Engine;
    using Microsoft.WPSync.Sync.Rules;
    using Microsoft.WPSync.UI;
    using Microsoft.WPSync.UI.Models;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;

    public class SqmHelper : ISqmHelper
    {
        private const uint AppId = 0x16;
        private bool isSqmEnabled;
        private uint musicCount;
        private double musicSize;
        private uint photoCount;
        private double photoSize;
        private ISqmManager sqmManager;
        private uint videoCount;
        private double videoSize;

        public SqmHelper(ISqmManager manager)
        {
            if (manager == null)
            {
                throw new ApplicationInitializationException(typeof(ISqmManager).ToString());
            }
            this.sqmManager = manager;
        }

        private static void CalculateAcquiredItemResults(ISyncPartnership partnership, SqmDeviceInfoStream stream)
        {
            double num = 0.0;
            uint num2 = 0;
            uint num3 = 0;
            uint num4 = 0;
            uint num5 = 0;
            List<SyncOperation> list = new List<SyncOperation>(partnership.SuccessfulAcquiredOperations);
            stream.AcquiredItemsCount = (uint) list.Count;
            foreach (SyncOperation operation in list)
            {
                int? nullable2 = operation.Item.Properties.NullableIntForKey("Size");
                long? nullable = nullable2.HasValue ? new long?((long) nullable2.GetValueOrDefault()) : null;
                if (nullable.HasValue)
                {
                    num += (double) nullable.Value;
                }
                if (operation.Item.Properties.BooleanForKey("Picture", false))
                {
                    num3++;
                }
                else
                {
                    if (operation.Item.Properties.BooleanForKey("Podcast", false))
                    {
                        num5++;
                        continue;
                    }
                    if (operation.Item.Properties.BooleanForKey("Has Video", false))
                    {
                        num4++;
                        continue;
                    }
                    if (operation.Item.Properties.BooleanForKey("Audio", false))
                    {
                        num2++;
                    }
                }
            }
            stream.AcquiredItemsSize = (uint) (num / 1000000.0);
            stream.AcquiredPhotoItemsCount = num3;
            stream.AcquiredMusicItemsCount = num2;
            stream.AcquiredVideoItemsCount = num4;
            stream.AcquiredPodcastItemsCount = num5;
        }

        private static void CalculateCumulativeSyncResults(ISyncPartnership partnership, SqmDeviceInfoStream stream)
        {
            double num = 0.0;
            List<SyncOperation> list = new List<SyncOperation>(partnership.SuccessfulCumulativeSyncOperations);
            stream.ItemsSuccessfullySyncedCount = (uint) list.Count;
            foreach (SyncOperation operation in list)
            {
                if ((operation.OperationType != SyncOperationType.Delete) && (operation.OperationType != SyncOperationType.Update))
                {
                    int? nullable2 = operation.Item.Properties.NullableIntForKey("Size");
                    long? nullable = nullable2.HasValue ? new long?((long) nullable2.GetValueOrDefault()) : null;
                    if (nullable.HasValue)
                    {
                        num += (double) nullable.Value;
                    }
                }
            }
            stream.ItemsSuccessfullySyncedSize = (uint) (num / 1000000.0);
        }

        private void CalculateDeviceStorageInfo(ISyncPartnership partnership, SqmDeviceInfoStream stream)
        {
            this.musicSize = 0.0;
            this.photoSize = 0.0;
            this.videoSize = 0.0;
            double num = 0.0;
            double freeBytes = 0.0;
            double totalBytes = 0.0;
            double num4 = 0.0;
            double num5 = 0.0;
            IDevice device = partnership.Device;
            StorageInfo info = (from i in device.StorageDevices.Values
                where i.StorageType == StorageType.FixedRam
                select i).FirstOrDefault<StorageInfo>();
            if (info != null)
            {
                this.musicSize += info.UsedStorageMusic;
                this.photoSize += info.UsedStoragePictures;
                this.videoSize += info.UsedStorageVideos;
                num += info.UsedStorageDocs;
                freeBytes = info.FreeBytes;
                totalBytes = info.TotalBytes;
            }
            info = (from i in device.StorageDevices.Values
                where i.StorageType == StorageType.RemovableRam
                select i).FirstOrDefault<StorageInfo>();
            if (info != null)
            {
                this.musicSize += info.UsedStorageMusic;
                this.photoSize += info.UsedStoragePictures;
                this.videoSize += info.UsedStorageVideos;
                num += info.UsedStorageDocs;
                num5 = info.TotalBytes;
                num4 = info.FreeBytes;
            }
            stream.InternalCapacity = (uint) (totalBytes / 1000000.0);
            stream.InternalFreeSpace = (uint) (freeBytes / 1000000.0);
            stream.SDCardCapacity = (uint) (num5 / 1000000.0);
            stream.SDCardFreeSpace = (uint) (num4 / 1000000.0);
            stream.MusicFileSize = (uint) (this.musicSize / 1000000.0);
            stream.PhotoFileSize = (uint) (this.photoSize / 1000000.0);
            stream.VideoFileSize = (uint) (this.videoSize / 1000000.0);
            stream.DocumentFileSize = (uint) (num / 1000000.0);
            stream.StorageAreaCount = (uint) device.StorageDevices.Count;
        }

        private void CalculateLibrarySizes(IMusicSyncSource musicSource)
        {
            this.musicCount = 0;
            this.musicSize = 0.0;
            this.photoCount = 0;
            this.photoSize = 0.0;
            this.videoCount = 0;
            this.videoSize = 0.0;
            if (musicSource != null)
            {
                this.CountFileTypes(musicSource);
            }
            if (DependencyContainer.PhotoSource != null)
            {
                this.CountFileTypes(DependencyContainer.PhotoSource);
            }
            this.sqmManager.Set(SqmSettings.PCMusicLibraryFileCount, this.musicCount);
            this.sqmManager.Set(SqmSettings.PCMusicLibraryFileSize, (uint) (this.musicSize / 1000000.0));
            this.sqmManager.Set(SqmSettings.PCVideoLibraryFileCount, this.videoCount);
            this.sqmManager.Set(SqmSettings.PCVideoLibraryFileSize, (uint) (this.videoSize / 1000000.0));
            this.sqmManager.Set(SqmSettings.PCPhotoLibraryFileCount, this.photoCount);
            this.sqmManager.Set(SqmSettings.PCPhotoLibraryFileSize, (uint) (this.photoSize / 1000000.0));
        }

        private static void CalculateMusicSyncSelections(IMusicSyncSource source, ISyncRules rules, SqmSyncContentStream stream)
        {
            uint num = 0;
            uint num2 = 0;
            uint num3 = 0;
            uint num4 = 0;
            uint num5 = 0;
            uint num6 = 0;
            if (rules.IsMusicSyncEnabled)
            {
                if (rules.SyncAllMusic)
                {
                    num = (uint) source.Playlists.Options.Count<ISyncSelectionOption>();
                    num2 = (uint) source.Genres.Options.Count<ISyncSelectionOption>();
                    num3 = (uint) source.Artists.Options.Count<ISyncSelectionOption>();
                    num4 = 0;
                }
                else
                {
                    num = (uint) source.Playlists.Options.Where<ISyncSelectionOption>(delegate (ISyncSelectionOption o) {
                        if (o.IsSelectedForSync.HasValue)
                        {
                            return o.IsSelectedForSync.Value;
                        }
                        return false;
                    }).Count<ISyncSelectionOption>();
                    num2 = (uint) source.Genres.Options.Where<ISyncSelectionOption>(delegate (ISyncSelectionOption o) {
                        if (o.IsSelectedForSync.HasValue)
                        {
                            return o.IsSelectedForSync.Value;
                        }
                        return false;
                    }).Count<ISyncSelectionOption>();
                    num3 = (uint) source.Artists.Options.Where<ISyncSelectionOption>(delegate (ISyncSelectionOption o) {
                        if (o.IsSelectedForSync.HasValue)
                        {
                            return o.IsSelectedForSync.Value;
                        }
                        return false;
                    }).Count<ISyncSelectionOption>();
                    num4 = (uint) (from o in source.Artists.Options
                        where !o.IsSelectedForSync.HasValue
                        select o).Count<ISyncSelectionOption>();
                }
            }
            if (rules.IsTVMoviesSyncEnabled)
            {
                if (rules.SyncAllTvMovies)
                {
                    num5 = (uint) source.MoviesTVShows.Options.Count<ISyncSelectionOption>();
                }
                else
                {
                    num5 = (uint) source.MoviesTVShows.Options.Where<ISyncSelectionOption>(delegate (ISyncSelectionOption o) {
                        if (o.IsSelectedForSync.HasValue)
                        {
                            return o.IsSelectedForSync.Value;
                        }
                        return false;
                    }).Count<ISyncSelectionOption>();
                }
            }
            if (rules.IsPodcastSyncEnabled)
            {
                if (rules.SyncAllPodcasts)
                {
                    num6 = (uint) source.Podcasts.Options.Count<ISyncSelectionOption>();
                }
                else
                {
                    num6 = (uint) source.Podcasts.Options.Where<ISyncSelectionOption>(delegate (ISyncSelectionOption o) {
                        if (o.IsSelectedForSync.HasValue)
                        {
                            return o.IsSelectedForSync.Value;
                        }
                        return false;
                    }).Count<ISyncSelectionOption>();
                }
            }
            stream.PlaylistFileSelectedCount = num;
            stream.GenreFileSelectedCount = num2;
            stream.ArtistFileSelectedCount = num3;
            stream.MusicAlbumFileSelectedCount = num4;
            stream.MovieFileSelectedCount = num5;
            stream.PodcastSeriesSelectedCount = num6;
        }

        private void CalculatePhotoSyncSelections(IPictureSyncSource source, ISyncRules rules, SqmSyncContentStream stream)
        {
            uint num = 0;
            uint num2 = 0;
            if (rules.IsPhotoVideoSyncEnabled)
            {
                int num3;
                if (rules.SyncAllPhotosVideos)
                {
                    num = (uint) source.PhotoVideoAlbums.Options.Count<ISyncSelectionOption>();
                }
                else
                {
                    num = (uint) this.CountTreeItemsFullySelected(source.PhotoVideoAlbums);
                }
                num2 = (uint) this.CountTreeItemsSelected(source.PhotoVideoAlbums, out num3);
            }
            stream.PhotoTreeSelectedCount = num;
            stream.PhotoFolderSelectedCount = num2;
        }

        private static void CalculateSyncResults(ISyncPartnership partnership, SqmSyncContentStream stream)
        {
            long? nullable;
            double num = 0.0;
            double num2 = 0.0;
            uint num3 = 0;
            List<SyncOperation> list = new List<SyncOperation>(partnership.AttemptedSyncOperations);
            stream.AttemptedFileCount = (uint) list.Count;
            foreach (SyncOperation operation in list)
            {
                int? nullable2 = operation.Item.Properties.NullableIntForKey("Size");
                nullable = nullable2.HasValue ? new long?((long) nullable2.GetValueOrDefault()) : null;
                if (nullable.HasValue && (operation.OperationType != SyncOperationType.Delete))
                {
                    num += (double) nullable.Value;
                }
            }
            stream.AttemptedFileSize = (uint) (num / 1000000.0);
            num = 0.0;
            List<SyncOperation> list2 = new List<SyncOperation>(partnership.SuccessfulSyncOperations);
            stream.SuccessfulFileCount = (uint) list2.Count;
            foreach (SyncOperation operation2 in list2)
            {
                int? nullable4 = operation2.Item.Properties.NullableIntForKey("Size");
                nullable = nullable4.HasValue ? new long?((long) nullable4.GetValueOrDefault()) : null;
                if (nullable.HasValue)
                {
                    if (operation2.OperationType == SyncOperationType.Delete)
                    {
                        num2 += (double) nullable.Value;
                    }
                    else
                    {
                        num += (double) nullable.Value;
                    }
                }
                if (operation2.OperationType == SyncOperationType.DeferredTransferTo)
                {
                    num3++;
                }
                MarkFileType(operation2.Item, stream);
            }
            stream.SuccessfulFileSize = (uint) (num / 1000000.0);
            stream.DeletedFileSize = (uint) (num2 / 1000000.0);
            stream.SuccessfulTranscodedFileCount = num3;
        }

        private void CountFileTypes(ISyncSource source)
        {
            List<ISyncable> list = new List<ISyncable>(source.AllItems.Values);
            foreach (ISyncable syncable in list)
            {
                int? nullable2 = syncable.Properties.NullableIntForKey("Size");
                long? nullable = nullable2.HasValue ? new long?((long) nullable2.GetValueOrDefault()) : null;
                if (syncable.Properties.BooleanForKey("Picture", false))
                {
                    this.photoCount++;
                    if (nullable.HasValue)
                    {
                        this.photoSize += (double) nullable.Value;
                    }
                }
                else
                {
                    if (syncable.Properties.BooleanForKey("Has Video", false))
                    {
                        this.videoCount++;
                        if (nullable.HasValue)
                        {
                            this.videoSize += (double) nullable.Value;
                        }
                        continue;
                    }
                    this.musicCount++;
                    if (nullable.HasValue)
                    {
                        this.musicSize += (double) nullable.Value;
                    }
                }
            }
        }

        private int CountTreeItemsFullySelected(ISyncOptionList list)
        {
            List<ISyncSelectionOption> list2 = new List<ISyncSelectionOption>(list.Options);
            foreach (ISyncSelectionOption option in list2)
            {
                if (option.IsSelectedForSync.HasValue)
                {
                    if (option.IsSelectedForSync.Value)
                    {
                        int num;
                        if (!option.HasChildOptions)
                        {
                            return 1;
                        }
                        if (this.CountTreeItemsSelected(option.ChildOptions, out num) == num)
                        {
                            return 1;
                        }
                        return this.CountTreeItemsFullySelected(option.ChildOptions);
                    }
                    if (option.HasChildOptions)
                    {
                        return this.CountTreeItemsFullySelected(option.ChildOptions);
                    }
                    return 0;
                }
                if (option.HasChildOptions)
                {
                    return this.CountTreeItemsFullySelected(option.ChildOptions);
                }
            }
            return 0;
        }

        private int CountTreeItemsSelected(ISyncOptionList list, out int totalCount)
        {
            int num = 0;
            totalCount = list.Count<KeyValuePair<string, ISyncSelectionOption>>();
            List<ISyncSelectionOption> list2 = new List<ISyncSelectionOption>(list.Options);
            foreach (ISyncSelectionOption option in list2)
            {
                if (option.IsSelectedForSync.HasValue && option.IsSelectedForSync.Value)
                {
                    num++;
                }
                if (option.HasChildOptions)
                {
                    int num2;
                    num += this.CountTreeItemsSelected(option.ChildOptions, out num2);
                    totalCount += num2;
                }
            }
            return num;
        }

        public void CreateDeviceInfoStream(ISyncPartnership partnership, int uniqueIndex)
        {
            if ((partnership != null) && this.isSqmEnabled)
            {
                SqmDeviceInfoStream stream = new SqmDeviceInfoStream {
                    DeviceIndex = (uint) (uniqueIndex + 1)
                };
                if (partnership.Device != null)
                {
                    stream.MakeModel = partnership.Device.ManufacturerModelID;
                }
                this.CalculateDeviceStorageInfo(partnership, stream);
                CalculateAcquiredItemResults(partnership, stream);
                CalculateCumulativeSyncResults(partnership, stream);
                SetAutoImportSetting(partnership.Device, stream);
                this.sqmManager.SetStream(stream);
            }
        }

        public void CreateSyncContentStream(ISyncPartnership partnership, SyncStartType type, SyncStoppedReason reason, TimeSpan syncDuration)
        {
            if ((partnership != null) && this.isSqmEnabled)
            {
                SqmSyncContentStream stream = new SqmSyncContentStream();
                SetSyncType(partnership, type, stream);
                if (partnership.Device != null)
                {
                    string winMoDeviceId = partnership.Device.WinMoDeviceId;
                    long? nullable = (long?) DeviceSettings.Get(winMoDeviceId, "SyncCount");
                    uint num = 0;
                    if (nullable.HasValue)
                    {
                        num = (uint) nullable.Value;
                    }
                    stream.SyncIndex = ++num;
                    DeviceSettings.Set(winMoDeviceId, "SyncCount", num);
                }
                ISyncRules ruleManager = partnership.RuleManager;
                SetGlobalRules(stream, ruleManager);
                CalculateMusicSyncSelections(partnership.MusicAndMovieProvider, partnership.RuleManager, stream);
                this.CalculatePhotoSyncSelections(partnership.PhotoAndVideoProvider, partnership.RuleManager, stream);
                CalculateSyncResults(partnership, stream);
                stream.OperationTime = (uint) syncDuration.TotalMilliseconds;
                SetSyncResult(reason, stream);
                this.sqmManager.SetStream(stream);
            }
        }

        public void EndSqmSession(uint uniquelyConnectedDeviceCount)
        {
            if (this.isSqmEnabled)
            {
                IMusicSyncSource iTunesMusicSource;
                this.sqmManager.Set(SqmSettings.LibrarySource, (uint)(GlobalSetting.IsMusicSourceITunes() ? 1 : 2));
                this.sqmManager.Set(SqmSettings.Locale, (uint) Thread.CurrentThread.CurrentCulture.LCID);
                this.sqmManager.Set(SqmSettings.NumberPhonesConnected, uniquelyConnectedDeviceCount);
                Version version = new Version(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion);
                this.sqmManager.Set(SqmSettings.AppVersionMajor, (uint) version.Major);
                this.sqmManager.Set(SqmSettings.AppVersionMinor, (uint) version.Minor);
                this.sqmManager.Set(SqmSettings.AppVersionBuildMajor, (uint) version.Build);
                this.sqmManager.Set(SqmSettings.AppVersionBuildMinor, (uint) version.Revision);
                Version version2 = Environment.OSVersion.Version;
                this.sqmManager.Set(SqmSettings.OSVersionMajor, (uint) version2.Major);
                this.sqmManager.Set(SqmSettings.OSVersionMinor, (uint) version2.Minor);
                Version version3 = ITunesApi.Version;
                if (version3 != null)
                {
                    this.sqmManager.Set(SqmSettings.iTunesVersionMajor, (uint) version3.Major);
                    this.sqmManager.Set(SqmSettings.iTunesVersionMinor, (uint) version3.Minor);
                }
                if (GlobalSetting.IsMusicSourceITunes())
                {
                    iTunesMusicSource = DependencyContainer.ITunesMusicSource;
                }
                else
                {
                    iTunesMusicSource = DependencyContainer.WindowsLibraryMusicSource;
                }
                this.CalculateLibrarySizes(iTunesMusicSource);
                this.sqmManager.SetAppInfo(0x16, version);
                this.sqmManager.EndSession();
                this.sqmManager.Shutdown();
            }
        }

        public void InitSqm()
        {
            this.isSqmEnabled = (bool) GlobalSetting.GetApplicationSetting("SendSqmInfo");
            this.sqmManager.InitSqm(this.isSqmEnabled, GlobalSetting.SqmFilesDirectoryForApplication(true));
        }

        private static void MarkFileType(ISyncable item, SqmSyncContentStream stream)
        {
            string localPath = null;
            if (item.OriginalLocation != null)
            {
                localPath = item.OriginalLocation.LocalPath;
            }
            if (string.IsNullOrEmpty(localPath))
            {
                localPath = (string) item.Properties.FirstObjectForKeys(new string[] { ZMEDIAITEM_STRINGATTRIBUTE.ZMEDIAITEM_ATTRIBUTE_FILEPATH.ToString(), "DevicePath" });
            }
            if (string.IsNullOrEmpty(localPath))
            {
                localPath = (string) item.Properties.ObjectForKey("Location");
            }
            string extension = Path.GetExtension(localPath);
            if (extension != null)
            {
                switch (extension.ToUpperInvariant())
                {
                    case ".JPG":
                    case ".JPEG":
                        stream.MarkFileType(SqmFileTypes.jpg, true);
                        return;

                    case ".PNG":
                        stream.MarkFileType(SqmFileTypes.png, true);
                        return;

                    case ".GIF":
                        stream.MarkFileType(SqmFileTypes.gif, true);
                        return;

                    case ".MP3":
                        stream.MarkFileType(SqmFileTypes.mp3, true);
                        return;

                    case ".WMA":
                        stream.MarkFileType(SqmFileTypes.wma, true);
                        return;

                    case ".AAC":
                        stream.MarkFileType(SqmFileTypes.aac, true);
                        return;

                    case ".MP4":
                        stream.MarkFileType(SqmFileTypes.mp4, true);
                        return;

                    case ".WMV":
                        stream.MarkFileType(SqmFileTypes.wmv, true);
                        return;

                    case ".M4R":
                        stream.MarkFileType(SqmFileTypes.m4r, true);
                        return;
                }
                stream.MarkFileType(SqmFileTypes.other, true);
            }
        }

        public void OnDeviceConnected(IDevice device, int index)
        {
            if (device != null)
            {
                bool isSqmEnabled = this.isSqmEnabled;
            }
        }

        public void OnSqmOptinChanged(bool value)
        {
            this.sqmManager.SetEnabled(value);
            this.isSqmEnabled = value;
        }

        private static void SetAutoImportSetting(IDevice device, SqmDeviceInfoStream stream)
        {
            bool? nullable = (bool?) DeviceSettings.Get(device.WinMoDeviceId, "ImportPictures");
            bool flag = nullable.HasValue ? nullable.GetValueOrDefault() : false;
            if (device.IsFirstConnect)
            {
                if (flag)
                {
                    stream.IsAutoImportEnabled = SqmAutoImportSetting.FirstConnectYes;
                }
                else
                {
                    stream.IsAutoImportEnabled = SqmAutoImportSetting.FirstConnectNo;
                }
            }
            else if (flag)
            {
                stream.IsAutoImportEnabled = SqmAutoImportSetting.Yes;
            }
            else
            {
                stream.IsAutoImportEnabled = SqmAutoImportSetting.No;
            }
        }

        private static void SetGlobalRules(SqmSyncContentStream stream, ISyncRules rules)
        {
            stream.MarkGlobalSyncOption(SqmGlobalSyncOptions.MusicEnabled, rules.IsMusicSyncEnabled);
            stream.MarkGlobalSyncOption(SqmGlobalSyncOptions.AllMusic, rules.SyncAllMusic);
            stream.MarkGlobalSyncOption(SqmGlobalSyncOptions.PhotosEnabled, rules.IsPhotoVideoSyncEnabled);
            stream.MarkGlobalSyncOption(SqmGlobalSyncOptions.AllPhotos, rules.SyncAllPhotosVideos);
            stream.MarkGlobalSyncOption(SqmGlobalSyncOptions.IncludeVideos, rules.SyncIncludeVideos);
            stream.MarkGlobalSyncOption(SqmGlobalSyncOptions.VideosEnabled, rules.IsTVMoviesSyncEnabled);
            stream.MarkGlobalSyncOption(SqmGlobalSyncOptions.AllVideos, rules.SyncAllTvMovies);
            stream.MarkGlobalSyncOption(SqmGlobalSyncOptions.PodcastsEnabled, rules.IsPodcastSyncEnabled);
            stream.MarkGlobalSyncOption(SqmGlobalSyncOptions.PodcastSyncAll, rules.SyncAllPodcasts);
            stream.MarkGlobalSyncOption(SqmGlobalSyncOptions.MusicEnabled, rules.IsMusicSyncEnabled);
            switch (rules.PodcastSyncCount)
            {
                case 1:
                    if (!rules.SyncUnplayedPodcastsOnly)
                    {
                        stream.MarkGlobalSyncOption(SqmGlobalSyncOptions.PodcastSync1MostRecent, true);
                        return;
                    }
                    stream.MarkGlobalSyncOption(SqmGlobalSyncOptions.PodcastSync1Unplayed, true);
                    return;

                case 3:
                    if (!rules.SyncUnplayedPodcastsOnly)
                    {
                        stream.MarkGlobalSyncOption(SqmGlobalSyncOptions.PodcastSync3MostRecent, true);
                        return;
                    }
                    stream.MarkGlobalSyncOption(SqmGlobalSyncOptions.PodcastSync3Unplayed, true);
                    return;

                case 5:
                    if (!rules.SyncUnplayedPodcastsOnly)
                    {
                        stream.MarkGlobalSyncOption(SqmGlobalSyncOptions.PodcastSync5MostRecent, true);
                        return;
                    }
                    stream.MarkGlobalSyncOption(SqmGlobalSyncOptions.PodcastSync5Unplayed, true);
                    return;
            }
            if (rules.SyncUnplayedPodcastsOnly)
            {
                stream.MarkGlobalSyncOption(SqmGlobalSyncOptions.PodcastSyncAllUnplayed, true);
            }
            else
            {
                stream.MarkGlobalSyncOption(SqmGlobalSyncOptions.PodcastSyncAll, true);
            }
        }

        private static void SetSyncResult(SyncStoppedReason reason, SqmSyncContentStream stream)
        {
            switch (reason)
            {
                case SyncStoppedReason.Completed:
                    stream.SyncResult = SqmSyncTerminationMethods.Completed;
                    return;

                case SyncStoppedReason.Cancelled:
                    stream.SyncResult = SqmSyncTerminationMethods.Canceled;
                    return;

                case SyncStoppedReason.Aborted:
                    stream.SyncResult = SqmSyncTerminationMethods.Aborted;
                    return;

                case SyncStoppedReason.Paused:
                case SyncStoppedReason.CouldNotStart:
                case SyncStoppedReason.Unknown:
                    break;

                case SyncStoppedReason.Disposed:
                    stream.SyncResult = SqmSyncTerminationMethods.Quit;
                    break;

                default:
                    return;
            }
        }

        private static void SetSyncType(ISyncPartnership partnership, SyncStartType type, SqmSyncContentStream stream)
        {
            if (partnership.IsFirstSync)
            {
                stream.SyncType = SqmSyncType.FirstTimeSync;
            }
            else
            {
                switch (type)
                {
                    case SyncStartType.AutoSync:
                        stream.SyncType = SqmSyncType.AutoSync;
                        return;

                    case SyncStartType.ManualSync:
                        stream.SyncType = SqmSyncType.ManualSync;
                        return;

                    case SyncStartType.Delete:
                        stream.SyncType = SqmSyncType.Delete;
                        return;

                    case SyncStartType.CopyToPC:
                        stream.SyncType = SqmSyncType.CopyToPC;
                        return;

                    case SyncStartType.SendRingtones:
                        stream.SyncType = SqmSyncType.Ringtones;
                        return;
                }
            }
        }
    }
}

