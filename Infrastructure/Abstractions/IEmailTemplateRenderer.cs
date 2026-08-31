namespace Infrastructure.Abstractions;

internal interface IEmailTemplateRenderer
{
    Task<string> RenderAsync<TModel>(
        string templateName,
        TModel model);
}
