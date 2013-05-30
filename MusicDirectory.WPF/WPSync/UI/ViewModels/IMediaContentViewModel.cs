namespace Microsoft.WPSync.UI.ViewModels
{
    using Microsoft.WPSync.Sync.Rules;
    using System;
    using System.ComponentModel;
    using System.Windows.Data;

    public interface IMediaContentViewModel : INotifyPropertyChanged
    {
        CollectionViewSource Artists { get; }

        bool ExcludedDrmMovies { get; }

        bool ExcludedDrmMusic { get; }

        CollectionViewSource Genres { get; }

        string MoviesTVSeriesFilterString { get; set; }

        string MoviesTVSeriesNoContent { get; }

        CollectionViewSource MoviesTVShows { get; }

        string MusicFilterString { get; set; }

        string MusicNoContent { get; }

        string PhotosVideosFilterString { get; set; }

        string PhotosVideosNoContent { get; }

        CollectionViewSource PhotoVideoAlbums { get; }

        CollectionViewSource Playlists { get; }

        CollectionViewSource Podcasts { get; }

        string PodcastsFilterString { get; set; }

        string PodcastsNoContent { get; }

        CollectionViewSource Ringtones { get; }

        string RingtonesFilterString { get; set; }

        string RingtonesNoContent { get; }

        ISyncRules SyncRules { get; }
    }
}

