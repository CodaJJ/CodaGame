// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using JetBrains.Annotations;

namespace CodaGame
{
    /// <summary>
    /// Represents a handle for an object managed by a getter system.
    /// </summary>
    /// <remarks>
    /// This class provides mechanisms to interact with the object and perform actions when the object is set by getter system.
    /// </remarks>
    /// <typeparam name="T_OBJECT">The type of the object.</typeparam>
    public class GetterObjectHandle<T_OBJECT>
    {
        // The getter that owns the handle.
        [NotNull] private readonly _IObjectHandleGetter _m_getter;
        
        // The managed object.
        private T_OBJECT _m_object;
        // The action to be performed when the object is set.
        private NotNullAction<T_OBJECT> _m_onObjectSet;
        // Whether the object is loaded.
        private bool _m_isLoaded;
        // The action to be performed when the object is loaded.
        private Action _m_onLoadDone;
        // Whether the handle is enabled.
        private bool _m_isEnable;
        
        
        internal GetterObjectHandle([NotNull] _IObjectHandleGetter _getter)
        {
            _m_getter = _getter;
            
            _m_isEnable = true;
        }
        
        
        /// <summary>
        /// Gets the managed object.
        /// </summary>
        /// <remarks>
        /// Do not make the object broken or modify it after release the handle.
        /// </remarks>
        [CanBeNull] public T_OBJECT obj { get { return _m_object; } }
        /// <summary>
        /// Gets whether the handle enable.
        /// </summary>
        /// <remarks>
        /// When the handle is released, it will be disabled infinitely and the object will be null.
        /// </remarks>
        public bool enable { get { return _m_isEnable; } }
        /// <summary>
        /// Gets whether the object is loaded.
        /// </summary>
        /// <remarks>
        /// Loaded state doesn't mean the object is not null, if the load object failed or the handle is released before object is set, "isLoaded" will still be true.
        /// </remarks>
        public bool isLoaded { get { return _m_isLoaded; } }
        
        
        /// <summary>
        /// Performs an action with the managed object.
        /// </summary>
        /// <remarks>
        /// The object will be surely set when the action is performed.
        /// </remarks>
        public void ActionWithObject(NotNullAction<T_OBJECT> _action)
        {
            if (!_m_isEnable)
            {
                Console.LogWarning(SystemNames.ObjectGetter, "The handle is released, it's meaningless to call this function.");
                return;
            }
            
            if (_action == null)
                return;
            
            if (_m_object != null)
                _action.Invoke(_m_object);
            else
                _m_onObjectSet += _action;
        }
        /// <summary>
        /// Adds an action to be performed when the object is loaded (load failed or released before load done will still trigger the action).
        /// </summary>
        /// <remarks>
        /// Because of the getter system is async, you can add an action to be performed when the async load is done.
        /// </remarks>
        public void AddLoadDoneAction(Action _action)
        {
            if (!_m_isEnable)
            {
                Console.LogWarning(SystemNames.ObjectGetter, "The handle is released, it's meaningless to call this function.");
                return;
            }
            
            if (_action == null)
                return;
            
            if (_m_isLoaded)
                _action.Invoke();
            else
                _m_onLoadDone += _action;
        }
        
        
        /// <summary>
        /// Sets the object, called by getter system.
        /// </summary>
        internal void SetObject(T_OBJECT _object)
        {
            _m_object = _object;
            if (_m_object != null)
                _m_onObjectSet?.Invoke(_m_object);
            
            _m_onObjectSet = null;
        }
        /// <summary>
        /// Sets the load done, called by getter system.
        /// </summary>
        internal void SetLoadDone()
        {
            _m_isLoaded = true;
            _m_onLoadDone?.Invoke();
            _m_onLoadDone = null;
        }
        /// <summary>
        /// Disables the handle, called by getter system.
        /// </summary>
        internal void SetDisable()
        {
            _m_isEnable = false;
        }
        /// <summary>
        /// Checks whether the getter is the owner of the handle.
        /// </summary>
        internal bool CheckOwner(_IObjectHandleGetter _owner)
        {
            return _m_getter == _owner;
        }
    }

    /// <summary>
    /// Represents a handle for an object managed by a getter system.
    /// </summary>
    /// <remarks>
    /// This class provides mechanisms to interact with the object and perform actions when the object is set by getter system.
    /// </remarks>
    /// <typeparam name="T_KEY">The key of the object.</typeparam>
    /// <typeparam name="T_OBJECT">The type of the object.</typeparam>
    public class GetterObjectHandle<T_KEY, T_OBJECT> : GetterObjectHandle<T_OBJECT>
    {
        // The key of the object.
        [NotNull] private readonly T_KEY _m_key;
        
        
        internal GetterObjectHandle([NotNull] _IObjectHandleGetter _getter, [NotNull] T_KEY _key)
            : base(_getter)
        {
            _m_key = _key;
        }
        
        
        /// <summary>
        /// Gets the key of the object.
        /// </summary>
        [NotNull] public T_KEY key { get { return _m_key; } }
    }
}