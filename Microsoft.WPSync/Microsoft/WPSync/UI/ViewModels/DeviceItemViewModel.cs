using System.ComponentModel;

namespace Microsoft.WPSync.UI.ViewModels
{
    using Microsoft.WPSync.Device;
    using Microsoft.WPSync.Shared;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.CompilerServices;

    public class DeviceItemViewModel : INotifyPropertyChanged
    {
        private string album;
        private string albumUpper;
        private string artist;
        private string artistUpper;
        private int disk;
        private string duration;
        private bool isChecked;
        private string location;
        private string name;
        private string nameUpper;
        private string size;
        private string sortName;
        private bool sortNameCalculated;
        private int track;

        public bool DoesMatchFilter(string filter)
        {
            return (string.IsNullOrEmpty(filter) || ((!string.IsNullOrEmpty(this.nameUpper) && this.nameUpper.Contains(filter)) || ((!string.IsNullOrEmpty(this.albumUpper) && this.albumUpper.Contains(filter)) || (!string.IsNullOrEmpty(this.artistUpper) && this.artistUpper.Contains(filter)))));
        }

        private static string MakeLocationString(int? storageLocation)
        {
            if (!storageLocation.HasValue)
            {
                return "?";
            }
            var zmedialibStoragelocationFlag = (ZMEDIALIB_STORAGELOCATION_FLAG)storageLocation.Value;
            if (zmedialibStoragelocationFlag <= ZMEDIALIB_STORAGELOCATION_FLAG.ZMEDIALIB_STORAGELOCATION_FLAG_EXTERNAL_1)
            {
                switch (zmedialibStoragelocationFlag)
                {
                    case ZMEDIALIB_STORAGELOCATION_FLAG.ZMEDIALIB_STORAGELOCATION_FLAG_CLOUD:
                    case ZMEDIALIB_STORAGELOCATION_FLAG.ZMEDIALIB_STORAGELOCATION_FLAG_INTERNAL:
                        return string.Empty;

                    case (ZMEDIALIB_STORAGELOCATION_FLAG.ZMEDIALIB_STORAGELOCATION_FLAG_INTERNAL | ZMEDIALIB_STORAGELOCATION_FLAG.ZMEDIALIB_STORAGELOCATION_FLAG_CLOUD):
                        goto Label_0051;

                    case ZMEDIALIB_STORAGELOCATION_FLAG.ZMEDIALIB_STORAGELOCATION_FLAG_EXTERNAL_0:
                    case ZMEDIALIB_STORAGELOCATION_FLAG.ZMEDIALIB_STORAGELOCATION_FLAG_EXTERNAL_1:
                        goto Label_004B;
                }
                goto Label_0051;
            }
            if ((zmedialibStoragelocationFlag != ZMEDIALIB_STORAGELOCATION_FLAG.ZMEDIALIB_STORAGELOCATION_FLAG_EXTERNAL_2) && (zmedialibStoragelocationFlag != ZMEDIALIB_STORAGELOCATION_FLAG.ZMEDIALIB_STORAGELOCATION_FLAG_EXTERNAL_3))
            {
                goto Label_0051;
            }
        Label_004B:
            return "•";
        Label_0051:
            return "?";
        }

        public override string ToString()
        {
            return string.Empty;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies")]
        public void UpdateProperties(ZmdbPropertyList properties, DeviceItemType type)
        {
            if (properties != null)
            {
                this.ZmdbProperties = properties;
                this.ItemType = type;
                this.Location = MakeLocationString(properties[ZMEDIAITEM_INTATTRIBUTE.ZMEDIAITEM_ATTRIBUTE_STORAGELOCATION]);
                string str = properties[ZMEDIAITEM_STRINGATTRIBUTE.ZMEDIAITEM_ATTRIBUTE_NAME];
                this.Name = !string.IsNullOrWhiteSpace(str) ? str : properties[ZMEDIAITEM_STRINGATTRIBUTE.ZMEDIAITEM_ATTRIBUTE_FILENAME];
                if (this.Name == null)
                {
                    this.Name = string.Empty;
                }
                int? nullable3 = properties[ZMEDIAITEM_INTATTRIBUTE.ZMEDIAITEM_ATTRIBUTE_FILESIZE];
                ulong? sizeInBytes = nullable3.HasValue ? new ulong?((ulong) ((long) nullable3.GetValueOrDefault())) : null;
                this.Size = StringUtilities.MakeSizeString(sizeInBytes);
                ulong? nullable5 = sizeInBytes;
                this.ZmdbSize = nullable5.HasValue ? nullable5.GetValueOrDefault() : ((ulong) 0L);
                if (((type == DeviceItemType.Music) || (type == DeviceItemType.AudioPodcast)) || (type == DeviceItemType.VideoPodcast))
                {
                    this.Artist = properties[ZMEDIAITEM_STRINGATTRIBUTE.ZMEDIAITEM_ATTRIBUTE_ARTIST] ?? string.Empty;
                }
                if (((type == DeviceItemType.Music) || (type == DeviceItemType.Video)) || ((type == DeviceItemType.AudioPodcast) || (type == DeviceItemType.VideoPodcast)))
                {
                    int? nullable6 = properties[ZMEDIAITEM_INTATTRIBUTE.ZMEDIAITEM_ATTRIBUTE_DURATION];
                    long? ms = nullable6.HasValue ? new long?((long) nullable6.GetValueOrDefault()) : null;
                    this.Duration = StringUtilities.MakeTimeString(ms);
                    long? nullable8 = ms;
                    this.ZmdbDuration = nullable8.HasValue ? nullable8.GetValueOrDefault() : 0L;
                }
                switch (type)
                {
                    case DeviceItemType.Photo:
                    {
                        string str2 = properties[ZMEDIAITEM_STRINGATTRIBUTE.ZMEDIAITEM_ATTRIBUTE_FILEPATH];
                        this.Album = !string.IsNullOrWhiteSpace(str2) ? (Path.GetFileName(Path.GetDirectoryName(str2)) ?? string.Empty) : string.Empty;
                        return;
                    }
                    case DeviceItemType.Music:
                    {
                        this.Album = properties[ZMEDIAITEM_STRINGATTRIBUTE.ZMEDIAITEM_ATTRIBUTE_ALBUM] ?? string.Empty;
                        int? nullable9 = properties[ZMEDIAITEM_INTATTRIBUTE.ZMEDIAITEM_ATTRIBUTE_DISKNUMBER];
                        this.Disk = nullable9.HasValue ? nullable9.GetValueOrDefault() : 0;
                        int? nullable10 = properties[ZMEDIAITEM_INTATTRIBUTE.ZMEDIAITEM_ATTRIBUTE_TRACKNUMBER];
                        this.Track = nullable10.HasValue ? nullable10.GetValueOrDefault() : 0;
                        return;
                    }
                    case DeviceItemType.Video:
                        return;

                    case DeviceItemType.AudioPodcast:
                        this.Album = properties[ZMEDIAITEM_STRINGATTRIBUTE.ZMEDIAITEM_ATTRIBUTE_PODCASTSERIES] ?? string.Empty;
                        return;

                    case DeviceItemType.VideoPodcast:
                        this.Album = properties[ZMEDIAITEM_STRINGATTRIBUTE.ZMEDIAITEM_ATTRIBUTE_VIDEOSERIES] ?? string.Empty;
                        return;
                }
            }
        }

        public string Album
        {
            get
            {
                return this.album;
            }
            set
            {
                this.album = value;
                this.albumUpper = (value == null) ? null : value.ToUpperInvariant();
                this.OnPropertyChanged("Album");
            }
        }

        public string Artist
        {
            get
            {
                return this.artist;
            }
            set
            {
                this.artist = value;
                this.artistUpper = (value == null) ? null : value.ToUpperInvariant();
                this.OnPropertyChanged("Artist");
            }
        }

        public int Disk
        {
            get
            {
                return this.disk;
            }
            set
            {
                this.disk = value;
                this.OnPropertyChanged("Disk");
            }
        }

        public string Duration
        {
            get
            {
                return this.duration;
            }
            set
            {
                this.duration = value;
                this.OnPropertyChanged("Duration");
            }
        }

        public bool IsChecked
        {
            get
            {
                return this.isChecked;
            }
            set
            {
                this.isChecked = value;
                this.OnPropertyChanged("IsChecked");
            }
        }

        public DeviceItemType ItemType { get; set; }

        public string Location
        {
            get
            {
                return this.location;
            }
            set
            {
                this.location = value;
                this.OnPropertyChanged("Location");
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
                this.nameUpper = (value == null) ? null : value.ToUpperInvariant();
                this.sortNameCalculated = false;
                this.OnPropertyChanged("Name");
            }
        }

        public string Size
        {
            get
            {
                return this.size;
            }
            set
            {
                this.size = value;
                this.OnPropertyChanged("Size");
            }
        }

        public string SortName
        {
            get
            {
                string str;
                if (this.sortNameCalculated)
                {
                    return (this.sortName ?? this.name);
                }
                this.sortNameCalculated = true;
                if (StringUtilities.HasArticlePrefix(this.name, out str))
                {
                    this.sortName = str;
                    return this.sortName;
                }
                return this.name;
            }
        }

        public int Track
        {
            get
            {
                return this.track;
            }
            set
            {
                this.track = value;
                this.OnPropertyChanged("Track");
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Zmdb")]
        public long ZmdbDuration { get; set; }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Zmdb")]
        public string ZmdbId { get; set; }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Zmdb"), SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ZmdbPropertyList ZmdbProperties { get; set; }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Zmdb")]
        public ulong ZmdbSize { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

