using AuthECAPI.Controllers;
using AuthECAPI.Extensions;
using AuthECAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Extension Methods added for "IServiceCollection" type
builder.Services.AddSwaggerExplorer()
                .InjectDBContext(builder.Configuration)
                .AddConfigForAppSettings(builder.Configuration)
                .AddIdentityHandlersAndStore()
                .ConfigureIdentityOptions()
                .AddIdentityAuth(builder.Configuration);

var app = builder.Build();

app.ConfigureSwaggerExplorer()
    .ConfigureCors(builder.Configuration)
    .AddIdentityAuthMiddlewares();

app.UseHttpsRedirection();

app.MapControllers();

app.MapGroup("/api")
   .MapIdentityApi<AppUser>();

app.MapGroup("/api")
    .MapIdentityUserEndpoints(builder.Configuration);

app.Run();
