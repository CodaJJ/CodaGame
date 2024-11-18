
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityGameFramework.Base;

namespace UnityGameFramework
{
    /// <summary>
    /// A safe synchronous object getter.
    /// </summary>
    /// <remarks>
    /// <para>
    /// ”Safe“ means that the getter will make sure that the object is managed by this getter when you release it.
    /// So you don't need to specify the key when you release the object.
    /// </para>
    /// <para>Safe getters are slower than unsafe getters, but they are more secure.</para>
    /// </remarks>
    /// <typeparam name="T_KEY">The key's type for objects</typeparam>
    /// <typeparam name="T_OBJECT">The object's type</typeparam>
    public abstract class _ASafeSyncObjectGetter<T_KEY, T_OBJECT> : _AObjectGetter<T_KEY, T_OBJECT>
    {
        // The dictionary that stores the key of the object.
        [NotNull] private readonly Dictionary<T_OBJECT, T_KEY> _m_handleToKey;
        

        protected _ASafeSyncObjectGetter(string _name, int _initialCapacityOfCacheList = 4)
            : base(_name, _initialCapacityOfCacheList)
        {
            _m_handleToKey = new Dictionary<T_OBJECT, T_KEY>();
        }
        protected _ASafeSyncObjectGetter(int _initialCapacityOfCacheList = 4)
            : this($"AnonymousSafeSyncObjectGetter_{Serialize.NextSafeSyncObjectGetter()}", _initialCapacityOfCacheList)
        {
        }


        /// <summary>
        /// Get the object by key.
        /// </summary>
        /// <remarks>
        /// <para>It will return the object directly if the object is in the cache, or will load the object by a synchronous way.</para>
        /// </remarks>
        public T_OBJECT Get(T_KEY _key)
        {
            if (_key == null)
            {
                Console.LogWarning(SystemNames.ObjectGetter, $"-- {name} -- : Failed to get the object because the key is null");
                return default;
            }
            
            if (TryGetFromCache(_key, out T_OBJECT obj))
            {
                _m_handleToKey.Add(obj, _key);
                Console.LogVerbose(SystemNames.ObjectGetter, $"-- {name} -- key-{_key} -- : Get the object from the cache, now the using count is {_m_handleToKey.Count}");
                return obj;
            }
            
            obj = LoadObject(_key);
            if (obj == null)
            {
                Console.LogWarning(SystemNames.ObjectGetter, $"-- {name} -- key-{_key} -- : Failed to get the object from the loader.");
                return default;
            }
            
            _m_handleToKey.Add(obj, _key);
            Console.LogVerbose(SystemNames.ObjectGetter, $"-- {name} -- key-{_key} -- : Get the object from the loader, now the using count is {_m_handleToKey.Count}");
            return obj;
        }
        /// <summary>
        /// Release the object.
        /// </summary>
        /// <remarks>
        /// <para>This function will check whether the object is managed by the getter.</para>
        /// </remarks>
        public void Release(T_OBJECT _obj)
        {
            if (_obj == null)
            {
                Console.LogWarning(SystemNames.ObjectGetter, $"-- {name} -- : Failed to release the object because the object is null");
                return;
            }
            
            if (!_m_handleToKey.TryGetValue(_obj, out T_KEY key))
            {
                Console.LogWarning(SystemNames.ObjectGetter, $"-- {name} -- : Failed to release the object because the object is not managed by the getter.");
                return;
            }
            
            _m_handleToKey.Remove(_obj);
            Console.LogVerbose(SystemNames.ObjectGetter, $"-- {name} -- key-{key} -- : Release the object, now the using count is {_m_handleToKey.Count}");
            PushBackToCache(key, _obj);
        }
        /// <summary>
        /// Try to get the key of the object.
        /// </summary>
        public bool TryGetTheKey(T_OBJECT _obj, out T_KEY _key)
        {
            _key = default;
            if (_obj == null)
            {
                Console.LogWarning(SystemNames.ObjectGetter, $"-- {name} -- : Failed to get the key because the object is null");
                return false;
            }
            
            if (!_m_handleToKey.TryGetValue(_obj, out _key))
            {
                Console.LogWarning(SystemNames.ObjectGetter, $"-- {name} -- : Failed to get the key because the object is not managed by the getter.");
                return false;
            }

            return true;
        }
        
        
        /// <summary>
        /// Load the object by key.
        /// </summary>
        /// <remarks>
        /// <para>Implement this function to customize the loading process of the object which is a new one you want.</para>
        /// </remarks>
        protected abstract T_OBJECT LoadObject(T_KEY _key);
    }
    /// <summary>
    /// A safe synchronous object getter.
    /// </summary>
    /// <remarks>
    /// <para>"Safe“ means that the getter will make sure that the object is managed by this getter when you release it.</para>
    /// <para>Safe getters are slower than unsafe getters, but they are more secure.</para>
    /// </remarks>
    /// <typeparam name="T_OBJECT">The object's type</typeparam>
    public abstract class _ASafeSyncObjectGetter<T_OBJECT> : _AObjectGetter<T_OBJECT>
    {
        // The set that stores the using object.
        [NotNull] private readonly HashSet<T_OBJECT> _m_objects;


        protected _ASafeSyncObjectGetter(string _name, int _initialCapacityOfCacheList = 4)
            : base(_name, _initialCapacityOfCacheList)
        {
            _m_objects = new HashSet<T_OBJECT>();
        }
        protected _ASafeSyncObjectGetter(int _initialCapacityOfCacheList)
            : this($"AnonymousSafeSyncObjectGetter_{Serialize.NextSafeSyncObjectGetter()}", _initialCapacityOfCacheList)
        {
        }


        /// <summary>
        /// Get the object.
        /// </summary>
        /// <remarks>
        /// <para>It will return the object directly if the object is in the cache, or will load the object by a synchronous way.</para>
        /// </remarks>
        public T_OBJECT Get()
        {
            if (TryGetFromCache(out T_OBJECT obj))
            {
                _m_objects.Add(obj);
                Console.LogVerbose(SystemNames.ObjectGetter, $"-- {name} -- : Get the object from the cache, now the using count is {_m_objects.Count}");
                return obj;
            }

            obj = LoadObject();
            if (obj == null)
            {
                Console.LogWarning(SystemNames.ObjectGetter, $"-- {name} -- : Failed to get the object from the loader.");
                return default;
            }

            _m_objects.Add(obj);
            Console.LogVerbose(SystemNames.ObjectGetter, $"-- {name} -- : Get the object from the loader, now the using count is {_m_objects.Count}");
            return obj;
        }
        /// <summary>
        /// Release the object.
        /// </summary>
        /// <remarks>
        /// <para>This function will check whether the object is managed by the getter.</para>
        /// </remarks>
        public void Release(T_OBJECT _obj)
        {
            if (_obj == null)
            {
                Console.LogWarning(SystemNames.ObjectGetter, $"-- {name} -- : Failed to release the object because the object is null");
                return;
            }
            
            if (!_m_objects.Contains(_obj))
            {
                Console.LogWarning(SystemNames.ObjectGetter, $"-- {name} -- : Failed to release the object because the object is not managed by the getter.");
                return;
            }
            
            _m_objects.Remove(_obj);
            Console.LogVerbose(SystemNames.ObjectGetter, $"-- {name} -- : Release the object, now the using count is {_m_objects.Count}");
            PushBackToCache(_obj);
        }
        
        
        /// <summary>
        /// Load the object.
        /// </summary>
        /// <remarks>
        /// <para>Implement this function to customize the loading process of the object which is a new one you want.</para>
        /// </remarks>
        protected abstract T_OBJECT LoadObject();
    }
}