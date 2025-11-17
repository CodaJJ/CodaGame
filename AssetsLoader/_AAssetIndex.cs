// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;

namespace CodaGame
{
    /// <summary>
    /// The base class for asset index representation.
    /// </summary>
    /// <remarks>
    /// <para>This class encapsulates the concept of an asset index, which is defined by a group index and an asset index within that group.</para>
    /// </remarks>
    [Serializable]
    public abstract class _AAssetIndex : IEquatable<_AAssetIndex>
    {
        // Private fields to store group and asset indices
        private readonly int _m_groupIndex;
        private readonly int _m_assetIndex;


        protected _AAssetIndex(int _groupIndex, int _assetIndex)
        {
            _m_groupIndex = _groupIndex;
            _m_assetIndex = _assetIndex;
        }


        /// <summary>
        /// Gets a unique identifier for the asset index by combining the group index and asset index.
        /// </summary>
        public long uniqueIndex { get { return ((long)_m_groupIndex << 32) | (uint)_m_assetIndex; } }
        /// <summary>
        /// Gets the group index of the asset.
        /// </summary>
        public int groupIndex { get { return _m_groupIndex; } }
        /// <summary>
        /// Gets the asset index within the group.
        /// </summary>
        public int assetIndex { get { return _m_assetIndex; } }
        
        
        /// <summary>
        /// Converts the asset index to an addressable key.
        /// </summary>
        public abstract string ToAddressableKey();
        
        
        public bool Equals(_AAssetIndex _other)
        {
            if (_other is null) return false;
            if (ReferenceEquals(this, _other)) return true;
            return _m_groupIndex == _other._m_groupIndex && _m_assetIndex == _other._m_assetIndex;
        }
        public override bool Equals(object _obj)
        {
            if (_obj is null) return false;
            if (ReferenceEquals(this, _obj)) return true;
            if (_obj.GetType() != GetType()) return false;
            return Equals((_AAssetIndex)_obj);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(_m_groupIndex, _m_assetIndex);
        }
    }
}