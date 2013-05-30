namespace Microsoft.WPSync.UI.Views
{
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
    public class SyncErrorsPanel : Window, IComponentConnector
    {
        private bool _contentLoaded;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal ListBox errorItems;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button okButton;

        public SyncErrorsPanel(ErrorsViewModel viewModel)
        {
            MouseButtonEventHandler handler = null;
            this.InitializeComponent();
            this.ViewModel = viewModel;
            base.DataContextChanged += new DependencyPropertyChangedEventHandler(this.SyncErrorsPanel_DataContextChanged);
            if (handler == null)
            {
                handler = delegate {
                    base.DragMove();
                };
            }
            base.MouseDown += handler;
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/WindowsPhone;V0.9.0.0;component/views/syncerrorspanel.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        private void OnOkButtonClicked(object sender, RoutedEventArgs e)
        {
            base.Close();
        }

        private void SyncErrorsPanel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.errorItems.DataContext = this.ViewModel.ItemErrorStrings;
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes"), SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily"), DebuggerNonUserCode, EditorBrowsable(EditorBrowsableState.Never)]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.errorItems = (ListBox) target;
                    return;

                case 2:
                    this.okButton = (Button) target;
                    this.okButton.Click += new RoutedEventHandler(this.OnOkButtonClicked);
                    return;
            }
            this._contentLoaded = true;
        }

        private ErrorsViewModel ViewModel { get; set; }
    }
}

