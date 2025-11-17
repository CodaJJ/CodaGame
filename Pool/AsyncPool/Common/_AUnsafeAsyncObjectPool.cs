// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using CodaGame.Base;

namespace CodaGame
{
    /// <summary>
    /// A unsafe asynchronous object pool.
    /// </summary>
    /// <remarks>
    /// <para>
    /// "Unsafe" means that the pool will not make sure that the object is managed by this pool when you release it.
    /// So you need to specify the key when you release the object.
    /// </para>
    /// <para>Also you need to deal with the asynchronous "Get" function.</para>
    /// <para>Unsafe pools are faster than safe pools, but you need to be careful when using them.</para>
    /// </remarks>
    /// <typeparam name="T_KEY">The key's type for objects</typeparam>
    /// <typeparam name="T_OBJECT">The object's type</typeparam>
    public abstract class _AUnsafeAsyncObjectPool<T_KEY, T_OBJECT> : _AObjectPool<T_KEY, T_OBJECT>
    {
        protected _AUnsafeAsyncObjectPool(string _name, int _initialCapacityOfCacheList = 4)
            : base(_name, _initialCapacityOfCacheList)
        {
        }
        protected _AUnsafeAsyncObjectPool(int _initialCapacityOfCacheList = 4)
            : this($"UnsafeAsyncObjectPool_{Serialize.NextUnsafeAsyncObjectPool()}", _initialCapacityOfCacheList)
        {
        }


        /// <summary>
        /// Get the object by key.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The "_complete" delegate will must be invoked whether the object is loaded successfully or not.
        /// Also if the loading process stuck, "_complete" delegate will not be invoked. 
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
                Console.LogVerbose(SystemNames.ObjectPool, name, $"key-{_key} -- : Get the object from the cache");
                _complete.Invoke(obj);
                return;
            }
            
            LoadObject(_key, _obj =>
            {
                if (_obj == null)
                {
                    Console.LogWarning(SystemNames.ObjectPool, name, $"key-{_key} -- : Failed to get the object from the loader.");
                    _complete.Invoke(default);
                    return;
                }
                
                Console.LogVerbose(SystemNames.ObjectPool, name, $"key-{_key} -- : Get the object from the loader");
                _complete.Invoke(_obj);
            });
        }
        /// <summary>
        /// Release the object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// You need to specify the key and make sure that the key is correct.
        /// Otherwise, the object will store in the cache with the wrong key.
        /// </para>
        /// </remarks>
        public void Release(T_KEY _key, T_OBJECT _obj)
        {
            if (_obj == null)
            {
                Console.LogWarning(SystemNames.ObjectPool, name, "Failed to release the object because the object is null");
                return;
            }
            
            if (_key == null)
            {
                Console.LogWarning(SystemNames.ObjectPool, name, "Failed to release the object in because the key is null");
                return;
            }
            
            Console.LogVerbose(SystemNames.ObjectPool, name, $"key-{_key} -- : Release the object");
            PushBackToCache(_key, _obj);
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
    /// <para>"Unsafe" means that the pool will not make sure that the object is managed by this pool when you release it.</para>
    /// <para>Also you need to deal with the asynchronous "Get" function.</para>
    /// <para>Unsafe pools are faster than safe pools, but you need to be careful when using them.</para>
    /// </remarks>
    /// <typeparam name="T_OBJECT">The object's type</typeparam>
    public abstract class _AUnsafeAsyncObjectPool<T_OBJECT> : _AObjectPool<T_OBJECT>
    {
        protected _AUnsafeAsyncObjectPool(string _name, int _initialCapacityOfCacheList = 4)
            : base(_name, _initialCapacityOfCacheList)
        {
        }
        protected _AUnsafeAsyncObjectPool(int _initialCapacityOfCacheList = 4)
            : this($"UnsafeAsyncObjectPool_{Serialize.NextUnsafeAsyncObjectPool()}", _initialCapacityOfCacheList)
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
                Console.LogVerbose(SystemNames.ObjectPool, name, "Get the object from the cache");
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
                
                Console.LogVerbose(SystemNames.ObjectPool, name, "Get the object from the loader");
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
            
            Console.LogVerbose(SystemNames.ObjectPool, name, "Release the object");
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