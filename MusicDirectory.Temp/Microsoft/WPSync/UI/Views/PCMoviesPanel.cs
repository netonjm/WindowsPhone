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
    public class PCMoviesPanel : UserControl, IComponentConnector
    {
        private bool _contentLoaded;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button cancelButton;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal TextBlock drmMessage;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal ListBox moviesListBox;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button syncButton;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal CheckBox syncMovies;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal CheckBox syncMoviesAll;

        public PCMoviesPanel()
        {
            this.InitializeComponent();
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/WindowsPhone;V0.9.0.0;component/views/pcmoviespanel.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        private void listBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            KeyboardHelper.ListBoxPreviewKeyDown(sender as ListBox, e);
        }

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily"), SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes"), SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), EditorBrowsable(EditorBrowsableState.Never), DebuggerNonUserCode]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.syncMovies = (CheckBox) target;
                    return;

                case 2:
                    this.syncMoviesAll = (CheckBox) target;
                    return;

                case 3:
                    this.moviesListBox = (ListBox) target;
                    this.moviesListBox.PreviewKeyDown += new KeyEventHandler(this.listBox_PreviewKeyDown);
                    return;

                case 4:
                    this.drmMessage = (TextBlock) target;
                    return;

                case 5:
                    this.cancelButton = (Button) target;
                    return;

                case 6:
                    this.syncButton = (Button) target;
                    return;
            }
            this._contentLoaded = true;
        }
    }
}

