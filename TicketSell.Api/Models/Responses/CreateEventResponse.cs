using System.Text.Json.Serialization;

namespace TicketSell.Api.Models.Responses;

public class CreateEventResponse
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
}