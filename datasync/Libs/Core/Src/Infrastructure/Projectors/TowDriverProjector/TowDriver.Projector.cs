using datasync.Libs.Core.Src.Infrastructure.Projectors.TowDriverProjector.Models;
using datasync.Libs.Core.Src.Infrastructure.Projectors.TowDriverProjector.Types;
using Datasync.Core;
using MassTransit.Transports;
using MongoDB.Driver;
using RabbitMQ.Contracts;
using System.Reflection;
using System.Text.Json;

namespace datasync.Libs.Core.Src.Infrastructure.Projectors.TowDriverProjector
{
    public class TowDriverProjector : IProjector
    {
        private readonly IMongoCollection<MongoTowDriver> _towDriverCollection;

        public TowDriverProjector()
        {
            MongoClient client = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION_URI"));
            IMongoDatabase database = client.GetDatabase(Environment.GetEnvironmentVariable("DATABASE_NAME"));
            _towDriverCollection = database.GetCollection<MongoTowDriver>("tow-drivers");
        }
        public void Project(EventType @event)
        {
            var method = GetType().GetMethod($"On{@event.Type}", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (method == null) return;

            Type typeOfContext = null;
            if (@event.Type.Contains("TowDriver", StringComparison.OrdinalIgnoreCase)) typeOfContext = typeof(TowDriverContext);

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

        private async Task OnTowDriverCreated(DomainEvent @event)
        {
            var context = @event.Context;
            var towDriver = new MongoTowDriver
            (
                @event.PublisherId,
                context.GetProperty<string>("TowDriverName"),
                context.GetProperty<string>("TowDriverEmail"),
                context.GetProperty<string>("LicenseOwnerName"),
                context.GetProperty<DateOnly>("LicenseIssueDate"),
                context.GetProperty<DateOnly>("LicenseExpirationDate"),
                context.GetProperty<string>("MedicalCertificateOwnerName"),
                context.GetProperty<int>("MedicalCertificateOwnerAge"),
                context.GetProperty<DateOnly>("MedicalCertificateIssueDate"),
                context.GetProperty<DateOnly>("MedicalCertificateExpirationDate"),
                context.GetProperty<int>("TowDriverIdentificationNumber"),
                context.GetProperty<string>("TowDriverLocation"),
                context.GetProperty<string>("TowDriverStatus")
            );
            await _towDriverCollection.InsertOneAsync(towDriver);
        }

        private async Task OnTowDriverNameUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var name = context.GetProperty<string>("TowDriverName");

            await MongoHelper.Update(
                _towDriverCollection,
                @event.PublisherId,
                towDriver => towDriver.Name,
                name
            );
        }

        private async Task OnTowDriverEmailUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var email = context.GetProperty<string>("TowDriverEmail");

            await MongoHelper.Update(
                _towDriverCollection,
                @event.PublisherId,
                towDriver => towDriver.Email,
                email
            );
        }

        private async Task OnTowDriverDrivingLicenseUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var drivingLicenseOwnerName = context.GetProperty<string>("LicenseOwnerName");
            var drivingLicenseIssueDate = context.GetProperty<DateOnly>("LicenseIssueDate");
            var drivingLicenseExpirationDate = context.GetProperty<DateOnly>("LicenseExpirationDate");

            var filter = Builders<MongoTowDriver>.Filter.Eq(towDriver => towDriver.TowDriverId, @event.PublisherId);
            var update = Builders<MongoTowDriver>.Update
                .Set(towDriver => towDriver.DrivingLicenseOwnerName, drivingLicenseOwnerName)
                .Set(towDriver => towDriver.DrivingLicenseIssueDate, drivingLicenseIssueDate)
                .Set(towDriver => towDriver.DrivingLicenseExpirationDate, drivingLicenseExpirationDate);

            await _towDriverCollection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
        }

        private async Task OnTowDriverMedicalCertificateUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var medicalCertificateOwnerName = context.GetProperty<string>("MedicalCertificateOwnerName");
            var medicalCertificateOwnerAge = context.GetProperty<int>("MedicalCertificateOwnerAge");
            var medicalCertificateIssueDate = context.GetProperty<DateOnly>("MedicalCertificateIssueDate");
            var medicalCertificateExpirationDate = context.GetProperty<DateOnly>("MedicalCertificateExpirationDate");

            var filter = Builders<MongoTowDriver>.Filter.Eq(towDriver => towDriver.TowDriverId, @event.PublisherId);
            var update = Builders<MongoTowDriver>.Update
                .Set(towDriver => towDriver.MedicalCertificateOwnerName, medicalCertificateOwnerName)
                .Set(towDriver => towDriver.MedicalCertificateOwnerAge, medicalCertificateOwnerAge)
                .Set(towDriver => towDriver.MedicalCertificateIssueDate, medicalCertificateIssueDate)
                .Set(towDriver => towDriver.MedicalCertificateExpirationDate, medicalCertificateExpirationDate);

            await _towDriverCollection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
        }

        private async Task OnTowDriverIdentificationNumberUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var identificationNumber = context.GetProperty<int>("TowDriverIdentificationNumber");

            await MongoHelper.Update(
                _towDriverCollection,
                @event.PublisherId,
                towDriver => towDriver.IdentificationNumber,
                identificationNumber
            );
        }

        private async Task OnTowDriverLocationUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var location = context.GetProperty<string>("TowDriverLocation");

            await MongoHelper.Update(
                _towDriverCollection,
                @event.PublisherId,
                towDriver => towDriver.Location,
                location
            );
        }
        private async Task OnTowDriverStatusUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var TowDriverStatus = context.GetProperty<string>("TowDriverStatus");

            await MongoHelper.Update(
                _towDriverCollection,
                @event.PublisherId,
                towDriver => towDriver.Status,
                TowDriverStatus
            );
        }
    }
}
