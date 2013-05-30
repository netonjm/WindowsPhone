namespace Microsoft.WPSync.UI.Design
{
    using Microsoft.WPSync.Shared;
    using Microsoft.WPSync.Sync.Rules;
    using Microsoft.WPSync.UI.Properties;
    using Microsoft.WPSync.UI.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Windows.Data;

    internal class DesignContentViewModel : PropChangeNotifier, IMediaContentViewModel, INotifyPropertyChanged
    {
        public DesignContentViewModel()
        {
            this.SyncRules = new DesignSyncRules();
            List<SelectableOptionViewModel> list = new List<SelectableOptionViewModel>();
            AddOption(list, "REM").CreateChildSelectableOption("Album", "Automatic for the People");
            AddOption(list, "U2").CreateChildSelectableOption("Album", "Joshua Tree");
            AddOption(list, "Michael Jackson").CreateChildSelectableOption("Album", "Thriller");
            AddOption(list, "Pick Floyd").CreateChildSelectableOption("Album", "The Wall");
            AddOption(list, "Journey").CreateChildSelectableOption("Album", "Faithfully");
            this.Artists = AddCollectionViewSource(list);
            list = new List<SelectableOptionViewModel>();
            DesignSyncSelectionOption option = AddOption(list, "Pop");
            option = AddOption(list, "Classical");
            option = AddOption(list, "90s");
            option = AddOption(list, "80s");
            option = AddOption(list, "Classic Rock");
            this.Genres = AddCollectionViewSource(list);
            list = new List<SelectableOptionViewModel>();
            option = AddOption(list, "My Favorites");
            option = AddOption(list, "Party mix");
            option = AddOption(list, "Newest albums");
            option = AddOption(list, "Chill");
            option = AddOption(list, "Jed's list");
            this.Playlists = AddCollectionViewSource(list);
            list = new List<SelectableOptionViewModel>();
            option = AddOption(list, "Bill and Ted's Great Adventure");
            option = AddOption(list, "Jurasic Park");
            option = AddOption(list, "Raider's of the Lost Ark");
            option = AddOption(list, "The Muppet Movie");
            option = AddOption(list, "Columbo");
            option = AddOption(list, "American Idol");
            option = AddOption(list, "Star Trek: Next Generation");
            this.MoviesTVShows = AddCollectionViewSource(list);
            list = new List<SelectableOptionViewModel>();
            option = AddOption(list, "Wait Wait Don't Tell Me");
            option = AddOption(list, "This American Life");
            option = AddOption(list, "Windows Phone Basics");
            this.Podcasts = AddCollectionViewSource(list);
            list = new List<SelectableOptionViewModel>();
            option = AddOption(list, "Doh!");
            option = AddOption(list, "Hah hah!");
            option = AddOption(list, "Can't Touch This");
            this.Ringtones = AddCollectionViewSource(list);
            list = new List<SelectableOptionViewModel>();
            option = AddOption(list, "My Pictures (no items)");
            AddOption(list, "Public Pictures").CreateChildSelectableOption("Album", "Summer Vacation");
            this.PhotoVideoAlbums = AddCollectionViewSource(list);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static CollectionViewSource AddCollectionViewSource(List<SelectableOptionViewModel> collection)
        {
            CollectionViewSource source = new CollectionViewSource {
                Source = collection
            };
            source.View.SortDescriptions.Add(new SortDescription("Label", ListSortDirection.Ascending));
            source.View.Refresh();
            foreach (SelectableOptionViewModel model in collection)
            {
                if (model.ChildrenCount > 0)
                {
                    model.ResetView();
                }
            }
            return source;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static DesignSyncSelectionOption AddOption(List<SelectableOptionViewModel> list, string name)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }
            DesignSyncSelectionOption option = new DesignSyncSelectionOption(name) {
                Label = name
            };
            list.Add(new SelectableOptionViewModel(option, null, ""));
            return option;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public CollectionViewSource Artists { get; set; }

        public bool ExcludedDrmMovies
        {
            get
            {
                return true;
            }
        }

        public bool ExcludedDrmMusic
        {
            get
            {
                return true;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public CollectionViewSource Genres { get; set; }

        public string MoviesTVSeriesFilterString { get; set; }

        public string MoviesTVSeriesNoContent
        {
            get
            {
                return Resources.NoContentHeader;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public CollectionViewSource MoviesTVShows { get; set; }

        public string MusicFilterString { get; set; }

        public string MusicNoContent
        {
            get
            {
                return Resources.NoContentHeader;
            }
        }

        public string PhotosVideosFilterString { get; set; }

        public string PhotosVideosNoContent
        {
            get
            {
                return Resources.NoContentHeader;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public CollectionViewSource PhotoVideoAlbums { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public CollectionViewSource Playlists { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public CollectionViewSource Podcasts { get; set; }

        public string PodcastsFilterString { get; set; }

        public string PodcastsNoContent
        {
            get
            {
                return Resources.NoContentHeader;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public CollectionViewSource Ringtones { get; set; }

        public string RingtonesFilterString { get; set; }

        public string RingtonesNoContent
        {
            get
            {
                return Resources.NoContentHeader;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ISyncRules SyncRules { get; set; }
    }
}

