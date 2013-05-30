namespace Microsoft.WPSync.UI
{
    using Microsoft.WPSync.Logging;
    //using Microsoft.WPSync.UI.Properties;
    using Microsoft.WPSync.UI.ViewModels;
    using Microsoft.WPSync.UI.Views;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media.Imaging;

    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public class MainWindow : Window, IComponentConnector
    {
        private bool _contentLoaded;
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Grid windowContent;

        public MainWindow()
        {
            using (new OperationLogger())
            {
                this.InitializeComponent();
                
                //base.Title = Resources.ApplicationTitle;
                base.Icon = BitmapFrame.Create(new Uri("pack://application:,,,/resources/WindowsPhoneConnectorIcon.ico"));
                this.CreateViewPanels();
                base.DataContextChanged += new DependencyPropertyChangedEventHandler(this.MainWindow_DataContextChanged);
                base.KeyUp += new KeyEventHandler(this.MainWindow_KeyUp);
                base.ContentRendered += new EventHandler(this.MainWindow_ContentRendered);
            }
        }

        private void Controller_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //base.Dispatcher.BeginInvoke(delegate {
            //    string propertyName = e.PropertyName;
            //    if (propertyName != null)
            //    {
            //        if (!(propertyName == "CurrentViewState"))
            //        {
            //            if (!(propertyName == "MainWindowCursor"))
            //            {
            //                return;
            //            }
            //        }
            //        else
            //        {
            //            this.UpdateWindowContent();
            //            return;
            //        }
            //        if (this.Controller != null)
            //        {
            //            this.Cursor = this.Controller.MainViewModel.MainWindowCursor;
            //        }
            //    }
            //}, new object[0]);
        }

        private void CreateViewPanels()
        {
            if ((this.PanelSet != null) && (this.PanelSet.Count > 0))
            {
                foreach (UIElement element in this.PanelSet.Values)
                {
                    this.windowContent.Children.Remove(element);
                }
            }
            this.PanelSet = new Dictionary<MainViewState, UIElement>();
            UIElement element2 = new ConnectedPanel();
            this.PanelSet[MainViewState.UnconnectedState] = new UnconnectedPanel();
            this.PanelSet[MainViewState.FirstConnectPanel] = new FirstConnectPanel();
            this.PanelSet[MainViewState.LockedPanel] = new DeviceLockedPanel();
            this.PanelSet[MainViewState.PCTab] = element2;
            this.PanelSet[MainViewState.PhoneTab] = element2;
            this.PanelSet[MainViewState.WebTab] = element2;
            foreach (UIElement element3 in this.PanelSet.Values.Distinct<UIElement>())
            {
                if (element3 != null)
                {
                    element3.Visibility = Visibility.Hidden;
                    this.windowContent.Children.Add(element3);
                }
            }
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/WindowsPhone;V0.9.0.0;component/mainwindow.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        private void InitViewModel()
        {
            this.Controller.MainViewModel.PropertyChanged += new PropertyChangedEventHandler(this.Controller_PropertyChanged);
            this.UpdateWindowContent();
        }

        private void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            this.Controller.OnStartup();
        }

        private void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (base.DataContext != null)
            {
                this.InitViewModel();
            }
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            this.Controller.OnKeyUp(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (e == null)
            {
                base.OnClosing(e);
            }
            else
            {
                this.Controller.OnClosing(e);
                if (!e.Cancel)
                {
                    base.DataContext = null;
                    base.OnClosing(e);
                }
            }
        }

        private void SetMainContentPanel(UIElement panel)
        {
            this.PanelSet.Values.ToList<UIElement>().ForEach(delegate (UIElement p) {
                if (p != null)
                {
                    p.Visibility = Visibility.Hidden;
                }
            });
            panel.Visibility = Visibility.Visible;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), EditorBrowsable(EditorBrowsableState.Never), SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes"), SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily"), DebuggerNonUserCode]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            if (connectionId == 1)
            {
                this.windowContent = (Grid) target;
            }
            else
            {
                this._contentLoaded = true;
            }
        }

        private void UpdateWindowContent()
        {
            this.SetMainContentPanel(this.PanelSet[this.Controller.MainViewModel.CurrentViewState]);
        }

        private IMainController Controller
        {
            get
            {
                return (base.DataContext as IMainController);
            }
        }

        private Dictionary<MainViewState, UIElement> PanelSet { get; set; }
    }
}

