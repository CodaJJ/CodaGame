// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using JetBrains.Annotations;

namespace CodaGame
{
    public delegate void AsyncFunction(Action _complete);
    public delegate void AsyncFunction<in T_1>(T_1 _arg1, Action _complete);
    public delegate void NotNullAction<in T_1>([NotNull] T_1 _arg1);
    public delegate void CompleteCallback(bool _success);
}