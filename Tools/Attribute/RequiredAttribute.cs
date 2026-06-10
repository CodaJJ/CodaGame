// Copyright (c) 2025 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System.Diagnostics;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// Marks a serialized reference field as mandatory, showing an error in the Inspector
    /// when the reference is not wired.
    /// </summary>
    /// <remarks>
    /// Code reading a [Required] field is expected to use it without null checks — a missing
    /// reference is a setup error, surfaced by the Inspector error box (and by the resulting
    /// NullReferenceException at runtime). Only object reference fields are checked; the
    /// attribute has no effect on value-typed fields.
    /// </remarks>
    [Conditional("UNITY_EDITOR")]
    public class RequiredAttribute : PropertyAttribute
    {
        private readonly string _m_message;


        public RequiredAttribute(string _message = null)
        {
            _m_message = _message;
        }


        public string message { get { return _m_message; } }
    }
}
