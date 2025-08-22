using Microsoft.EntityFrameworkCore;
using TicketSell.Api.Application.Services;
using TicketSell.Api.Infrastructure;
using TicketSell.Api.Infrastructure.Middlewares;
using TicketSell.Api.Infrastructure.Options;

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
var connectionString = builder.Configuration.GetConnectionString("TicketSellDb");
builder.Services.AddScoped<ITicketSellRepository>(sp =>
{
    var databaseInitializer = sp.GetRequiredService<DatabaseInitializer>();
    return new TicketSellRepositoryAdoNet(connectionString!, databaseInitializer);
});
builder.Services.AddScoped<DatabaseInitializer>();
builder.Services.AddOptions<TicketSellSettings>(nameof(TicketSellSettings));

// Application part
builder.Services.AddScoped<IUserProvider, UserProvider>();

var app = builder.Build();

await InitializeDb(app);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseMiddleware<BasicAuthMiddleware>();
app.MapControllers();
app.Run();

async Task InitializeDb(WebApplication webApplication)
{
    using var scope = webApplication.Services.CreateScope();
    var databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await databaseInitializer.InitializeAsync();
}