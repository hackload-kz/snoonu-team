using Microsoft.EntityFrameworkCore;
using TicketSell.Api.Domain.Models;
using TicketSell.Api.Infrastructure;

namespace TicketSell.Api.Application.Services;

public interface IUserProvider
{
    Task<User?> GetUser(string email, string password, CancellationToken cancellationToken);
}

public class UserProvider(AppDbContext dbContext) : IUserProvider
{
    public async Task<User?> GetUser(string email, string password, CancellationToken cancellationToken)
    {
        return await dbContext
            .Users
            .FirstOrDefaultAsync(
                x => x.Email == email && x.PasswordPlain == password && x.IsActive,
                cancellationToken);
    }
}