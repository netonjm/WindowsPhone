namespace Microsoft.WPSync.UI
{
    using Microsoft.WPSync.UI.ViewModels;
    using System;
    using System.Windows;

    public interface IWindow
    {
        void Close();
        void Show();
        //bool? ShowDialogWithModel(System.Windows.Window view, DialogType dialogType, DialogViewModel viewModel);

        System.Windows.Window Window { get; }
    }
}

