namespace LockIn.Abstractions;

public interface IAppDataPathProvider
{
    string GetDataDirectory();   // %AppData%\LockIn
    string GetDatabasePath();    // %AppData%\LockIn\lockin.db
    void EnsureDataDirectory();
}