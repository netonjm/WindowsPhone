namespace Microsoft.WPSync.UI.Views
{
    using Microsoft.WPSync.UI.Utilities;
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Markup;

    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public class StorageGaugePanel : UserControl, IComponentConnector
    {
        private bool _contentLoaded;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal ListBox chooser;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal ItemsControl gaugeBar;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal ItemsControl gaugeLegend;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Grid mainGrid;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button storageAreaChooserButton;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Popup storageAreaChooserPanel;

        public StorageGaugePanel()
        {
            this.InitializeComponent();
        }

        private void chooser_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.storageAreaChooserPanel.IsOpen = false;
        }

        private void chooser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.storageAreaChooserPanel.IsOpen = false;
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/WindowsPhone;V0.9.0.0;component/views/storagegaugepanel.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        private void OnStorageAreaChooserButtonClick(object sender, RoutedEventArgs e)
        {
            this.storageAreaChooserPanel.IsOpen = true;
            this.chooser.Focus();
        }

        private void storageAreaChooserPanel_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            KeyboardHelper.PopupPreviewKeyDown(sender as Popup, e);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), DebuggerNonUserCode, SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily"), EditorBrowsable(EditorBrowsableState.Never), SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.mainGrid = (Grid) target;
                    return;

                case 2:
                    this.storageAreaChooserPanel = (Popup) target;
                    this.storageAreaChooserPanel.PreviewKeyDown += new KeyEventHandler(this.storageAreaChooserPanel_PreviewKeyDown);
                    return;

                case 3:
                    this.chooser = (ListBox) target;
                    this.chooser.SelectionChanged += new SelectionChangedEventHandler(this.chooser_SelectionChanged);
                    this.chooser.MouseUp += new MouseButtonEventHandler(this.chooser_MouseUp);
                    return;

                case 4:
                    this.storageAreaChooserButton = (Button) target;
                    this.storageAreaChooserButton.Click += new RoutedEventHandler(this.OnStorageAreaChooserButtonClick);
                    return;

                case 5:
                    this.gaugeBar = (ItemsControl) target;
                    return;

                case 6:
                    this.gaugeLegend = (ItemsControl) target;
                    return;
            }
            this._contentLoaded = true;
        }
    }
}

