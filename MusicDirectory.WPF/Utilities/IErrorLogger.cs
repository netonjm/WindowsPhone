namespace Microsoft.WPSync.UI.Utilities
{
    using System;

    public interface IErrorLogger
    {
        void LogException(Exception exception);
    }
}

