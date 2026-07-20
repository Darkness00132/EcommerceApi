using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Ecommerce.Services;
using Infrastructure;
using Infrastructure.Services;
using Infrastructure.Settings;
using Infrastructure.ThirdPartyServices.Email;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region Settings

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

builder.Services.Configure<FrontendSettings>(
    builder.Configuration.GetSection("Frontend"));

#endregion

#region Serilog

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

#endregion

#region Database

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

#endregion

#region Identity

builder.Services
    .AddIdentity<AppUser, AppRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;

        options.User.RequireUniqueEmail = true;

        options.SignIn.RequireConfirmedEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

#endregion

#region JWT

var jwt = builder.Configuration.GetSection("Jwt");

builder.Services
.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwt["Key"]!))
    };
});

builder.Services.AddAuthorization();

#endregion

#region Controllers

builder.Services.AddControllers();

#endregion

#region Documentation

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();

        document.Components.SecuritySchemes ??=
            new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["Bearer"] =
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Enter JWT token"
            };

        document.SetReferenceHostDocument();

        return Task.CompletedTask;
    });
});

#endregion

#region Security

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "XSRF-TOKEN";
    options.HeaderName = "X-XSRF-TOKEN";

    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

#endregion

#region Register Services

builder.Services.AddAutoMapper(config => config.AddMaps(AppDomain.CurrentDomain.GetAssemblies()));
builder.Services.AddMemoryCache(config => config.SizeLimit = 1024);

builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<IJwtService,JwtService>();
builder.Services.AddScoped<IRefreshTokenService,RefreshTokenService>();
builder.Services.AddSingleton<RazorService>();
builder.Services.AddTransient<IEmailService , EmailService>();
//builder.Services.AddScoped<>();

#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/", options =>
    {
        options.WithTitle("Ecommerce API Documentation");
    });
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler();
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseSerilogRequestLogging();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

#region initalize roles
using var scope = app.Services.CreateScope();

var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

foreach (var roleName in Enum.GetNames<UserRoles>())
{
    if (!await roleManager.RoleExistsAsync(roleName))
    {
        await roleManager.CreateAsync(new AppRole
        {
            Name = roleName
        });
    }
}
#endregion

app.Run();