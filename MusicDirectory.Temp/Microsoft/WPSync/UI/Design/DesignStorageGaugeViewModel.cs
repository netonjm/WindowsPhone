namespace Microsoft.WPSync.UI.Design
{
    using Microsoft.WPSync.UI;
    using Microsoft.WPSync.UI.ViewModels;
    using System;
    using System.Diagnostics.CodeAnalysis;

    internal class DesignStorageGaugeViewModel : StorageGaugeViewModel
    {
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public DesignStorageGaugeViewModel(IMainController controller) : base(controller)
        {
            base.ResetModelState();
        }
    }
}

