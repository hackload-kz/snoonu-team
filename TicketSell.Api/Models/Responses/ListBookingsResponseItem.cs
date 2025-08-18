using System.Text.Json.Serialization;

namespace TicketSell.Api.Models.Responses;

public class ListBookingsResponseItem
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("event_id")]
    public long EventId { get; set; }

    [JsonPropertyName("seats")]
    public List<ListEventsResponseItemSeat> Seats { get; set; } = new();
}