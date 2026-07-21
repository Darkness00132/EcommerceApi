namespace Application.Features.Auth.Dtos
{
    public class AuthResponse
    {
        public string AccessToken { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;

        public DateTime AccessTokenExpiration { get; set; }
    }
}
