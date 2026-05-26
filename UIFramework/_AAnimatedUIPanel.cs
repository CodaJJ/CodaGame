// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using JetBrains.Annotations;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// Convenience base class for UI panels whose show/hide transitions are driven by Legacy
    /// <see cref="Animation"/> clips.
    /// </summary>
    /// <remarks>
    /// <para>Inherit from this instead of <see cref="_AUIPanel"/> when the show/hide effect is a named
    /// animation clip. Assign the <see cref="Animation"/> component and the show/hide clip names in the
    /// Inspector; the default <c>OnShow</c>/<c>OnHide</c> implementations will play them and fire the
    /// completion callback when the clip's nominal duration elapses.</para>
    /// <para>If the Animation component is unassigned, a clip name is empty, or the named clip is not
    /// registered on the component, the completion callback is invoked immediately with no log — leave a
    /// clip name blank to opt that phase out of any animation.</para>
    /// </remarks>
    public abstract class _AAnimatedUIPanel : _AUIPanel
    {
        [SerializeField] private Animation _m_animation;
        [SerializeField] private string _m_showClipName;
        [SerializeField] private string _m_hideClipName;


        protected override void OnShow([NotNull] Action _complete)
        {
            PlayClip(_m_showClipName, _complete);
        }
        protected override void OnHide([NotNull] Action _complete)
        {
            PlayClip(_m_hideClipName, _complete);
        }


        private void PlayClip(string _clipName, [NotNull] Action _complete)
        {
            if (_m_animation == null || string.IsNullOrEmpty(_clipName) || _m_animation[_clipName] == null)
            {
                _complete.Invoke();
                return;
            }

            _m_animation.Play(_clipName, _complete);
        }
    }
}
