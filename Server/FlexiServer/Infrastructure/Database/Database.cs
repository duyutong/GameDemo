using LiteDB;
namespace FlexiServer.Infrastructure.Database
{
    public class Database
    {
        public LiteDatabase Db { get; }
        public ILiteCollection<AccountInfo_DB> Accounts { get; }
        public Database(string role) 
        {
            var dbDir = Path.Combine("Infrastructure", "Data", role);
            if(!Directory.Exists(dbDir)) Directory.CreateDirectory(dbDir);

            var dbPath = Path.Combine(dbDir, "ServerData.db");
            Db = new LiteDatabase(dbPath);
            Accounts = LiteDbHelper.GetCollection<AccountInfo_DB>(Db);
        }
        public void InitializeDatabase() 
        {
            Accounts.EnsureIndex(x => x.Account, unique: true);
        }
        public void Dispose()
        {
            Db?.Dispose();
        }
    }
}
