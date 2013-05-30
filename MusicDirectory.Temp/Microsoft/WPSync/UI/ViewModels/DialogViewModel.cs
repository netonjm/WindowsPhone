namespace Microsoft.WPSync.UI.ViewModels
{
    using Microsoft.WPSync.Shared;
    using System;

    public abstract class DialogViewModel : PropChangeNotifier
    {
        protected DialogViewModel()
        {
        }

        public virtual void Cancel()
        {
        }

        public abstract void Commit();
        public virtual void Init()
        {
        }

        public virtual bool CanCommit
        {
            get
            {
                return true;
            }
        }
    }
}

