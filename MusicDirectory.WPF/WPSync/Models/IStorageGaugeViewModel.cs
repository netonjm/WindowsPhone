using System.Windows.Shapes;

namespace Microsoft.WPSync.UI.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;

    public interface IStorageGaugeViewModel
    {
        string CurrentStorageArea { get; }

        string FreeBytesString { get; }

        [SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly")]
        //StorageGaugePanel Panel { set; }

        [SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly")]
        double PanelWidth { set; }

        ICollection<string> StorageAreas { get; }

        ICollection<Rectangle> StorageBars { get; }

        ICollection<UIElement> StorageLegends { get; }

        string TotalBytesString { get; }

        string UsedBytesString { get; }

        System.Windows.Visibility Visibility { get; }
    }
}

