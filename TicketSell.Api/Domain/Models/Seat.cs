namespace TicketSell.Api.Domain.Models;

public class Seat
{
    public long Id { get; set; }

    public int Row { get; set; }

    public int Number { get; set; }

    public SeatStatusEnum Status { get; set; }
}