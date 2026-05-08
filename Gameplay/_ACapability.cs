// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using JetBrains.Annotations;

namespace CodaGame
{
    /// <summary>
    /// Base class for Capability (pure logic) modules on an Actor.
    /// Type-unique per Actor. `ownedTags` / `blockTags` are immutable (set at construction).
    /// Lifecycle: OnInit -> N * (ShouldActivate true -> OnActivate -> N * OnLogicTick/OnShowTick -> OnDeactivate) -> OnDiscard.
    /// Framework auto-pushes blockTags into owner's blocked set on Activate and pops on Deactivate.
    /// </summary>
    public abstract class _ACapability
    {
        [NotNull] private readonly _AActor _m_owner;
        private readonly int _m_priority;
        private readonly ReadOnlyList<int> _m_ownedTags;
        private readonly ReadOnlyList<int> _m_blockTags;
        private bool _m_isActive;
        

        protected _ACapability([NotNull] _AActor _owner, int _priority, ReadOnlyList<int> _ownedTags, ReadOnlyList<int> _blockTags)
        {
            _m_owner = _owner;
            _m_priority = _priority;
            _m_ownedTags = _ownedTags;
            _m_blockTags = _blockTags;
        }
        

        [NotNull] public _AActor owner { get { return _m_owner; } }
        public int priority { get { return _m_priority; } }
        public ReadOnlyList<int> ownedTags { get { return _m_ownedTags; } }
        public ReadOnlyList<int> blockTags { get { return _m_blockTags; } }
        public bool isActive { get { return _m_isActive; } }
        

        internal void Activate()
        {
            _m_owner.PushBlockTags(_m_blockTags);
            _m_isActive = true;
            OnActivate();
        }
        internal void Deactivate()
        {
            OnDeactivate();
            _m_isActive = false;
            _m_owner.PopBlockTags(_m_blockTags);
        }


        protected internal virtual void OnInit() { }
        protected internal virtual void OnDiscard() { }

        /// <summary>Must read only Attributes (never tags). Called every LogicTick while the Capability is registered.</summary>
        protected internal abstract bool ShouldActivate();

        protected virtual void OnActivate() { }
        protected virtual void OnDeactivate() { }
        protected internal virtual void OnLogicTick() { }
        protected internal virtual void OnShowTick(float _alpha) { }
    }
}
