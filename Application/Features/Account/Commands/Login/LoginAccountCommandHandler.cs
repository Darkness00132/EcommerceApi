using Application.Features.Account.Dto;
using Application.Features.Account.Services;
using Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Account.Commands.Login;

internal class LoginAccountCommandHandler(UserManager<AppUser> userManager, AccountTokenService tokenService) : IRequestHandler<LoginAccountCommand, AccountTokenDto>
{
    public async Task<AccountTokenDto> Handle(LoginAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var isCorrectPassword = await userManager.CheckPasswordAsync(user, request.Password);

        if (!isCorrectPassword)
            throw new UnauthorizedAccessException("Invalid email or password.");

        return await tokenService.CreateAsync(user, cancellationToken);
    }
}

