namespace Application.Common
{
    public record FileDto(Stream Content, string ContentType, string? FileName);
}
