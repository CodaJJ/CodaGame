// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using CodaGame.Base;
using JetBrains.Annotations;

namespace CodaGame
{
    /// <summary>
    /// A safe asynchronous object pool.
    /// </summary>
    /// <remarks>
    /// <para>
    /// "Safe" means that the pool will make sure that the object is managed by this pool when you release it.
    /// So you don't need to specify the key when you release the object.
    /// </para>
    /// <para>Also you need to deal with the asynchronous "Get" function.</para>
    /// <para>Safe pools are slower than unsafe pools, but they are more secure.</para>
    /// </remarks>
    /// <typeparam name="T_KEY">The key's type for objects</typeparam>
    /// <typeparam name="T_OBJECT">The object's type</typeparam>
    public abstract class _ASafeAsyncObjectPool<T_KEY, T_OBJECT> : _AObjectPool<T_KEY, T_OBJECT>
    {
        // The dictionary that stores the key of the object.
        [NotNull] private readonly Dictionary<T_OBJECT, T_KEY> _m_handleToKey;
        

        protected _ASafeAsyncObjectPool(string _name, int _initialCapacityOfCacheList = 4)
            : base(_name, _initialCapacityOfCacheList)
        {
            _m_handleToKey = new Dictionary<T_OBJECT, T_KEY>();
        }
        protected _ASafeAsyncObjectPool(int _initialCapacityOfCacheList = 4)
            : this($"SafeAsyncObjectPool_{Serialize.NextSafeAsyncObjectPool()}", _initialCapacityOfCacheList)
        {
        }


        /// <summary>
        /// Get the object by key.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The "_complete" delegate will must be invoked whether the object is loaded successfully or not.
        /// Also, if the loading process stuck, "_complete" delegate will not be invoked. 
        /// </para>
        /// </remarks>
        public void Get(T_KEY _key, Action<T_OBJECT> _complete)
        {
            if (_complete == null)
            {
                Console.LogWarning(SystemNames.ObjectPool, name, "Failed to get the object because the complete action is null");
                return;
            }

            if (_key == null)
            {
                Console.LogWarning(SystemNames.ObjectPool, name, "Failed to get the object because the key is null");
                return;
            }
            
            if (TryGetFromCache(_key, out T_OBJECT obj))
            {
                _m_handleToKey.Add(obj, _key);
                Console.LogVerbose(SystemNames.ObjectPool, name, $"key-{_key} -- Get the object from cache, now the using count is {_m_handleToKey.Count}");
                _complete.Invoke(obj);
                return;
            }
            
            LoadObject(_key, _obj =>
            {
                if (_obj == null)
                {
                    Console.LogWarning(SystemNames.ObjectPool, name, $"key-{_key} --: Failed to get the object from loader.");
                    _complete.Invoke(default);
                    return;
                }
                _m_handleToKey.Add(_obj, _key);
                Console.LogVerbose(SystemNames.ObjectPool, name, $"key-{_key} --: Get the object from the loader, now the using count is {_m_handleToKey.Count}");
                _complete.Invoke(_obj);
            });
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
            Console.LogVerbose(SystemNames.ObjectPool, name, $"key-{key} --: Release the object, now the using count is {_m_handleToKey.Count}");
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
        /// <para>When the object is loaded, call the '_complete' function to notify the pool system that the object is ready.</para>
        /// </remarks>
        protected abstract void LoadObject(T_KEY _key, Action<T_OBJECT> _complete);
    }
    /// <summary>
    /// A safe asynchronous object pool.
    /// </summary>
    /// <remarks>
    /// <para>"Safe" means that the pool will make sure that the object is managed by this pool when you release it.</para>
    /// <para>Also you need to deal with the asynchronous "Get" function.</para>
    /// <para>Safe pools are slower than unsafe pools, but they are more secure.</para>
    /// </remarks>
    /// <typeparam name="T_OBJECT">The object's type</typeparam>
    public abstract class _ASafeAsyncObjectPool<T_OBJECT> : _AObjectPool<T_OBJECT>
    {
        // The set that stores the using object.
        [NotNull] private readonly HashSet<T_OBJECT> _m_objects;


        protected _ASafeAsyncObjectPool(string _name, int _initialCapacityOfCacheList = 4)
            : base(_name, _initialCapacityOfCacheList)
        {
            _m_objects = new HashSet<T_OBJECT>();
        }
        protected _ASafeAsyncObjectPool(int _initialCapacityOfCacheList = 4)
            : this($"SafeAsyncObjectPool_{Serialize.NextSafeAsyncObjectPool()}", _initialCapacityOfCacheList)
        {
        }
        
        
        /// <summary>
        /// Get the object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The "_complete" delegate will must be invoked whether the object is loaded successfully or not.
        /// But if the loading process stuck, "_complete" delegate will not be invoked.
        /// </para>
        /// </remarks>
        public void Get(Action<T_OBJECT> _complete)
        {
            if (_complete == null)
            {
                Console.LogWarning(SystemNames.ObjectPool, name, "Failed to get the object because the complete action is null");
                return;
            }
            
            if (TryGetFromCache(out T_OBJECT obj))
            {
                _m_objects.Add(obj);
                Console.LogVerbose(SystemNames.ObjectPool, name, $"Get the object from the cache, now the using count is {_m_objects.Count}");
                _complete.Invoke(obj);
                return;
            }
            
            LoadObject(_obj =>
            {
                if (_obj == null)
                {
                    Console.LogWarning(SystemNames.ObjectPool, name, "Failed to get the object from the loader.");
                    _complete.Invoke(default);
                    return;
                }
                _m_objects.Add(_obj);
                Console.LogVerbose(SystemNames.ObjectPool, name, $"Get the object from the loader, now the using count is {_m_objects.Count}");
                _complete.Invoke(_obj);
            });
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
        /// <para>When the object is loaded, call the '_complete' function to notify the pool system that the object is ready.</para>
        /// </remarks>
        protected abstract void LoadObject(Action<T_OBJECT> _complete);
    }
}