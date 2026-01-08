using System;
using CodaGame.Base;
using JetBrains.Annotations;

namespace CodaGame
{
    /// <summary>
    /// Event bus for decoupled communication between game systems.
    /// </summary>
    [PublicAPI]
    public static class EventBus 
    {
        [NotNull] private static EventBusManager manager { get { return EventBusManager.instance; } }
        

        /// <summary>
        /// Subscribe to an event with no parameters
        /// </summary>
        public static void Subscribe(EventKey _key, Action _callback) 
        {
            manager.Subscribe(_key, _callback);
        }
        /// <summary>
        /// Subscribe to an event with 1 parameter
        /// </summary>
        public static void Subscribe<T1>(EventKey<T1> _key, Action<T1> _callback) 
        {
            manager.Subscribe(_key, _callback);
        }
        /// <summary>
        /// Subscribe to an event with 2 parameters
        /// </summary>
        public static void Subscribe<T1, T2>(EventKey<T1, T2> _key, Action<T1, T2> _callback)
        {
            manager.Subscribe(_key, _callback);
        }
        /// <summary>
        /// Subscribe to an event with 3 parameters
        /// </summary>
        public static void Subscribe<T1, T2, T3>(EventKey<T1, T2, T3> _key, Action<T1, T2, T3> _callback)
        {
            manager.Subscribe(_key, _callback);
        }
        /// <summary>
        /// Subscribe to an event with 4 parameters
        /// </summary>
        public static void Subscribe<T1, T2, T3, T4>(EventKey<T1, T2, T3, T4> _key, Action<T1, T2, T3, T4> _callback)
        {
            manager.Subscribe(_key, _callback);
        }

        /// <summary>
        /// Unsubscribe from an event with no parameters
        /// </summary>
        public static void Unsubscribe(EventKey _key, Action _callback)
        {
            manager.Unsubscribe(_key, _callback);
        }
        /// <summary>
        /// Unsubscribe from an event with 1 parameter
        /// </summary>
        public static void Unsubscribe<T1>(EventKey<T1> _key, Action<T1> _callback)
        {
            manager.Unsubscribe(_key, _callback);
        }
        /// <summary>
        /// Unsubscribe from an event with 2 parameters
        /// </summary>
        public static void Unsubscribe<T1, T2>(EventKey<T1, T2> _key, Action<T1, T2> _callback)
        {
            manager.Unsubscribe(_key, _callback);
        }
        /// <summary>
        /// Unsubscribe from an event with 3 parameters
        /// </summary>
        public static void Unsubscribe<T1, T2, T3>(EventKey<T1, T2, T3> _key, Action<T1, T2, T3> _callback)
        {
            manager.Unsubscribe(_key, _callback);
        }
        /// <summary>
        /// Unsubscribe from an event with 4 parameters
        /// </summary>
        public static void Unsubscribe<T1, T2, T3, T4>(EventKey<T1, T2, T3, T4> _key, Action<T1, T2, T3, T4> _callback)
        {
            manager.Unsubscribe(_key, _callback);
        }

        /// <summary>
        /// Trigger an event with no parameters
        /// </summary>
        public static void Trigger(EventKey _key)
        {
            manager.Trigger(_key);
        }
        /// <summary>
        /// Trigger an event with 1 parameter
        /// </summary>
        public static void Trigger<T1>(EventKey<T1> _key, T1 _arg1)
        {
            manager.Trigger(_key, _arg1);
        }
        /// <summary>
        /// Trigger an event with 2 parameters
        /// </summary>
        public static void Trigger<T1, T2>(EventKey<T1, T2> _key, T1 _arg1, T2 _arg2)
        {
            manager.Trigger(_key, _arg1, _arg2);
        }
        /// <summary>
        /// Trigger an event with 3 parameters
        /// </summary>
        public static void Trigger<T1, T2, T3>(EventKey<T1, T2, T3> _key, T1 _arg1, T2 _arg2, T3 _arg3)
        {
            manager.Trigger(_key, _arg1, _arg2, _arg3);
        }
        /// <summary>
        /// Trigger an event with 4 parameters
        /// </summary>
        public static void Trigger<T1, T2, T3, T4>(EventKey<T1, T2, T3, T4> _key, T1 _arg1, T2 _arg2, T3 _arg3, T4 _arg4)
        {
            manager.Trigger(_key, _arg1, _arg2, _arg3, _arg4);
        }

        /// <summary>
        /// Clear all subscribers for a specific event
        /// </summary>
        public static void Clear(EventKey _key)
        {
            manager.Clear(_key);
        }
        /// <summary>
        /// Clear all subscribers for all events
        /// </summary>
        public static void ClearAll()
        {
            manager.ClearAll();
        }
    }
}
