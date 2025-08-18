using System.Text.Json.Serialization;

namespace TicketSell.Api.Models.Responses;

public class ListEventsResponseItem
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
}