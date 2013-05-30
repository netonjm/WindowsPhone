namespace Microsoft.WPSync.UI.Views
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;

    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public class SettingsPanel : UserControl, IComponentConnector
    {
        private bool _contentLoaded;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Grid appSettings;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button cancelButton;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal TextBox deviceNameText;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Grid deviceSettings;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button forgetPhone;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal CheckBox importPictures;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button okButton;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal RadioButton optimizeForQuality;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal RadioButton optimizeForSize;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal CheckBox playSoundOnSyncComplete;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal CheckBox resizePhotos;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal CheckBox sendUsageStats;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal CheckBox showSyncErrors;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal CheckBox syncAutomatically;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal RadioButton syncFromITunes;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal RadioButton syncFromWindowsLibraries;

        public SettingsPanel()
        {
            this.InitializeComponent();
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/WindowsPhone;V0.9.0.0;component/views/settingspanel.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        [DebuggerNonUserCode, SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes"), SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily"), SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), EditorBrowsable(EditorBrowsableState.Never)]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.deviceSettings = (Grid) target;
                    return;

                case 2:
                    this.deviceNameText = (TextBox) target;
                    return;

                case 3:
                    this.syncAutomatically = (CheckBox) target;
                    return;

                case 4:
                    this.importPictures = (CheckBox) target;
                    return;

                case 5:
                    this.resizePhotos = (CheckBox) target;
                    return;

                case 6:
                    this.optimizeForQuality = (RadioButton) target;
                    return;

                case 7:
                    this.optimizeForSize = (RadioButton) target;
                    return;

                case 8:
                    this.forgetPhone = (Button) target;
                    return;

                case 9:
                    this.appSettings = (Grid) target;
                    return;

                case 10:
                    this.playSoundOnSyncComplete = (CheckBox) target;
                    return;

                case 11:
                    this.sendUsageStats = (CheckBox) target;
                    return;

                case 12:
                    this.showSyncErrors = (CheckBox) target;
                    return;

                case 13:
                    this.syncFromITunes = (RadioButton) target;
                    return;

                case 14:
                    this.syncFromWindowsLibraries = (RadioButton) target;
                    return;

                case 15:
                    this.cancelButton = (Button) target;
                    return;

                case 0x10:
                    this.okButton = (Button) target;
                    return;
            }
            this._contentLoaded = true;
        }
    }
}

