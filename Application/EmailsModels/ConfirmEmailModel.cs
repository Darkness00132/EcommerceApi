namespace Application.EmailModels
{
    public class ConfirmEmailModel
    {
        public string FirstName { get; init; } = null!;
        public string ConfirmationLink { get; init; } = null!;
    }
}
