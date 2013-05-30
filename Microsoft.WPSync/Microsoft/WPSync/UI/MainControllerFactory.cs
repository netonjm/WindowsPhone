namespace Microsoft.WPSync.UI
{
    using System;

    public class MainControllerFactory
    {
        private readonly IMainFactory factory;

        public MainControllerFactory(IMainFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }
            this.factory = factory;
        }

        public MainController Create(IWindow view)
        {
            return new MainController(view, this.factory);
        }
    }
}

