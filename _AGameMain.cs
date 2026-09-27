// Copyright (c) 2025 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using CodaGame.Base;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace CodaGame
{
    public abstract class _AGameMain : MonoBehaviour
    {
        [NotNull]
        protected internal static _AGameMain instance
        {
            get
            {
                if (_g_instance == null)
                {
                    // Do NOT route this through Console.LogCrush — Console.Log reads
                    // _AGameMain.instance.logLevel, which re-enters this getter and recurses
                    // until StackOverflow kills the Editor process (no managed catch possible).
                    // Use UnityEngine.Debug directly to break the cycle.
                    UnityEngine.Debug.LogError($"[{SystemNames.Main}] GameMain instance is null.");
                    throw new System.NullReferenceException("GameMain instance is null.");
                }

                return _g_instance;
            }
        }
        private static _AGameMain _g_instance;

        /// <summary>
        /// Null-safe variant of <see cref="instance"/>. Returns null instead of throwing when
        /// no GameMain exists yet — for callers (Editor tooling, internal logging) that must
        /// not re-enter <see cref="instance"/> from inside their own error path.
        /// </summary>
        internal static _AGameMain instanceOrNull { get { return _g_instance; } }


        /// <summary>
        /// The log level of the console.
        /// </summary>
        /// <remarks>
        /// <para>The log level is used to filter the log messages.</para>
        /// <para>The log messages with a log type lower than the log level will be ignored.(Verbose &lt; Debug &lt; System &lt; Warning &lt; Error &lt; Crush)</para>
        /// </remarks>
        public ELogLevel logLevel = ELogLevel.Debug;
        
        [SerializeField, RuntimeReadOnly]
        private int _m_logicFps = 60;
        // Both input snapshots and timestamp boundaries retain this much logic time, including the current frame.
        private const float _k_inputHistorySeconds = 2f;
        private int _m_inputHistoryFrameCount;
        [SerializeField, RuntimeReadOnly]
        private string _m_gameVersion = "1.0.0";
        [SerializeField, RuntimeReadOnly]
        private InspectorList<Transform> _m_uiLayers;
        [SerializeField, RuntimeReadOnly]
        private AudioMixer _m_audioMixer;

        [NotNull] private readonly LogicLoop _m_logicLoopTask;
        [NotNull] private readonly ShowLoop _m_showLoopTask;

        private float _m_gameSpeed = 1f;
        // The Unity Scene this GameMain instance lives in. Cached in Awake. By project convention this is
        // the persistent "Base" scene that is loaded at game launch and never unloaded; all other scenes
        // are loaded additively (typically via the Stage system).
        private Scene _m_baseScene;


        public _AGameMain()
        {
            _m_logicLoopTask = new LogicLoop(this);
            _m_showLoopTask = new ShowLoop(this);
        }


        public int logicFps { get { return _m_logicFps; } }
        public int logicFrameCount { get { return _m_logicLoopTask.frameCount; } }
        public float logicAlpha { get { return _m_logicLoopTask.currentAlpha; } }
        /// <summary>
        /// Multiplier on simulation speed. 1 = normal, 0.5 = slow-mo, 2 = double speed, 0 = pause.
        /// All changes (zero or non-zero) take effect immediately within the current OnTick:
        /// the catch-up loop re-reads gameSpeed each iteration and recomputes the per-tick interval.
        /// Negative values are clamped to 0.
        /// </summary>
        public float gameSpeed { get { return _m_gameSpeed; } set { _m_gameSpeed = Mathf.Max(0f, value); } }
        public string gameVersion { get { return _m_gameVersion; } }
        public ReadOnlyList<Transform> uiLayers { get { return _m_uiLayers; } }
        public AudioMixer audioMixer { get { return _m_audioMixer; } }
        /// <summary>
        /// The persistent Base scene this <see cref="_AGameMain"/> lives in. By project convention this scene
        /// is loaded at game launch and never unloaded; all other scenes are loaded additively (typically via
        /// the Stage system).
        /// </summary>
        public Scene baseScene { get { return _m_baseScene; } }
        
        
        internal int inputHistoryFrameCount { get { return _m_inputHistoryFrameCount; } }


        /// <summary>
        /// Finds the logic frame containing the given wallclock time, supporting variable gameSpeed and pause.
        /// Returns false when the time predates the oldest retained boundary or history is not initialized.
        ///
        /// Internal: the result is only meaningful for "recent" wallclock times (within the history
        /// window) and depends on framework-internal state. Outside the framework, use logicFrameCount
        /// for the current frame.
        /// </summary>
        internal bool TryCalculateLogicFrameIndex(double _timeSinceStartup, out int _frame)
        {
            return _m_logicLoopTask.TryCalculateLogicFrameIndex(_timeSinceStartup, out _frame);
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
            _m_baseScene = gameObject.scene;

            if (_m_logicFps <= 0)
            {
                Console.LogWarning(SystemNames.Main, "Logic FPS must be greater than 0. Setting to default value of 60.");
                _m_logicFps = 60;
            }
            _m_inputHistoryFrameCount = Mathf.CeilToInt(_k_inputHistorySeconds * _m_logicFps);

            if (InputSystem.settings.updateMode != InputSettings.UpdateMode.ProcessEventsInDynamicUpdate)
            {
                InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
                Console.LogWarning(SystemNames.Main, "Input System update mode set to ProcessEventsInDynamicUpdate.");
            }

            Physics.autoSyncTransforms = false;
            Physics2D.autoSyncTransforms = false;

            _m_logicLoopTask.Run();
            _m_showLoopTask.Run();
        }
        private void Start()
        {
            // Defer OnStartGame by one frame. The very first frame can have anomalous Time readings —
            // unscaledTime may include pre-Awake startup time and the first deltaTime can be large — which
            // throws off delay-based scheduling kicked off by downstream gameplay code. By yielding one
            // frame here, every Flow / Stage / Task scheduled from OnStartGame onward sees canonical Time.
            Task.RunNextFrameActionTask(OnStartGame);
        }
        private void OnDestroy()
        {
            _m_logicLoopTask.Stop();
            _m_showLoopTask.Stop();

            if (_g_instance == this)
                _g_instance = null;
        }


        /// <summary>
        /// Project-specific game entry point. Called once after framework initialization and after every
        /// other component in the Base scene has finished its <c>Awake</c>. Implementations typically start
        /// the initial <c>Flow</c>, load the first Main <c>Stage</c>, or trigger save-load.
        /// </summary>
        protected abstract void OnStartGame();


        private class LogicLoop : _AEveryFrameContinuousTask
        {
            [NotNull] private readonly _AGameMain _m_gameMain;
            [NotNull] private TickRecord[] _m_history = System.Array.Empty<TickRecord>();

            private double _m_lastRealTime;
            private double _m_accumulator;   // wallclock leftover since last tick (gameSpeed enters via interval, not addition)
            private int _m_frameCount;
            private float _m_currentAlpha;
            private int _m_historyHead;      // index of the most recent record
            private int _m_historyCount;     // only initialized boundaries may participate in lookup


            private struct TickRecord
            {
                public double wallTime;   // wallclock at which this frame started
                public int frame;
            }


            public LogicLoop([NotNull] _AGameMain _gameMain)
                : base("Main Logic Loop", UpdateType.Update, true)
            {
                _m_gameMain = _gameMain;
            }


            public int frameCount { get { return _m_frameCount; } }
            public float currentAlpha { get { return _m_currentAlpha; } }


            /// <summary>
            /// Maps a wallclock time to the logic frame whose window contains it. Walks the history
            /// ring buffer backward and returns the most recent recorded frame whose start wallclock
            /// is &lt;= the query time. Out-of-history queries fail; frame 0 is a valid frame, not a fallback.
            /// </summary>
            public bool TryCalculateLogicFrameIndex(double _wallTime, out int _frame)
            {
                for (int i = 0; i < _m_historyCount; ++i)
                {
                    int idx = (_m_historyHead - i + _m_history.Length) % _m_history.Length;
                    TickRecord rec = _m_history[idx];
                    if (rec.wallTime <= _wallTime)
                    {
                        _frame = rec.frame;
                        return true;
                    }
                }
                _frame = default;
                return false;
            }


            protected override void OnRun()
            {
                double now = Time.realtimeSinceStartupAsDouble;
                _m_lastRealTime = now;
                _m_accumulator = 0;
                _m_frameCount = 0;
                _m_currentAlpha = 0f;

                // Seed history with frame 0 starting at startup wallclock.
                _m_history = new TickRecord[_m_gameMain.inputHistoryFrameCount];
                _m_historyHead = 0;
                _m_historyCount = 1;
                _m_history[0] = new TickRecord { wallTime = now, frame = 0 };
            }
            protected override void OnStop()
            {
            }
            protected override void OnTick(float _)
            {
                int fps = _m_gameMain._m_logicFps;
                double now = Time.realtimeSinceStartupAsDouble;

                if (fps <= 0)
                {
                    _m_lastRealTime = now;
                    return;
                }

                if (_m_gameMain._m_gameSpeed <= 0f)
                {
                    // Paused at entry: skip wallclock that would have been accumulated.
                    _m_lastRealTime = now;
                    return;
                }

                double wallDelta = now - _m_lastRealTime;
                _m_lastRealTime = now;
                // Accumulator stores raw wallclock leftover; gameSpeed enters via the interval below
                // so mid-loop speed changes (slow-mo / speed-up / hit-stop) take effect immediately.
                _m_accumulator += wallDelta;

                while (true)
                {
                    float speed = _m_gameMain._m_gameSpeed;
                    if (speed <= 0f)
                        break;   // hit-stop or pause set inside last LogicTick

                    double interval = 1.0 / (fps * speed);   // wallclock per tick at current speed
                    if (_m_accumulator < interval)
                        break;

                    _m_accumulator -= interval;

                    Physics.SyncTransforms();
                    Physics2D.SyncTransforms();

                    LogicTick();

                    // Increment then record AFTER LogicTick: matches the original semantic
                    // where logicFrameCount inside LogicTick is "ticks completed so far".
                    _m_frameCount++;
                    double boundaryWallTime = now - _m_accumulator;   // accumulator is wallclock
                    _m_historyHead = (_m_historyHead + 1) % _m_history.Length;
                    _m_history[_m_historyHead] = new TickRecord { wallTime = boundaryWallTime, frame = _m_frameCount };
                    if (_m_historyCount < _m_history.Length)
                        _m_historyCount++;
                    // Open the new frame before the next catch-up tick or ShowTick can query input.
                    CodaGame.Base.PlayerInputManager.instance.AdvanceLogicFrame(_m_frameCount);
                }

                // Alpha: under variable speed, interval depends on current speed. When paused, freeze at 1
                // so ShowTick renders the just-ticked end state (visual freeze for hit-stop).
                float endSpeed = _m_gameMain._m_gameSpeed;
                if (endSpeed > 0f)
                {
                    double currentInterval = 1.0 / (fps * endSpeed);
                    _m_currentAlpha = Mathf.Clamp01((float)(_m_accumulator / currentInterval));
                }
                else
                {
                    _m_currentAlpha = 1f;
                }
            }


            private void LogicTick()
            {
                ActorManager.instance.LogicTick();
            }
        }
        private class ShowLoop : _AEveryFrameContinuousTask
        {
            [NotNull] private readonly _AGameMain _m_gameMain;


            public ShowLoop([NotNull] _AGameMain _gameMain)
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
                ActorManager.instance.ShowTick(_m_gameMain.logicAlpha);
            }
        }
    }
}
