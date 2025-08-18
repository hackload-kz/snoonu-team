using System.Text.Json.Serialization;

namespace TicketSell.Api.Models.Responses;

public class ListEventsResponseItemSeat
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
}