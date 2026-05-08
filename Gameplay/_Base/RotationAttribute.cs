// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using JetBrains.Annotations;
using UnityEngine;

namespace CodaGame.Base
{
    /// <summary>
    /// Built-in world-space rotation attribute. Logic code writes `current`; ShowTick slerps
    /// `last -> current` by alpha and assigns to `owner.transform.rotation`.
    /// </summary>
    public class RotationAttribute : _AShowSyncAttribute
    {
        [NotNull] private readonly Transform _m_transform;
        private Quaternion _m_current;
        private Quaternion _m_last;


        public RotationAttribute([NotNull] _AActor _owner) : base(_owner)
        {
            _m_transform = _owner.transform;
            _m_current = _m_transform.rotation;
            _m_last = _m_current;
        }


        public virtual Quaternion current { get { return _m_current; } set { _m_current = value; } }
        public virtual Quaternion last { get { return _m_last; } }


        /// <summary>Sets both `current` and `last` to the given rotation; next ShowTick shows it instantly.</summary>
        public virtual void Snap(Quaternion _rotation)
        {
            _m_current = _rotation;
            CollapseShow();
        }


        protected internal override void OnCaptureLast() { _m_last = _m_current; }
        protected internal override void OnShowSync(float _alpha)
        {
            _m_transform.rotation = Quaternion.Slerp(_m_last, _m_current, _alpha);
        }
    }
}
