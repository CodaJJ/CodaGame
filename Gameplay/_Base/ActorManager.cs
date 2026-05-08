// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System.Collections.Generic;
using JetBrains.Annotations;

namespace CodaGame
{
    internal class ActorManager
    {
        private static ActorManager _g_instance;
        [NotNull] internal static ActorManager instance { get { return _g_instance ??= new ActorManager(); } }


        [ItemNotNull, NotNull] private readonly List<_AActor> _m_actors = new List<_AActor>();
        // Pending register/unregister ops queued during a tick; flushed at the end of LogicTick / ShowTick.
        // Processed in call order so that Register+Unregister of the same actor in one tick cancels out.
        [NotNull] private readonly List<PendingActorOp> _m_pendingActorOps = new List<PendingActorOp>();
        private bool _m_isLogicTicking;
        private bool _m_isShowTicking;


        private ActorManager()
        {
        }


        internal void Register([NotNull] _AActor _actor)
        {
            if (_m_isShowTicking)
                Console.LogError(SystemNames.Gameplay, $"Actor.Register fired during ShowTick — actor spawn logic belongs in LogicTick. Actor: {_actor.name}");

            if (_m_isLogicTicking || _m_isShowTicking)
                _m_pendingActorOps.Add(new PendingActorOp(PendingActorOpType.Register, _actor));
            else
                InsertActorSorted(_actor);
        }
        internal void Unregister([NotNull] _AActor _actor)
        {
            if (_m_isShowTicking)
                Console.LogError(SystemNames.Gameplay, $"Actor.Unregister fired during ShowTick — actor destroy logic belongs in LogicTick. Actor: {_actor.name}");

            if (_m_isLogicTicking || _m_isShowTicking)
                _m_pendingActorOps.Add(new PendingActorOp(PendingActorOpType.Unregister, _actor));
            else
                _m_actors.Remove(_actor);
        }
        internal void LogicTick()
        {
            _m_isLogicTicking = true;
            try
            {
                foreach (_AActor actor in _m_actors)
                    actor.LogicTick();

                // Flush any Register/Unregister queued during the tick so next tick sees them.
                FlushPendingActorOps();
            }
            finally
            {
                _m_isLogicTicking = false;
            }
        }
        internal void ShowTick(float _alpha)
        {
            _m_isShowTicking = true;
            try
            {
                foreach (_AActor actor in _m_actors)
                    actor.ShowTick(_alpha);

                // Drain anything queued during ShowTick callbacks (with error logged at queue time).
                // Effective on next LogicTick.
                FlushPendingActorOps();
            }
            finally
            {
                _m_isShowTicking = false;
            }
        }
        

        private void InsertActorSorted([NotNull] _AActor _actor)
        {
            int insertAt = _m_actors.Count;
            for (int i = 0; i < _m_actors.Count; ++i)
            {
                if (_m_actors[i].priority < _actor.priority)
                {
                    insertAt = i;
                    break;
                }
            }
            _m_actors.Insert(insertAt, _actor);
        }
        private void FlushPendingActorOps()
        {
            if (_m_pendingActorOps.Count == 0)
                return;

            for (int i = 0; i < _m_pendingActorOps.Count; ++i)
            {
                var op = _m_pendingActorOps[i];
                if (op.type == PendingActorOpType.Unregister)
                    _m_actors.Remove(op.actor);
                else
                    InsertActorSorted(op.actor);
            }
            _m_pendingActorOps.Clear();
        }
        private enum PendingActorOpType : byte { Register, Unregister }
        private readonly struct PendingActorOp
        {
            public readonly PendingActorOpType type;
            [NotNull] public readonly _AActor actor;
            public PendingActorOp(PendingActorOpType _type, [NotNull] _AActor _actor)
            {
                type = _type;
                actor = _actor;
            }
        }
    }
}
