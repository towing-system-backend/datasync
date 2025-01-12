﻿using MongoDB.Bson.Serialization.Attributes;

namespace Datasync.Core
{
    public class MongoOrder(
        string orderId,
        string status,
        string issueLocation,
        string destination,
        string issuer,
        string? TowDriverAssigned,
        string details,
        string name,
        string image,
        string policyId,
        string phoneNumber,
        int IdentificationNumber,
        decimal totalCost,
        double totalDistance,
        List<MongoAdditionalCost>? additionalCosts
    ) :
        IEntity
    {
        [BsonId]
        public string OrderId = orderId;
        public string Status = status;
        public string IssueLocation = issueLocation;
        public string Destination = destination;
        public string? TowDriverAssigned = TowDriverAssigned;
        public string Details = details;
        public string Name = name;
        public string Image = image;
        public string PolicyId = policyId;
        public string PhoneNumber = phoneNumber;
        public decimal TotalCost = totalCost;
        public double TotalDistance = totalDistance;
        public List<MongoAdditionalCost>? AdditionalCosts = additionalCosts;

        string IEntity._id => OrderId;
    }

    public class MongoAdditionalCost
    (
        string id,
        string? name = null,
        string? category = null,
        decimal? amount = null
    ) : IEntity
    {
        [BsonId]
        public string? Id = id;
        public string? Name = name;
        public string? Category = category;
        public decimal? Amount = amount;
        string IEntity._id => Id;
    }
}
