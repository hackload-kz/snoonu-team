using Microsoft.EntityFrameworkCore;
using TicketSell.Api.Domain.Models;

namespace TicketSell.Api.Infrastructure;

public interface ITicketSellRepository
{
    Task ResetDatabase();

    //TODO: think about ElasticSearch
    //TODO: add indexes
    Task<List<Event>> GetEvents(
        string? query,
        DateTimeOffset? date,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken);
}

public class TicketSellRepositoryEntityFramework(
    DatabaseInitializer initializer,
    AppDbContext dbContext) : ITicketSellRepository
{
    public async Task ResetDatabase()
    {
        await dbContext.Events.ExecuteDeleteAsync();
        await dbContext.Bookings.ExecuteDeleteAsync();
        await dbContext.Seats.ExecuteDeleteAsync();

        await initializer.LoadEvents(CancellationToken.None);
    }

    public async Task<List<Event>> GetEvents(string? query, DateTimeOffset? date, int? page, int? pageSize, CancellationToken cancellationToken)
    {
        page ??= 1;
        pageSize ??= 20;

        var eventsQuery = dbContext.Events.AsQueryable();

        if (date is not null)
        {
            eventsQuery = eventsQuery.Where(x => x.StartAt.Date == date.Value.UtcDateTime.Date);
        }

        if (!string.IsNullOrEmpty(query))
        {
            eventsQuery = eventsQuery.Where(x =>
                x.Title.Contains(query)
                || (x.Description != null && x.Description.Contains(query))
                || (x.Type != null && x.Type.Contains(query))
                || (x.Provider != null && x.Provider.Contains(query)));
        }

        eventsQuery = eventsQuery.Skip((page.Value - 1) * pageSize.Value);
        eventsQuery = eventsQuery.Take(pageSize.Value);

        return await eventsQuery.ToListAsync(cancellationToken);
    }
}