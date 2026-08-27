using RazorLight;

namespace Infrastructure.Services;

internal sealed class RazorLightRenderer
{
    private const string ResourceRoot = "Infrastructure.EmailTemplates";

    private readonly RazorLightEngine _engine;

    public RazorLightRenderer()
    {
        _engine = new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(typeof(RazorLightRenderer))
            .SetOperatingAssembly(typeof(RazorLightRenderer).Assembly)
            .UseMemoryCachingProvider()
            .Build();
    }

    public Task<string> RenderAsync<TModel>(
        string templateName,
        TModel model)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(templateName);
        ArgumentNullException.ThrowIfNull(model);

        var name = templateName.EndsWith(
            ".cshtml",
            StringComparison.OrdinalIgnoreCase)
                ? templateName[..^".cshtml".Length]
                : templateName;

        var key = $"{ResourceRoot}.{name}.cshtml";

        return _engine.CompileRenderAsync(key, model);
    }
}
