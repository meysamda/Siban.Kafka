using Confluent.Kafka;

namespace KafkaMessageBus.Abstractions
{
    public interface IKafkaMessageBusSerializer<T> : ISerializer<T>
    {
    }
}
