namespace Datasync.Core
{
    public record OrderContext(
        string Status,
        string IssueLocation,
        string Destination,
        string Issuer,
        string? TowDriverAssigned,
        string Details,
        string Name,
        string Image,
        string PolicyId,
        string PhoneNumber,
        int IdentificationNumber,
        decimal TotalCost,
        double TotalDistance,
        List<AdditionalCostContext>? AdditionalCosts 
    );

    public record AdditionalCostContext(
        string Id,
        decimal? Amount,
        string? Name,
        string? Category
    );
}
