using Application;
using Microsoft.CodeAnalysis;
using RazorLight;

namespace Infrastructure.Services
{
    public class RazorService
    {
        private readonly RazorLightEngine _razorEngine;

        public RazorService()
        {
            var infrastructureAssembly = typeof(RazorService).Assembly;
            var applicationAssembly = typeof(AssemblyReference).Assembly;

            _razorEngine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(
                    infrastructureAssembly,
                    "Infrastructure.Services.Email.Templates")
                .SetOperatingAssembly(infrastructureAssembly)
                .AddMetadataReferences(MetadataReference.CreateFromFile(applicationAssembly.Location))
                .UseMemoryCachingProvider()
                .EnableDebugMode()
                .Build();
        }

        public async Task<string> GetHtmlAsync<TModel>(string templateName, TModel model)
        {
            // Ensure .cshtml extension is present
            string key = templateName.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase)
                ? templateName
                : $"{templateName}.cshtml";

            return await _razorEngine.CompileRenderAsync(key, model);
        }
    }
}