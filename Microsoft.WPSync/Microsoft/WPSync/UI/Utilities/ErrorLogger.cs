namespace Microsoft.WPSync.UI.Utilities
{
    using Microsoft.WPSync.Settings;
    using System;
    using System.IO;

    public class ErrorLogger : IErrorLogger
    {
        private const string lastExceptionFilename = "lastexception.txt";

        public void LogException(Exception exception)
        {
            string str2;
            string path = Path.Combine(GlobalSetting.SettingsDirectoryForApplication(), "lastexception.txt");
            if (exception == null)
            {
                str2 = "Exception was null";
            }
            else
            {
                str2 = exception.ToString();
                if (string.IsNullOrWhiteSpace(str2))
                {
                    str2 = exception.GetType().ToString();
                }
            }
            try
            {
                File.WriteAllText(path, str2);
            }
            catch
            {
            }
        }
    }
}

