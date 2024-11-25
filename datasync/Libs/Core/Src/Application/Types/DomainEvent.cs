namespace Datasync.Core
{
    public record DomainEvent(
        string PublisherId,
        string Type,
        Object Context,
        DateTime OcurredDate
    );
}
