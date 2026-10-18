using NLog.Extensions.Logging;
using NLog.Web;
using projectFamilyTasks.Api.Extensions;
using projectFamilyTasks.Application;
using projectFamilyTasks.Api.Middleware;
using projectFamilyTasks.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();

builder.Host.UseNLog(new NLogAspNetCoreOptions { RemoveLoggerFactoryFilter = false });

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddApiLayer(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();

app.UseRateLimiter();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<PasswordChangeRequiredMiddleware>();

app.MapControllers();

await app.InitializeDatabaseAsync();

app.Run();

public partial class Program;
