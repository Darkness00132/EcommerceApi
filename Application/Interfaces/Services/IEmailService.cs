namespace Application.Interfaces.Services
{
    public interface IEmailService
    {
        public Task SendAsync<TModel>(string to, string subject, string templateName, TModel model);
    }
}
