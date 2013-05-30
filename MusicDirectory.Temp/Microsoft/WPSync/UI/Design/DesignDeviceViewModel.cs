namespace Microsoft.WPSync.UI.Design
{
    using Microsoft.WPSync.Device;
    using Microsoft.WPSync.Shared;
    using Microsoft.WPSync.UI.Properties;
    using Microsoft.WPSync.UI.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Windows.Data;

    public class DesignDeviceViewModel : PropChangeNotifier, IDeviceViewModel
    {
        private IDevice device;
        private IDevicePropertiesViewModel props;

        public DesignDeviceViewModel()
        {
            List<DesignDeviceItem> collection = new List<DesignDeviceItem>();
            DesignDeviceItem item = new DesignDeviceItem {
                Name = "Open arms",
                Album = "Frontiers",
                Artist = "Journey",
                Duration = "3:20",
                Size = "35 kB"
            };
            collection.Add(item);
            DesignDeviceItem item2 = new DesignDeviceItem {
                Name = "Faithfully",
                Album = "Frontiers",
                Artist = "Journey",
                Duration = "3:43",
                Size = "38 kB"
            };
            collection.Add(item2);
            DesignDeviceItem item3 = new DesignDeviceItem {
                Name = "Send Her My Love",
                Album = "Frontiers",
                Artist = "Journey",
                Duration = "4:23",
                Size = "43 kB"
            };
            collection.Add(item3);
            DesignDeviceItem item4 = new DesignDeviceItem {
                Name = "Thriller",
                Album = "Thriller",
                Artist = "Michael Jackson",
                Duration = "6:03",
                Size = "56 kB"
            };
            collection.Add(item4);
            this.Music = AddCollectionViewSource(collection);
            collection = new List<DesignDeviceItem>();
            DesignDeviceItem item5 = new DesignDeviceItem {
                Name = "baby pic 1",
                Album = "New baby - Month 1"
            };
            collection.Add(item5);
            DesignDeviceItem item6 = new DesignDeviceItem {
                Name = "baby pic 2",
                Album = "New baby - Month 1"
            };
            collection.Add(item6);
            DesignDeviceItem item7 = new DesignDeviceItem {
                Name = "baby pic 3",
                Album = "New baby - Month 1"
            };
            collection.Add(item7);
            DesignDeviceItem item8 = new DesignDeviceItem {
                Name = "baby pic 4",
                Album = "New baby - Month 1"
            };
            collection.Add(item8);
            DesignDeviceItem item9 = new DesignDeviceItem {
                Name = "baby pic 5",
                Album = "New baby - Month 1"
            };
            collection.Add(item9);
            this.PhotoVideoAlbums = AddCollectionViewSource(collection);
            collection = new List<DesignDeviceItem>();
            DesignDeviceItem item10 = new DesignDeviceItem {
                Name = "Toy Story",
                Duration = "1:15",
                Size = "23 MB"
            };
            collection.Add(item10);
            DesignDeviceItem item11 = new DesignDeviceItem {
                Name = "Toy Story 2",
                Duration = "1:23",
                Size = "24 MB"
            };
            collection.Add(item11);
            DesignDeviceItem item12 = new DesignDeviceItem {
                Name = "Toy Story 3",
                Duration = "1:05",
                Size = "21 MB"
            };
            collection.Add(item12);
            this.MoviesTVShows = AddCollectionViewSource(collection);
            collection = new List<DesignDeviceItem>();
            DesignDeviceItem item13 = new DesignDeviceItem {
                Name = "Wait Wait Don't Tell Me",
                Duration = "1:15",
                Size = "23 MB"
            };
            collection.Add(item13);
            DesignDeviceItem item14 = new DesignDeviceItem {
                Name = "Wait Wait Don't Tell Me",
                Duration = "1:23",
                Size = "24 MB"
            };
            collection.Add(item14);
            this.Podcasts = AddCollectionViewSource(collection);
        }

        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists"), SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static CollectionViewSource AddCollectionViewSource(List<DesignDeviceItem> collection)
        {
            CollectionViewSource source = new CollectionViewSource {
                Source = collection
            };
            source.View.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            source.View.Refresh();
            return source;
        }

        public void CopyItemsToPC(IEnumerable<DeviceItemViewModel> list)
        {
            throw new NotImplementedException();
        }

        public void DeleteItems(IEnumerable<DeviceItemViewModel> list)
        {
            throw new NotImplementedException();
        }

        public void TransferRingtones()
        {
            throw new NotImplementedException();
        }

        public IDevice Device
        {
            get
            {
                return this.device;
            }
            set
            {
                this.device = value;
                this.Props = new DevicePropertiesViewModel(value);
            }
        }

        public string MoviesTVSeriesFilterString { get; set; }

        public string MoviesTVSeriesNoContent
        {
            get
            {
                return Microsoft.WPSync.UI.Properties.Resources.NoContentHeader;
            }
        }

        public CollectionViewSource MoviesTVShows { get; set; }

        public CollectionViewSource Music { get; set; }

        public string MusicFilterString { get; set; }

        public string MusicNoContent
        {
            get
            {
                return Microsoft.WPSync.UI.Properties.Resources.NoContentHeader;
            }
        }

        public string PhotosVideosFilterString { get; set; }

        public string PhotosVideosNoContent
        {
            get
            {
                return Microsoft.WPSync.UI.Properties.Resources.NoContentHeader;
            }
        }

        public CollectionViewSource PhotoVideoAlbums { get; set; }

        public CollectionViewSource Podcasts { get; set; }

        public string PodcastsFilterString { get; set; }

        public string PodcastsNoContent
        {
            get
            {
                return Microsoft.WPSync.UI.Properties.Resources.NoContentHeader;
            }
        }

        public IDevicePropertiesViewModel Props
        {
            get
            {
                return this.props;
            }
            set
            {
                this.props = value;
                this.OnPropertyChanged("Props");
            }
        }
    }
}

