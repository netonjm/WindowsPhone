namespace Microsoft.WPSync.UI.ViewModels
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;

    public class PodcastsSelectionItem
    {
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="unplayed")]
        public PodcastsSelectionItem(string label, bool unplayedOnly, int syncCount)
        {
            this.Label = label;
            this.UnplayedOnly = unplayedOnly;
            this.SyncCount = syncCount;
        }

        public override string ToString()
        {
            return this.Label;
        }

        public string Label { get; set; }

        public int SyncCount { get; set; }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Unplayed")]
        public bool UnplayedOnly { get; set; }
    }
}

