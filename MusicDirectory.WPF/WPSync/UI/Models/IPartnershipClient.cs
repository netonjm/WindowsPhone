using Microsoft.WPSync.Sync.Engine;
using Microsoft.WPSync.Sync.Rules;

namespace Microsoft.WPSync.UI.Models
{
    using System;
    using System.Collections.Generic;

    public interface IPartnershipClient
    {
        void HandleException(Exception exception);
        void HandleSourceErrors(ISyncPartnership partnership, ICollection<SourceResult> errors);
        void HandleSyncErrors(ISyncPartnership partnership, ICollection<SyncResult> errors);
        void OnDeviceLocked(ISyncPartnership partnership);
    }
}

