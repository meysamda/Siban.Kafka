﻿using System;

namespace KafkaMessageBus.Abstractions
{
    public interface IMessage
    {
        Guid MessageId { get; set; }
    }
}