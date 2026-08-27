namespace Application.Common.Files;

public sealed record FileDto(
    string FileName,
    string ContentType,
    long Length,
    Stream Content);
