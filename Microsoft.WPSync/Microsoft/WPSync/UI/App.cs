namespace Microsoft.WPSync.UI
{
    using Microsoft.WPSync.Logging;
    using Microsoft.WPSync.Settings;
    using Microsoft.WPSync.UI.Properties;
    using Microsoft.WPSync.UI.Utilities;
    using Microsoft.WPSync.UI.Views;
    using System;
    using System.CodeDom.Compiler;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Markup;

    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public class App : Application
    {
        private bool _contentLoaded;
        public const int ApplicationReturnCodeAlreadyRunning = 1;
        public const int ApplicationReturnCodeTimeout = 2;
        private static Mutex SingleInstanceMutex = new Mutex(true, "{478985CF-4769-4503-997D-8B07DA6631CE}");
        private const int SW_RESTORE = 9;

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/WindowsPhone;V0.9.0.0;component/app.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);
        [DebuggerNonUserCode, STAThread]
        public static void Main()
        {
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (SingleInstanceCheck())
            {
                base.OnStartup(e);
                base.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                if (GlobalSetting.TimeoutDate < DateTime.Now)
                {
                    new TimedOutDialog().ShowDialog();
                    Application.Current.Shutdown(2);
                }
                else
                {
                    if (GlobalSetting.TimeoutWarningDate < DateTime.Now)
                    {
                        new TimeoutWarningDialog().ShowDialog();
                    }
                    AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.OnUnhandledException);
                    base.ShutdownMode = ShutdownMode.OnLastWindowClose;
                    DependencyContainer.ResolveIMainWindow().Show();
                }
            }
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            using (new OperationLogger())
            {
                Exception exceptionObject = (Exception) args.ExceptionObject;
                if ((exceptionObject is XamlParseException) && (exceptionObject.InnerException != null))
                {
                    exceptionObject = exceptionObject.InnerException;
                }
                IErrorLogger logger = new ErrorLogger();
                logger.LogException(exceptionObject);
                if (base.Windows.Count == 0)
                {
                    MessageBox.Show(Resources.UnhandledExceptionText);
                }
                else
                {
                    MessageBox.Show(base.Windows[0], Resources.UnhandledExceptionText);
                }
            }
        }

        //public int Run()
        //{
        //    return base.Run();
        //}

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        public static bool SingleInstanceCheck()
        {
            if (SingleInstanceMutex.WaitOne(TimeSpan.Zero, true))
            {
                return true;
            }
            Logger.TraceInformation("Managed:UI", "SingleInstanceCheck: found another instance; exiting", string.Empty);
            Process thisProc = Process.GetCurrentProcess();
            Process process = Process.GetProcessesByName(thisProc.ProcessName).FirstOrDefault<Process>(delegate (Process p) {
                if (p.Id != thisProc.Id)
                {
                    IntPtr mainWindowHandle = p.MainWindowHandle;
                    return true;
                }
                return false;
            });
            if (process != null)
            {
                IntPtr hWnd = process.MainWindowHandle;
                if (IsIconic(hWnd))
                {
                    ShowWindow(hWnd, 9);
                }
                SetForegroundWindow(hWnd);
            }
            MessageBox.Show(Resources.AppAlreadyRunningMessage, Resources.AppAlreadyRunningTitle, MessageBoxButton.OK, MessageBoxImage.Asterisk);
            Application.Current.Shutdown(1);
            return false;
        }
    }
}

