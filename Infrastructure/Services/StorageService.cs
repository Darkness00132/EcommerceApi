using Amazon.S3;
using Amazon.S3.Model;
using Application.Common;
using Application.Interfaces.Services;
using Infrastructure.Settings;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services
{
    public class StorageService : IStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly StorageSettings _storageSettings;
        private readonly ILogger<StorageService> _logger;
        private readonly IDistributedCache _cache;

        private static readonly TimeSpan MaxDuration = TimeSpan.FromDays(7);
        public StorageService(IAmazonS3 s3Client, IOptions<StorageSettings> storageSettings, ILogger<StorageService> logger, IDistributedCache cache)
        {
            _s3Client = s3Client;
            _storageSettings = storageSettings.Value;
            _logger = logger;
            _cache = cache;
        }

        public async Task DeleteAsync(string key, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                _logger.LogWarning("DeleteAsync called with empty key. Skipping operation.");
                return;
            }
            try
            {
                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = _storageSettings.BucketName,
                    Key = key
                };

                await _s3Client.DeleteObjectAsync(deleteRequest, cancellationToken);

                _logger.LogInformation("Successfully deleted file with key {Key} from bucket {Bucket}", key, _storageSettings.BucketName);
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError(ex, "S3 error occurred while deleting key {Key}", key);
                throw;
            }
        }

        public async Task DeleteManyAsync(List<string> keys, CancellationToken cancellationToken = default)
        {
            var validKeys = keys?.Where(k => !string.IsNullOrWhiteSpace(k)).ToList();
            if (validKeys == null || !validKeys.Any())
            {
                _logger.LogWarning("DeleteManyAsync called with empty key list. Skipping operation.");
                return;
            }

            try
            {
                var deleteObjectsRequest = new DeleteObjectsRequest
                {
                    BucketName = _storageSettings.BucketName,
                    Objects = validKeys.Select(k => new KeyVersion { Key = k }).ToList()
                };

                var response = await _s3Client.DeleteObjectsAsync(deleteObjectsRequest, cancellationToken);

                _logger.LogInformation("Successfully deleted {Count} files in batch from bucket {Bucket}", response.DeletedObjects.Count, _storageSettings.BucketName);
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError(ex, "S3 error occurred while deleting batch of keys");
                throw;
            }
        }

        public async Task<List<string>> UploadManyAsync(List<FileDto> files, string destination, CancellationToken cancellationToken = default)
        {
            if (files == null || !files.Any())
            {
                throw new ArgumentException("files cannot be null or empty");
            }

            var uploadTasks = files
                .Select(file => UploadAsync(file, destination, cancellationToken))
                .ToList();

            try
            {
                // Wait for all uploads to complete in parallel
                var uploadedKeys = await Task.WhenAll(uploadTasks);
                return uploadedKeys.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Batch upload failed. Cleaning up completed uploads...");

                // Collect keys from tasks that succeeded before/during the failure
                var successfulKeys = uploadTasks
                    .Where(t => t.IsCompletedSuccessfully)
                    .Select(t => t.Result)
                    .ToList();

                if (successfulKeys.Any())
                {
                    await DeleteManyAsync(successfulKeys, CancellationToken.None);
                }

                throw;
            }
        }

        public async Task<string> UploadAsync(FileDto file, string destination, CancellationToken cancellationToken = default)
        {
            if (file == null || file.Content == null || file.Content.Length == 0)
            {
                throw new ArgumentException("File content cannot be null or empty.", nameof(file));
            }

            var fileExtension = Path.GetExtension(file.FileName);
            var cleanDestination = destination.Trim('/');
            var objectKey = string.IsNullOrEmpty(cleanDestination)
            ? $"{Guid.NewGuid()}{fileExtension}"
            : $"{cleanDestination}/{Guid.NewGuid()}{fileExtension}";

            try
            {
                var putRequest = new PutObjectRequest
                {
                    BucketName = _storageSettings.BucketName,
                    Key = objectKey,
                    InputStream = file.Content,
                    ContentType = file.ContentType
                };
                await _s3Client.PutObjectAsync(putRequest, cancellationToken);

                _logger.LogInformation("Successfully uploaded file {FileName} with object key {Key}", file.FileName, objectKey);

                return objectKey;
            }
            catch(AmazonS3Exception ex)
            {
                _logger.LogError(ex, "S3 error occurred while uploading file {FileName} to destination {Destination}", file.FileName, destination);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while uploading file {FileName}", file.FileName);
                throw;
            }
        }

        public async Task<string> GeneratePresignedUrlAsync(string? imageKey, TimeSpan? duration = null)
        {
            if (string.IsNullOrWhiteSpace(imageKey))
                throw new ArgumentException("image key cannot be null or empty");

            var requestedDuration = duration ?? MaxDuration;
            if (requestedDuration > MaxDuration)
                requestedDuration = MaxDuration;

            string cacheKey = $"presigned_url_{imageKey}_{requestedDuration.TotalMinutes}";

            string? cachedUrl = await _cache.GetStringAsync(cacheKey);
            if (cachedUrl != null) return cachedUrl;

            var request = new GetPreSignedUrlRequest
            {
                BucketName = _storageSettings.BucketName,
                Key = imageKey,
                Expires = DateTime.UtcNow.Add(requestedDuration)
            };
            string newUrl = _s3Client.GetPreSignedURL(request);

            // Use 1 hour for long durations, but fallback to 5% for short ones so it never goes negative
            var buffer = requestedDuration > TimeSpan.FromHours(2)
                            ? TimeSpan.FromHours(1)
                            : TimeSpan.FromMinutes(requestedDuration.TotalMinutes * 0.05);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = requestedDuration - buffer
            };

            await _cache.SetStringAsync(cacheKey, newUrl,options);

            return newUrl;
        }
    }
}
