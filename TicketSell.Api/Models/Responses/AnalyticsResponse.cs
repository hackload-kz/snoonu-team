using System.Text.Json.Serialization;

namespace TicketSell.Api.Models.Responses;

public class AnalyticsResponse
{
    [JsonPropertyName("event_id")]
    public long EventId { get; set; }

    [JsonPropertyName("total_seats")]
    public int TotalSeats { get; set; }

    [JsonPropertyName("sold_seats")]
    public int SoldSeats { get; set; }

    [JsonPropertyName("reserved_seats")]
    public int ReservedSeats { get; set; }

    [JsonPropertyName("free_seats")]
    public int FreeSeats { get; set; }

    [JsonPropertyName("total_revenue")]
    public decimal TotalRevenue { get; set; }

    [JsonPropertyName("bookings_count")]
    public int BookingsCount { get; set; }
}