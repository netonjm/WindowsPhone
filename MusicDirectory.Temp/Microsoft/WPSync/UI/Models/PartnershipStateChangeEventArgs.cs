namespace Microsoft.WPSync.UI.Models
{
    using System;
    using System.Runtime.CompilerServices;

    public partial class PartnershipStateChangeEventArgs : EventArgs
    {
        public  PartnershipStateChangeEventArgs(PartnershipState newState, PartnershipState oldState)
        {
            this.NewState = newState;
            this.OldState = oldState;
        }

        public PartnershipState NewState { get; private set; }

        public PartnershipState OldState { get; private set; }
    }
}

