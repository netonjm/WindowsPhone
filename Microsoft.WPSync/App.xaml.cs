using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Microsoft.WPSync.UI;

namespace Microsoft.WPSync
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public  App()
        {
            DependencyContainer.ResolveIMainWindow().Show();
        }

    }
}
