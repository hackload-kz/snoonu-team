using TicketSell.Api.Domain.Models;

namespace TicketSell.Api.Infrastructure;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;

public class TicketSellRepositoryAdoNet(string connectionString, DatabaseInitializer initializer)
    : ITicketSellRepository
{
    public async Task ResetDatabase()
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM \"Bookings\"; DELETE FROM \"Seats\"; DELETE FROM \"Events\";";
        await cmd.ExecuteNonQueryAsync();

        await initializer.LoadEvents(CancellationToken.None);
    }

    public async Task<List<Event>> GetEvents(string? query, DateTimeOffset? date, int? page, int? pageSize, CancellationToken cancellationToken)
    {
        page ??= 1;
        pageSize ??= 20;

        var events = new List<Event>();
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync(cancellationToken);

        var sql = """
                  SELECT "Id", "Title", "Description", "Type", "Provider", "StartAt"
                  FROM "Events"
                  WHERE 1=1
                  """;

        if (date is not null)
            sql += " AND CAST(\"StartAt\" AS date) = @date";

        if (!string.IsNullOrEmpty(query))
            sql += " AND (\"Title\" ILIKE @q OR \"Description\" ILIKE @q OR \"Type\" ILIKE @q OR \"Provider\" ILIKE @q)";

        sql += " ORDER BY \"StartAt\" OFFSET @skip LIMIT @take";

        await using var cmd = new NpgsqlCommand(sql, conn);
        if (date is not null)
            cmd.Parameters.AddWithValue("@date", date.Value.UtcDateTime.Date);
        if (!string.IsNullOrEmpty(query))
            cmd.Parameters.AddWithValue("@q", "%" + query + "%");

        cmd.Parameters.AddWithValue("@skip", (page.Value - 1) * pageSize.Value);
        cmd.Parameters.AddWithValue("@take", pageSize.Value);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            events.Add(new Event
            {
                Id = reader.GetInt64(0),
                Title = reader.GetString(1),
                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                Type = reader.IsDBNull(3) ? null : reader.GetString(3),
                Provider = reader.IsDBNull(4) ? null : reader.GetString(4),
                StartAt = reader.GetDateTime(5)
            });
        }

        return events;
    }

    public async Task<List<Booking>> GetBookings(long customerId)
    {
        var bookings = new List<Booking>();
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand("SELECT \"Id\", \"EventId\", \"UserId\", \"Status\" FROM \"Bookings\" WHERE \"UserId\"=@uid", conn);
        cmd.Parameters.AddWithValue("@uid", customerId);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            bookings.Add(new Booking
            {
                Id = reader.GetInt64(0),
                EventId = reader.GetInt64(1),
                UserId = reader.GetInt64(2),
                Status = (BookingStatus)reader.GetInt32(3)
            });
        }

        return bookings;
    }

    public async Task<Booking> GetBooking(long customerId, long bookingId)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand("SELECT \"Id\", \"EventId\", \"UserId\", \"Status\" FROM \"Bookings\" WHERE \"Id\"=@bid AND \"UserId\"=@uid", conn);
        cmd.Parameters.AddWithValue("@bid", bookingId);
        cmd.Parameters.AddWithValue("@uid", customerId);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) throw new Exception("Booking not found");

        return new Booking
        {
            Id = reader.GetInt64(0),
            EventId = reader.GetInt64(1),
            UserId = reader.GetInt64(2),
            Status = (BookingStatus)reader.GetInt32(3)
        };
    }

    public async Task<long> AddBooking(long customerId, long eventId)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        var sql = """
                  INSERT INTO "Bookings" ("EventId", "UserId", "Status")
                  VALUES (@eid, @uid, @status)
                  RETURNING "Id";
                  """;


        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@eid", eventId);
        cmd.Parameters.AddWithValue("@uid", customerId);
        cmd.Parameters.AddWithValue("@status", (int)BookingStatus.Created);

        var id = (long)(await cmd.ExecuteScalarAsync() ?? 0);
        return id;
    }

    public async Task<Booking> InitiatePayment(long customerId, long bookingId)
    {
        return await UpdateStatus(customerId, bookingId, BookingStatus.PaymentInitiated);
    }

    public async Task CancelPayment(long customerId, long bookingId)
    {
        await UpdateStatus(customerId, bookingId, BookingStatus.Cancelled);
    }

    public async Task ApprovePayment(long bookingId)
    {
        await UpdateStatus(null, bookingId, BookingStatus.PaymentCompleted);
    }

    public async Task CancelPayment(long bookingId)
    {
        await UpdateStatus(null, bookingId, BookingStatus.Cancelled);
    }

    private async Task<Booking> UpdateStatus(long? customerId, long bookingId, BookingStatus status)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        var sql = "UPDATE \"Bookings\" SET \"Status\"=@status WHERE \"Id\"=@bid";
        if (customerId != null)
            sql += " AND \"UserId\"=@uid";

        sql += "; SELECT \"Id\", \"EventId\", \"UserId\", \"Status\" FROM \"Bookings\" WHERE \"Id\"=@bid";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@status", (int)status);
        cmd.Parameters.AddWithValue("@bid", bookingId);
        if (customerId != null)
            cmd.Parameters.AddWithValue("@uid", customerId);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) throw new Exception("Booking not found");

        return new Booking
        {
            Id = reader.GetInt64(0),
            EventId = reader.GetInt64(1),
            UserId = reader.GetInt64(2),
            Status = (BookingStatus)reader.GetInt32(3)
        };
    }
}
