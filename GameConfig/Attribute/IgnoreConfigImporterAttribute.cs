// Copyright (c) 2025 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;

namespace CodaGame
{
    /// <summary>
    /// Mark a config class with this attribute to exclude it from the config importer.
    /// Useful for test configs, deprecated configs, or base classes that shouldn't be imported.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class IgnoreConfigImporterAttribute : Attribute
    {
    }
}
