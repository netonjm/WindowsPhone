namespace Microsoft.WPSync.UI.ViewModels
{
    using Microsoft.WPSync.Shared;
    using Microsoft.WPSync.Sync.Engine;
    using Microsoft.WPSync.Sync.Rules;
    using Microsoft.WPSync.UI;
    using Microsoft.WPSync.UI.Models;
    using Microsoft.WPSync.UI.Properties;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Data;

    public class MediaContentViewModel : PropChangeNotifier, IMediaContentViewModel, INotifyPropertyChanged
    {
        private CollectionViewSource artists;
        private SyncOptionListViewModel artistsSource;
        private readonly IMainController controller;
        private ISyncPartnership currentPartnership;
        private CollectionViewSource genres;
        private SyncOptionListViewModel genresSource;
        private static IComparer labelSorter = new LabelSorter();
        private string moviesTVSeriesFilterString;
        private string moviesTVSeriesFilterStringUpper;
        private CollectionViewSource moviesTVShows;
        private SyncOptionListViewModel moviesTVShowsSource;
        private string musicFilterString;
        private string musicFilterStringUpper;
        private IMusicSyncSource musicProvider;
        private string photosVideosFilterString;
        private string photosVideosFilterStringUpper;
        private CollectionViewSource photoVideoAlbums;
        private SyncOptionListViewModel photoVideoAlbumsSource;
        private IPictureSyncSource pictureProvider;
        private CollectionViewSource playlists;
        private SyncOptionListViewModel playlistsSource;
        private CollectionViewSource podcasts;
        private string podcastsFilterString;
        private string podcastsFilterStringUpper;
        private SyncOptionListViewModel podcastsSource;
        private CollectionViewSource ringtones;
        private string ringtonesFilterString;
        private string ringtonesFilterStringUpper;
        private SyncOptionListViewModel ringtonesSource;
        private ISyncRules syncRules;

        public MediaContentViewModel(IMainController controller)
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }
            this.controller = controller;
            controller.PropertyChanged += new PropertyChangedEventHandler(this.OnControllerPropertyChanged);
            this.InitCollectionViews();
        }

        private static SyncOptionListViewModel GetViewModel(ISyncOptionList model, string filterString)
        {
            return new SyncOptionListViewModel(model, filterString);
        }

        private void InitCollectionViews()
        {
            this.artists = new CollectionViewSource();
            this.genres = new CollectionViewSource();
            this.playlists = new CollectionViewSource();
            this.moviesTVShows = new CollectionViewSource();
            this.podcasts = new CollectionViewSource();
            this.ringtones = new CollectionViewSource();
            this.photoVideoAlbums = new CollectionViewSource();
        }

        private void OnControllerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string str;
            if (((str = e.PropertyName) != null) && (str == "CurrentSyncPartnership"))
            {
                this.OnPartnershipChanged();
            }
        }

        private void OnMoviesTVSeriesFilterStringChanged()
        {
            if ((this.moviesTVShows != null) && (this.moviesTVShows.View != null))
            {
                UpdateFilterStrings(this.moviesTVShowsSource, this.moviesTVSeriesFilterStringUpper);
                this.moviesTVShows.View.Refresh();
            }
        }

        private void OnMusicAndMovieProviderChanged()
        {
            //this.controller.Dispatcher.BeginInvoke(delegate {
            //    this.ResetMusicAndMovies();
            //}, new object[0]);
        }

        private void OnMusicAndMovieProviderModelDataChanged(object sender, ModelDataChangedEventArgs e)
        {
            //this.controller.Dispatcher.BeginInvoke(delegate {
            //    this.OnMusicAndMovieProviderChanged();
            //}, new object[0]);
        }

        private void OnMusicAndMovieProviderPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string str;
            if (((str = e.PropertyName) != null) && ((str == "ExcludedDrmMusic") || (str == "ExcludedDrmMovies")))
            {
                this.OnPropertyChanged(e.PropertyName);
            }
        }

        private void OnMusicFilterStringChanged()
        {
            if ((((this.artists != null) && (this.artists.View != null)) && ((this.genres != null) && (this.genres.View != null))) && ((this.playlists != null) && (this.playlists.View != null)))
            {
                UpdateFilterStrings(this.artistsSource, this.musicFilterStringUpper);
                this.artists.View.Refresh();
                UpdateFilterStrings(this.genresSource, this.musicFilterStringUpper);
                this.genres.View.Refresh();
                UpdateFilterStrings(this.playlistsSource, this.musicFilterStringUpper);
                this.playlists.View.Refresh();
            }
        }

        private void OnPartnershipChanged()
        {
            if (this.currentPartnership != null)
            {
                this.currentPartnership.PropertyChanged -= new PropertyChangedEventHandler(this.OnPartnershipPropertyChanged);
                this.SyncRules.PropertyChanged -= new PropertyChangedEventHandler(this.OnSyncRulesPropertyChanged);
            }
            this.currentPartnership = this.controller.CurrentSyncPartnership;
            if (this.currentPartnership != null)
            {
                this.currentPartnership.PropertyChanged += new PropertyChangedEventHandler(this.OnPartnershipPropertyChanged);
                this.SyncRules = this.currentPartnership.RuleManager;
                this.SyncRules.PropertyChanged += new PropertyChangedEventHandler(this.OnSyncRulesPropertyChanged);
            }
            else
            {
                this.SyncRules = null;
            }
            this.ResetMusicAndMovieProvider();
            this.ResetPhotoAndVideoProvider();
        }

        private void OnPartnershipPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Action method = null;
            Action action2 = null;
            string propertyName = e.PropertyName;
            if (propertyName != null)
            {
                if (!(propertyName == "MusicAndMovieProvider"))
                {
                    if (!(propertyName == "PhotoAndVideoProvider"))
                    {
                        return;
                    }
                }
                else
                {
                    if (method == null)
                    {
                        method = delegate {
                            this.ResetMusicAndMovieProvider();
                        };
                    }
                    this.controller.Dispatcher.Invoke(method, new object[0]);
                    return;
                }
                if (action2 == null)
                {
                    action2 = delegate {
                        this.ResetPhotoAndVideoProvider();
                    };
                }
                this.controller.Dispatcher.Invoke(action2, new object[0]);
            }
        }

        private void OnPhotoAndVideoProviderChanged()
        {
            //this.controller.Dispatcher.BeginInvoke(delegate {
            //    this.ResetPhotosAndVideos();
            //}, new object[0]);
        }

        private void OnPhotoAndVideoProviderModelDataChanged(object sender, ModelDataChangedEventArgs e)
        {
            //this.controller.Dispatcher.BeginInvoke(delegate {
            //    this.OnPhotoAndVideoProviderChanged();
            //}, new object[0]);
        }

        private void OnPhotosVideosFilterStringChanged()
        {
            if ((this.photoVideoAlbums != null) && (this.photoVideoAlbums.View != null))
            {
                UpdateFilterStrings(this.photoVideoAlbumsSource, this.photosVideosFilterStringUpper);
                this.photoVideoAlbums.View.Refresh();
            }
        }

        private void OnPodcastsFilterStringChanged()
        {
            if ((this.podcasts != null) && (this.podcasts.View != null))
            {
                UpdateFilterStrings(this.podcastsSource, this.podcastsFilterStringUpper);
                this.podcasts.View.Refresh();
            }
        }

        private void OnRingtonesFilterStringChanged()
        {
            if ((this.ringtones != null) && (this.ringtones.View != null))
            {
                UpdateFilterStrings(this.ringtonesSource, this.ringtonesFilterStringUpper);
                this.ringtones.View.Refresh();
            }
        }

        private void OnSyncRulesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsMusicSyncEnabled":
                case "SyncAllMusic":
                    UpdateAutomaticSelections(this.artistsSource, this.SyncRules.IsMusicSyncEnabled, this.SyncRules.SyncAllMusic);
                    UpdateAutomaticSelections(this.playlistsSource, this.SyncRules.IsMusicSyncEnabled, this.SyncRules.SyncAllMusic);
                    UpdateAutomaticSelections(this.genresSource, this.SyncRules.IsMusicSyncEnabled, this.SyncRules.SyncAllMusic);
                    return;

                case "IsTVMoviesSyncEnabled":
                case "SyncAllTvMovies":
                    UpdateAutomaticSelections(this.moviesTVShowsSource, this.SyncRules.IsTVMoviesSyncEnabled, this.SyncRules.SyncAllTvMovies);
                    return;

                case "IsPodcastSyncEnabled":
                case "SyncAllPodcasts":
                    UpdateAutomaticSelections(this.podcastsSource, this.SyncRules.IsPodcastSyncEnabled, this.SyncRules.SyncAllPodcasts);
                    return;

                case "SyncUnplayedPodcastsOnly":
                case "PodcastSyncCount":
                case "SyncIncludeVideos":
                    break;

                case "IsPhotoVideoSyncEnabled":
                case "SyncAllPhotosVideos":
                    UpdateAutomaticSelections(this.photoVideoAlbumsSource, this.SyncRules.IsPhotoVideoSyncEnabled, this.SyncRules.SyncAllPhotosVideos);
                    break;

                default:
                    return;
            }
        }

        private void ResetArtists()
        {
            if (this.artistsSource != null)
            {
                this.artistsSource.Dispose();
                this.artistsSource = null;
            }
            if (this.MusicAndMovieProvider != null)
            {
                this.artistsSource = GetViewModel(this.MusicAndMovieProvider.Artists, this.musicFilterStringUpper);
                ResetPivotOptions(this.artistsSource, this.artists, this.SyncRules.IsMusicSyncEnabled, this.SyncRules.SyncAllMusic);
            }
            else
            {
                this.artists.Source = null;
            }
        }

        private void ResetGenres()
        {
            if (this.genresSource != null)
            {
                this.genresSource.Dispose();
                this.genresSource = null;
            }
            if (this.MusicAndMovieProvider != null)
            {
                this.genresSource = GetViewModel(this.MusicAndMovieProvider.Genres, this.musicFilterStringUpper);
                ResetPivotOptions(this.genresSource, this.genres, this.SyncRules.IsMusicSyncEnabled, this.SyncRules.SyncAllMusic);
            }
            else
            {
                this.genres.Source = null;
            }
        }

        private void ResetMoviesTVShows()
        {
            if (this.moviesTVShowsSource != null)
            {
                this.moviesTVShowsSource.Dispose();
                this.moviesTVShowsSource = null;
            }
            if (this.MusicAndMovieProvider != null)
            {
                this.moviesTVShowsSource = GetViewModel(this.MusicAndMovieProvider.MoviesTVShows, this.moviesTVSeriesFilterStringUpper);
                ResetPivotOptions(this.moviesTVShowsSource, this.moviesTVShows, this.SyncRules.IsTVMoviesSyncEnabled, this.SyncRules.SyncAllTvMovies);
            }
            else
            {
                this.moviesTVShows.Source = null;
            }
        }

        private void ResetMusicAndMovieProvider()
        {
            if (this.MusicAndMovieProvider != null)
            {
                this.MusicAndMovieProvider.ModelDataChanged -= new EventHandler<ModelDataChangedEventArgs>(this.OnMusicAndMovieProviderModelDataChanged);
                this.MusicAndMovieProvider.PropertyChanged -= new PropertyChangedEventHandler(this.OnMusicAndMovieProviderPropertyChanged);
            }
            if (this.currentPartnership != null)
            {
                this.MusicAndMovieProvider = this.controller.CurrentSyncPartnership.MusicAndMovieProvider;
                if (this.MusicAndMovieProvider != null)
                {
                    this.MusicAndMovieProvider.ModelDataChanged += new EventHandler<ModelDataChangedEventArgs>(this.OnMusicAndMovieProviderModelDataChanged);
                    this.MusicAndMovieProvider.PropertyChanged += new PropertyChangedEventHandler(this.OnMusicAndMovieProviderPropertyChanged);
                }
            }
            else
            {
                this.MusicAndMovieProvider = null;
            }
        }

        private void ResetMusicAndMovies()
        {
            this.ResetArtists();
            this.ResetGenres();
            this.ResetPlaylists();
            this.ResetMoviesTVShows();
            this.ResetPodcasts();
            this.ResetRingtones();
        }

        private void ResetPhotoAndVideoProvider()
        {
            if (this.PhotoAndVideoProvider != null)
            {
                this.PhotoAndVideoProvider.ModelDataChanged -= new EventHandler<ModelDataChangedEventArgs>(this.OnPhotoAndVideoProviderModelDataChanged);
            }
            if (this.currentPartnership != null)
            {
                this.PhotoAndVideoProvider = this.currentPartnership.PhotoAndVideoProvider;
                if (this.PhotoAndVideoProvider != null)
                {
                    this.PhotoAndVideoProvider.ModelDataChanged += new EventHandler<ModelDataChangedEventArgs>(this.OnPhotoAndVideoProviderModelDataChanged);
                }
            }
            else
            {
                this.PhotoAndVideoProvider = null;
            }
        }

        private void ResetPhotosAndVideos()
        {
            this.ResetPhotoVideoAlbums();
            this.OnPropertyChanged("PhotoVideoAlbums");
        }

        private void ResetPhotoVideoAlbums()
        {
            if (this.photoVideoAlbumsSource != null)
            {
                this.photoVideoAlbumsSource.Dispose();
                this.photoVideoAlbumsSource = null;
            }
            if (this.PhotoAndVideoProvider != null)
            {
                this.photoVideoAlbumsSource = GetViewModel(this.PhotoAndVideoProvider.PhotoVideoAlbums, this.photosVideosFilterStringUpper);
                ResetPivotOptions(this.photoVideoAlbumsSource, this.photoVideoAlbums, this.SyncRules.IsPhotoVideoSyncEnabled, this.SyncRules.SyncAllPhotosVideos);
            }
            else
            {
                this.photoVideoAlbums.Source = null;
            }
        }

        private static void ResetPivotOptions(SyncOptionListViewModel list, CollectionViewSource viewSource, bool enabled, bool all)
        {
            UpdateAutomaticSelections(list, enabled, all);
            viewSource.Source = list;
            ListCollectionView view = (ListCollectionView) viewSource.View;
            if (view != null)
            {
                using (view.DeferRefresh())
                {
                    view.CustomSort = labelSorter;
                    view.Filter = new Predicate<object>(SelectableOptionViewModel.DoesMatchFilter);
                }
            }
        }

        private void ResetPlaylists()
        {
            if (this.playlistsSource != null)
            {
                this.playlistsSource.Dispose();
                this.playlistsSource = null;
            }
            if (this.MusicAndMovieProvider != null)
            {
                this.playlistsSource = GetViewModel(this.MusicAndMovieProvider.Playlists, this.musicFilterStringUpper);
                ResetPivotOptions(this.playlistsSource, this.playlists, this.SyncRules.IsMusicSyncEnabled, this.SyncRules.SyncAllMusic);
            }
            else
            {
                this.playlists.Source = null;
            }
        }

        private void ResetPodcasts()
        {
            if (this.podcastsSource != null)
            {
                this.podcastsSource.Dispose();
                this.podcastsSource = null;
            }
            if (this.MusicAndMovieProvider != null)
            {
                this.podcastsSource = GetViewModel(this.MusicAndMovieProvider.Podcasts, this.podcastsFilterStringUpper);
                ResetPivotOptions(this.podcastsSource, this.podcasts, this.SyncRules.IsPodcastSyncEnabled, this.SyncRules.SyncAllPodcasts);
            }
            else
            {
                this.podcasts.Source = null;
            }
        }

        private void ResetRingtones()
        {
            if (this.ringtonesSource != null)
            {
                this.ringtonesSource.Dispose();
                this.ringtonesSource = null;
            }
            if (this.MusicAndMovieProvider != null)
            {
                this.ringtonesSource = GetViewModel(this.MusicAndMovieProvider.Ringtones, this.ringtonesFilterStringUpper);
                ResetPivotOptions(this.ringtonesSource, this.ringtones, true, false);
            }
            else
            {
                this.ringtones.Source = null;
            }
        }

        protected static void UpdateAutomaticSelections(SyncOptionListViewModel list, bool isSyncEnabled, bool syncAll)
        {
            if (list != null)
            {
                bool autoSelectOn = false;
                bool autoSelectValue = false;
                if (!isSyncEnabled)
                {
                    autoSelectOn = true;
                }
                else if (syncAll)
                {
                    autoSelectOn = true;
                    autoSelectValue = true;
                }
                list.SetAutoSelect(autoSelectOn, autoSelectValue);
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId="string")]
        protected static void UpdateFilterStrings(SyncOptionListViewModel list, string filterString)
        {
            if (list != null)
            {
                list.FilterString = filterString;
                foreach (SelectableOptionViewModel model in list)
                {
                    model.FilterString = filterString;
                }
            }
        }

        public CollectionViewSource Artists
        {
            get
            {
                return this.artists;
            }
        }

        public bool ExcludedDrmMovies
        {
            get
            {
                return ((this.MusicAndMovieProvider != null) && this.MusicAndMovieProvider.ExcludedDrmMovies);
            }
        }

        public bool ExcludedDrmMusic
        {
            get
            {
                return ((this.MusicAndMovieProvider != null) && this.MusicAndMovieProvider.ExcludedDrmMusic);
            }
        }

        public CollectionViewSource Genres
        {
            get
            {
                return this.genres;
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
                if (this.moviesTVSeriesFilterString != value)
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
                    this.OnMoviesTVSeriesFilterStringChanged();
                    this.OnPropertyChanged("MoviesTVSeriesNoContent");
                    this.OnPropertyChanged("MoviesTVSeriesFilterString");
                }
            }
        }

        public string MoviesTVSeriesNoContent
        {
            get
            {
                if (!string.IsNullOrEmpty(this.MoviesTVSeriesFilterString))
                {
                    return StringUtilities.MergeBiDiMixedStrings(Resources.NoResultsText, this.MoviesTVSeriesFilterString, string.Empty);
                }
                return Resources.NoContentHeader;
            }
        }

        public CollectionViewSource MoviesTVShows
        {
            get
            {
                return this.moviesTVShows;
            }
        }

        public IMusicSyncSource MusicAndMovieProvider
        {
            get
            {
                return this.musicProvider;
            }
            set
            {
                this.musicProvider = value;
                this.OnMusicAndMovieProviderChanged();
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
                if (this.musicFilterString != value)
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
                    this.OnMusicFilterStringChanged();
                    this.OnPropertyChanged("MusicNoContent");
                    this.OnPropertyChanged("MusicFilterString");
                }
            }
        }

        public string MusicNoContent
        {
            get
            {
                if (!string.IsNullOrEmpty(this.MusicFilterString))
                {
                    return StringUtilities.MergeBiDiMixedStrings(Resources.NoResultsText, this.MusicFilterString, string.Empty);
                }
                return Resources.NoContentHeader;
            }
        }

        public IPictureSyncSource PhotoAndVideoProvider
        {
            get
            {
                return this.pictureProvider;
            }
            set
            {
                this.pictureProvider = value;
                this.OnPhotoAndVideoProviderChanged();
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
                if (this.photosVideosFilterString != value)
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
                    this.OnPhotosVideosFilterStringChanged();
                    this.OnPropertyChanged("PhotosVideosNoContent");
                    this.OnPropertyChanged("PhotosVideosFilterString");
                }
            }
        }

        public string PhotosVideosNoContent
        {
            get
            {
                if (!string.IsNullOrEmpty(this.PhotosVideosFilterString))
                {
                    return StringUtilities.MergeBiDiMixedStrings(Resources.NoResultsText, this.PhotosVideosFilterString, string.Empty);
                }
                return Resources.NoContentHeader;
            }
        }

        public CollectionViewSource PhotoVideoAlbums
        {
            get
            {
                return this.photoVideoAlbums;
            }
        }

        public CollectionViewSource Playlists
        {
            get
            {
                return this.playlists;
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
                if (this.podcastsFilterString != value)
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
                    this.OnPodcastsFilterStringChanged();
                    this.OnPropertyChanged("PodcastsNoContent");
                    this.OnPropertyChanged("PodcastsFilterString");
                }
            }
        }

        public string PodcastsNoContent
        {
            get
            {
                if (!string.IsNullOrEmpty(this.PodcastsFilterString))
                {
                    return StringUtilities.MergeBiDiMixedStrings(Resources.NoResultsText, this.PodcastsFilterString, string.Empty);
                }
                return Resources.NoContentHeader;
            }
        }

        public CollectionViewSource Ringtones
        {
            get
            {
                return this.ringtones;
            }
        }

        public string RingtonesFilterString
        {
            get
            {
                return this.ringtonesFilterString;
            }
            set
            {
                if (this.ringtonesFilterString != value)
                {
                    this.ringtonesFilterString = value;
                    if (value != null)
                    {
                        this.ringtonesFilterStringUpper = StringUtilities.RemoveDiacritics(value.ToUpperInvariant());
                    }
                    else
                    {
                        this.ringtonesFilterStringUpper = value;
                    }
                    this.OnRingtonesFilterStringChanged();
                    this.OnPropertyChanged("RingtonesNoContent");
                    this.OnPropertyChanged("RingtonesFilterString");
                }
            }
        }

        public string RingtonesNoContent
        {
            get
            {
                if (!string.IsNullOrEmpty(this.RingtonesFilterString))
                {
                    return StringUtilities.MergeBiDiMixedStrings(Resources.NoResultsText, this.RingtonesFilterString, string.Empty);
                }
                return Resources.NoContentHeader;
            }
        }

        public ISyncRules SyncRules
        {
            get
            {
                return this.syncRules;
            }
            set
            {
                this.syncRules = value;
                this.OnPropertyChanged("SyncRules");
            }
        }
    }
}

