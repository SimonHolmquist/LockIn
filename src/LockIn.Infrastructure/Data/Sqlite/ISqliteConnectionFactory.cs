using Microsoft.Data.Sqlite;

namespace LockIn.Infrastructure.Data.Sqlite;
internal interface ISqliteConnectionFactory
{
    SqliteConnection Create(bool readOnly);
}
