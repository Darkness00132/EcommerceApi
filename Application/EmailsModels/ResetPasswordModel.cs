namespace Application.EmailModels
{
    public class ResetPasswordModel
    {
        public string FirstName { get; set; } = null!;
        public string ResetLink { get; set; } = null!;
    }
}
