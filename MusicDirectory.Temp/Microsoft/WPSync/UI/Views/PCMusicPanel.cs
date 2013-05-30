namespace Microsoft.WPSync.UI.Views
{
    using Microsoft.WPSync.UI.Controls;
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
    public class PCMusicPanel : UserControl, IComponentConnector
    {
        private bool _contentLoaded;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Label artistsLabel;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal TreeListBox artistsList;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button cancelButton;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal TextBlock drmMessage;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Label genresLabel;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal ListBox genresList;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Label playlistsLabel;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal ListBox playlistsList;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal CheckBox syncAllMusicCheckBox;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button syncButton;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal CheckBox syncMusicCheckBox;

        public PCMusicPanel()
        {
            this.InitializeComponent();
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"), DebuggerNonUserCode]
        internal Delegate _CreateDelegate(Type delegateType, string handler)
        {
            return Delegate.CreateDelegate(delegateType, this, handler);
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/WindowsPhone;V0.9.0.0;component/views/pcmusicpanel.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        private void listBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            KeyboardHelper.ListBoxPreviewKeyDown(sender as ListBox, e);
        }

        [EditorBrowsable(EditorBrowsableState.Never), SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily"), SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes"), DebuggerNonUserCode]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.syncMusicCheckBox = (CheckBox) target;
                    return;

                case 2:
                    this.syncAllMusicCheckBox = (CheckBox) target;
                    return;

                case 3:
                    this.playlistsLabel = (Label) target;
                    return;

                case 4:
                    this.playlistsList = (ListBox) target;
                    this.playlistsList.PreviewKeyDown += new KeyEventHandler(this.listBox_PreviewKeyDown);
                    return;

                case 5:
                    this.genresLabel = (Label) target;
                    return;

                case 6:
                    this.genresList = (ListBox) target;
                    this.genresList.PreviewKeyDown += new KeyEventHandler(this.listBox_PreviewKeyDown);
                    return;

                case 7:
                    this.artistsLabel = (Label) target;
                    return;

                case 8:
                    this.artistsList = (TreeListBox) target;
                    return;

                case 9:
                    this.drmMessage = (TextBlock) target;
                    return;

                case 10:
                    this.cancelButton = (Button) target;
                    return;

                case 11:
                    this.syncButton = (Button) target;
                    return;
            }
            this._contentLoaded = true;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void treeListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            KeyboardHelper.TreeListBoxPreviewKeyDown(sender as TreeListBox, e);
        }
    }
}

