using System.Linq.Expressions;
using MongoDB.Driver;

namespace Datasync.Core
{
    public static class MongoHelper
    {
        public static async Task MongoUpdate<T, U>(IMongoCollection<T> collection, string publisherId, Expression<Func<T, U>> propertyExpression, U newValue) where T : IEntity
        {
            var filter = Builders<T>.Filter.Eq(collection => collection.Id, publisherId);
            var update = Builders<T>.Update.Set(propertyExpression, newValue);

            await collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
        }
    }
}
