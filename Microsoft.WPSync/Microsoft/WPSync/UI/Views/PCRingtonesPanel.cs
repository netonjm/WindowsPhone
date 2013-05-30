namespace Microsoft.WPSync.UI.Views
{
    using Microsoft.WPSync.UI;
    using Microsoft.WPSync.UI.Utilities;
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;

    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public class PCRingtonesPanel : UserControl, IComponentConnector
    {
        private bool _contentLoaded;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button cancelButton;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal ListBox ringtonesListBox;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button sendRingtonesButton;

        public PCRingtonesPanel()
        {
            this.InitializeComponent();
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/WindowsPhone;V0.9.0.0;component/views/pcringtonespanel.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        private void listBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            KeyboardHelper.ListBoxPreviewKeyDown(sender as ListBox, e);
        }

        private void OnSendRingtoneButtonClicked(object sender, RoutedEventArgs e)
        {
            this.MainController.MainViewModel.DeviceViewModel.TransferRingtones();
        }

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily"), EditorBrowsable(EditorBrowsableState.Never), SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), DebuggerNonUserCode, SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.ringtonesListBox = (ListBox) target;
                    this.ringtonesListBox.PreviewKeyDown += new KeyEventHandler(this.listBox_PreviewKeyDown);
                    return;

                case 2:
                    this.cancelButton = (Button) target;
                    return;

                case 3:
                    this.sendRingtonesButton = (Button) target;
                    this.sendRingtonesButton.Click += new RoutedEventHandler(this.OnSendRingtoneButtonClicked);
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

