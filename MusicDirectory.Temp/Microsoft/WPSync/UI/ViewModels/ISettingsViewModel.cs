namespace Microsoft.WPSync.UI.ViewModels
{
    using System;

    public interface ISettingsViewModel
    {
        void Commit();
        void Init();
        bool Validate();
    }
}

