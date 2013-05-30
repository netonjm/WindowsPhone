namespace Microsoft.WPSync.UI.Models
{
    using System;

    public enum PartnershipState
    {
        Uninitialized,
        Idle,
        LoadingSources,
        ApplyingRules,
        VerifyingSources,
        RespondingToChanges,
        PreparingSync,
        Syncing,
        CancelingSync,
        DeviceLocked,
        ShuttingDown
    }
}

