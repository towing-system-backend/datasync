namespace Datasync.Core
{
    public record TowContext(
        string Brand,
        string Model,
        string Color,
        string LicensePlate,
        string Location,
        int Year,
        string SizeType,
        string Status
    );
}