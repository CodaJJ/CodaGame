// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System.Collections.Generic;
using CodaGame.Base;
using JetBrains.Annotations;

namespace CodaGame
{
    /// <summary>
    /// A game logic.
    /// </summary>
    /// <remarks>
    /// <para>It's an ECS like game logic, but the system only handles the game rules, also we have capability class in Entity for the entity behaviour.</para>
    /// </remarks>
    public abstract class _AGameLogic
    {
        // The name of this game logic.
        private readonly string _m_name;
        // The time step of this game logic in seconds, meaning the time between each tick.
        private readonly float _m_timeStep;

        // The list of entities in this game logic.
        [ItemNotNull, NotNull] private readonly List<_AGameEntity> _m_entities;
        // The list of systems in this game logic.
        [ItemNotNull, NotNull] private readonly List<_AGameSystem> _m_systems;

        // The tick task of this game logic.
        [NotNull] private readonly TickTask _m_tickTask;
        
        // Whether this game logic is running.
        private bool _m_isRunning;
        // Whether this game logic is paused.
        private bool _m_isPause;
        // The game speed of this game logic, default is 1.
        private float _m_gameSpeed;
        
        
        protected _AGameLogic(string _name, float _timeStep = 1 / 60f)
        {
            _m_name = _name;
            _m_timeStep = _timeStep;
            
            _m_systems = new List<_AGameSystem>();
            _m_entities = new List<_AGameEntity>();
            _m_tickTask = new TickTask(this);

            _m_gameSpeed = 1f;

            if (_m_timeStep <= 0)
            {
                _m_timeStep = 1 / 60f;
                Console.LogError(SystemNames.GameLogic, _m_name, "Time step is set to 0 or negative. Defaulting to 1/60.");
            }
        }
        
        
        /// <summary>
        /// The name of this game logic.
        /// </summary>
        public string name { get { return _m_name; } }
        /// <summary>
        /// The time step of this game logic in seconds.
        /// </summary>
        /// <remarks>
        /// <para>Meaning the time between each tick.</para>
        /// </remarks>
        public float timeStep { get { return _m_timeStep; } }
        /// <summary>
        /// The game speed of this game logic.
        /// </summary>
        public float gameSpeed
        {
            get { return _m_gameSpeed; }
            set
            {
                if (value < 0f)
                    value = 0f;
                
                _m_gameSpeed = value;
            }
        }
        /// <summary>
        /// Whether this game logic is running.
        /// </summary>
        public bool isRunning { get { return _m_isRunning; } }
        /// <summary>
        /// Whether this game logic is paused.
        /// </summary>
        public bool isPause { get { return _m_isPause; } }


        /// <summary>
        /// Start the game logic.
        /// </summary>
        public void Start()
        {
            if (_m_isRunning)
            {
                Console.LogWarning(SystemNames.GameLogic, _m_name, "Game logic is already running.");
                return;
            }

            OnGameStart();

            foreach (_AGameEntity entity in _m_entities)
            {
                entity.Init();
            }
            foreach (_AGameSystem system in _m_systems)
            {
                system.Start();
            }
            
            _m_tickTask.Run();

            _m_isRunning = true;
            _m_isPause = false;
            
            OnGameStartComplete();
            
            Console.LogSystem(SystemNames.GameLogic, _m_name, "Game logic started successfully.");
        }
        /// <summary>
        /// Stop the game logic.
        /// </summary>
        public void Stop()
        {
            if (!_m_isRunning)
            {
                Console.LogWarning(SystemNames.GameLogic, _m_name, "Game logic is not running.");
                return;
            }

            OnGameStop();
            
            foreach (_AGameSystem system in _m_systems)
            {
                system.Stop();
            }
            foreach (_AGameEntity entity in _m_entities)
            {
                entity.Reset();
            }
            
            _m_tickTask.Stop();
            
            _m_isRunning = false;
            _m_isPause = false;
            
            OnGameStopComplete();
            
            Console.LogSystem(SystemNames.GameLogic, _m_name, "Game logic stopped successfully.");
        }
        /// <summary>
        /// Pause the game logic.
        /// </summary>
        public void Pause()
        {
            if (!_m_isRunning)
                return;
            
            _m_isPause = true;
            Console.LogSystem(SystemNames.GameLogic, _m_name, "Game logic paused.");
        }
        /// <summary>
        /// Resume the game logic.
        /// </summary>
        public void Resume()
        {
            if (!_m_isRunning)
                return;
            
            _m_isPause = false;
            Console.LogSystem(SystemNames.GameLogic, _m_name, "Game logic resumed.");
        }
        /// <summary>
        /// Add an entity to the game logic.
        /// </summary>
        public void AddEntity(_AGameEntity _entity)
        {
            if (_entity == null)
                return;
            
            _m_entities.Add(_entity);
         
            if (_m_isRunning)
                _entity.Init();
            
            foreach (_AGameSystem system in _m_systems)
            {
                system.TryAddEntity(_entity);
            }
            
            Console.LogVerbose(SystemNames.GameLogic, _m_name, $"Entity '{_entity.name}' added to game logic.");
        }
        /// <summary>
        /// Remove an entity from the game logic.
        /// </summary>
        public void RemoveEntity(_AGameEntity _entity)
        {
            if (_entity == null)
                return;

            if (!_m_entities.Remove(_entity))
            {
                Console.LogWarning(SystemNames.GameLogic, _m_name, $"Entity '{_entity.name}' not found in game logic.");
                return;
            }

            foreach (_AGameSystem system in _m_systems)
            {
                system.TryRemoveEntity(_entity);
            }
            
            if (_m_isRunning)
                _entity.Reset();
            
            Console.LogVerbose(SystemNames.GameLogic, _m_name, $"Entity '{_entity.name}' removed from game logic.");
        }
        /// <summary>
        /// Add a system to the game logic.
        /// </summary>
        public void AddSystem(_AGameSystem _system)
        {
            if (_system == null)
                return;
            
            _m_systems.Add(_system);
            
            foreach (_AGameEntity entity in _m_entities)
            {
                _system.TryAddEntity(entity);
            }
            
            if (_m_isRunning)
                _system.Start();
            
            Console.LogVerbose(SystemNames.GameLogic, _m_name, $"System '{_system.name}' added to game logic.");
        }
        /// <summary>
        /// Remove a system from the game logic.
        /// </summary>
        public void RemoveSystem(_AGameSystem _system)
        {
            if (_system == null)
                return;

            if (!_m_systems.Remove(_system))
            {
                Console.LogWarning(SystemNames.GameLogic, _m_name, $"System '{_system.name}' not found in game logic.");
                return;
            }
            
            foreach (_AGameEntity entity in _m_entities)
            {
                _system.TryRemoveEntity(entity);
            }
            
            if (_m_isRunning)
                _system.Stop();
            
            Console.LogVerbose(SystemNames.GameLogic, _m_name, $"System '{_system.name}' removed from game logic.");
        }
        
        
        protected virtual void OnGameStart() { }
        protected virtual void OnGameStartComplete() { }
        protected virtual void OnGameStop() { }
        protected virtual void OnGameStopComplete() { }
        protected virtual void OnTick() { }
        

        private void Tick()
        {
            float deltaTime = _m_timeStep;
            
            OnTick();
            
            foreach (_AGameSystem system in _m_systems)
            {
                system.OnTick(deltaTime);
            }
        }


        private class TickTask : _AEveryFrameContinuousTask
        {
            [NotNull] private readonly _AGameLogic _m_gameLogic;
            
            private float _m_timeCounter; 
            
            
            public TickTask([NotNull] _AGameLogic _gameLogic) 
                : base($"{_gameLogic.name}'s tick task", UpdateType.Update, false)
            {
                _m_gameLogic = _gameLogic;
            }
            

            protected override void OnRun()
            {
            }
            protected override void OnStop()
            {
            }
            protected override void OnTick(float _deltaTime)
            {
                if (_m_gameLogic._m_isPause)
                    return;
                if (_m_gameLogic._m_timeStep <= 0)
                    return;

                _m_timeCounter += _deltaTime * _m_gameLogic._m_gameSpeed;
                while (_m_timeCounter >= _m_gameLogic._m_timeStep)
                {
                    _m_gameLogic.Tick();
                    _m_timeCounter -= _m_gameLogic._m_timeStep;
                }
            }
        }
    }
}