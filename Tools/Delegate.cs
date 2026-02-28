// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using JetBrains.Annotations;

namespace CodaGame
{
    public delegate void AsyncFunction(Action _complete);
    public delegate void AsyncFunction<in T_ARG1>(T_ARG1 _arg1, Action _complete);
    public delegate void NotNullAction<in T_ARG1>([NotNull] T_ARG1 _arg1);
    public delegate bool NotNullPredicate<in T_ARG>([NotNull] T_ARG _arg);
    public delegate T_RESULT NotNullFunc<in T_ARG, out T_RESULT>([NotNull] T_ARG _arg);
    public delegate void CompleteCallback(bool _success);
    public delegate string AssetAddressResolver(int _assetIndex);
}