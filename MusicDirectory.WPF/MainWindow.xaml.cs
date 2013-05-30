using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Microsoft.WPSync.Device;
using MusicDirectory.WindowsStore;

namespace MusicDirectory.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

              try
              {
                  IDeviceManager manager = null;
                  manager = new DeviceManager(0);
              }
              catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            


            //ListCanciones.ItemsSource = new ObservableCollection<String>()
            //                                {
            //                                    "sadsadsa","sdasda"
            //                                };

            //var elemento = DependencyContainer.ResolveISyncSelectionsModelFactory();
            // var modelo = elemento.CreateISyncSelectionModel()

        }
    }
}
