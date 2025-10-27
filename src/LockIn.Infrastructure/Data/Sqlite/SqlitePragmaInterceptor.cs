using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace LockIn.Infrastructure.Data.Sqlite;

internal sealed class SqlitePragmaInterceptor : DbConnectionInterceptor
{
    private readonly bool _readOnly;

    public SqlitePragmaInterceptor(bool readOnly)
    {
        _readOnly = readOnly;
    }

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        if (connection is not SqliteConnection sqlite) return;

        using var cmd = sqlite.CreateCommand();
        // Siempre
        cmd.CommandText = "PRAGMA foreign_keys=ON; PRAGMA busy_timeout=5000;";
        cmd.ExecuteNonQuery();

        // WAL + synchronous solo si no estamos en read-only
        if (!_readOnly)
        {
            cmd.CommandText = "PRAGMA journal_mode=WAL; PRAGMA synchronous=NORMAL;";
            cmd.ExecuteNonQuery();
        }

        base.ConnectionOpened(connection, eventData);
    }
}