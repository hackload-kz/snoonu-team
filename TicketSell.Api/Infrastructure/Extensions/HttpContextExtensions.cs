using TicketSell.Api.Domain.Models;
using TicketSell.Api.Infrastructure.Middlewares;

namespace TicketSell.Api.Infrastructure.Extensions;

public static class HttpContextExtensions
{
    public static User GetCurrentCustomer(this HttpContext httpContext)
    {
        var user =
            httpContext.Items.TryGetValue(BasicAuthMiddleware.UserDataKey, out var userObj)
                ? userObj as User
                : null;

        if (user is null)
        {
            throw new UnauthorizedAccessException();
        }

        return user;
    }
}