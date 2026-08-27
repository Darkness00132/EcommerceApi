using Application.Common.Files;

namespace Ecommerce.Api.Extensions;

/// <summary>
/// Provides extension methods for form files.
/// </summary>
public static class FormFileExtensions
{
    /// <summary>
    /// Converts an <see cref="IFormFile"/> to a <see cref="FileDto"/>.
    /// </summary>
    /// <param name="file">The uploaded form file.</param>
    /// <returns>The application file DTO.</returns>
    public static FileDto ToFileDto(this IFormFile file)
    {
        return new FileDto(
            file.FileName,
            file.ContentType,
            file.Length,
            file.OpenReadStream());
    }
}
