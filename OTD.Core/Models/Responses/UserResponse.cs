namespace OTD.Core.Models
{
    public class LoginResponse
    {
        public string AuthToken { get; set; }
        public DateTime AccessTokenExpireDate { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
