namespace datasync.Libs.Core.Src.Infrastructure.Projectors.OrderProjector.Types
{
    public record OrderContext(
        string Status,
        string IssueLocation,
        string Destination,
        string? TowDriverAssigned,
        string Details,
        string Name,
        string Image,
        string PolicyId,
        string PhoneNumber,
        decimal TotalCost,
        List<AdditionalCostContext>? AdditionalCosts 
    );

    public record AdditionalCostContext(
        string Id,
        decimal? Amount,
        string? Name,
        string? Category
    );
}
