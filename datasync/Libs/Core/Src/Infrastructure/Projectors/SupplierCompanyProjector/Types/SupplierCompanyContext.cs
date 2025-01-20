namespace Datasync.Core
{
    public record TowDriver(string TowDriverId);
    public record Department(string DepartmentId, string Name, List<string> Employees);
    public record Policy(string PolicyId, string Title, int CoverageAmount, decimal Price, string Type, DateOnly IssuanceDate, DateOnly ExpirationDate);
    public record SupplierCompanyContext(
        string SupplierCompanyId,
        List<Department> Departments,
        List<Policy> Policies,
        List<string> TowDrivers,
        string Name,
        string PhoneNumber,
        string Type,
        string Rif,
        string State,
        string City,
        string Street
    );
}
