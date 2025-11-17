// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System.Collections.Generic;
using CodaGame.Base;
using JetBrains.Annotations;

namespace CodaGame
{
    /// <summary>
    /// A safe synchronous object pool.
    /// </summary>
    /// <remarks>
    /// <para>
    /// ”Safe“ means that the pool will make sure that the object is managed by this pool when you release it.
    /// So you don't need to specify the key when you release the object.
    /// </para>
    /// <para>Safe pools are slower than unsafe pools, but they are more secure.</para>
    /// </remarks>
    /// <typeparam name="T_KEY">The key's type for objects</typeparam>
    /// <typeparam name="T_OBJECT">The object's type</typeparam>
    public abstract class _ASafeSyncObjectPool<T_KEY, T_OBJECT> : _AObjectPool<T_KEY, T_OBJECT>
    {
        // The dictionary that stores the key of the object.
        [NotNull] private readonly Dictionary<T_OBJECT, T_KEY> _m_handleToKey;
        

        protected _ASafeSyncObjectPool(string _name, int _initialCapacityOfCacheList = 4)
            : base(_name, _initialCapacityOfCacheList)
        {
            _m_handleToKey = new Dictionary<T_OBJECT, T_KEY>();
        }
        protected _ASafeSyncObjectPool(int _initialCapacityOfCacheList = 4)
            : this($"SafeSyncObjectPool_{Serialize.NextSafeSyncObjectPool()}", _initialCapacityOfCacheList)
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
                Console.LogWarning(SystemNames.ObjectPool, name, "Failed to get the object because the key is null");
                return default;
            }
            
            if (TryGetFromCache(_key, out T_OBJECT obj))
            {
                _m_handleToKey.Add(obj, _key);
                Console.LogVerbose(SystemNames.ObjectPool, name, $"key-{_key} -- : Get the object from the cache, now the using count is {_m_handleToKey.Count}");
                return obj;
            }
            
            obj = LoadObject(_key);
            if (obj == null)
            {
                Console.LogWarning(SystemNames.ObjectPool, name, $"key-{_key} -- : Failed to get the object from the loader.");
                return default;
            }
            
            _m_handleToKey.Add(obj, _key);
            Console.LogVerbose(SystemNames.ObjectPool, name, $"key-{_key} -- : Get the object from the loader, now the using count is {_m_handleToKey.Count}");
            return obj;
        }
        /// <summary>
        /// Release the object.
        /// </summary>
        /// <remarks>
        /// <para>This function will check whether the object is managed by the pool.</para>
        /// </remarks>
        public void Release(T_OBJECT _obj)
        {
            if (_obj == null)
            {
                Console.LogWarning(SystemNames.ObjectPool, name, "Failed to release the object because the object is null");
                return;
            }
            
            if (!_m_handleToKey.TryGetValue(_obj, out T_KEY key))
            {
                Console.LogWarning(SystemNames.ObjectPool, name, "Failed to release the object because the object is not managed by the pool.");
                return;
            }
            
            _m_handleToKey.Remove(_obj);
            Console.LogVerbose(SystemNames.ObjectPool, name, $"key-{key} -- : Release the object, now the using count is {_m_handleToKey.Count}");
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
                Console.LogWarning(SystemNames.ObjectPool, name, "Failed to get the key because the object is null");
                return false;
            }
            
            if (!_m_handleToKey.TryGetValue(_obj, out _key))
            {
                Console.LogWarning(SystemNames.ObjectPool, name, "Failed to get the key because the object is not managed by the pool.");
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
    /// A safe synchronous object pool.
    /// </summary>
    /// <remarks>
    /// <para>"Safe“ means that the pool will make sure that the object is managed by this pool when you release it.</para>
    /// <para>Safe pools are slower than unsafe pools, but they are more secure.</para>
    /// </remarks>
    /// <typeparam name="T_OBJECT">The object's type</typeparam>
    public abstract class _ASafeSyncObjectPool<T_OBJECT> : _AObjectPool<T_OBJECT>
    {
        // The set that stores the using object.
        [NotNull] private readonly HashSet<T_OBJECT> _m_objects;


        protected _ASafeSyncObjectPool(string _name, int _initialCapacityOfCacheList = 4)
            : base(_name, _initialCapacityOfCacheList)
        {
            _m_objects = new HashSet<T_OBJECT>();
        }
        protected _ASafeSyncObjectPool(int _initialCapacityOfCacheList)
            : this($"SafeSyncObjectPool_{Serialize.NextSafeSyncObjectPool()}", _initialCapacityOfCacheList)
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
                Console.LogVerbose(SystemNames.ObjectPool, name, $"Get the object from the cache, now the using count is {_m_objects.Count}");
                return obj;
            }

            obj = LoadObject();
            if (obj == null)
            {
                Console.LogWarning(SystemNames.ObjectPool, name, "Failed to get the object from the loader.");
                return default;
            }

            _m_objects.Add(obj);
            Console.LogVerbose(SystemNames.ObjectPool, name, $"Get the object from the loader, now the using count is {_m_objects.Count}");
            return obj;
        }
        /// <summary>
        /// Release the object.
        /// </summary>
        /// <remarks>
        /// <para>This function will check whether the object is managed by the pool.</para>
        /// </remarks>
        public void Release(T_OBJECT _obj)
        {
            if (_obj == null)
            {
                Console.LogWarning(SystemNames.ObjectPool, name, "Failed to release the object because the object is null");
                return;
            }
            
            if (!_m_objects.Contains(_obj))
            {
                Console.LogWarning(SystemNames.ObjectPool, name, "Failed to release the object because the object is not managed by the pool.");
                return;
            }
            
            _m_objects.Remove(_obj);
            Console.LogVerbose(SystemNames.ObjectPool, name, $"Release the object, now the using count is {_m_objects.Count}");
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