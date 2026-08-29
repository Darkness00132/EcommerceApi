    using Application.Abstractions.Services;
    using Ecommerce.Api.Constants;
    using Ecommerce.Api.ExceptionHandling;
    using Ecommerce.Api.Services;
    using Hangfire;
    using Microsoft.OpenApi;
    using Serilog;
    using Scalar.AspNetCore;

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();

    builder.Services.AddAntiforgery(options => {
        options.Cookie.Name = AccountApiConstants.AntiforgeryCookieName;

        options.HeaderName = AccountApiConstants.AntiforgeryHeaderName;

        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

    builder.Services.AddApplication(builder.Configuration);
    builder.Services.AddInfrastructure(builder.Configuration);

    builder.Services.AddOpenApi("v1", options => {
        options.AddDocumentTransformer((document, _, _) => {
            var components = document.Components ??= new OpenApiComponents();
            components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "Enter a valid JWT access token."
            };

            return Task.CompletedTask;
        });
    });

    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

    builder.Services.AddSerilog(configuration => {
        configuration.WriteTo.Console();
    });

    var app = builder.Build();

    app.UseExceptionHandler();

    if (app.Environment.IsProduction()) {
        app.UseHsts();
    }

    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseHangfireDashboard(
        "/hangfire",
        new DashboardOptions {
            DashboardTitle = "Ecommerce Background Jobs",
            AppPath = "/"
        });

    app.MapOpenApi();
    app.MapScalarApiReference("/",options => {
        options.WithTitle("Ecommerce API");
        options.Authentication = new ScalarAuthenticationOptions {
            PreferredSecuritySchemes = ["Bearer"]
        };
    });

    app.MapControllers();

    app.Run();
