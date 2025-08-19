using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using TicketSell.Api.Application.Services;

namespace TicketSell.Api.Infrastructure.Middlewares;

public class BasicAuthMiddleware(
    RequestDelegate next)
{
    private const string AuthorizationHeader = "Authorization";
    private const string BasicAuthScheme = "Basic";
    public const string UserDataKey = "UserData";

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.ContainsKey(AuthorizationHeader))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        try
        {
            var authHeader = AuthenticationHeaderValue.Parse(context.Request.Headers[AuthorizationHeader]!);
            if (!BasicAuthScheme.Equals(authHeader.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var credentialBytes = Convert.FromBase64String(authHeader.Parameter ?? string.Empty);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
            if (credentials.Length != 2)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var username = credentials[0];
            var password = credentials[1];

            if (!await ValidateUser(context, username, password))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            await next(context);
        }
        catch
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
    }

    private async Task<bool> ValidateUser(HttpContext context, string username, string password)
    {
        var userProvider = context.RequestServices.GetRequiredService<IUserProvider>();
        var user = await userProvider.GetUser(username, password, CancellationToken.None);
        context.Items[UserDataKey] = user;
        return user is not null;
    }
}