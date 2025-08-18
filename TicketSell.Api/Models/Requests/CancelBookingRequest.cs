using System.Text.Json.Serialization;

namespace TicketSell.Api.Models.Requests;

public class CancelBookingRequest
{
    [JsonPropertyName("booking_id")]
    public long BookingId { get; set; }
}