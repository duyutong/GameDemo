using System.Numerics;
using FlexiServer.Infrastructure.Database;
using static EnumDefinitions;
using LiteDB;
namespace FlexiServer.Infrastructure.Database
{
    [CollectionName("users")]
    public class AccountInfo_DB
    {
        [BsonId]
        public int BsonId { get; set; }
        public string Account { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime CreateTime { get; set; }

    }
}
