namespace RabbitMQ.Contracts
{
    public record EventType(
        string PublisherId,
        string Type,
        string Context,
        DateTime OcurredDate
    );
}
