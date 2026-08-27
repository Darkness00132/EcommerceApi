using Application.Common.Files;
using Application.Constants;

namespace Application.Abstractions.Services;

public interface IImageManipulationService
{
    Task<FileDto> ResizeImageAsync(FileDto file, ImageType type
        , CancellationToken cancellationToken = default);
}
