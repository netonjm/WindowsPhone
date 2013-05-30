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
    public class PCPhotosPanel : UserControl, IComponentConnector
    {
        private bool _contentLoaded;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button cancelButton;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal TreeListBox pictureAlbumsTreeView;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal CheckBox syncAllPictures;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Button syncButton;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal CheckBox syncIncludeVideo;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal CheckBox syncPicturesText;

        public PCPhotosPanel()
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
                Uri resourceLocator = new Uri("/WindowsPhone;V0.9.0.0;component/views/pcphotospanel.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        [DebuggerNonUserCode, SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily"), EditorBrowsable(EditorBrowsableState.Never), SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes"), SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.syncPicturesText = (CheckBox) target;
                    return;

                case 2:
                    this.syncAllPictures = (CheckBox) target;
                    return;

                case 3:
                    this.syncIncludeVideo = (CheckBox) target;
                    return;

                case 4:
                    this.pictureAlbumsTreeView = (TreeListBox) target;
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

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void treeListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            KeyboardHelper.TreeListBoxPreviewKeyDown(sender as TreeListBox, e);
        }
    }
}

