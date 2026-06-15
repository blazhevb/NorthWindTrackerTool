using NorthWindTracker.Application;
using NorthWindTracker.Infrastructure;
using NorthWindTracker.Infrastructure.Initialisation;
using Serilog;

// Delete log file on each start so it is overwritten, not appended
var logPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../logs/logs.txt"));
if (File.Exists(logPath)) File.Delete(logPath);

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, config) => config.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

await app.Services.CreateScope().ServiceProvider
    .GetRequiredService<DatabaseInitializer>()
    .EnsureCreatedAsync();

app.UseCors();
app.MapControllers();
app.MapOpenApi();

app.Run();
