using MediatR;

namespace Application.Features.Account.Commands.Register;

public sealed record RegisterAccountCommand(string FirstName, string LastName, string Email, string Password) : IRequest;

