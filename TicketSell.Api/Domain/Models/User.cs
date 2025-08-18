namespace TicketSell.Api.Domain.Models;

public class User
{
    public long UserId { get; set; }

    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public string? PasswordPlain { get; set; }

    public required string FirstName { get; set; }

    public required string Surname { get; set; }

    public DateOnly? Birthday { get; set; }

    public DateTimeOffset RegisteredAt { get; set; }

    public bool IsActive { get; set; }

    public DateTimeOffset LastLoggedIn { get; set; }
}