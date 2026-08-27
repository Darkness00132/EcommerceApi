using Application.Abstractions.Services;
using Application.Exceptions;
using Application.Features.Account.Dto;
using Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Account.Queries.GetCurrent;

public sealed record GetCurrentAccountQuery() : IRequest<CurrentAccountDto>;

public sealed class GetCurrentAccountQueryHandler(UserManager<AppUser> userManager, ICurrentUserService currentUserService) : IRequestHandler<GetCurrentAccountQuery, CurrentAccountDto>
{
    public async Task<CurrentAccountDto> Handle(GetCurrentAccountQuery request, CancellationToken cancellationToken)
    {
        var UserId = currentUserService.UserId.ToString();
        var user = await userManager.FindByIdAsync(UserId.ToString());

        if (user is null)
            throw new NotFoundException("User", UserId);

        return new(user.Id, user.Email!, user.DisplayName);
    }
}

