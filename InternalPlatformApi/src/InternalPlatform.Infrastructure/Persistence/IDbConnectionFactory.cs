using System.Data;

namespace InternalPlatform.Infrastructure.Persistence;

public interface IDbConnectionFactory
{
    IDbConnection Create();
}
