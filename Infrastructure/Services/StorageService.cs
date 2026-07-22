using Application.Common;
using Application.Interfaces.Services;

namespace Infrastructure.Services
{
    public class StorageService : IStorageService
    {
        public Task DeleteAsync(string key, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<string> UploadAsync(FileDto file, string destination, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
