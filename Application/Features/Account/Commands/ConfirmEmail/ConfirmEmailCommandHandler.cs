using Application.Exceptions;
using Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Account.Commands.ConfirmEmail;

internal class ConfirmEmailCommandHandler(UserManager<AppUser> userManager) : IRequestHandler<ConfirmEmailCommand>
{
    public async Task Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email) ?? throw new UnauthorizedAccessException("The email confirmation request is invalid.");

        var result = await userManager.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded) {
            var errors = result.Errors
                .GroupBy(x => x.Code)
                .ToDictionary(x => x.Key, x => x.Select(e => e.Description)
                .ToArray());

            throw new ValidationException(errors);
        }
    }
}

