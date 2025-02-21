namespace TruckLoadingApp.Blazor.Models
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public UserInfo User { get; set; } = new UserInfo();
    }
}
