using System.Text.Json.Serialization;

namespace TicketSell.Api.Models.Requests;

public class CreateBookingRequest
{
    [JsonPropertyName("event_id")]
    public long EventId { get; set; }
}