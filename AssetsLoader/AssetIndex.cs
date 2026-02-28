// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// Identifies an asset by group and asset index.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each group must register a resolver via <see cref="RegisterGroup"/> before any asset in
    /// that group can be loaded. The resolver converts an asset index (int) to an Addressable key (string).
    /// </para>
    /// <para>
    /// Typical setup in game initialization:
    /// <code>
    /// AssetIndex.RegisterGroup(MyGroup.Id, i => $"Characters/{(CharAsset)i}");
    /// </code>
    /// </para>
    /// </remarks>
    [Serializable]
    public struct AssetIndex : IEquatable<AssetIndex>
    {
        /// <summary>
        /// Represents an invalid asset index. Returned when a lookup fails.
        /// </summary>
        public static readonly AssetIndex Invalid = new AssetIndex(-1, 0);
        
        
        [NotNull] private static readonly Dictionary<int, AssetAddressResolver> _g_groupResolvers = new Dictionary<int, AssetAddressResolver>();
        
        
        /// <summary>
        /// Registers a resolver for a group.
        /// The resolver converts an asset index to an Addressable key.
        /// </summary>
        public static void RegisterGroup(int _groupId, AssetAddressResolver _resolver)
        {
            if (_resolver == null)
            {
                Console.LogWarning(SystemNames.Assets, "Resolver cannot be null");
                return;
            }
            if (_groupId < 0)
            {
                Console.LogWarning(SystemNames.Assets, "Group ID cannot be negative");
                return;
            }
            if (!_g_groupResolvers.TryAdd(_groupId, _resolver))
            {
                Console.LogWarning(SystemNames.Assets, $"Group '{_groupId}' is already registered");
                return;
            }
            Console.LogSystem(SystemNames.Assets, $"Group '{_groupId}' registered");
        }
        
        
        [SerializeField] private int _m_groupIndex;
        [SerializeField] private int _m_assetIndex;


        public AssetIndex(int _groupIndex, int _assetIndex)
        {
            _m_groupIndex = _groupIndex;
            _m_assetIndex = _assetIndex;
        }


        /// <summary>
        /// Gets the group index.
        /// </summary>
        public int groupIndex { get { return _m_groupIndex; } }
        /// <summary>
        /// Gets the asset index within the group.
        /// </summary>
        public int assetIndex { get { return _m_assetIndex; } }
        /// <summary>
        /// Gets a unique identifier combining group and asset index.
        /// </summary>
        public long uniqueIndex { get { return ((long)_m_groupIndex << 32) | (uint)_m_assetIndex; } }
        /// <summary>
        /// Returns true if this index is valid (groupIndex >= 0).
        /// </summary>
        public bool isValid { get { return _m_groupIndex >= 0; } }


        /// <summary>
        /// Converts this index to an Addressable key using the registered group resolver.
        /// Returns null if the group has not been registered.
        /// </summary>
        public string ToAddressableKey()
        {
            if (!_g_groupResolvers.TryGetValue(_m_groupIndex, out AssetAddressResolver resolver))
            {
                Console.LogWarning(SystemNames.Assets, $"Asset group '{_m_groupIndex}' has no registered resolver");
                return null;
            }
            return resolver.Invoke(_m_assetIndex);
        }


        public bool Equals(AssetIndex _other) => _m_groupIndex == _other._m_groupIndex && _m_assetIndex == _other._m_assetIndex;
        public override bool Equals(object _obj) => _obj is AssetIndex other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(_m_groupIndex, _m_assetIndex);
        public static bool operator ==(AssetIndex _a, AssetIndex _b) => _a.Equals(_b);
        public static bool operator !=(AssetIndex _a, AssetIndex _b) => !_a.Equals(_b);
    }
}