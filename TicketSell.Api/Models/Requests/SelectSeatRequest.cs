using System.Text.Json.Serialization;

namespace TicketSell.Api.Models.Requests;

public class SelectSeatRequest
{
    [JsonPropertyName("booking_id")]
    public long BookingId { get; set; }

    [JsonPropertyName("seat_id")]
    public long SeatId { get; set; }
}