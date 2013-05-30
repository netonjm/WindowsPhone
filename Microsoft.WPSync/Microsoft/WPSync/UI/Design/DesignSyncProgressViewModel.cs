namespace Microsoft.WPSync.UI.Design
{
    using Microsoft.WPSync.UI;
    using Microsoft.WPSync.UI.ViewModels;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;

    public class DesignSyncProgressViewModel : SyncProgressViewModel
    {
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"), SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId="Microsoft.WPSync.UI.SyncProgressViewModel.set_Caption(System.String)")]
        public DesignSyncProgressViewModel(IMainController controller) : this(controller, true)
        {
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"), SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId="Microsoft.WPSync.UI.SyncProgressViewModel.set_Caption(System.String)"), SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId="Microsoft.WPSync.UI.ViewModels.SyncProgressViewModel.set_Caption(System.String)")]
        public DesignSyncProgressViewModel(IMainController controller, bool visible) : base(controller)
        {
            base.Header = "Doing things ...";
            base.SubHeader = "42 Left";
            base.Caption = "Loading some data (for design)...";
            if (visible)
            {
                base.Visibility = Visibility.Visible;
            }
            else
            {
                base.Visibility = Visibility.Hidden;
            }
            base.CurrentProgress = 50f;
            base.ProgressLeft = 50f;
        }

        public void SetHeader(string headerText)
        {
            base.Header = headerText;
        }

        public void SetVisibility(bool state)
        {
            if (state)
            {
                base.Visibility = Visibility.Visible;
            }
            else
            {
                base.Visibility = Visibility.Hidden;
            }
        }
    }
}

