// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using JetBrains.Annotations;

namespace CodaGame
{
    /// <summary>
    /// Abstract base for Attributes that need to interpolate logic-frame data into the show layer.
    /// Framework calls OnCaptureLast at the start of each LogicTick (before any logic mutates `current`),
    /// then OnShowSync(alpha) every ShowTick to interpolate `last -> current` and apply to the show layer.
    /// Subclasses may also expose CollapseShow() — collapses `last` onto `current` so the next ShowTick
    /// shows no interpolation. Used for teleports, respawns, cuts, or any instant state change.
    /// Built-in concrete examples (in CodaGame.Base): PositionAttribute, RotationAttribute, ScaleAttribute.
    /// </summary>
    public abstract class _AShowSyncAttribute : _AAttribute
    {
        protected _AShowSyncAttribute([NotNull] _AActor _owner) : base(_owner) { }
        

        /// <summary>
        /// Forces `last` to equal `current` so the next ShowTick performs no interpolation.
        /// Default impl just calls OnCaptureLast(). Fancy subclasses (springs, history buffers,
        /// reconcile queues) can override to also clear additional interpolation state.
        /// </summary>
        public virtual void CollapseShow() { OnCaptureLast(); }


        protected internal abstract void OnCaptureLast();
        protected internal abstract void OnShowSync(float _alpha);
    }
}
