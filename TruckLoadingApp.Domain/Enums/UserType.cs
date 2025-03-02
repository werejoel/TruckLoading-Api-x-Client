namespace TruckLoadingApp.Domain.Enums
{
    /// <summary>
    /// Represents the different types of users in the system.
    /// </summary>
    public enum UserType
    {
        /// <summary>
        /// Represents a shipper user.
        /// </summary>
        Shipper = 1,
        /// <summary>
        /// Represents a trucker user.
        /// </summary>
        Trucker = 2,
        /// <summary>
        /// Represents a company user.
        /// </summary>
        Company = 3,
        /// <summary>
        /// Represents an administrator user.
        /// </summary>
        Admin = 4
    }
}
