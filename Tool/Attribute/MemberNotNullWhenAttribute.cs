// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

#if !UNITY_2023_1_OR_NEWER // todo: I'm not sure if this is supported in 2023.1 or newer
namespace System.Diagnostics.CodeAnalysis
{
    // A copy of the original attribute for code compatibility
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    internal sealed class MemberNotNullWhenAttribute : Attribute
    {
        public MemberNotNullWhenAttribute(bool returnValue, string member)
        {
            this.ReturnValue = returnValue;
            this.Members = new string[1]{ member };
        }

        public MemberNotNullWhenAttribute(bool returnValue, params string[] members)
        {
            this.ReturnValue = returnValue;
            this.Members = members;
        }

        public bool ReturnValue { get; }

        public string[] Members { get; }
    }
}
#endif