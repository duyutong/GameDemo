using FlexiServer.Models;
using FlexiServer.Models.Common;
using System.Numerics;
using FlexiServer.Infrastructure.Database;
using static EnumDefinitions;
using LiteDB;
namespace FlexiServer.Models.Common
{
    [CollectionName("users")]
    public class AccountInfo
    {
        [BsonId]
        public int BsonId { get; set; }
        public string Account { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime CreateTime { get; set; }

    }
}
