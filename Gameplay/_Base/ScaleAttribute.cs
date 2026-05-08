// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using JetBrains.Annotations;
using UnityEngine;

namespace CodaGame.Base
{
    /// <summary>
    /// Built-in local-space scale attribute. Logic code writes `current`; ShowTick lerps
    /// `last -> current` by alpha and assigns to `owner.transform.localScale`.
    /// </summary>
    public class ScaleAttribute : _AShowSyncAttribute
    {
        [NotNull] private readonly Transform _m_transform;
        private Vector3 _m_current;
        private Vector3 _m_last;


        public ScaleAttribute([NotNull] _AActor _owner) : base(_owner)
        {
            _m_transform = _owner.transform;
            _m_current = _m_transform.localScale;
            _m_last = _m_current;
        }


        public virtual Vector3 current { get { return _m_current; } set { _m_current = value; } }
        public virtual Vector3 last { get { return _m_last; } }


        /// <summary>Sets both `current` and `last` to the given scale; next ShowTick shows it instantly.</summary>
        public virtual void Snap(Vector3 _scale)
        {
            _m_current = _scale;
            CollapseShow();
        }


        protected internal override void OnCaptureLast() { _m_last = _m_current; }
        protected internal override void OnShowSync(float _alpha)
        {
            _m_transform.localScale = Vector3.Lerp(_m_last, _m_current, _alpha);
        }
    }
}
