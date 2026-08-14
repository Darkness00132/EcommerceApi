namespace Ecommerce.Api.Contracts.Account;

public sealed record WebAccountResponse(string AccessToken, DateTime AccessTokenExpiresAt);
