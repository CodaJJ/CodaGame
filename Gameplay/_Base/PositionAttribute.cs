// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using JetBrains.Annotations;
using UnityEngine;

namespace CodaGame.Base
{
    /// <summary>
    /// Built-in world-space position attribute. Logic code writes `current`; ShowTick lerps
    /// `last -> current` by alpha and assigns to `owner.transform.position`.
    /// Subclass and override OnShowSync to disable, e.g. when a Rigidbody drives the Transform.
    /// </summary>
    public class PositionAttribute : _AShowSyncAttribute
    {
        [NotNull] private readonly Transform _m_transform;
        private Vector3 _m_current;
        private Vector3 _m_last;


        public PositionAttribute([NotNull] _AActor _owner) : base(_owner)
        {
            _m_transform = _owner.transform;
            _m_current = _m_transform.position;
            _m_last = _m_current;
        }


        public virtual Vector3 current { get { return _m_current; } set { _m_current = value; } }
        public virtual Vector3 last { get { return _m_last; } }


        /// <summary>
        /// Sets both `current` and `last` to the given position; next ShowTick shows it instantly.
        /// </summary>
        public virtual void Snap(Vector3 _position)
        {
            _m_current = _position;
            CollapseShow();
        }


        protected internal override void OnCaptureLast()
        {
            _m_last = _m_current;
        }
        protected internal override void OnShowSync(float _alpha)
        {
            _m_transform.position = Vector3.Lerp(_m_last, _m_current, _alpha);
        }
    }
}
