using MongoDB.Bson.Serialization.Attributes;

namespace Datasync.Core
{
    public class MongoUser(string userId, string name, string email, int identificationNumber) : IEntity
    {
        [BsonId]
        public string UserId = userId;
        public string Name = name;
        public string Email = email;
        public int IdentificationNumber = identificationNumber;
        string IEntity.Id => UserId;
    }
}

