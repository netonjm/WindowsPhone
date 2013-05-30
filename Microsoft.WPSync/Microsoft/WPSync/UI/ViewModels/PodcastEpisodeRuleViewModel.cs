using Microsoft.WPSync.Properties;

namespace Microsoft.WPSync.UI.ViewModels
{
    using Microsoft.WPSync.Shared;
    using Microsoft.WPSync.Sync.Rules;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Threading;

    public class PodcastEpisodeRuleViewModel : PropChangeNotifier
    {
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        private static readonly List<PodcastsSelectionItem> DefaultEpisodeRules = new List<PodcastsSelectionItem> { new PodcastsSelectionItem(Resources.AllEpisodesText, true, 0x7fffffff), new PodcastsSelectionItem(Resources.Recent1EpisodeText, false, 1), new PodcastsSelectionItem(Resources.Recent3EpisodeText, true, 3), new PodcastsSelectionItem(Resources.Recent5EpisodeText, true, 5), new PodcastsSelectionItem(Resources.AllUnplayedText, true, 0x7fffffff), new PodcastsSelectionItem(Resources.Unplayed1Text, true, 1), new PodcastsSelectionItem(Resources.Unplayed3Text, true, 3), new PodcastsSelectionItem(Resources.Unplayed5Text, true, 5) };
        private bool doesEpisodeSyncRuleExist;
        private ICollection<PodcastsSelectionItem> episodeRules;
        private bool propChangesDisabled;
        private PodcastsSelectionItem selectedEpisodeRule;
        private ISyncRules syncRules;

        private void OnDoesEpisodeSyncRuleExistChanged()
        {
            if (!this.DoesEpisodeSyncRuleExist)
            {
                this.SelectedEpisodeRule = null;
            }
            else
            {
                this.SelectedEpisodeRule = DefaultEpisodeRules[0];
            }
        }

        private void OnEpisodeRuleChanged()
        {
            if (this.SyncRules != null)
            {
                bool unplayedOnly;
                int syncCount;
                if (this.SelectedEpisodeRule != null)
                {
                    unplayedOnly = this.SelectedEpisodeRule.UnplayedOnly;
                    syncCount = this.SelectedEpisodeRule.SyncCount;
                }
                else
                {
                    unplayedOnly = false;
                    syncCount = 0x7fffffff;
                }
                this.SyncRules.SyncUnplayedPodcastsOnly = unplayedOnly;
                this.SyncRules.PodcastSyncCount = syncCount;
            }
        }

        private void OnGlobalRuleChanged(object sender, PropertyChangedEventArgs e)
        {
            //Dispatcher.CurrentDispatcher.BeginInvoke(delegate {
            //    string str;
            //    if (((str = e.PropertyName) != null) && (str == "IsPodcastSyncEnabled"))
            //    {
            //        this.OnSyncPodcastsChanged();
            //    }
            //}, new object[0]);
        }

        private void OnSyncPodcastsChanged()
        {
            if (!this.SyncRules.IsPodcastSyncEnabled)
            {
                this.propChangesDisabled = true;
                this.DoesEpisodeSyncRuleExist = false;
                this.EpisodeRules = null;
                this.SelectedEpisodeRule = null;
                this.propChangesDisabled = false;
            }
            else
            {
                this.OnSyncRulesChanged();
            }
        }

        private void OnSyncRulesChanged()
        {
            bool unplayedOnly = false;
            int episodeCount = 0;
            if (this.SyncRules != null)
            {
                unplayedOnly = this.SyncRules.SyncUnplayedPodcastsOnly;
                episodeCount = this.SyncRules.PodcastSyncCount;
            }
            if (((this.SyncRules == null) || !this.SyncRules.IsPodcastSyncEnabled) || (!unplayedOnly && (episodeCount == 0)))
            {
                this.DoesEpisodeSyncRuleExist = false;
                this.EpisodeRules = null;
                this.SelectedEpisodeRule = null;
            }
            else
            {
                this.EpisodeRules = DefaultEpisodeRules;
                this.DoesEpisodeSyncRuleExist = true;
                this.SelectedEpisodeRule = DefaultEpisodeRules.Find(r => (r.UnplayedOnly == unplayedOnly) && (r.SyncCount == episodeCount));
            }
        }

        public bool DoesEpisodeSyncRuleExist
        {
            get
            {
                return this.doesEpisodeSyncRuleExist;
            }
            set
            {
                this.doesEpisodeSyncRuleExist = value;
                if (!this.propChangesDisabled)
                {
                    this.OnDoesEpisodeSyncRuleExistChanged();
                }
                this.OnPropertyChanged("DoesEpisodeSyncRuleExist");
            }
        }

        public ICollection<PodcastsSelectionItem> EpisodeRules
        {
            get
            {
                return this.episodeRules;
            }
            private set
            {
                this.episodeRules = value;
                this.OnPropertyChanged("EpisodeRules");
            }
        }

        public PodcastsSelectionItem SelectedEpisodeRule
        {
            get
            {
                return this.selectedEpisodeRule;
            }
            set
            {
                this.selectedEpisodeRule = value;
                if (!this.propChangesDisabled)
                {
                    this.OnEpisodeRuleChanged();
                }
                this.OnPropertyChanged("SelectedEpisodeRule");
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
                if (this.syncRules != null)
                {
                    this.syncRules.PropertyChanged -= new PropertyChangedEventHandler(this.OnGlobalRuleChanged);
                }
                this.syncRules = value;
                this.propChangesDisabled = true;
                this.OnSyncRulesChanged();
                this.propChangesDisabled = false;
                if (this.syncRules != null)
                {
                    this.syncRules.PropertyChanged += new PropertyChangedEventHandler(this.OnGlobalRuleChanged);
                }
                this.OnPropertyChanged("SyncRules");
            }
        }
    }
}

