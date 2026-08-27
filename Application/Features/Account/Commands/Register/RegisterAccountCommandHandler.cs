using Application.Abstractions.Services;
using Application.Exceptions;
using Application.Features.Account.Dto;
using Domain.Entities.Identity;
using Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Application.Features.Account.Commands.Register;

internal class RegisterAccountCommandHandler(UserManager<AppUser> userManager, IBackgroundJobService backgroundJobs, IConfiguration configuration) : IRequestHandler<RegisterAccountCommand>
{
    public async Task Handle(RegisterAccountCommand request, CancellationToken cancellationToken)
    {
        if (await userManager.FindByEmailAsync(request.Email) is not null)
            throw new ConflictException("A user with this email already exists.");

        var user = new AppUser(new FullName(request.FirstName, request.LastName), request.Email);

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) {
            throw new ValidationException(result.Errors.GroupBy(x => x.Code).ToDictionary(x => x.Key, x => x.Select(e => e.Description).ToArray()));
        }

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var frontendUrl = configuration["FrontendUrl"] ?? throw new InvalidOperationException("FrontendUrl configuration is required.");

        var url = $"{frontendUrl.TrimEnd('/')}/account/confirm-email?email={Uri.EscapeDataString(user.Email!)}&token={Uri.EscapeDataString(token)}";

        var email = user.Email!;
        var model = new EmailConfirmationEmailModel(user.DisplayName, url);

        backgroundJobs.Enqueue<IEmailSender>(sender => sender.SendAsync(email, "Confirm your email", "ConfirmEmail", model));
    }
}


