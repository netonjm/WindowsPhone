namespace Microsoft.WPSync.UI.Views
{
    using Microsoft.WPSync.Settings;
    using Microsoft.WPSync.UI;
    using Microsoft.WPSync.UI.Utilities;
    using Microsoft.WPSync.UI.ViewModels;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Markup;

    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public class ConnectedPanel : UserControl, IComponentConnector
    {
        private bool _contentLoaded;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal TextBlock batteryVoltage;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal DeviceChooser chooser;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Grid contentGrid;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Grid contentHeader;
        private Dictionary<ContentViewState, Button> contentTabSet;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button deviceChooserButton;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Popup deviceChooserPanel;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal StackPanel deviceStatus;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Grid footerPanel;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button help;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Grid mainGrid;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal StackPanel mainTabs;
        private Dictionary<MainViewState, Button> mainTabSet;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button MoviesTVPanel;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button MusicPanel;
        private Dictionary<ContentViewState, UIElement> pcContentPanelSet;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button PCTab;
        private Dictionary<ContentViewState, UIElement> phoneContentPanelSet;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Image phoneImage;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal TextBlock phoneModel;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal TextBlock phoneName;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button PhoneTab;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button PhotosVideosPanel;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button PodcastsPanel;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button RingtonesPanel;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal TextBox searchText;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal StackPanel settingsAndHelp;
        private Dictionary<ContentViewState, UIElement> specialContentPanelSet;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal StorageGaugePanel storageGauge;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal SyncProgressPanel syncProgress;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button updateSettings;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button WebTab;

        public ConnectedPanel()
        {
            this.InitializeComponent();
            this.CreateViewPanels();
            base.DataContextChanged += new DependencyPropertyChangedEventHandler(this.OnPanelDataContextChanged);
        }

        [DebuggerNonUserCode, SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Delegate _CreateDelegate(Type delegateType, string handler)
        {
            return Delegate.CreateDelegate(delegateType, this, handler);
        }

        private void CreateViewPanels()
        {
            this.pcContentPanelSet = new Dictionary<ContentViewState, UIElement>();
            this.pcContentPanelSet[ContentViewState.MusicPanel] = new PCMusicPanel();
            this.pcContentPanelSet[ContentViewState.PhotosVideosPanel] = new PCPhotosPanel();
            this.pcContentPanelSet[ContentViewState.MoviesTVPanel] = new PCMoviesPanel();
            this.pcContentPanelSet[ContentViewState.PodcastsPanel] = new PCPodcastsPanel();
            this.pcContentPanelSet[ContentViewState.RingtonesPanel] = new PCRingtonesPanel();
            foreach (UIElement element in this.pcContentPanelSet.Values)
            {
                Grid.SetRow(element, 2);
                Grid.SetColumn(element, 0);
                element.Visibility = Visibility.Hidden;
                this.contentGrid.Children.Add(element);
            }
            this.phoneContentPanelSet = new Dictionary<ContentViewState, UIElement>();
            this.phoneContentPanelSet[ContentViewState.MusicPanel] = new PhoneMusicPanel();
            this.phoneContentPanelSet[ContentViewState.PhotosVideosPanel] = new PhonePhotosPanel();
            this.phoneContentPanelSet[ContentViewState.MoviesTVPanel] = new PhoneMoviesPanel();
            this.phoneContentPanelSet[ContentViewState.PodcastsPanel] = new PhonePodcastsPanel();
            foreach (UIElement element2 in this.phoneContentPanelSet.Values)
            {
                Grid.SetRow(element2, 2);
                Grid.SetColumn(element2, 0);
                element2.Visibility = Visibility.Hidden;
                this.contentGrid.Children.Add(element2);
            }
            this.specialContentPanelSet = new Dictionary<ContentViewState, UIElement>();
            this.specialContentPanelSet[ContentViewState.SettingsPanel] = new SettingsPanel();
            foreach (UIElement element3 in this.specialContentPanelSet.Values)
            {
                Grid.SetRow(element3, 2);
                Grid.SetColumn(element3, 0);
                element3.Visibility = Visibility.Hidden;
                this.contentGrid.Children.Add(element3);
            }
            this.mainTabSet = new Dictionary<MainViewState, Button>();
            this.mainTabSet[MainViewState.PCTab] = this.PCTab;
            this.mainTabSet[MainViewState.PhoneTab] = this.PhoneTab;
            this.mainTabSet[MainViewState.WebTab] = this.WebTab;
            this.contentTabSet = new Dictionary<ContentViewState, Button>();
            this.contentTabSet[ContentViewState.MusicPanel] = this.MusicPanel;
            this.contentTabSet[ContentViewState.PhotosVideosPanel] = this.PhotosVideosPanel;
            this.contentTabSet[ContentViewState.MoviesTVPanel] = this.MoviesTVPanel;
            this.contentTabSet[ContentViewState.PodcastsPanel] = this.PodcastsPanel;
            this.contentTabSet[ContentViewState.RingtonesPanel] = this.RingtonesPanel;
        }

        private void deviceChooserPanel_Opened(object sender, EventArgs e)
        {
            this.chooser.Focus();
        }

        private void deviceChooserPanel_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            KeyboardHelper.PopupPreviewKeyDown(sender as Popup, e);
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/WindowsPhone;V0.9.0.0;component/views/connectedpanel.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        private void InitViewModel()
        {
            this.MainController.MainViewModel.PropertyChanged += new PropertyChangedEventHandler(this.OnMainViewModelPropertyChanged);
            this.MainController.MainViewModel.StorageGaugeViewModel.Panel = this.storageGauge;
            this.UpdateClientView();
        }

        private void mainTab_Click(object sender, RoutedEventArgs e)
        {
            this.MainController.MainViewModel.OnUserChangingMainView((MainViewState) ((Button) sender).CommandParameter);
        }

        private void OnMainViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //base.Dispatcher.BeginInvoke(delegate {
            //    string str;
            //    if (((str = e.PropertyName) != null) && ((str == "CurrentViewState") || (str == "CurrentContentViewState")))
            //    {
            //        this.UpdatePanelContent();
            //    }
            //}, new object[0]);
        }

        private void OnPanelDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (base.DataContext != null)
            {
                this.InitViewModel();
            }
        }

        private void SetContentPanel(UIElement panel)
        {
            this.pcContentPanelSet.Values.ToList<UIElement>().ForEach(delegate (UIElement p) {
                p.Visibility = Visibility.Hidden;
            });
            this.phoneContentPanelSet.Values.ToList<UIElement>().ForEach(delegate (UIElement p) {
                p.Visibility = Visibility.Hidden;
            });
            this.specialContentPanelSet.Values.ToList<UIElement>().ForEach(delegate (UIElement p) {
                p.Visibility = Visibility.Hidden;
            });
            panel.Visibility = Visibility.Visible;
        }

        [DebuggerNonUserCode, EditorBrowsable(EditorBrowsableState.Never), SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily"), SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.mainGrid = (Grid) target;
                    return;

                case 2:
                    this.contentGrid = (Grid) target;
                    return;

                case 3:
                    this.deviceChooserPanel = (Popup) target;
                    this.deviceChooserPanel.Opened += new EventHandler(this.deviceChooserPanel_Opened);
                    this.deviceChooserPanel.PreviewKeyDown += new KeyEventHandler(this.deviceChooserPanel_PreviewKeyDown);
                    return;

                case 4:
                    this.chooser = (DeviceChooser) target;
                    return;

                case 5:
                    this.settingsAndHelp = (StackPanel) target;
                    return;

                case 6:
                    this.updateSettings = (Button) target;
                    return;

                case 7:
                    this.help = (Button) target;
                    return;

                case 8:
                    this.deviceStatus = (StackPanel) target;
                    return;

                case 9:
                    this.phoneImage = (Image) target;
                    return;

                case 10:
                    this.phoneName = (TextBlock) target;
                    return;

                case 11:
                    this.deviceChooserButton = (Button) target;
                    return;

                case 12:
                    this.phoneModel = (TextBlock) target;
                    return;

                case 13:
                    this.batteryVoltage = (TextBlock) target;
                    return;

                case 14:
                    this.mainTabs = (StackPanel) target;
                    return;

                case 15:
                    this.PCTab = (Button) target;
                    this.PCTab.Click += new RoutedEventHandler(this.mainTab_Click);
                    return;

                case 0x10:
                    this.PhoneTab = (Button) target;
                    this.PhoneTab.Click += new RoutedEventHandler(this.mainTab_Click);
                    return;

                case 0x11:
                    this.WebTab = (Button) target;
                    this.WebTab.Click += new RoutedEventHandler(this.mainTab_Click);
                    return;

                case 0x12:
                    this.contentHeader = (Grid) target;
                    return;

                case 0x13:
                    this.MusicPanel = (Button) target;
                    return;

                case 20:
                    this.PhotosVideosPanel = (Button) target;
                    return;

                case 0x15:
                    this.MoviesTVPanel = (Button) target;
                    return;

                case 0x16:
                    this.PodcastsPanel = (Button) target;
                    return;

                case 0x17:
                    this.RingtonesPanel = (Button) target;
                    return;

                case 0x18:
                    this.searchText = (TextBox) target;
                    return;

                case 0x19:
                    this.footerPanel = (Grid) target;
                    return;

                case 0x1a:
                    this.storageGauge = (StorageGaugePanel) target;
                    return;

                case 0x1b:
                    this.syncProgress = (SyncProgressPanel) target;
                    return;
            }
            this._contentLoaded = true;
        }

        private void UpdateClientView()
        {
            if (this.MainController != null)
            {
                this.UpdatePanelContent();
            }
        }

        private void UpdatePanelContent()
        {
            if (this.MainController == null)
            {
                return;
            }
            MainViewState currentViewState = this.MainController.MainViewModel.CurrentViewState;
            if (((currentViewState != MainViewState.PCTab) && (currentViewState != MainViewState.PhoneTab)) && (currentViewState != MainViewState.WebTab))
            {
                return;
            }
            this.UpdateTabs();
            ContentViewState currentContentViewState = this.MainController.MainViewModel.CurrentContentViewState;
            if (this.specialContentPanelSet.ContainsKey(currentContentViewState))
            {
                this.SetContentPanel(this.specialContentPanelSet[currentContentViewState]);
            }
            else
            {
                switch (currentViewState)
                {
                    case MainViewState.PCTab:
                        this.SetContentPanel(this.pcContentPanelSet[currentContentViewState]);
                        goto Label_008E;

                    case MainViewState.PhoneTab:
                        this.SetContentPanel(this.phoneContentPanelSet[currentContentViewState]);
                        goto Label_008E;
                }
            }
        Label_008E:
            CommandManager.InvalidateRequerySuggested();
        }

        private void UpdateTabs()
        {
            MainViewState currentViewState = this.MainController.MainViewModel.CurrentViewState;
            ContentViewState currentContentViewState = this.MainController.MainViewModel.CurrentContentViewState;
            foreach (Button button in this.mainTabSet.Values)
            {
                button.Tag = "off";
            }
            this.mainTabSet[currentViewState].Tag = "";
            foreach (Button button2 in this.contentTabSet.Values)
            {
                button2.Tag = "off";
            }
            if (this.contentTabSet.ContainsKey(currentContentViewState))
            {
                this.contentTabSet[currentContentViewState].Tag = "";
            }
            if (currentContentViewState == ContentViewState.SettingsPanel)
            {
                this.updateSettings.Tag = "selected";
                this.mainTabSet[currentViewState].Tag = "off";
            }
            else
            {
                this.updateSettings.Tag = "";
            }
            if (currentViewState == MainViewState.PhoneTab)
            {
                this.RingtonesPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.RingtonesPanel.Visibility = Visibility.Visible;
            }
            if ((currentViewState == MainViewState.PCTab) && GlobalSetting.IsMusicSourceWindowsLibraries())
            {
                this.PodcastsPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.PodcastsPanel.Visibility = Visibility.Visible;
            }
        }

        private IMainController MainController
        {
            get
            {
                return (base.DataContext as IMainController);
            }
        }
    }
}

