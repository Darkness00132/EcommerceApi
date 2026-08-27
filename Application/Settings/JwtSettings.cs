namespace Application.Settings;

public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = null!;

    public string Audience { get; init; } = null!;

    public string Key { get; init; } = null!;

    public int AccessTokenExpirationInMinutes { get; init; }

    public int RefreshTokenExpirationInDays { get; init; }
}
