// Copyright (c) 2025 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.IO;
using JetBrains.Annotations;
using UnityEngine;

namespace CodaGame
{
    [Serializable]
    public struct AssetPath : IEquatable<AssetPath>
    {
        [SerializeField] private string _m_unityPath;


        public AssetPath(string _unityPath)
        {
            _m_unityPath = NormalizePath(_unityPath);
        }


        public string unityPath
        {
            get { return _m_unityPath; }
            set { _m_unityPath = NormalizePath(value); }
        }
        [NotNull] public string absolutePath
        {
            get
            {
                string path;
                if (string.IsNullOrEmpty(_m_unityPath))
                    path = Directory.GetCurrentDirectory();
                else if (Path.IsPathRooted(_m_unityPath))
                    path = _m_unityPath;
                else 
                    path = Path.Combine(Directory.GetCurrentDirectory(), _m_unityPath);
                
                path = path.Replace('\\', '/');
                while (path.Contains("//"))
                    path = path.Replace("//", "/");
                return path;
            }
        }


        public bool Equals(AssetPath _other)
        {
            return string.Equals(_m_unityPath, _other._m_unityPath, StringComparison.OrdinalIgnoreCase);
        }
        public override bool Equals(object _obj)
        {
            return _obj is AssetPath other && Equals(other);
        }
        public override int GetHashCode()
        {
            return _m_unityPath != null
                ? StringComparer.OrdinalIgnoreCase.GetHashCode(_m_unityPath)
                : 0;
        }
        public override string ToString()
        {
            return _m_unityPath ?? string.Empty;
        }


        public static implicit operator string(AssetPath _assetPath)
        {
            return _assetPath._m_unityPath;
        }
        public static implicit operator AssetPath(string _unityPath)
        {
            return new AssetPath(_unityPath);
        }
        public static bool operator ==(AssetPath _left, AssetPath _right)
        {
            return _left.Equals(_right);
        }
        public static bool operator !=(AssetPath _left, AssetPath _right)
        {
            return !_left.Equals(_right);
        }


        private static string NormalizePath(string _path)
        {
            if (string.IsNullOrEmpty(_path))
                return string.Empty;

            try
            {
                // Convert to absolute path if not already
                if (!Path.IsPathRooted(_path))
                    _path = Path.GetFullPath(_path);
                // Convert to relative path (relative to project root)
                _path = Path.GetRelativePath(Directory.GetCurrentDirectory(), _path);
                // Normalize slashes to forward slashes (Unity standard)
                _path = _path.Replace('\\', '/');
                // Remove duplicate slashes
                while (_path.Contains("//"))
                    _path = _path.Replace("//", "/");
                // Remove trailing slash (except for single-character paths)
                if (_path.EndsWith("/") && _path.Length > 1)
                    _path = _path.TrimEnd('/');

                return _path;
            }
            catch (Exception e)
            {
                Console.LogError(SystemNames.System, $"Invalid path: {_path}. Error: {e.Message}");
                return string.Empty;
            }
        }
    }
}