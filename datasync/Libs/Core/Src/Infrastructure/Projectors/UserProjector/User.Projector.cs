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

        public void Project(EventType @event)
        {
            var method = GetType().GetMethod($"On{@event.Type}", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (method == null) return;

            var context = JsonSerializer.Deserialize<UserContext>(@event.Context)!;
            var newEvent = new DomainEvent(
                @event.PublisherId,
                @event.Type,
                context,
                @event.OcurredDate
            );

            method.Invoke(this, new object[] { newEvent });
        }
        private async Task OnUserCreated(DomainEvent @event)
        {
            var context = @event.Context;
            var user = new MongoUser
            (
                @event.PublisherId,
                context.GetProperty<string>("SupplierCompanyId"),
                context.GetProperty<string>("Name"),
                context.GetProperty<string>("Image"),
                context.GetProperty<string>("Email"),
                context.GetProperty<string>("Role"),
                context.GetProperty<string>("Status"),
                context.GetProperty<string>("PhoneNumber"),
                context.GetProperty<int>("IdentificationNumber")
            );

            await _userCollection.InsertOneAsync(user);
        }

        private async Task OnSupplierCompanyIdUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var supplierCompanyId = context.GetProperty<string>("SupplierCompanyId");

            await MongoHelper.Update(
                _userCollection,
                @event.PublisherId,
                user => user.SupplierCompanyId,
                supplierCompanyId
            );
        }

        private async Task OnUserNameUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var name = context.GetProperty<string>("Name");
            await MongoHelper.Update(
                _userCollection,
                @event.PublisherId,
                user => user.Name,
                name
            );
        }

        private async Task OnUserImageUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var image = context.GetProperty<string>("Image");

            await MongoHelper.Update(
                _userCollection,
                @event.PublisherId,
                user => user.Image,
                image
            );
        }

        private async Task OnUserEmailUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var email = context.GetProperty<string>("Email");

            await MongoHelper.Update(
                _userCollection,
                @event.PublisherId,
                user => user.Email,
                email
            );
        }

        private async Task OnUserRoleUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var role = context.GetProperty<string>("Role");

            await MongoHelper.Update(
                _userCollection,
                @event.PublisherId,
                user => user.Role,
                role
            );
        }

        private async Task OnUserStatusUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var status = context.GetProperty<string>("Status");

            await MongoHelper.Update(
                _userCollection,
                @event.PublisherId,
                user => user.Status,
                status
            );
        }

        private async Task OnUserPhoneNumberUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var phoneNumber = context.GetProperty<string>("PhoneNumber");

            await MongoHelper.Update(
                _userCollection,
                @event.PublisherId,
                user => user.PhoneNumber,
                phoneNumber
            );
        }

        private async Task OnUserIdentificationNumberUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var identificationNumber = context.GetProperty<int>("IdentificationNumber");

            await MongoHelper.Update(
                _userCollection,
                @event.PublisherId,
                user => user.IdentificationNumber,
                identificationNumber
            );
        }
    }
}