// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using CodaGame.Base;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CodaGame
{
    public class GameMain : MonoBehaviour
    {
        [NotNull]
        public static GameMain instance
        {
            get
            {
                if (_g_instance == null)
                    Console.LogCrush(SystemNames.Main, "GameMain instance is null.");
                
                return _g_instance;
            }
        }
        private static GameMain _g_instance;


        [SerializeField, RuntimeReadOnly]
        private int _m_logicFps = 60;

        [NotNull] private readonly LogicLoop _m_logicLoopTask;
        [NotNull] private readonly ShowLoop _m_showLoopTask;


        public GameMain()
        {
            _m_logicLoopTask = new LogicLoop(this);
            _m_showLoopTask = new ShowLoop(this);
        }
        
        
        public int logicFps { get { return _m_logicFps; } }
        public int logicFrameCount { get { return _m_logicLoopTask.frameCount; } }


        public int CalculateLogicFrameIndex(double _timeSinceStartup)
        {
            double relativeTime = _timeSinceStartup - _m_logicLoopTask.startTime;
            if (relativeTime < 0.0)
                return 0;

            return (int)(relativeTime * _m_logicFps);
        }
        
        
        private void Awake()
        {
            if (_g_instance != null && _g_instance != this)
            {
                Destroy(gameObject);
                Console.LogWarning(SystemNames.Main, "Duplicate GameMain instance detected. Destroying the new instance.");
                return;
            }
            
            _g_instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (_m_logicFps <= 0)
            {
                Console.LogWarning(SystemNames.Main, "Logic FPS must be greater than 0. Setting to default value of 60.");
                _m_logicFps = 60;
            }

            if (InputSystem.settings.updateMode != InputSettings.UpdateMode.ProcessEventsInDynamicUpdate)
            {
                InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
                Console.LogWarning(SystemNames.Main, "Input System update mode set to ProcessEventsInDynamicUpdate.");
            }
            
            _m_logicLoopTask.Run();
            _m_showLoopTask.Run();
        }
        private void OnDestroy()
        {
            _m_logicLoopTask.Stop();
            _m_showLoopTask.Stop();
            
            if (_g_instance == this)
                _g_instance = null;
        }


        private class LogicLoop : _AEveryFrameContinuousTask
        {
            [NotNull] private readonly GameMain _m_gameMain;
            private readonly double _m_timeInterval;

            private double _m_startTime;
            private int _m_frameCount;


            public LogicLoop([NotNull] GameMain _gameMain) 
                : base("Main Logic Loop", UpdateType.Update, true)
            {
                _m_gameMain = _gameMain;
                _m_timeInterval = 1f / _m_gameMain._m_logicFps;
            }
            
            
            public double startTime { get { return _m_startTime; } }
            public int frameCount { get { return _m_frameCount; } }
            
            
            protected override void OnRun()
            {
                _m_startTime = Time.realtimeSinceStartupAsDouble;
                _m_frameCount = 0;
            }
            protected override void OnStop()
            {
            }
            protected override void OnTick(float _)
            {
                double nowTime = Time.realtimeSinceStartupAsDouble - _m_startTime;
                double lastLogicTime = _m_frameCount * _m_timeInterval;
                double deltaTime = nowTime - lastLogicTime;
                while (deltaTime >= _m_timeInterval)
                {
                    deltaTime -= _m_timeInterval;
                    
                    LogicTick();
                    
                    _m_frameCount++;
                }
            }


            private void LogicTick()
            {
            }
        }
        private class ShowLoop : _AEveryFrameContinuousTask
        {
            [NotNull] private readonly GameMain _m_gameMain;
            
            
            public ShowLoop([NotNull] GameMain _gameMain) 
                : base("Main Show Loop", UpdateType.LateUpdate, false)
            {
                _m_gameMain = _gameMain;
            }
            
            
            protected override void OnRun()
            {
            }
            protected override void OnStop()
            {
            }
            protected override void OnTick(float _deltaTime)
            {
            }
        }
    }
}