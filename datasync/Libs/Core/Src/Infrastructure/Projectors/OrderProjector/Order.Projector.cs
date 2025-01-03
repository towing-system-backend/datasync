using datasync.Libs.Core.Src.Infrastructure.Projectors.OrderProjector.Models;
using datasync.Libs.Core.Src.Infrastructure.Projectors.OrderProjector.Types;
using Datasync.Core;
using MongoDB.Driver;
using RabbitMQ.Contracts;
using System;
using System.Reflection;
using System.Text.Json;

namespace datasync.Libs.Core.Src.Infrastructure.Projectors.OrderProjector
{
    public class OrderProjector : IProjector
    {

        private readonly IMongoCollection<MongoOrder> _orderCollection;
        public OrderProjector()
        {
            MongoClient client = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION_URI"));
            IMongoDatabase database = client.GetDatabase(Environment.GetEnvironmentVariable("DATABASE_NAME"));
            _orderCollection = database.GetCollection<MongoOrder>("orders");
        }
        public void Project(EventType @event)
        {
            var method = GetType().GetMethod($"On{@event.Type}", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if(method == null) return;

            Type typeOfContext = null;
            if (@event.Type.Contains("Order", StringComparison.OrdinalIgnoreCase)) typeOfContext = typeof(OrderContext);
            if (@event.Type.Contains("AdditionalCost", StringComparison.OrdinalIgnoreCase)) typeOfContext = typeof(AdditionalCostContext);

            if (typeOfContext == null) return;

            var context = JsonSerializer.Deserialize(@event.Context, typeOfContext!);
            var newEvent = new DomainEvent(
                @event.PublisherId,
                @event.Type,
                context!,
                @event.OcurredDate
            );
            method.Invoke(this, new object[] { newEvent });

            return;
        }

        private async Task OnOrderCreated(DomainEvent @event)
        {
            var context = @event.Context;
            var order = new MongoOrder
            (
                @event.PublisherId,
                context.GetProperty<string>("Status"),
                context.GetProperty<string>("IssueLocation"),
                context.GetProperty<string>("Destination"),
                context.GetProperty<string>("TowDriverAssigned"),
                context.GetProperty<string>("Details"),
                context.GetProperty<string>("Name"),
                context.GetProperty<string>("Image"),
                context.GetProperty<string>("PolicyId"),
                context.GetProperty<string>("PhoneNumber"),
                context.GetProperty<decimal>("TotalCost"),
                new List<MongoAdditionalCost>()
            );

            await _orderCollection.InsertOneAsync(order);
        }
        private async Task OnOrderStatusUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var status = context.GetProperty<string>("Status");

            await MongoHelper.Update(
                _orderCollection,
                @event.PublisherId,
                order => order.Status,
                status
            );
        }

        private async Task OnOrderTowDriverAssignedUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var towDriverAssigned = context.GetProperty<string>("TowDriverAssigned");

            await MongoHelper.Update(
                _orderCollection,
                @event.PublisherId,
                order => order.TowDriverAssigned,
                towDriverAssigned
            );
        }

        private async Task OnOrderDestinationLocationUpdated(DomainEvent @event) 
        {
            var context = @event.Context;
            var destination = context.GetProperty<string>("Destination");

            await MongoHelper.Update(
                _orderCollection,
                @event.PublisherId,
                order => order.Destination,
                destination
            );
        }

        private async Task OnAdditionalCostCreated(DomainEvent @event)
        {
            var context = @event.Context;
            var additionalCost = new MongoAdditionalCost(
                context.GetProperty<string>("Id"),
                context.GetProperty<string>("Name"),
                context.GetProperty<string>("Category"),
                context.GetProperty<decimal>("Amount")
            );

            var filter = Builders<MongoOrder>.Filter.Eq(order => order.OrderId, @event.PublisherId);
            var update = Builders<MongoOrder>.Update.Push(order => order.AdditionalCosts, additionalCost);
            await _orderCollection.UpdateOneAsync(filter, update);
        }

        private async Task OnAdditionalCostRemoved(DomainEvent @event)
        {
            var context = @event.Context;
            var additionalCostId = context.GetProperty<string>("Id");

            var filter = Builders<MongoOrder>.Filter.Eq(order => order.OrderId, @event.PublisherId);
            var update = Builders<MongoOrder>.Update.PullFilter(
                 order => order.AdditionalCosts,
                 cost => cost.Id == additionalCostId
            );
            await _orderCollection.UpdateOneAsync(filter, update);
        }

    }
}
