// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace CodaGame.Base 
{
    /// <summary>
    /// Manages event subscriptions and triggering for an event bus system.
    /// </summary>
    internal class EventBusManager 
    {
        [NotNull] public static EventBusManager instance { get { return _g_instance ??= new EventBusManager(); } }
        private static EventBusManager _g_instance;
        
        
        // Maps event IDs to their corresponding delegates (subscribers).
        [NotNull] private readonly Dictionary<uint, Delegate> _m_subscribers;
        // Lock object for thread-safe operations.
        [NotNull] private readonly object _m_lock;
        
        
        private EventBusManager() 
        {
            _m_subscribers = new Dictionary<uint, Delegate>();
            _m_lock = new object();
        }
        

        /// <summary>
        /// Subscribe to an event with a callback.
        /// </summary>
        public void Subscribe(EventKey _key, Action _callback)
        {
            if (NullCheckAndLog(_key, _callback)) return;
            lock (_m_lock)
                AddSubscriber(_key.id, _callback);
        }
        /// <summary>
        /// Subscribe to an event with one parameter.
        /// </summary>
        public void Subscribe<T1>(EventKey<T1> _key, Action<T1> _callback) 
        {
            if (NullCheckAndLog(_key, _callback)) return;
            lock (_m_lock) 
                AddSubscriber(_key.id, _callback);
        }
        /// <summary>
        /// Subscribe to an event with two parameters.
        /// </summary>
        public void Subscribe<T1, T2>(EventKey<T1, T2> _key, Action<T1, T2> _callback) 
        {
            if (NullCheckAndLog(_key, _callback)) return;
            lock (_m_lock)
                AddSubscriber(_key.id, _callback);
        }
        /// <summary>
        /// Subscribe to an event with three parameters.
        /// </summary>
        public void Subscribe<T1, T2, T3>(EventKey<T1, T2, T3> _key, Action<T1, T2, T3> _callback) 
        {
            if (NullCheckAndLog(_key, _callback)) return;
            lock (_m_lock) 
                AddSubscriber(_key.id, _callback);
        }
        /// <summary>
        /// Subscribe to an event with four parameters.
        /// </summary>
        public void Subscribe<T1, T2, T3, T4>(EventKey<T1, T2, T3, T4> _key, Action<T1, T2, T3, T4> _callback) 
        {
            if (NullCheckAndLog(_key, _callback)) return;
            lock (_m_lock)
                AddSubscriber(_key.id, _callback);
        }
        
        /// <summary>
        /// Unsubscribe from an event with a callback.
        /// </summary>
        public void Unsubscribe(EventKey _key, Action _callback) 
        {
            if (NullCheckAndLog(_key, _callback)) return;
            lock (_m_lock)
                RemoveSubscriber(_key.id, _callback);
        }
        /// <summary>
        /// Unsubscribe from an event with one parameter.
        /// </summary>
        public void Unsubscribe<T1>(EventKey<T1> _key, Action<T1> _callback) 
        {
            if (NullCheckAndLog(_key, _callback)) return;
            lock (_m_lock)
                RemoveSubscriber(_key.id, _callback);
        }
        /// <summary>
        /// Unsubscribe from an event with two parameters.
        /// </summary>
        public void Unsubscribe<T1, T2>(EventKey<T1, T2> _key, Action<T1, T2> _callback) 
        {
            if (NullCheckAndLog(_key, _callback)) return;
            lock (_m_lock)
                RemoveSubscriber(_key.id, _callback);
        }
        /// <summary>
        /// Unsubscribe from an event with three parameters.
        /// </summary>
        public void Unsubscribe<T1, T2, T3>(EventKey<T1, T2, T3> _key, Action<T1, T2, T3> _callback) 
        {
            if (NullCheckAndLog(_key, _callback)) return;
            lock (_m_lock)
                RemoveSubscriber(_key.id, _callback);
        }
        /// <summary>
        /// Unsubscribe from an event with four parameters.
        /// </summary>
        public void Unsubscribe<T1, T2, T3, T4>(EventKey<T1, T2, T3, T4> _key, Action<T1, T2, T3, T4> _callback) 
        {
            if (NullCheckAndLog(_key, _callback)) return;
            lock (_m_lock)
                RemoveSubscriber(_key.id, _callback);
        }

        /// <summary>
        /// Trigger an event without parameters.
        /// </summary>
        public void Trigger(EventKey _key)
        {
            if (NullCheckAndLog(_key)) return;
            
            Delegate callbacks;
            lock (_m_lock)
            {    
                if (!_m_subscribers.TryGetValue(_key.id, out callbacks)) 
                    return;
            }
            
            (callbacks as Action)?.Invoke();
        }
        /// <summary>
        /// Trigger an event with one parameter.
        /// </summary>
        public void Trigger<T1>(EventKey<T1> _key, T1 _arg1) 
        {
            if (NullCheckAndLog(_key)) return;
            
            Delegate callbacks;
            lock (_m_lock) 
            {
                if (!_m_subscribers.TryGetValue(_key.id, out callbacks)) 
                    return;
            }
            
            (callbacks as Action<T1>)?.Invoke(_arg1);
        }
        /// <summary>
        /// Trigger an event with two parameters.
        /// </summary>
        public void Trigger<T1, T2>(EventKey<T1, T2> _key, T1 _arg1, T2 _arg2) 
        {
            if (NullCheckAndLog(_key)) return;
            
            Delegate callbacks;
            lock (_m_lock) 
            {
                if (!_m_subscribers.TryGetValue(_key.id, out callbacks)) 
                    return;
            }
            
            (callbacks as Action<T1, T2>)?.Invoke(_arg1, _arg2);
        }
        /// <summary>
        /// Trigger an event with three parameters.
        /// </summary>
        public void Trigger<T1, T2, T3>(EventKey<T1, T2, T3> _key, T1 _arg1, T2 _arg2, T3 _arg3) 
        {
            if (NullCheckAndLog(_key)) return;
            
            Delegate callbacks;
            lock (_m_lock) 
            {
                if (!_m_subscribers.TryGetValue(_key.id, out callbacks)) 
                    return;
            }
            
            (callbacks as Action<T1, T2, T3>)?.Invoke(_arg1, _arg2, _arg3);
        }
        /// <summary>
        /// Trigger an event with four parameters.
        /// </summary>
        public void Trigger<T1, T2, T3, T4>(EventKey<T1, T2, T3, T4> _key, T1 _arg1, T2 _arg2, T3 _arg3, T4 _arg4) 
        {
            if (NullCheckAndLog(_key)) return;
            
            Delegate callbacks;
            lock (_m_lock) 
            {
                if (!_m_subscribers.TryGetValue(_key.id, out callbacks)) 
                    return;
            }
            
            (callbacks as Action<T1, T2, T3, T4>)?.Invoke(_arg1, _arg2, _arg3, _arg4);
        }
        
        /// <summary>
        /// Clear all subscribers for a specific event key.
        /// </summary>
        /// <param name="_key"></param>
        public void Clear(EventKey _key) 
        {
            if (NullCheckAndLog(_key)) return;
            lock (_m_lock) 
                _m_subscribers.Remove(_key.id);
        }
        /// <summary>
        /// Clear all subscribers for all events.
        /// </summary>
        public void ClearAll() 
        {
            lock (_m_lock) 
                _m_subscribers.Clear();
        }

        // Adds a subscriber delegate for the specified event ID.
        private void AddSubscriber(uint _eventId, Delegate _callback) 
        {
            if (_m_subscribers.TryGetValue(_eventId, out Delegate existing))
                _m_subscribers[_eventId] = Delegate.Combine(existing, _callback);
            else
                _m_subscribers[_eventId] = _callback;
        }
        // Removes a subscriber delegate for the specified event ID.
        private void RemoveSubscriber(uint _eventId, Delegate _callback)
        {
            if (_m_subscribers.TryGetValue(_eventId, out Delegate existing))
            {
                Delegate updated = Delegate.Remove(existing, _callback);
                if (updated == null)
                    _m_subscribers.Remove(_eventId);
                else
                    _m_subscribers[_eventId] = updated;
            }
        }
        // Null checks with logging for debugging.
        [ContractAnnotation("_key:null => true; _callback:null => true")]
        private bool NullCheckAndLog(EventKey _key, Delegate _callback)
        {
            if (_key == null) 
            {
                Console.LogError(SystemNames.EventBus, "EventKey is null.");
                return true;
            }
            if (_callback == null) 
            {
                Console.LogError(SystemNames.EventBus, $"Callback is null for EventKey '{_key}'.");
                return true;
            }
            return false;
        }
        // Null check with logging for debugging.
        [ContractAnnotation("_key:null => true")]
        private bool NullCheckAndLog(EventKey _key)
        {
            if (_key == null) 
            {
                Console.LogError(SystemNames.EventBus, "EventKey is null.");
                return true;
            }
            return false;
        }
    }

}
