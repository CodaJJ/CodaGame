// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace CodaGame
{
    public abstract class _AGameEntity
    {
        [NotNull] private readonly Dictionary<Type, _AGameComponent> _m_components;
        [ItemNotNull, NotNull] private readonly List<_AGameCapability> _m_capabilities;
        
        private readonly string _m_name;
        
        
        protected _AGameEntity(string _name)
        {
            _m_components = new Dictionary<Type, _AGameComponent>();
            _m_capabilities = new List<_AGameCapability>();
            
            _m_name = _name;
        }
        
        
        public string name { get { return _m_name; } }
        
        
        public void AddComponent<T>(T _component) where T : _AGameComponent
        {
            _m_components[typeof(T)] = _component;
        
            if (_component is _AGameComponent baseComp)
            {
                // baseComp.Initialize(this);
            }
        }
        
        
        internal void Init()
        {
        }
        internal void Reset()
        {
        }
    }
}