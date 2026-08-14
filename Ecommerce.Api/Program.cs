using Application.Abstractions.Services;
using Ecommerce.Api.Constants;
using Ecommerce.Api.ExceptionHandling;
using Ecommerce.Api.Services;
using Hangfire;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = AccountApiConstants.AntiforgeryCookieName;

    options.HeaderName = AccountApiConstants.AntiforgeryHeaderName;

    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    var xmlFileName =
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    var xmlFilePath =
        Path.Combine(AppContext.BaseDirectory, xmlFileName);

    options.IncludeXmlComments(xmlFilePath);
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddSerilog(configuration =>
{
    configuration.WriteTo.Console();
});

var app = builder.Build();

app.UseExceptionHandler();

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.RoutePrefix = string.Empty;

    options.SwaggerEndpoint(
        "/swagger/v1/swagger.json",
        "Ecommerce API v1");
});

if (app.Environment.IsProduction())
{
    app.UseHsts();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard(
    "/hangfire",
    new DashboardOptions
    {
        DashboardTitle = "Ecommerce Background Jobs",
        AppPath = "/"
    });

app.MapControllers();

app.Run();