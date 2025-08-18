using Microsoft.EntityFrameworkCore;
using TicketSell.Api.Application.Services;
using TicketSell.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

//Database part
builder.Services.AddDbContext<AppDbContext>(
    opt =>
        opt.UseNpgsql(builder.Configuration.GetConnectionString("TicketSellDb")));
builder.Services.AddScoped<ITicketSellRepository, TicketSellRepositoryEntityFramework>();
builder.Services.AddScoped<DatabaseInitializer>();


// Application part
builder.Services.AddScoped<IUserProvider, UserProvider>();

var app = builder.Build();

await InitializeDb(app);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

async Task InitializeDb(WebApplication webApplication)
{
    using var scope = webApplication.Services.CreateScope();
    var databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await databaseInitializer.InitializeAsync();
}