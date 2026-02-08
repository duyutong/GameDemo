using LiteDB;
using System.Reflection;

namespace FlexiServer.Infrastructure.Database
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CollectionNameAttribute : Attribute
    {
        public string Name { get; }
        public CollectionNameAttribute(string name)
        {
            Name = name;
        }
    }
    public class LiteDbHelper
    {
        public static ILiteCollection<T> GetCollection<T>(LiteDatabase db)
        {
            var attr = typeof(T).GetCustomAttribute<CollectionNameAttribute>();
            if (attr == null)
                throw new Exception($"Missing CollectionName for {typeof(T).Name}");

            return db.GetCollection<T>(attr.Name);
        }
    }
}
