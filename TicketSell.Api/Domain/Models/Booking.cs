namespace TicketSell.Api.Domain.Models;

public class Booking
{
    public long Id { get; set; }

    public long EventId { get; set; }

    public List<long> Seats { get; set; } = new();

    public BookingStatus Status { get; set; }
}