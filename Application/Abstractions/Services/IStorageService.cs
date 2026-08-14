using Application.Common.Files;

namespace Application.Abstractions.Services;

public interface IStorageService
{
    Task<string> UploadAsync(
        FileDto file,
        string destination,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<string>> UploadManyAsync(
        IEnumerable<FileDto> files,
        string destination,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string key,
        CancellationToken cancellationToken = default);

    Task DeleteManyAsync(
        IEnumerable<string> keys,
        CancellationToken cancellationToken = default);
}
