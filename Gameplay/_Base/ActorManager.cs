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
        // Only registration is deferred during a tick. Unregister removes from both lists immediately.
        [ItemNotNull, NotNull] private readonly List<_AActor> _m_pendingActors = new List<_AActor>();
        private int _m_nextActorIndex;
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
                _m_pendingActors.Add(_actor);
            else
                InsertActorSorted(_actor);
        }
        internal void Unregister([NotNull] _AActor _actor)
        {
            if (_m_isShowTicking)
                Console.LogError(SystemNames.Gameplay, $"Actor.Unregister fired during ShowTick — actor destroy logic belongs in LogicTick. Actor: {_actor.name}");

            // The Actor may have been enabled and disabled before its registration was flushed.
            _m_pendingActors.Remove(_actor);

            int index = _m_actors.IndexOf(_actor);
            if (index < 0)
                return;

            _m_actors.RemoveAt(index);
            // The cursor advances before the callback, so removing the current or an earlier
            // Actor shifts the next Actor left. Removing a later Actor needs no correction.
            if ((_m_isLogicTicking || _m_isShowTicking) && index < _m_nextActorIndex)
                --_m_nextActorIndex;
        }
        internal void LogicTick()
        {
            _m_isLogicTicking = true;
            _m_nextActorIndex = 0;
            try
            {
                while (_m_nextActorIndex < _m_actors.Count)
                    _m_actors[_m_nextActorIndex++].LogicTick();
            }
            finally
            {
                _m_isLogicTicking = false;
                _m_nextActorIndex = 0;
                // Commit new registrations even if a callback throws; they run on the next dispatch.
                FlushPendingActors();
            }
        }
        internal void ShowTick(float _alpha)
        {
            _m_isShowTicking = true;
            _m_nextActorIndex = 0;
            try
            {
                while (_m_nextActorIndex < _m_actors.Count)
                    _m_actors[_m_nextActorIndex++].ShowTick(_alpha);
            }
            finally
            {
                _m_isShowTicking = false;
                _m_nextActorIndex = 0;
                FlushPendingActors();
            }
        }
        

        private void InsertActorSorted([NotNull] _AActor _actor)
        {
            if (_actor == null || !_actor.isActiveAndEnabled)
                return;
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
        private void FlushPendingActors()
        {
            foreach (_AActor actor in _m_pendingActors)
                InsertActorSorted(actor);
            _m_pendingActors.Clear();
        }
    }
}
