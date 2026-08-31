using Infrastructure.Abstractions;
using RazorLight;

namespace Infrastructure.Services;

internal class RazorLightRenderer : IEmailTemplateRenderer
{
    private const string ResourceRoot =
        "Infrastructure.EmailTemplates";

    private readonly RazorLightEngine _engine;

    public RazorLightRenderer()
    {
        var assembly = typeof(RazorLightRenderer).Assembly;

        _engine = new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(assembly, ResourceRoot)
            .SetOperatingAssembly(assembly)
            .UseMemoryCachingProvider()
            .EnableDebugMode()
            .Build();
    }

    public Task<string> RenderAsync<TModel>(
        string templateName,
        TModel model)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(templateName);
        ArgumentNullException.ThrowIfNull(model);

        var key = templateName.EndsWith(
            ".cshtml",
            StringComparison.OrdinalIgnoreCase)
                ? templateName[..^".cshtml".Length]
                : templateName;

        return _engine.CompileRenderAsync(key, model);
    }
}
