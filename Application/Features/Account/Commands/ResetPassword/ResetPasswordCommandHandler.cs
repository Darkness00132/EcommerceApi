using Application.Abstractions.Repositories;
using Application.Exceptions;
using Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Account.Commands.ResetPassword;

internal class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IRepository<RefreshToken> _refreshTokens;
    private readonly IUnitOfWork _unitOfWork;

    public ResetPasswordCommandHandler(UserManager<AppUser> userManager, IRepository<RefreshToken> refreshTokens, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _refreshTokens = refreshTokens;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email)
            ?? throw new UnauthorizedException("The password reset request is invalid.");

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

        if (!result.Succeeded) {
            var errors = result.Errors
                .GroupBy(x => x.Code)
                .ToDictionary(x => x.Key, x => x.Select(e => e.Description)
                .ToArray());

            throw new ValidationException(errors);
        }

        var activeRefreshTokens = await _refreshTokens.ListAsync(
            x => x.UserId == user.Id && x.RevokedAt == null,
            cancellationToken);

        foreach (var refreshToken in activeRefreshTokens) {
            user.RevokeRefreshToken(refreshToken.Token);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

