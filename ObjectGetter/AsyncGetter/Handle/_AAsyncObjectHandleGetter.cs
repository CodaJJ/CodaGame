
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityGameFramework.Base;

namespace UnityGameFramework
{
    /// <summary>
    /// A object handle getter that can get multiple object handles with different keys.
    /// </summary>
    /// <remarks>
    /// <para>The object handle is a instance that handles the object, you can access the object behavior easily by the handle.</para>
    /// <para>
    /// The interaction between the getter and the handle is synchronous, so you can get the object handle and use it immediately.
    /// Though the object loading is asynchronous, the handle will deal with this for you.
    /// </para>
    /// </remarks>
    /// <typeparam name="T_KEY">The key's type for objects</typeparam>
    /// <typeparam name="T_OBJECT">The object's type</typeparam>
    public abstract class _AAsyncObjectHandleGetter<T_KEY, T_OBJECT> : _AObjectGetter<T_KEY, T_OBJECT>, _IObjectHandleGetter
    {
        private int _m_handleCount;
        
        
        protected _AAsyncObjectHandleGetter(string _name, int _initialCapacityOfCacheList = 4) 
            : base(_name, _initialCapacityOfCacheList)
        {
        }
        protected _AAsyncObjectHandleGetter(int _initialCapacityOfCacheList = 4)
            : this($"ObjectHandleGetter_{Serialize.NextObjectHandleGetter()}", _initialCapacityOfCacheList)
        {
        }


        /// <summary>
        /// Get the object handle.
        /// </summary>
        /// <remarks>
        /// <para>If the object is in the cache, the handle will be set with the object from the cache, otherwise that will be set with the object loading.</para>
        /// </remarks>
        public GetterObjectHandle<T_KEY, T_OBJECT> Get(T_KEY _key)
        {
            if (_key == null)
            {
                Console.LogWarning(SystemNames.ObjectGetter, $"-- {name} -- : The key is null, it's meaningless to call this function.");
                return null;
            }
            
            GetterObjectHandle<T_KEY, T_OBJECT> handle = new GetterObjectHandle<T_KEY, T_OBJECT>(this, _key);
            _m_handleCount++;
            
            if (TryGetFromCache(_key, out T_OBJECT obj))
            {
                Console.LogVerbose(SystemNames.ObjectGetter, $"-- {name} -- key-{_key} -- : Get a handle, the object got from the cache, now the handle count is {_m_handleCount}.");
                
                handle.SetObject(obj);
                handle.SetLoadDone();
                return handle;
            }
            
            Console.LogVerbose(SystemNames.ObjectGetter, $"-- {name} -- key-{_key} -- : The cache is empty, through loading process to get a handle, now the handle count is {_m_handleCount}.");
            LoadObject(_key, _newObj =>
            {
                if (!handle.enable)
                {
                    Console.LogVerbose(SystemNames.ObjectGetter, $"-- {name} -- key-{_key} -- : The handle is disabled before the object is loaded, push the loaded object to the cache, now the handle count is {_m_handleCount}.");
                    PushBackToCache(_key, _newObj);
                    return;
                }
                
                Console.LogVerbose(SystemNames.ObjectGetter, $"-- {name} -- key-{_key} -- : The object is loaded, set the object to the handle.");
                handle.SetObject(_newObj);
                handle.SetLoadDone();
            });
            
            return handle;
        }
        /// <summary>
        /// Release the object handle.
        /// </summary>
        /// <remarks>
        /// <para>You can release the handle whenever you want, but you should release it when you don't need it anymore.</para>
        /// </remarks>
        public void Release(GetterObjectHandle<T_KEY, T_OBJECT> _handle)
        {
            if (_handle == null)
            {
                Console.LogWarning(SystemNames.ObjectGetter, $"-- {name} -- : The handle is null, it's meaningless to call this function.");
                return;
            }
            
            if (_handle.CheckOwner(this))
            {
                Console.LogWarning(SystemNames.ObjectGetter, $"-- {name} -- : The handle is not owned by this getter, it's meaningless to call this function.");
                return;
            }

            if (!_handle.enable)
            {
                Console.LogWarning(SystemNames.ObjectGetter, $"-- {name} -- : The handle is released, it's meaningless to call this function.");
                return;
            }

            if (_handle.obj != null)
            {
                Console.LogVerbose(SystemNames.ObjectGetter, $"-- {name} -- key-{_handle.key} -- : The handle has a loaded obj when releasing, push the object back to the cache, now the handle count is {_m_handleCount}.");
                PushBackToCache(_handle.key, _handle.obj);
                _handle.SetObject(default);
            }
            _handle.SetDisable();
            _m_handleCount--;
            Console.LogVerbose(SystemNames.ObjectGetter, $"-- {name} -- key-{_handle.key} -- : The handle is released, now the handle count is {_m_handleCount}.");
            if (!_handle.isLoaded)
                _handle.SetLoadDone();
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
    /// A object handle getter that can get multiple object handles.
    /// </summary>
    /// <remarks>
    /// <para>The object handle is a instance that handles the object, you can access the object behavior easily by the handle.</para>
    /// <para>
    /// The interaction between the getter and the handle is synchronous, so you can get the object handle and use it immediately.
    /// Though the object loading is asynchronous, the handle will deal with this for you.
    /// </para>
    /// </remarks>
    /// <typeparam name="T_OBJECT">The object's type</typeparam>
    public abstract class _AAsyncObjectHandleGetter<T_OBJECT> : _AObjectGetter<T_OBJECT>, _IObjectHandleGetter
    {
        private int _m_handleCount;
        
        
        protected _AAsyncObjectHandleGetter(string _name, int _initialCapacityOfCacheList = 4) 
            : base(_name, _initialCapacityOfCacheList)
        {
            _m_handleCount = 0;
        }
        protected _AAsyncObjectHandleGetter(int _initialCapacityOfCacheList = 4)
            : this($"MultiObjectGetter_{Serialize.NextObjectHandleGetter()}", _initialCapacityOfCacheList)
        {
        }
        
        
        /// <summary>
        /// Get the object handle.
        /// </summary>
        /// <remarks>
        /// <para>If the object is in the cache, the handle will be set with the object from the cache, otherwise that will be set with the object loading.</para>
        /// </remarks>
        public GetterObjectHandle<T_OBJECT> Get()
        {
            GetterObjectHandle<T_OBJECT> handle = new GetterObjectHandle<T_OBJECT>(this);
            _m_handleCount++;
            
            if (TryGetFromCache(out T_OBJECT obj))
            {
                handle.SetObject(obj);
                handle.SetLoadDone();
                
                Console.LogVerbose(SystemNames.ObjectGetter, $"-- {name} -- : Get a handle with the object got from the cache, now the handle count is {_m_handleCount}.");
                return handle;
            }
            
            LoadObject(_newObj =>
            {
                if (!handle.enable)
                {
                    Console.LogVerbose(SystemNames.ObjectGetter, $"-- {name} -- : The handle is disabled before the object is loaded, push the loaded object to the cache, now the handle count is {_m_handleCount}.");
                    PushBackToCache(_newObj);
                    return;
                }
                
                Console.LogVerbose(SystemNames.ObjectGetter, $"-- {name} -- : The object is loaded, set the object to the handle.");
                handle.SetObject(_newObj);
                handle.SetLoadDone();
            });
            
            Console.LogVerbose(SystemNames.ObjectGetter, $"-- {name} -- : The cache is empty, get a handle with the object loading, now the handle count is {_m_handleCount}.");
            return handle;
        }
        /// <summary>
        /// Release the object handle.
        /// </summary>
        /// <remarks>
        /// <para>You can release the handle whenever you want, but you should release it when you don't need it anymore.</para>
        /// </remarks>
        public void Release(GetterObjectHandle<T_OBJECT> _handle)
        {
            if (_handle == null)
            {
                Console.LogWarning(SystemNames.ObjectGetter, $"-- {name} -- : The handle is null, it's meaningless to call this function.");
                return;
            }
            
            if (_handle.CheckOwner(this))
            {
                Console.LogWarning(SystemNames.ObjectGetter, $"-- {name} -- : The handle is not owned by this getter, it's meaningless to call this function.");
                return;
            }

            if (!_handle.enable)
            {
                Console.LogWarning(SystemNames.ObjectGetter, $"-- {name} -- : The handle is released, it's meaningless to call this function.");
                return;
            }
            
            if (_handle.obj != null)
                PushBackToCache(_handle.obj);
            _handle.SetDisable();
            _m_handleCount--;
            Console.LogVerbose(SystemNames.ObjectGetter, $"-- {name} -- : The handle is released, now the handle count is {_m_handleCount}.");
            if (!_handle.isLoaded)
                _handle.SetLoadDone();
        }
        
        
        /// <summary>
        /// Load the object by key.
        /// </summary>
        /// <remarks>
        /// <para>Implement this function to customize the loading process of the object which is a new one you want.</para>
        /// <para>When the object is loaded, call the '_complete' function to notify the getter system that the object is ready.</para>
        /// </remarks>
        protected abstract void LoadObject(Action<T_OBJECT> _complete);
    }
}