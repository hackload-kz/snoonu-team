namespace TicketSell.Api.Domain.Models;

public class Event
{
    public long Id { get; set; }

    public required string Title { get; set; }

    public bool External { get; set; }
}