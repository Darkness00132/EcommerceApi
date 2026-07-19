namespace Ecommerce.DTOs.Auth
{
    public class ResetPasswordRequest
    {
        public string Email { get; set; } = null!;

        public string Token { get; set; } = null!;

        public string NewPassword { get; set; } = null!;
    }
}
