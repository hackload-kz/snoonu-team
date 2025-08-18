using Microsoft.EntityFrameworkCore;

namespace TicketSell.Api.Infrastructure;

public interface ITicketSellRepository
{
    Task ResetDatabase();
}

public class TicketSellRepositoryEntityFramework(
    DatabaseInitializer initializer,
    AppDbContext dbContext): ITicketSellRepository
{

    public async Task ResetDatabase()
    {
        await dbContext.Events.ExecuteDeleteAsync();
        await dbContext.Bookings.ExecuteDeleteAsync();
        await dbContext.Seats.ExecuteDeleteAsync();

        await initializer.LoadEvents(CancellationToken.None);
    }
}