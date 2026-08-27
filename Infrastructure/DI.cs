using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.Constants;
using Application.Settings;
using CloudinaryDotNet;
using Domain.Entities.Identity;
using Hangfire;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

public static class DI
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add infrastructure services here
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("The DefaultConnection connection string is not configured.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddIdentity<AppUser, AppRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        var jwt = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? throw new InvalidOperationException("JWT settings are required.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters {
                ValidateIssuer = true,
                ValidIssuer = jwt.Issuer,
                ValidateAudience = true,
                ValidAudience = jwt.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                ValidateLifetime = true,
                NameClaimType = JwtRegisteredClaimNames.Sub,
                RoleClaimType = System.Security.Claims.ClaimTypes.Role
            });

        services.AddSingleton<ICloudinary>(sp => {
            var account = new Account(
                configuration.GetValue<string>("Cloudinary:CloudName"),
                configuration.GetValue<string>("Cloudinary:ApiKey"),
                configuration.GetValue<string>("Cloudinary:ApiSecret"));

            return new Cloudinary(account);
        });

        services.AddHangfire(options => {
            options.UseSqlServerStorage(connectionString);
        });

        services.AddHangfireServer(options => {
            options.Queues =
            [
                BackgroundJobQueues.Critical,
                BackgroundJobQueues.Default,
                BackgroundJobQueues.Low
            ];
        });

        services.AddSingleton<RazorLightRenderer>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.AddScoped<IEmailSender, MailKitEmailSender>();
        services.AddScoped<IBackgroundJobService, HangfireBackgroundJobService>();
        services.AddScoped<IStorageService, CloudinaryStorageService>();
        services.AddScoped<IImageManipulationService, SixLaborsImageService>();

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
