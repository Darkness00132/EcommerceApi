using RazorLight;

internal sealed class RazorLightRenderer
{
    private readonly RazorLightEngine _engine;

    public RazorLightRenderer()
    {
        _engine = new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(typeof(RazorLightRenderer))
            .UseMemoryCachingProvider()
            .EnableDebugMode()
            .Build();
    }

    public Task<string> RenderAsync<TModel>(
        string templateName,
        TModel model)
    {
        var name = templateName?.Trim() ?? string.Empty;

        try
        {
            name = Path.GetFileName(name) ?? name;
        }
        catch
        {
            // If Path.GetFileName fails for any reason, fall back to the raw name.
        }

        name = name.Replace(" ", string.Empty);

        if (name.EndsWith(".cshtml", System.StringComparison.OrdinalIgnoreCase))
        {
            name = name.Substring(0, name.Length - ".cshtml".Length);
        }

        var key = $"Infrastructure.EmailTemplates.{name}.cshtml";

        return _engine.CompileRenderAsync(key, model);
    }
}