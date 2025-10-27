using LockIn.Abstractions;
using Microsoft.Data.Sqlite;

namespace LockIn.Infrastructure.Data.Sqlite;

internal sealed class SqliteConnectionFactory : ISqliteConnectionFactory
{
    private readonly IAppDataPathProvider _paths;
    public SqliteConnectionFactory(IAppDataPathProvider paths) => _paths = paths;

    public SqliteConnection Create(bool readOnly)
    {
        var csb = new SqliteConnectionStringBuilder
        {
            DataSource = _paths.GetDatabasePath(),
            Mode = readOnly ? SqliteOpenMode.ReadOnly : SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Shared,
            Pooling = true
        };
        return new SqliteConnection(csb.ConnectionString);
    }
}