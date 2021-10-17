using System.Threading;
using System.Threading.Tasks;

namespace KafkaMessageBus.Abstractions
{
    public interface IMessageProcessor<in TMessage>
    {
        Task Process(TMessage message, CancellationToken cancellationToken);
    }
}
