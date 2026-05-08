// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using JetBrains.Annotations;
using UnityEngine;

namespace CodaGame.Base
{
    /// <summary>
    /// 3D SFX playback anchored to a fixed world position.
    /// </summary>
    internal class PositionedPlayback : Playback
    {
        public PositionedPlayback([NotNull] _AAudioDefinition _definition, [NotNull] AudioSourcePool _sourcePool, AudioScope _scope, Vector3 _position)
            : base(_definition, _sourcePool, _scope)
        {
            if (source != null)
                source.transform.position = _position;
        }
    }
}
