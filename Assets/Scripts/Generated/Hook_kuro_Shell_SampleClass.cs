// This file was automatically generated by kuroneko.
// ReSharper disable InconsistentNaming
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using MonoHook;
using kuro.Core;

namespace kuro.ReflectionTool
{
    public partial struct kuro_Shell_SampleClass
    {
        public static class __hook__
        {

            [MonoHookAttribute(MonoHookType.Method, false)]
            public class InvokeDelegate
            {
                public static string __assembly__ = "Assembly-CSharp";
                /// <summary>
                /// <see cref="kuro.Shell+SampleClass"/>
                /// </summary>
                public static string __type__ = "kuro.Shell+SampleClass";
                public delegate int __delegate__(int value);

                public struct Param
                {
                    public int value;

                    public kuro_Shell_SampleClass __this__;
                    public int __result__;
                    public __delegate__ __original__;
                    public object __userdata__;
                }

                public delegate bool BeginDelegate(ref Param param);
                public delegate void EndDelegate(ref Param param);
                public static BeginDelegate OnBegin = null;
                public static EndDelegate OnEnd = null;

                [MethodImpl(MethodImplOptions.NoOptimization)]
                public int __replace__(int value)
                {
                    Param param = new();
                    param.value = value;
                    param.__this__ = new(this);
                    param.__result__ = default;
                    param.__original__ = __original_wrap_;

                    if (OnBegin?.Invoke(ref param) == true)
                    {
                        return param.__result__;
                    }

                    param.__result__ = __original_wrap_(param.value);

                    OnEnd?.Invoke(ref param);
                    return param.__result__;
                }

                [MethodImpl(MethodImplOptions.NoOptimization)]
                public int __original__(int value)
                {
                    return default;
                }

                [MethodImpl(MethodImplOptions.NoOptimization)]
                public int __original_wrap_(int value)
                {
                    return __original__(value);
                }
            }

            [MonoHookAttribute(MonoHookType.Method, false)]
            public class set_Value
            {
                public static string __assembly__ = "Assembly-CSharp";
                /// <summary>
                /// <see cref="kuro.Shell+SampleClass"/>
                /// </summary>
                public static string __type__ = "kuro.Shell+SampleClass";
                public delegate void __delegate__(int value);

                public struct Param
                {
                    public int value;

                    public kuro_Shell_SampleClass __this__;
                    public __delegate__ __original__;
                    public object __userdata__;
                }

                public delegate bool BeginDelegate(ref Param param);
                public delegate void EndDelegate(ref Param param);
                public static BeginDelegate OnBegin = null;
                public static EndDelegate OnEnd = null;

                [MethodImpl(MethodImplOptions.NoOptimization)]
                public void __replace__(int value)
                {
                    Param param = new();
                    param.value = value;
                    param.__this__ = new(this);
                    param.__original__ = __original_wrap_;

                    if (OnBegin?.Invoke(ref param) == true)
                    {
                        return;
                    }

                    __original_wrap_(param.value);

                    OnEnd?.Invoke(ref param);
                }

                [MethodImpl(MethodImplOptions.NoOptimization)]
                public void __original__(int value)
                {
                }

                [MethodImpl(MethodImplOptions.NoOptimization)]
                public void __original_wrap_(int value)
                {
                    __original__(value);
                }
            }

            [MonoHookAttribute(MonoHookType.Constructor, false)]
            public class _ctor
            {
                public static string __assembly__ = "Assembly-CSharp";
                /// <summary>
                /// <see cref="kuro.Shell+SampleClass"/>
                /// </summary>
                public static string __type__ = "kuro.Shell+SampleClass";
                public delegate void __delegate__(int value);

                public struct Param
                {
                    public int value;

                    public kuro_Shell_SampleClass __this__;
                    public __delegate__ __original__;
                    public object __userdata__;
                }

                public delegate bool BeginDelegate(ref Param param);
                public delegate void EndDelegate(ref Param param);
                public static BeginDelegate OnBegin = null;
                public static EndDelegate OnEnd = null;

                [MethodImpl(MethodImplOptions.NoOptimization)]
                public void __replace__(int value)
                {
                    Param param = new();
                    param.value = value;
                    param.__this__ = new(this);
                    param.__original__ = __original_wrap_;

                    if (OnBegin?.Invoke(ref param) == true)
                    {
                        return;
                    }

                    __original_wrap_(param.value);

                    OnEnd?.Invoke(ref param);
                }

                [MethodImpl(MethodImplOptions.NoOptimization)]
                public void __original__(int value)
                {
                }

                [MethodImpl(MethodImplOptions.NoOptimization)]
                public void __original_wrap_(int value)
                {
                    __original__(value);
                }
            }
        }
    }
}
