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
            _projectors.ToList().ForEach(async projector => await projector.Project(context.Message));

            return Task.CompletedTask;
        }
    }
}
