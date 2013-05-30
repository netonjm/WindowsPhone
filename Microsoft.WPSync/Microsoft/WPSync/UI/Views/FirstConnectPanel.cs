namespace Microsoft.WPSync.UI.Views
{
    using Microsoft.WPSync.UI;
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;

    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public class FirstConnectPanel : UserControl, IComponentConnector
    {
        private bool _contentLoaded;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button cancelButton;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal TextBox deviceNameText;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button help;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal CheckBox importPictures;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Image phoneImage;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal TextBlock phoneModel;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal CheckBox syncAutomatically;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal RadioButton syncFromITunes;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal RadioButton syncFromWindowsLibraries;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button syncWithThisComputer;

        public FirstConnectPanel()
        {
            this.InitializeComponent();
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/WindowsPhone;V0.9.0.0;component/views/firstconnectpanel.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        private void syncWithThisComputer_Click(object sender, RoutedEventArgs e)
        {
            if (this.MainController.MainViewModel.DeviceSettingsViewModel.Validate() && this.MainController.MainViewModel.AppSettingsViewModel.Validate())
            {
                this.MainController.MainViewModel.AppSettingsViewModel.Commit();
                if (this.MainController.PartnerWithPhone())
                {
                    this.MainController.MainViewModel.DeviceSettingsViewModel.Commit();
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerNonUserCode, SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes"), SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.help = (Button) target;
                    return;

                case 2:
                    this.phoneImage = (Image) target;
                    return;

                case 3:
                    this.phoneModel = (TextBlock) target;
                    return;

                case 4:
                    this.deviceNameText = (TextBox) target;
                    return;

                case 5:
                    this.syncAutomatically = (CheckBox) target;
                    return;

                case 6:
                    this.importPictures = (CheckBox) target;
                    return;

                case 7:
                    this.syncFromITunes = (RadioButton) target;
                    return;

                case 8:
                    this.syncFromWindowsLibraries = (RadioButton) target;
                    return;

                case 9:
                    this.cancelButton = (Button) target;
                    return;

                case 10:
                    this.syncWithThisComputer = (Button) target;
                    this.syncWithThisComputer.Click += new RoutedEventHandler(this.syncWithThisComputer_Click);
                    return;
            }
            this._contentLoaded = true;
        }

        private Microsoft.WPSync.UI.MainController MainController
        {
            get
            {
                return (base.DataContext as Microsoft.WPSync.UI.MainController);
            }
        }
    }
}

