using Application.Abstractions.Repositories;
using Application.Exceptions;
using Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Account.Commands.ResetPassword;

internal class ResetPasswordCommandHandler(UserManager<AppUser> userManager, IRepository<Domain.Entities.Identity.RefreshToken> refreshTokens, IUnitOfWork unitOfWork) : IRequestHandler<ResetPasswordCommand>
{
    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email)
            ?? throw new UnauthorizedException("The password reset request is invalid.");

        var result = await userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

        if (!result.Succeeded) {
            var errors = result.Errors
                .GroupBy(x => x.Code)
                .ToDictionary(x => x.Key, x => x.Select(e => e.Description)
                .ToArray());

            throw new ValidationException(errors);
        }

        var activeRefreshTokens = await refreshTokens.ListAsync(
            x => x.UserId == user.Id && x.RevokedAt == null,
            cancellationToken);

        foreach (var refreshToken in activeRefreshTokens) {
            refreshToken.Revoke();
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

