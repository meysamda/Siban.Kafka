﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Siban.Kafka.Abstractions
{    
    public interface ISubscriptionMessageBus
    {
        IEnumerable<string> BootstrapServers { get; }
        
        Task SubscribeForMessageValueAsync<TValue>(
            IEnumerable<string> topics,
            Func<TValue, CancellationToken, Task<bool>> handleMethod,
            Action<ISubscribeOptions<string, TValue>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default);

        Task SubscribeForMessageAsync<TKey, TValue>(
            IEnumerable<string> topics,
            Func<Message<TKey, TValue>, CancellationToken, Task<bool>> handleMethod,
            Action<ISubscribeOptions<TKey, TValue>> defaultOptionsModifier = null,
            CancellationToken cancellationToken = default);

        void Unsubscribe<TKey, TValue>(string consumerName = "default");
    }
}