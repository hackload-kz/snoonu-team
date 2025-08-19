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

    Task<List<Booking>> GetBookings(long customerId);

    Task<Booking> GetBooking(long customerId, long bookingId);

    Task<long> AddBooking(long customerId, long eventId);

    Task<Booking> InitiatePayment(long customerId, long bookingId);

    Task CancelPayment(long customerId, long bookingId);

    Task ApprovePayment(long bookingId);

    Task CancelPayment(long bookingId);
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

    public async Task<List<Booking>> GetBookings(long customerId)
    {
        return await dbContext.Bookings.Where(x => x.UserId == customerId).ToListAsync();
    }

    public async Task<Booking> GetBooking(long customerId, long bookingId)
    {
        return await dbContext.Bookings.Where(x => x.Id == bookingId && x.UserId == customerId).FirstAsync();

    }

    public async Task<long> AddBooking(long customerId, long eventId)
    {
        var trackedBooking = await dbContext.Bookings.AddAsync(new Booking
        {
            EventId = eventId,
            UserId = customerId
        });

        await dbContext.SaveChangesAsync();

        return trackedBooking.Entity.Id;
    }

    public async Task<Booking> InitiatePayment(long customerId, long bookingId)
    {
       var booking = await dbContext.Bookings.Where(x => x.Id == bookingId && x.UserId == customerId).FirstAsync();
       booking.Status = BookingStatus.PaymentInitiated;
       await dbContext.SaveChangesAsync();

       return booking;
    }

    public async Task CancelPayment(long customerId, long bookingId)
    {
        var booking = await dbContext.Bookings.Where(x => x.Id == bookingId && x.UserId == customerId).FirstAsync();
        booking.Status = BookingStatus.Cancelled;
        await dbContext.SaveChangesAsync();
    }

    public async Task ApprovePayment(long bookingId)
    {
        var booking = await dbContext.Bookings.Where(x => x.Id == bookingId).FirstAsync();
        booking.Status = BookingStatus.PaymentCompleted;
        await dbContext.SaveChangesAsync();
    }

    public async Task CancelPayment(long bookingId)
    {
        var booking = await dbContext.Bookings.Where(x => x.Id == bookingId).FirstAsync();
        booking.Status = BookingStatus.Cancelled;
        await dbContext.SaveChangesAsync();
    }
}