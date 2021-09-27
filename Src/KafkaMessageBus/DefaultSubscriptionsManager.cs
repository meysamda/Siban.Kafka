using System;
using System.Collections.Generic;
using System.Linq;
using KafkaMessageBus.Abstractions;

namespace KafkaMessageBus
{
    public partial class DefaultSubscriptionsManager : ISubscriptionsManager
    {
        private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;
        private readonly List<Type> _messageTypes;

        public event EventHandler<string> OnEventRemoved;

        public DefaultSubscriptionsManager()
        {
            _handlers = new Dictionary<string, List<SubscriptionInfo>>();
            _messageTypes = new List<Type>();
        }

        public bool IsEmpty
        {
            
            get
            {
                lock (this)
                {
                    return !_handlers.Keys.Any();
                }
            }
        }

        public void Clear()
        {
            lock (this)
            {
                _handlers.Clear();
            }
        }

        public void AddSubscription<TMessage, TMessageHandler>(string messageName)
            where TMessage : IMessage
        {
            DoAddSubscription(typeof(TMessageHandler), messageName, isDynamic: false);

            lock (this)
            {
                if (!_messageTypes.Contains(typeof(TMessage)))
                {
                    _messageTypes.Add(typeof(TMessage));
                }    
            }
        }

        public void AddSubscription<TMessage, TMessageHandler>()
            where TMessage : IMessage
        {
            var messageName = GetMessageKey<TMessage>();

            DoAddSubscription(typeof(TMessageHandler), messageName, isDynamic: false);

            lock (this)
            {
                if (!_messageTypes.Contains(typeof(TMessage)))
                {
                    _messageTypes.Add(typeof(TMessage));
                }    
            }
        }

        private void DoAddSubscription(Type handlerType, string messageName, bool isDynamic)
        {
            lock (this)
            {
                if (!HasSubscriptionsForMessage(messageName))
                {
                    _handlers.Add(messageName, new List<SubscriptionInfo>());
                }

                if (_handlers[messageName].Any(s => s.HandlerType == handlerType))
                {
                    throw new ArgumentException(
                        $"Handler Type {handlerType.Name} already registered for '{messageName}'", nameof(handlerType));
                }

                if (isDynamic)
                {
                    _handlers[messageName].Add(SubscriptionInfo.Dynamic(handlerType));
                }
                else
                {
                    _handlers[messageName].Add(SubscriptionInfo.Typed(handlerType));
                }
            }
        }

        public void RemoveSubscription<TMessage, TMessageHandler>()
            where TMessage : IMessage
        {
            var handlerToRemove = FindSubscriptionToRemove<TMessage, TMessageHandler>();
            var eventName = GetMessageKey<TMessage>();
            DoRemoveHandler(eventName, handlerToRemove);
        }

        private void DoRemoveHandler(string messageName, SubscriptionInfo subsToRemove)
        {
            lock (this)
            {
                if (subsToRemove != null)
                {
                    _handlers[messageName].Remove(subsToRemove);
                    if (!_handlers[messageName].Any())
                    {
                        _handlers.Remove(messageName);
                        var eventType = _messageTypes.SingleOrDefault(e => e.Name == messageName);
                        if (eventType != null)
                        {
                            _messageTypes.Remove(eventType);
                        }
                        RaiseOnEventRemoved(messageName);
                    }
                }
            }
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForMessage<TMessage>() where TMessage : IMessage
        {
            var key = GetMessageKey<TMessage>();
            return GetHandlersForMessage(key);
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForMessage(string eventName)
        {
            lock (this)
            {
                return _handlers[eventName];    
            }
        }

        private void RaiseOnEventRemoved(string eventName)
        {
            var handler = OnEventRemoved;
            handler?.Invoke(this, eventName);
        }

        private SubscriptionInfo FindSubscriptionToRemove<TMessage, TMessageHandler>()
             where TMessage : IMessage
        {
            var messageName = GetMessageKey<TMessage>();
            return DoFindSubscriptionToRemove(messageName, typeof(TMessageHandler));
        }

        private SubscriptionInfo DoFindSubscriptionToRemove(string messageName, Type handlerType)
        {
            if (!HasSubscriptionsForMessage(messageName))
            {
                return null;
            }

            lock (this)
            {
                return _handlers[messageName].SingleOrDefault(s => s.HandlerType == handlerType);
            }
        }

        public bool HasSubscriptionsForMessage<TMessage>() where TMessage : IMessage
        {
            var key = GetMessageKey<TMessage>();
            return HasSubscriptionsForMessage(key);
        }
        public bool HasSubscriptionsForMessage(string messageName)
        {
            lock (this)
            {
                return _handlers.ContainsKey(messageName);
            }
        }

        public Type GetMessageTypeByName(string messageName)
        {
            lock (this)
            {
                return _messageTypes.SingleOrDefault(t => t.Name == messageName);
            }
        }

        public string GetMessageKey<TMessage>()
        {
            return typeof(TMessage).Name;
        }
    }
}
