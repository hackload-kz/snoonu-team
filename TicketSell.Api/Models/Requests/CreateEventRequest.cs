using System.Text.Json.Serialization;

namespace TicketSell.Api.Models.Requests;

public class CreateEventRequest
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("external")]
    public bool External { get; set; } = false;
}