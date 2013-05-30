using Windows.UI.Xaml.Data;
using Microsoft.WPSync.Device;
using System;
using System.Collections.Generic;

namespace Microsoft.WPSync.UI.ViewModels
{


    public interface IDeviceViewModel
    {
        void CopyItemsToPC(IEnumerable<DeviceItemViewModel> list);
        void DeleteItems(IEnumerable<DeviceItemViewModel> list);
        void TransferRingtones();

        IDevice Device { get; set; }

        string MoviesTVSeriesFilterString { get; set; }

        string MoviesTVSeriesNoContent { get; }

        CollectionViewSource MoviesTVShows { get; }

        CollectionViewSource Music { get; }

        string MusicFilterString { get; set; }

        string MusicNoContent { get; }

        string PhotosVideosFilterString { get; set; }

        string PhotosVideosNoContent { get; }

        CollectionViewSource PhotoVideoAlbums { get; }

        CollectionViewSource Podcasts { get; }

        string PodcastsFilterString { get; set; }

        string PodcastsNoContent { get; }

        IDevicePropertiesViewModel Props { get; }
    }
}

