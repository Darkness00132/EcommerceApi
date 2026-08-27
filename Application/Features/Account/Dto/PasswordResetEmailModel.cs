namespace Application.Features.Account.Dto;

public sealed record PasswordResetEmailModel(string RecipientName, string ResetUrl);


