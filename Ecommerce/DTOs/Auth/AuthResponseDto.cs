namespace Ecommerce.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;

        public DateTime AccessTokenExpiration { get; set; }

        public string Email { get; set; } = null!;

        public IList<string> Roles { get; set; } = [];
    }
}
