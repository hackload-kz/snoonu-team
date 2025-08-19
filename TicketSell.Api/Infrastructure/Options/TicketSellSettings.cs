namespace TicketSell.Api.Infrastructure.Options;

public class TicketSellSettings
{
    public required string HostName { get; set; }

    public required string PaymentUrl { get; set; }

    public required string EventProviderUrl { get; set; }

    public required string PaymentMerchantId { get; set; }

    public required string PaymentMerchantPassword { get; set; }
}