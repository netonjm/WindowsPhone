﻿namespace Microsoft.WPSync.UI
{
    using Microsoft.WPSync.UI.Properties;
    using System;
    using System.Globalization;
    using System.Windows;

    internal class Errors
    {
        private Errors()
        {
        }

        public static void ShowError(string errorFormat, params object[] errorArgs)
        {
            MessageBox.Show(string.Format(CultureInfo.CurrentCulture, errorFormat, errorArgs), Resources.ApplicationTitle, MessageBoxButton.OK, MessageBoxImage.Hand);
        }
    }
}

