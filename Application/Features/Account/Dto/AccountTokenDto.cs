namespace Application.Features.Account.Dto;

public sealed record AccountTokenDto(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt, DateTime RefreshTokenExpiresAt);

