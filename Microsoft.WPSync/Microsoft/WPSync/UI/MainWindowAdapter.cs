namespace Microsoft.WPSync.UI
{
    using System;
    using System.Windows;

    public class MainWindowAdapter : WindowAdapter
    {
        private bool initialized;
        private MainControllerFactory viewModelFactory;

        public MainWindowAdapter(Window window, MainControllerFactory factory) : base(window)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }
            this.viewModelFactory = factory;
        }

        private void EnsureInitialized()
        {
            if (!this.initialized)
            {
                MainController controller = this.viewModelFactory.Create(this);
                base.Window.DataContext = controller;
                this.initialized = true;
            }
        }

        public override void Show()
        {
            this.EnsureInitialized();
            base.Show();
        }
    }
}

