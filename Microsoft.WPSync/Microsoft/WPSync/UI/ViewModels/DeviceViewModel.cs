namespace Microsoft.WPSync.UI.ViewModels
{
    using Microsoft.WPSync.Device;
    using Microsoft.WPSync.Shared;
    using Microsoft.WPSync.Sync.Engine;
    using Microsoft.WPSync.Sync.Rules;
    using Microsoft.WPSync.UI;
    using Microsoft.WPSync.UI.Properties;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Data;
    using System.Windows.Threading;

    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class DeviceViewModel : PropChangeNotifier, IDeviceViewModel
    {
        private readonly IMainController controller;
        private IDevice device;
        private string moviesTVSeriesFilterString;
        private string moviesTVSeriesFilterStringUpper;
        private CollectionViewSource moviesTVShows;
        private BatchingTimer moviesTVShowsRefresh;
        private List<DeviceItemViewModel> moviesTVShowsSource;
        private CollectionViewSource music;
        private string musicFilterString;
        private string musicFilterStringUpper;
        private BatchingTimer musicRefresh;
        private List<DeviceItemViewModel> musicSource;
        private string photosVideosFilterString;
        private string photosVideosFilterStringUpper;
        private CollectionViewSource photoVideoAlbums;
        private BatchingTimer photoVideoAlbumsRefresh;
        private List<DeviceItemViewModel> photoVideoAlbumsSource;
        private CollectionViewSource podcasts;
        private string podcastsFilterString;
        private string podcastsFilterStringUpper;
        private BatchingTimer podcastsRefresh;
        private List<DeviceItemViewModel> podcastsSource;
        private IDevice previousDevice;
        private IDevicePropertiesViewModel props;

        public DeviceViewModel(IMainController controller)
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }
            this.controller = controller;
            controller.PropertyChanged += new PropertyChangedEventHandler(this.OnControllerPropertyChanged);
            this.InitCollectionViews();
        }

        public void CopyItemsToPC(IEnumerable<DeviceItemViewModel> list)
        {
            if (list != null)
            {
                foreach (DeviceItemViewModel model in list)
                {
                    this.controller.CurrentSyncPartnership.Engine.ScheduleOperation(SyncableItemFromDeviceItem(model), SyncOperationType.TransferFrom);
                }
                this.controller.MainViewModel.DoSyncCommand.Execute(SyncStartType.CopyToPC);
            }
        }

        public void DeleteItems(IEnumerable<DeviceItemViewModel> list)
        {
            if (list != null)
            {
                foreach (DeviceItemViewModel model in list)
                {
                    this.controller.CurrentSyncPartnership.Engine.ScheduleOperation(SyncableItemFromDeviceItem(model), SyncOperationType.Delete);
                }
                this.controller.MainViewModel.DoSyncCommand.Execute(SyncStartType.Delete);
            }
        }

        private void InitCollectionViews()
        {
            SortDescription description = new SortDescription("SortName", ListSortDirection.Ascending);
            SortDescription description2 = new SortDescription("Album", ListSortDirection.Ascending);
            SortDescription description3 = new SortDescription("Artist", ListSortDirection.Ascending);
            SortDescription description4 = new SortDescription("Disk", ListSortDirection.Ascending);
            SortDescription description5 = new SortDescription("Track", ListSortDirection.Ascending);
            SortDescription description6 = new SortDescription("ZmdbId", ListSortDirection.Ascending);
            this.music = new CollectionViewSource();
            this.music.SortDescriptions.Add(description3);
            this.music.SortDescriptions.Add(description2);
            this.music.SortDescriptions.Add(description4);
            this.music.SortDescriptions.Add(description5);
            this.music.SortDescriptions.Add(description6);
            this.musicSource = new List<DeviceItemViewModel>();
            this.music.Source = this.musicSource;
            this.music.View.Filter = item => ((DeviceItemViewModel) item).DoesMatchFilter(this.musicFilterStringUpper);
            //this.musicRefresh = new BatchingTimer(delegate {
            //    this.refreshView(this.music);
            //}, 200);
            this.moviesTVShows = new CollectionViewSource();
            this.moviesTVShows.SortDescriptions.Add(description);
            this.moviesTVShows.SortDescriptions.Add(description6);
            this.moviesTVShowsSource = new List<DeviceItemViewModel>();
            this.moviesTVShows.Source = this.moviesTVShowsSource;
            this.moviesTVShows.View.Filter = item => ((DeviceItemViewModel) item).DoesMatchFilter(this.moviesTVSeriesFilterStringUpper);

            //this.moviesTVShowsRefresh = new BatchingTimer(() => {
            //    this.refreshView(this.moviesTVShows);
            //}, 200);

            this.podcasts = new CollectionViewSource();
            this.podcasts.SortDescriptions.Add(description);
            this.podcasts.SortDescriptions.Add(description6);
            this.podcastsSource = new List<DeviceItemViewModel>();
            this.podcasts.Source = this.podcastsSource;
            this.podcasts.View.Filter = item => ((DeviceItemViewModel) item).DoesMatchFilter(this.podcastsFilterStringUpper);
       
            //this.podcastsRefresh = new BatchingTimer( new System.Action(() => {
            //    this.refreshView(this.podcasts);
            //}, 200);
            
            this.photoVideoAlbums = new CollectionViewSource();
            this.photoVideoAlbums.SortDescriptions.Add(description);
            this.photoVideoAlbums.SortDescriptions.Add(description6);
            this.photoVideoAlbumsSource = new List<DeviceItemViewModel>();
            this.photoVideoAlbums.Source = this.photoVideoAlbumsSource;
            this.photoVideoAlbums.View.Filter = item => ((DeviceItemViewModel) item).DoesMatchFilter(this.photosVideosFilterStringUpper);
            //this.photoVideoAlbumsRefresh = new BatchingTimer(delegate {
            //    this.refreshView(this.photoVideoAlbums);
            //}, 200);
        }

        private void OnControllerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Action method = null;
            string str;
            if (((str = e.PropertyName) != null) && (str == "CurrentDevice"))
            {
                if (method == null)
                {
                    method = delegate {
                        this.Device = this.controller.CurrentDevice;
                    };
                }
                this.controller.Dispatcher.BeginInvoke(method, new object[0]);
            }
        }

        private void OnDeviceChanged()
        {
            if (this.previousDevice != null)
            {
                this.previousDevice.ZmdbChangedEvent -= new EventHandler(this.OnDeviceZmdbChangedEvent);
                this.previousDevice.PropertyChanged -= new PropertyChangedEventHandler(this.OnDevicePropertyChanged);
            }
            this.previousDevice = this.device;
            if (this.device != null)
            {
                this.ResetDevice();
                this.device.ZmdbChangedEvent += new EventHandler(this.OnDeviceZmdbChangedEvent);
                this.device.PropertyChanged += new PropertyChangedEventHandler(this.OnDevicePropertyChanged);
            }
            else
            {
                this.Props = null;
            }
        }

        private void OnDevicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CachedIsLocked")
            {
                this.Props.EnsureImageIsLoaded();
            }
        }

        private void OnDeviceZmdbChangedEvent(object sender, EventArgs e)
        {
            //this.controller.Dispatcher.BeginInvoke(DispatcherPriority.Render, delegate {
            //    this.ResetBrowsingContent();
            //});
        }

        private void refreshView(CollectionViewSource viewSource)
        {
            //this.controller.Dispatcher.BeginInvoke(delegate {
            //    viewSource.View.Refresh();
            //}, new object[0]);
        }

        private void ResetBrowsingContent()
        {
            Tuple<ZmdbPropertyList, DeviceItemType>[] models = new Tuple<ZmdbPropertyList, DeviceItemType>[] { new Tuple<ZmdbPropertyList, DeviceItemType>(this.Device.Zmdb.GetList(ZMEDIALIST_TYPE.ZMEDIALIST_TYPE_ALL_AUDIO), DeviceItemType.Music) };
            Tuple<ZmdbPropertyList, DeviceItemType>[] tupleArray2 = new Tuple<ZmdbPropertyList, DeviceItemType>[] { new Tuple<ZmdbPropertyList, DeviceItemType>(this.Device.Zmdb.GetList(ZMEDIALIST_TYPE.ZMEDIALIST_TYPE_ALL_VIDEOS_NONPODCASTS), DeviceItemType.Video) };
            Tuple<ZmdbPropertyList, DeviceItemType>[] tupleArray3 = new Tuple<ZmdbPropertyList, DeviceItemType>[] { new Tuple<ZmdbPropertyList, DeviceItemType>(this.Device.Zmdb.GetList(ZMEDIALIST_TYPE.ZMEDIALIST_TYPE_ALL_PODCASTAUDIO), DeviceItemType.AudioPodcast), new Tuple<ZmdbPropertyList, DeviceItemType>(this.Device.Zmdb.GetList(ZMEDIALIST_TYPE.ZMEDIALIST_TYPE_ALL_VIDEOS_PODCAST), DeviceItemType.VideoPodcast) };
            Tuple<ZmdbPropertyList, DeviceItemType>[] tupleArray4 = new Tuple<ZmdbPropertyList, DeviceItemType>[] { new Tuple<ZmdbPropertyList, DeviceItemType>(this.Device.Zmdb.GetList(ZMEDIALIST_TYPE.ZMEDIALIST_TYPE_ALL_PICTURES), DeviceItemType.Photo) };
            this.ResetDeviceContentType(this.musicSource, models, this.music);
            this.ResetDeviceContentType(this.moviesTVShowsSource, tupleArray2, this.moviesTVShows);
            this.ResetDeviceContentType(this.podcastsSource, tupleArray3, this.podcasts);
            this.ResetDeviceContentType(this.photoVideoAlbumsSource, tupleArray4, this.photoVideoAlbums);
            this.OnPropertyChanged("Music");
            this.OnPropertyChanged("MoviesTVShows");
            this.OnPropertyChanged("Podcasts");
            this.OnPropertyChanged("PhotoVideoAlbums");
        }

        private void ResetDevice()
        {
            this.Props = new DevicePropertiesViewModel(this.Device);
            this.ResetBrowsingContent();
        }

        private void ResetDeviceContentType(IList<DeviceItemViewModel> items, IEnumerable<Tuple<ZmdbPropertyList, DeviceItemType>> models, CollectionViewSource viewSource)
        {
            if (this.Device != null)
            {
                for (int i = items.Count - 1; i >= 0; i--)
                {
                    DeviceItemViewModel model = items[i];
                    object obj2 = null;
                    DeviceItemType other = DeviceItemType.Other;
                    foreach (Tuple<ZmdbPropertyList, DeviceItemType> tuple in models)
                    {
                        if (tuple.Item1.TryGetValue(model.ZmdbId, out obj2))
                        {
                            tuple.Item1.Remove(model.ZmdbId);
                            other = tuple.Item2;
                            break;
                        }
                    }
                    if (obj2 != null)
                    {
                        model.UpdateProperties((ZmdbPropertyList) obj2, other);
                    }
                    else
                    {
                        items.RemoveAt(i);
                    }
                }
                foreach (Tuple<ZmdbPropertyList, DeviceItemType> tuple2 in models)
                {
                    foreach (KeyValuePair<string, object> pair in tuple2.Item1)
                    {
                        DeviceItemViewModel model3 = new DeviceItemViewModel {
                            ZmdbId = pair.Key
                        };
                        DeviceItemViewModel item = model3;
                        item.UpdateProperties((ZmdbPropertyList) pair.Value, tuple2.Item2);
                        items.Add(item);
                    }
                }
            }
            else
            {
                items.Clear();
            }
            using (viewSource.View.DeferRefresh())
            {
                foreach (SortDescription description in viewSource.SortDescriptions)
                {
                    viewSource.View.SortDescriptions.Remove(description);
                    viewSource.View.SortDescriptions.Add(description);
                }
            }
        }

        private static ISyncable SyncableItemFromDeviceItem(DeviceItemViewModel deviceItem)
        {
            Dictionary props = PropertyMapping.MediaPropertiesFromZmdbProperties(deviceItem.ZmdbProperties);
            SyncableMediaItem item = new SyncableMediaItem(deviceItem.Name, props);
            ItemIdentifier identifier = new ItemIdentifier {
                IdValue = deviceItem.ZmdbId,
                ItemIdSpace = IdSpaceConstants.ZmdbId
            };
            item.ItemId = identifier;
            item.LastUpdate = DateTime.UtcNow;
            switch (deviceItem.ItemType)
            {
                case DeviceItemType.Photo:
                    item.Properties.SetObjectForKey(true, "Picture", null);
                    return item;

                case DeviceItemType.Music:
                case DeviceItemType.AudioPodcast:
                    item.Properties.SetObjectForKey(true, "Audio", null);
                    return item;

                case DeviceItemType.Video:
                case DeviceItemType.VideoPodcast:
                    item.Properties.SetObjectForKey(true, "Has Video", null);
                    return item;
            }
            return item;
        }

        public void TransferRingtones()
        {
            bool flag = false;
            foreach (ISyncSelectionOption option in this.controller.CurrentSyncPartnership.MusicAndMovieProvider.Ringtones.Options)
            {
                if (option.IsSelectedForSync.HasValue && option.IsSelectedForSync.Value)
                {
                    foreach (ISyncable syncable in option.ItemsToSync)
                    {
                        this.controller.CurrentSyncPartnership.Engine.ScheduleOperation(syncable, SyncOperationType.TransferTo);
                        flag = true;
                    }
                    continue;
                }
            }
            if (flag)
            {
                this.controller.MainViewModel.DoSyncCommand.Execute(SyncStartType.SendRingtones);
            }
        }

        public IDevice Device
        {
            get
            {
                return this.device;
            }
            set
            {
                if ((this.device == null) || (this.device != value))
                {
                    this.device = value;
                    this.OnDeviceChanged();
                    this.OnPropertyChanged("Device");
                }
            }
        }

        public string MoviesTVSeriesFilterString
        {
            get
            {
                return this.moviesTVSeriesFilterString;
            }
            set
            {
                this.moviesTVSeriesFilterString = value;
                if (value != null)
                {
                    this.moviesTVSeriesFilterStringUpper = StringUtilities.RemoveDiacritics(value.ToUpperInvariant());
                }
                else
                {
                    this.moviesTVSeriesFilterStringUpper = value;
                }
                this.moviesTVShowsRefresh.PerformAction();
                this.OnPropertyChanged("MoviesTVSeriesNoContent");
                this.OnPropertyChanged("MoviesTVSeriesFilterString");
            }
        }

        public string MoviesTVSeriesNoContent
        {
            get
            {
                if (!string.IsNullOrEmpty(this.MoviesTVSeriesFilterString))
                {
                    return StringUtilities.MergeBiDiMixedStrings(Microsoft.WPSync.UI.Properties.Resources.NoResultsText, this.MoviesTVSeriesFilterString, string.Empty);
                }
                return Microsoft.WPSync.UI.Properties.Resources.NoContentHeader;
            }
        }

        public CollectionViewSource MoviesTVShows
        {
            get
            {
                return this.moviesTVShows;
            }
        }

        public CollectionViewSource Music
        {
            get
            {
                return this.music;
            }
        }

        public string MusicFilterString
        {
            get
            {
                return this.musicFilterString;
            }
            set
            {
                this.musicFilterString = value;
                if (value != null)
                {
                    this.musicFilterStringUpper = StringUtilities.RemoveDiacritics(value.ToUpperInvariant());
                }
                else
                {
                    this.musicFilterStringUpper = value;
                }
                this.musicRefresh.PerformAction();
                this.OnPropertyChanged("MusicNoContent");
                this.OnPropertyChanged("MusicFilterString");
            }
        }

        public string MusicNoContent
        {
            get
            {
                if (!string.IsNullOrEmpty(this.MusicFilterString))
                {
                    return StringUtilities.MergeBiDiMixedStrings(Microsoft.WPSync.UI.Properties.Resources.NoResultsText, this.MusicFilterString, string.Empty);
                }
                return Microsoft.WPSync.UI.Properties.Resources.NoContentHeader;
            }
        }

        public string PhotosVideosFilterString
        {
            get
            {
                return this.photosVideosFilterString;
            }
            set
            {
                this.photosVideosFilterString = value;
                if (value != null)
                {
                    this.photosVideosFilterStringUpper = StringUtilities.RemoveDiacritics(value.ToUpperInvariant());
                }
                else
                {
                    this.photosVideosFilterStringUpper = value;
                }
                this.photoVideoAlbumsRefresh.PerformAction();
                this.OnPropertyChanged("PhotosVideosNoContent");
                this.OnPropertyChanged("PhotosVideosFilterString");
            }
        }

        public string PhotosVideosNoContent
        {
            get
            {
                if (!string.IsNullOrEmpty(this.PhotosVideosFilterString))
                {
                    return StringUtilities.MergeBiDiMixedStrings(Microsoft.WPSync.UI.Properties.Resources.NoResultsText, this.PhotosVideosFilterString, string.Empty);
                }
                return Microsoft.WPSync.UI.Properties.Resources.NoContentHeader;
            }
        }

        public CollectionViewSource PhotoVideoAlbums
        {
            get
            {
                return this.photoVideoAlbums;
            }
        }

        public CollectionViewSource Podcasts
        {
            get
            {
                return this.podcasts;
            }
        }

        public string PodcastsFilterString
        {
            get
            {
                return this.podcastsFilterString;
            }
            set
            {
                this.podcastsFilterString = value;
                if (value != null)
                {
                    this.podcastsFilterStringUpper = StringUtilities.RemoveDiacritics(value.ToUpperInvariant());
                }
                else
                {
                    this.podcastsFilterStringUpper = value;
                }
                this.podcastsRefresh.PerformAction();
                this.OnPropertyChanged("PodcastsNoContent");
                this.OnPropertyChanged("PodcastsFilterString");
            }
        }

        public string PodcastsNoContent
        {
            get
            {
                if (!string.IsNullOrEmpty(this.PodcastsFilterString))
                {
                    return StringUtilities.MergeBiDiMixedStrings(Microsoft.WPSync.UI.Properties.Resources.NoResultsText, this.PodcastsFilterString, string.Empty);
                }
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

