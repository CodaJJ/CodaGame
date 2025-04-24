// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System.Collections.Generic;
using JetBrains.Annotations;

namespace CodaGame.Base
{
    /// <summary>
    /// Base class for value controllers.
    /// </summary>
    /// <remarks>
    /// <para>The ValueController manages a value that can be influenced by multiple behaviours, offsets, and constraints.</para>
    /// <para>
    /// For behaviours, the final value is blended from all behaviours. Behaviours with higher priority will 
    /// override those with lower priority during blending.
    /// </para>
    /// <para>And offsets are added to the current value after blending behaviours.</para>
    /// <para>
    /// Constraints are applied in the order they are added. Each constraint modifies the value once,
    /// and the result is not re-evaluated after each application. Therefore, make sure to add constraints in the correct order.
    /// </para>
    /// </remarks>
    public abstract partial class _AValueController<T_VALUE>
        where T_VALUE : struct
    {
        private readonly string _m_name;
        
        [ItemNotNull, NotNull] private readonly List<ValueBehaviourLayer> _m_behaviourLayers;
        [ItemNotNull, NotNull] private readonly List<_AValueOffset<T_VALUE>> _m_offsetList;
        [ItemNotNull, NotNull] private readonly List<_AValueConstraint<T_VALUE>> _m_constraintList;
     
        // Temporary lists used for traversal to avoid modifying the original list while iterating.
        [ItemNotNull, NotNull] private readonly List<ValueBehaviourLayer> _m_layerListForTraversal;
        [ItemNotNull, NotNull] private readonly List<_AValueOffset<T_VALUE>> _m_offsetListForTraversal;
        [ItemNotNull, NotNull] private readonly List<_AValueConstraint<T_VALUE>> _m_constraintListForTraversal;
        
        // The final value after all processing.
        private T_VALUE _m_value;
        // The base value is a lowest priority behaviour that is always present.
        private T_VALUE _m_baseValue;
        // The dirty flag is used to indicate if the value needs to be re-evaluated.
        private bool _m_isDirty;


        protected _AValueController(string _name, T_VALUE _initValue)
        {
            _m_name = _name;
            _m_baseValue = _initValue;
            
            _m_behaviourLayers = new List<ValueBehaviourLayer>();
            _m_offsetList = new List<_AValueOffset<T_VALUE>>();
            _m_constraintList = new List<_AValueConstraint<T_VALUE>>();
            
            _m_layerListForTraversal = new List<ValueBehaviourLayer>();
            _m_offsetListForTraversal = new List<_AValueOffset<T_VALUE>>();
            _m_constraintListForTraversal = new List<_AValueConstraint<T_VALUE>>();
            SetDirty();
        }
        
        
        /// <summary>
        /// The name of the value controller.
        /// </summary>
        public string name { get { return _m_name; } }
        /// <summary>
        /// The current value of the value controller.
        /// </summary>
        /// <remarks>
        /// <para>The final value after all processing.</para>
        /// </remarks>
        public T_VALUE value
        {
            get
            {
                if (_m_isDirty)
                {
                    _m_value = _Evaluate();
                    _m_isDirty = false;
                }
                
                return _m_value;
            }
        }
        /// <summary>
        /// The base value of the value controller.
        /// </summary>
        /// <remarks>
        /// <para>The base value is a lowest priority behaviour that is always present.</para>
        /// </remarks>
        public T_VALUE baseValue { set { _m_baseValue = value; } }


        /// <summary>
        /// Adds a behaviour to the value controller.
        /// </summary>
        /// <remarks>
        /// <para>Behaviours are blended together to produce the final value.</para>
        /// <para>When added, a behaviour will go through a fade-in process.</para>
        /// <para>If the behaviour is currently fading out, this will interrupt the fade-out and smoothly fade it back in
        /// from where it left off.</para>
        /// </remarks>
        public void AddBehaviour(_AValueBehaviour<T_VALUE> _behaviour)
        {
            if (_behaviour == null)
            {
                Console.LogWarning(SystemNames.ValueController, _m_name, "Add behaviour failed, behaviour is null.");
                return;
            }

            // Check if the behaviour is already added to this controller
            if (_behaviour.IsBehaviourAdded(this))
            {
                _behaviour.StartFadeIn();
                return;
            }

            // Check if the behaviour is already added to another controller
            if (_behaviour.IsBehaviourAdded())
            {
                Console.LogWarning(SystemNames.ValueController, _m_name, "Add behaviour failed, behaviour is already added to another controller.");
                return;
            }
            
            _behaviour.SetBehaviourAdded(this);
            ValueBehaviourLayer layer = _GetOrCreateBehaviourLayer(_behaviour.priority);
            layer.AddBehaviour(_behaviour);
            _behaviour.StartFadeIn();
            SetDirty();
        }
        /// <summary>
        /// Removes a behaviour from the value controller.
        /// </summary>
        /// <remarks>
        /// <para>The behaviour will start fading out when removed, instead of disappearing instantly.</para>
        /// <para>
        /// If the behaviour is added again while fading out, it will fade back in smoothly
        /// from its current fade-out progress.
        /// </para>
        /// </remarks>
        public void RemoveBehaviour(_AValueBehaviour<T_VALUE> _behaviour)
        {
            if (_behaviour == null)
            {
                Console.LogWarning(SystemNames.ValueController, _m_name, "Remove behaviour failed, behaviour is null");
                return;
            }

            if (!_behaviour.IsBehaviourAdded(this))
            {
                Console.LogWarning(SystemNames.ValueController, _m_name, "Remove behaviour failed, behaviour is not added");
                return;
            }
            
            _behaviour.StartFadeOut();
            if (!_behaviour.isDead)
                return;
            
            _behaviour.SetBehaviourRemoved();
            ValueBehaviourLayer layer = _GetOrCreateBehaviourLayer(_behaviour.priority);
            layer.RemoveBehaviour(_behaviour);
            if (layer.IsEmpty())
                _m_behaviourLayers.RemoveSorted(layer);
            SetDirty();
        }
        /// <summary>
        /// Immediately removes a behaviour from the value controller.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Unlike <see cref="RemoveBehaviour"/>, this skips the fade-out process
        /// and removes the behaviour right away.
        /// </para>
        /// <para>
        /// Use this when you want something gone *now*, no questions asked —
        /// like emergency cleanup or when you're resetting the system.
        /// </para>
        /// </remarks>
        public void ForceRemoveBehaviour(_AValueBehaviour<T_VALUE> _behaviour)
        {
            if (_behaviour == null)
            {
                Console.LogWarning(SystemNames.ValueController, _m_name, "Force remove behaviour failed, behaviour is null");
                return;
            }
            
            if (!_behaviour.IsBehaviourAdded(this))
            {
                Console.LogWarning(SystemNames.ValueController, _m_name, "Force remove behaviour failed, behaviour is not added");
                return;
            }

            _behaviour.SetBehaviourRemoved();
            ValueBehaviourLayer layer = _GetOrCreateBehaviourLayer(_behaviour.priority);
            layer.RemoveBehaviour(_behaviour);
            if (layer.IsEmpty())
                _m_behaviourLayers.RemoveSorted(layer);
            SetDirty();
        }
        /// <summary>
        /// Adds an offset to the value controller.
        /// </summary>
        /// <remarks>
        /// <para>Offsets are simple values added on top of the current result.</para>
        /// <para>This is useful for temporary boosts or external modifiers.</para>
        /// </remarks>
        public void AddOffset(_AValueOffset<T_VALUE> _offset)
        {
            if (_offset == null)
            {
                Console.LogWarning(SystemNames.ValueController, _m_name, "Add offset failed, offset is null");
                return;
            }
            
            if (_offset.IsOffsetAdded())
            {
                Console.LogWarning(SystemNames.ValueController, _m_name, "Add offset failed, offset is already added");
                return;
            }
            
            _offset.SetOffsetAdded(this);
            _m_offsetList.Add(_offset);
            SetDirty();
        }
        /// <summary>
        /// Removes an offset from the value controller.
        /// </summary>
        /// <remarks>
        /// <para>If the offset was not previously added, nothing happens.</para>
        /// </remarks>
        public void RemoveOffset(_AValueOffset<T_VALUE> _offset)
        {
            if (_offset == null)
            {
                Console.LogWarning(SystemNames.ValueController, _m_name, "Remove offset failed, offset is null");
                return;
            }
            
            if (!_offset.IsOffsetAdded(this))
            {
                Console.LogWarning(SystemNames.ValueController, _m_name, "Remove offset failed, offset is not added");
                return;
            }
            
            _offset.SetOffsetRemoved();
            _m_offsetList.Remove(_offset);
            SetDirty();
        }
        /// <summary>
        /// Adds a constraint to the value controller.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Constraints are applied in the order they’re added,
        /// and each one limits or modifies the final value.
        /// </para>
        /// <para>
        /// Be careful with the order — constraints are not re-evaluated after each one,
        /// so the sequence matters.
        /// </para>
        /// </remarks>
        public void AddConstraint(_AValueConstraint<T_VALUE> _constraint)
        {
            if (_constraint == null)
            {
                Console.LogWarning(SystemNames.ValueController, _m_name, "Add constraint failed, constraint is null");
                return;
            }
            
            if (_constraint.IsConstraintAdded())
            {
                Console.LogWarning(SystemNames.ValueController, _m_name, "Add constraint failed, constraint is already added");
                return;
            }
            
            _constraint.SetConstraintAdded(this);
            _m_constraintList.Add(_constraint);
            SetDirty();
        }
        /// <summary>
        /// Removes a constraint from the value controller.
        /// </summary>
        /// <remarks>
        /// <para>If the constraint was not previously added, nothing happens.</para>
        /// </remarks>
        public void RemoveConstraint(_AValueConstraint<T_VALUE> _constraint)
        {
            if (_constraint == null)
            {
                Console.LogWarning(SystemNames.ValueController, _m_name, "Remove constraint failed, constraint is null");
                return;
            }
            
            if (!_constraint.IsConstraintAdded(this))
            {
                Console.LogWarning(SystemNames.ValueController, _m_name, "Remove constraint failed, constraint is not added");
                return;
            }
            
            _constraint.SetConstraintRemoved();
            _m_constraintList.Remove(_constraint);
            SetDirty();
        }
        /// <summary>
        /// Updates the value controller.
        /// </summary>
        public void Update(float _deltaTime)
        {
            _m_layerListForTraversal.Clear();
            _m_offsetListForTraversal.Clear();
            _m_constraintListForTraversal.Clear();
            
            _m_layerListForTraversal.AddRange(_m_behaviourLayers);
            _m_offsetListForTraversal.AddRange(_m_offsetList);
            _m_constraintListForTraversal.AddRange(_m_constraintList);

            foreach (ValueBehaviourLayer layer in _m_layerListForTraversal)
                layer.Update(_deltaTime);
            
            foreach (_AValueOffset<T_VALUE> offset in _m_offsetListForTraversal)
            {
                offset.InternalUpdate(_deltaTime);
                if (offset.isFinished)
                    RemoveOffset(offset);
            }
            
            foreach (_AValueConstraint<T_VALUE> constraint in _m_constraintListForTraversal)
                constraint.InternalUpdate(_deltaTime);
            
            SetDirty();
        }
        /// <summary>
        /// Marks the value controller as dirty.
        /// </summary>
        /// <remarks>
        /// <para>Use this to indicate that the value needs to be re-evaluated.</para>
        /// </remarks>
        public void SetDirty()
        {
            _m_isDirty = true;
        }
        
        
        /// <summary>
        /// Adds two values together.
        /// </summary>
        protected abstract T_VALUE Add(T_VALUE _value1, T_VALUE _value2);
        /// <summary>
        /// Interpolates between two values based on a given factor.
        /// </summary>
        protected abstract T_VALUE Lerp(T_VALUE _value1, T_VALUE _value2, float _t);
        
        
        [NotNull]
        private ValueBehaviourLayer _GetOrCreateBehaviourLayer(int _priority)
        {
            ValueBehaviourLayer layer = _m_behaviourLayers.Find(_layer => _layer.priority == _priority);
            if (layer != null)
                return layer;
            
            layer = new ValueBehaviourLayer(this, _priority);
            _m_behaviourLayers.InsertSorted(layer);
            return layer;
        }
        private T_VALUE _Evaluate()
        {
            T_VALUE behaviourValue = _EvaluateBehaviours();
            T_VALUE offsetValue = _EvaluateOffset();
            T_VALUE finalValue = _EvaluateConstraint(Add(behaviourValue, offsetValue));
            return finalValue;
        }
        private T_VALUE _EvaluateBehaviours()
        {
            T_VALUE behaviourValue = _m_baseValue;
            foreach (ValueBehaviourLayer layer in _m_behaviourLayers)
            {
                if (layer.IsEmpty())
                    continue;
                
                behaviourValue = Lerp(behaviourValue, layer.Evaluate(), layer.blendFactor);
            }
            
            return behaviourValue;
        }
        private T_VALUE _EvaluateOffset()
        {
            T_VALUE offsetValue = default;
            foreach (_AValueOffset<T_VALUE> offset in _m_offsetList)
                offsetValue = Add(offsetValue, offset.InternalEvaluate());

            return offsetValue;
        }
        private T_VALUE _EvaluateConstraint(T_VALUE _value)
        {
            T_VALUE clampedValue = _value;
            foreach (_AValueConstraint<T_VALUE> constraint in _m_constraintList)
                clampedValue = constraint.InternalEvaluate(clampedValue);

            return clampedValue;
        }
    }
}