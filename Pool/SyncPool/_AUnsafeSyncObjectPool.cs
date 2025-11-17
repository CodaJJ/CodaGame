// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using CodaGame.Base;

namespace CodaGame
{
    /// <summary>
    /// A unsafe synchronous object pool.
    /// </summary>
    /// <remarks>
    /// <para>
    /// "Unsafe" means that the pool will not make sure that the object is managed by this pool when you release it.
    /// So you need to specify the key when you release the object.
    /// </para>
    /// <para>Unsafe pools are faster than safe pools, but you need to be careful when using them.</para>
    /// </remarks>
    /// <typeparam name="T_KEY">The key's type for objects</typeparam>
    /// <typeparam name="T_OBJECT">The object's type</typeparam>
    public abstract class _AUnsafeSyncObjectPool<T_KEY, T_OBJECT> : _AObjectPool<T_KEY, T_OBJECT>
    {
        protected _AUnsafeSyncObjectPool(string _name, int _initialCapacityOfCacheList = 4)
            : base(_name, _initialCapacityOfCacheList)
        {
        }
        protected _AUnsafeSyncObjectPool(int _initialCapacityOfCacheList = 4)
            : this($"UnsafeSyncObjectPool_{Serialize.NextUnsafeSyncObjectPool()}", _initialCapacityOfCacheList)
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
                Console.LogVerbose(SystemNames.ObjectPool, name, $"key-{_key} -- : Get the object from the cache");
                return obj;
            }
            
            obj = LoadObject(_key);
            if (obj == null)
            {
                Console.LogWarning(SystemNames.ObjectPool, name, $"key-{_key} -- : Failed to get the object from the loader.");
                return obj;
            }
                
            Console.LogVerbose(SystemNames.ObjectPool, name, $"key-{_key} -- : Get the object from the loader");
            return obj;
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
                Console.LogWarning(SystemNames.ObjectPool, name, "Failed to release the object because the key is null");
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
        /// </remarks>
        protected abstract T_OBJECT LoadObject(T_KEY _key);
    }
    /// <summary>
    /// A safe synchronous object pool.
    /// </summary>
    /// <remarks>
    /// <para>"Unsafe" means that the pool will not make sure that the object is managed by this pool when you release it.</para>
    /// <para>Unsafe pools are faster than safe pools, but you need to be careful when using them.</para>
    /// </remarks>
    /// <typeparam name="T_OBJECT">The object's type</typeparam>
    public abstract class _AUnsafeSyncObjectPool<T_OBJECT> : _AObjectPool<T_OBJECT>
    {
        protected _AUnsafeSyncObjectPool(string _name, int _initialCapacityOfCacheList = 4)
            : base(_name, _initialCapacityOfCacheList)
        {
        }
        protected _AUnsafeSyncObjectPool(int _initialCapacityOfCacheList = 4)
            : this($"UnsafeSyncObjectPool_{Serialize.NextUnsafeSyncObjectPool()}", _initialCapacityOfCacheList)
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
                Console.LogVerbose(SystemNames.ObjectPool, name, "Get the object from the cache");
                return obj;
            }

            obj = LoadObject();
            if (obj == null)
            {
                Console.LogWarning(SystemNames.ObjectPool, name, "Failed to get the object from the loader.");
                return default;
            }
                
            Console.LogVerbose(SystemNames.ObjectPool, name, "Get the object from the loader");
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
            
            Console.LogVerbose(SystemNames.ObjectPool, name, "Release the object");
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