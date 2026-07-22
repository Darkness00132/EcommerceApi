using Application.Common;
using Domain.Enums;

namespace Application.Interfaces.Services
{
    public interface IImageProcessor
    {
        Task<FileDto> ProcessAsync(
        FileDto file,
        ImageType imageType,
        CancellationToken cancellationToken = default);
    }
}
