using MongoDB.Driver;
using RabbitMQ.Contracts;
using System.Reflection;
using System.Text.Json;

namespace Datasync.Core
{
    public class UserProjector : IProjector
    {
        private readonly IMongoCollection<MongoUser> _userCollection;
        public UserProjector()
        {
            MongoClient client = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION_URI"));
            IMongoDatabase database = client.GetDatabase(Environment.GetEnvironmentVariable("DATABASE_NAME"));
            _userCollection = database.GetCollection<MongoUser>("users");
        }

        public Task Project(EventType @event)
        {
            var method = GetType().GetMethod($"On{@event.Type}", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (method == null) return Task.FromException(new InvalidOperationException($"Projector for event {@event.Type} not found."));

            var context = JsonSerializer.Deserialize<UserContext>(@event.Context)!;
            var newEvent = new DomainEvent(
                @event.PublisherId,
                @event.Type,
                context,
                @event.OcurredDate
            );
            method.Invoke(this, new object[] { newEvent });

            return Task.CompletedTask;
        }

        private async Task OnUserNameUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var name = context.GetProperty<string>("Name");

            await MongoHelper.MongoUpdate(
                _userCollection,
                @event.PublisherId,
                user => user.Name,
                name
            );
        }

        private async Task OnUserEmailUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var email = context.GetProperty<string>("Email");

            await MongoHelper.MongoUpdate(
                _userCollection,
                @event.PublisherId,
                user => user.Email,
                email
            );
        }

        private async Task OnUserIdentificationNumberUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var identificationNumber = context.GetProperty<int>("IdentificationNumber");

            await MongoHelper.MongoUpdate(
                _userCollection,
                @event.PublisherId,
                user => user.IdentificationNumber,
                identificationNumber
            );
        }

        private async Task OnUserCreated(DomainEvent @event)
        {
            var context = @event.Context;
            var user = new MongoUser
            (
                @event.PublisherId,
                context.GetProperty<string>("Name"),
                context.GetProperty<string>("Email"),
                context.GetProperty<int>("IdentificationNumber")
            );

            await _userCollection.InsertOneAsync(user);
        }
    }
}
