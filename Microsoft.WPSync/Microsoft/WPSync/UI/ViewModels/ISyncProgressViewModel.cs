namespace Microsoft.WPSync.UI.ViewModels
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Windows;

    public interface ISyncProgressViewModel
    {
        bool AddProgress(float percent);
        void ClearProgress();
        void SetProgress(float current, float maximum);
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        void UpdateProgressBar(string header, string subHeader, string caption, float current, float maximum, bool lockProgress = false);

        string Caption { get; }

        float CurrentProgress { get; }

        string Header { get; }

        bool IsProgressLocked { get; set; }

        float ProgressLeft { get; }

        string SubHeader { get; }

        System.Windows.Visibility Visibility { get; }
    }
}

