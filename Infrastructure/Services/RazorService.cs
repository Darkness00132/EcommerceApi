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
                    "Infrastructure.ThirdPartyServices.Email.Templates")
                    .SetOperatingAssembly(infrastructureAssembly)
                    .AddMetadataReferences( MetadataReference.CreateFromFile(applicationAssembly.Location))
                    .UseMemoryCachingProvider()
                    .Build();
        }


        public async Task<string> GetHtmlAsync<TModel>(
            string templateName,
            TModel model)
        {
            return await _razorEngine.CompileRenderAsync($"{templateName}.cshtml", model);
        }
    }
}