using Confluent.Kafka;

namespace KafkaMessageBus.Abstractions
{
    public interface IMessageBusSerializer<T> : ISerializer<T>
    {
    }
}
