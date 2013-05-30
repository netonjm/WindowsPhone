namespace Microsoft.WPSync.UI.ViewModels
{
    using Microsoft.WPSync.Device;
    using Microsoft.WPSync.Logging;
    using Microsoft.WPSync.Shared;
    using Microsoft.WPSync.UI;
    using Microsoft.WPSync.UI.Models;
    using Microsoft.WPSync.UI.Properties;
    using Microsoft.WPSync.UI.Views;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Automation;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Windows.Threading;

    public class StorageGaugeViewModel : PropChangeNotifier, IStorageGaugeViewModel
    {
        private readonly IMainController controller;
        private ISyncPartnership currentPartnership;
        private string currentStorageArea = "";
        private ulong freeBytes;
        private string freeBytesString;
        private const double minimumBarWidth = 3.0;
        private ulong otherBytes;
        private string otherBytesString;
        private StorageGaugePanel panel;
        private double panelWidth = 300.0;
        private ICollection<string> storageAreas;
        private ulong totalBytes;
        private string totalBytesString;
        private string usedBytesString;
        private System.Windows.Visibility visibility = System.Windows.Visibility.Hidden;

        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies")]
        public StorageGaugeViewModel(IMainController controller)
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }
            this.controller = controller;
            this.StorageBarInfos = new Dictionary<string, StorageBarInfo>();
            this.InitStorageBars();
            controller.PropertyChanged += new PropertyChangedEventHandler(this.OnControllerPropertyChanged);
        }

        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies")]
        private void CalculateBars()
        {
            this.SetBytesFor(Microsoft.WPSync.UI.Properties.Resources.StorageGaugeFreeText, this.freeBytes);
            foreach (StorageBarInfo info in this.StorageBarInfos.Values)
            {
                Rectangle bar = info.Bar;
                if (info.BytesUsed > 0.0)
                {
                    bar.Width = Math.Round((double) ((info.BytesUsed / ((double) this.totalBytes)) * this.PanelWidth));
                    bar.Width = Math.Max(bar.Width, 3.0);
                }
            }
        }

        private static UIElement CreateLegend(string label, StorageBarInfo info, out TextBlock legendByte)
        {
            StackPanel panel3 = new StackPanel {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(30.0, 0.0, 0.0, 30.0)
            };
            StackPanel panel = panel3;
            Rectangle rectangle2 = new Rectangle {
                Height = 7.0,
                Width = 7.0,
                Fill = info.Color,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0.0, 6.0, 0.0, 0.0),
                SnapsToDevicePixels = true
            };
            Rectangle element = rectangle2;
            panel.Children.Add(element);
            StackPanel panel4 = new StackPanel {
                Orientation = Orientation.Vertical
            };
            StackPanel panel2 = panel4;
            panel.Children.Add(panel2);
            TextBlock block = new TextBlock {
                Text = label,
                Style = Application.Current.Resources["storageLegend"] as Style
            };
            panel2.Children.Add(block);
            TextBlock block2 = new TextBlock {
                Style = Application.Current.Resources["storageLegendSubtext"] as Style
            };
            legendByte = block2;
            AutomationProperties.SetAutomationId(legendByte, GetAutomationId(label));
            panel2.Children.Add(legendByte);
            return panel;
        }

        private void CreateStorageBarInfo(string label, Brush color)
        {
            TextBlock block;
            Rectangle rectangle2 = new Rectangle {
                Height = 6.0,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Fill = color,
                Margin = new Thickness(0.0, 0.0, 1.0, 0.0)
            };
            Rectangle rectangle = rectangle2;
            StorageBarInfo info2 = new StorageBarInfo {
                Color = color,
                Bar = rectangle
            };
            StorageBarInfo info = info2;
            info.Legend = CreateLegend(label, info, out block);
            info.LegendByte = block;
            this.StorageBarInfos[label] = info;
        }

        private void gaugeBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.PanelWidth = this.Panel.gaugeBar.ActualWidth;
        }

        private static string GetAutomationId(string label)
        {
            string str = string.Empty;
            if (label.Equals(Microsoft.WPSync.UI.Properties.Resources.MusicStorageText))
            {
                return "musicBytes";
            }
            if (label.Equals(Microsoft.WPSync.UI.Properties.Resources.PhotosStorageText))
            {
                return "photosBytes";
            }
            if (label.Equals(Microsoft.WPSync.UI.Properties.Resources.VideosStorageText))
            {
                return "videosBytes";
            }
            if (label.Equals(Microsoft.WPSync.UI.Properties.Resources.DocumentsText))
            {
                return "documentsBytes";
            }
            if (label.Equals(Microsoft.WPSync.UI.Properties.Resources.AppsStorageText))
            {
                return "appsBytes";
            }
            if (label.Equals(Microsoft.WPSync.UI.Properties.Resources.SystemStorageText))
            {
                return "systemBytes";
            }
            if (label.Equals(Microsoft.WPSync.UI.Properties.Resources.OtherStorageText))
            {
                return "otherBytes";
            }
            if (label.Equals(Microsoft.WPSync.UI.Properties.Resources.StorageGaugeFreeText))
            {
                str = "freeBytesLegend";
            }
            return str;
        }

        private StorageInfo GetStorageDeviceInfo(IDevice device)
        {
            StorageInfo info;
            if ((device.StorageDevices == null) || (device.StorageDevices.Count < 1))
            {
                info = new StorageInfo();
                this.currentStorageArea = "";
                this.OnPropertyChanged("CurrentStorageArea");
                return info;
            }
            if ((this.CurrentStorageArea == null) || !device.StorageDevices.TryGetValue(this.CurrentStorageArea, out info))
            {
                info = device.StorageDevices.FirstOrDefault<KeyValuePair<string, StorageInfo>>().Value;
                this.currentStorageArea = device.StorageDevices.FirstOrDefault<KeyValuePair<string, StorageInfo>>().Key;
                this.OnPropertyChanged("CurrentStorageArea");
            }
            return info;
        }

        private void InitStorageBars()
        {
            this.CreateStorageBarInfo(Microsoft.WPSync.UI.Properties.Resources.SystemStorageText, new SolidColorBrush(StorageColors.System));
            this.CreateStorageBarInfo(Microsoft.WPSync.UI.Properties.Resources.MusicStorageText, new SolidColorBrush(StorageColors.Music));
            this.CreateStorageBarInfo(Microsoft.WPSync.UI.Properties.Resources.PhotosStorageText, new SolidColorBrush(StorageColors.Photos));
            this.CreateStorageBarInfo(Microsoft.WPSync.UI.Properties.Resources.VideosStorageText, new SolidColorBrush(StorageColors.Video));
            this.CreateStorageBarInfo(Microsoft.WPSync.UI.Properties.Resources.DocumentsText, new SolidColorBrush(StorageColors.Documents));
            this.CreateStorageBarInfo(Microsoft.WPSync.UI.Properties.Resources.PodcastsStorageText, new SolidColorBrush(StorageColors.Podcasts));
            this.CreateStorageBarInfo(Microsoft.WPSync.UI.Properties.Resources.AppsStorageText, new SolidColorBrush(StorageColors.Apps));
            this.CreateStorageBarInfo(Microsoft.WPSync.UI.Properties.Resources.OtherStorageText, new SolidColorBrush(StorageColors.Other));
            this.CreateStorageBarInfo(Microsoft.WPSync.UI.Properties.Resources.StorageGaugeFreeText, new SolidColorBrush(StorageColors.Free));
        }

        private void OnControllerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Action method = null;
            string str;
            if (((this.Panel != null) && ((str = e.PropertyName) != null)) && (str == "CurrentSyncPartnership"))
            {
                if (method == null)
                {
                    method = delegate {
                        this.OnPartnershipChanged();
                    };
                }
                this.Panel.Dispatcher.BeginInvoke(DispatcherPriority.Normal, method);
            }
        }

        private void OnDevicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Action method = null;
            if (e.PropertyName == "StorageDevices")
            {
                if (method == null)
                {
                    method = delegate {
                        this.UpdateStorageGauge();
                    };
                }
                this.Panel.Dispatcher.Invoke(DispatcherPriority.Normal, method);
            }
        }

        private void OnPanelAttached()
        {
            this.panel.gaugeBar.SizeChanged += new SizeChangedEventHandler(this.gaugeBar_SizeChanged);
            this.ResetModelState();
            this.UpdateStorageGauge();
        }

        private void OnPartnershipChanged()
        {
            if (this.currentPartnership != null)
            {
                this.currentPartnership.StateChanged -= new EventHandler<PartnershipStateChangeEventArgs>(this.OnPartnershipStateChanged);
                if (this.currentPartnership.Device != null)
                {
                    this.currentPartnership.Device.PropertyChanged -= new PropertyChangedEventHandler(this.OnDevicePropertyChanged);
                }
            }
            this.currentPartnership = this.controller.CurrentSyncPartnership;
            if (this.currentPartnership != null)
            {
                this.currentPartnership.StateChanged += new EventHandler<PartnershipStateChangeEventArgs>(this.OnPartnershipStateChanged);
                this.currentPartnership.Device.PropertyChanged += new PropertyChangedEventHandler(this.OnDevicePropertyChanged);
            }
            this.ResetModelState();
        }

        private void OnPartnershipStateChanged(object sender, PartnershipStateChangeEventArgs e)
        {
            if (this.Panel != null)
            {
                ISyncPartnership partnership = sender as ISyncPartnership;
                if (partnership == this.controller.CurrentSyncPartnership)
                {
                    this.Panel.Dispatcher.BeginInvoke(DispatcherPriority.Normal, delegate {
                        this.ResetModelState();
                    });
                    if ((e.NewState == PartnershipState.Idle) && ((e.OldState == PartnershipState.CancelingSync) || (e.OldState == PartnershipState.Syncing)))
                    {
                        this.controller.CurrentDevice.ReloadZmdbIfChanged(false);
                    }
                }
            }
        }

        public void ResetModelState()
        {
            ISyncPartnership currentSyncPartnership = this.controller.CurrentSyncPartnership;
            if ((currentSyncPartnership != null) && ((((currentSyncPartnership.CurrentState != PartnershipState.Syncing) && (currentSyncPartnership.CurrentState != PartnershipState.CancelingSync)) && (currentSyncPartnership.CurrentState != PartnershipState.LoadingSources)) && (currentSyncPartnership.CurrentState != PartnershipState.VerifyingSources)))
            {
                this.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.Visibility = System.Windows.Visibility.Hidden;
            }
            CommandManager.InvalidateRequerySuggested();
            this.UpdateStorageGauge();
        }

        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies")]
        private void SetBytesFor(string label, ulong bytes)
        {
            StorageBarInfo info;
            if (this.StorageBarInfos.TryGetValue(label, out info))
            {
                info.BytesUsed = bytes;
                if (bytes > 0L)
                {
                    info.LegendByte.Text = StringUtilities.MakeSizeString(new ulong?(bytes));
                }
            }
        }

        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies"), SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId="Microsoft.WPSync.Logging.Logger.TraceWarning(System.String,System.String,System.String)")]
        private void UpdateOverallByteUsage(ulong total, ulong free, ulong used)
        {
            this.totalBytes = total;
            if ((free + used) > total)
            {
                Logger.TraceWarning("Managed:UI", "Reported free + used bytes > total bytes", string.Empty);
                this.otherBytes = 0L;
            }
            else
            {
                this.otherBytes = (total - free) - used;
            }
            this.freeBytes = free;
            this.TotalBytesString = StringUtilities.MakeSizeString(new ulong?(total));
            this.UsedBytesString = StringUtilities.MakeSizeString(new ulong?(used));
            this.FreeBytesString = StringUtilities.MakeSizeString(new ulong?(free));
            this.OtherBytesString = StringUtilities.MakeSizeString(new ulong?(this.otherBytes));
        }

        private void UpdateStorageGauge()
        {
            IDevice currentDevice = this.controller.CurrentDevice;
            if (currentDevice != null)
            {
                using (new OperationLogger())
                {
                    this.StorageAreas = (currentDevice.StorageDevices == null) ? null : ((ICollection<string>) currentDevice.StorageDevices.Keys);
                    StorageInfo storageDeviceInfo = this.GetStorageDeviceInfo(currentDevice);
                    this.UpdateOverallByteUsage(storageDeviceInfo.TotalBytes, storageDeviceInfo.FreeBytes, ((((storageDeviceInfo.UsedStorageSystem + storageDeviceInfo.UsedStorageMusic) + storageDeviceInfo.UsedStoragePictures) + storageDeviceInfo.UsedStorageVideos) + storageDeviceInfo.UsedStoragePodcasts) + storageDeviceInfo.UsedStorageApps);
                    this.SetBytesFor(Microsoft.WPSync.UI.Properties.Resources.SystemStorageText, storageDeviceInfo.UsedStorageSystem);
                    this.SetBytesFor(Microsoft.WPSync.UI.Properties.Resources.AppsStorageText, storageDeviceInfo.UsedStorageApps);
                    this.SetBytesFor(Microsoft.WPSync.UI.Properties.Resources.MusicStorageText, storageDeviceInfo.UsedStorageMusic);
                    this.SetBytesFor(Microsoft.WPSync.UI.Properties.Resources.PhotosStorageText, storageDeviceInfo.UsedStoragePictures);
                    this.SetBytesFor(Microsoft.WPSync.UI.Properties.Resources.VideosStorageText, storageDeviceInfo.UsedStorageVideos);
                    this.SetBytesFor(Microsoft.WPSync.UI.Properties.Resources.PodcastsStorageText, storageDeviceInfo.UsedStoragePodcasts);
                    this.SetBytesFor(Microsoft.WPSync.UI.Properties.Resources.OtherStorageText, this.otherBytes);
                    this.CalculateBars();
                    this.OnPropertyChanged("StorageBars");
                    this.OnPropertyChanged("StorageLegends");
                }
            }
        }

        public string CurrentStorageArea
        {
            get
            {
                return this.currentStorageArea;
            }
            set
            {
                if (this.currentStorageArea != value)
                {
                    this.currentStorageArea = value;
                    this.UpdateStorageGauge();
                    this.OnPropertyChanged("CurrentStorageArea");
                }
            }
        }

        public string FreeBytesString
        {
            get
            {
                return this.freeBytesString;
            }
            set
            {
                this.freeBytesString = value;
                this.OnPropertyChanged("FreeBytesString");
            }
        }

        public string OtherBytesString
        {
            get
            {
                return this.otherBytesString;
            }
            set
            {
                this.otherBytesString = value;
                this.OnPropertyChanged("OtherBytesString");
            }
        }

        public StorageGaugePanel Panel
        {
            get
            {
                return this.panel;
            }
            set
            {
                this.panel = value;
                this.OnPanelAttached();
            }
        }

        public double PanelWidth
        {
            get
            {
                return this.panelWidth;
            }
            set
            {
                this.panelWidth = value;
                this.CalculateBars();
            }
        }

        public ICollection<string> StorageAreas
        {
            get
            {
                return this.storageAreas;
            }
            private set
            {
                this.storageAreas = value;
                this.OnPropertyChanged("StorageAreas");
            }
        }

        private Dictionary<string, StorageBarInfo> StorageBarInfos { get; set; }

        public ICollection<Rectangle> StorageBars
        {
            get
            {
                return (from i in this.StorageBarInfos.Values
                    where i.BytesUsed > 0.0
                    select i.Bar).ToList<Rectangle>();
            }
        }

        public ICollection<UIElement> StorageLegends
        {
            get
            {
                return (from i in this.StorageBarInfos.Values
                    where i.BytesUsed > 0.0
                    select i.Legend).ToList<UIElement>();
            }
        }

        public string TotalBytesString
        {
            get
            {
                return this.totalBytesString;
            }
            private set
            {
                this.totalBytesString = value;
                this.OnPropertyChanged("TotalBytesString");
            }
        }

        public string UsedBytesString
        {
            get
            {
                return this.usedBytesString;
            }
            set
            {
                this.usedBytesString = value;
                this.OnPropertyChanged("UsedBytesString");
            }
        }

        public System.Windows.Visibility Visibility
        {
            get
            {
                return this.visibility;
            }
            private set
            {
                this.visibility = value;
                this.OnPropertyChanged("Visibility");
            }
        }

        private class StorageBarInfo
        {
            public Rectangle Bar { get; set; }

            public double BytesUsed { get; set; }

            public Brush Color { get; set; }

            public UIElement Legend { get; set; }

            public TextBlock LegendByte { get; set; }
        }
    }
}

