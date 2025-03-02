using System;

namespace TruckLoadingApp.API.DTOs
{
    public class TruckerDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsApproved { get; set; }
        public string TruckOwnerType { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
} 