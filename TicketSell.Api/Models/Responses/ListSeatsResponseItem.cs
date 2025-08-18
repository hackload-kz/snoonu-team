using System.Text.Json.Serialization;
using TicketSell.Api.Domain.Models;

namespace TicketSell.Api.Models.Responses;

public class ListSeatsResponseItem
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("row")]
    public long Row { get; set; }

    [JsonPropertyName("number")]
    public long Number { get; set; }

    [JsonPropertyName("status")]
    public SeatStatusEnum Status { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }
}