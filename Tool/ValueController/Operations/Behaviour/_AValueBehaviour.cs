// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using CodaGame.Base;

namespace CodaGame
{
    /// <summary>
    /// Base class for value behaviours.
    /// </summary>
    /// <remarks>
    /// <para>Linear interpolation is default for fade in and fade out.</para>
    /// </remarks>
    public abstract class _AValueBehaviour<T_VALUE>
        where T_VALUE : struct
    {
        //           ↓←-------------------↑
        //           ↓                    ↑
        // Dead => FadeIn => Active => FadeOut => Dead
        //           ↓                    ↑
        //           ↓-------------------→↑
        private enum State { FadeIn, Active, FadeOut, Dead }
        
        private readonly int _m_priority;
        // Zero or negative values mean no fade in/out.
        private readonly float _m_fadeInTime;
        private readonly float _m_fadeOutTime;
        
        // The controller that owns this behaviour.
        private _AValueController<T_VALUE> _m_controller;
        // Blend weight for this behaviour, used during fade-in and fade-out transitions.
        // Ranges from 0 to 1: 0 means fully faded out, 1 means fully active.
        private float _m_blendFactor;
        // Timer used to track progress of fade-in or fade-out.
        private float _m_fadeTimer;
        // The current state of the behaviour.
        private State _m_state;
        
        
        /// <summary>
        /// Constructor for the behaviour.
        /// </summary>
        /// <param name="_priority">Higher priority behaviours override lower ones.</param>
        /// <param name="_fadeInTime">Zero or negative values mean no fade in.</param>
        /// <param name="_fadeOutTime">Zero or negative values mean no fade out.</param>
        protected _AValueBehaviour(int _priority, float _fadeInTime = 0.5f, float _fadeOutTime = 0.5f)
        {
            _m_priority = _priority;
            _m_fadeInTime = _fadeInTime;
            _m_fadeOutTime = _fadeOutTime;
            _m_state = State.Dead;
        }
        

        /// <summary>
        /// The controller that owns this behaviour.
        /// </summary>
        public int priority { get { return _m_priority; } }
        /// <summary>
        /// Is the behaviour currently not active?
        /// </summary>
        public bool isDead { get { return _m_state == State.Dead; } }
        /// <summary>
        /// Is the behaviour currently fading?
        /// </summary>
        public bool isFading { get { return _m_state is State.FadeIn or State.FadeOut; } }
        
        /// <summary>
        /// The weight of this behaviour. 
        /// </summary>
        /// <remarks>
        /// <para>Used to determine how much this behaviour contributes to the final value.</para>
        /// </remarks>
        public abstract float weight { get; }
        
        
        /// <summary>
        /// Blend weight for this behaviour
        /// </summary>
        /// <remarks>
        /// <para>Used during fade-in and fade-out transitions. Ranges from 0 to 1: 0 means fully faded out, 1 means fully active.</para>
        /// </remarks>
        internal float blendFactor { get { return _m_blendFactor; } }
        

        /// <summary>
        /// Update the behaviour.
        /// </summary>
        protected abstract void Update(float _deltaTime);
        /// <summary>
        /// Evaluate the behaviour.
        /// </summary>
        protected abstract T_VALUE Evaluate();
        
        /// <summary>
        /// Fade in function.
        /// </summary>
        /// <remarks>
        /// <para>Only used when the fade-in time is greater than 0.</para>
        /// <para>
        /// <paramref name="_value"/> is the normalized time, progressing from 0 to 1 during the entire fade-in process.
        /// </para>
        /// <para>
        /// When <paramref name="_value"/> reaches 1, the fade-in is considered complete,
        /// and the return value will typically be 1—meaning this behaviour will be fully blended in.
        /// </para>
        /// </remarks>
        protected virtual float FadeIn(float _value)
        {
            return _value;
        }
        /// <summary>
        /// Inverse fade-in function.
        /// </summary>
        /// <remarks>
        /// <para>This is the inverse of <see cref="FadeIn(float)"/>.</para>
        /// <para>
        /// Used when a fade-in is started while the behaviour was still fading out,
        /// allowing a smooth and seamless transition back into fade-in from the current state.
        /// </para>
        /// <para>If you're sure fade-in/out won't ever interrupt an ongoing fade-out, you can just return 0 here.</para>
        /// </remarks>
        protected virtual float InverseFadeIn(float _value)
        {
            return _value;
        }
        /// <summary>
        /// Fade out function.
        /// </summary>
        /// <remarks>
        /// <para>Only used when the fade-out time is greater than 0.</para>
        /// <para>
        /// <paramref name="_value"/> is the normalized time, progressing from 0 to 1 during the entire fade-out process.
        /// </para>
        /// <para>
        /// When <paramref name="_value"/> reaches 1, the fade-out is considered complete,
        /// and the return value will typically be 1 as well—meaning this behaviour's weight in blending becomes 0.
        /// </para>
        /// </remarks>
        protected virtual float FadeOut(float _value)
        {
            return _value;
        }
        /// <summary>
        /// Inverse fade out function.
        /// </summary>
        /// <remarks>
        /// <para>Inverse of <see cref="FadeOut(float)"/>.</para>
        /// <para>
        /// Used when a fade-out is interrupted and needs to transition seamlessly into a fade-in,
        /// starting from the current fade-out position.
        /// </para>
        /// <para>
        /// If you can guarantee that fade-in/out will never be interrupted, you can safely return 0 here.
        /// </para>
        /// </remarks>
        protected virtual float InverseFadeOut(float _value)
        {
            return _value;
        }
        /// <summary>
        /// Called when this behaviour is actually removed from the controller.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Note that this is not triggered immediately when <c>RemoveBehaviour</c> is called;
        /// instead, it is invoked after fade-out effect has completed,
        /// indicating that the behaviour is no longer active within the controller.
        /// </para>
        /// </remarks>
        protected virtual void OnRemoved()
        {
            
        }


        /// <summary>
        /// Check if the behaviour is added to a controller.
        /// </summary>
        /// <remarks>
        /// <para><paramref name="_controller"/> is optional. If provided, it checks if this behaviour is added to that controller.</para>
        /// </remarks>
        internal bool IsBehaviourAdded(_AValueController<T_VALUE> _controller = null)
        {
            if (_controller != null)
                return _m_controller == _controller;
            
            return _m_controller != null;
        }
        /// <summary>
        /// Set the controller that owns this behaviour.
        /// </summary>
        internal void SetBehaviourAdded(_AValueController<T_VALUE> _controller)
        {
            if (_m_controller != null)
            {
                Console.LogWarning(SystemNames.ValueController, _m_controller.name, "Behaviour already added to another controller");
                return;
            }

            _m_controller = _controller;
        }
        /// <summary>
        /// Set the controller that owns this behaviour to null.
        /// </summary>
        internal void SetBehaviourRemoved()
        {
            OnRemovedInternal();
            OnRemoved();
            
            _m_controller = null;
            _m_state = State.Dead;
            _m_blendFactor = 0f;
            _m_fadeTimer = 0f;
        }
        /// <summary>
        /// Start the fade-in process.
        /// </summary>
        /// <remarks>
        /// <para>It's safe to call this method multiple times, only active when the state is fade-out or dead.</para>
        /// <para>If the fade-in time is 0 or negative, the behaviour will be set to active immediately.</para>
        /// </remarks>
        internal void StartFadeIn()
        {
            if (_m_state is State.FadeIn or State.Active)
                return;
            
            if (_m_fadeInTime > 0)
            {
                _m_state = State.FadeIn;
                _m_fadeTimer = InverseFadeIn(_m_blendFactor) * _m_fadeInTime;
                return;
            }
            
            _m_blendFactor = 1f;
            _m_state = State.Active;
        }
        /// <summary>
        /// Start the fade-out process.
        /// </summary>
        /// <remarks>
        /// <para>It's safe to call this method multiple times, only active when the state is fade-in or active.</para>
        /// <para>If the fade-out time is 0 or negative, the behaviour will be set to dead immediately.</para>
        /// </remarks>
        internal void StartFadeOut()
        {
            if (_m_state is State.FadeOut or State.Dead)
                return;
            
            if (_m_fadeOutTime > 0)
            {
                _m_state = State.FadeOut;
                _m_fadeTimer = InverseFadeOut(1f - _m_blendFactor) * _m_fadeOutTime;
                return;
            }
            
            _m_blendFactor = 0f;
            _m_state = State.Dead;
        }
        /// <summary>
        /// Update the behaviour.
        /// </summary>
        /// <remarks>
        /// <para>There is a simple state machine to handle the fade-in and fade-out process.</para>
        /// </remarks>
        internal void InternalUpdate(float _deltaTime)
        {
            switch (_m_state)
            {
                case State.FadeIn:
                    _m_fadeTimer += _deltaTime;
                    if (_m_fadeTimer >= _m_fadeInTime)
                    {
                        _m_blendFactor = 1f;
                        _m_state = State.Active;
                    }
                    else
                        _m_blendFactor = FadeIn(_m_fadeTimer / _m_fadeInTime);
                    break;
                case State.FadeOut:
                    _m_fadeTimer += _deltaTime;
                    if (_m_fadeTimer >= _m_fadeOutTime)
                    {
                        _m_blendFactor = 0f;
                        _m_state = State.Dead;
                    }
                    else
                        _m_blendFactor = 1f - FadeOut(_m_fadeTimer / _m_fadeOutTime);
                    break;
            }

            Update(_deltaTime);
        }
        /// <summary>
        /// Evaluate the behaviour.
        /// </summary>
        /// <remarks>
        /// <para>Just keeps the interface clean.</para>
        /// </remarks>
        internal T_VALUE InternalEvaluate()
        {
            return Evaluate();
        }


        /// <summary>
        /// Additional internal method to be called when the behaviour is removed.
        /// </summary>
        private protected virtual void OnRemovedInternal() { }
    }
}