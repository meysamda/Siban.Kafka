using System;
using KafkaMessageBus.Abstractions;

namespace Samples.Messages
{
    public class TempMessage : IMessage
    {
        public string Body { get; set; }
        public int Number { get; set; }
        
        public Guid MessageId { get; set; }
    }
}
