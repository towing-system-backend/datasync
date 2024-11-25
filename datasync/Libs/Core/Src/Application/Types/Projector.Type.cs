using RabbitMQ.Contracts;

namespace Datasync.Core
{
    public interface IProjector
    {
        Task Project(EventType @event);
    }
}
