// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

#if !UNITY_2023_1_OR_NEWER // todo: I'm not sure if this is supported in 2023.1 or newer
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    internal sealed class MemberNotNullAttribute : Attribute
    {
        public MemberNotNullAttribute(string member) => this.Members = new string[1]
        {
            member
        };

        public MemberNotNullAttribute(params string[] members) => this.Members = members;

        public string[] Members { get; }
    }
}
#endif