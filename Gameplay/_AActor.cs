// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using CodaGame.Base;

namespace CodaGame
{
    /// <summary>
    /// Base MonoBehaviour for all gameplay actors. Owns Attributes (pure data, type-unique)
    /// and Capabilities (pure logic, type-unique), plus a refcounted block-tag set.
    /// Registered with ActorManager while enabled. Transform is driven by built-in
    /// PositionAttribute / RotationAttribute / ScaleAttribute (CodaGame.Base) unless replaced.
    /// Common transform state is exposed directly via `position` / `rotation` / `scale` properties
    /// and the `Snap(...)` method; the underlying attributes are still retrievable via
    /// GetAttribute&lt;PositionAttribute&gt;() etc. when `last` or fancy access is needed.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class _AActor : MonoBehaviour
    {
        [SerializeField, RuntimeReadOnly]
        private int _m_priority = 0;

        [NotNull] private readonly Dictionary<Type, _AAttribute> _m_attributes = new Dictionary<Type, _AAttribute>();
        [ItemNotNull, NotNull] private readonly List<_AShowSyncAttribute> _m_showSyncAttrs = new List<_AShowSyncAttribute>();
        [ItemNotNull, NotNull] private readonly List<_ACapability> _m_capabilities = new List<_ACapability>();
        [NotNull] private readonly Dictionary<int, int> _m_blockTagRefCount = new Dictionary<int, int>();

        // Pending Capability add/remove ops (flushed at the start of LogicTick, processed in call order).
        [NotNull] private readonly List<PendingCapOp> _m_pendingCapOps = new List<PendingCapOp>();

        [NotNull] private PositionAttribute _m_positionAttr;
        [NotNull] private RotationAttribute _m_rotationAttr;
        [NotNull] private ScaleAttribute _m_scaleAttr;

        private bool _m_isRegistered;
        private bool _m_attributesLocked;


        public int priority { get { return _m_priority; } }

        /// <summary>
        /// World-space position. Setter writes the logic-frame `current`; smooth interpolation
        /// to the Transform happens automatically each ShowTick. Use Snap(position: ...) for instant moves.
        /// </summary>
        public Vector3 position { get { return _m_positionAttr.current; } set { _m_positionAttr.current = value; } }
        /// <summary>
        /// World-space rotation. Setter writes the logic-frame `current`; smooth slerp
        /// to the Transform happens automatically each ShowTick. Use Snap(rotation: ...) for instant changes.
        /// </summary>
        public Quaternion rotation { get { return _m_rotationAttr.current; } set { _m_rotationAttr.current = value; } }
        /// <summary>
        /// Local scale. Setter writes the logic-frame `current`; smooth interpolation
        /// to the Transform happens automatically each ShowTick. Use Snap(scale: ...) for instant changes.
        /// </summary>
        public Vector3 scale { get { return _m_scaleAttr.current; } set { _m_scaleAttr.current = value; } }
        
        
        /// <summary>
        /// Sets any combination of position/rotation/scale and collapses their show-layer
        /// interpolation in one call. Next ShowTick displays the new values instantly (no lerp).
        /// Common uses: teleport, respawn, cutscene cut.
        /// </summary>
        public void Snap(Vector3? _position = null, Quaternion? _rotation = null, Vector3? _scale = null)
        {
            if (_position.HasValue) _m_positionAttr.Snap(_position.Value);
            if (_rotation.HasValue) _m_rotationAttr.Snap(_rotation.Value);
            if (_scale.HasValue)    _m_scaleAttr.Snap(_scale.Value);
        }
        
        public T GetAttribute<T>() where T : _AAttribute
        {
            return _m_attributes.TryGetValue(typeof(T), out _AAttribute a) ? (T)a : null;
        }
        public bool TryGetAttribute<T>(out T _attr) where T : _AAttribute
        {
            if (_m_attributes.TryGetValue(typeof(T), out _AAttribute a))
            {
                _attr = (T)a;
                return true;
            }
            _attr = null;
            return false;
        }

        /// <summary>
        /// Queues a Capability for addition. Actually inserted at the start of the next LogicTick.
        /// Same-frame Add then Remove of the same instance cancels out as expected (call-order semantics).
        /// </summary>
        public void AddCapability(_ACapability _cap)
        {
            if (_cap == null)
            {
                Console.LogError(SystemNames.Gameplay, $"Cannot add null Capability to Actor {name}.");
                return;
            }
            _m_pendingCapOps.Add(new PendingCapOp(PendingCapOpType.Add, _cap));
        }
        /// <summary>
        /// Queues a Capability instance for removal. Actually removed at the start of the next LogicTick.
        /// </summary>
        public void RemoveCapability(_ACapability _cap)
        {
            if (_cap == null)
            {
                Console.LogError(SystemNames.Gameplay, $"Cannot remove null Capability from Actor {name}.");
                return;
            }
            _m_pendingCapOps.Add(new PendingCapOp(PendingCapOpType.Remove, _cap));
        }
        /// <summary>
        /// Queues removal of all Capabilities of type T currently visible to the actor — both
        /// the one in the flushed list (if any) and any pending Adds of T queued this frame.
        /// Each match gets its own Remove op appended; duplicate-Add error logs are preserved as
        /// learning signals.
        /// </summary>
        public void RemoveCapability<T>() where T : _ACapability
        {
            // Match every pending Add of T (so an Add+RemoveCapability<T> sequence cancels out).
            int pendingCount = _m_pendingCapOps.Count;   // snapshot before append
            for (int i = 0; i < pendingCount; ++i)
            {
                PendingCapOp op = _m_pendingCapOps[i];
                if (op is { type: PendingCapOpType.Add, cap: T })
                    _m_pendingCapOps.Add(new PendingCapOp(PendingCapOpType.Remove, op.cap));
            }

            // And the existing flushed instance, if any.
            foreach (_ACapability capability in _m_capabilities)
            {
                if (capability is T)
                {
                    _m_pendingCapOps.Add(new PendingCapOp(PendingCapOpType.Remove, capability));
                    return;
                }
            }
        }
        /// <summary>
        /// Returns the Capability of type T that will exist after the next flush — i.e. it accounts
        /// for pending Add/Remove ops queued this frame. Returns null if no T will be present.
        /// (Type-unique invariant guarantees at most one matching T.)
        /// </summary>
        public T GetCapability<T>() where T : _ACapability
        {
            // Start from flushed state.
            _ACapability current = null;
            foreach (_ACapability capability in _m_capabilities)
            {
                if (capability is T)
                {
                    current = capability;
                    break;
                }
            }

            // Apply pending ops in call order to converge on the post-flush logical state.
            for (int i = 0; i < _m_pendingCapOps.Count; ++i)
            {
                var op = _m_pendingCapOps[i];
                if (op.cap is not T)
                    continue;

                if (op.type == PendingCapOpType.Add)
                {
                    if (current == null)
                        current = op.cap;
                    // else: this pending Add will be rejected at flush (type-unique violation) — do not adopt.
                }
                else // Remove
                {
                    if (current == op.cap)
                        current = null;
                    // else: Remove of a cap not currently considered "the T" is a no-op here.
                }
            }
            return (T)current;
        }
        /// <summary>
        /// Tries to get the Capability of type T. Same post-flush semantics as GetCapability&lt;T&gt;.
        /// </summary>
        public bool TryGetCapability<T>(out T _cap) where T : _ACapability
        {
            _cap = GetCapability<T>();
            return _cap != null;
        }


        /// <summary>
        /// Subclasses may override to return a custom Position attribute.
        /// </summary>
        protected virtual PositionAttribute CreatePositionAttribute() { return new PositionAttribute(this); }
        /// <summary>
        /// Subclasses may override to return a custom Rotation attribute.
        /// </summary>
        protected virtual RotationAttribute CreateRotationAttribute() { return new RotationAttribute(this); }
        /// <summary>
        /// Subclasses may override to return a custom Scale attribute.
        /// </summary>
        protected virtual ScaleAttribute CreateScaleAttribute() { return new ScaleAttribute(this); }

        /// <summary>
        /// Register user Attributes here via AddAttribute&lt;T&gt;. Called once during Awake, after built-ins are added.
        /// </summary>
        protected virtual void OnInitAttributes() { }
        /// <summary>
        /// Register user Capabilities here via AddCapability. Called once during Awake, after OnInitAttributes.
        /// </summary>
        protected virtual void OnInitCapabilities() { }

        /// <summary>
        /// Registers a user Attribute on this Actor. Must be called from OnInitAttributes()
        /// (or from a built-in factory). Calling after Awake will Crush — Attributes are static
        /// for the lifetime of the Actor; Capability lookups via GetAttribute&lt;T&gt;() are cached
        /// at construction and won't see Attributes added later.
        /// </summary>
        protected T AddAttribute<T>(T _attr) where T : _AAttribute
        {
            if (_attr == null)
            {
                Console.LogError(SystemNames.Gameplay, $"Cannot add null Attribute to Actor {name}.");
                return null;
            }
            if (_m_attributesLocked)
            {
                Console.LogError(SystemNames.Gameplay, $"Cannot add Attribute {typeof(T).Name} after Awake. Register all Attributes in OnInitAttributes().");
                return _attr;
            }
            AddAttributeInternal(_attr);
            return _attr;
        }
        
        
        // ---------- Tick (called by ActorManager) ----------
        /// <summary>
        /// Per-actor LogicTick body: flush pending capability ops, snapshot show-sync `last`,
        /// resolve activations (priority desc), then run OnLogicTick on active capabilities.
        /// </summary>
        internal void LogicTick()
        {
            FlushPendingCapabilityOps();

            // Pass 1: snapshot show-sync `last` BEFORE any logic mutates `current`.
            foreach (_AShowSyncAttribute attr in _m_showSyncAttrs)
                attr.OnCaptureLast();

            // Pass 2: activation resolution in priority desc order.
            foreach (_ACapability cap in _m_capabilities)
            {
                bool wantsActive = cap.ShouldActivate();
                bool blocked = IsCapabilityBlocked(cap);
                bool shouldBeActive = wantsActive && !blocked;

                if (shouldBeActive && !cap.isActive)
                    cap.Activate();
                else if (!shouldBeActive && cap.isActive)
                    cap.Deactivate();
            }

            // Pass 3: OnLogicTick for active capabilities in priority desc order.
            foreach (_ACapability cap in _m_capabilities)
            {
                if (cap.isActive)
                    cap.OnLogicTick();
            }
        }
        /// <summary>
        /// Per-actor ShowTick body: run OnShowTick on active capabilities, then OnShowSync on
        /// show-sync attributes (interpolation to view layer).
        /// </summary>
        internal void ShowTick(float _alpha)
        {
            foreach (_ACapability cap in _m_capabilities)
            {
                if (cap.isActive)
                    cap.OnShowTick(_alpha);
            }
            foreach (_AShowSyncAttribute attr in _m_showSyncAttrs)
                attr.OnShowSync(_alpha);
        }
        
        internal void FlushPendingCapabilityOps()
        {
            if (_m_pendingCapOps.Count == 0)
                return;

            // Process in call order: Add(cap) followed by Remove(cap) cancels out (cap goes through
            // OnInit then OnDiscard); Remove(old) then Add(new) of same type swaps cleanly.
            foreach (PendingCapOp op in _m_pendingCapOps)
            {
                if (op.type == PendingCapOpType.Remove)
                    ApplyRemoveCapability(op.cap);
                else
                    ApplyAddCapability(op.cap);
            }
            _m_pendingCapOps.Clear();
        }
        internal bool IsBlocked(int _tag)
        {
            return _m_blockTagRefCount.TryGetValue(_tag, out int c) && c > 0;
        }
        internal void PushBlockTags(ReadOnlyList<int> _tags)
        {
            foreach (int t in _tags)
            {
                _m_blockTagRefCount.TryGetValue(t, out int c);
                _m_blockTagRefCount[t] = c + 1;
            }
        }
        internal void PopBlockTags(ReadOnlyList<int> _tags)
        {
            foreach (int t in _tags)
            {
                if (!_m_blockTagRefCount.TryGetValue(t, out int c) || c <= 0)
                    Console.LogCrush(SystemNames.Gameplay, $"PopBlockTags underflow: tag {t} on Actor {name}.");
                
                if (c == 1)
                    _m_blockTagRefCount.Remove(t);
                else
                    _m_blockTagRefCount[t] = c - 1;
            }
        }
        internal bool IsCapabilityBlocked([NotNull] _ACapability _cap)
        {
            ReadOnlyList<int> owned = _cap.ownedTags;
            foreach (int t in owned)
                if (IsBlocked(t))
                    return true;
            return false;
        }


        // ---------- Unity lifecycle ----------
        private void Awake()
        {
            // Built-in transform sync attributes.
            _m_positionAttr = CreatePositionAttribute() ?? new PositionAttribute(this);
            _m_rotationAttr = CreateRotationAttribute() ?? new RotationAttribute(this);
            _m_scaleAttr    = CreateScaleAttribute()    ?? new ScaleAttribute(this);
            AddAttributeInternal(_m_positionAttr);
            AddAttributeInternal(_m_rotationAttr);
            AddAttributeInternal(_m_scaleAttr);

            // User hooks. Attributes lock between the two so Capability constructors observe a
            // finalized Attribute set when they cache GetAttribute<T>() references.
            OnInitAttributes();
            _m_attributesLocked = true;
            OnInitCapabilities();
        }
        private void OnEnable()
        {
            if (!_m_isRegistered)
            {
                ActorManager.instance.Register(this);
                _m_isRegistered = true;
            }
        }
        private void OnDisable()
        {
            if (_m_isRegistered)
            {
                // Deactivate everything (framework-managed pop of block tags).
                DeactivateAllCapabilities();
                ActorManager.instance.Unregister(this);
                _m_isRegistered = false;
            }
        }
        private void OnDestroy()
        {
            // Safety: ensure unregistered (OnDisable may not have fired if destroyed while disabled).
            if (_m_isRegistered)
            {
                DeactivateAllCapabilities();
                ActorManager.instance.Unregister(this);
                _m_isRegistered = false;
            }

            // Discard capabilities then attributes in reverse registration order.
            for (int i = _m_capabilities.Count - 1; i >= 0; i--)
                _m_capabilities[i].OnDiscard();
            _m_capabilities.Clear();

            foreach (_AAttribute attr in _m_attributes.Values)
                attr?.OnDiscard();
            _m_attributes.Clear();
            _m_showSyncAttrs.Clear();
        }

        private void AddAttributeInternal([NotNull] _AAttribute _attr)
        {
            Type t = _attr.GetType();
            if (!_m_attributes.TryAdd(t, _attr))
            {
                Console.LogError(SystemNames.Gameplay, $"Duplicate Attribute of type {t.Name} on Actor {name}.");
                return;
            }

            if (_attr is _AShowSyncAttribute sync)
                _m_showSyncAttrs.Add(sync);
            _attr.OnInit();
        }

        private void ApplyAddCapability([NotNull] _ACapability _cap)
        {
            Type t = _cap.GetType();
            foreach (_ACapability capability in _m_capabilities)
            {
                if (capability.GetType() == t)
                {
                    Console.LogError(SystemNames.Gameplay, $"Duplicate Capability of type {t.Name} on Actor {name}.");
                    return;
                }
            }

            // Insert to keep priority desc; stable insertion among equal priorities.
            int insertAt = _m_capabilities.Count;
            for (int j = 0; j < _m_capabilities.Count; ++j)
            {
                if (_m_capabilities[j].priority < _cap.priority)
                {
                    insertAt = j;
                    break;
                }
            }
            _m_capabilities.Insert(insertAt, _cap);
            _cap.OnInit();
        }
        private void ApplyRemoveCapability([NotNull] _ACapability _cap)
        {
            int idx = _m_capabilities.IndexOf(_cap);
            if (idx < 0)
                return;   // already removed or never added

            if (_cap.isActive)
                _cap.Deactivate();
            _cap.OnDiscard();
            _m_capabilities.RemoveAt(idx);
        }
        private void DeactivateAllCapabilities()
        {
            // Iterate in reverse priority order (low priority first out). This only matters
            // for symmetry with the resolution order; behavior is identical either way since
            // we clear the block-tag refcount dict afterwards.
            for (int i = _m_capabilities.Count - 1; i >= 0; --i)
            {
                _ACapability cap = _m_capabilities[i];
                if (cap.isActive)
                    cap.Deactivate();
            }
        }


        private enum PendingCapOpType : byte
        {
            Add, Remove
        }
        private readonly struct PendingCapOp
        {
            public readonly PendingCapOpType type;
            [NotNull] public readonly _ACapability cap;
            public PendingCapOp(PendingCapOpType _type, [NotNull] _ACapability _cap)
            {
                type = _type;
                cap = _cap;
            }
        }
    }
}
