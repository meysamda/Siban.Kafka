using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace KafkaMessageBus.Abstractions
{
    public interface IConsumeResultProcessor<Tkey, TMessage>
    {
        Task Process(ConsumeResult<Tkey, TMessage> consumerResult, IConsumer<Tkey, TMessage> consumer, CancellationToken cancellationToken = default(CancellationToken));
    }
}
