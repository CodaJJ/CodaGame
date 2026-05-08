// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using JetBrains.Annotations;
using UnityEngine;

namespace CodaGame.Base
{
    /// <summary>
    /// 3D SFX playback that follows a <see cref="Transform"/>. The source position is synced every
    /// LateUpdate; if the follower is destroyed, the playback stops automatically.
    /// </summary>
    internal class FollowerPlayback : Playback
    {
        [NotNull] private readonly Transform _m_follower;
        private TaskHandle _m_followerSyncTask;


        public FollowerPlayback([NotNull] _AAudioDefinition _definition, [NotNull] AudioSourcePool _sourcePool, AudioScope _scope, [NotNull] Transform _follower)
            : base(_definition, _sourcePool, _scope)
        {
            _m_follower = _follower;
            if (source != null)
                source.transform.position = _m_follower.position;
        }


        protected override void OnPlayStarted()
        {
            _m_followerSyncTask = Task.RunContinuousActionTask(_dt =>
            {
                if (!isAlive) return;
                if (_m_follower == null)
                {
                    Stop(0f);
                    return;
                }
                source.transform.position = _m_follower.position;
            }, _duration: -1f, UpdateType.LateUpdate, useUnscaledTime);
        }
        protected override void OnRecycle()
        {
            _m_followerSyncTask.StopTask();
            _m_followerSyncTask = default;
        }
    }
}
