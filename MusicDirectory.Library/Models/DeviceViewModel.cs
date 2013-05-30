using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WPSync.Device;
using Microsoft.WPSync.UI.ViewModels;
using Windows.UI.Xaml.Data;
using System.ComponentModel;
using Microsoft.WPSync.Shared;

namespace MusicDirectory.WindowsStore.Models
{
    class DeviceViewModel : PropChangeNotifier, IDeviceViewModel
    {
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

        public IDevice Device { get; set; }
        public string MoviesTVSeriesFilterString { get; set; }
        public string MoviesTVSeriesNoContent { get; private set; }
        public CollectionViewSource MoviesTVShows { get; private set; }
        CollectionViewSource IDeviceViewModel.Music
        {
            get { return Music; }
        }

        public string MusicFilterString { get; set; }
        public string MusicNoContent { get; private set; }
        public string PhotosVideosFilterString { get; set; }
        public string PhotosVideosNoContent { get; private set; }
        public CollectionViewSource PhotoVideoAlbums { get; private set; }
        public CollectionViewSource Podcasts { get; private set; }
        public string PodcastsFilterString { get; set; }
        public string PodcastsNoContent { get; private set; }

        public IDevicePropertiesViewModel Props { get; private set; }
        private CollectionViewSource music;
        public CollectionViewSource Music
        {
            get
            {
                return this.music;
            }
        }

    }
}
