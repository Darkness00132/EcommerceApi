using Application.Abstractions.Services;
using Application.Common.Files;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Infrastructure.Services;

internal class CloudinaryStorageService(ICloudinary cloudinary)
    : IStorageService
{
    public async Task<string> UploadAsync(
        FileDto file,
        string destination,
        CancellationToken cancellationToken = default)
    {
        var uploadParams = new ImageUploadParams {
            File = new FileDescription(
                file.FileName,
                file.Content),
            Folder = destination,
            UseFilename = true,
            UniqueFilename = true
        };

        var result = await cloudinary.UploadAsync(
            uploadParams,
            cancellationToken);

        ValidateUploadResult(result);

        return result.PublicId;
    }

    public async Task<IEnumerable<string>> UploadManyAsync(
        IEnumerable<FileDto> files,
        string destination,
        CancellationToken cancellationToken = default)
    {
        var tasks = files.Select(file =>
            UploadAsync(file, destination, cancellationToken));

        return await Task.WhenAll(tasks);
    }

    public async Task DeleteAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        var result = await cloudinary.DestroyAsync(
            new DeletionParams(key) {
                Invalidate = true
            });

        ValidateDeleteResult(result, key);
    }

    public async Task DeleteManyAsync(
        IEnumerable<string> keys,
        CancellationToken cancellationToken = default)
    {
        var publicIds = keys.ToArray();

        if (publicIds.Length == 0)
            return;

        var result = await cloudinary.DeleteResourcesAsync(
            ResourceType.Image,
            publicIds);

        if (result.Error is not null) {
            throw new InvalidOperationException(
                result.Error.Message);
        }
    }

    private static void ValidateUploadResult(
        ImageUploadResult result)
    {
        if (result.Error is not null) {
            throw new InvalidOperationException(
                result.Error.Message);
        }

        if (string.IsNullOrWhiteSpace(result.PublicId)) {
            throw new InvalidOperationException(
                "Cloudinary did not return a public id.");
        }
    }

    private static void ValidateDeleteResult(
        DeletionResult result,
        string key)
    {
        if (result.Error is not null) {
            throw new InvalidOperationException(
                result.Error.Message);
        }

        if (result.Result is not ("ok" or "not found")) {
            throw new InvalidOperationException(
                $"Failed to delete file '{key}'.");
        }
    }
}
