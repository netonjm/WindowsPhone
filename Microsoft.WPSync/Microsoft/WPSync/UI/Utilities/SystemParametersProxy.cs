namespace Microsoft.WPSync.UI.Utilities
{
    using Microsoft.Win32;
    using Microsoft.WPSync.Shared;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;

    public class SystemParametersProxy : PropChangeNotifier
    {
        private static SystemParametersProxy instance;

        private SystemParametersProxy()
        {
            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(this.OnUserPreferenceChanged);
        }

        private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (((sender != null) && (e != null)) && (e.Category == UserPreferenceCategory.Accessibility))
            {
                this.OnPropertyChanged("HighContrast");
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public bool HighContrast
        {
            get
            {
                return SystemParameters.HighContrast;
            }
        }

        public static SystemParametersProxy Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SystemParametersProxy();
                }
                return instance;
            }
        }
    }
}

