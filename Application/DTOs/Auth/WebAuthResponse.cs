namespace Application.DTOs.Auth
{
    public class WebAuthResponse
    {
        public string AccessToken { get; set; } = null!;

        public DateTime AccessTokenExpiration { get; set; }
    }
}
