using Application.Abstractions.Services;
using Application.Common.Files;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Infrastructure.Services.Storage;

internal class AzureBlobStorageService : IStorageService
{
    private readonly BlobContainerClient _containerClient;

    public AzureBlobStorageService(BlobContainerClient containerClient)
    {
        _containerClient = containerClient;
    }

    public async Task<string> UploadAsync(
        FileDto file,
        string destination,
        CancellationToken cancellationToken = default)
    {
        var normalizedFolder = string.IsNullOrWhiteSpace(destination)
            ? string.Empty
            : $"{destination.Trim('/')}";

        var blobKey = $"{normalizedFolder}/{Guid.NewGuid()}_{file.FileName}".TrimStart('/');
        var blobClient = _containerClient.GetBlobClient(blobKey);

        if (file.Content.CanSeek && file.Content.Position != 0) {
            file.Content.Position = 0;
        }

        await blobClient.UploadAsync(
            file.Content,
            new BlobUploadOptions {
                HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType }
            },
            cancellationToken);

        return blobKey;
    }

    public async Task<IEnumerable<string>> UploadManyAsync(
        IEnumerable<FileDto> files,
        string destination,
        CancellationToken cancellationToken = default)
    {
        var uploadTasks = files.Select(file => UploadAsync(file, destination, cancellationToken));
        return await Task.WhenAll(uploadTasks);
    }

    public async Task DeleteAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            return;

        var cleanKey = key.TrimStart('/');
        var blobClient = _containerClient.GetBlobClient(cleanKey);

        await blobClient.DeleteIfExistsAsync(
            DeleteSnapshotsOption.IncludeSnapshots,
            cancellationToken: cancellationToken);
    }

    public async Task DeleteManyAsync(
        IEnumerable<string> keys,
        CancellationToken cancellationToken = default)
    {
        var deleteTasks = keys
            .Where(key => !string.IsNullOrWhiteSpace(key))
            .Select(key => DeleteAsync(key, cancellationToken));

        await Task.WhenAll(deleteTasks);
    }
}
