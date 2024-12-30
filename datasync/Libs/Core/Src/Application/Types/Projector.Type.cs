using RabbitMQ.Contracts;

namespace Datasync.Core
{
    public interface IProjector
    {
        public void Project(EventType @event);
    }
}
