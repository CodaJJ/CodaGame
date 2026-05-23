// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// Inspector-friendly enum→GameObjects mapping. Each enum value owns a list of GameObjects; calling
    /// <see cref="SetState"/> activates the entry matching the target value and deactivates everything
    /// else. Use as a <c>[SerializeField]</c> inside any panel/widget that needs branched visual states
    /// (e.g. save-slot "Empty / Normal / Corrupted" branches, button "Enabled / Disabled / Highlighted",
    /// etc.).
    /// </summary>
    /// <remarks>
    /// <para>The companion property drawer (Editor only) auto-syncs the serialized list to the enum values
    /// in declaration order — the Inspector only shows one row per enum value with a GameObject list to
    /// drag into, no enum dropdown to pick from. Adding / removing / reordering enum values in source code
    /// is reflected automatically the next time the asset is selected.</para>
    /// <para>If an enum value has no entry (e.g. the list was just edited), it falls back to "hide all" for
    /// that state. A GameObject shared between a matching entry and a non-matching entry always ends up
    /// active — the matching entry wins (see <see cref="SetState"/> for the single-pass deferred-activation
    /// implementation).</para>
    /// </remarks>
    /// <typeparam name="T_STATE">The enum type whose values key the branches.</typeparam>
    [Serializable]
    public class MultiStateShow<T_STATE> where T_STATE : Enum
    {
        [SerializeField] private List<Entry> _m_entries;


        /// <summary>
        /// Activate the GameObjects whose entry state equals <paramref name="_state"/>; deactivate every
        /// other entry's GameObjects. Safe to call before <see cref="_m_entries"/> has been populated
        /// (no-op). GameObjects within an entry that are null are silently skipped (via SafeSetActive).
        /// </summary>
        /// <remarks>
        /// Single pass over the entry list with deferred activation. Non-matching entries' GameObjects are
        /// deactivated immediately; the matching entry's targets are remembered and activated once at the
        /// end. This guarantees a GameObject shared between matching and non-matching entries ends up
        /// active (matching wins), regardless of entry order. Assumes canonical form (at most one entry per
        /// enum value, as enforced by the companion property drawer) — if multiple entries share the same
        /// state value, only the last one's targets are activated.
        /// </remarks>
        public void SetState(T_STATE _state)
        {
            if (_m_entries == null)
                return;

            EqualityComparer<T_STATE> cmp = EqualityComparer<T_STATE>.Default;
            List<GameObject> deferred = null;

            for (int i = 0; i < _m_entries.Count; i++)
            {
                Entry entry = _m_entries[i];
                List<GameObject> targets = entry.targets;
                if (targets == null)
                    continue;

                bool matching = cmp.Equals(entry.state, _state);
                if (matching)
                {
                    deferred = targets;
                    continue;
                }
                
                targets.SafeSetActive(false);
            }

            deferred?.SafeSetActive(true);
        }


        /// <summary>
        /// One enum value → its list of GameObjects. The companion property drawer enforces "one Entry per
        /// enum value in declaration order" and hides the <see cref="state"/> field from the Inspector;
        /// the field is still serialized so the runtime can match entries by state at <see cref="SetState"/>
        /// time (resilient to enum reordering between sessions).
        /// </summary>
        [Serializable]
        public struct Entry
        {
            public T_STATE state;
            public List<GameObject> targets;
        }
    }
}
