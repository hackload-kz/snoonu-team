namespace TicketSell.Api.Domain.Models;

public enum BookingStatus
{
    Created,
    SeatsChosen,
    PaymentInitiated,
    PaymentCompleted,
    Cancelled
}