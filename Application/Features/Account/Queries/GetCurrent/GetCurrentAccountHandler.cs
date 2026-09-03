using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.Exceptions;
using Application.Features.Account.Dto;
using Domain.Entities.Identity;
using MediatR;

namespace Application.Features.Account.Queries.GetCurrent;

internal class GetCurrentAccountHandler : IRequestHandler<GetCurrentAccountQuery, CurrentAccountDto>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IRepository<AppUser> _userRepository;

    public GetCurrentAccountHandler(ICurrentUserService currentUser, IRepository<AppUser> userRepository)
    {
        _currentUser = currentUser;
        _userRepository = userRepository;
    }

    public async Task<CurrentAccountDto> Handle(GetCurrentAccountQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository
            .ProjectToSingleOrDefaultAsync<CurrentAccountDto>(u => u.Id == _currentUser.UserId, cancellationToken);
        if (user is null)
            throw new NotFoundException("Current user not found.");

        return user;
    }
}
