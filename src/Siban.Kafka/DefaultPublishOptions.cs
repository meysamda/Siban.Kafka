﻿using System;
using Confluent.Kafka;
using Siban.Kafka.Abstractions;

namespace Siban.Kafka
{
    public class DefaultPublishOptions<TKey, TMessage> : IPublishOptions<TKey, TMessage>
    {
        public ISerializer<TMessage> ValueSerializer { get; set; }
        public ISerializer<TKey> KeySerializer { get; set; }
        public ProducerConfig ProducerConfig { get; set; }
        public Action<Error> ErrorHandler { get; set; }
        public Action<LogMessage> LogHandler { get; set; }
        public string ProducerName { get; set; }
    }
}