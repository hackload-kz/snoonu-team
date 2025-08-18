using System.Text.Json.Serialization;

namespace TicketSell.Api.Domain.Models;

public class Event
{
    public long Id { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public string? Type { get; set; }

    [JsonPropertyName("datetime_start")]
    public DateTimeOffset StartAt { get; set; }

    public string? Provider { get; set; }

    public bool External { get; set; }
}