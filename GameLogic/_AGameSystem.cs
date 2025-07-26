// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace CodaGame
{
    public class _AGameSystem
    {
        private readonly string _m_name;
        
        private _AGameLogic _gameLogic;
        private bool _m_isRunning;


        public _AGameSystem(string _name)
        {
            _m_name = _name;
        }
        
        
        public string name { get { return _m_name; } }
        
        
        internal void Start()
        {
        }
        internal void Stop()
        {
        }
        internal void OnTick(float _deltaTime)
        {
        }
        internal void TryRemoveEntity(_AGameEntity _entity)
        {
        }
        internal void TryAddEntity(_AGameEntity _entity)
        {
        }
    }
}