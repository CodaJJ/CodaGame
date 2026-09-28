// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using JetBrains.Annotations;

namespace CodaGame
{
    /// <summary>
    /// Base class for all Attribute (pure data) modules on an Actor.
    /// Type-unique per Actor. Lifecycle: OnInit (Actor Awake) -> OnResetFrameValues each LogicTick -> OnDiscard (Actor OnDestroy).
    /// Attribute callbacks must not disable or destroy Actors; lifetime changes belong in Capability.OnLogicTick.
    /// </summary>
    public abstract class _AAttribute
    {
        [NotNull] private readonly _AActor _m_owner;
        

        protected _AAttribute([NotNull] _AActor _owner)
        {
            _m_owner = _owner;
        }
        

        [NotNull] public _AActor owner { get { return _m_owner; } }
        

        protected internal virtual void OnInit() { }

        /// <summary>
        /// Resets only this attribute's frame-local contributions to their neutral values.
        /// Called after show snapshots and before capability activation resolution each LogicTick.
        /// Preserve persistent state (velocity, contacts, profiles, health, etc.). Do not read or
        /// mutate other attributes or trigger capabilities: reset order is not a dependency contract.
        /// </summary>
        protected internal virtual void OnResetFrameValues() { }

        protected internal virtual void OnDiscard() { }
    }
}
