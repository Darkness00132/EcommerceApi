using Application.Common;

namespace Application.Interfaces.Services
{
    public interface IStorageService
    {
        Task<string> UploadAsync(FileDto file, string destination, CancellationToken cancellationToken = default);
        Task DeleteAsync(string key, CancellationToken cancellationToken = default);
    }
}
