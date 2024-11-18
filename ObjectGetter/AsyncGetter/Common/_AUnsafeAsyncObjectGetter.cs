
using System;
using UnityGameFramework.Base;

namespace UnityGameFramework
{
    /// <summary>
    /// A unsafe asynchronous object getter.
    /// </summary>
    /// <remarks>
    /// <para>
    /// "Unsafe" means that the getter will not make sure that the object is managed by this getter when you release it.
    /// So you need to specify the key when you release the object.
    /// </para>
    /// <para>Also you need to deal with the asynchronous "Get" function.</para>
    /// <para>Unsafe getters are faster than safe getters, but you need to be careful when using them.</para>
    /// </remarks>
    /// <typeparam name="T_KEY">The key's type for objects</typeparam>
    /// <typeparam name="T_OBJECT">The object's type</typeparam>
    public abstract class _AUnsafeAsyncObjectGetter<T_KEY, T_OBJECT> : _AObjectGetter<T_KEY, T_OBJECT>
    {
        protected _AUnsafeAsyncObjectGetter(string _name, int _initialCapacityOfCacheList = 4)
            : base(_name, _initialCapacityOfCacheList)
        {
        }
        protected _AUnsafeAsyncObjectGetter(int _initialCapacityOfCacheList = 4)
            : this($"AnonymousUnsafeAsyncObjectGetter_{Serialize.NextUnsafeAsyncObjectGetter()}", _initialCapacityOfCacheList)
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
                Console.LogWarning(SystemNames.ObjectGetter, $"-- {name} -- : Failed to get the object because the complete action is null");
                return;
            }

            if (_key == null)
            {
                Console.LogWarning(SystemNames.ObjectGetter, $"-- {name} -- : Failed to get the object because the key is null");
                return;
            }
            
            if (TryGetFromCache(_key, out T_OBJECT obj))
            {
                Console.LogVerbose(SystemNames.ObjectGetter, $"-- {name} -- key-{_key} -- : Get the object from the cache");
                _complete.Invoke(obj);
                return;
            }
            
            LoadObject(_key, _obj =>
            {
                if (_obj == null)
                {
                    Console.LogWarning(SystemNames.ObjectGetter, $"-- {name} -- key-{_key} -- : Failed to get the object from the loader.");
                    _complete.Invoke(default);
                    return;
                }
                
                Console.LogVerbose(SystemNames.ObjectGetter, $"-- {name} -- key-{_key} -- : Get the object from the loader");
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
                Console.LogWarning(SystemNames.ObjectGetter, $"-- {name} -- : Failed to release the object because the object is null");
                return;
            }
            
            if (_key == null)
            {
                Console.LogWarning(SystemNames.ObjectGetter, $"-- {name} -- : Failed to release the object in because the key is null");
                return;
            }
            
            Console.LogVerbose(SystemNames.ObjectGetter, $"-- {name} -- key-{_key} -- : Release the object");
            PushBackToCache(_key, _obj);
        }
        
        
        /// <summary>
        /// Load the object by key.
        /// </summary>
        /// <remarks>
        /// <para>Implement this function to customize the loading process of the object which is a new one you want.</para>
        /// <para>When the object is loaded, call the '_complete' function to notify the getter system that the object is ready.</para>
        /// </remarks>
        protected abstract void LoadObject(T_KEY _key, Action<T_OBJECT> _complete);
    }
    /// <summary>
    /// A safe asynchronous object getter.
    /// </summary>
    /// <remarks>
    /// <para>"Unsafe" means that the getter will not make sure that the object is managed by this getter when you release it.</para>
    /// <para>Also you need to deal with the asynchronous "Get" function.</para>
    /// <para>Unsafe getters are faster than safe getters, but you need to be careful when using them.</para>
    /// </remarks>
    /// <typeparam name="T_OBJECT">The object's type</typeparam>
    public abstract class _AUnsafeAsyncObjectGetter<T_OBJECT> : _AObjectGetter<T_OBJECT>
    {
        protected _AUnsafeAsyncObjectGetter(string _name, int _initialCapacityOfCacheList = 4)
            : base(_name, _initialCapacityOfCacheList)
        {
        }
        protected _AUnsafeAsyncObjectGetter(int _initialCapacityOfCacheList = 4)
            : this($"AnonymousUnsafeAsyncObjectGetter_{Serialize.NextUnsafeAsyncObjectGetter()}", _initialCapacityOfCacheList)
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
                Console.LogWarning(SystemNames.ObjectGetter, $"-- {name} -- : Failed to get the object because the complete action is null");
                return;
            }
            
            if (TryGetFromCache(out T_OBJECT obj))
            {
                Console.LogVerbose(SystemNames.ObjectGetter, $"-- {name} -- : Get the object from the cache");
                _complete.Invoke(obj);
                return;
            }
            
            LoadObject(_obj =>
            {
                if (_obj == null)
                {
                    Console.LogWarning(SystemNames.ObjectGetter, $"-- {name} -- : Failed to get the object from the loader.");
                    _complete.Invoke(default);
                    return;
                }
                
                Console.LogVerbose(SystemNames.ObjectGetter, $"-- {name} -- : Get the object from the loader");
                _complete.Invoke(_obj);
            });
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
            
            Console.LogVerbose(SystemNames.ObjectGetter, $"-- {name} -- : Release the object");
            PushBackToCache(_obj);
        }
        
        
        /// <summary>
        /// Load the object.
        /// </summary>
        /// <remarks>
        /// <para>Implement this function to customize the loading process of the object which is a new one you want.</para>
        /// <para>When the object is loaded, call the '_complete' function to notify the getter system that the object is ready.</para>
        /// </remarks>
        protected abstract void LoadObject(Action<T_OBJECT> _complete);
    }
}