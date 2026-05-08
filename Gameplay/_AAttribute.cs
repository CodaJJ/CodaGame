// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using JetBrains.Annotations;

namespace CodaGame
{
    /// <summary>
    /// Base class for all Attribute (pure data) modules on an Actor.
    /// Type-unique per Actor. Lifecycle: OnInit (Actor Awake or runtime add) -> OnDiscard (Actor OnDestroy or runtime remove).
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
        protected internal virtual void OnDiscard() { }
    }
}
