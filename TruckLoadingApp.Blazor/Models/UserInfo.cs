namespace TruckLoadingApp.Blazor.Models
{
    public class UserInfo
    {
        public string Email { get; set; } = string.Empty;
        public UserType UserType { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
