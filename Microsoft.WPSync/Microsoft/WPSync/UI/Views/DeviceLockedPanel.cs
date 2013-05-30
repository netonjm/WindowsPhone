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
    public class DeviceLockedPanel : UserControl, IComponentConnector
    {
        private bool _contentLoaded;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button cancelButton;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button findYourPhoneLink;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button help;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal TextBlock phoneName;

        public DeviceLockedPanel()
        {
            this.InitializeComponent();
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/WindowsPhone;V0.9.0.0;component/views/devicelockedpanel.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily"), DebuggerNonUserCode, EditorBrowsable(EditorBrowsableState.Never), SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes"), SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.help = (Button) target;
                    return;

                case 2:
                    this.phoneName = (TextBlock) target;
                    return;

                case 3:
                    this.findYourPhoneLink = (Button) target;
                    return;

                case 4:
                    this.cancelButton = (Button) target;
                    return;
            }
            this._contentLoaded = true;
        }
    }
}

