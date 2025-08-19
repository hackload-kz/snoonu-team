using System.Globalization;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using TicketSell.Api.Domain.Models;

namespace TicketSell.Api.Infrastructure;

public class DatabaseInitializer(
    AppDbContext dbContext,
    IHostEnvironment environment,
    ILogger<DatabaseInitializer> logger)
{
    private const int BatchSize = 100;

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.Database.MigrateAsync(cancellationToken);

        await LoadUsers(cancellationToken);
        await LoadEvents(cancellationToken);
    }

    public async Task LoadEvents(CancellationToken cancellationToken)
    {
        await LoadEventsInternal(Path.Combine(environment.ContentRootPath, "Infrastructure/DatabaseData/events.json"), 1, cancellationToken);
        await LoadEventsInternal(Path.Combine(environment.ContentRootPath, "Infrastructure/DatabaseData/events_archive.json"), 100000, cancellationToken);
    }

    private async Task LoadEventsInternal(string jsonPath, long minId, CancellationToken cancellationToken)
    {
        if (dbContext.Events.Any(x => x.Id >= minId))
        {
            logger.LogWarning("Events are already loaded. Skipped loading events from file");
            return;
        }

        if (!File.Exists(jsonPath))
        {
            logger.LogWarning("File {Path} not found. Events are not loaded", jsonPath);
            return;
        }

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        await using var fs = File.OpenRead(jsonPath);

        var buffer = new List<Event>(BatchSize);
        var totalInserted = 0;

        await foreach (var item in JsonSerializer.DeserializeAsyncEnumerable<Event>(fs, options, cancellationToken))
        {
            if (item is null) continue;
            item.StartAt = item.StartAt.ToUniversalTime();
            buffer.Add(item);

            if (buffer.Count >= BatchSize)
            {
                totalInserted += await InsertBatchAsync(buffer, cancellationToken);
                buffer.Clear();
            }
        }

        if (buffer.Count > 0)
        {
            totalInserted += await InsertBatchAsync(buffer, cancellationToken);
            buffer.Clear();
        }

        logger.LogInformation("Inserted {Count} events", totalInserted);
    }

    private async Task LoadUsers(CancellationToken cancellationToken)
    {
        if (dbContext.Users.Any())
        {
            logger.LogWarning("Users are already loaded. Skipped loading Users from file");
            return;
        }

        var filePath = Path.Combine(environment.ContentRootPath, "Infrastructure/DatabaseData/users.csv");

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            BadDataFound = null
        });

        csv.Context.RegisterClassMap<UserMap>();

        var batch = new List<User>(BatchSize);
        foreach (var user in csv.GetRecords<User>())
        {
            user.RegisteredAt = user.RegisteredAt.ToUniversalTime();
            user.LastLoggedIn = user.LastLoggedIn.ToUniversalTime();
            batch.Add(user);


            if (batch.Count >= BatchSize)
            {
                await InsertBatchAsync(batch, cancellationToken);
                batch = new List<User>(BatchSize);
            }
        }

        if (batch.Count > 0)
        {
            await InsertBatchAsync(batch, cancellationToken);
        }
    }

    private async Task<int> InsertBatchAsync<T>(
        List<T> batch,
        CancellationToken cancellationToken) where T : class
    {
        var prevDetectChanges = dbContext.ChangeTracker.AutoDetectChangesEnabled;
        try
        {
            dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

            dbContext.Set<T>().AddRange(batch);

            var inserted = await dbContext.SaveChangesAsync(cancellationToken);

            dbContext.ChangeTracker.Clear();

            return inserted;
        }
        finally
        {
            dbContext.ChangeTracker.AutoDetectChangesEnabled = prevDetectChanges;
        }
    }

    private sealed class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Map(m => m.UserId).Index(0);
            Map(m => m.Email).Index(1);
            Map(m => m.PasswordHash).Index(2);
            Map(m => m.PasswordPlain).Index(3).Optional();
            Map(m => m.FirstName).Index(4);
            Map(m => m.Surname).Index(5);
            Map(m => m.Birthday).Index(6).Convert(row =>
            {
                var value = row.Row.GetField(6);
                return string.IsNullOrWhiteSpace(value) ? null : DateOnly.Parse(value, CultureInfo.InvariantCulture);
            });
            Map(m => m.RegisteredAt).Index(7).TypeConverterOption.Format("yyyy-MM-dd HH:mm:ss");
            Map(m => m.IsActive).Index(8);
            Map(m => m.LastLoggedIn).Index(9).TypeConverterOption.Format("yyyy-MM-dd HH:mm:ss");
        }
    }
}