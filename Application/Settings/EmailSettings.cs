namespace Application.Settings;

public sealed class EmailSettings
{
    public const string SectionName = "Email";
    public string Host { get; init; } = null!;
    public int Port { get; init; } = 587;
    public bool UseSslOnConnect { get; init; }
    public string UserName { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string From { get; init; } = null!;
}
