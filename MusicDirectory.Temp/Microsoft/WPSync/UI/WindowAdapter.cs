namespace Microsoft.WPSync.UI
{
    using Microsoft.WPSync.UI.ViewModels;
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;

    public class WindowAdapter : IWindow
    {
        public WindowAdapter(System.Windows.Window window)
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }
            this.Window = window;
        }

        public virtual void Close()
        {
            this.Window.Close();
        }

        public virtual void Show()
        {
            this.Window.Show();
        }

        public bool? ShowDialogWithModel(System.Windows.Window view, DialogType dialogType, DialogViewModel viewModel)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }
            view.Owner = this.Window;
            view.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            view.ShowInTaskbar = false;
            if (viewModel != null)
            {
                viewModel.Init();
                view.DataContext = viewModel;
            }
            if (dialogType == DialogType.Modal)
            {
                return view.ShowDialog();
            }
            view.Show();
            return null;
        }

        public System.Windows.Window Window { get; protected set; }
    }
}

