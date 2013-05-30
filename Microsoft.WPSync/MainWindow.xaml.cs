using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Markup;
using Microsoft.WPSync.Device;
using Microsoft.WPSync.Logging;
using Microsoft.WPSync.Shared;
using Microsoft.WPSync.Shared.Sqm;
using Microsoft.WPSync.Sync.Engine;
using Microsoft.WPSync.UI;
using Microsoft.WPSync.UI.Models;
using Microsoft.WPSync.UI.Utilities;
using Microsoft.WPSync.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.WPSync.UI.Views;
using WictorDeviceManagerLibrary;

namespace Microsoft.WPSync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IComponentConnector
    {
   
        public MainWindow()
        {
            //InitializeComponent();
             using (new OperationLogger())
             {
                 this.InitializeComponent();
                 //base.Title = Resources.ApplicationTitle;
             
                 base.KeyUp += new KeyEventHandler(this.MainWindow_KeyUp);
                 base.ContentRendered += new EventHandler(this.MainWindow_ContentRendered);
             }
        }



        private void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            this.Controller.OnStartup();
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
            this.PanelSet.Values.ToList<UIElement>().ForEach(delegate(UIElement p)
            {
                if (p != null)
                {
                    p.Visibility = Visibility.Hidden;
                }
            });
            panel.Visibility = Visibility.Visible;
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
