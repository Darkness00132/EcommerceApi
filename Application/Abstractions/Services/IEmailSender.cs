namespace Application.Abstractions.Services;

public interface IEmailSender
{
    Task SendAsync<TModel>(
        string to,
        string subject,
        string templateFileName,
        TModel model,
        CancellationToken cancellationToken = default);
}
