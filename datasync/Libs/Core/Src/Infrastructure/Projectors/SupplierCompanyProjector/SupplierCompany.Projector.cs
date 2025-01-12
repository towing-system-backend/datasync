using MongoDB.Driver;
using RabbitMQ.Contracts;
using System.Reflection;
using System.Text.Json;

namespace Datasync.Core
{
    public class SupplierCompanyProjector : IProjector
    {
        private readonly IMongoCollection<MongoSupplierCompany> _supplierCompanyCollection;
        public SupplierCompanyProjector()
        {
            MongoClient client = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION_URI"));
            IMongoDatabase database = client.GetDatabase(Environment.GetEnvironmentVariable("DATABASE_NAME"));
            _supplierCompanyCollection = database.GetCollection<MongoSupplierCompany>("supplier-companies");
        }
        public void Project(EventType @event)
        {
            var method = GetType().GetMethod($"On{@event.Type}", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (method == null) return;

            Type typeOfContext = null;
            if (@event.Type.Contains("SupplierCompany", StringComparison.OrdinalIgnoreCase)) 
                typeOfContext = typeof(SupplierCompanyContext);
            if (@event.Type.Contains("Department", StringComparison.OrdinalIgnoreCase)) 
                typeOfContext = typeof(Department);
            if (@event.Type.Contains("Policy", StringComparison.OrdinalIgnoreCase))
                typeOfContext = typeof(Policy);
            if (@event.Type.Contains("TowDriver", StringComparison.OrdinalIgnoreCase))
                typeOfContext = typeof(TowDriver);

            if (typeOfContext == null) return;

            var context = JsonSerializer.Deserialize(@event.Context, typeOfContext!);

            var newEvent = new DomainEvent(
                @event.PublisherId,
                @event.Type,
                context,
                @event.OcurredDate
            );

            method.Invoke(this, new object[] { newEvent });
        }

        private async Task OnSupplierCompanyCreated(DomainEvent @event)
        {
            var context = @event.Context;
            var departments = context.GetProperty<List<Department>>("Departments");
            var policies = context.GetProperty<List<Policy>>("Policies");

            var mongoDepartments = departments.Select(department => new MongoDepartment(
                    department.DepartmentId,
                    department.Name,
                    department.Employees
                )
            ).ToList();

            var mongoPolicies = policies.Select(policy => new MongoPolicy(
                    policy.PolicyId,
                    policy.Title,
                    policy.CoverageAmount,
                    policy.Price, policy.Type,
                    policy.IssuanceDate,
                    policy.ExpirationDate
                )
            ).ToList();

            var supplierCompany = new MongoSupplierCompany
            (
                @event.PublisherId,
                mongoDepartments,
                mongoPolicies,
                context.GetProperty<List<string>>("TowDrivers"),
                context.GetProperty<string>("Name"),
                context.GetProperty<string>("PhoneNumber"),
                context.GetProperty<string>("Type"),
                context.GetProperty<string>("Rif"),
                context.GetProperty<string>("State"),
                context.GetProperty<string>("City"),
                context.GetProperty<string>("Street")
            );
            await _supplierCompanyCollection.InsertOneAsync(supplierCompany);
        }

        private async Task OnDepartmentRegistered(DomainEvent @event)
        {
            var context = @event.Context;
            var departmentId = context.GetProperty<string>("DepartmentId");
            var departmentName = context.GetProperty<string>("Name");
            var employees = context.GetProperty<List<string>>("Employees");

            var mongoDepartment = new MongoDepartment(
                departmentId,
                departmentName,
                employees
            );

            await MongoHelper.AddToSet(
                _supplierCompanyCollection,
                @event.PublisherId,
                sc => sc.Departments,
                mongoDepartment
            );
        }

        private async Task OnPolicyRegistered(DomainEvent @event)
        {
            var context = @event.Context;
            var supplierCompanyId = @event.PublisherId;
            var policyId = context.GetProperty<string>("PolicyId");
            var title = context.GetProperty<string>("Title");
            var coverageAmount = context.GetProperty<int>("CoverageAmount");
            var price = context.GetProperty<decimal>("Price");
            var type = context.GetProperty<string>("Type");
            var issuanceDate = context.GetProperty<DateTime>("IssuanceDate");
            var expirationDate = context.GetProperty<DateTime>("ExpirationDate");

            var mongoPolicy = new MongoPolicy(
                policyId,
                title,
                coverageAmount,
                price,
                type,
                issuanceDate,
                expirationDate
            );

            await MongoHelper.AddToSet(
                _supplierCompanyCollection,
                supplierCompanyId,
                sc => sc.Policies,
                mongoPolicy
            );
        }

        private async Task OnTowDriverRegistered(DomainEvent @event)
        {
            var context = @event.Context;
            var supplierCompanyId = @event.PublisherId;
            var towDriver = context.GetProperty<string>("TowDriverId");

            await MongoHelper.AddToSet(
                _supplierCompanyCollection,
                supplierCompanyId,
                sc => sc.TowDrivers,
                towDriver
            );
        }

        private async Task OnSupplierCompanyDepartmentsUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var supplierCompanyId = @event.PublisherId;
            var departments = context.GetProperty<List<Department>>("Departments");

            var mongoDepartments = departments.Select(department => new MongoDepartment(
                    department.DepartmentId,
                    department.Name,
                    department.Employees
                )
            ).ToList();

            await MongoHelper.Update(
                _supplierCompanyCollection,
                @event.PublisherId,
                supplierCompany => supplierCompany.Departments,
                mongoDepartments
            );
        }

        private async Task OnSupplierCompanyPoliciesUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var supplierCompanyId = @event.PublisherId;
            var policies = context.GetProperty<List<Policy>>("Policies");

            var mongoPolicies = policies.Select(policy => new MongoPolicy(
                    policy.PolicyId,
                    policy.Title,
                    policy.CoverageAmount,
                    policy.Price,
                    policy.Type,
                    policy.IssuanceDate,
                    policy.ExpirationDate
                )
            ).ToList();

            await MongoHelper.Update(
                _supplierCompanyCollection,
                @event.PublisherId,
                supplierCompany => supplierCompany.Policies,
                mongoPolicies
            );
        }

        private async Task OnSupplierCompanyTowDriversUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var towDrivers = context.GetProperty<List<string>>("TowDrivers");

            await MongoHelper.Update(
                _supplierCompanyCollection,
                @event.PublisherId,
                supplierCompany => supplierCompany.TowDrivers,
                towDrivers
            );
        }

        private async Task OnSupplierCompanyNameUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var name = context.GetProperty<string>("Name");

            await MongoHelper.Update(
                _supplierCompanyCollection,
                @event.PublisherId,
                supplierCompany => supplierCompany.Name,
                name
            );
        }

        private async Task OnSupplierCompanyPhoneNumberUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var phoneNumber = context.GetProperty<string>("PhoneNumber");

            await MongoHelper.Update(
                _supplierCompanyCollection,
                @event.PublisherId,
                supplierCompany => supplierCompany.PhoneNumber,
                phoneNumber
            );
        }

        private async Task OnSupplierCompanyTypeUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var type = context.GetProperty<string>("Type");

            await MongoHelper.Update(
                _supplierCompanyCollection,
                @event.PublisherId,
                supplierCompany => supplierCompany.Type,
                type
            );
        }

        private async Task OnSupplierCompanyRifUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var rif = context.GetProperty<string>("Rif");

            await MongoHelper.Update(
                _supplierCompanyCollection,
                @event.PublisherId,
                supplierCompany => supplierCompany.Rif,
                rif
            );
        }

        private async Task OnSupplierCompanyAddressUpdated(DomainEvent @event)
        {
            var context = @event.Context;
            var state = context.GetProperty<string>("State");
            var city = context.GetProperty<string>("City");
            var street = context.GetProperty<string>("Street");

            await MongoHelper.Update(
                _supplierCompanyCollection,
                @event.PublisherId,
                supplierCompany => supplierCompany.State,
                state
            );

            await MongoHelper.Update(
                _supplierCompanyCollection,
                @event.PublisherId,
                supplierCompany => supplierCompany.City,
                city
            );

            await MongoHelper.Update(
                _supplierCompanyCollection,
                @event.PublisherId,
                supplierCompany => supplierCompany.Street,
                street
            );
        }
    }
}
