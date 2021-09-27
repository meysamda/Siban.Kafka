using System;
using KafkaMessageBus.Abstractions;

namespace Samples.Publisher
{
    class TempMessage : IMessage
    {
        public string Body { get; set; }
        public int Number { get; set; }
        
        public Guid MessageId { get; set; }
        public DateTime? MessageExpireDate { get; set; }
    }
}
