using RabbitMQ.Contracts;
using Datasync.Core;
using MassTransit;

namespace DataSync.Infrastructure
{
    public class DatasyncController(IEnumerable<IProjector> projectors) : IConsumer<EventType>
    {
        private readonly IEnumerable<IProjector> _projectors = projectors;

        public Task Consume(ConsumeContext<EventType> context)
        {
            foreach (var projector in _projectors)
            {
                projector.Project(context.Message);
            }

            return Task.CompletedTask;
        }
    }
}
