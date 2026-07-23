using Amazon.S3;
using Application;
using Application.Common.Behavior;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using Application.Settings;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using Hangfire;
using Infrastructure.Persistence;
using Infrastructure.Repostiories;
using Infrastructure.Services;
using Infrastructure.Services.Email;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region Settings

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.Configure<StorageSettings>(
    builder.Configuration.GetSection("StorageSettings"));

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

builder.Services.AddStackExchangeRedisCache(options => 
{ 
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

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

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(CachingBehavior<,>));
});

builder.Services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddAutoMapper(config => config.AddMaps(AppDomain.CurrentDomain.GetAssemblies()));

builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfireServer(options => options.Queues = new[]
{
    BackgroundJopPriority.Critical.ToString().ToLowerInvariant(),
    BackgroundJopPriority.Default.ToString().ToLowerInvariant(),
    BackgroundJopPriority.Low.ToString().ToLowerInvariant()
});

builder.Services.AddSingleton<IAmazonS3>(options =>
{
    var settings = options.GetRequiredService<IOptions<StorageSettings>>().Value;
    var config = new AmazonS3Config
    {
        ServiceURL = settings.ServiceUrl,
        AuthenticationRegion = settings.Region,
        ForcePathStyle = true
    };
    return new AmazonS3Client(settings.AccessKey, settings.SecretKey, config);
});

builder.Services.AddScoped<AuthenticationResultFactory>();

builder.Services.AddScoped<IJwtService,JwtService>();
builder.Services.AddScoped<IRefreshTokenService,RefreshTokenService>();


builder.Services.AddSingleton<RazorService>();
builder.Services.AddScoped<IStorageService,StorageService>();
builder.Services.AddScoped<IImageProcessor, ImageProcessor>();
builder.Services.AddTransient<IEmailService , EmailService>();
builder.Services.AddScoped<IBackgroundJobService, HangFireService>();

//repositories
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddScoped<ICategoryRepository,CategoryRepository>();
builder.Services.AddScoped<IProductRepository,ProductRepository>();
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
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();
app.MapHangfireDashboard("/hangfire")
    .RequireAuthorization(new AuthorizeAttribute { Roles = UserRoles.Admin.ToString() });

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