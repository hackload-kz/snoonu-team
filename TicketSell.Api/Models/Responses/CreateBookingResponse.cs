using System.Text.Json.Serialization;

namespace TicketSell.Api.Models.Responses;

public class CreateBookingResponse
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
}