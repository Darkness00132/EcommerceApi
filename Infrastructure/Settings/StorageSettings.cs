namespace Infrastructure.Settings
{
    public class StorageSettings
    {
        public string BucketName { get; set; } = null!;
        public string AccessKey { get; set; } = null!;
        public string SecretKey { get; set; } = null!;
        public string ServiceUrl { get; set; } = null!;
        public string Region { get; set; } = null!;
    }
}
