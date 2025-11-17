// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace CodaGame.Base
{
    /// <summary>
    /// A base class that provides some basic functions for the pool system.
    /// </summary>
    /// <remarks>
    /// <para>The pool system is a cache system that can get objects easily and efficiently.</para>
    /// </remarks>
    /// <typeparam name="T_KEY">The type of key used to index the objects.</typeparam>
    /// <typeparam name="T_OBJECT">The type of object that is handled by pool system.</typeparam>
    public abstract class _AObjectPool<T_KEY, T_OBJECT>
    {
        // The name of the pool.
        private readonly string _m_name;
        // The capacity of the cache list, specially used for occupied pool and shared pool.
        private readonly int _m_cacheListCapacity;
        // The dictionary that maps the key to the cache list.
        [NotNull] private readonly Dictionary<T_KEY, List<T_OBJECT>> _m_keyToCacheList;
        

        internal _AObjectPool(string _name, int _capacity = 4)
        {
            _m_name = _name;
            _m_cacheListCapacity = _capacity;
            
            _m_keyToCacheList = new Dictionary<T_KEY, List<T_OBJECT>>();
        }


        /// <summary>
        /// The name of the pool, usually used for debugging.
        /// </summary>
        public string name { get { return _m_name; } }
        
        
        /// <summary>
        /// Release the cache.
        /// </summary>
        /// <remarks>
        /// <para>Caches that are currently in use will not be released.</para>
        /// <para>Also, you can use the predicate to specify the condition to release the cache.</para>
        /// </remarks>
        /// <param name="_predicate">The predicate to specify the condition to release the cache, return true will destroy the cached object.</param>
        public void ClearCache(Predicate<T_OBJECT> _predicate = null)
        {
            if (_predicate == null)
            {
                Console.LogSystem(SystemNames.ObjectPool, _m_name, "The caches that have not been used will be deleted.");
                foreach (List<T_OBJECT> cacheList in _m_keyToCacheList.Values)
                {
                    foreach (T_OBJECT obj in cacheList)
                    {
                        DestroyObject(obj);
                        Console.LogVerbose(SystemNames.ObjectPool, _m_name, $"The object({obj}) is destroyed in the cache list.");
                    }
                }
                
                _m_keyToCacheList.Clear();
            }
            else
            {
                Console.LogSystem(SystemNames.ObjectPool, _m_name, "The caches that meet the condition will be deleted.");
                
                List<T_KEY> keysEmpty = new List<T_KEY>();
                foreach (KeyValuePair<T_KEY, List<T_OBJECT>> keyValuePair in _m_keyToCacheList)
                {
                    List<T_OBJECT> cacheList = keyValuePair.Value;
                    for (int i = cacheList.Count - 1; i >= 0; i--)
                    {
                        T_OBJECT obj = cacheList[i];
                        if (_predicate(obj))
                        {
                            cacheList.RemoveAt(i);
                            DestroyObject(obj);
                            Console.LogVerbose(SystemNames.ObjectPool, _m_name, $"The object({obj}) is destroyed in the cache list.");
                        }
                    }

                    Console.LogVerbose(SystemNames.ObjectPool, _m_name, $"Now the key({keyValuePair.Key})'s cache list has {cacheList.Count} objects.");
                    if (cacheList.Count == 0)
                        keysEmpty.Add(keyValuePair.Key);
                }

                foreach (T_KEY key in keysEmpty)
                    _m_keyToCacheList.Remove(key);
            }
        }

        
        /// <summary>
        /// Destroy the object.
        /// </summary>
        /// <remarks>
        /// <para>Implement this function to customize the destruction process of the object when it is no longer in use.</para>
        /// </remarks>
        protected abstract void DestroyObject(T_OBJECT _object);
        /// <summary>
        /// Reset the object.
        /// </summary>
        /// <remarks>
        /// <para>Implement this function to customize the reset process of the object when it is returned to the cache and awaits further use.</para>
        /// </remarks>
        protected abstract void ResetObject(T_OBJECT _object);
        
        
        /// <summary>
        /// Try to get the object from the cache.
        /// </summary>
        /// <remarks>
        /// <para>The object will be removed from the cache list if it is successfully retrieved.</para>
        /// </remarks>
        private protected bool TryGetFromCache(T_KEY _key, out T_OBJECT _object)
        {
            _object = default;

            if (_key == null)
            {
                Console.LogWarning(SystemNames.ObjectPool, _m_name, "The key is null, when trying to get object from cache.");
                return false;
            }

            if (!_m_keyToCacheList.TryGetValue(_key, out List<T_OBJECT> objectList))
            {
                Console.LogVerbose(SystemNames.ObjectPool, _m_name, $"The key({_key}) is not found in the cache list.");
                return false;
            }

            if (objectList.Count == 0)
            {
                Console.LogVerbose(SystemNames.ObjectPool, _m_name, $"The key({_key})'s cache list is empty.");
                return false;
            }
            
            _object = objectList.GetLastAndRemove();
            if (objectList.Count == 0)
                _m_keyToCacheList.Remove(_key);
            
            Console.LogVerbose(SystemNames.ObjectPool, _m_name, $"The object({_object}) is retrieved from the cache list, now the key({_key})'s cache list has {objectList.Count} objects.");
            return true;
        }
        /// <summary>
        /// Push the object back to the cache.
        /// </summary>
        private protected void PushBackToCache(T_KEY _key, T_OBJECT _object)
        {
            if (_object == null)
            {
                Console.LogWarning(SystemNames.ObjectPool, _m_name, "The object is null, when trying to push back to cache.");
                return;
            }

            if (_key == null)
            {
                DestroyObject(_object);
                Console.LogWarning(SystemNames.ObjectPool, _m_name, $"The key is null, when trying to push back to cache, so the object({_object}) is destroyed.");
                return;
            }

            ResetObject(_object);
            if (!_m_keyToCacheList.TryGetValue(_key, out List<T_OBJECT> objectList))
            {
                objectList = new List<T_OBJECT>(_m_cacheListCapacity);
                _m_keyToCacheList.Add(_key, objectList);
            }
            
            objectList.Add(_object);
            Console.LogVerbose(SystemNames.ObjectPool, _m_name, $"The object({_object}) is pushed back to the cache list, now the key({_key})'s cache list has {objectList.Count} objects.");
        }
    }
    /// <summary>
    /// A base class that provides some basic functions for the pool system.
    /// </summary>
    /// <remarks>
    /// <para>The pool system is a cache system that can get objects easily and efficiently.</para>
    /// </remarks>
    /// <typeparam name="T_OBJECT">The type of object that is handled by pool system.</typeparam>
    public abstract class _AObjectPool<T_OBJECT>
    {
        // The name of the pool.
        private readonly string _m_name;
        // The list that stores the objects that are returned to the cache.
        [NotNull] private readonly List<T_OBJECT> _m_cacheList;
        

        internal _AObjectPool(string _name, int _capacity = 4)
        {
            _m_name = _name;
            
            _m_cacheList = new List<T_OBJECT>(_capacity);
        }


        /// <summary>
        /// The name of the pool, usually used for debugging.
        /// </summary>
        public string name { get { return _m_name; } }


        /// <summary>
        /// Release the cache.
        /// </summary>
        /// <remarks>
        /// <para>Caches that are currently in use will not be released.</para>
        /// <para>Also, you can use the predicate to specify the condition to release the cache.</para>
        /// </remarks>
        /// <param name="_predicate">The predicate to specify the condition to release the cache, return true will destroy the cached object.</param>
        public void ClearCache(Predicate<T_OBJECT> _predicate)
        {
            if (_predicate == null)
            {
                Console.LogSystem(SystemNames.ObjectPool, _m_name, "The caches that have not been used will be deleted");
                foreach (T_OBJECT obj in _m_cacheList)
                {
                    DestroyObject(obj);
                    Console.LogVerbose(SystemNames.ObjectPool, _m_name, $"The object({obj}) is destroyed in the cache list.");
                }

                _m_cacheList.Clear();
            }
            else
            {
                Console.LogSystem(SystemNames.ObjectPool, _m_name, "The caches that meet the condition will be deleted");
                for (int i = _m_cacheList.Count - 1; i >= 0; i--)
                {
                    T_OBJECT obj = _m_cacheList[i];
                    if (_predicate(obj))
                    {
                        _m_cacheList.RemoveAt(i);
                        DestroyObject(obj);
                        Console.LogVerbose(SystemNames.ObjectPool, _m_name, $"The object({obj}) is destroyed in the cache list.");
                    }
                }

                Console.LogVerbose(SystemNames.ObjectPool, _m_name, $"Now The cache list has {_m_cacheList.Count} objects.");
            }
        }

        
        /// <summary>
        /// Destroy the object.
        /// </summary>
        /// <remarks>
        /// <para>Implement this function to customize the destruction process of the object when it is no longer in use.</para>
        /// </remarks>
        protected abstract void DestroyObject(T_OBJECT _object);
        /// <summary>
        /// Reset the object.
        /// </summary>
        /// <remarks>
        /// <para>Implement this function to customize the reset process of the object when it is returned to the cache and awaits further use.</para>
        /// </remarks>
        protected abstract void ResetObject(T_OBJECT _object);
        
        
        /// <summary>
        /// Try to get the object from the cache.
        /// </summary>
        /// <remarks>
        /// <para>The object will be removed from the cache list if it is successfully retrieved.</para>
        /// </remarks>
        private protected bool TryGetFromCache(out T_OBJECT _object)
        {
            _object = default;

            if (_m_cacheList.Count == 0)
            {
                Console.LogVerbose(SystemNames.ObjectPool, _m_name, "The cache list is empty.");
                return false;
            }
            
            _object = _m_cacheList.GetLastAndRemove();
            Console.LogVerbose(SystemNames.ObjectPool, _m_name, $"The object({_object}) is retrieved from the cache list, now the cache list has {_m_cacheList.Count} objects.");
            return true;
        }
        /// <summary>
        /// Push the object back to the cache.
        /// </summary>
        private protected void PushBackToCache(T_OBJECT _object)
        {
            if (_object == null)
            {
                Console.LogWarning(SystemNames.ObjectPool, _m_name, "The object is null, when trying to push back to cache.");
                return;
            }
            
            ResetObject(_object);
            _m_cacheList.Add(_object);
            Console.LogVerbose(SystemNames.ObjectPool, _m_name, $"The object({_object}) is pushed back to the cache list, now the cache list has {_m_cacheList.Count} objects.");
        }
    }
}