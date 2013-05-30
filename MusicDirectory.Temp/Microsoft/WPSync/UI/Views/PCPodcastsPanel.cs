namespace Microsoft.WPSync.UI.Views
{
    using Microsoft.WPSync.UI.Utilities;
    using Microsoft.WPSync.UI.ViewModels;
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;

    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public class PCPodcastsPanel : UserControl, IComponentConnector
    {
        private bool _contentLoaded;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button cancelButton;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Grid mainGrid;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal CheckBox onlySyncCheckBox;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal ComboBox onlySyncComboBox;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal ListBox podcastsListBox;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal StackPanel settingsPanel;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal CheckBox syncAllPodcastsCheckBox;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button syncButton;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal CheckBox syncPodcastsCheckBox;

        public PCPodcastsPanel()
        {
            this.InitializeComponent();
            this.EpisodeRuleViewModel = new PodcastEpisodeRuleViewModel();
            this.onlySyncComboBox.DataContext = this.EpisodeRuleViewModel;
            this.onlySyncCheckBox.DataContext = this.EpisodeRuleViewModel;
            this.settingsPanel.DataContextChanged += new DependencyPropertyChangedEventHandler(this.OnSettingsDataContextChanged);
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/WindowsPhone;V0.9.0.0;component/views/pcpodcastspanel.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        private void listBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            KeyboardHelper.ListBoxPreviewKeyDown(sender as ListBox, e);
        }

        private void OnSettingsDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.ContentViewModel != null)
            {
                this.ContentViewModel.PropertyChanged += new PropertyChangedEventHandler(this.OnViewModelPropertyChanged);
                this.EpisodeRuleViewModel.SyncRules = this.ContentViewModel.SyncRules;
            }
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Action method = null;
            if (e.PropertyName == "SyncRules")
            {
                if (method == null)
                {
                    method = delegate {
                        if (this.ContentViewModel != null)
                        {
                            this.EpisodeRuleViewModel.SyncRules = this.ContentViewModel.SyncRules;
                        }
                    };
                }
                base.Dispatcher.BeginInvoke(method, new object[0]);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily"), SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), DebuggerNonUserCode, EditorBrowsable(EditorBrowsableState.Never), SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.mainGrid = (Grid) target;
                    return;

                case 2:
                    this.settingsPanel = (StackPanel) target;
                    return;

                case 3:
                    this.syncPodcastsCheckBox = (CheckBox) target;
                    return;

                case 4:
                    this.syncAllPodcastsCheckBox = (CheckBox) target;
                    return;

                case 5:
                    this.onlySyncCheckBox = (CheckBox) target;
                    return;

                case 6:
                    this.onlySyncComboBox = (ComboBox) target;
                    return;

                case 7:
                    this.podcastsListBox = (ListBox) target;
                    this.podcastsListBox.PreviewKeyDown += new KeyEventHandler(this.listBox_PreviewKeyDown);
                    return;

                case 8:
                    this.cancelButton = (Button) target;
                    return;

                case 9:
                    this.syncButton = (Button) target;
                    return;
            }
            this._contentLoaded = true;
        }

        private IMediaContentViewModel ContentViewModel
        {
            get
            {
                return (this.settingsPanel.DataContext as IMediaContentViewModel);
            }
        }

        public PodcastEpisodeRuleViewModel EpisodeRuleViewModel { get; private set; }
    }
}

