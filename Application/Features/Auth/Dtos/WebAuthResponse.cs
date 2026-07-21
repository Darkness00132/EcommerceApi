namespace Application.Features.Auth.Dtos
{
    public class WebAuthResponse
    {
        public string AccessToken { get; set; } = null!;

        public DateTime AccessTokenExpiration { get; set; }
    }
}
