using RabbitMQ.Client;

namespace Kunlun.LPS.Worker.MessageQueue.Consumers
{
    public interface IMessageQueueConsumer
    {
        void Register(IModel channel);

        string Name { get; }

    }
}
