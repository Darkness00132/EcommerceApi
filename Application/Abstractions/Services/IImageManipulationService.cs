using Application.Common.Files;
using Application.Constants;

namespace Application.Abstractions.Services;

public interface IImageManipulationService
{
    Task<FileDto> ResizeImage(FileDto file, ImageType type
        ,CancellationToken cancellationToken=default);
}
