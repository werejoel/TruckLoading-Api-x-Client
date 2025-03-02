namespace TruckLoadingApp.Domain.Enums
{
    public enum TruckOperationalStatusEnum
    {
        Active = 1,
        Inactive = 2,
        UnderMaintenance = 3,
        AwaitingApproval = 4, // 🚀 New status for newly registered trucks
        Decommissioned = 5 // 🚀 Trucks that are no longer in use
    }
}
