namespace TruckLoadingApp.Domain.Enums
{
    public enum LoadStatusEnum
    {
        Created = 1,
        Available = 2,
        Assigned = 3,
        PickedUp = 4,
        Delivered = 5,
        Cancelled = 6,
        Pending = 7,
        PendingBooking = 8,
        Draft = 9
    }
}
