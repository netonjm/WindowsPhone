namespace Microsoft.WPSync.UI.Models
{
    using System;
    using System.Runtime.CompilerServices;

    public class ContentLoadedEventArgs : EventArgs
    {
        public ContentLoadedEventArgs(Microsoft.WPSync.UI.Models.ContentLoadedType type)
        {
            this.ContentLoadedType = type;
        }

        public Microsoft.WPSync.UI.Models.ContentLoadedType ContentLoadedType { get; private set; }
    }
}

