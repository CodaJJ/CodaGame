// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace CodaGame.Base
{
    public abstract partial class _AValueController<T_VALUE>
        where T_VALUE : struct
    {
        /// <summary>
        /// A layer that groups value behaviours with the same priority.
        /// </summary>
        /// <remarks>
        /// <para>This layer collects behaviours that share the same priority and blends their values together.</para>
        /// </remarks>
        private class ValueBehaviourLayer : IComparable<ValueBehaviourLayer>
        {
            // A reference to the controller that owns this layer.
            [NotNull] private readonly _AValueController<T_VALUE> _m_valueController;
            private readonly int _m_priority;
            
            [ItemNotNull, NotNull] private readonly List<_AValueBehaviour<T_VALUE>> _m_behaviours;
            // A temporary list used for traversal to avoid modifying the original list while iterating.
            [ItemNotNull, NotNull] private readonly List<_AValueBehaviour<T_VALUE>> _m_behavioursForTraversal;
            // Collection of behaviours' value and weight for blending.
            [NotNull] private readonly List<WeightedValue> _m_weightedValues;
            
            // The blend factor for this layer.
            // Blend factor when mixing this layer with lower-priority layers.
            // A value of 1 means this layer fully overrides any lower ones.
            private float _m_blendFactor;
            // Flag to indicate if the blend factor needs to be recalculated.
            private bool _m_blendFactorDirty;


            public ValueBehaviourLayer([NotNull] _AValueController<T_VALUE> _controller, int _priority)
            {
                _m_valueController = _controller;
                _m_behaviours = new List<_AValueBehaviour<T_VALUE>>();
                _m_behavioursForTraversal = new List<_AValueBehaviour<T_VALUE>>();
                _m_weightedValues = new List<WeightedValue>();

                _m_priority = _priority;
            }


            /// <summary>
            /// The priority of this layer.
            /// </summary>
            /// <remarks>
            /// <para>Higher priority layers override lower ones.</para>
            /// </remarks>
            public int priority { get { return _m_priority; } }
            /// <summary>
            /// The blend factor for this layer.
            /// </summary>
            /// <remarks>
            /// <para>
            /// Blend factor when mixing this layer with lower-priority layers.
            /// A value of 1 means this layer fully overrides any lower ones.
            /// </para>
            /// </remarks>
            public float blendFactor
            {
                get
                {
                    if (_m_blendFactorDirty)
                    {
                        _m_blendFactor = _UpdateBlendFactor();
                        _m_blendFactorDirty = false;
                    }
                    return _m_blendFactor;
                }
            }


            /// <summary>
            /// Evaluates the blended value of this layer.
            /// </summary>
            /// <remarks>
            /// <para>This method blends the values of all behaviours in this layer based on their weights.</para>
            /// </remarks>
            public T_VALUE Evaluate()
            {
                // Collect all behaviours' values and weights.
                _m_weightedValues.Clear();
                foreach (_AValueBehaviour<T_VALUE> behaviour in _m_behaviours)
                {
                    float finalWeight = behaviour.weight * behaviour.blendFactor;
                    if (finalWeight <= 0f)
                        continue;

                    T_VALUE value = behaviour.InternalEvaluate();
                    _m_weightedValues.Add(new WeightedValue(value, finalWeight));
                }
                
                if (_m_weightedValues.Count == 0)
                    return default;
                if (_m_weightedValues.Count == 1)
                    return _m_weightedValues[0].value;

                // Blend the values together.
                // This is a binary tree blending algorithm with a little optimization.
                int count = _m_weightedValues.Count;
                while (count > 1)
                {
                    int index = 0;
                    for (int i = 0; i < count; i += 2)
                    {
                        if (i + 1 >= count)
                            _m_weightedValues[index] = _m_weightedValues[i];
                        else
                        {
                            (T_VALUE v1, float w1) = _m_weightedValues[i];
                            (T_VALUE v2, float w2) = _m_weightedValues[i + 1];
                            float total = w1 + w2;
                            float t = w2 / total;

                            T_VALUE blended = _m_valueController.Lerp(v1, v2, t);
                            _m_weightedValues[index] = new WeightedValue(blended, total);
                        }

                        index++;
                    }
                    
                    count = index;
                }

                return _m_weightedValues[0].value;
            }
            /// <summary>
            /// Updates all behaviours in this layer.
            /// </summary>
            public void Update(float _deltaTime)
            {
                _m_behavioursForTraversal.Clear();
                _m_behavioursForTraversal.AddRange(_m_behaviours);
                foreach (_AValueBehaviour<T_VALUE> behaviour in _m_behavioursForTraversal)
                {
                    behaviour.InternalUpdate(_deltaTime);
                    if (behaviour.isDead)
                        _m_valueController.RemoveBehaviour(behaviour);
                }

                _SetBlendFactorDirty();
            }
            /// <summary>
            /// Adds a behaviour to this layer.
            /// </summary>
            /// <remarks>
            /// <para>There is no validation check, because the controller has already done it.</para>
            /// </remarks>
            public void AddBehaviour(_AValueBehaviour<T_VALUE> _behaviour)
            {
                _m_behaviours.Add(_behaviour);
                _SetBlendFactorDirty();
            }
            /// <summary>
            /// Removes a behaviour from this layer.
            /// </summary>
            /// <remarks>
            /// <para>There is no validation check, because the controller has already done it.</para>
            /// </remarks>
            public void RemoveBehaviour(_AValueBehaviour<T_VALUE> _behaviour)
            {
                _m_behaviours.Remove(_behaviour);
                _SetBlendFactorDirty();
            }
            /// <summary>
            /// Checks if this layer is empty.
            /// </summary>
            /// <remarks>
            /// <para>No behaviours in this layer.</para>
            /// </remarks>
            public bool IsEmpty()
            {
                return _m_behaviours.Count == 0;
            }

            /// <summary>
            /// Compares this layer with another layer based on their priorities.
            /// </summary>
            /// <remarks>
            /// <para>From lowest to highest priority.</para>
            /// </remarks>
            public int CompareTo(ValueBehaviourLayer _other)
            {
                if (_other == null)
                    return 1;

                return _m_priority.CompareTo(_other._m_priority);
            }
            
            
            private void _SetBlendFactorDirty()
            {
                _m_blendFactorDirty = true;
            }
            private float _UpdateBlendFactor()
            {
                float factor = 0f;
                foreach (_AValueBehaviour<T_VALUE> behaviour in _m_behaviours)
                {
                    if (behaviour.blendFactor > factor)
                        factor = behaviour.blendFactor;
                }
                
                return factor;
            }
        }
        
        
        private struct WeightedValue
        {
            public T_VALUE value;
            public float weight;
            

            public WeightedValue(T_VALUE _value, float _weight)
            {
                value = _value;
                weight = _weight;
            }
            

            public void Deconstruct(out T_VALUE _value, out float _weight)
            {
                _value = value;
                _weight = weight;
            }
        }
    }
}