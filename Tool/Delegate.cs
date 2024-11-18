
using System;
using JetBrains.Annotations;

namespace UnityGameFramework
{
    public delegate void AsyncFunction(Action _complete);
    public delegate void AsyncFunction<in T_1>(T_1 _arg1, Action _complete);
    public delegate void NotNullAction<in T_1>([NotNull] T_1 _arg1);
}