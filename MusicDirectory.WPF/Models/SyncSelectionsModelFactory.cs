using MusicDirectory.WindowsStore;

namespace Microsoft.WPSync.UI.Models
{
    using Microsoft.WPSync.Sync.Rules;
    using Microsoft.WPSync.UI;
    using System;

    public class SyncSelectionsModelFactory : ISyncSelectionsModelFactory
    {
        public ISyncSelectionsModel CreateISyncSelectionModel(ISyncSelectionsModelClient client)
        {
            return DependencyContainer.ResolveISyncSelectionsModel(client);
        }

       
    }
}

