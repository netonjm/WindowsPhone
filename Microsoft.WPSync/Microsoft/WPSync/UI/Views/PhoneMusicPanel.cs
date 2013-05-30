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
    using System.Windows.Input;
    using System.Windows.Markup;

    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public class PhoneMusicPanel : UserControl, IComponentConnector
    {
        private bool _contentLoaded;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button cancelButton;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button deleteButton;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal FrameworkElement dummyElement;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal DataGrid itemsDataGrid;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button saveToPcButton;

        public PhoneMusicPanel()
        {
            this.InitializeComponent();
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            KeyboardHelper.DataGridPreviewKeyDown(sender as DataGrid, e);
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/WindowsPhone;V0.9.0.0;component/views/phonemusicpanel.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily"), DebuggerNonUserCode, EditorBrowsable(EditorBrowsableState.Never), SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes"), SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.dummyElement = (FrameworkElement) target;
                    return;

                case 2:
                    this.itemsDataGrid = (DataGrid) target;
                    this.itemsDataGrid.PreviewKeyDown += new KeyEventHandler(this.dataGrid_PreviewKeyDown);
                    return;

                case 3:
                    this.saveToPcButton = (Button) target;
                    return;

                case 4:
                    this.deleteButton = (Button) target;
                    return;

                case 5:
                    this.cancelButton = (Button) target;
                    return;
            }
            this._contentLoaded = true;
        }
    }
}

