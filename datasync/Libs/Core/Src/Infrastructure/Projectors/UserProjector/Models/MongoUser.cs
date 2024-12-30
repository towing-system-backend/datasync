using MongoDB.Bson.Serialization.Attributes;

namespace Datasync.Core
{
    public class MongoUser(
        string userId,
        string supplierCompanyId,
        string name,
        string image,
        string email,
        string role,
        string status,
        string phoneNumber,
        int identificationNumber
        ) : IEntity
    {
        [BsonId]
        public string UserId = userId;
        public string SupplierCompanyId = supplierCompanyId;
        public string Name = name;
        public string Image = image;
        public string Email = email;
        public string Role = role;
        public string Status = status;
        public string PhoneNumber = phoneNumber;
        public int IdentificationNumber = identificationNumber;
        public DateTime CreatedAt = DateTime.Now;
        string IEntity._id => UserId;
    }
}