using Application.Common;

namespace Application.Interfaces.Services
{
    public interface IStorageService
    {
        Task<string> GeneratePresignedUrlAsync(string? imageKey, TimeSpan? duration = null);

        Task<string> UploadAsync(FileDto file, string destination, CancellationToken cancellationToken = default);

        Task<List<string>> UploadManyAsync(List<FileDto> files, string destination, CancellationToken cancellationToken = default);

        Task DeleteAsync(string key, CancellationToken cancellationToken = default);

        Task DeleteManyAsync(List<string> keys, CancellationToken cancellationToken = default);
    }
}
