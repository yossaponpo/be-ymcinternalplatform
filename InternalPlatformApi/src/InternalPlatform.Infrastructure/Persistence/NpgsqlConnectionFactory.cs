using System.Data;
using Npgsql;

namespace InternalPlatform.Infrastructure.Persistence;

public sealed class NpgsqlConnectionFactory(string connectionString)
    : IDbConnectionFactory
{
    public IDbConnection Create()
        => new NpgsqlConnection(connectionString);
}
