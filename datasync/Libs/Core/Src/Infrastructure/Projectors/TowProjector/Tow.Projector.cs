using MongoDB.Driver;
using RabbitMQ.Contracts;
using System.Reflection;
using System.Text.Json;

namespace Datasync.Core
{
    public class TowProjector : IProjector
    {
        private readonly IMongoCollection<MongoTow> _towCollection;
        public TowProjector()
        {
            MongoClient client = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION_URI"));
            IMongoDatabase database = client.GetDatabase(Environment.GetEnvironmentVariable("DATABASE_NAME"));
            _towCollection = database.GetCollection<MongoTow>("tows");
        }

        public void Project(EventType @event)
        {
            var method = GetType().GetMethod($"On{@event.Type}", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (method == null) return;
            var context = JsonSerializer.Deserialize<TowContext>(@event.Context)!;
            var newEvent = new DomainEvent(
                @event.PublisherId,
                @event.Type,
                context,
                @event.OcurredDate
            );

            method.Invoke(this, new object[] { newEvent });
        }
        
        private async Task OnTowCreated(DomainEvent @event)
        {
            var context = @event.Context;
            var tow = new MongoTow
            (
                @event.PublisherId,
                context.GetProperty<string>("Brand"),
                context.GetProperty<string>("Model"),
                context.GetProperty<string>("Color"),
                context.GetProperty<string>("LicensePlate"),
                context.GetProperty<string>("Location"),
                context.GetProperty<int>("Year"),
                context.GetProperty<string>("SizeType"),
                context.GetProperty<string>("Status")
            );

            await _towCollection.InsertOneAsync(tow);
        }

        private async Task OnTowBrandUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var brand = context.GetProperty<string>("Brand");

            await MongoHelper.Update(
                _towCollection,
                @event.PublisherId,
                tow => tow.Brand,
                brand
            );
        }

        private async Task OnTowModelUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var model = context.GetProperty<string>("Model");

            await MongoHelper.Update(
                _towCollection,
                @event.PublisherId,
                tow => tow.Model,
                model
            );
        }

        private async Task OnTowColorUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var color = context.GetProperty<string>("Color");

            await MongoHelper.Update(
                _towCollection,
                @event.PublisherId,
                tow => tow.Color,
                color
            );                          
        }

        private async Task OnTowLicensePlateUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var licensePlate = context.GetProperty<string>("LicensePlate");

            await MongoHelper.Update(
                _towCollection,
                @event.PublisherId,
                tow => tow.LicensePlate,
                licensePlate
            );
        }

        private async Task OnTowLocationUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var location = context.GetProperty<string>("Location");

            await MongoHelper.Update(
                _towCollection,
                @event.PublisherId,
                tow => tow.Location,
                location
            );
        }

        private async Task OnTowYearUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var year = context.GetProperty<int>("Year");

            await MongoHelper.Update(
                _towCollection,
                @event.PublisherId,
                tow => tow.Year,
                year
            );
        }

        private async Task OnTowSizeTypeUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var sizeType = context.GetProperty<string>("SizeType");

            await MongoHelper.Update(
                _towCollection,
                @event.PublisherId,
                tow => tow.SizeType,
                sizeType
            );
        }

        private async Task OnTowStatusUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var status = context.GetProperty<string>("Status");

            await MongoHelper.Update(
                _towCollection,
                @event.PublisherId,
                tow => tow.Status,
                status
            );
        }
    }
}