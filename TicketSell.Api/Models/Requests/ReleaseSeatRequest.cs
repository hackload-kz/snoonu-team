using System.Text.Json.Serialization;

namespace TicketSell.Api.Models.Requests;

public class ReleaseSeatRequest
{
    [JsonPropertyName("seat_id")]
    public long SeatId { get; set; }
}