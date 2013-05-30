namespace Microsoft.WPSync.UI.Views
{
    using Microsoft.WPSync.Settings;
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;

    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public class TimeoutWarningDialog : Window, IComponentConnector
    {
        private bool _contentLoaded;

        public TimeoutWarningDialog()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(GlobalSetting.DownloadLink.AbsoluteUri);
            }
            catch (Win32Exception exception)
            {
                if (exception.ErrorCode == -2147467259)
                {
                    MessageBox.Show(exception.Message);
                }
            }
            catch (Exception exception2)
            {
                MessageBox.Show(exception2.Message);
            }
            base.Close();
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/WindowsPhone;V0.9.0.0;component/views/timeoutwarningdialog.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily"), EditorBrowsable(EditorBrowsableState.Never), DebuggerNonUserCode, SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            if (connectionId == 1)
            {
                ((Button) target).Click += new RoutedEventHandler(this.Button_Click);
            }
            else
            {
                this._contentLoaded = true;
            }
        }
    }
}

