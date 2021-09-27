using KafkaMessageBus.Abstractions;

namespace Samples.Publisher
{
    class TempMessageProducerOptions : IProducerOptions<string, TempMessage>
    {
        public string Topic { get; set; }
        public IMessageBusSerializer<TempMessage> ValueSerializer { get; set; }
        public IMessageBusSerializer<string> KeySerializer { get; set; }
    }
}
